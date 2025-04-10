using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// ������� ������� - ������� ��� ���������� ���� ����
/// </summary>
[InternalBufferCapacity(10)]//���������� ��������� � ������
public struct LockPositionBufferElement : IBufferElementData, IEnableableComponent
{
    public float3 worldPosition;

    //��������� ����������� ������� �������� ������� ��������� �������� ������� �
    //���������� ����� ������� ������, ��������� �������� �������� = �������� ������� �� ��������
    public static implicit operator LockPositionBufferElement(float3 worldPosition)
    {
        return new LockPositionBufferElement { worldPosition = worldPosition };
    }
}