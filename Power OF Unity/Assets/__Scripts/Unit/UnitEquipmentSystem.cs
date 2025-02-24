using System;
using System.Collections.Generic;
using static EquipmentGrid;
/// <summary>
/// ������� ���������� ������(����� ����� ������ � ����������� �����). �������� �� ��������� � �������� ���������� ���������� �����.<br/>
/// �������� �� ������ ����������(���������� �� �����) ������ ��� �����.
/// </summary>
public class UnitEquipmentSystem : IToggleActivity
{

    public event EventHandler OnEquipmentGridsCleared; // ����������� ����� �������   
    //public event EventHandler OnClearingSlotFromAnotherPlacedObject; //��� ������� ����� ��� ������� ������������ �������  

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
        ClearEquipmentGrid();//����� �������������, ������� ����. �����
        _equipmentGrid.SetActiveGrid(gridState);
        SetItemArmorGrid(gridState);
        UpdateEquipmentGrid();
    }

    public void SetActive(bool active)
    {
        if (_isActive == active) //���� ���������� ��������� ���� �� �������
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
    /// ��������� ����� ����������
    /// </summary>
    private void FillEquipmentGrid()
    {
        List<PlacedObjectGridParameters> equipmentList = _selectedUnit.GetUnitEquipment().GetEquipmentList();

        for (int index = 0; index < equipmentList.Count; index++)
        {
            // ������� �������� �������� �������  
            GridSystemXY<GridObjectEquipmentXY> gridSystemXY = _equipmentGrid.GetActiveGridSystemXY(equipmentList[index].slot);
            if (gridSystemXY == null)
                continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����

            PlacedObject placedObject = PlacedObject.CreateInGrid(equipmentList[index], _pickUpDropPlacedObject, gridSystemXY);
            placedObject.SetDropPositionWhenDeleted(_itemSelectButtonsSystemUI.GetWeaponSelectContainerTransform().position); // �������� �������� ���������� ������ (����� �������� ����� ��������� �.�. ��� ��������� ������������� � ����� �����)
            // ����������� PlacedObjectGridParameters � ����� ���������� - placedObject
            equipmentList[index] = new PlacedObjectGridParameters(
                equipmentList[index].slot,
                equipmentList[index].gridPositioAnchor,
                equipmentList[index].placedObjectTypeSO,
                placedObject);

            // ��������� �������� � �����
            if (!_equipmentGrid.TryAddPlacedObjectAtGridPosition(placedObject.GetGridPositionAnchor(), placedObject, placedObject.GetGridSystemXY()))
                UnityEngine.Object.Destroy(placedObject.gameObject); // �� ������ ������      
        }
    }
    /// <summary>
    /// ������� ����������� ������ � ���������� �����.  
    /// </summary>
    public void AddPlacedObjectAtUnitEquipment(PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().AddPlacedObjectAtEquipmentList(placedObject);//�������� ���������� ������ � ���������� �����.     
    }

    /// <summary>
    /// ������� ����������� ������ �� ����� � ���������� �����. � ���������<br/>
    /// !!! ���� ���� ������� � ������� ������� � ��������� ������� �� ��������� ��� bool ���������� .
    /// </summary>
    public void RemoveFromGridAndUnitEquipmentWithCheck(PlacedObject placedObject, bool returnInResourcesAndStartPosition = false)
    {
        RemoveFromGridAndUnitEquipment(placedObject, returnInResourcesAndStartPosition);

        // ���� ��� ���������� ��
        if (placedObject.GetPlacedObjectTypeSO() is BodyArmorTypeSO)
        {
            // ����� ���� ���� �� ���� � ������� �� ����           
            if (_equipmentGrid.TryGetPlacedObjectByType<HeadArmorTypeSO>(out PlacedObject placedObjectHeadArmor))
            {
                RemoveFromGridAndUnitEquipment(placedObjectHeadArmor, returnInResourcesAndStartPosition: true);
            }
        }
    }
    /// <summary>
    /// ������� ����������� ������ �� ����� � ���������� �����.<br/>
    /// !!! ���� ���� ������� � ������� ������� � ��������� ������� �� ��������� ��� bool ���������� .
    /// </summary>
    private void RemoveFromGridAndUnitEquipment(PlacedObject placedObject, bool returnInResourcesAndStartPosition = false)
    {
        _equipmentGrid.RemovePlacedObjectAtGrid(placedObject, returnInResourcesAndStartPosition);// ������ �� ������� �������� �������      
        _selectedUnit.GetUnitEquipment().RemovePlacedObjectAtEquipmentList(placedObject);
        if (returnInResourcesAndStartPosition)
            ReturnPlacedObjectInResourcesAndStartPosition(placedObject);
    }

    /// <summary>
    /// ������� ����������� ������ � ������� ������� � ��������� �������
    /// </summary>
    public void ReturnPlacedObjectInResourcesAndStartPosition(PlacedObject placedObject)
    {
        _warehouseManager.PlusCountPlacedObject(placedObject.GetPlacedObjectTypeSO());
        placedObject.SetFlagMoveStartPosition(true);
    }

    /// <summary>
    /// �������� ���� ��� ������� ������������ �������
    /// </summary>
    public void CleanSlotFromAnotherPlacedObject(EquipmentSlot equipmentSlot)
    {
        PlacedObject placedObject = _selectedUnit.GetUnitEquipment().GetPlacidObjectFromSinglSlot(equipmentSlot);
        if (placedObject != null)
        {
            _selectedUnit.GetUnitAnimator().SetSkipCurrentChangeWeaponEvent(true);
            _selectedUnit.GetUnitEquipsViewFarm().SetSkipCurrentChangeWeaponEvent(true);
            //OnClearingSlotFromAnotherPlacedObject?.Invoke(this,EventArgs.Empty); // ������� �������� ������� ����� ����� UnitAnimator � UnitEquipViewFarm ������ �����������
            RemoveFromGridAndUnitEquipmentWithCheck(placedObject, returnInResourcesAndStartPosition: true);
        }
    }

    /// <summary>
    /// ���������� � ������� ���������� ����������
    /// </summary>
    public bool CompatibleWithOtherEquipment(PlacedObject placedObject)
    {
        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();
        switch (placedObjectTypeSO)
        {
            case HeadArmorTypeSO headArmorTypeSO: // ���� ��� ���� �� �������� �� ������������� � ������ ��� ����
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
