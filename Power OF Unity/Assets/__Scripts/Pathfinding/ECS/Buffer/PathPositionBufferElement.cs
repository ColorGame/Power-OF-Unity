using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// ������� ������� - �������� ������� ����
/// </summary>
[InternalBufferCapacity(20)]//���������� ��������� � ������
public struct PathPositionBufferElement: IBufferElementData
{
    public int3 gridPosition;

    //��������� ����������� ������� �������� ������� ��������� �������� ������� �
    //���������� ����� ������� ������, ��������� �������� �������� = �������� ������� �� ��������
    public static implicit operator PathPositionBufferElement(int3 gridPosition)
    {
        return new PathPositionBufferElement { gridPosition = gridPosition };
    }
}
