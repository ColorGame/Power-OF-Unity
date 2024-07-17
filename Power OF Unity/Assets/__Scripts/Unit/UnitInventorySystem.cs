using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Система инвенторя юнитов(звено между сеткой и инвентарем юнита). Отвечает за настройку и загрузку инвенторя ВЫБРАННОГО юнита
/// </summary>
public class UnitInventorySystem
{
    public event EventHandler<Unit> OnSelectedUnitChanged; // Изменен выбранный юнит
    public event EventHandler OnInventoryGridsCleared; // Инвентарные сетки очищены
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // Объект добавлен в сетку Интвенторя

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
        if (UnitFriendList.Count != 0)
        {
            _selectedUnit = UnitFriendList[0];
        }

        UpdateInventoryGrid();
    }


    private void UpdateInventoryGrid()
    {
        _inventoryGrid.ClearInventoryGridAndDestroyPlacedObjects();
        OnInventoryGridsCleared?.Invoke(this, EventArgs.Empty);

        List<PlacedObjectParameters> placedObjectList = _selectedUnit.GetUnitInventory().GetPlacedObjectList();
        foreach (PlacedObjectParameters placedObjectParameters in placedObjectList)
        {
            // Создадим   
            PlacedObject placedObject = PlacedObject.CreateInGrid(placedObjectParameters, _pickUpDropPlacedObject, _inventoryGrid);
            // Попробуем добавить в сетку
            if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(placedObject.GetGridPositionAnchor(), placedObject, placedObject.GetGridSystemXY()))
            {
                OnAddPlacedObjectAtInventoryGrid?.Invoke(this, placedObject); // Запустим событие
            }
            else
            {
                UnityEngine.Object.Destroy(placedObject.gameObject);
            }
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
        OnSelectedUnitChanged?.Invoke(this, _selectedUnit); // Подписываятся кнопки выбора юнита для настройки инвенторя UnitSelectAtInventoryButton
    }

    public Unit GetSelectedUnit() { return _selectedUnit; }
}
