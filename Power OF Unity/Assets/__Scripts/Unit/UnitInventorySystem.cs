using System;
using System.Collections.Generic;


/// <summary>
/// ������� ��������� ������(����� ����� ������ � ���������� �����). �������� �� ��������� � �������� ��������� ���������� �����
/// </summary>
public class UnitInventorySystem :IToggleActivity
{

    public event EventHandler OnInventoryGridsCleared; // ����������� ����� �������
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // ������ �������� � ����� ����������

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
       _inventoryGridVisual.SetActive(active); // ������� ���������� ������ ��� �� �� ������ ��� �������� (��� ���� ��� �� ��������� )
        if (active)
        {
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

            _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  ������ �������� � ����� ����������
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������

            _selectedUnit = _unitManager.GetSelectedUnit();
            UpdateInventoryGrid();
        }
        else 
        { 
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;

            _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid -= PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  ������ �������� � ����� ����������
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid -= PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������
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
    /// ��������� ����� ���������
    /// </summary>
    private void FillInventoryGrid()
    {
        List<PlacedObjectParameters> placedObjectList = _selectedUnit.GetUnitInventory().GetPlacedObjectList();
        foreach (PlacedObjectParameters placedObjectParameters in placedObjectList)
        {
            // ��������   
            PlacedObject placedObject = PlacedObject.CreateInGrid(placedObjectParameters, _pickUpDropPlacedObject, _inventoryGrid);
            // ��������� �������� � �����
            if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(placedObject.GetGridPositionAnchor(), placedObject, placedObject.GetGridSystemXY()))
            {
                OnAddPlacedObjectAtInventoryGrid?.Invoke(this, placedObject); // �������� �������
            }
            else
            {
                UnityEngine.Object.Destroy(placedObject.gameObject);
            }
        }
    }

    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>
    private void PickUpDrop_OnAddPlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitInventory().AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// ������ ���������� ������ �� ������ "����������� �������� � ����� ���������".
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
