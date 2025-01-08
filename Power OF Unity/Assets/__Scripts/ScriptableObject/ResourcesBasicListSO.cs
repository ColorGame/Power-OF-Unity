using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ������� ������ �������� ����������. ����� ����� ���� ��������� (���� ������)
/// </summary>
[CreateAssetMenu(fileName = "ResourcesBasicListSO", menuName = "ScriptableObjects/Date/ResourcesBasicListSO")]
public class ResourcesBasicListSO : ScriptableObject
{
    [Header("��������� ���������� �����")]
    [SerializeField] private uint _coin =100000;
    [Header("������ ��������� � ���������(Action)")]
    [SerializeField] private List <PlacedObjectTypeAndCount> _placedObjectWithActionList= new(); 
    [Header("������ ��������� �����(Armor)")]
    [SerializeField] private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList = new();
    [Header("���������� ������� ��������")]
    [SerializeField] private uint _count = 3;

    private Dictionary<PlacedObjectTypeSO, uint> _allPlacedObjectCountDictionary = null;

    [ContextMenu("��������������")]
    private void DownloadSheets() // ������������� ������
    {
        foreach(PlacedObjectTypeSO placedObjectType in PlacedObjectGeneralListForAutoCompletionSO.Instance.ArmorSOArray)
        {
            _placedObjectTypeArmorList.Add(new PlacedObjectTypeAndCount(placedObjectType, _count));
        }
        foreach (PlacedObjectTypeSO placedObjectType in PlacedObjectGeneralListForAutoCompletionSO.Instance.ActionSOArray)
        {
            _placedObjectWithActionList.Add(new PlacedObjectTypeAndCount(placedObjectType, _count));
        }
    }    

    public uint GetCoin() {  return _coin; }

    public Dictionary<PlacedObjectTypeSO, uint> GetAllPlacedObjectCountDictionary()
    {
        if (_allPlacedObjectCountDictionary == null)
        {
            _allPlacedObjectCountDictionary = CompleteDictionary();
        }
        return _allPlacedObjectCountDictionary;
    }

    private Dictionary<PlacedObjectTypeSO, uint> CompleteDictionary()
    {
        Dictionary<PlacedObjectTypeSO, uint> allPlacedObjectCountDictionary = new Dictionary<PlacedObjectTypeSO, uint>();

        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectWithActionList)
        {
            if(!allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeAndCount.placedObjectTypeSO)) //���� ����� ���� ��� � ������ ������� �� �������
            {
                allPlacedObjectCountDictionary[placedObjectTypeAndCount.placedObjectTypeSO] = placedObjectTypeAndCount.count;
            }
        }
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectTypeArmorList)
        {
            if (!allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeAndCount.placedObjectTypeSO)) //���� ����� ���� ��� � ������ ������� �� �������
            {
                allPlacedObjectCountDictionary[placedObjectTypeAndCount.placedObjectTypeSO] = placedObjectTypeAndCount.count;
            }
        }
        return allPlacedObjectCountDictionary;
    }

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


