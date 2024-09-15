using System;
using System.Collections;
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
    public event EventHandler<uint> OnChangCoinCount; // Изменено количество монет

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

    /// <summary>
    /// Добавить монет
    /// </summary>
    public void PlusCoin(uint coin)
    {
        _coin += coin;
        OnChangCoinCount?.Invoke(this, _coin);
    }
    /// <summary>
    /// Попробую убавить(потратить) монеты
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
    /// Попробуем купить 
    /// </summary>
    /// <remarks>
    /// В аргумент передать сумму покупки и словарь объектов покупки (Keys-Объект, Values-Количесвто)
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
    /// Продать (проверка, на достаточность объектов для продажи, ДОЛЖНА происходит на стороне запросившего класса)
    /// </summary>
    /// /// <remarks>
    /// В аргумент передать сумму продажи и словарь объектов для продажи (Keys-Объект, Values-Количесвто)
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
    /// Добавить количество переданного PlacedObjectTypeSO (в аргумент можно передать число>=0)
    /// </summary>
    /// <remarks>
    /// Если этого типа предмета нету в словаре, то добавим его (при открытии нового предмета).
    /// </remarks>
    public void PlusCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
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
    public bool TryMinusCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectCountDictionary.ContainsKey(placedObjectTypeSO)) // Если этого типа нет в словаре
        {
            return false;
        }

        if (number > _allPlacedObjectCountDictionary[placedObjectTypeSO]) // Если отнимае больше чем у нас есть то 
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
    public uint GetCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _allPlacedObjectCountDictionary[placedObjectTypeSO]; } // Проверять не надо т.к. по ирархии вызовов этот метод вызывается в цикле перебора имеющихся ключей
}
