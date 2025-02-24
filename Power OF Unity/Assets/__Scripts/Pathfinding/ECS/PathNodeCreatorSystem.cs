using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

/// <summary>
/// Система создания УЗЛОВ ПУТИ
/// </summary>
partial struct PathNodeCreatorSystem : ISystem
{
  
    private NativeArray<PathNode> pathNodeDefaultArray;
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponentData(state.SystemHandle, new PathNodeDefaultArray());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Выполниться 1 кадр когда включим PathfindingGridDate
        foreach (RefRO<PathfindingGridDate> pathfindingGridDate in SystemAPI.Query<RefRO<PathfindingGridDate>>())
        {
            // В цикле настроим все ячейки на возможность проходимости, будем стрелять лучом из каждой позиции и для начала Сделаем все узлы непроходимыми
            // и если 1ый луч попал в пол то сделаем проходимым а 2рым луч проверим на препядствие _obstaclesDoorMousePlaneCoverLayerMask,
            // если они есть установим эту ячейку опять не проходимой
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;// Для создания луча      

            CollisionFilter collisionMousePlaneFilter = new CollisionFilter
            {//https://discussions.unity.com/t/how-do-i-use-layermasks/481 -обхъъяснение битового сдвига
                BelongsTo = ~0u, // Луч будет принадлежать каждому слою поэтому инвертируем все "0" с помощью оператора побитового инвертирования (~) и преобразуемв в uint с помощбю "u"
                CollidesWith = 1u << 6,// Колизия будет с 6 - MousePlane слоем маски поэтому воспользуемся сдвигом битовой маски ЭТО ПОЛ (НУЖЕН РЕФАКТОРИНГ)
                GroupIndex = 0,
            };
            CollisionFilter collisionObstaclesCoverFilter = new CollisionFilter
            {
                BelongsTo = ~0u, 
                CollidesWith = 1u << 8 | 1u << 11,// Колизия будет с 8 - Obstacles и 11 - Cover слоем маски поэтому воспользуемся сдвигом битовой маски (НУЖЕН РЕФАКТОРИНГ)
                GroupIndex = 0,
            };

            pathNodeDefaultArray = new NativeArray<PathNode>(pathfindingGridDate.ValueRO.width * pathfindingGridDate.ValueRO.height * pathfindingGridDate.ValueRO.floorAmount, Allocator.Persistent);

            for (int x = 0; x < pathfindingGridDate.ValueRO.width; x++)
            {
                for (int z = 0; z < pathfindingGridDate.ValueRO.height; z++)
                {
                    for (int flooor = 0; flooor < pathfindingGridDate.ValueRO.floorAmount; flooor++)
                    {
                        PathNode pathNode = new PathNode();

                        pathNode.gridPosition = new int3(x, flooor, z);
                        pathNode.worldPosition = GetWorldPosition(pathNode.gridPosition, pathfindingGridDate.ValueRO.cellSize, pathfindingGridDate.ValueRO.anchorGrid, pathfindingGridDate.ValueRO.floorHeight);
                        pathNode.index = CalculateIndex(x, flooor, z, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                        pathNode.gCost = int.MaxValue;
                        pathNode.cameFromNodeIndex = -1; //Установим недопустимое значение

                        pathNode.isInAir = false; //По умолчанию узел НЕ в воздухе
                        pathNode.isWalkable = false; //По умолчанию сделаем НЕпроходимой

                        //Выстрелим ЛУЧ. // Для данного коллайдера что бы все правильно работало нужно стрелять СВЕРХУ ВНИЗ,  поэтому сместим его вверх,
                        //и будем стрелять вниз, лучом размер которого в два раза больше чем смещение вниз, луч будет взаимодействовать с выбранной маской слоя
                        RaycastInput raycastInput = new RaycastInput
                        {
                            Start = pathNode.worldPosition + new int3(0, 1, 0),
                            End = pathNode.worldPosition + new int3(0, -1, 0),
                            Filter = collisionMousePlaneFilter
                        };
                      
                        if (collisionWorld.CastRay(raycastInput)) //Если попали в пол установим этот узел проходимым
                        {
                            pathNode.isWalkable = true;
                           
                        }
                        else // Если под ячейкой нет пола то установим флаг в воздухе
                        {
                            pathNode.isInAir = true;
                           // Debug.Log($"{pathNode.gridPosition}в воздухе");
                        }

                        //Выстрелим ЛУЧ. ЛУч не может стрелять внутри колайдера(внутри стены), поэтому сместим его вниз, и будем стрелять вверх, лучом размер которого в два раза больше чем смещение вниз, луч будет взаимодействовать с выбранной маской слоя
                        // МОЖНО СДЕЛАТЬ НАСТРОЙКИ в UNITY и не смещать выстрел луча. Project Settings/Physics/Queries Hit Backfaces - поставить галочку, и тогда можно стрелять из нутри колайдера
                        raycastInput = new RaycastInput
                        {
                            Start = pathNode.worldPosition + new int3(0, -1, 0),
                            End = pathNode.worldPosition + new int3(0, 1, 0),
                            Filter = collisionObstaclesCoverFilter
                        };

                        if (collisionWorld.CastRay(raycastInput)) // Если луч попал в препятствие или в укрытие то установим ячейку НЕ ПРОХОДИМОЙ
                        {
                            pathNode.isWalkable = false;
                            //Debug.Log($"{pathNode.gridPosition}препятсвие");
                        }

                        pathNodeDefaultArray[pathNode.index] = pathNode;
                    }
                }
            }

            state.EntityManager.SetComponentData(state.SystemHandle, new PathNodeDefaultArray
            {
                pathNodeArray = pathNodeDefaultArray,
            });
          
        }
        state.Enabled = false;
    }


    [BurstCompile]
    public
    void OnDestroy(ref SystemState state)
    {
        pathNodeDefaultArray.Dispose();
    }

  
    /// <summary> 
    /// Вычислим индекс узла (преобразуем координаты узла в плоский индекс, что бы уйти от трехмерного массива[x,floor, z]).<br/>
    /// Начинаем с нуля
    /// </summary>
    private static int CalculateIndex(int x, int floor, int z, int gridWidthXSize, int floorAmount)// https://stackoverflow.com/questions/51157907/calculating-3d-coordinates-from-a-flattened-3d-array
    {
        return x + floor * gridWidthXSize + z * gridWidthXSize * floorAmount;
    }
    
    /// <summary>
    /// Получить мировые координаты для данной ячейки сетки.<br/>
    /// (вернет координаты центра ячейки)
    /// </summary>
    private static float3 GetWorldPosition(int3 gridPosition, float cellSize, float3 anchorGrid, float floorHeight) // Получить мировое положение
    {
        return new float3(
            gridPosition.x * cellSize, // Учтем размер ячейуи
            gridPosition.y * floorHeight, // Учтем этаж и высоту этажа
            gridPosition.z * cellSize)
            + anchorGrid; //Добавим смещение якоря сетки в мировом пространстве
    }
}
