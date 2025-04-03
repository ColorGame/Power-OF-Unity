using Unity.Entities;
using Unity.Mathematics;


/// <summary>
/// ������� ������� - ������� ������� ����
/// </summary>
[InternalBufferCapacity(20)]//���������� ��������� � ������
public struct PathWorldPositionBufferElement : IBufferElementData
{
    public float3 worldPosition;

    //��������� ����������� ������� �������� ������� ��������� �������� ������� �
    //���������� ����� ������� ������, ��������� �������� �������� = �������� ������� �� ��������
    public static implicit operator PathWorldPositionBufferElement(float3 worldPosition)
    {
        return new PathWorldPositionBufferElement { worldPosition = worldPosition };
    }
}