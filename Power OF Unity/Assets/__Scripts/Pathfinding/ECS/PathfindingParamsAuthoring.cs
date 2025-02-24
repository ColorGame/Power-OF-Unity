using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// ��������� ������ ����
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
            SetComponentEnabled<PathfindingParams>(entity, false); // �� ��������� �������
        }
    }
}
/// <summary>
/// ��������� ������ ����.<br/>
/// ��� ��������� ������ ���� ���� �������� ���� ���������, � �������� ������ ��� ������.<br/>
/// ��� ���������� ������� ������� ����, ���� ��������� � onPathComplete ���������� ������� (isPathComplete==true),<br/>
/// � ����� ����� � onPathComplete �������� ������ ������� ����.
/// </summary>
public struct PathfindingParams : IComponentData, IEnableableComponent
{
    public int3 startPosition;
    public int3 endPosition;
    public bool onPathComplete;


   /* /// <summary>
    /// ������� ���� ��������. ����� �������� ������ ������� ����
    /// </summary>
    public struct OnPathCompleteEvent
    {
        public bool isPathComplete;
        public DynamicBuffer<PathPositionBufferElement> pathPositionBuffer;
    }*/
}
