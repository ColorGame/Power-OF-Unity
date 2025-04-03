using Unity.Collections;
using Unity.Mathematics;

/// <summary>
/// ���� ���� (��������� � ������ ������ �����)
/// </summary>
public struct PathNode 
{
    /// <summary>
    /// X,Y,Z (XZ-��������� Y- ����)
    /// </summary>
    public int3 gridPosition;
    public float3 worldPosition;
    /// <summary>
    /// ������ ���� � �������
    /// </summary>
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
    /// <summary>
    /// ����� ������ �� ����� ����?
    /// </summary>
    public bool isWalkable;
    /// <summary>
    /// ���� ���� � �������?
    /// </summary>
    public bool isInAir;
    /// <summary>
    /// ������ ������� ������� �� ������ ���� �� ����� ����
    /// </summary>
    public NativeList<float3> pathWorldPositionList;
    /// <summary>
    /// ������ �� ������� ����(�����)
    /// </summary>
    public int cameFromNodeIndex;
    /// <summary>
    /// gCost + hCost
    /// </summary>
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
