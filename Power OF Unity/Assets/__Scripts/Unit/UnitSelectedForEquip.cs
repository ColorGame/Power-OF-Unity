using UnityEngine;


/// <summary>
///  Юнит которого ВЫБРАЛИ для экипировки и настройки инвенторя
/// </summary>
public class UnitSelectedForEquip 
{

    private Unit _selectedUnit; //
    private PickUpDrop _pickUpDrop;

    public void Init(PickUpDrop pickUpDrop)
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
        _selectedUnit.GetUnitInventory().AddPlacedObjectList(placedObject);
    }
    /// <summary>
    /// Удалим полученный объект из Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>
    private void PickUpDrop_OnRemovePlacedObjectAtInventoryGrid(object sender, PlacedObject placedObject)
    {
        _selectedUnit.GetUnitInventory().RemovePlacedObjectList(placedObject);
    }


    public void SetSelectedUnit(Unit selectedUnit)
    {
        _selectedUnit = selectedUnit;
        Debug.Log($"Выбрал -{_selectedUnit.GetUnitTypeSO<UnitTypeSO>().GetName()} ");
    }
}
