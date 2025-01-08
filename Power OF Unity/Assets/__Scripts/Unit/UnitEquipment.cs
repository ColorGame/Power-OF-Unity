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
        //����� ����� ��������� ����������� �������� ���������� � ���������� ������   
    }

    public event EventHandler<MainOtherWeapon> OnChangeMainWeapon; // �������� �������� ������
    public event EventHandler<MainOtherWeapon> OnChangeOtherWeapon; // �������� �������������� ������
    public event EventHandler<HeadArmorTypeSO> OnChangeHeadArmor; // �������� ����� ������
    public event EventHandler<BodyArmorTypeSO> OnChangeBodyArmor; // �������� ����� ����

    /// <summary>
    /// �������� � �������������� ������
    /// </summary>   
    public struct MainOtherWeapon
    {
        public PlacedObjectTypeWithActionSO mainWeapon;
        public PlacedObjectTypeWithActionSO otherWeapon;
        public MainOtherWeapon(PlacedObjectTypeWithActionSO mainWeapon, PlacedObjectTypeWithActionSO otherWeapon)
        {
            this.mainWeapon = mainWeapon;
            this.otherWeapon = otherWeapon;
        }
    }
    /// <summary>
    /// ������ ���������� �����
    /// </summary>
    private List<PlacedObjectGridParameters> _equipmentList = new List<PlacedObjectGridParameters>();

    private PlacedObjectTypeWithActionSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeWithActionSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeWithActionSO> _placedObjec�BagSlotList = new List<PlacedObjectTypeWithActionSO>();
    private List<GrenadeTypeSO> _grenadeInBagList = new List<GrenadeTypeSO>(); // ������ ������ � ������

    private HeadArmorTypeSO _headArmor;
    private BodyArmorTypeSO _bodyArmor;

    /// <summary>
    /// �������� ���������� ������ � ������ ���������� �����.
    /// </summary>  
    public void AddPlacedObjectAtEquipmentList(PlacedObject placedObject)
    {
        PlacedObjectGridParameters placedObjectGridParameters = placedObject.GetPlacedObjectGridParameters();
        _equipmentList.Add(placedObjectGridParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectGridParameters.slot)
        {
            case EquipmentSlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                OnChangeMainWeapon?.Invoke(this, new MainOtherWeapon
                {
                    mainWeapon = _placedObjectMainWeaponSlot,
                    otherWeapon = _placedObjectOtherWeaponSlot
                });
                break;
            case EquipmentSlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                OnChangeOtherWeapon?.Invoke(this, new MainOtherWeapon
                {
                    mainWeapon = _placedObjectMainWeaponSlot,
                    otherWeapon = _placedObjectOtherWeaponSlot
                });
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
    /// ������� ���������� ������ �� ������ ���������� �����.
    /// </summary>  
    public void RemovePlacedObjectAtEquipmentList(PlacedObject placedObject)
    {
        PlacedObjectGridParameters placedObjectGridParameters = placedObject.GetPlacedObjectGridParameters();
        _equipmentList.Remove(placedObjectGridParameters);

        PlacedObjectTypeSO placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();

        switch (placedObjectGridParameters.slot)
        {
            case EquipmentSlot.MainWeaponSlot:
                _placedObjectMainWeaponSlot = null;
                OnChangeMainWeapon?.Invoke(this, new MainOtherWeapon
                {
                    mainWeapon = _placedObjectMainWeaponSlot,
                    otherWeapon = _placedObjectOtherWeaponSlot
                });
                break;
            case EquipmentSlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = null;
                OnChangeOtherWeapon?.Invoke(this, new MainOtherWeapon
                {
                    mainWeapon = _placedObjectMainWeaponSlot,
                    otherWeapon = _placedObjectOtherWeaponSlot
                });
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
    /// �������� ������ ����������
    /// </summary>
    public void ClearEquipmentList()
    {
        _equipmentList.Clear();
        _placedObjectMainWeaponSlot = null;
        OnChangeMainWeapon?.Invoke(this, new MainOtherWeapon
        {
            mainWeapon = _placedObjectMainWeaponSlot,
            otherWeapon = _placedObjectOtherWeaponSlot
        });
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
            return headArmorTypeSO.GetCompatibleArmorList().Contains(_bodyArmor.GetBodyArmorType());
        }
        else { return false; }
    }

    /// <summary>
    /// �������� ����������� ������ �� ������, ��� ����� ������������ ������ ���� �������
    /// </summary>
    public PlacedObject GetPlacidObjectFromSinglSlot(EquipmentSlot equipmentSlot)
    {
        switch (equipmentSlot)
        {
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:
            case EquipmentSlot.BodyArmorSlot:
            case EquipmentSlot.HeadArmorSlot:
                foreach (PlacedObjectGridParameters parameters in _equipmentList)
                {
                    if (parameters.slot == equipmentSlot)
                        return parameters.placedObject;
                }
                break;
        }
        return null;
    }


    public PlacedObjectTypeWithActionSO GetPlacedObjectOtherWeaponSlot() { return _placedObjectOtherWeaponSlot; }
    public PlacedObjectTypeWithActionSO GetPlacedObjectMainWeaponSlot() { return _placedObjectMainWeaponSlot; }
    public MainOtherWeapon GetMainOtherWeapon() { return new MainOtherWeapon(mainWeapon: _placedObjectMainWeaponSlot, otherWeapon: _placedObjectOtherWeaponSlot); }
    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectBagSlotList() { return _placedObjec�BagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagList; }
    public HeadArmorTypeSO GetHeadArmor() { return _headArmor; }
    public BodyArmorTypeSO GetBodyArmor() { return _bodyArmor; }
    public List<PlacedObjectGridParameters> GetEquipmentList() { return _equipmentList; }


}
