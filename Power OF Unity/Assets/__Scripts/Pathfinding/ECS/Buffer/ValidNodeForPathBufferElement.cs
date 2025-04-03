using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// ������� ������� -���������� �������� ������� ��� ����
/// </summary>
[InternalBufferCapacity(20)]//���������� ��������� � ������
public struct ValidNodeForPathBufferElement: IBufferElementData
{
    public int3 gridPosition;

    //��������� ����������� ������� �������� ������� ��������� �������� ������� �
    //���������� ����� ������� ������, ��������� �������� �������� = �������� ������� �� ��������
    public static implicit operator ValidNodeForPathBufferElement(int3 gridPosition)
    {
        return new ValidNodeForPathBufferElement { gridPosition = gridPosition };
    }
}
