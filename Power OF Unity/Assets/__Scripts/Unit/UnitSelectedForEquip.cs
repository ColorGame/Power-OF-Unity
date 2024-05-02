using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  Юнит которого ВЫБРАЛИ для экипировки и настройки инвенторя
/// </summary>
public class UnitSelectedForEquip 
{
    public UnitSelectedForEquip()// Конструктор что бы отследить количество созданных new T() ОН ДОЛЖЕН БЫТЬ ОДИН
    {
    }

   [SerializeField] private UnitInventory _selectedUnitEquipment; // Временно установим через инспектор или выбрать из UnitManager первый в списке


    private PickUpDrop _pickUpDrop;

    public void Initialize(PickUpDrop pickUpDrop)
    {       
        _pickUpDrop = pickUpDrop;

        _pickUpDrop.OnAddPlacedObjectAtInventoryGrid += PickUpDrop_OnAddPlacedObjectAtInventoryGrid;//  Объект добавлен в сетку Интвенторя
        _pickUpDrop.OnRemovePlacedObjectAtInventoryGrid += PickUpDrop_OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя
    }

    
    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnAddPlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnitEquipment.AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// Удалим полученный объект из Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnRemovePlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnitEquipment.RemovePlacedObjectList(placedObject);
    }


    public void SetSelectedUnit(UnitInventory unitEquipment)
    {
        _selectedUnitEquipment = unitEquipment;
        Debug.Log($"Выбрал -{_selectedUnitEquipment.name} ");
    }
}
