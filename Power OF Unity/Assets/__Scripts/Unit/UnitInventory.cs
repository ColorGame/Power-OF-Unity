using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Инвентарь юнита (Хранилище для предметов, которыми оснащен игрок). 
/// </summary>
public class UnitInventory
{
    public UnitInventory(Unit unit)
    {
        _unit = unit;
        _placedObjectList = new List<PlacedObjectParameters>(); 
    }

    public event EventHandler<PlacedObject> OnAddPlacedObjectList; // Событие Добавлен предмет в инвентарь
    public event EventHandler<PlacedObject> OnRemovePlacedObjectList; // Событие Удален предмет из инвенторя

    private Unit _unit;
    private List<PlacedObjectParameters> _placedObjectList; // Список предметов инвентаря


    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {       
        _placedObjectList.Add(placedObject.GetPlacedObjectParameters());
        foreach (BaseAction baseAction in placedObject.GetPlacedObjectTypeSO().GetBaseActionsArray())
        {
            _unit.AddBaseActionsList(baseAction);
        }
        OnAddPlacedObjectList?.Invoke(this, placedObject);
    }
    /// <summary>
    /// Удалить полученный объект из Списока "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>  
    public void RemovePlacedObjectList(PlacedObject placedObject)
    {
        //_placedObjectList.Remove(placedObject);

        OnRemovePlacedObjectList?.Invoke(this, placedObject);
    }


    /// <summary>
    /// Размещенный Объект в Сетке Инвенторя
    /// </summary>
    [Serializable]
    public struct PlacedObjectOnInventoryGrid
    {
        public GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY; //Сетка на которой добавили размещенный объект
        public Vector2Int gridPositioAnchor; // Сеточная позиция Якоря размещенного объекта
        public PlacedObjectTypeSO placedObjectTypeSO;
    }
    //private List<PlacedObjectOnInventoryGrid> _placedObjectOnInventoryGrid; // Список Размещенных Объектов в Сетке Инвенторя.
}
