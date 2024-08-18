using System;
using System.Collections.Generic;


/// <summary>
/// Система инвенторя юнитов(звено между сеткой и инвентарем юнита). Отвечает за настройку и загрузку инвенторя ВЫБРАННОГО юнита
/// </summary>
public class UnitInventorySystem :IToggleActivity
{

    public event EventHandler OnInventoryGridsCleared; // Инвентарные сетки очищены
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // Объект добавлен в сетку Интвенторя

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitManager _unitManager;
    private InventoryGrid _inventoryGrid;
    private InventoryGridVisual _inventoryGridVisual;
    private Unit _selectedUnit;


    public void Init(PickUpDropPlacedObject pickUpDropPlacedObject, UnitManager unitManager, InventoryGrid inventoryGrid, InventoryGridVisual inventoryGridVisual)
    {
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitManager = unitManager;
        _inventoryGrid = inventoryGrid;
        _inventoryGridVisual = inventoryGridVisual;
    }

   

    public void SetActive(bool active)
    {
       _inventoryGridVisual.SetActive(active); // Сначала активируем визуал что бы он сделал все подписки (при откл это не актуально )
        if (active)
        {
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

            _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  Объект добавлен в сетку Интвенторя
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя

            _selectedUnit = _unitManager.GetSelectedUnit();
            UpdateInventoryGrid();
        }
        else 
        { 
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;

            _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid -= PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  Объект добавлен в сетку Интвенторя
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid -= PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя
            ClearInventoryGrid();
        }
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        _selectedUnit = newSelectedUnit;
        UpdateInventoryGrid();
    }

    private void UpdateInventoryGrid()
    {
        ClearInventoryGrid();

        if (_selectedUnit != null)
        {
            FillInventoryGrid();
        }
    }

    private void ClearInventoryGrid()
    {
        _inventoryGrid.ClearInventoryGridAndDestroyPlacedObjects();
        OnInventoryGridsCleared?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Заполнить сетку инвенторя
    /// </summary>
    private void FillInventoryGrid()
    {
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

    public void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
