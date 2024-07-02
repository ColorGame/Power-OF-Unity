using System;
using System.Collections.Generic;

/// <summary>
/// ��������� ����� (��������� ��� ���������, �������� ������� �����). 
/// </summary>
public class UnitInventory
{
    /// <summary>
    /// ��������� ����� (��������� ��� ���������, �������� ������� �����). 
    /// </summary>
    public UnitInventory(Unit unit)
    {
        _unit = unit;
        _placedObjectList = new List<PlacedObjectParameters>();
    }

    public event EventHandler<PlacedObject> OnAddPlacedObjectList; // ������� �������� ������� � ���������
    public event EventHandler<PlacedObject> OnRemovePlacedObjectList; // ������� ������ ������� �� ���������

    private Unit _unit;
    private List<PlacedObjectParameters> _placedObjectList = new List<PlacedObjectParameters>(); // ������ ��������� ���������

    private PlacedObjectTypeSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeSO> _placedObjec�BagSlotList;
    private List<GrenadeTypeSO> _grenadeInBagSOList = new List<GrenadeTypeSO>(); // ������ ������ � ������

    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {
        PlacedObjectParameters placedObjectParameters= placedObject.GetPlacedObjectParameters();
        _placedObjectList.Add(placedObjectParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectParameters.slot)
        {            
            case InventorySlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = placedObjectTypeSO;
                break;
            case InventorySlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = placedObjectTypeSO;
                break;
            case InventorySlot.BagSlot:
                _placedObjec�BagSlotList.Add(placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Add(grenadeTypeSO);
                }
                break;
        }
           
        OnAddPlacedObjectList?.Invoke(this, placedObject);
    }
    /// <summary>
    /// ������� ���������� ������ �� ������� "����������� �������� � ����� ���������".
    /// </summary>  
    public void RemovePlacedObjectList(PlacedObject placedObject)
    {
        PlacedObjectParameters placedObjectParameters = placedObject.GetPlacedObjectParameters();
        _placedObjectList.Remove(placedObjectParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectParameters.slot)
        {
            case InventorySlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = null;
                break;
            case InventorySlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = null;
                break;
            case InventorySlot.BagSlot:
                _placedObjec�BagSlotList.Remove(placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Remove(grenadeTypeSO);
                }
                break;
        }      

        OnRemovePlacedObjectList?.Invoke(this, placedObject);
    }

    public PlacedObjectTypeSO GetPlacedObjectOtherWeaponSlot() { return _placedObjectOtherWeaponSlot; }
    public PlacedObjectTypeSO GetPlacedObjectMainWeaponSlot() { return _placedObjectMainWeaponSlot; }
    public List<PlacedObjectTypeSO > GetPlacedObjectBagSlotList() { return _placedObjec�BagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagSOList; }
    public List<PlacedObjectParameters> GetPlacedObjectList() {  return _placedObjectList; }


}
