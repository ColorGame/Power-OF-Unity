using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static PathNodeSystem;
/// <summary>
/// ������� ������ ����. ����� ����������� ��� ��������� PathfindingParams.<br/>�������� � ������ �������� ����� PathfindingProviderSystem
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //�������� � ������
partial struct PathfindingSystem : ISystem
{
    private const int MOVE_STRAIGHT_COST = 10;// ��������� �������� ����� (��� �������� ����� 10 � �� 1 ��� �� �� ������������ float)
    private const int MOVE_DIAGONAL_COST = 14;// ��������� �������� �� ��������� ( ������������� �� ������� �������� ������ ���������� �� ����� ��������� ������� �������������� ������������. � ����� ��� �������� ����� 14 � �� 1,4 ��� �� �� ������������ float)



    /* public void OnUpdate(ref SystemState state)
     {
         //����������� ��� ���������� PathfindingParams    
         foreach ((
             RefRW<PathfindingParams> pathfindingParams,
             RefRO<PathfindingGridDate> pathfindingGridDate)
             in SystemAPI.Query<
                 RefRW<PathfindingParams>,
                 RefRO<PathfindingGridDate>>())
         {

             PathNodeSystem.DefaultPathNodeArray defaultPathNodeArray = SystemAPI.GetSingleton<PathNodeSystem.DefaultPathNodeArray>(); //���������� � �������� ������� ������� �������� ����� ���������
             NativeArray<PathNode> pathNodeTempArray = new NativeArray<PathNode>(defaultPathNodeArray.pathNodeTempArray, Allocator.Temp); // ��������� ������ ��������� ��������� ������

             int3 startGridPosition = pathfindingParams.ValueRO.startGridPosition;
             int3 endGridPosition = pathfindingParams.ValueRO.endGridPosition;

             *//* //����������� hCost � ���� ����� ��� �������� endGridPosition.
              foreach (RefRW<PathNode> pathNode in SystemAPI.Query<RefRW<PathNode>>())
              {
                  pathNode.ValueRW.hCost = CalculateHeuristicDistance(pathNode.ValueRO.gridPosition, endGridPosition);
                  pathNode.ValueRW.startPositionPath = startGridPosition;
              }*//*


             //����������� hCost � ���� ����� ��� �������� endGridPosition. � ������� cameFromNodeIndex 
             for (int index = 0; index < pathNodeTempArray.Length; index++)
             {
                 PathNode pathNode = pathNodeTempArray[index];
                 pathNode.hCost = CalculateHeuristicDistance(pathNode.gridPosition, endGridPosition);
                 // pathNode.cameFromNodeIndex = -1; // ����� �� ����� �.�. �� ������ ��������  ��������� ������

                 pathNodeTempArray[index] = pathNode;
             }

             //������ �������� ����� (y- floo ������� �������)
             NativeArray<int3> neighbourOffsetArray = new NativeArray<int3>(8, Allocator.Temp);
             neighbourOffsetArray[0] = new int3(-1, 0, 0); // Left
             neighbourOffsetArray[1] = new int3(+1, 0, 0); // Right
             neighbourOffsetArray[2] = new int3(0, 0, +1); // Up
             neighbourOffsetArray[3] = new int3(0, 0, -1); // Down
             neighbourOffsetArray[4] = new int3(-1, 0, -1); // Left Down
             neighbourOffsetArray[5] = new int3(-1, 0, +1); // Left Up
             neighbourOffsetArray[6] = new int3(+1, 0, -1); // Right Down
             neighbourOffsetArray[7] = new int3(+1, 0, +1); // Right Up

             //�������� ������ ���������� � ��������� ����
             int endNodeIndex = CalculateIndex(endGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
             int startNodeIndex = CalculateIndex(startGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);

             PathNode startNode = pathNodeTempArray[startNodeIndex];
             startNode.gCost = 0;
             startNode.CalculateFCost();
             pathNodeTempArray[startNode.index] = startNode; //�����������

             // ������ �������� ������ ������ ���� � �� ��� ����
             NativeList<int> openList = new NativeList<int>(Allocator.Temp);     //��� ���� ������� ��������� �����
             NativeList<int> closedList = new NativeList<int>(Allocator.Temp);   //��� ����, � ������� ��� ��� ���������� �����

             openList.Add(startNode.index);//������� � ������ ��������� ����

             //�������� � ����� gCost  fCost
             while (openList.Length > 0)// ���� � �������� ������ ���� �������� �� ��� �������� ��� ���� ���� ��� ������. ���� ����� �������� ���� �� ��������� ��� ������
             {
                 PathNode currentNode = GetLowestCostFNode(openList, pathNodeTempArray); // ������� ���� � ���������� F (� ������ ��� startNode)

                 if (currentNode.index == endNodeIndex)
                 {
                     // �������� ����!
                     break;
                 }

                 // ������� ������� ���� �� ��������� ������
                 for (int index = 0; index < openList.Length; index++)
                 {
                     if (openList[index] == currentNode.index)
                     {
                         openList.RemoveAtSwapBack(index); //������� �� �������
                         break;
                     }
                 }
                 // � ������� � �������� ������ // ��� �������� ��� �� ������ �� ����� ����
                 closedList.Add(currentNode.index);

                 // ��������� ���� �������� ����
                 for (int index = 0; index < neighbourOffsetArray.Length; index++)
                 {
                     int3 neighbourOffset = neighbourOffsetArray[index];
                     int3 neighbourPosition = currentNode.gridPosition + neighbourOffset;// new int3(currentNode.gridPosition.x + neighbourOffset.x, currentNode.gridPosition.z + neighbourOffset.y); // ������� ������

                     if (!IsPositionInsideGrid(neighbourPosition, pathfindingGridDate.ValueRO))
                     {
                         // � ������ ���������������� �������                      
                         continue;
                     }

                     int neighbourNodeIndex = CalculateIndex(neighbourPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);

                     if (closedList.Contains(neighbourNodeIndex))
                     {
                         // ��� ������ ���� ����
                         continue;
                     }

                     PathNode neighbourNode = pathNodeTempArray[neighbourNodeIndex];
                     if (!neighbourNode.isWalkable)
                     {
                         // �����������
                         continue;
                     }

                     // ��������������� ��������� G = ������� G + ��������� ����������� �� �������� � ��������� ����
                     int tentativeGCost = currentNode.gCost + CalculateHeuristicDistance(currentNode.gridPosition, neighbourPosition);

                     if (tentativeGCost < neighbourNode.gCost)// ���� ��������������� ��������� G ������ ��������� G ��������� ����(�� ��������� ��� ����� �������� MaxValue )
                                                              //  �� - �� ����� ����� ���� ��� �� ������� � ���� �������� ����
                     {
                         neighbourNode.cameFromNodeIndex = currentNode.index;// ���������� - �� �������� ���� ������ - � �������� ���� ����
                         neighbourNode.gCost = tentativeGCost;  // ��������� �� �������� ���� ����������� �������� G
                         neighbourNode.CalculateFCost();
                         pathNodeTempArray[neighbourNodeIndex] = neighbourNode; // �.�. �� �������� � ������ �� �������� ��

                         if (!openList.Contains(neighbourNode.index))// ���� �������� ������ �� �������� ����� ��������� ���� �� ������� ���
                         {
                             openList.Add(neighbourNode.index);
                         }
                     }

                 }
             }           

             PathNode endNode = pathNodeTempArray[endNodeIndex];
             if (endNode.cameFromNodeIndex == -1)
             {
                 // �� ����� ����!
                 Debug.Log("�� ����� ����!");
             }
             else
             {
                 //����� ����
                 CalculatePath(pathNodeTempArray, endNode);
                 pathfindingParams.ValueRW.isPathfindingComplete = true;
             }

             //�������� ����� � ���� ����� �� ���������� Update
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
        //����������� ��� ���������� PathfindingParams � ����� ���������� ������ �����������
        foreach ((
            RefRW<PathfindingParams> pathfindingParams,
            RefRO<PathfindingGridDate> pathfindingGridDate,
            Entity pathfindingEntity)
            in SystemAPI.Query<
                RefRW<PathfindingParams>,
                RefRO<PathfindingGridDate>>().
                WithPresent<PathfindingGridDate>().WithEntityAccess())
        {          
            DefaultPathNodeArray defaultPathNodeArray = SystemAPI.GetSingleton<DefaultPathNodeArray>(); //���������� � �������� ������� ������� �������� ����� ���������

            ValidGridPositionPathNodeDict validGridPositionPathNodeDict = SystemAPI.GetSingleton<ValidGridPositionPathNodeDict>();
            validGridPositionPathNodeDict.dictionary.Clear();

            //������ �������� ����� (y- floo ������� �������)
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
            for (int x = -moveDistance; x <= moveDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� maxMoveDistance
            {
                for (int y = -moveDistance; y <= moveDistance; y++)
                {
                    for (int z = -moveDistance; z <= moveDistance; z++)
                    {
                        int3 offsetGridPosition = new int3(x, y, z); // ��������� �������� �������. ��� ������� ���������(0,0, floor-����) �������� ��� ���� 
                        int3 testGridPosition = startGridPosition + offsetGridPosition; // ����������� �������� �������

                        if (!IsPositionInsideGrid(testGridPosition, pathfindingGridDate.ValueRO))
                            continue; // ����������� �������� ������� ��� �����  

                        /*if (testGridPosition.Equals(startGridPosition)) // ������ �������� �.�. ���� �� ������� ����� ����� ������ ���� ���������� � ��� � �������� ����
                        {
                            Debug.Log($"{testGridPosition} -- ���������");
                            continue; // ��������� ���� � ������
                        }*/
                        // ��������� ������ ��������� ��������� ������
                        NativeArray<PathNode> pathNodeTempArray = new NativeArray<PathNode>(defaultPathNodeArray.pathNodeArray, Allocator.Temp);

