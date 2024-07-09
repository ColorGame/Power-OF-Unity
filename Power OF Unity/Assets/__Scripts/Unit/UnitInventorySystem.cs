using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ������� ��������� ������(����� ����� ������ � ���������� �����). �������� �� ��������� � �������� ��������� ���������� �����
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
            // �������� � ��������� ����������   
            PlacedObject.CreateAddTryPlacedInGrid(placedObjectParameters, _inventoryGrid, _pickUpDropPlacedObject);
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
        Debug.Log($"������ -{_selectedUnit.GetUnitTypeSO<UnitTypeSO>().GetName()} ");
    }
}
