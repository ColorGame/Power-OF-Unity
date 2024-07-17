using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// ������� ��������� ������(����� ����� ������ � ���������� �����). �������� �� ��������� � �������� ��������� ���������� �����
/// </summary>
public class UnitInventorySystem
{
    public event EventHandler<Unit> OnSelectedUnitChanged; // ������� ��������� ����
    public event EventHandler OnInventoryGridsCleared; // ����������� ����� �������
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // ������ �������� � ����� ����������

    private Unit _selectedUnit;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitManager _unitManager;
    private InventoryGrid _inventoryGrid;


    public void Init(PickUpDropPlacedObject pickUpDropPlacedObject, UnitManager unitManager, InventoryGrid inventoryGrid)
    {
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitManager = unitManager;
        _inventoryGrid = inventoryGrid;

        _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  ������ �������� � ����� ����������
        _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������

        SelectUnitAtStart();
    }

    /// <summary>
    /// ������� ����� ��� ������
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

    public void SetSelectedUnit(Unit selectedUnit)
    {
        _selectedUnit = selectedUnit;
        UpdateInventoryGrid();
        OnSelectedUnitChanged?.Invoke(this, _selectedUnit); // ������������� ������ ������ ����� ��� ��������� ��������� UnitSelectAtInventoryButton
    }

    public Unit GetSelectedUnit() { return _selectedUnit; }
}
