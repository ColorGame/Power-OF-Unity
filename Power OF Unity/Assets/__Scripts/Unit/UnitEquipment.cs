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
        //Здесь можно загрузить стандартную загрузку экипировки
        _placedObjectList = new List<PlacedObjectGridParameters>();
    }

    private List<PlacedObjectGridParameters> _placedObjectList; // Список предметов экипировки

    private PlacedObjectTypeWithActionSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeWithActionSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeWithActionSO> _placedObjecеBagSlotList = new List<PlacedObjectTypeWithActionSO>();
    private List<GrenadeTypeSO> _grenadeInBagSOList = new List<GrenadeTypeSO>(); // Список гранат в багаже

    private PlacedObjectTypeArmorSO _placedObjectArmorHeadSlot;
    private PlacedObjectTypeArmorSO _placedObjectArmorBodySlot;

    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
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
                break;
            case EquipmentSlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = (PlacedObjectTypeWithActionSO)placedObjectTypeSO;
                break;
            case EquipmentSlot.BagSlot:
                _placedObjecеBagSlotList.Add((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Add(grenadeTypeSO);
                }
                break;
            case EquipmentSlot.ArmorHeadSlot:
                _placedObjectArmorHeadSlot = (PlacedObjectTypeArmorSO)placedObjectTypeSO;
                break;
            case EquipmentSlot.ArmorBodySlot:
                _placedObjectArmorBodySlot = (PlacedObjectTypeArmorSO)placedObjectTypeSO;
                break;
        }

    }
    /// <summary>
    /// Удалить полученный объект из Списока "Размещенных Объектов в Сетке Инвенторя".
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
                break;
            case EquipmentSlot.OtherWeaponsSlot:
                _placedObjectOtherWeaponSlot = null;
                break;
            case EquipmentSlot.BagSlot:
                _placedObjecеBagSlotList.Remove((PlacedObjectTypeWithActionSO)placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Remove(grenadeTypeSO);
                }
                break;
            case EquipmentSlot.ArmorHeadSlot:
                _placedObjectArmorHeadSlot = null;
                break;
            case EquipmentSlot.ArmorBodySlot:
                _placedObjectArmorBodySlot = null;
                break;
        }
    }
    /// <summary>
    /// Очистить список размещенных объектов
    /// </summary>
    public void ClearPlacedObjectList()
    {
        _placedObjectList.Clear();       
    }


    public PlacedObjectTypeWithActionSO GetPlacedObjectOtherWeaponSlot() { return _placedObjectOtherWeaponSlot; }
    public PlacedObjectTypeWithActionSO GetPlacedObjectMainWeaponSlot() { return _placedObjectMainWeaponSlot; }
    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectBagSlotList() { return _placedObjecеBagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagSOList; }
    public PlacedObjectTypeArmorSO GetPlacedObjectArmorHeadSlot() { return _placedObjectArmorHeadSlot; }
    public PlacedObjectTypeArmorSO GetPlacedObjectArmorBodySlot() {  return _placedObjectArmorBodySlot; }
    public List<PlacedObjectGridParameters> GetPlacedObjectList() { return _placedObjectList; }


}
