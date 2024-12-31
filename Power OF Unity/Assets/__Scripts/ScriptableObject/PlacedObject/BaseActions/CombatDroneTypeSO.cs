using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/CombatDroneType")]

public class CombatDroneTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] CombatDroneType _combatDroneType;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<CombatDroneAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _combatDroneType);
    }

    public CombatDroneType GetVisionItemType()
    {
        return _combatDroneType;
    }
}