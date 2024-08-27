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
    private List<PlacedObjectTypeAndCount> _placedObjectWithActionList; // ������ ����� ����������� �������� c ���������
    private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList; // ������ ����������� �������� ���� �����

    // ������������ ������ ��� �������� �� ����������
    private HashSet<PlacedObjectTypeSO> _allPlacedObjectTypeSOList; // C����� ���� ����� ����������� ��������

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
            _placedObjectWithActionList = resourcesBasicListSO.GetPlacedObjectWithActionList();
            _placedObjectTypeArmorList = resourcesBasicListSO.GetPlacedObjectArmorList();
        }
        else
        {
            // ����������� ��������
        }


        FillAllPlacedObjectTypeSOList();
    }
    /// <summary>
    /// �������� ������������ ������ ���� ����� ����������� ��������
    /// </summary>
    private void FillAllPlacedObjectTypeSOList()
    {
        _allPlacedObjectTypeSOList = new HashSet<PlacedObjectTypeSO>();
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectWithActionList)
        {
            _allPlacedObjectTypeSOList.Add(placedObjectTypeAndCount.placedObjectTypeSO);
        }
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _placedObjectTypeArmorList)
        {
            _allPlacedObjectTypeSOList.Add(placedObjectTypeAndCount.placedObjectTypeSO);
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
    /// �������� � ������ ���������� PlacedObjectTypeSO (� �������� ����� �������� �����>=0)
    /// </summary>
    /// <remarks>
    /// � ����������� �� ���� PlacedObjectTypeSO ����������� � ������ ������. ���� ����� ���� �������� ���� � ������������ ������, �� �������� � ������� ���.
    /// </remarks>
    public void AddPlacedObjectTypeList(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectTypeSOList.Contains(placedObjectTypeSO)) // ���� ����� ���� ��� � ������������ ������ (����� ����������� ����� �������)
        {
            AddNewPlacedObjectTypeAndCountList(placedObjectTypeSO, number);
            return;// ������� �����. ��� ����
        }

        // � ����������� �� ����������� ���� ��������� ������ ������
        List<PlacedObjectTypeAndCount> placedObjectTypeSOList = new List<PlacedObjectTypeAndCount>();
        switch (placedObjectTypeSO)
        {
            case PlacedObjectTypeWithActionSO:
                placedObjectTypeSOList = _placedObjectWithActionList;
                break;
            case PlacedObjectTypeArmorSO:
                placedObjectTypeSOList = _placedObjectTypeArmorList;
                break;
        }

         TryChangeCountPlacedObjectType(placedObjectTypeSOList, placedObjectTypeSO, (int)number);
    }
    /// <summary>
    /// �������� ��������� ,�� ���������� ����������, PlacedObjectTypeSO 
    /// </summary>
    /// <remarks>(� �������� ����� �������� �����>=0)</remarks>
    public bool TryDecreaseCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number =1) 
    {
        if (!_allPlacedObjectTypeSOList.Contains(placedObjectTypeSO)) // ���� ����� ���� ��� � ������������ ������
        {            
            return false;
        }
        // � ����������� �� ����������� ���� ��������� ������ ������
        List<PlacedObjectTypeAndCount> placedObjectTypeSOList = new List<PlacedObjectTypeAndCount>();
        switch (placedObjectTypeSO)
        {
            case PlacedObjectTypeWithActionSO:
                placedObjectTypeSOList = _placedObjectWithActionList;
                break;
            case PlacedObjectTypeArmorSO:
                placedObjectTypeSOList = _placedObjectTypeArmorList;
                break;
        }

        return TryChangeCountPlacedObjectType(placedObjectTypeSOList, placedObjectTypeSO, -(int)number);      
    }

    /// <summary>
    /// ���������� � ������ ������ ���� PlacedObjectTypeAndCount
    /// </summary>
    private void AddNewPlacedObjectTypeAndCountList(PlacedObjectTypeSO placedObjectTypeSO, uint number)
    {
        List<PlacedObjectTypeAndCount> PlacedObjectTypeSOList = new List<PlacedObjectTypeAndCount>();
        switch (placedObjectTypeSO)
        {
            case PlacedObjectTypeWithActionSO:
                PlacedObjectTypeSOList = _placedObjectWithActionList;
                break;
            case PlacedObjectTypeArmorSO:
                PlacedObjectTypeSOList = _placedObjectTypeArmorList;
                break;
        }
        PlacedObjectTypeSOList.Add(new PlacedObjectTypeAndCount(placedObjectTypeSO, (int)number));
        _allPlacedObjectTypeSOList.Add(placedObjectTypeSO);
    }

    /// <summary>
    /// ��������� �������� (+/-) ���������� PlacedObjectTypeSO � ���������� ������
    /// </summary><remarks>
    /// ������������� � ����� ����� number ("+"����������/"-"��������)
    /// </remarks>
    private bool TryChangeCountPlacedObjectType(List<PlacedObjectTypeAndCount> placedObjectTypeAndCountList, PlacedObjectTypeSO placedObjectTypeSO, int number)
    {
        if(number == 0) return true; // ���� ��������� ������� �� ������ ������ true

        for (int index = 0; index < placedObjectTypeAndCountList.Count; index++)
        {
            if (placedObjectTypeAndCountList[index].placedObjectTypeSO == placedObjectTypeSO)
            {
                // �������� ��� ��������� (number - �������������)
                if (number < 0 && placedObjectTypeAndCountList[index].count < Math.Abs(number)) // ���� ������� ������ ��� � ��� ���� �� 
                {
                    return false;
                }
                else
                {
                    placedObjectTypeAndCountList[index] = new PlacedObjectTypeAndCount
                    (
                        placedObjectTypeAndCountList[index].placedObjectTypeSO,
                        placedObjectTypeAndCountList[index].count + number
                    );
                    OnChangCountPlacedObject?.Invoke(this, placedObjectTypeAndCountList[index]);
                    return true;
                }
            }
        }
        // ���� ������ ���� ������ � �� ����� ���������� placedObjectTypeSO ��
        return false;
    }



    public List<PlacedObjectTypeAndCount> GetPlacedObjectWithActionList() { return _placedObjectWithActionList; }
    public List<PlacedObjectTypeAndCount> GetPlacedObjectTypeArmorList() { return _placedObjectTypeArmorList; }
}
