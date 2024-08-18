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
    public UnitInventory()
    {
        //����� ����� ��������� ����������� �������� ���������
        _placedObjectList = new List<PlacedObjectParameters>();
    }

    private List<PlacedObjectParameters> _placedObjectList; // ������ ��������� ���������

    private PlacedObjectTypeWithActionSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeWithActionSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeWithActionSO> _placedObjec�BagSlotList = new List<PlacedObjectTypeWithActionSO>();
    private List<GrenadeTypeSO> _grenadeInBagSOList = new List<GrenadeTypeSO>(); // ������ ������ � ������

    private PlacedObjectTypeArmorSO _placedObjectArmorHeadSlot;
    private PlacedObjectTypeArmorSO _placedObjectArmorBodySlot;

    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {
        PlacedObjectParameters placedObjectParameters = placedObject.GetPlacedObjectParameters();
        _placedObjectList.Add(placedObjectParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectParameters.slot)
        {
            case InventorySlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                break;
            case InventorySlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                break;
            case InventorySlot.BagSlot:
                _placedObjec�BagSlotList.Add((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Add(grenadeTypeSO);
                }
                break;
            case InventorySlot.ArmorHeadSlot:
                _placedObjectArmorHeadSlot = (PlacedObjectTypeArmorSO)placedObjectTypeSO;
                break;
            case InventorySlot.ArmorBodySlot:
                _placedObjectArmorBodySlot = (PlacedObjectTypeArmorSO)placedObjectTypeSO;
                break;
        }

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
                _placedObjec�BagSlotList.Remove((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Remove(grenadeTypeSO);
                }
                break;
            case InventorySlot.ArmorHeadSlot:
                _placedObjectArmorHeadSlot = null;
                break;
            case InventorySlot.ArmorBodySlot:
                _placedObjectArmorBodySlot = null;
                break;
        }
    }
    /// <summary>
    /// ����� ���� ��������� ���������� � ������� �� ����
    /// </summary>
    public void RemoveAllInventory()
    {
        // ����������� ������ �������� ���������
        _placedObjectMainWeaponSlot = null;
        _placedObjectOtherWeaponSlot = null;
        _placedObjec�BagSlotList.Clear();
        _grenadeInBagSOList.Clear();

        _placedObjectArmorHeadSlot = null;
        _placedObjectArmorBodySlot = null;
    }


    public PlacedObjectTypeWithActionSO GetPlacedObjectOtherWeaponSlot() { return _placedObjectOtherWeaponSlot; }
    public PlacedObjectTypeWithActionSO GetPlacedObjectMainWeaponSlot() { return _placedObjectMainWeaponSlot; }
    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectBagSlotList() { return _placedObjec�BagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagSOList; }
    public PlacedObjectTypeArmorSO GetPlacedObjectArmorHeadSlot() { return _placedObjectArmorHeadSlot; }
    public PlacedObjectTypeArmorSO GetPlacedObjectArmorBodySlot() {  return _placedObjectArmorBodySlot; }
    public List<PlacedObjectParameters> GetPlacedObjectList() { return _placedObjectList; }


}
