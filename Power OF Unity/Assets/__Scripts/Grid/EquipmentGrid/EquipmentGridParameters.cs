
using System;
using UnityEngine;

[Serializable] // ����� ��������� ��������� ����� ������������ � ����������
public struct EquipmentGridParameters  // �������� ��������� ����� � ��������� ������. ������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#
{                    
    public EquipmentSlot slot;
    public Transform anchorGridTransform; //����� ���������� �����
    public int width;       //������
    public int height;      //������    
    public  float cellSize;  // ������ ������

    public EquipmentGridParameters(EquipmentSlot slot, int width, int height, float cellSize, Transform anchorGridTransform)
    {
        this.slot = slot;
        this.anchorGridTransform = anchorGridTransform;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
    }
}
