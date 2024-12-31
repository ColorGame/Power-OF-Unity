using System;
using System.Collections.Generic;
/// <summary>
/// Система экипировки юнитов(звено между сеткой и экипировкой юнита). Отвечает за настройку и загрузку экипировки ВЫБРАННОГО юнита.<br/>
/// Отвечает за ЛОГИКУ экипировки(размещение на сетке) оружия или брони.
/// </summary>
public class UnitEquipmentSystem :IToggleActivity
{

    public event EventHandler OnEquipmentGridsCleared; // Инвентарные сетки очищены
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtEquipmentGrid; // Объект добавлен в сетку Интвенторя

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitManager _unitManager;
    private EquipmentGrid _equipmentGrid;
    private WarehouseManager _warehouseManager;
    private ItemSelectButtonsSystemUI _itemSelectButtonsSystemUI;

    private ItemGridVisual _itemGridVisual;
    private ArmorGridVisual _armorGridVisual;

    private Unit _selectedUnit;
    private bool _isActive;


    public void Init(PickUpDropPlacedObject pickUpDropPlacedObject, UnitManager unitManager, EquipmentGrid equipmentGrid, WarehouseManager resourcesManager, ItemGridVisual itemGridVisual, ArmorGridVisual armorGridVisual, ItemSelectButtonsSystemUI itemSelectButtonsSystemUI)
    {
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitManager = unitManager;
        _equipmentGrid = equipmentGrid;  
        _itemGridVisual = itemGridVisual;
        _armorGridVisual = armorGridVisual;
        _itemSelectButtonsSystemUI = itemSelectButtonsSystemUI;
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
            if (_isActive == false) // Если до этого было отключено то
            {
                _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

                _equipmentGrid.OnAddPlacedObjectAtEquipmentGrid += EquipmentGrid_OnAddPlacedObjectAtEquipmentGrid;//  Объект добавлен в сетку Интвенторя
                _equipmentGrid.OnRemovePlacedObjectAtEquipmentGrid += EquipmentGrid_OnRemovePlacedObjectAtEquipmentGrid; // Объект удален из сетки Интвенторя
            }
            _selectedUnit = _unitManager.GetSelectedUnit();
            UpdateEquipmentGrid();
        }
        else 
        { 
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;

            _equipmentGrid.OnAddPlacedObjectAtEquipmentGrid -= EquipmentGrid_OnAddPlacedObjectAtEquipmentGrid;//  Объект добавлен в сетку Интвенторя
            _equipmentGrid.OnRemovePlacedObjectAtEquipmentGrid -= EquipmentGrid_OnRemovePlacedObjectAtEquipmentGrid; // Объект удален из сетки Интвенторя
            ClearEquipmentGrid();
        }
        _isActive = active;
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
            placedObject.SetDropPositionWhenDeleted(_itemSelectButtonsSystemUI.GetWeaponSelectContainerTransform().position); // Пердадим середину контейнера оружия (можно передать любой контейнер т.к. они визуально распологаются в одном месте)
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
    private void EquipmentGrid_OnAddPlacedObjectAtEquipmentGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// Удалим полученный объект из Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void EquipmentGrid_OnRemovePlacedObjectAtEquipmentGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().RemovePlacedObjectList(placedObject);

        // Если это бронежилет то
        if (placedObject.GetPlacedObjectTypeSO() is BodyArmorTypeSO)
        {
            RemoveHeadArmorInEquipmentGrid();
        }
    }

    /// <summary>
    /// Удалить ШЛЕМ из экипировки
    /// </summary>
    private void RemoveHeadArmorInEquipmentGrid()
    {
        // СНЯТЬ ШЛЕМ если он есть И ВЕРНУТЬ НА БАЗУ           
        if (_equipmentGrid.TryGetPlacedObjectByType<HeadArmorTypeSO>(out PlacedObject placedObjectHeadArmor))
        {
            _equipmentGrid.RemovePlacedObjectAtGrid(placedObjectHeadArmor);// Удалим из текущей сеточной позиции  (внутри сработает событие OnRemovePlacedObjectAtEquipmentGrid)   
            _warehouseManager.PlusCountPlacedObject(placedObjectHeadArmor.GetPlacedObjectTypeSO());
            placedObjectHeadArmor.SetFlagMoveStartPosition(true);           
        }
    }

    public void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
