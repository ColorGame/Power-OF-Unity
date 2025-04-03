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
           // AddBuffer<ValidNodeForPathBufferElement>(entity);
           // AddBuffer<PathWorldPositionBufferElement>(entity);
            SetComponentEnabled<PathfindingParams>(entity, false); // �� ��������� �������
        }
    }
}
/// <summary>
/// ��������� ������ ����.<br/>
/// ��� ��������� ������ ���� ���� �������� ���� ���������, � �������� ������ ��� ������.<br/>
/// PathfindingProvider - ����������� ���� ���������
/// </summary>
public struct PathfindingParams : IComponentData, IEnableableComponent
{
    public int3 startGridPosition;      
    /// <summary>
    /// ������������ ��������� �������� � �����.<br/>
    /// G-���������.
    /// </summary>
    public int maxMoveDistance; 
}
