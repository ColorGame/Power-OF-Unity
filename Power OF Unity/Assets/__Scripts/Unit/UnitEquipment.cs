using UnityEngine;

/// <summary>
/// Ёкипирует юнита предметами из его инвентар€ (создает и удал€ет прикрипленные объекты). 
/// </summary>
/// <remarks>
///  омпонент должен быть прикриплен к юниту
/// </remarks>
public class UnitEquipment : MonoBehaviour
{
    private Transform _rightHandTransform = null;
    private Transform _leftHandTransform = null;

    private void Start()
    {
        // подписатьс€ на событие смена визуала юнита дл€ получени€ ј “”јЋ№Ќџ’ rightHandTransform и leftHandTransform т.к. дл€ разной брони разные скелеты
    }


    /// <summary>
    ///  Ёкиперовать ќ–”∆»≈ћ, юнита к которому прикриплен этот скрипт
    /// </summary>
    public void EquipWeapon(PlacedObject placedObject)
    {
        //ѕолучить из PlacedObject  SO а из него на какую руку и префаб
    }

    /// <summary>
    ///  Ёкиперовать Ѕ–ќЌ≈…, юнита к которому прикриплен этот скрипт
    /// </summary>
    public void EquipArmor()
    {

    }

    public void FreeHands() // ќсвободить руки
    {

    }
}