                        //�������� ������ ������������ ���� � ������� ��� ����                    
                        int testNodeIndex = CalculateIndex(testGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                        PathNode testPathNode = pathNodeTempArray[testNodeIndex];

                        if (!testPathNode.isWalkable)// �����������              
                            continue;
                        //Debug.Log($"1 - �������� {testGridPosition}");

                        //�������� ��������� ���� � �������� ���
                        PathNode startNode = pathNodeTempArray[startNodeIndex]; // ������� �����
                        startNode.gCost = 0;
                        startNode.hCost = CalculateHeuristicDistance(startGridPosition, testGridPosition);
                        startNode.CalculateFCost();
                        pathNodeTempArray[startNode.index] = startNode; //����������� ����������������� �����

                        // ������ �������� ������ ������ ���� � �� ��� ����
                        NativeList<int> openList = new NativeList<int>(Allocator.Temp);     //��� ���� ������� ��������� �����
                        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);   //��� ����, � ������� ��� ��� ���������� �����

                        openList.Add(startNode.index);//������� � ������ ��������� ����

                        //�������� � ����� gCost  fCost
                        while (openList.Length > 0)// ���� � �������� ������ ���� �������� �� ��� �������� ��� ���� ���� ��� ������. ���� ����� �������� ���� �� ��������� ��� ������
                        {
                            PathNode currentNode = GetLowestCostFNode(openList, pathNodeTempArray); // ������� ���� � ���������� F (� ������ ��� startNode)
                            //Debug.Log($"2 - ���� � ���������� F - {currentNode.gridPosition}");
                            if (currentNode.index == testNodeIndex)
                            {
                                // Debug.Log($"3 - �������� ����!{currentNode.gridPosition} ");
                                break;// �������� ����!   
                            }

                            // ������� ������� ���� �� ��������� ������
                            for (int i = 0; i < openList.Length; i++)
                            {
                                if (openList[i] == currentNode.index)
                                {
                                    openList.RemoveAtSwapBack(i); //������� �� �������
                                    break;
                                }
                            }
                            // � ������� � �������� ������ // ��� �������� ��� �� ������ �� ����� ����
                            closedList.Add(currentNode.index);

                            // ��������� ���� �������� ����
                            for (int i = 0; i < neighbourOffsetArray.Length; i++)
                            {
                                int3 neighbourOffset = neighbourOffsetArray[i];
                                int3 neighbourPosition = currentNode.gridPosition + neighbourOffset;// new int3(currentNode.gridPosition.x + neighbourOffset.x, currentNode.gridPosition.z + neighbourOffset.y); // ������� ������

                                if (!IsPositionInsideGrid(neighbourPosition, pathfindingGridDate.ValueRO))
                                    continue; // � ������ ���������������� �������      

                                int neighbourNodeIndex = CalculateIndex(neighbourPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                                if (closedList.Contains(neighbourNodeIndex))
                                    continue; // ��� ������ ���� ����                        

                                PathNode neighbourNode = pathNodeTempArray[neighbourNodeIndex];
                                if (!neighbourNode.isWalkable)
                                    continue; // �����������

                                // ��������������� ��������� G = ������� G + ��������� ����������� �� �������� � ��������� ����
                                int tentativeGCost = currentNode.gCost + CalculateHeuristicDistance(currentNode.gridPosition, neighbourPosition);
                                if (tentativeGCost > (moveDistance * MOVE_STRAIGHT_COST))
                                    continue; // ������ ����


                                //Debug.Log($"4 - �����{neighbourNode.gridPosition} ");

                                if (tentativeGCost < neighbourNode.gCost)// ���� ��������������� ��������� G ������ ��������� G ��������� ����(�� ��������� ��� ����� �������� MaxValue )
                                                                         //  �� - �� ����� ����� ���� ��� �� ������� � ���� �������� ����
                                {
                                    //Debug.Log($"5 - ��������� G{tentativeGCost} �� {currentNode.gridPosition} � {neighbourNode.gridPosition} ");
                                    neighbourNode.cameFromNodeIndex = currentNode.index;// ���������� - �� �������� ���� ������ - � �������� ���� ����
                                    neighbourNode.gCost = tentativeGCost;  // ��������� �� �������� ���� ����������� �������� G
                                    neighbourNode.hCost = CalculateHeuristicDistance(neighbourNode.gridPosition, testGridPosition);
                                    neighbourNode.CalculateFCost();
                                    pathNodeTempArray[neighbourNodeIndex] = neighbourNode; // �.�. �� �������� � ������ �� �������� ��
                                    //Debug.Log($"6 - ������ ������ �������� {neighbourNode.cameFromNodeIndex} ");
                                    if (!openList.Contains(neighbourNode.index))// ���� �������� ������ �� �������� ����� ��������� ���� �� ������� ���                                    
                                        openList.Add(neighbourNode.index);
                                }
                            }
                        }

                        // Debug.Log($"7 - ��������� ���� ��� - {pathNodeTempArray[testNodeIndex].gridPosition} - ������ � -  {pathNodeTempArray[testNodeIndex].cameFromNodeIndex}");
                        CalculatePath(pathNodeTempArray, validGridPositionPathNodeDict, pathNodeTempArray[testNodeIndex]);

                        openList.Dispose();
                        closedList.Dispose();
                        pathNodeTempArray.Dispose();
                    }
                }
            }



            /*foreach (var collection in validGridPositionPathNodeDict.dictionary)
            {
                Debug.Log($"���� ����� - {collection.Key}");
                for (int index = 0; index < validGridPositionPathNodeDict.dictionary[collection.Key].pathWorldPositionList.Length; index++)
                {
                    Debug.Log($"������ - {index} - ���� - {validGridPositionPathNodeDict.dictionary[collection.Key].pathWorldPositionList[index]}");
                }
            }  */

            //�������������� �� ���� �.�. Native Collections - ��� ������ ������ �� ������ ������
            /* //������� �������� � ����������� � ��� validGridPositionPathNodeDict
             Entity pathNodeSystemEntity = SystemAPI.GetSingletonEntity<ValidGridPositionPathNodeDict>();
             state.EntityManager.SetComponentData(pathNodeSystemEntity, validGridPositionPathNodeDict);*/

            //�������� ������� ���� ��������
            pathfindingParams.ValueRW.onPathfindingComplete = true;
            Debug.Log($"onPathfindingComplete = true");

            //�������� ����� � ���� ����� �� ���������� Update
            SystemAPI.SetComponentEnabled<PathfindingParams>(pathfindingEntity, false);
            neighbourOffsetArray.Dispose();
        }
    }


    /// <summary>
    /// ���������� ���� � �������� � ����<br/>
    /// ��������� ����������������� �������� ���� � "configuredPathNodeArray"
    /// </summary>
    private static void CalculatePath(NativeArray<PathNode> pathNodeTempArray, ValidGridPositionPathNodeDict validNodeForPath, PathNode endNode)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // �� ������� ����� ����!
            // Debug.Log($"�� ����� ����!{endNode.gridPosition}");
        }
        else
        {
            //����� ����. ������ � ����� � �������� ������   
            endNode.pathWorldPositionList.Clear(); //������� �.�. ���������� ����� ���������� � ������ ���� ����� ��������� ����
            endNode.pathWorldPositionList.Add(endNode.worldPosition);
            PathNode currentNode = endNode;

            while (currentNode.cameFromNodeIndex != -1)//� ����� �������� ���� �� ������ ���� � �������� �� ���� ������
            {
                PathNode cameFromNode = pathNodeTempArray[currentNode.cameFromNodeIndex];//�� ������� ������ ���� � �������� ������
                endNode.pathWorldPositionList.Add(cameFromNode.worldPosition);
                currentNode = cameFromNode;
            }

            //������ ��������� ������� �.�. ��� ��������� ������� ����� � �� ��� �� ���� ������ �.�. �� ��� �� ���
            endNode.pathWorldPositionList.RemoveAtSwapBack(endNode.pathWorldPositionList.Length - 1);

            /* foreach (float3 worldPosition in endNode.pathWorldPositionList)
             {
                 Debug.Log($"����� ���� ����� ���������� - {worldPosition}");
             }*/

            validNodeForPath.dictionary.Add(endNode.gridPosition, endNode);

            //  Debug.Log($"{validNodeForPath.dictionary.Count()}");          
        }
    }
    /* /// <summary>
     /// ���������� ����
     /// </summary>
     private static NativeList<int3> CalculatePath(NativeArray<PathNode> pathNodeTempArray, PathNode endNode)
     {
         if (endNode.cameFromNodeIndex == -1)
         {
             // �� ������� ����� ����!
             return new NativeList<int3>(Allocator.Temp); //������ ������� ������
         }
         else
         {
             // ����� ����
             NativeList<int3> path = new NativeList<int3>(Allocator.Temp);
             path.Add(endNode.gridPosition);
             //������ � ����� � ��������� ������ (�������� ������ ����). ����� ������ �� ��������� ������� ���� ������������ �.�. � ��� cameFromNodeIndex=(-1)
             PathNode currentNode = endNode;
             while (currentNode.cameFromNodeIndex != -1) //� ����� �������� ���� �� ������ ���� � �������� �� ���� ������
             {
                 PathNode cameFromNode = pathNodeTempArray[currentNode.cameFromNodeIndex]; //�� ������� ������ ���� � �������� ������
                 path.Add(cameFromNode.gridPosition);
                 currentNode = cameFromNode; // ���� � �������� ������ ����������� �������
             }

             return path;
         }
     }*/

    /// <summary>
    /// ������� ������ �����?
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
    /// ��������� �������������(���������������) ���������� ����� ������ 2�� ������ ��� ����� �������� "H".<br/>
    /// ��� endGridPosition H=0.
    /// </summary>
    private static int CalculateHeuristicDistance(int3 gridPositionA, int3 gridPositionB) // ��������� �������������(���������������) ���������� ����� ������ 2�� ��������� ��������� ��� ����� �������� "H"
    {
        int3 gridPositionDistance = gridPositionA - gridPositionB; //����� ���� ������������� (��������� ���������� ��������� ����� ������ � ������ ������ - �� ������� ���� �������� ������������ ������ �����)

        // ��� �������� ������ �� ������ (����� ������������ ��� ������ ����� ������)
        //int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.y); // ������� ����� ��������� �� ������ (�� ��������� �������� �� ��������� ������ �� ������. �������� �� ����� (0,0) � ����� (3,2) �������� ��� ���� � ����� � ��� ���� ����� ����� 5)

        int xDistsnce = math.abs(gridPositionDistance.x); //����������� � (0,0) �� (1,1) ����� �������� 1 ����������, ������ ���� ����������� �� ������ // ���� � (0,0) �� (2,1) �� ��������� ����� ��� ����� ����. ������� ��� ������� ���������� ���������� ���� ����� ����������� ����� �� ���������� (x,y)
        int zDistsnce = math.abs(gridPositionDistance.z);
        int remaining = math.abs(xDistsnce - zDistsnce); // ���������� ��������� ����� ������������ �� ������, ����� �� ������
        int CalculateDistance = MOVE_DIAGONAL_COST * math.min(xDistsnce, zDistsnce) + MOVE_STRAIGHT_COST * remaining; // ������ ����������� ���������� � ������� GridPositionXZ ��� ����� �������� �� ��������� � �������� �� ������
        return CalculateDistance;
    }

    /// <summary>
    /// �������� ���� ���� � ����� ������ ���������� F <br/>
    /// ���� ��������� ����� ����� ���������� ���������� F �� �������� ������ ���������� � ������
    /// </summary>
    private static PathNode GetLowestCostFNode(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]]; //���� ���� � ���������� ���������� 
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
    /// �������� ������ ���� (����������� ���������� ���� � ������� ������, ��� �� ���� �� ����������� ������� [x, floor, z] ).<br/>
    /// �������� � ����
    /// </summary>
    private static int CalculateIndex(int x, int floor, int z, int gridWidthXSize, int floorAmount)// https://stackoverflow.com/questions/51157907/calculating-3d-coordinates-from-a-flattened-3d-array
    {
        return x + floor * gridWidthXSize + z * gridWidthXSize * floorAmount;
    }
    /// <summary> 
    /// �������� ������ ���� (����������� ���������� ���� � ������� ������, ��� �� ���� �� ����������� ������� [x, floor, z] ).<br/>
    /// �������� � ����
    /// </summary>
    private static int CalculateIndex(int3 gridPosition, int gridWidthXSize, int floorAmount)// https://stackoverflow.com/questions/51157907/calculating-3d-coordinates-from-a-flattened-3d-array
    {
        return gridPosition.x + gridPosition.y * gridWidthXSize + gridPosition.z * gridWidthXSize * floorAmount;
    }
    /// <summary>
    /// �������� ������� ���������� ��� ������ ������ �����.<br/>
    /// (������ ���������� ������ ������)
    /// </summary>
    private static float3 GetWorldPosition(int3 gridPosition, float cellSize, float3 anchorGrid, float floorHeight) // �������� ������� ���������
    {
        return new float3(
            gridPosition.x * cellSize, // ����� ������ ������
            gridPosition.y * floorHeight, // ����� ���� � ������ �����
            gridPosition.z * cellSize)
            + anchorGrid; //������� �������� ����� ����� � ������� ������������
    }
}
