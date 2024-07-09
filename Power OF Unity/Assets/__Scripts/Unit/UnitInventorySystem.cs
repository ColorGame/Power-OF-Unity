using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Система инвенторя юнитов(звено между сеткой и инвентарем юнита). Отвечает за настройку и загрузку инвенторя ВЫБРАННОГО юнита
/// </summary>
public class UnitInventorySystem 
{

    private Unit _selectedUnit;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitManager _unitManager;
    private InventoryGrid _inventoryGrid;

    public void Init(PickUpDropPlacedObject pickUpDropPlacedObject, UnitManager unitManager, InventoryGrid inventoryGrid)
    {       
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitManager = unitManager;
        _inventoryGrid = inventoryGrid;

        _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  Объект добавлен в сетку Интвенторя
        _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя

        SelectUnitAtStart();
    }

    /// <summary>
    /// Выбрать юнита при старте
    /// </summary>
    private void SelectUnitAtStart()
    {
        List<Unit> UnitFriendList = _unitManager.GetUnitFriendList();
        if (UnitFriendList.Count!=0)
        {
            _selectedUnit = UnitFriendList[0];
        }

        UpdateInventoryGrid();
    }

  
    private void UpdateInventoryGrid ()
    {
        List<PlacedObjectParameters> placedObjectList = _selectedUnit.GetUnitInventory().GetPlacedObjectList();
        _inventoryGrid.ClearInventoryGridAndDestroyPlacedObjects();
        foreach (PlacedObjectParameters placedObjectParameters in placedObjectList)
        {
            // Создадим и попробуем разместить   
            PlacedObject.CreateAddTryPlacedInGrid(placedObjectParameters, _inventoryGrid, _pickUpDropPlacedObject);
        }
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
        UpdateInventoryGrid();
        Debug.Log($"Выбрал -{_selectedUnit.GetUnitTypeSO<UnitTypeSO>().GetName()} ");
    }
}
