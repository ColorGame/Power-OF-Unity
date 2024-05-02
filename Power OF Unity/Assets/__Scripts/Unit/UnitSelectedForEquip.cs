using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  ���� �������� ������� ��� ���������� � ��������� ���������
/// </summary>
public class UnitSelectedForEquip 
{
    public UnitSelectedForEquip()// ����������� ��� �� ��������� ���������� ��������� new T() �� ������ ���� ����
    {
    }

   [SerializeField] private UnitInventory _selectedUnitEquipment; // �������� ��������� ����� ��������� ��� ������� �� UnitManager ������ � ������


    private PickUpDrop _pickUpDrop;

    public void Initialize(PickUpDrop pickUpDrop)
    {       
        _pickUpDrop = pickUpDrop;

        _pickUpDrop.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  ������ �������� � ����� ����������
        _pickUpDrop.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������
    }

    
    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>
    private void PickUpDrop_OnAddPlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnitEquipment.AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// ������ ���������� ������ �� ������ "����������� �������� � ����� ���������".
    /// </summary>
    private void PickUpDrop_OnRemovePlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnitEquipment.RemovePlacedObjectList(placedObject);
    }


    public void SetSelectedUnit(UnitInventory unitEquipment)
    {
        _selectedUnitEquipment = unitEquipment;
        Debug.Log($"������ -{_selectedUnitEquipment.name} ");
    }
}
