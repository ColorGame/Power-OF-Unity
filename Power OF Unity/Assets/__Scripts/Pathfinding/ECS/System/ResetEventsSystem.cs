using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static PathNodeSystem;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)] // Изменим порядок выполнения системы . Поместим в группу LateSimulationSystemGroup в конец списка
partial struct ResetEventsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<PathfindingParams> pathfindingParams in SystemAPI.Query<RefRW<PathfindingParams>>().WithPresent<PathfindingParams>())
        {
            pathfindingParams.ValueRW.onPathfindingComplete = false;
          //  Debug.Log($"onPathfindingComplete = false");
        }

        foreach (RefRW<ValidGridPositionPointsPathDict> validGridPositionPathNodeDict in SystemAPI.Query<RefRW<ValidGridPositionPointsPathDict>>())
        {
            validGridPositionPathNodeDict.ValueRW.onRegister = false;
          //  Debug.Log($"onRegister = false");
        }
    }
}
