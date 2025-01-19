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

    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _combatDroneType = SheetProcessor.ParseEnum<CombatDroneType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.CombatDrone2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.CombatDronePrefab3DArray);

        _canPlacedOnSlotArray = new EquipmentSlot[] { EquipmentSlot.BagSlot };

        base.AutoCompletion();
    }

    public CombatDroneType GetCombatDroneType() {return _combatDroneType;}
}