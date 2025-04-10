using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// ������� ������� - ������� ��� ������������� ���� ����
/// </summary>
[InternalBufferCapacity(10)]//���������� ��������� � ������
public struct UnlockPositionBufferElement: IBufferElementData, IEnableableComponent
{
    public int3 gridPosition;

    //��������� ����������� ������� �������� ������� ��������� �������� ������� �
    //���������� ����� ������� ������, ��������� �������� �������� = �������� ������� �� ��������
    public static implicit operator UnlockPositionBufferElement(int3 gridPosition)
    {
        return new UnlockPositionBufferElement { gridPosition = gridPosition };
    }
}
