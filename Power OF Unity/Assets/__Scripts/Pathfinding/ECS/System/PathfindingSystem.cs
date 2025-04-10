using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static PathNodeSystem;
/// <summary>
/// Система поиска пути. Будет запускаться при включении PathfindingParams.<br/>Работать с данной системой через PathfindingProviderSystem
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //Поместим в группу
partial struct PathfindingSystem : ISystem
{
    private const int MOVE_STRAIGHT_COST = 10;// Стоимость движения прямо (для удобства взяли 10 а не 1 что бы не использовать float)
    private const int MOVE_DIAGONAL_COST = 14;// Стоимость движения по диоганали ( расчитывается по теореме пифагора корень квадратный из суммы квадратов катетов прямоугольного треугольника. И опять для удобства взяли 14 а не 1,4 что бы не использовать float)



    /* public void OnUpdate(ref SystemState state)
     {
         //Запуститься при включенной PathfindingParams    
         foreach ((
             RefRW<PathfindingParams> pathfindingParams,
             RefRO<PathfindingGridDate> pathfindingGridDate)
             in SystemAPI.Query<
                 RefRW<PathfindingParams>,
                 RefRO<PathfindingGridDate>>())
         {

             PathNodeSystem.DefaultPathNodeArray defaultPathNodeArray = SystemAPI.GetSingleton<PathNodeSystem.DefaultPathNodeArray>(); //Прикриплен к сущности системы поэтому получаем через синголтон
             NativeArray<PathNode> pathNodeTempArray = new NativeArray<PathNode>(defaultPathNodeArray.pathNodeTempArray, Allocator.Temp); // Временный массив Скопируем дефолтный массив

             int3 startGridPosition = pathfindingParams.ValueRO.startGridPosition;
             int3 endGridPosition = pathfindingParams.ValueRO.endGridPosition;

             *//* //Пересчитаем hCost у всех узлов для заданной endGridPosition.
              foreach (RefRW<PathNode> pathNode in SystemAPI.Query<RefRW<PathNode>>())
              {
                  pathNode.ValueRW.hCost = CalculateHeuristicDistance(pathNode.ValueRO.gridPosition, endGridPosition);
                  pathNode.ValueRW.startPositionPath = startGridPosition;
              }*//*


             //Пересчитаем hCost у всех узлов для заданной endGridPosition. И сбросим cameFromNodeIndex 
             for (int index = 0; index < pathNodeTempArray.Length; index++)
             {
                 PathNode pathNode = pathNodeTempArray[index];
                 pathNode.hCost = CalculateHeuristicDistance(pathNode.gridPosition, endGridPosition);
                 // pathNode.cameFromNodeIndex = -1; // Сброс не нужен т.к. мы всегда копируем  дефолтный массив

                 pathNodeTempArray[index] = pathNode;
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

             //Вычислим индекс стартового и конечного узла
             int endNodeIndex = CalculateIndex(endGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
             int startNodeIndex = CalculateIndex(startGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);

             PathNode startNode = pathNodeTempArray[startNodeIndex];
             startNode.gCost = 0;
             startNode.CalculateFCost();
             pathNodeTempArray[startNode.index] = startNode; //Перезапишем

             // Списки содержат только индекс узла а не сам узел
             NativeList<int> openList = new NativeList<int>(Allocator.Temp);     //ВСЕ УЗЛЫ КОТОРЫЕ ПРЕДСТОИТ НАЙТИ
             NativeList<int> closedList = new NativeList<int>(Allocator.Temp);   //ВСЕ УЗЛЫ, В КОТОРЫХ УЖЕ БЫЛ ПРОИЗВЕДЕН ПОИСК

             openList.Add(startNode.index);//Добавим в список начальный узел

             //Заполним у узлов gCost  fCost
             while (openList.Length > 0)// если в Открытом списке есть элементы то это означает что есть узлы для поиска. Цикл будет работать пока не переберет все ячейки
             {
                 PathNode currentNode = GetLowestCostFNode(openList, pathNodeTempArray); // текущий узел с наименьшим F (в начале это startNode)

                 if (currentNode.index == endNodeIndex)
                 {
                     // Достигли цели!
                     break;
                 }

                 // Удалить текущий узел из открытого списка
                 for (int index = 0; index < openList.Length; index++)
                 {
                     if (openList[index] == currentNode.index)
                     {
                         openList.RemoveAtSwapBack(index); //удалить по индексу
                         break;
                     }
                 }
                 // И добавим в закрытый список // ЭТО ОЗНАЧАЕТ ЧТО МЫ ИСКАЛИ ПО ЭТОМУ УЗЛУ
                 closedList.Add(currentNode.index);

                 // Переберем всех соседние узлы
                 for (int index = 0; index < neighbourOffsetArray.Length; index++)
                 {
                     int3 neighbourOffset = neighbourOffsetArray[index];
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

                     PathNode neighbourNode = pathNodeTempArray[neighbourNodeIndex];
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
                         pathNodeTempArray[neighbourNodeIndex] = neighbourNode; // т.к. мы работали с копией то сохраним ее

                         if (!openList.Contains(neighbourNode.index))// Если открытый список не содержит этого соседнего узла то добавим его
                         {
                             openList.Add(neighbourNode.index);
                         }
                     }

                 }
             }           

             PathNode endNode = pathNodeTempArray[endNodeIndex];
             if (endNode.cameFromNodeIndex == -1)
             {
                 // Не нашел путь!
                 Debug.Log("Не нашел путь!");
             }
             else
             {
                 //Нашел путь
                 CalculatePath(pathNodeTempArray, endNode);
                 pathfindingParams.ValueRW.isPathfindingComplete = true;
             }

             //Выключим чтобы в след кадре не запустился Update
          //   SystemAPI.SetComponentEnabled<PathfindingParams>(pathfindingEntity, false);


             openList.Dispose();
             closedList.Dispose();
             pathNodeTempArray.Dispose();
             neighbourOffsetArray.Dispose();
         }
     }*/

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Запуститься при включенной PathfindingParams и любым состояними других компонентов
        foreach ((
            RefRW<PathfindingParams> pathfindingParams,
            RefRO<PathfindingGridDate> pathfindingGridDate,
            Entity pathfindingEntity)
            in SystemAPI.Query<
                RefRW<PathfindingParams>,
                RefRO<PathfindingGridDate>>().
                WithPresent<PathfindingGridDate>().WithEntityAccess())
        {          
            DefaultPathNodeArray defaultPathNodeArray = SystemAPI.GetSingleton<DefaultPathNodeArray>(); //Прикриплен к сущности системы поэтому получаем через синголтон

            ValidGridPositionPathNodeDict validGridPositionPathNodeDict = SystemAPI.GetSingleton<ValidGridPositionPathNodeDict>();
            validGridPositionPathNodeDict.dictionary.Clear();

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

            int3 startGridPosition = pathfindingParams.ValueRO.startGridPosition;
            int startNodeIndex = CalculateIndex(startGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);

            int moveDistance = pathfindingParams.ValueRO.maxMoveDistance;
            for (int x = -moveDistance; x <= moveDistance; x++) // Юнит это центр нашей позиции с координатами unitGridPosition, поэтому переберем допустимые значения в условном радиусе maxMoveDistance
            {
                for (int y = -moveDistance; y <= moveDistance; y++)
                {
                    for (int z = -moveDistance; z <= moveDistance; z++)
                    {
                        int3 offsetGridPosition = new int3(x, y, z); // Смещенная сеточная позиция. Где началом координат(0,0, floor-этаж) является сам юнит 
                        int3 testGridPosition = startGridPosition + offsetGridPosition; // Тестируемая Сеточная позиция

                        if (!IsPositionInsideGrid(testGridPosition, pathfindingGridDate.ValueRO))
                            continue; // Тестируемая Сеточная позиция ВНЕ СЕТКИ  

                        /*if (testGridPosition.Equals(startGridPosition)) // ЛИШНИЯ ПРОВЕРКА т.к. узел на котором стоит игрок должен быть непроходим а это я проверяю ниже
                        {
                            Debug.Log($"{testGridPosition} -- пропускаю");
                            continue; // Пропустим узел с юнитом
                        }*/
                        // Временный массив Скопируем дефолтный массив
                        NativeArray<PathNode> pathNodeTempArray = new NativeArray<PathNode>(defaultPathNodeArray.pathNodeArray, Allocator.Temp);

                        //Вычислим индекс тестируемого узла и получим сам узел                    
                        int testNodeIndex = CalculateIndex(testGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                        PathNode testPathNode = pathNodeTempArray[testNodeIndex];

                        if (!testPathNode.isWalkable)// Непроходимо              
                            continue;
                        //Debug.Log($"1 - Тестирую {testGridPosition}");

                        //Настроим стартовый узел и сохраним его
                        PathNode startNode = pathNodeTempArray[startNodeIndex]; // получим копию
                        startNode.gCost = 0;
                        startNode.hCost = CalculateHeuristicDistance(startGridPosition, testGridPosition);
                        startNode.CalculateFCost();
                        pathNodeTempArray[startNode.index] = startNode; //Перезапишем отредактированную копию

                        // Списки содержат только индекс узла а не сам узел
                        NativeList<int> openList = new NativeList<int>(Allocator.Temp);     //ВСЕ УЗЛЫ КОТОРЫЕ ПРЕДСТОИТ НАЙТИ
                        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);   //ВСЕ УЗЛЫ, В КОТОРЫХ УЖЕ БЫЛ ПРОИЗВЕДЕН ПОИСК

                        openList.Add(startNode.index);//Добавим в список начальный узел

                        //Заполним у узлов gCost  fCost
                        while (openList.Length > 0)// если в Открытом списке есть элементы то это означает что есть узлы для поиска. Цикл будет работать пока не переберет все ячейки
                        {
                            PathNode currentNode = GetLowestCostFNode(openList, pathNodeTempArray); // текущий узел с наименьшим F (в начале это startNode)
                            //Debug.Log($"2 - Узел с наименьшим F - {currentNode.gridPosition}");
                            if (currentNode.index == testNodeIndex)
                            {
                                // Debug.Log($"3 - Достигли цели!{currentNode.gridPosition} ");
                                break;// Достигли цели!   
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
                                    continue; // у соседа недействительная позиция      

                                int neighbourNodeIndex = CalculateIndex(neighbourPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                                if (closedList.Contains(neighbourNodeIndex))
                                    continue; // Уже искали этот узел                        

                                PathNode neighbourNode = pathNodeTempArray[neighbourNodeIndex];
                                if (!neighbourNode.isWalkable)
                                    continue; // Непроходимо

                                // Предварительная стоимость G = текущая G + стоимость перемещения от текущего к соседнему узлу
                                int tentativeGCost = currentNode.gCost + CalculateHeuristicDistance(currentNode.gridPosition, neighbourPosition);
                                if (tentativeGCost > (moveDistance * MOVE_STRAIGHT_COST))
                                    continue; // Далеко идти


                                //Debug.Log($"4 - Сосед{neighbourNode.gridPosition} ");

                                if (tentativeGCost < neighbourNode.gCost)// Если Предварительная стоимость G меньше стоимости G соседнего узла(по умолчанию она имеет значение MaxValue )
                                                                         //  то - Мы нашли луший путь что бы попасть в этот соседний узел
                                {
                                    //Debug.Log($"5 - Стоимость G{tentativeGCost} из {currentNode.gridPosition} в {neighbourNode.gridPosition} ");
                                    neighbourNode.cameFromNodeIndex = currentNode.index;// Установить - на соседний узел пришел - С текущего Узла Пути
                                    neighbourNode.gCost = tentativeGCost;  // Установим на соседний узем расчитанный параметр G
                                    neighbourNode.hCost = CalculateHeuristicDistance(neighbourNode.gridPosition, testGridPosition);
                                    neighbourNode.CalculateFCost();
                                    pathNodeTempArray[neighbourNodeIndex] = neighbourNode; // т.к. мы работали с копией то сохраним ее
                                    //Debug.Log($"6 - Индекс откуда приперся {neighbourNode.cameFromNodeIndex} ");
                                    if (!openList.Contains(neighbourNode.index))// Если открытый список не содержит этого соседнего узла то добавим его                                    
                                        openList.Add(neighbourNode.index);
                                }
                            }
                        }

                        // Debug.Log($"7 - Расчитать путь для - {pathNodeTempArray[testNodeIndex].gridPosition} - пришол с -  {pathNodeTempArray[testNodeIndex].cameFromNodeIndex}");
                        CalculatePath(pathNodeTempArray, validGridPositionPathNodeDict, pathNodeTempArray[testNodeIndex]);

                        openList.Dispose();
                        closedList.Dispose();
                        pathNodeTempArray.Dispose();
                    }
                }
            }



            /*foreach (var collection in validGridPositionPathNodeDict.dictionary)
            {
                Debug.Log($"Узел ИЗМЕН - {collection.Key}");
                for (int index = 0; index < validGridPositionPathNodeDict.dictionary[collection.Key].pathWorldPositionList.Length; index++)
                {
                    Debug.Log($"ИНДЕКС - {index} - УЗЕЛ - {validGridPositionPathNodeDict.dictionary[collection.Key].pathWorldPositionList[index]}");
                }
            }  */

            //Перезаписывать не надо т.к. Native Collections - это прямая ссылка на ячейку памяти
            /* //Получим сущность и перезапишем у нее validGridPositionPathNodeDict
             Entity pathNodeSystemEntity = SystemAPI.GetSingletonEntity<ValidGridPositionPathNodeDict>();
             state.EntityManager.SetComponentData(pathNodeSystemEntity, validGridPositionPathNodeDict);*/

            //Запустим событие путь расчитан
            pathfindingParams.ValueRW.onPathfindingComplete = true;
            Debug.Log($"onPathfindingComplete = true");

            //Выключим чтобы в след кадре не запустился Update
            SystemAPI.SetComponentEnabled<PathfindingParams>(pathfindingEntity, false);
            neighbourOffsetArray.Dispose();
        }
    }


