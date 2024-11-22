using System;
using System.Collections.Generic;
/// <summary>
/// ���������� ����� (��������� ��� ���������, �������� ������� �����). 
/// </summary>
public class UnitEquipment
{
    /// <summary>
    /// ���������� ����� (��������� ��� ���������, �������� ������� �����). 
    /// </summary>
    public UnitEquipment()
    {
        //����� ����� ��������� ����������� �������� ����������       
    }

    public event EventHandler<PlacedObjectTypeWithActionSO> OnChangeMainWeapon; // �������� �������� ������
    public event EventHandler<HeadArmorTypeSO> OnChangeHeadArmor; // �������� ����� ������
    public event EventHandler<BodyArmorTypeSO> OnChangeBodyArmor; // �������� ����� ����

    private List<PlacedObjectGridParameters> _placedObjectList = new List<PlacedObjectGridParameters>(); // ������ "����������� �������� � ����� ���������"

    private PlacedObjectTypeWithActionSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeWithActionSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeWithActionSO> _placedObjec�BagSlotList = new List<PlacedObjectTypeWithActionSO>();
    private List<GrenadeTypeSO> _grenadeInBagList = new List<GrenadeTypeSO>(); // ������ ������ � ������

    private HeadArmorTypeSO _headArmor;
    private BodyArmorTypeSO _bodyArmor;

    /// <summary>
    /// �������� ���������� ������ � ������ "����������� �������� � ����� ���������".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {
        PlacedObjectGridParameters placedObjectGridParameters = placedObject.GetPlacedObjectGridParameters();
        _placedObjectList.Add(placedObjectGridParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectGridParameters.slot)
        {
            case EquipmentSlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                OnChangeMainWeapon?.Invoke(this, _placedObjectMainWeaponSlot);
                break;
            case EquipmentSlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                break;
            case EquipmentSlot.BagSlot:
                _placedObjec�BagSlotList.Add((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagList.Add(grenadeTypeSO);
                }
                break;
            case EquipmentSlot.HeadArmorSlot:
                _headArmor = (HeadArmorTypeSO)placedObjectTypeSO;
                OnChangeHeadArmor?.Invoke(this, _headArmor);
                break;
            case EquipmentSlot.BodyArmorSlot:
                _bodyArmor = (BodyArmorTypeSO)placedObjectTypeSO;
                OnChangeBodyArmor?.Invoke(this, _bodyArmor);
                break;
        }

    }
    /// <summary>
    /// ������� ���������� ������ �� ������ "����������� �������� � ����� ���������".
    /// </summary>  
    public void RemovePlacedObjectList(PlacedObject placedObject)
    {
        PlacedObjectGridParameters placedObjectGridParameters = placedObject.GetPlacedObjectGridParameters();
        _placedObjectList.Remove(placedObjectGridParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectGridParameters.slot)
        {
            case EquipmentSlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = null;
                OnChangeMainWeapon?.Invoke(this, _placedObjectMainWeaponSlot);
                break;
            case EquipmentSlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = null;
                break;
            case EquipmentSlot.BagSlot:
                _placedObjec�BagSlotList.Remove((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagList.Remove(grenadeTypeSO);
                }
                break;
            case EquipmentSlot.HeadArmorSlot:
                _headArmor = null;
                OnChangeHeadArmor?.Invoke(this, _headArmor);
                break;
            case EquipmentSlot.BodyArmorSlot:
                _bodyArmor = null;
                OnChangeBodyArmor?.Invoke(this, _bodyArmor);
                break;
        }
    }
    /// <summary>
    /// �������� ������ ����������� ��������
    /// </summary>
    public void ClearPlacedObjectList()
    {
        _placedObjectList.Clear();
        _placedObjectMainWeaponSlot = null;
        OnChangeMainWeapon?.Invoke(this, _placedObjectMainWeaponSlot);
        _headArmor = null;
        OnChangeHeadArmor?.Invoke(this, _headArmor);
        _bodyArmor = null;
        OnChangeBodyArmor?.Invoke(this, _bodyArmor);
    }

    /// <summary>
    /// ���� ��������� � ������ ��� ����?
    /// </summary>
    public bool IsHeadArmorCompatibleWithBodyArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (_bodyArmor != null)
        {
            return headArmorTypeSO.GetCompatibleArmorList().Contains(_bodyArmor.GetPlacedObjectType());
        }
        else { return false; }
    }


    public PlacedObjectTypeWithActionSO GetPlacedObjectOtherWeaponSlot() { return _placedObjectOtherWeaponSlot; }
    public PlacedObjectTypeWithActionSO GetPlacedObjectMainWeaponSlot() { return _placedObjectMainWeaponSlot; }
    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectBagSlotList() { return _placedObjec�BagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagList; }
    public HeadArmorTypeSO GetHeadArmor() { return _headArmor; }
    public BodyArmorTypeSO GetBodyArmor() { return _bodyArmor; }
    public List<PlacedObjectGridParameters> GetPlacedObjectList() { return _placedObjectList; }


}
