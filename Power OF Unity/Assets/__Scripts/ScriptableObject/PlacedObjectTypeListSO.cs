using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlacedObjectTypeListSO", menuName = "ScriptableObjects/PlacedObjectTypeList")]
public class PlacedObjectTypeListSO : ScriptableObject
{ // ����� ����� ���� ��������� (���� ������)

    [SerializeField]private List<PlacedObjectTypeWithActionSO> _placedObjectWithActionList; // ������ ����� ����������� ��������
    

    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectWithActionList() { return _placedObjectWithActionList; }
}

