using System;
using System.Collections;
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
    public event EventHandler<uint> OnChangCoinCount; // �������� ���������� �����

    private uint _coin;
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

    /// <summary>
    /// �������� �����
    /// </summary>
    public void PlusCoin(uint coin)
    {
        _coin += coin;
        OnChangCoinCount?.Invoke(this, _coin);
    }
    /// <summary>
    /// �������� �������(���������) ������
    /// </summary>
    public bool TryMinusCoins(uint coin)
    {
        if (_coin >= coin)
        {
            _coin -= coin;
            OnChangCoinCount?.Invoke(this, _coin);
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// ��������� ������ 
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����� ������� � ������� �������� ������� (Keys-������, Values-����������)
    /// </remarks>
    public bool TryBuy(uint sumBuy, Dictionary<PlacedObjectTypeSO, uint> buyObjectCountDictionary)
    {
        if (TryMinusCoins(sumBuy))
        {
            foreach (PlacedObjectTypeSO placedObjectTypeSO in buyObjectCountDictionary.Keys)
            {
                PlusCountPlacedObject(placedObjectTypeSO, buyObjectCountDictionary[placedObjectTypeSO]);
            }
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// ������� (��������, �� ������������� �������� ��� �������, ������ ���������� �� ������� ������������ ������)
    /// </summary>
    /// /// <remarks>
    /// � �������� �������� ����� ������� � ������� �������� ��� ������� (Keys-������, Values-����������)
    /// </remarks>
    public void Sell(uint sumSell, Dictionary<PlacedObjectTypeSO, uint> sellObjectCountDictionary)
    {
        PlusCoin(sumSell);
        foreach (PlacedObjectTypeSO placedObjectTypeSO in sellObjectCountDictionary.Keys)
        {
            MinusCountPlacedObjectType(placedObjectTypeSO, sellObjectCountDictionary[placedObjectTypeSO]);
        }
    }

    /// <summary>
    /// �������� ���������� ����������� PlacedObjectTypeSO (� �������� ����� �������� �����>=0)
    /// </summary>
    /// <remarks>
    /// ���� ����� ���� �������� ���� � �������, �� ������� ��� (��� �������� ������ ��������).
    /// </remarks>
    public void PlusCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
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
    public bool TryMinusCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // ���� ����� ���� ��� � �������
        {
            return false;
        }

        if (number > _allPlacedObjectCountDictionary[placedObjectTypeSO]) // ���� ������� ������ ��� � ��� ���� �� 
        {
            return false;
        }

        MinusCountPlacedObjectType(placedObjectTypeSO, number);
        return true;
    }

    private void MinusCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number)
    {
        _allPlacedObjectCountDictionary[placedObjectTypeSO] -= number;
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
    }

    public uint GetCountCoin() { return _coin; }

    public IEnumerable<PlacedObjectTypeSO> GetAllPlacedObjectTypeSOList() { return _allPlacedObjectCountDictionary.Keys; }
    public uint GetCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _allPlacedObjectCountDictionary[placedObjectTypeSO]; } // ��������� �� ���� �.�. �� ������� ������� ���� ����� ���������� � ����� �������� ��������� ������
}
