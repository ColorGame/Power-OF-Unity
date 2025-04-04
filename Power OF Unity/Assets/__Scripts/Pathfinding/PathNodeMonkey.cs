using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeMonkey // ���� ������ ���� // ����� ��������� � ������ ������ �����, � ����� ������� � ���� ������� ������ ��� ������� ������ ���� G H F
                      // ����������� ����� C#// ����� ������������ ����������� ��� �������� ����� ����� ������� �� �� ��������� MonoBehaviour
                      // ����� �������� GridObjectUnitXZ
{

    private GridPositionXZ _gridPosition;
    private int _gCost;  // ��������� �������� �� ��������� ������. �� ��������� ����������� �� ������� ��������. ����� ���������� � ������������� ������������(������ ���������� �� ����� ��������� ������)
                         // � ��������� ������� G = 0. ����� ��������� � ������ ������ �� ������ ��� �������� ����������� �� G = 0 + gCost. ��������� ��� ���, �������� �� ��������� ����� G = 0 + gCost +������ ���������� �� ����� ���������-gCost  
    private int _hCost;  // ��������� �������� �� ���� �� ����������� ���� (����� ������ ����� �� ������)// ��� ����� � ���� ��� ������ ����� ��� �����
    private int _fCost;  // ����� g+h
    private PathNodeMonkey _cameFromPathNode; // �������� �� PathNodeMonkey // � ������ ������ ��� ����� ������ �� ������ ������ �� �������
    private bool _isWalkeble = true; // ����� ������ // ��� ��������� ������������ �����

    public PathNodeMonkey(GridPositionXZ gridPosition) // �����������
    {
        _gridPosition = gridPosition;
    }

    public override string ToString() // ������������� ToString(). ����� ��� ���������� ������� � ����� � ����� � ���� ������ (����� ����� ��������� �������� ������������ ������)
    {
        return _gridPosition.ToString();
    }

    public int GetGCost()
    {
        return _gCost;
    }

    public int GetHCost()
    {
        return _hCost;
    }

    public int GetFCost()
    {
        return _fCost;
    }

    public void SetGCost(int gCost)
    {
        _gCost = gCost;
    }

    public void SetHCost(int hCost)
    {
        _hCost = hCost;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }

    public void ResetCameFromPathNode() // ����� ������ � ���� ���� ( ������� ������ �� ���������� ���� ����, ����� ���������� ���������� ���� �� ������ �� ������� ���������� ������ ����)
    {
        _cameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNodeMonkey pathNode) // ���������� ������  � ���� ����
    {
        _cameFromPathNode = pathNode;
    }

    public PathNodeMonkey GetCameFromPathNode() // �������� ������  � ���� ����
    {
        return _cameFromPathNode;
    }

    public GridPositionXZ GetGridPosition()
    {
        return _gridPosition;
    }

    public bool GetIsWalkable()
    {
        return _isWalkeble;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        _isWalkeble= isWalkable;
    }
}
