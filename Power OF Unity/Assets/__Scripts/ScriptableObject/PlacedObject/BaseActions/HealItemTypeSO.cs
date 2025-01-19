using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/HealItemType")]

public class HealItemTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] HealItemType _healItemType;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<HealAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _healItemType);
    }

    public HealItemType GetHealItemType()
    {
        return _healItemType;
    }


    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _healItemType = SheetProcessor.ParseEnum<HealItemType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.HealItem2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.HealItemPrefab3DArray);

        _canPlacedOnSlotArray = new EquipmentSlot[] {EquipmentSlot.OtherWeaponsSlot, EquipmentSlot.BagSlot };

        base.AutoCompletion();
    }
}
