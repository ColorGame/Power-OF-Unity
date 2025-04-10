using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// Параметры поиска пути
/// </summary>
public class PathfindingParamsAuthoring : MonoBehaviour
{
    public class Baker : Baker<PathfindingParamsAuthoring>
    {
        public override void Bake(PathfindingParamsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PathfindingParams());
            AddBuffer<UnlockPositionBufferElement>(entity);
            AddBuffer<LockPositionBufferElement>(entity);
            //По умолчанию выключу компоненты
            SetComponentEnabled<PathfindingParams>(entity, false); 
            SetComponentEnabled<UnlockPositionBufferElement>(entity, false); 
            SetComponentEnabled<LockPositionBufferElement>(entity, false); 
        }
    }
}
/// <summary>
/// Параметры поиска пути.<br/>
/// Для активации поиска пути надо включить этот компонент, и передать данные для поиска.<br/>
/// PathfindingSystem - настраивает этот компонент и включает его.<br/>
/// PathfindingProviderSystem - считывает и выключает
/// </summary>
public struct PathfindingParams : IComponentData, IEnableableComponent
{
    public int3 startGridPosition;      
    /// <summary>
    /// Максимальная дистанция движения в сетке.<br/>
    /// G-параментр.
    /// </summary>
    public int maxMoveDistance;
    /// <summary>
    /// Событие путь расчитан
    /// </summary>
    public bool onPathfindingComplete;
}
