using UnityEngine;


/// <summary>
///  ���� �������� ������� ��� ���������� � ��������� ���������
/// </summary>
public class UnitSelectedForEquip 
{

    private Unit _selectedUnit; //
    private PickUpDrop _pickUpDrop;

    public void Init(PickUpDrop pickUpDrop)
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
