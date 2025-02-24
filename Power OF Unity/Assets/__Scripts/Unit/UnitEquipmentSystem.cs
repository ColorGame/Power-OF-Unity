using System;
using System.Collections.Generic;
using static EquipmentGrid;
/// <summary>
/// Система экипировки юнитов(звено между сеткой и экипировкой юнита). Отвечает за настройку и загрузку экипировки ВЫБРАННОГО юнита.<br/>
/// Отвечает за ЛОГИКУ экипировки(размещение на сетке) оружия или брони.
/// </summary>
public class UnitEquipmentSystem : IToggleActivity
{

    public event EventHandler OnEquipmentGridsCleared; // Инвентарные сетки очищены   
    //public event EventHandler OnClearingSlotFromAnotherPlacedObject; //При Очистке слота для другого размещаемого объекта  

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitManager _unitManager;
    private EquipmentGrid _equipmentGrid;
    private WarehouseManager _warehouseManager;
    private ItemSelectButtonsSystemUI _itemSelectButtonsSystemUI;

    private ItemGridVisual _itemGridVisual;
    private ArmorGridVisual _armorGridVisual;

    private Unit _selectedUnit;
    private bool _isActive=true;


    public void Init(PickUpDropPlacedObject pickUpDropPlacedObject, UnitManager unitManager, EquipmentGrid equipmentGrid, WarehouseManager warehouseManager, ItemGridVisual itemGridVisual, ArmorGridVisual armorGridVisual, ItemSelectButtonsSystemUI itemSelectButtonsSystemUI)
    {
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitManager = unitManager;
        _equipmentGrid = equipmentGrid;
        _warehouseManager = warehouseManager;
        _itemGridVisual = itemGridVisual;
        _armorGridVisual = armorGridVisual;
        _itemSelectButtonsSystemUI = itemSelectButtonsSystemUI;
    }

    public void SetActiveEquipmentGrid(GridState gridState)
    {
        ClearEquipmentGrid();//Перед переключением, очистим пред. сетку
        _equipmentGrid.SetActiveGrid(gridState);
        SetItemArmorGrid(gridState);
        UpdateEquipmentGrid();
    }

    public void SetActive(bool active)
    {
        if (_isActive == active) //Если предыдущее состояние тоже то выходим
            return;

        _isActive = active;

        SetItemArmorGrid(_equipmentGrid.GetStateGrid(), active);
       
        if (active)
        {
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
            _selectedUnit = _unitManager.GetSelectedUnit();
            UpdateEquipmentGrid();
        }
        else
        {
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
            ClearEquipmentGrid();
        }
    }

