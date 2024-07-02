using System;
using System.Collections.Generic;

/// <summary>
/// Инвентарь юнита (Хранилище для предметов, которыми оснащен игрок). 
/// </summary>
public class UnitInventory
{
    /// <summary>
    /// Инвентарь юнита (Хранилище для предметов, которыми оснащен игрок). 
    /// </summary>
    public UnitInventory(Unit unit)
    {
        _unit = unit;
        _placedObjectList = new List<PlacedObjectParameters>();
    }

    public event EventHandler<PlacedObject> OnAddPlacedObjectList; // Событие Добавлен предмет в инвентарь
    public event EventHandler<PlacedObject> OnRemovePlacedObjectList; // Событие Удален предмет из инвенторя

    private Unit _unit;
    private List<PlacedObjectParameters> _placedObjectList = new List<PlacedObjectParameters>(); // Список предметов инвентаря

    private PlacedObjectTypeSO _placedObjectMainWeaponSlot;
    private PlacedObjectTypeSO _placedObjectOtherWeaponSlot;
    private List<PlacedObjectTypeSO> _placedObjecеBagSlotList;
    private List<GrenadeTypeSO> _grenadeInBagSOList = new List<GrenadeTypeSO>(); // Список гранат в багаже

    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
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
                _placedObjecеBagSlotList.Add(placedObjectTypeSO);
                if (placedObjectTypeSO is GrenadeTypeSO grenadeTypeSO)
                {
                    _grenadeInBagSOList.Add(grenadeTypeSO);
                }
                break;
        }
           
        OnAddPlacedObjectList?.Invoke(this, placedObject);
    }
    /// <summary>
    /// Удалить полученный объект из Списока "Размещенных Объектов в Сетке Инвенторя".
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
                _placedObjecеBagSlotList.Remove(placedObjectTypeSO);
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
    public List<PlacedObjectTypeSO > GetPlacedObjectBagSlotList() { return _placedObjecеBagSlotList; }
    public List<GrenadeTypeSO> GetGrenadeTypeSOList() { return _grenadeInBagSOList; }
    public List<PlacedObjectParameters> GetPlacedObjectList() {  return _placedObjectList; }


}
