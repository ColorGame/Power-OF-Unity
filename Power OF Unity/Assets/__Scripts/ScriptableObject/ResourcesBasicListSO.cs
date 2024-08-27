using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ������� ������ �������� ����������. ����� ����� ���� ��������� (���� ������)
/// </summary>
[CreateAssetMenu(fileName = "ResourcesBasicListSO", menuName = "ScriptableObjects/ResourcesBasicListSO")]
public class ResourcesBasicListSO : ScriptableObject
{
    [Header("��������� ���������� �����")]
    [SerializeField] private int _coin;
    [Header("������ ��������� � ���������(Action)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectWithActionList; // ������ ����� ����������� �������� c ���������
    [Header("������ ��������� �����(Armor)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList; // ������ ����������� �������� ���� �����


    public int GetCoin() {  return _coin; }
    public List<PlacedObjectTypeAndCount> GetPlacedObjectWithActionList()
    {
        // �������� ��������� PlacedObjectTypeSO
        // ��� ����������� ������ �� �������� placedObjectTypeSO, ����� �������� ������ ������ � �����������.
       
        return _placedObjectWithActionList.GroupBy(x=>x.placedObjectTypeSO).Select(x=>x.First()).ToList(); 
    }
    public List<PlacedObjectTypeAndCount> GetPlacedObjectArmorList()
    {
        return _placedObjectTypeArmorList.GroupBy(x => x.placedObjectTypeSO).Select(x => x.First()).ToList(); 
    }
}


