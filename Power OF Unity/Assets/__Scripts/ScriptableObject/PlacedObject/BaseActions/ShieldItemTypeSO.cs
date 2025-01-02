using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/ShieldItemType")]

public class ShieldItemTypeSO : PlacedObjectTypeWithActionSO//(нельзя положить в рюкзак)
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



    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _shieldItemType = SheetProcessor.ParseEnum<ShieldItemType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.ShieldItem2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.ShieldItemPrefab3DArray);

        base.AutoCompletion();
    }
}
