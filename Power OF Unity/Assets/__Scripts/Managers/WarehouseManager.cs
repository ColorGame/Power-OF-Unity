using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������� ��������� (������, ������ �����...)
/// </summary>
/// <remarks>
/// �������� ���������� �� ����� �� �����������.
/// </remarks>
public class WarehouseManager
{
    public WarehouseManager(TooltipUI tooltipUI)
    {
        Init(tooltipUI);
    }

    public event EventHandler<PlacedObjectTypeAndCount> OnChangCountPlacedObject; // �������� ���������� ����������� ��������

    private int _coin;    
    private Dictionary<PlacedObjectTypeSO, uint> _allPlacedObjectCountDictionary = null;

    private TooltipUI _tooltipUI;

    private void Init(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;
        InitPlacedObjectList(firstStart: true);
    }

    private void InitPlacedObjectList(bool firstStart)
    {
        if (firstStart)
        {
            ResourcesBasicListSO resourcesBasicListSO = Resources.Load<ResourcesBasicListSO>(typeof(ResourcesBasicListSO).Name); // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                                                 // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� BuildingTypeListSO (������ ����� ����) � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������
            _coin = resourcesBasicListSO.GetCoin();
            _allPlacedObjectCountDictionary = resourcesBasicListSO.GetAllPlacedObjectCountDictionary();            
        }
        else
        {
            // ����������� ��������
        }
    }


    public void AddCoin(int coin)
    {
        _coin += coin;
    }
    /// <summary>
    /// �������� ��������� ������
    /// </summary>
    public bool TrySpendCoins(int coin)
    {
        if (_coin >= coin)
        {
            _coin -= coin;
            return true;
        }
        else { return false; }
    }

    /// <summary>
    /// �������� ���������� ����������� PlacedObjectTypeSO (� �������� ����� �������� �����>=0)
    /// </summary>
    /// <remarks>
    /// ���� ����� ���� �������� ���� � �������, �� ������� ��� (��� �������� ������ ��������).
    /// </remarks>
    public void AddCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // ���� ����� ���� ��� � ������ ������ (����� ��������� ����� �������)
        {
            _allPlacedObjectCountDictionary.Add(placedObjectTypeSO, number); // ������� � ������� ����� ��� placedObject
            //_allPlacedObjectCountDictionary[placedObjectTypeSO] = number; // �������������� ������
        }
        else
        {
            _allPlacedObjectCountDictionary[placedObjectTypeSO] += number; // ������� � �������� ����������
        }
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
    }

    /// <summary>
    /// �������� ��������� ,�� ���������� ����������, PlacedObjectTypeSO 
    /// </summary>
    /// <remarks>(� �������� ����� �������� �����>=0)</remarks>
    public bool TryDecreaseCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // ���� ����� ���� ��� � �������
        {
            return false;
        }

        if (number > _allPlacedObjectCountDictionary[placedObjectTypeSO]) // ���� ������� ������ ��� � ��� ���� �� 
        {
            return false;
        }

        _allPlacedObjectCountDictionary[placedObjectTypeSO] -= number;
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
        return true;
    }

    public int GetCountCoin() { return _coin; }

    public IEnumerable<PlacedObjectTypeSO> GetAllPlacedObjectTypeSOList() { return _allPlacedObjectCountDictionary.Keys; }
    public uint GetCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _allPlacedObjectCountDictionary[placedObjectTypeSO]; } // ��������� �� ���� �.�. �� ������� ������� ���� ����� ���������� � ����� �������� ��������� ������
}
