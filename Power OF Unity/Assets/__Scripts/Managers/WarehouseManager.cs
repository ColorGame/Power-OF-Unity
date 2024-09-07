using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление ресурсами (монеты, оружие броня...)
/// </summary>
/// <remarks>
/// Предметы экипировки на юните не учитываются.
/// </remarks>
public class WarehouseManager
{
    public WarehouseManager(TooltipUI tooltipUI)
    {
        Init(tooltipUI);
    }

    public event EventHandler<PlacedObjectTypeAndCount> OnChangCountPlacedObject; // Изменено количество размещенных объектов

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
            ResourcesBasicListSO resourcesBasicListSO = Resources.Load<ResourcesBasicListSO>(typeof(ResourcesBasicListSO).Name); // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                                                 // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр BuildingTypeListSO (список будет один) и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра
            _coin = resourcesBasicListSO.GetCoin();
            _allPlacedObjectCountDictionary = resourcesBasicListSO.GetAllPlacedObjectCountDictionary();            
        }
        else
        {
            // Реализовать загрузку
        }
    }


    public void AddCoin(int coin)
    {
        _coin += coin;
    }
    /// <summary>
    /// Попробую потратить монеты
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
    /// Добавить количество переданного PlacedObjectTypeSO (в аргумент можно передать число>=0)
    /// </summary>
    /// <remarks>
    /// Если этого типа предмета нету в словаре, то добавим его (при открытии нового предмета).
    /// </remarks>
    public void AddCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // Если этого типа нет в списке ключей (когда открываем новый предмет)
        {
            _allPlacedObjectCountDictionary.Add(placedObjectTypeSO, number); // Добавим в словарь новый тип placedObject
            //_allPlacedObjectCountDictionary[placedObjectTypeSO] = number; // Альтернативная запись
        }
        else
        {
            _allPlacedObjectCountDictionary[placedObjectTypeSO] += number; // Добавим к текущему количеству
        }
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
    }

    /// <summary>
    /// Попробую уменьшить ,на переданное количество, PlacedObjectTypeSO 
    /// </summary>
    /// <remarks>(в аргумент можно передать число>=0)</remarks>
    public bool TryDecreaseCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // Если этого типа нет в словаре
        {
            return false;
        }

        if (number > _allPlacedObjectCountDictionary[placedObjectTypeSO]) // Если отнимае больше чем у нас есть то 
        {
            return false;
        }

        _allPlacedObjectCountDictionary[placedObjectTypeSO] -= number;
        OnChangCountPlacedObject?.Invoke(this, new PlacedObjectTypeAndCount(placedObjectTypeSO, _allPlacedObjectCountDictionary[placedObjectTypeSO]));
        return true;
    }

    public int GetCountCoin() { return _coin; }

    public IEnumerable<PlacedObjectTypeSO> GetAllPlacedObjectTypeSOList() { return _allPlacedObjectCountDictionary.Keys; }
    public uint GetCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _allPlacedObjectCountDictionary[placedObjectTypeSO]; } // Проверять не надо т.к. по ирархии вызовов этот метод вызывается в цикле перебора имеющихся ключей
}
