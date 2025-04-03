using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

/// <summary>
/// ������� ����� ����.<br/>
/// ������� ���� � ������ ������ �����, � ����� �������� � ������� "PathWorldPositionBufferElement"
/// </summary>
partial struct PathNodeSystem : ISystem
{       
    private CollisionFilter _collisionMousePlaneFilter;
    private CollisionFilter _collisionObstaclesCoverFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponent<DefaultPathNodeArray>(state.SystemHandle);
        state.EntityManager.AddComponent<ValidGridPositionPathNodeDict>(state.SystemHandle);

        _collisionMousePlaneFilter = new CollisionFilter
        {//https://discussions.unity.com/t/how-do-i-use-layermasks/481 -������������ �������� ������
            BelongsTo = ~0u, // ��� ����� ������������ ������� ���� ������� ����������� ��� "0" � ������� ��������� ���������� �������������� (~) � ������������ � uint � ������� "u"
            CollidesWith = 1u << 6,// ������� ����� � 6 - MousePlane ����� ����� ������� ������������� ������� ������� ����� ��� ��� (����� �����������)
            GroupIndex = 0,
        };
        _collisionObstaclesCoverFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << 8 | 1u << 11,// ������� ����� � 8 - Obstacles � 11 - Cover ����� ����� ������� ������������� ������� ������� ����� (����� �����������)
            GroupIndex = 0,
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // ����������� 1 ���� ����� ������� PathfindingGridDate
        foreach ((
            RefRW<PathfindingGridDate> pathfindingGridDate,
            Entity entity)
            in SystemAPI.Query<RefRW<PathfindingGridDate>>().WithEntityAccess())
        {
            // � ����� �������� ��� ������ �� ����������� ������������, ����� �������� ����� �� ������ ������� � ��� ������ ������� ��� ���� �������������
            // � ���� 1�� ��� ����� � ��� �� ������� ���������� � 2��� ��� �������� �� ����������� _obstaclesDoorMousePlaneCoverLayerMask,
            // ���� ��� ���� ��������� ��� ������ ����� �� ����������
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;// ��� �������� ����                              

            NativeArray<PathNode> pathNodeDefaultArray = new NativeArray<PathNode>(pathfindingGridDate.ValueRO.width * pathfindingGridDate.ValueRO.height * pathfindingGridDate.ValueRO.floorAmount, Allocator.Persistent);

            for (int x = 0; x < pathfindingGridDate.ValueRO.width; x++)
            {
                for (int z = 0; z < pathfindingGridDate.ValueRO.height; z++)
                {
                    for (int flooor = 0; flooor < pathfindingGridDate.ValueRO.floorAmount; flooor++)
                    {
                        // Entity pathNodeEntity = state.EntityManager.CreateEntity(typeof(PathNode)); //�������� �������� ���� PathNode
                        PathNode pathNode = new PathNode(); // �������� � �������������� ����

                        pathNode.gridPosition = new int3(x, flooor, z);
                        pathNode.worldPosition = GetWorldPosition(pathNode.gridPosition, pathfindingGridDate.ValueRO.cellSize, pathfindingGridDate.ValueRO.anchorGrid, pathfindingGridDate.ValueRO.floorHeight);
                        pathNode.index = CalculateIndex(x, flooor, z, pathfindingGridDate.ValueRO.width, pathfindingGridDate.ValueRO.floorAmount);
                        pathNode.gCost = int.MaxValue;
                        pathNode.hCost = default;
                        pathNode.fCost = default;
                        pathNode.cameFromNodeIndex = -1; //��������� ������������ ��������

                        pathNode.isInAir = false; //�� ��������� ���� �� � �������
                        pathNode.isWalkable = false; //�� ��������� ������� ������������

                        pathNode.pathWorldPositionList = new NativeList<float3>(Allocator.Persistent);


                        //��������� ���. // ��� ������� ���������� ��� �� ��� ��������� �������� ����� �������� ������ ����,  ������� ������� ��� �����,
                        //� ����� �������� ����, ����� ������ �������� � ��� ���� ������ ��� �������� ����, ��� ����� ����������������� � ��������� ������ ����
                        RaycastInput raycastInput = new RaycastInput
                        {
                            Start = pathNode.worldPosition + new int3(0, 1, 0),
                            End = pathNode.worldPosition + new int3(0, -1, 0),
                            Filter = _collisionMousePlaneFilter
                        };

                        if (collisionWorld.CastRay(raycastInput)) //���� ������ � ��� ��������� ���� ���� ����������
                        {
                            pathNode.isWalkable = true;
                        }
                        else // ���� ��� ������� ��� ���� �� ��������� ���� � �������
                        {
                            pathNode.isInAir = true;
                            // Debug.Log($"{pathNode.gridPosition}� �������");
                        }

                        //��������� ���. ��� �� ����� �������� ������ ���������(������ �����), ������� ������� ��� ����, � ����� �������� �����, ����� ������ �������� � ��� ���� ������ ��� �������� ����, ��� ����� ����������������� � ��������� ������ ����
                        // ����� ������� ��������� � UNITY � �� ������� ������� ����. Project Settings/Physics/Queries Hit Backfaces - ��������� �������, � ����� ����� �������� �� ����� ���������
                        raycastInput = new RaycastInput
                        {
                            Start = pathNode.worldPosition + new int3(0, -1, 0),
                            End = pathNode.worldPosition + new int3(0, 1, 0),
                            Filter = _collisionObstaclesCoverFilter
                        };

                        if (collisionWorld.CastRay(raycastInput)) // ���� ��� ����� � ����������� ��� � ������� �� ��������� ������ �� ����������
                        {
                            pathNode.isWalkable = false;
                            //Debug.Log($"{pathNode.gridPosition}����������");
                        }

                        pathNodeDefaultArray[pathNode.index] = pathNode;
                    }
                }
            }

            state.EntityManager.SetComponentData(state.SystemHandle, new DefaultPathNodeArray
            {
                pathNodeArray = pathNodeDefaultArray
            });

            state.EntityManager.SetComponentData(state.SystemHandle, new ValidGridPositionPathNodeDict
            {
                //������ ������� ������� �������� ��� � ��� �����. ����� �� ����� �� ������� ������� ������� (���� ��� ������ ��������� ��� ����)
                dictionary = new NativeParallelHashMap<int3, PathNode>(pathNodeDefaultArray.Length, Allocator.Persistent)
            });

            //  state.Enabled = false;
            //�������� ����� � ���� ����� �� ���������� Update
            SystemAPI.SetComponentEnabled<PathfindingGridDate>(entity, false);
        }
    }


    [BurstCompile]
    public
    void OnDestroy(ref SystemState state)
    {      
        RefRW<DefaultPathNodeArray> defaultPathNodeArray = state.EntityManager.GetComponentDataRW<DefaultPathNodeArray>(state.SystemHandle);//������� ������ ������
        foreach (PathNode pathNode in defaultPathNodeArray.ValueRW.pathNodeArray)
        {
            pathNode.pathWorldPositionList.Dispose();          
        }
        defaultPathNodeArray.ValueRW.pathNodeArray.Dispose();     

        RefRW<ValidGridPositionPathNodeDict> validNodeForPath = state.EntityManager.GetComponentDataRW<ValidGridPositionPathNodeDict>(state.SystemHandle);//������� ������ ������
        validNodeForPath.ValueRW.dictionary.Dispose();
    }


    /// <summary> 
    /// �������� ������ ���� (����������� ���������� ���� � ������� ������, ��� �� ���� �� ����������� �������[x,floor, z]).<br/>
    /// �������� � ����
    /// </summary>
    private static int CalculateIndex(int x, int floor, int z, int gridWidthXSize, int floorAmount)// https://stackoverflow.com/questions/51157907/calculating-3d-coordinates-from-a-flattened-3d-array
    {
        return x + floor * gridWidthXSize + z * gridWidthXSize * floorAmount;
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


    /// <summary>
    /// ������ ���� ����� ���� � ��������� ���������.<br/> �������� ���� �������� � �������� PathNodeSystem
    /// </summary>
    public struct DefaultPathNodeArray : IComponentData
    {
        public NativeArray<PathNode> pathNodeArray;
    }

    /// <summary>
    /// ������� - ���������� ������� ����� � ���� ����<br/>
    /// �������� ���� �������� � �������� PathNodeSystem
    /// </summary>
    public struct ValidGridPositionPathNodeDict : IComponentData
    {      
        public NativeParallelHashMap<int3, PathNode> dictionary;
    }
}
