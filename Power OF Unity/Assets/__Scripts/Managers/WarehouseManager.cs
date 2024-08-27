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
    private List<PlacedObjectTypeAndCount> _placedObjectWithActionList; // Список Типов Размещяемых объектов c ДЕЙСТВИЕМ
    private List<PlacedObjectTypeAndCount> _placedObjectTypeArmorList; // Список Размещяемых объектов типа БРОНЯ

    // Хэшированный список Для проверки на содержание
    private HashSet<PlacedObjectTypeSO> _allPlacedObjectTypeSOList; // Cписок всех типов Размещяемых объектов

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
            _placedObjectWithActionList = resourcesBasicListSO.GetPlacedObjectWithActionList();
            _placedObjectTypeArmorList = resourcesBasicListSO.GetPlacedObjectArmorList();
        }
        else
        {
            // Реализовать загрузку
        }


        FillAllPlacedObjectTypeSOList();
    }
    /// <summary>
    /// Заполним хэшированный список всех типов Размещяемых Объектов
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
    /// Добавить в список переданный PlacedObjectTypeSO (в аргумент можно передать число>=0)
    /// </summary>
    /// <remarks>
    /// В зависимости от типа PlacedObjectTypeSO распределим в нужный список. Если этого типа предмета нету в хэшированном списке, то создадим и добавим его.
    /// </remarks>
    public void AddPlacedObjectTypeList(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (!_allPlacedObjectTypeSOList.Contains(placedObjectTypeSO)) // Если этого типа нет в хэшированном списке (когда открывается новый предмет)
        {
            AddNewPlacedObjectTypeAndCountList(placedObjectTypeSO, number);
            return;// Выходим игнор. код ниже
        }

        // В зависимости от переданного типа определим нужный список
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
    /// Попробую уменьшить ,на переданное количество, PlacedObjectTypeSO 
    /// </summary>
    /// <remarks>(в аргумент можно передать число>=0)</remarks>
    public bool TryDecreaseCountPlacedObjectType(PlacedObjectTypeSO placedObjectTypeSO, uint number =1) 
    {
        if (!_allPlacedObjectTypeSOList.Contains(placedObjectTypeSO)) // Если этого типа нет в хэшированном списке
        {            
            return false;
        }
        // В зависимости от переданного типа определим нужный список
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
    /// Добавление в список НОВОГО типа PlacedObjectTypeAndCount
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
    /// Попробуем изменить (+/-) количество PlacedObjectTypeSO в переданном списке
    /// </summary><remarks>
    /// ЧУВСТВИТЕЛЬНА к знаку перед number ("+"прибавляем/"-"отнимаем)
    /// </remarks>
    private bool TryChangeCountPlacedObjectType(List<PlacedObjectTypeAndCount> placedObjectTypeAndCountList, PlacedObjectTypeSO placedObjectTypeSO, int number)
    {
        if(number == 0) return true; // Если изменения нулевые то просто вернем true

        for (int index = 0; index < placedObjectTypeAndCountList.Count; index++)
        {
            if (placedObjectTypeAndCountList[index].placedObjectTypeSO == placedObjectTypeSO)
            {
                // Проверка при отнимании (number - отрицательное)
                if (number < 0 && placedObjectTypeAndCountList[index].count < Math.Abs(number)) // Если отнимае больше чем у нас есть то 
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
        // Если прошли весь список и не нашли переданный placedObjectTypeSO то
        return false;
    }



    public List<PlacedObjectTypeAndCount> GetPlacedObjectWithActionList() { return _placedObjectWithActionList; }
    public List<PlacedObjectTypeAndCount> GetPlacedObjectTypeArmorList() { return _placedObjectTypeArmorList; }
}
