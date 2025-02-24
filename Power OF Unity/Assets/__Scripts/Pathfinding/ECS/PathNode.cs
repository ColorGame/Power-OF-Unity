using System;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// ���� ���� (��������� � ������ ������ �����)
/// </summary>
public struct PathNode: IComponentData
{
    /// <summary>
    /// X,Y,Z (XZ-��������� Y- ����)
    /// </summary>
    public int3 gridPosition;
    public float3 worldPosition;

    public int index;
    /// <summary>
    /// ��������� �������� �� ������ �� ������� ������<br/>
    /// �� ��������� ����������� �� ������� ��������. ����� ���������� � ������������� ������������(������ ���������� �� ����� ��������� ������)                         
    /// </summary>
    public int gCost;
    /// <summary>
    /// ������������� ��������� �������� �� ���� �� ����������� ���� (����� ������ ����� �� ������)<br/>
    /// ��� ����� � ���� ��� ������ ����� ��� �����
    /// </summary>
    public int hCost;
    /// <summary>
    /// ����� g+h
    /// </summary>
    public int fCost;

    public bool isWalkable;
    public bool isInAir;

    /// <summary>
    /// ������ �� ������� ����
    /// </summary>
    public int cameFromNodeIndex;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
