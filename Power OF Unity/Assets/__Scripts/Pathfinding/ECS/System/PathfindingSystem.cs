using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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

    private NativeArray<int3> _neighbourOffsetArray;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //������ �������� ����� (y- floo ������� �������)
        _neighbourOffsetArray = new NativeArray<int3>(8, Allocator.Persistent);
        _neighbourOffsetArray[0] = new int3(-1, 0, 0); // Left
        _neighbourOffsetArray[1] = new int3(+1, 0, 0); // Right
        _neighbourOffsetArray[2] = new int3(0, 0, +1); // Up
        _neighbourOffsetArray[3] = new int3(0, 0, -1); // Down
        _neighbourOffsetArray[4] = new int3(-1, 0, -1); // Left Down
        _neighbourOffsetArray[5] = new int3(-1, 0, +1); // Left Up
        _neighbourOffsetArray[6] = new int3(+1, 0, -1); // Right Down
        _neighbourOffsetArray[7] = new int3(+1, 0, +1); // Right Up
    }

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

            ValidGridPositionPointsPathDict validGridPositionPointsPathDict = SystemAPI.GetSingleton<ValidGridPositionPointsPathDict>();
            validGridPositionPointsPathDict.dictionary.Clear();

            //������������ ������ ��������� ������� - ����� ��� ���������� �������, ����� ���������� ���� ����� 
            NativeList<FindPathJob> findPathJobList = new NativeList<FindPathJob>(Allocator.Temp);
            //������ �������� �������, �� ��� � ���� ����������� ���������� �������
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);

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

                        //�������� ������� ������ ���� ��� ������ ����������� �������
                        FindPathJob findPathJob = new FindPathJob
                        {
                            pathNodeTempArray = new NativeArray<PathNode>(defaultPathNodeArray.pathNodeArray, Allocator.TempJob),//�������� ����� ������� ����� �������������
                            neighbourOffsetArray = _neighbourOffsetArray,
                            startNodeIndex = startNodeIndex,
                            testNodeIndex = CalculateIndex(testGridPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount),
                            startGridPosition = startGridPosition,
                            testGridPosition = testGridPosition,
                            pathfindingGridDate = pathfindingGridDate.ValueRO,
                            moveDistance = moveDistance
                        };

                        findPathJobList.Add(findPathJob);
                        jobHandleList.Add(findPathJob.Schedule(state.Dependency));
                    }
                }
            }
            //�������� ���������� ���� �������
            JobHandle.CompleteAll(jobHandleList.ToArray(Allocator.Temp));
            //� �������� ������ �������� ����� ���� ��� ������� ���� ���������� ��� ������������
            foreach (FindPathJob findPathJob in findPathJobList)
            {
                int testNodeIndex = findPathJob.testNodeIndex;
                CalculatePath(
                    findPathJob.pathNodeTempArray,//����������������� ������ � �������
                    defaultPathNodeArray.pointsPathArray[testNodeIndex],//������ �� ����� ���� ��� ������������� ����
                    validGridPositionPointsPathDict.dictionary,//������� ������� ����� ���������
                    testNodeIndex);
            }    

            /* foreach (var collection in validGridPositionPointsPathDict.dictionary)
             {
                 Debug.Log($"���� ����� - {collection.Key}");
                 for (int index = 0; index < validGridPositionPointsPathDict.dictionary[collection.Key].pathWorldPositionList.Length; index++)
                 {
                     Debug.Log($"������ - {index} - ���� - {validGridPositionPointsPathDict.dictionary[collection.Key].pathWorldPositionList[index]}");
                 }
             }*/

            //�������������� �� ���� �.�. Native Collections - ��� ������ ������ �� ������ ������
            /* //������� �������� � ����������� � ��� validGridPositionPointsPathDict
             Entity pathNodeSystemEntity = SystemAPI.GetSingletonEntity<ValidGridPositionPointsPathDict>();
             state.EntityManager.SetComponentData(pathNodeSystemEntity, validGridPositionPointsPathDict);*/

            //�������� ������� ���� ��������
            pathfindingParams.ValueRW.onPathfindingComplete = true;
            //  Debug.Log($"onPathfindingComplete = true");

            //�������� ����� � ���� ����� �� ���������� Update
            SystemAPI.SetComponentEnabled<PathfindingParams>(pathfindingEntity, false);
            findPathJobList.Dispose();
            jobHandleList.Dispose();
        }
    }


    [BurstCompile]
    public
    void OnDestroy(ref SystemState state)
    {
        _neighbourOffsetArray.Dispose();
    }

    /// <summary>
    /// ������� - ����� ����
    /// </summary>
    [BurstCompile]
    private partial struct FindPathJob : IJobEntity
    {
        /// <summary>
        /// ���������
        /// </summary>
        public NativeArray<PathNode> pathNodeTempArray;
       [ReadOnly] public NativeArray<int3> neighbourOffsetArray;

        public int startNodeIndex;
        public int testNodeIndex;
        public int3 startGridPosition;
        public int3 testGridPosition;

        public PathfindingGridDate pathfindingGridDate;
        public int moveDistance;


        public void Execute()
        {
            //�������� ������ ������������ ���� � ������� ��� ����      
            PathNode testPathNode = pathNodeTempArray[testNodeIndex];

            if (!testPathNode.isWalkable)// �����������              
                return;
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
                                                                                        //Debug.Log($"2 - ���� � ���������� F - {currentNodeJobe.gridPosition}");
                if (currentNode.index == testNodeIndex)
                {
                    // Debug.Log($"3 - �������� ����!{currentNodeJobe.gridPosition} ");
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
                    int3 neighbourPosition = currentNode.gridPosition + neighbourOffset;// new int3(currentNodeJobe.gridPosition.x + neighbourOffset.x, currentNodeJobe.gridPosition.z + neighbourOffset.y); // ������� ������

                    if (!IsPositionInsideGrid(neighbourPosition, pathfindingGridDate))
                        continue; // � ������ ���������������� �������      

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition, pathfindingGridDate.width, pathfindingGridDate.floorAmount);
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
                        //Debug.Log($"5 - ��������� G{tentativeGCost} �� {currentNodeJobe.gridPosition} � {neighbourNode.gridPosition} ");
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
            openList.Dispose();
            closedList.Dispose();
        }
    }



    /// <summary>
    /// ���������� ���� � �������� � PointsPath(����� ����)<br/>
    /// ��������� ����������������� �������� ���� � "configuredPathNodeArray"
    /// </summary>
    private static void CalculatePath(NativeArray<PathNode> pathNodeTempArray, PointsPath pointsPath, NativeHashMap<int3, PointsPath> validGridPositionPathNodeDict, int testNodeIndex)
    {
        PathNode endNode = pathNodeTempArray[testNodeIndex];

        if (endNode.cameFromNodeIndex == -1)
        {
            // �� ������� ����� ����!
            // Debug.Log($"�� ����� ����!{endNodeJobe.gridPosition}");
        }
        else
        {
            //����� ����. ������ � ����� � �������� ������   
            pointsPath.worldPositionList.Clear();//������� �.�. ���������� ����� ���������� � ������ ���� ����� ��������� ����
            pointsPath.worldPositionList.Add(endNode.worldPosition);

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)//� ����� �������� ���� �� ������ ���� � �������� �� ���� ������
            {
                PathNode cameFromNode = pathNodeTempArray[currentNode.cameFromNodeIndex];//�� ������� ������ ���� � �������� ������
                pointsPath.worldPositionList.Add(cameFromNode.worldPosition);
                currentNode = cameFromNode;
            }

            //������ ��������� ������� �.�. ��� ��������� ������� ����� � �� ��� �� ���� ������ �.�. �� ��� �� ���
            pointsPath.worldPositionList.RemoveAtSwapBack(pointsPath.worldPositionList.Length - 1);

            /* foreach (float3 worldPosition in endNodeJobe.pathWorldPositionList)
             {
                 Debug.Log($"����� ���� ����� ���������� - {worldPosition}");
             }*/

            validGridPositionPathNodeDict.Add(endNode.gridPosition, pointsPath);

            //  Debug.Log($"{validNodeForPath.dictionary.Count()}");          
        }
    }   

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
