using UnityEngine;

/// <summary>
/// Экипирует юнита предметами из его экипировки (создает и удаляет прикрипленные объекты). 
/// </summary>
public class UnitEquips 
{
    /// <summary>
    /// Оснащает юнита предметами из его экипировки (создает и удаляет прикрипленные объекты). 
    /// </summary>
    public UnitEquips(Unit unit)
    {
        _unit = unit;
    }

    private Unit _unit;
    private Transform _rightHandTransform = null;
    private Transform _leftHandTransform = null;

    public void SetupForSpawn()
    {
        // подписаться на событие смена визуала юнита для получения АКТУАЛЬНЫХ rightHandTransform и leftHandTransform т.к. для разной брони разные скелеты
        if (_unit.IsEnemy())
        {
            PlacedObjectTypeWithActionSO mainplacedObjectTypeWithActionSO =  _unit.GetUnitTypeSO<UnitEnemySO>().GetMainplacedObjectTypeWithActionSO(); // Получим основное оружие для экиперовки врага
            // Сделать
          //  PlacedObject placedObject = PlacedObject.CreateInWorld(_rightHandTransform.position, mainplacedObjectTypeWithActionSO, _unit.GetTransform(), _unit.Get);
         //   EquipWeapon(placedObject);
        }
    }


    /// <summary>
    ///  Экиперовать ОРУЖИЕМ, юнита к которому прикриплен этот скрипт
    /// </summary>
    public void EquipWeapon(PlacedObject placedObject)
    {
        //Получить из PlacedObject  SO а из него на какую руку и префаб

    }

    /// <summary>
    ///  Экиперовать БРОНЕЙ, юнита к которому прикриплен этот скрипт
    /// </summary>
    public void EquipArmor()
    {

    }

    public void FreeHands() // Освободить руки
    {
       /* if (EquipmentSlot.MainWeaponSlot == placedObject.GetActiveGridSystemXY().GetGridSlot())//Если удален из Сетки Основного Оружия
        {
           // Убрать экипировку
        }*/
    }
}