    /// <summary>
    /// Рассчитать путь и записать в узел<br/>
    /// Сохронять отредактированный конечный узел в "configuredPathNodeArray"
    /// </summary>
    private static void CalculatePath(NativeArray<PathNode> pathNodeTempArray, ValidGridPositionPathNodeDict validNodeForPath, PathNode endNode)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // Не удалось найти путь!
            // Debug.Log($"Не нашел путь!{endNode.gridPosition}");
        }
        else
        {
            //Нашел путь. Начнем с конца и заполним список   
            endNode.pathWorldPositionList.Clear(); //Очистим т.к. информация может оставаться в памяти даже после остановки игры
            endNode.pathWorldPositionList.Add(endNode.worldPosition);
            PathNode currentNode = endNode;

            while (currentNode.cameFromNodeIndex != -1)//В цикле проверим есть ли индекс узла с которого мы сюда пришли
            {
                PathNode cameFromNode = pathNodeTempArray[currentNode.cameFromNodeIndex];//По индексу найдем узел с которого пришли
                endNode.pathWorldPositionList.Add(cameFromNode.worldPosition);
                currentNode = cameFromNode;
            }

            //Удалим последнию позицию т.к. это стартовая позиция юнита и по ней не надо ходить т.к. мы уже на ней
            endNode.pathWorldPositionList.RemoveAtSwapBack(endNode.pathWorldPositionList.Length - 1);

            /* foreach (float3 worldPosition in endNode.pathWorldPositionList)
             {
                 Debug.Log($"точки пути после добавления - {worldPosition}");
             }*/

            validNodeForPath.dictionary.Add(endNode.gridPosition, endNode);

            //  Debug.Log($"{validNodeForPath.dictionary.Count()}");          
        }
    }
    /* /// <summary>
     /// Рассчитать путь
     /// </summary>
     private static NativeList<int3> CalculatePath(NativeArray<PathNode> pathNodeTempArray, PathNode endNode)
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
                 PathNode cameFromNode = pathNodeTempArray[currentNode.cameFromNodeIndex]; //По индексу найдем узел с которого пришли
                 path.Add(cameFromNode.gridPosition);
                 currentNode = cameFromNode; // Узел с которого пришли становиться текущим
             }

             return path;
         }
     }*/

    /// <summary>
    /// Позиция внутри сетки?
    /// </summary>
    private static bool IsPositionInsideGrid(int3 gridPosition, PathfindingGridDate pathfindingGridDate)
    {
        return
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.z >= 0 &&
            gridPosition.x < pathfindingGridDate.width &&
            gridPosition.y < pathfindingGridDate.floorAmount &&
            gridPosition.z < pathfindingGridDate.height;

    }

    /// <summary>
    /// Расчитать эвристическое(приблизительное) расстояние между любыми 2мя узлами ЭТО БУДЕТ ПАРАМЕТР "H".<br/>
    /// для endGridPosition H=0.
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
