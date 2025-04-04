using System;
using Unity.Entities;
using Unity.Mathematics;

public struct GridPositionXZ : IEquatable<GridPositionXZ> //��������� Equatable - ��������������    //������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#. ����� ���� ������ ����������� ����, ��������, int, double � �.�.,
                                                          //�� ���� �������� �����������. ��� ������ � ���������� �� �������� ����� �������� � �� ������.
                                                          //�� ������� ���� ��������� �.�. �� ����� ��������������� ����������� Vector2Int. �� �������� � X Y � ��� ����� X Z (����� ���� �� ������� �������������� �� � XZ ���� � �������, �� ��� ��������� ����� ����� ���� � ���� ��������������)
{
    public int x;
    public int z;
    /// <summary>
    /// ���� �� ������� ������������� ���� �������� �������
    /// </summary>
    public int floor;

    public GridPositionXZ(int x, int z, int floor) // ��������������� �����������
    {
        this.x = x;
        this.z = z;
        this.floor = floor;
    }

    public GridPositionXZ(int3 int3) // ��������������� �����������
    {
        this.x = int3.x;
        this.z = int3. z;
        this.floor = int3. y;
    }

    public override string ToString() // ������������� ToString(). ����� ������� � ������� Debug.Log ��������� ��������� X Z � ����
    {
        return $"x: {x}; z: {z}; _floor: {floor}";
    }

    public int3 ParseInt3()
    {
        return new int3(x, floor, z);
    }

    public static bool operator ==(GridPositionXZ a, GridPositionXZ b) // ���������� ��� ������� �������� ���������
    {
        return a.x == b.x && a.z == b.z && a.floor == b.floor;
    }

    public static bool operator !=(GridPositionXZ a, GridPositionXZ b) // ���������� ��� ������� �������� ���������
    {
        return !(a == b);
    }

    public static GridPositionXZ operator +(GridPositionXZ a, GridPositionXZ b) // ���������� ��� �����
    {
        return new GridPositionXZ(a.x + b.x, a.z + b.z, a.floor + b.floor);
    }

    public static GridPositionXZ operator -(GridPositionXZ a, GridPositionXZ b) // ���������� ��� ��������
    {
        return new GridPositionXZ(a.x - b.x, a.z - b.z, a.floor - b.floor);
    }

    public override bool Equals(object obj) // ������������� ��������������� ���������� ��������������� ���������
    {
        return obj is GridPositionXZ position &&
               x == position.x &&
               z == position.z &&
               floor == position.floor;
    }

    public override int GetHashCode() // ������������� ��������������� ����������
    {
        return HashCode.Combine(x, z, floor);
    }

    public bool Equals(GridPositionXZ other) // ���������� ���������� ���������
    {
        return this == other;
    }
}