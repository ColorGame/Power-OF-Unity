using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/SpotterFireItemType")]

public class SpotterFireItemTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] SpotterFireItemType _spotterFireItemType;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<SpotterFireAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _spotterFireItemType);
    }

    public SpotterFireItemType GetSpotterFireItemType()
    {
        return _spotterFireItemType;
    }



    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _spotterFireItemType = SheetProcessor.ParseEnum<SpotterFireItemType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.SpotterFireItem2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.SpotterFireItemPrefab3DArray);

        base.AutoCompletion();
    }

}
