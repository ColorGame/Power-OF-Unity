using Unity.Collections;
using Unity.Entities;
/// <summary>
/// ������ ���� ����� ���� �� ���������.<br/> �������� ���� �������� � �������� PathNodeCreatorSystem
/// </summary>
public struct PathNodeDefaultArray : IComponentData
{   
    public NativeArray<PathNode> pathNodeArray;
}