    private void SetItemArmorGrid(GridState gridState, bool active = true)
    {
        if (active)
        {
            switch (gridState)
            {
                case GridState.ItemGrid:
                    _itemGridVisual.Enable();
                    _armorGridVisual.Disable();
                    break;

                case GridState.ArmorGrid:
                    _armorGridVisual.Enable();
                    _itemGridVisual.Disable();
                    break;
            }
        }
        else
        {
            _itemGridVisual.Disable();
            _armorGridVisual.Disable();
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
        List<PlacedObjectGridParameters> equipmentList = _selectedUnit.GetUnitEquipment().GetEquipmentList();

        for (int index = 0; index < equipmentList.Count; index++)
        {
            // Получим активную сеточную систему  
            GridSystemXY<GridObjectEquipmentXY> gridSystemXY = _equipmentGrid.GetActiveGridSystemXY(equipmentList[index].slot);
            if (gridSystemXY == null)
                continue; // continue заставляет программу переходить к следующей итерации цикла 'for' игнорируя код ниже

            PlacedObject placedObject = PlacedObject.CreateInGrid(equipmentList[index], _pickUpDropPlacedObject, gridSystemXY);
            placedObject.SetDropPositionWhenDeleted(_itemSelectButtonsSystemUI.GetWeaponSelectContainerTransform().position); // Пердадим середину контейнера оружия (можно передать любой контейнер т.к. они визуально распологаются в одном месте)
            // Перезапишем PlacedObjectGridParameters с новым параметром - placedObject
            equipmentList[index] = new PlacedObjectGridParameters(
                equipmentList[index].slot,
                equipmentList[index].gridPositioAnchor,
                equipmentList[index].placedObjectTypeSO,
                placedObject);

            // Попробуем добавить в сетку
            if (!_equipmentGrid.TryAddPlacedObjectAtGridPosition(placedObject.GetGridPositionAnchor(), placedObject, placedObject.GetGridSystemXY()))
                UnityEngine.Object.Destroy(placedObject.gameObject); // на всякий случай      
        }
    }
    /// <summary>
    /// Добавть размещенный объект в экипировку юнита.  
    /// </summary>
    public void AddPlacedObjectAtUnitEquipment(PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().AddPlacedObjectAtEquipmentList(placedObject);//Добавить полученный объект в экипировку юнита.     
    }

    /// <summary>
    /// Удалить размещенный объект из сетки и экипировки юнита. С ПРОВЕРКОЙ<br/>
    /// !!! Если надо ВЕРНУТЬ в разделе ресурсы и стартовую позицию то передадим доп bool переменную .
    /// </summary>
    public void RemoveFromGridAndUnitEquipmentWithCheck(PlacedObject placedObject, bool returnInResourcesAndStartPosition = false)
    {
        RemoveFromGridAndUnitEquipment(placedObject, returnInResourcesAndStartPosition);

        // Если это бронежилет то
        if (placedObject.GetPlacedObjectTypeSO() is BodyArmorTypeSO)
        {
            // СНЯТЬ ШЛЕМ если он есть И ВЕРНУТЬ НА БАЗУ           
            if (_equipmentGrid.TryGetPlacedObjectByType<HeadArmorTypeSO>(out PlacedObject placedObjectHeadArmor))
            {
                RemoveFromGridAndUnitEquipment(placedObjectHeadArmor, returnInResourcesAndStartPosition: true);
            }
        }
    }
    /// <summary>
    /// Удалить размещенный объект из сетки и экипировки юнита.<br/>
    /// !!! Если надо ВЕРНУТЬ в разделе ресурсы и стартовую позицию то передадим доп bool переиенную .
    /// </summary>
    private void RemoveFromGridAndUnitEquipment(PlacedObject placedObject, bool returnInResourcesAndStartPosition = false)
    {
        _equipmentGrid.RemovePlacedObjectAtGrid(placedObject, returnInResourcesAndStartPosition);// Удалим из текущей сеточной позиции      
        _selectedUnit.GetUnitEquipment().RemovePlacedObjectAtEquipmentList(placedObject);
        if (returnInResourcesAndStartPosition)
            ReturnPlacedObjectInResourcesAndStartPosition(placedObject);
    }

    /// <summary>
    /// Вернуть размещенный объект в разделе ресурсы и стартовую позицию
    /// </summary>
    public void ReturnPlacedObjectInResourcesAndStartPosition(PlacedObject placedObject)
    {
        _warehouseManager.PlusCountPlacedObject(placedObject.GetPlacedObjectTypeSO());
        placedObject.SetFlagMoveStartPosition(true);
    }

    /// <summary>
    /// Очистить слот для другого размещаемого объекта
    /// </summary>
    public void CleanSlotFromAnotherPlacedObject(EquipmentSlot equipmentSlot)
    {
        PlacedObject placedObject = _selectedUnit.GetUnitEquipment().GetPlacidObjectFromSinglSlot(equipmentSlot);
        if (placedObject != null)
        {
            _selectedUnit.GetUnitAnimator().SetSkipCurrentChangeWeaponEvent(true);
            _selectedUnit.GetUnitEquipsViewFarm().SetSkipCurrentChangeWeaponEvent(true);
            //OnClearingSlotFromAnotherPlacedObject?.Invoke(this,EventArgs.Empty); // Сначала запустим событие чтобы класс UnitAnimator и UnitEquipViewFarm смогли настроиться
            RemoveFromGridAndUnitEquipmentWithCheck(placedObject, returnInResourcesAndStartPosition: true);
        }
    }

    /// <summary>
    /// Совместима с другими предметами экипировки
    /// </summary>
    public bool CompatibleWithOtherEquipment(PlacedObject placedObject)
    {
        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();
        switch (placedObjectTypeSO)
        {
            case HeadArmorTypeSO headArmorTypeSO: // Если это шлем то проверим на совместимость с броней для тела
                if (!_selectedUnit.GetUnitEquipment().IsHeadArmorCompatibleWithBodyArmor(headArmorTypeSO))
                    return false;
                break;
        }
        return true;
    }


    public void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
