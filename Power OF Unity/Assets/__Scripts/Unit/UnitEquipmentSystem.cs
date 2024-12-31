using System;
using System.Collections.Generic;
/// <summary>
/// ������� ���������� ������(����� ����� ������ � ����������� �����). �������� �� ��������� � �������� ���������� ���������� �����.<br/>
/// �������� �� ������ ����������(���������� �� �����) ������ ��� �����.
/// </summary>
public class UnitEquipmentSystem :IToggleActivity
{

    public event EventHandler OnEquipmentGridsCleared; // ����������� ����� �������
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtEquipmentGrid; // ������ �������� � ����� ����������

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
        ClearEquipmentGrid();//����� �������������, ������� ����. �����
        _equipmentGrid.SetActiveGrid(activeGridList);
    }

    public void SetActive(bool active)
    {
        _itemGridVisual.SetActive(active);
        _armorGridVisual.SetActive(active);
        if (active)
        {
            if (_isActive == false) // ���� �� ����� ���� ��������� ��
            {
                _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

                _equipmentGrid.OnAddPlacedObjectAtEquipmentGrid += EquipmentGrid_OnAddPlacedObjectAtEquipmentGrid;//  ������ �������� � ����� ����������
                _equipmentGrid.OnRemovePlacedObjectAtEquipmentGrid += EquipmentGrid_OnRemovePlacedObjectAtEquipmentGrid; // ������ ������ �� ����� ����������
            }
            _selectedUnit = _unitManager.GetSelectedUnit();
            UpdateEquipmentGrid();
        }
        else 
        { 
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;

            _equipmentGrid.OnAddPlacedObjectAtEquipmentGrid -= EquipmentGrid_OnAddPlacedObjectAtEquipmentGrid;//  ������ �������� � ����� ����������
            _equipmentGrid.OnRemovePlacedObjectAtEquipmentGrid -= EquipmentGrid_OnRemovePlacedObjectAtEquipmentGrid; // ������ ������ �� ����� ����������
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
    /// ��������� ����� ����������
    /// </summary>
    private void FillEquipmentGrid()
    {
        List<PlacedObjectGridParameters> placedObjectList = _selectedUnit.GetUnitEquipment().GetPlacedObjectList();
        foreach (PlacedObjectGridParameters placedObjectGridParameters in placedObjectList)
        {
            // ������� �������� �������� �������  
            GridSystemXY<GridObjectEquipmentXY> gridSystemXY = _equipmentGrid.GetActiveGridSystemXY(placedObjectGridParameters.slot);
            if (gridSystemXY == null) 
            {
                continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
            }
            PlacedObject placedObject = PlacedObject.CreateInGrid(placedObjectGridParameters, _pickUpDropPlacedObject, gridSystemXY);           
            placedObject.SetDropPositionWhenDeleted(_itemSelectButtonsSystemUI.GetWeaponSelectContainerTransform().position); // �������� �������� ���������� ������ (����� �������� ����� ��������� �.�. ��� ��������� ������������� � ����� �����)
            // ��������� �������� � �����
            if (_equipmentGrid.TryAddPlacedObjectAtGridPosition(placedObject.GetGridPositionAnchor(), placedObject, placedObject.GetGridSystemXY()))
            {
                OnAddPlacedObjectAtEquipmentGrid?.Invoke(this, placedObject); // �������� �������
            }
            else
            {
                UnityEngine.Object.Destroy(placedObject.gameObject); // �� ������ ������
            }
        }
    }

    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>
    private void EquipmentGrid_OnAddPlacedObjectAtEquipmentGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// ������ ���������� ������ �� ������ "����������� �������� � ����� ���������".
    /// </summary>
    private void EquipmentGrid_OnRemovePlacedObjectAtEquipmentGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitEquipment().RemovePlacedObjectList(placedObject);

        // ���� ��� ���������� ��
        if (placedObject.GetPlacedObjectTypeSO() is BodyArmorTypeSO)
        {
            RemoveHeadArmorInEquipmentGrid();
        }
    }

    /// <summary>
    /// ������� ���� �� ����������
    /// </summary>
    private void RemoveHeadArmorInEquipmentGrid()
    {
        // ����� ���� ���� �� ���� � ������� �� ����           
        if (_equipmentGrid.TryGetPlacedObjectByType<HeadArmorTypeSO>(out PlacedObject placedObjectHeadArmor))
        {
            _equipmentGrid.RemovePlacedObjectAtGrid(placedObjectHeadArmor);// ������ �� ������� �������� �������  (������ ��������� ������� OnRemovePlacedObjectAtEquipmentGrid)   
            _warehouseManager.PlusCountPlacedObject(placedObjectHeadArmor.GetPlacedObjectTypeSO());
            placedObjectHeadArmor.SetFlagMoveStartPosition(true);           
        }
    }

    public void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
