using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// Система поиска пути. Будет запускаться при включении PathfindingParams
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //Поместим в группу
partial struct PathfindingSystem : ISystem
{
    private const int MOVE_STRAIGHT_COST = 10;// Стоимость движения прямо (для удобства взяли 10 а не 1 что бы не использовать float)
    private const int MOVE_DIAGONAL_COST = 14;// Стоимость движения по диоганали ( расчитывается по теореме пифагора корень квадратный из суммы квадратов катетов прямоугольного треугольника. И опять для удобства взяли 14 а не 1,4 что бы не использовать float)


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Запуститься при включенной PathfindingParams    
        foreach ((
            RefRO<PathfindingParams> pathfindingParams,
            RefRO<PathfindingGridDate> pathfindingGridDate,
            Entity entity)
            in SystemAPI.Query<
                RefRO<PathfindingParams>,
                RefRO<PathfindingGridDate>>().WithEntityAccess())
        {

            PathNodeDefaultArray pathNodeDefaultArray = SystemAPI.GetSingleton<PathNodeDefaultArray>(); //Прикриплен к сущности системы поэтому получаем через синголтон
            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(pathNodeDefaultArray.pathNodeArray, Allocator.Temp); //Временный массив
            int3 startPosition = pathfindingParams.ValueRO.startPosition;
            int3 endPosition = pathfindingParams.ValueRO.endPosition;
                       

            //Пересчитаем hCost у всех узлов для заданной endPosition. И сбросим cameFromNodeIndex 
            for (int i = 0; i < pathNodeArray.Length; i++)
            {
                PathNode pathNode = pathNodeArray[i];
                pathNode.hCost = CalculateHeuristicDistance(pathNode.gridPosition, endPosition);
                // pathNode.cameFromNodeIndex = -1; // Сброс не нужен т.к. мы всегда копируем  дефолтный массив

                pathNodeArray[i] = pathNode;
            }
                     
            //Список соседних узлов (y- floo сделаем нулевым)
            NativeArray<int3> neighbourOffsetArray = new NativeArray<int3>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int3(-1, 0, 0); // Left
            neighbourOffsetArray[1] = new int3(+1, 0, 0); // Right
            neighbourOffsetArray[2] = new int3(0, 0, +1); // Up
            neighbourOffsetArray[3] = new int3(0, 0, -1); // Down
            neighbourOffsetArray[4] = new int3(-1, 0, -1); // Left Down
            neighbourOffsetArray[5] = new int3(-1, 0, +1); // Left Up
            neighbourOffsetArray[6] = new int3(+1, 0, -1); // Right Down
            neighbourOffsetArray[7] = new int3(+1, 0, +1); // Right Up

            int endNodeIndex = CalculateIndex(endPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount); //Вычислим индекс конечного узла

            PathNode startNode = pathNodeArray[CalculateIndex(startPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            // Списки содержат только индекс узла а не сам узел
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);     //ВСЕ УЗЛЫ КОТОРЫЕ ПРЕДСТОИТ НАЙТИ
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);   //ВСЕ УЗЛЫ, В КОТОРЫХ УЖЕ БЫЛ ПРОИЗВЕДЕН ПОИСК

            openList.Add(startNode.index);//Добавим в список начальный узел
           
            //Заполним у узлов gCost  fCost
            while (openList.Length > 0)// если в Открытом списке есть элементы то это означает что есть узлы для поиска. Цикл будет работать пока не переберет все ячейки
            {
                PathNode currentNode = GetLowestCostFNode(openList, pathNodeArray); // текущий узел с наименьшим F (в начале это startNode)

                if (currentNode.index == endNodeIndex)
                {
                    // Достигли цели!
                    break;
                }

                // Удалить текущий узел из открытого списка
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNode.index)
                    {
                        openList.RemoveAtSwapBack(i); //удалить по индексу
                        break;
                    }
                }
                // И добавим в закрытый список // ЭТО ОЗНАЧАЕТ ЧТО МЫ ИСКАЛИ ПО ЭТОМУ УЗЛУ
                closedList.Add(currentNode.index);
              
                // Переберем всех соседние узлы
                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int3 neighbourOffset = neighbourOffsetArray[i];
                    int3 neighbourPosition = currentNode.gridPosition + neighbourOffset;// new int3(currentNode.gridPosition.x + neighbourOffset.x, currentNode.gridPosition.z + neighbourOffset.y); // Позиция соседа

                    if (!IsPositionInsideGrid(neighbourPosition, pathfindingGridDate.ValueRO))
                    {
                        // у соседа недействительная позиция                      
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);

