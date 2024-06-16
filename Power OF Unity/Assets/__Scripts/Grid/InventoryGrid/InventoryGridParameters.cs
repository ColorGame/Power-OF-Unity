
using System;
using UnityEngine;

[Serializable] // ����� ��������� ��������� ����� ������������ � ����������
public struct InventoryGridParameters  // �������� ��������� ����� � ��������� ������. ������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#
{                    
    public InventorySlot slot;
    public int width;       //������
    public int height;      //������    
    public  float cellSize;  // ������ ������
    public Transform anchorGridTransform; //����� ���������� �����

    public InventoryGridParameters(InventorySlot gridName, int width, int height, float cellSize, Transform anchorGridTransform)
    {
        this.slot = gridName;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.anchorGridTransform = anchorGridTransform;
    }
}
