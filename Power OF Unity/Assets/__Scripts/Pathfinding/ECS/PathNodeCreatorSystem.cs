using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

/// <summary>
/// ������� �������� ����� ����
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
        // ����������� 1 ���� ����� ������� PathfindingGridDate
        foreach (RefRO<PathfindingGridDate> pathfindingGridDate in SystemAPI.Query<RefRO<PathfindingGridDate>>())
        {
            // � ����� �������� ��� ������ �� ����������� ������������, ����� �������� ����� �� ������ ������� � ��� ������ ������� ��� ���� �������������
            // � ���� 1�� ��� ����� � ��� �� ������� ���������� � 2��� ��� �������� �� ����������� _obstaclesDoorMousePlaneCoverLayerMask,
            // ���� ��� ���� ��������� ��� ������ ����� �� ����������
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;// ��� �������� ����      

            CollisionFilter collisionMousePlaneFilter = new CollisionFilter
            {//https://discussions.unity.com/t/how-do-i-use-layermasks/481 -������������ �������� ������
                BelongsTo = ~0u, // ��� ����� ������������ ������� ���� ������� ����������� ��� "0" � ������� ��������� ���������� �������������� (~) � ������������ � uint � ������� "u"
                CollidesWith = 1u << 6,// ������� ����� � 6 - MousePlane ����� ����� ������� ������������� ������� ������� ����� ��� ��� (����� �����������)
                GroupIndex = 0,
            };
            CollisionFilter collisionObstaclesCoverFilter = new CollisionFilter
            {
                BelongsTo = ~0u, 
                CollidesWith = 1u << 8 | 1u << 11,// ������� ����� � 8 - Obstacles � 11 - Cover ����� ����� ������� ������������� ������� ������� ����� (����� �����������)
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
                        pathNode.cameFromNodeIndex = -1; //��������� ������������ ��������

                        pathNode.isInAir = false; //�� ��������� ���� �� � �������
                        pathNode.isWalkable = false; //�� ��������� ������� ������������

                        //��������� ���. // ��� ������� ���������� ��� �� ��� ��������� �������� ����� �������� ������ ����,  ������� ������� ��� �����,
                        //� ����� �������� ����, ����� ������ �������� � ��� ���� ������ ��� �������� ����, ��� ����� ����������������� � ��������� ������ ����
                        RaycastInput raycastInput = new RaycastInput
                        {
                            Start = pathNode.worldPosition + new int3(0, 1, 0),
                            End = pathNode.worldPosition + new int3(0, -1, 0),
                            Filter = collisionMousePlaneFilter
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
                            Filter = collisionObstaclesCoverFilter
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
}
