using System;

public struct GridPositionEquipmentXY : IEquatable<GridPositionEquipmentXY> //��������� Equatable - ��������������    //������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#. ����� ���� ������ ����������� ����, ��������, int, double � �.�.,
                                                          //�� ���� �������� �����������. ��� ������ � ���������� �� �������� ����� �������� � �� ������.
                                                          //�� ������� ���� ��������� �.�. �� ����� ��������������� ����������� Vector2Int. �� �������� � X Y � ��� ����� X Z (����� ���� �� ������� �������������� �� � XZ ���� � �������, �� ��� ��������� ����� ����� ���� � ���� ��������������)
{
    public int x;
    public int z;
    public EquipmentSlot gridName;

    public GridPositionEquipmentXY(int x, int z, EquipmentSlot gridName) // ��������������� �����������
    {
        this.x = x;
        this.z = z;
        this.gridName = gridName;
    }


    public override string ToString() // ������������� ToString(). ����� ������� � ������� Debug.Log ��������� ��������� X Z � ����
    {
        return $"x: {x}; z: {z}; _floor: {gridName}";
    }

    public static bool operator ==(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // ���������� ��� ������� �������� ���������
    {
        return a.x == b.x && a.z == b.z && a.gridName == b.gridName;
    }

    public static bool operator !=(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // ���������� ��� ������� �������� ���������
    {
        return !(a == b);
    }

    public static GridPositionEquipmentXY operator +(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // ���������� ��� �����
    {
        return new GridPositionEquipmentXY(a.x + b.x, a.z + b.z, a.gridName);
    }

    public static GridPositionEquipmentXY operator -(GridPositionEquipmentXY a, GridPositionEquipmentXY b) // ���������� ��� ��������
    {
        return new GridPositionEquipmentXY(a.x - b.x, a.z - b.z, a.gridName);
    }

    public override bool Equals(object obj) // ������������� ��������������� ���������� ��������������� ���������
    {
        return obj is GridPositionEquipmentXY position &&
               x == position.x &&
               z == position.z &&
               gridName == position.gridName;
    }

    public override int GetHashCode() // ������������� ��������������� ����������
    {
        return HashCode.Combine(x, z, gridName);
    }

    public bool Equals(GridPositionEquipmentXY other) // ���������� ���������� ���������
    {
        return this == other;
    }
}
