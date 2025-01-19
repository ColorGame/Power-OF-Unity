using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/GrenadeType")]

public class GrenadeTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] GrenadeType _grenadeType;

    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<GrenadeAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _grenadeType);
    }

    public GrenadeType GetGrenadeType()
    {
        return _grenadeType;
    }

    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _grenadeType = SheetProcessor.ParseEnum<GrenadeType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.Grenade2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.GrenadePrefab3DArray);

        _canPlacedOnSlotArray = new EquipmentSlot[] {EquipmentSlot.BagSlot };

        base.AutoCompletion();
    }
}
