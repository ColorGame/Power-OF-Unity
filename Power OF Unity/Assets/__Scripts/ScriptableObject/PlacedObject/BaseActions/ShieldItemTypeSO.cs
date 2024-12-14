using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShieldItemType")]

public class ShieldItemTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] ShieldItemType _shieldItemType;

    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<ShieldAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _shieldItemType);
    }

    public ShieldItemType GetShieldItemType()
    {
        return _shieldItemType;
    }
}
