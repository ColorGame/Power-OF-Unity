using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor.Overlays;
using UnityEngine;
using static PathNodeSystem;

/// <summary>
/// Система УЗЛОВ ПУТИ.<br/>
/// Создает ДЕФОЛТНЫЙ СПИСОК УЗЛОВ ПУТИ.<br/>
/// Настраивает проходимость узлов.
/// </summary>


partial struct PathNodeSystem : ISystem
{
    private CollisionFilter _collisionMousePlaneFilter;
    private CollisionFilter _collisionObstaclesCoverFilter;

    private NativeArray<PathNode> _defaultPathNodeArray;
    private NativeParallelHashMap<int3, PathNode> _validGridPositionPathNodeDict;

    private Entity _createEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _createEntity = state.EntityManager.CreateEntity(); //создадим сущность 
        state.EntityManager.AddComponent<DefaultPathNodeArray>(_createEntity);
        state.EntityManager.AddComponent<ValidGridPositionPathNodeDict>(_createEntity);



        _collisionMousePlaneFilter = new CollisionFilter
        {//https://discussions.unity.com/t/how-do-i-use-layermasks/481 -обхъъяснение битового сдвига
            BelongsTo = ~0u, // Луч будет принадлежать каждому слою поэтому инвертируем все "0" с помощью оператора побитового инвертирования (~) и преобразуемв в uint с помощбю "u"
            CollidesWith = 1u << 6,// Колизия будет с 6 - MousePlane слоем маски поэтому воспользуемся сдвигом битовой маски ЭТО ПОЛ (НУЖЕН РЕФАКТОРИНГ)
            GroupIndex = 0,
        };
        _collisionObstaclesCoverFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << 8 | 1u << 11,// Колизия будет с 8 - Obstacles и 11 - Cover слоем маски поэтому воспользуемся сдвигом битовой маски (НУЖЕН РЕФАКТОРИНГ)
            GroupIndex = 0,
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Выполниться когда у сущности включим PathfindingGridDate
        foreach ((RefRO<PathfindingGridDate> pathfindingGridDate, Entity pathfindingEntity) in SystemAPI.Query<RefRO<PathfindingGridDate>>().WithEntityAccess())
        {
            // В цикле настроим все ячейки на возможность проходимости, будем стрелять лучом из каждой позиции и для начала Сделаем все узлы непроходимыми
            // и если 1ый луч попал в пол то сделаем проходимым а 2рым луч проверим на препядствие _obstaclesDoorMousePlaneCoverLayerMask,
            // если они есть установим эту ячейку опять не проходимой
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;// Для создания луча                              

            _defaultPathNodeArray = new NativeArray<PathNode>(pathfindingGridDate.ValueRO.width * pathfindingGridDate.ValueRO.height * pathfindingGridDate.ValueRO.floorAmount, Allocator.Persistent);

            for (int x = 0; x < pathfindingGridDate.ValueRO.width; x++)
            {
                for (int z = 0; z < pathfindingGridDate.ValueRO.height; z++)
                {
                    for (int flooor = 0; flooor < pathfindingGridDate.ValueRO.floorAmount; flooor++)
                    {
                        // Entity pathNodeEntity = state.EntityManager.CreateEntity(typeof(PathNode)); //создадим сущность типа PathNode
                        PathNode pathNode = new PathNode(); // Создадим и инициализируем узел

                        pathNode.gridPosition = new int3(x, flooor, z);
                        pathNode.worldPosition = GetWorldPosition(pathNode.gridPosition, pathfindingGridDate.ValueRO.cellSize, pathfindingGridDate.ValueRO.anchorGrid, pathfindingGridDate.ValueRO.floorHeight);
                        pathNode.index = CalculateIndex(x, flooor, z, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                        pathNode.gCost = int.MaxValue;
                        pathNode.hCost = default;
                        pathNode.fCost = default;
                        pathNode.cameFromNodeIndex = -1; //Установим недопустимое значение

                        pathNode.isInAir = false; //По умолчанию узел НЕ в воздухе
                        pathNode.isWalkable = false; //По умолчанию сделаем НЕпроходимой

                        pathNode.pathWorldPositionList = new NativeList<float3>(Allocator.Persistent);


                        //Выстрелим ЛУЧ. // Для данного коллайдера что бы все правильно работало нужно стрелять СВЕРХУ ВНИЗ,  поэтому сместим его вверх,
                        //и будем стрелять вниз, лучом размер которого в два раза больше чем смещение вниз, луч будет взаимодействовать с выбранной маской слоя
                        RaycastInput raycastInput = new RaycastInput
                        {
                            Start = pathNode.worldPosition + new int3(0, 1, 0),
                            End = pathNode.worldPosition + new int3(0, -1, 0),
                            Filter = _collisionMousePlaneFilter
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
                            Filter = _collisionObstaclesCoverFilter
                        };

                        if (collisionWorld.CastRay(raycastInput)) // Если луч попал в препятствие или в укрытие то установим ячейку НЕ ПРОХОДИМОЙ
                        {
                            pathNode.isWalkable = false;
                            //Debug.Log($"{pathNode.gridPosition}препятсвие");
                        }

                        _defaultPathNodeArray[pathNode.index] = pathNode;
                    }
                }
            }

            state.EntityManager.SetComponentData(_createEntity, new DefaultPathNodeArray
            {
                pathNodeArray = _defaultPathNodeArray
            });

            //Создам словарь такимже размером как и вся карта. Чтобы не выйти за пределы текущей ёмкости (Если для хотьбы допустимы все узлы)
            _validGridPositionPathNodeDict = new NativeParallelHashMap<int3, PathNode>(_defaultPathNodeArray.Length, Allocator.Persistent);
            state.EntityManager.SetComponentData(_createEntity, new ValidGridPositionPathNodeDict
            {
                dictionary = _validGridPositionPathNodeDict,
                onRegister = true,
            });

            Debug.Log($"Словапь СОЗДАН onRegister = {SystemAPI.GetSingleton<ValidGridPositionPathNodeDict>().dictionary.IsCreated}");

            //Выключим чтобы в след кадре не запустился Update
            state.EntityManager.SetComponentEnabled<PathfindingGridDate>(pathfindingEntity, false);
        }
    }


