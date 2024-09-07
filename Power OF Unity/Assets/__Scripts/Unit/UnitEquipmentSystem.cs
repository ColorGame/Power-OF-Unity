using System;
using System.Collections.Generic;


/// <summary>
/// Система экипировки юнитов(звено между сеткой и экипировкой юнита). Отвечает за настройку и загрузку экипировки ВЫБРАННОГО юнита
/// </summary>
public class UnitEquipmentSystem :IToggleActivity
{

    public event EventHandler OnEquipmentGridsCleared; // Инвентарные сетки очищены
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtEquipmentGrid; // Объект добавлен в сетку Интвенторя

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitManager _unitManager;
    private EquipmentGrid _equipmentGrid;

    private ItemGridVisual _itemGridVisual;
    private ArmorGridVisual _armorGridVisual;

    private Unit _selectedUnit;


    public void Init(PickUpDropPlacedObject pickUpDropPlacedObject, UnitManager unitManager, EquipmentGrid equipmentGrid, ItemGridVisual itemGridVisual, ArmorGridVisual armorGridVisual)
    {
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitManager = unitManager;
        _equipmentGrid = equipmentGrid;  
        _itemGridVisual = itemGridVisual;
        _armorGridVisual = armorGridVisual;
    }

    public void SetActiveEquipmentGrid(EquipmentGrid.GridState activeGridList) 
    {
        ClearEquipmentGrid();//Перед переключением, очистим пред. сетку
        _equipmentGrid.SetActiveGrid(activeGridList);
    }

    public void SetActive(bool active)
    {
        _itemGridVisual.SetActive(active);
        _armorGridVisual.SetActive(active);
        if (active)
        {
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid += PickUpDrop_OnAddPlacedObjectAtEquipmentGrid;//  Объект добавлен в сетку Интвенторя
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid += PickUpDrop_OnRemovePlacedObjectAtEquipmentGrid; // Объект удален из сетки Интвенторя

            _selectedUnit = _unitManager.GetSelectedUnit();
            UpdateEquipmentGrid();
        }
        else 
        { 
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;

            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid -= PickUpDrop_OnAddPlacedObjectAtEquipmentGrid;//  Объект добавлен в сетку Интвенторя
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid -= PickUpDrop_OnRemovePlacedObjectAtEquipmentGrid; // Объект удален из сетки Интвенторя
            ClearEquipmentGrid();
        }
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        _selectedUnit = newSelectedUnit;
        UpdateEquipmentGrid();
    }

    private void UpdateEquipmentGrid()
    {
        ClearEquipmentGrid();

        if (_selectedUnit != null)
        {
            FillEquipmentGrid();
        }
    }

    private void ClearEquipmentGrid()
    {
        _equipmentGrid.ClearEquipmentGridAndDestroyPlacedObjects();
        OnEquipmentGridsCleared?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Заполнить сетку экипировки
    /// </summary>
    private void FillEquipmentGrid()
    {
        List<PlacedObjectGridParameters> placedObjectList = _selectedUnit.GetUnitEquipment().GetPlacedObjectList();
        foreach (PlacedObjectGridParameters placedObjectGridParameters in placedObjectList)
        {
            // Получим активную сеточную систему  
            GridSystemXY<GridObjectEquipmentXY> gridSystemXY = _equipmentGrid.GetActiveGridSystemXY(placedObjectGridParameters.slot);
            if (gridSystemXY == null) 
            {
                continue; // continue заставляет программу переходить к следующей итерации цикла 'for' игнорируя код ниже
            }
            PlacedObject placedObject = PlacedObject.CreateInGrid(placedObjectGridParameters, _pickUpDropPlacedObject, gridSystemXY);
            // Попробуем добавить в сетку
            if (_equipmentGrid.TryAddPlacedObjectAtGridPosition(placedObject.GetGridPositionAnchor(), placedObject, placedObject.GetGridSystemXY()))
            {
                OnAddPlacedObjectAtEquipmentGrid?.Invoke(this, placedObject); // Запустим событие
            }
            else
            {
                UnityEngine.Object.Destroy(placedObject.gameObject); // на всякий случай
            }
        }
    }

    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnAddPlacedObjectAtEquipmentGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// Удалим полученный объект из Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnRemovePlacedObjectAtEquipmentGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().RemovePlacedObjectList(placedObject);
    }

    public void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
