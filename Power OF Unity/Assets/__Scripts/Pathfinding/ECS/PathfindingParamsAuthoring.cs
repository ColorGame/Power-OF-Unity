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
            AddBuffer<PathPositionBufferElement>(entity);
            SetComponentEnabled<PathfindingParams>(entity, false); // По умолчанию выключу
        }
    }
}
/// <summary>
/// Параметры поиска пути.<br/>
/// Для активации поиска пути надо включить этот компонент, и передать данные для поиска.<br/>
/// Для заполнения Буффера позиций пути, надо дождаться в onPathComplete завершения расчета (isPathComplete==true),<br/>
/// и после можно в onPathComplete забирать Буффер позиций пути.
/// </summary>
public struct PathfindingParams : IComponentData, IEnableableComponent
{
    public int3 startPosition;
    public int3 endPosition;
    public bool onPathComplete;


   /* /// <summary>
    /// Событие ПУТЬ РАСЧИТАН. Можно забирать Буффер позиций пути
    /// </summary>
    public struct OnPathCompleteEvent
    {
        public bool isPathComplete;
        public DynamicBuffer<PathPositionBufferElement> pathPositionBuffer;
    }*/
}
