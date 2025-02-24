using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// ������� ������ ����. ����� ����������� ��� ��������� PathfindingParams
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //�������� � ������
partial struct PathfindingSystem : ISystem
{
    private const int MOVE_STRAIGHT_COST = 10;// ��������� �������� ����� (��� �������� ����� 10 � �� 1 ��� �� �� ������������ float)
    private const int MOVE_DIAGONAL_COST = 14;// ��������� �������� �� ��������� ( ������������� �� ������� �������� ������ ���������� �� ����� ��������� ������� �������������� ������������. � ����� ��� �������� ����� 14 � �� 1,4 ��� �� �� ������������ float)


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //����������� ��� ���������� PathfindingParams    
        foreach ((
            RefRO<PathfindingParams> pathfindingParams,
            RefRO<PathfindingGridDate> pathfindingGridDate,
            Entity entity)
            in SystemAPI.Query<
                RefRO<PathfindingParams>,
                RefRO<PathfindingGridDate>>().WithEntityAccess())
        {

            PathNodeDefaultArray pathNodeDefaultArray = SystemAPI.GetSingleton<PathNodeDefaultArray>(); //���������� � �������� ������� ������� �������� ����� ���������
            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(pathNodeDefaultArray.pathNodeArray, Allocator.Temp); //��������� ������
            int3 startPosition = pathfindingParams.ValueRO.startPosition;
            int3 endPosition = pathfindingParams.ValueRO.endPosition;
                       

            //����������� hCost � ���� ����� ��� �������� endPosition. � ������� cameFromNodeIndex 
            for (int i = 0; i < pathNodeArray.Length; i++)
            {
                PathNode pathNode = pathNodeArray[i];
                pathNode.hCost = CalculateHeuristicDistance(pathNode.gridPosition, endPosition);
                // pathNode.cameFromNodeIndex = -1; // ����� �� ����� �.�. �� ������ ��������  ��������� ������

                pathNodeArray[i] = pathNode;
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

            int endNodeIndex = CalculateIndex(endPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount); //�������� ������ ��������� ����

            PathNode startNode = pathNodeArray[CalculateIndex(startPosition, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            // ������ �������� ������ ������ ���� � �� ��� ����
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);     //��� ���� ������� ��������� �����
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);   //��� ����, � ������� ��� ��� ���������� �����

            openList.Add(startNode.index);//������� � ������ ��������� ����
           
            //�������� � ����� gCost  fCost
            while (openList.Length > 0)// ���� � �������� ������ ���� �������� �� ��� �������� ��� ���� ���� ��� ������. ���� ����� �������� ���� �� ��������� ��� ������
            {
                PathNode currentNode = GetLowestCostFNode(openList, pathNodeArray); // ������� ���� � ���������� F (� ������ ��� startNode)

                if (currentNode.index == endNodeIndex)
                {
                    // �������� ����!
                    break;
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

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
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
                        pathNodeArray[neighbourNodeIndex] = neighbourNode; // �.�. �� �������� � ������ �� �������� ��
                      
                        if (!openList.Contains(neighbourNode.index))// ���� �������� ������ �� �������� ����� ��������� ���� �� ������� ���
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
                // �� ����� ����!
                Debug.Log("�� ����� ����!");
                // pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = -1 };
            }
            else
            {
                //����� ����
                CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
                // pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1 };
            }

            //�������� ����� � ���� ����� �� ���������� Update
            SystemAPI.SetComponentEnabled<PathfindingParams>(entity, false);
        }
    }

    



    /// <summary>
    /// ���������� ���� � �������� � ���������� ������
    /// </summary>
    private static void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode, DynamicBuffer<PathPositionBufferElement> pathPositionBuffer)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // �� ������� ����� ����!
        }
        else
        {
            //����� ����
            //������ � ����� � �������� ������
            pathPositionBuffer.Add(new PathPositionBufferElement { gridPosition = endNode.gridPosition });

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)//� ����� �������� ���� �� ������ ���� � �������� �� ���� ������
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];//�� ������� ������ ���� � �������� ������
                pathPositionBuffer.Add(new PathPositionBufferElement { gridPosition = cameFromNode.gridPosition });
                currentNode = cameFromNode;
            }
        }
    }
    /// <summary>
    /// ���������� ����
    /// </summary>
    private static NativeList<int3> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
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
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex]; //�� ������� ������ ���� � �������� ������
                path.Add(cameFromNode.gridPosition);
                currentNode = cameFromNode; // ���� � �������� ������ ����������� �������
            }

            return path;
        }
    }

    /// <summary>
    /// ������� ������ �����?
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
    /// ��������� �������������(���������������) ���������� ����� ������ 2�� ������ ��� ����� �������� "H"
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
