using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Система инвенторя юнита. Отвечает за настройку и загрузку инвенторя ВЫБРАННОГО юнита
/// </summary>
public class UnitInventorySystem 
{

    private Unit _selectedUnit;
    private PickUpDropPlacedObject _pickUpDrop;
    private UnitManager _unitManager;
    private InventoryGrid _inventoryGrid;

    public void Init(PickUpDropPlacedObject pickUpDrop, UnitManager unitManager, InventoryGrid inventoryGrid)
    {       
        _pickUpDrop = pickUpDrop;
        _unitManager = unitManager;
        _inventoryGrid = inventoryGrid;

        _pickUpDrop.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  Объект добавлен в сетку Интвенторя
        _pickUpDrop.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя

        SelectUnitAtStart();
    }

    private void SelectUnitAtStart()
    {
        List<Unit> UnitFriendList = _unitManager.GetUnitFriendList();
        if (UnitFriendList.Count!=0)
        {
            _selectedUnit = UnitFriendList[0];
        }       
        List<PlacedObjectParameters> placedObjectList = _selectedUnit.GetUnitInventory().GetPlacedObjectList();
        _inventoryGrid.ClearInventoryGridAndDestroyPlacedObjects();
        foreach (PlacedObjectParameters placedObjectParameters in placedObjectList)
        {
           // PlacedObject placedObject = PlacedObject.CreateInGrid(_inventoryGrid.GetG)
        }
        //
    }

    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnAddPlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitInventory().AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// Удалим полученный объект из Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnRemovePlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitInventory().RemovePlacedObjectList(placedObject);
    }


    public void SetSelectedUnit(Unit selectedUnit)
    {
        _selectedUnit = selectedUnit;
        Debug.Log($"Выбрал -{_selectedUnit.GetUnitTypeSO<UnitTypeSO>().GetName()} ");
    }
}