    [BurstCompile]
    public
    void OnDestroy(ref SystemState state)
    {
        foreach (PathNode pathNode in _defaultPathNodeArray)
        {
            pathNode.pathWorldPositionList.Dispose();
        }
        _defaultPathNodeArray.Dispose();
        _validGridPositionPathNodeDict.Dispose();
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
    /// Вычислим индекс узла (преобразуем координаты узла в плоский индекс, что бы уйти от трехмерного массива [x, floor, z] ).<br/>
    /// Начинаем с нуля
    /// </summary>
    private static int CalculateIndex(int3 gridPosition, int gridWidthXSize, int floorAmount)// https://stackoverflow.com/questions/51157907/calculating-3d-coordinates-from-a-flattened-3d-array
    {
        return gridPosition.x + gridPosition.y * gridWidthXSize + gridPosition.z * gridWidthXSize * floorAmount;
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


    /// <summary>
    /// Массив взех узлов пути в дефолтном состоянии.<br/> 
    /// Добавить этот компонет к сущности Pathfinding.<br/>  
    /// </summary>
    public struct DefaultPathNodeArray : IComponentData
    {
        public NativeArray<PathNode> pathNodeArray;
    }

    /// <summary>
    /// Словарь - допустимые позиции сетки и узлы пути.<br/> 
    /// Добавить этот компонет к сущности Pathfinding.<br/>  
    /// И выключить по умолчанию
    /// </summary>
    public struct ValidGridPositionPathNodeDict : IComponentData
    {
        public NativeParallelHashMap<int3, PathNode> dictionary;
        public bool onRegister;
    }
}
