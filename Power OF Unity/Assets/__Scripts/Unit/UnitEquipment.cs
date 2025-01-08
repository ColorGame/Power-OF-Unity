using System;
using System.Collections.Generic;
/// <summary>
/// Экипировка юнита (Хранилище для предметов, которыми оснащен игрок). 
/// </summary>
public class UnitEquipment
{
    /// <summary>
    /// Экипировка юнита (Хранилище для предметов, которыми оснащен игрок). 
    /// </summary>
    public UnitEquipment()
    {
        //Здесь можно загрузить стандартную загрузку экипировки и экипировку врагов   
    }

    public event EventHandler<MainOtherWeapon> OnChangeMainWeapon; // Изменено Основное оружие
    public event EventHandler<MainOtherWeapon> OnChangeOtherWeapon; // Изменено Дополнительное оружие
    public event EventHandler<HeadArmorTypeSO> OnChangeHeadArmor; // Изменена броня головы
    public event EventHandler<BodyArmorTypeSO> OnChangeBodyArmor; // Изменена броня тела

    /// <summary>
    /// Основное и дополнительное оружие
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
    /// Список экипировки юнита
    /// </summary>
    private List<PlacedObjectGridParameters> _equipmentList = new List<PlacedObjectGridParameters>();

    private PlacedObjectTypeWithActionSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeWithActionSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeWithActionSO> _placedObjecеBagSlotList = new List<PlacedObjectTypeWithActionSO>();
    private List<GrenadeTypeSO> _grenadeInBagList = new List<GrenadeTypeSO>(); // Список гранат в багаже

    private HeadArmorTypeSO _headArmor;
    private BodyArmorTypeSO _bodyArmor;

    /// <summary>
    /// Добавить полученный объект в Список экипировки юнита.
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
                _placedObjecеBagSlotList.Add((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
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
    /// Удалить полученный объект из Списка Экипировки юнита.
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
                _placedObjecеBagSlotList.Remove((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
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
    /// Очистить список экипировки
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
    /// Шлем совместим с броней для тела?
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
    /// Получить размещенный объект из слотов, где может разместиться только один предмет
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
    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectBagSlotList() { return _placedObjecеBagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagList; }
    public HeadArmorTypeSO GetHeadArmor() { return _headArmor; }
    public BodyArmorTypeSO GetBodyArmor() { return _bodyArmor; }
    public List<PlacedObjectGridParameters> GetEquipmentList() { return _equipmentList; }


}
