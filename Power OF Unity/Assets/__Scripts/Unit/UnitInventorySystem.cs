using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ������� ��������� �����. �������� �� ��������� � �������� ��������� ���������� �����
/// </summary>
public class UnitInventorySystem 
{

    private Unit _selectedUnit;
    private PickUpDropPlacedObject _pickUpDrop;
    private UnitManager _unitManager;
    private InventoryGrid _inventoryGrid;

    public void Init(PickUpDropPlacedObject pickUpDrop, UnitManager unitManager, InventoryGrid inventoryGrid)
    {       
        _pickUpDrop = pickUpDrop;
        _unitManager = unitManager;
        _inventoryGrid = inventoryGrid;

        _pickUpDrop.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  ������ �������� � ����� ����������
        _pickUpDrop.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������

        SelectUnitAtStart();
    }

    private void SelectUnitAtStart()
    {
        List<Unit> UnitFriendList = _unitManager.GetUnitFriendList();
        if (UnitFriendList.Count!=0)
        {
            _selectedUnit = UnitFriendList[0];
        }       
        List<PlacedObjectParameters> placedObjectList = _selectedUnit.GetUnitInventory().GetPlacedObjectList();
        _inventoryGrid.ClearInventoryGridAndDestroyPlacedObjects();
        foreach (PlacedObjectParameters placedObjectParameters in placedObjectList)
        {
           // PlacedObject placedObject = PlacedObject.CreateInGrid(_inventoryGrid.GetG)
        }
        //
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
        Debug.Log($"������ -{_selectedUnit.GetUnitTypeSO<UnitTypeSO>().GetName()} ");
    }
}