                    if (closedList.Contains(neighbourNodeIndex))
                    {
                        // Уже искали этот узел
                        continue;
                    }

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable)
                    {
                        // Непроходимо
                        continue;
                    }

                    // Предварительная стоимость G = текущая G + стоимость перемещения от текущего к соседнему узлу
                    int tentativeGCost = currentNode.gCost + CalculateHeuristicDistance(currentNode.gridPosition, neighbourPosition);
                    
                    if (tentativeGCost < neighbourNode.gCost)// Если Предварительная стоимость G меньше стоимости G соседнего узла(по умолчанию она имеет значение MaxValue )
                                                             //  то - Мы нашли луший путь что бы попасть в этот соседний узел
                    {
                        neighbourNode.cameFromNodeIndex = currentNode.index;// Установить - на соседний узел пришел - С текущего Узла Пути
                        neighbourNode.gCost = tentativeGCost;  // Установим на соседний узем расчитанный параметр G
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode; // т.к. мы работали с копией то сохраним ее
                      
                        if (!openList.Contains(neighbourNode.index))// Если открытый список не содержит этого соседнего узла то добавим его
                        {
                            openList.Add(neighbourNode.index);                           
                        }
                    }

                }
            }

            DynamicBuffer<PathPositionBufferElement> pathPositionBuffer = SystemAPI.GetBuffer<PathPositionBufferElement>(entity);
            pathPositionBuffer.Clear();

            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                // Не нашел путь!
                Debug.Log("Не нашел путь!");
                // pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = -1 };
            }
            else
            {
                //Нашел путь
                CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
                // pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1 };
            }

            //Выключим чтобы в след кадре не запустился Update
            SystemAPI.SetComponentEnabled<PathfindingParams>(entity, false);
        }
    }

    



    /// <summary>
    /// Рассчитать путь и записать в переданный буффер
    /// </summary>
    private static void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode, DynamicBuffer<PathPositionBufferElement> pathPositionBuffer)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // Не удалось найти путь!
        }
        else
        {
            //Нашел путь
            //Начнем с конца и заполним список
            pathPositionBuffer.Add(new PathPositionBufferElement { gridPosition = endNode.gridPosition });

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)//В цикле проверим есть ли индекс узла с которого мы сюда пришли
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];//По индексу найдем узел с которого пришли
                pathPositionBuffer.Add(new PathPositionBufferElement { gridPosition = cameFromNode.gridPosition });
                currentNode = cameFromNode;
            }
        }
    }
    /// <summary>
    /// Рассчитать путь
    /// </summary>
    private static NativeList<int3> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // Не удалось найти путь!
            return new NativeList<int3>(Allocator.Temp); //Вернем нулевой список
        }
        else
        {
            // Нашел путь
            NativeList<int3> path = new NativeList<int3>(Allocator.Temp);
            path.Add(endNode.gridPosition);
            //Начнем с конца и раскрутим клубок (заполним список пути). Когда дайдем до стартовой позиции цикл остановиться т.к. у нее cameFromNodeIndex=(-1)
            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1) //В цикле проверим есть ли индекс узла с которого мы сюда пришли
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex]; //По индексу найдем узел с которого пришли
                path.Add(cameFromNode.gridPosition);
                currentNode = cameFromNode; // Узел с которого пришли становиться текущим
            }

            return path;
        }
    }

    /// <summary>
    /// Позиция внутри сетки?
    /// </summary>
    private static bool IsPositionInsideGrid(int3 gridPosition, PathfindingGridDate pathfindingGridDate)
    {
        return
            gridPosition.x >= 0 &&
            gridPosition.z >= 0 &&
            gridPosition.x < pathfindingGridDate.width &&
            gridPosition.z < pathfindingGridDate.height;
    }

    /// <summary>
    /// Расчитать эвристическое(приблизительное) расстояние между любыми 2мя узлами ЭТО БУДЕТ ПАРАМЕТР "H"
    /// </summary>
    private static int CalculateHeuristicDistance(int3 gridPositionA, int3 gridPositionB) // Вычислить эвристическое(приблизительное) расстояние между любыми 2мя сеточными позициями ЭТО БУДЕТ ПАРАМЕТР "H"
    {
        int3 gridPositionDistance = gridPositionA - gridPositionB; //Может быть отрицательным (вычислить приращение координат между первой и второй точкой - на сколько надо сместить относительно первой точки)

        // для движение только по прямой (МОЖНО ИСПОЛЬЗОВАТЬ ДЛЯ ПОИСКА СЕТОК ДВЕРЕЙ)
        //int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.y); // получим сумму координат по модулю (не учитывает движение по диогонали только по прямой. Например из точки (0,0) в точку (3,2) двигался три раза в право и два раза вверх итого 5)

        int xDistsnce = math.abs(gridPositionDistance.x); //перемещение с (0,0) на (1,1) можем заменить 1 диоганалью, вместо ДВУХ перемещений по прямой // Если с (0,0) на (2,1) то диогональ будет все равно ОДНА. Поэтому для расчета количества диоганалей надо взять минимальное число из полученных (x,y)
        int zDistsnce = math.abs(gridPositionDistance.z);
        int remaining = math.abs(xDistsnce - zDistsnce); // Оставшееся растояние будет перемещением по прямой, берем по модулю
        int CalculateDistance = MOVE_DIAGONAL_COST * math.min(xDistsnce, zDistsnce) + MOVE_STRAIGHT_COST * remaining; // вернем вычесленное расстояние в системе GridPositionXZ как сумму смещений по диогонали и смещений по прямой
        return CalculateDistance;
    }

    /// <summary>
    /// Получите узел пути с самой низкой стоимостью F <br/>
    /// Если несколько ячеек имеют одинаковое наименьшее F то вернется первый попавшийся в списке
    /// </summary>
    private static PathNode GetLowestCostFNode(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]]; //Узел пути с наименьшей стоимостью 
        for (int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode;
    }

    /// <summary> 
    /// Вычислим индекс узла (преобразуем координаты узла в плоский индекс, что бы уйти от трехмерного массива [x, floor, z] ).<br/>
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
}
