using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ������������ ���� ��� _shortTooltipsText

public class GridDebugObject : MonoBehaviour // ������� ������� ����� (����������� �������� ��������� � ������ ������ � ������)
{

    [SerializeField] private TextMeshPro _textMeshPro; // � ���������� ���������� ��������� �������� ������ �������

    private object _gridObject; // �������� ���������� ��� �������� ������� ����� // ������ ���� GridObjectUnitXZ ������� object - ������������� ��� ��� �� �������� � ������� ������ ��������

    public virtual void SetGridObject(object gridObject) // ��������� ������ ����� // virtual- ���� ������������ �������������� ��� ������ 
    {
        _gridObject = gridObject; // ��������� ���������� ��� ������ ��� �������� ������
    }

    protected virtual void Update() // � �������� ���� ��������� ���������� ����� ����� ����� Event ����� ���� ������ ��� ������� �� ������ �����
    {
        _textMeshPro.text = _gridObject.ToString(); // � Update ����� ��������� ���������� ����������� ��������� �����        
    }
}
