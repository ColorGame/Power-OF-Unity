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
           // AddBuffer<ValidNodeForPathBufferElement>(entity);
           // AddBuffer<PathWorldPositionBufferElement>(entity);
            SetComponentEnabled<PathfindingParams>(entity, false); // По умолчанию выключу
        }
    }
}
/// <summary>
/// Параметры поиска пути.<br/>
/// Для активации поиска пути надо включить этот компонент, и передать данные для поиска.<br/>
/// PathfindingProvider - настраивает этот компонент
/// </summary>
public struct PathfindingParams : IComponentData, IEnableableComponent
{
    public int3 startGridPosition;      
    /// <summary>
    /// Максимальная дистанция движения в сетке.<br/>
    /// G-параментр.
    /// </summary>
    public int maxMoveDistance; 
}
