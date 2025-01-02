using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/SwordType")]

public class SwordTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] SwordType _swordType;
    [Header("Это оружие для одной руки (для отображения с ЩИТОМ)")]
    [SerializeField] private bool _isOneHand = false;

    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<SwordAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _swordType);
    }
    /// <summary>
    /// Это оружие для одной руки (для отображения с ЩИТОМ)
    /// </summary>
    public bool GetIsOneHand() { return _isOneHand; }
    public SwordType GetSwordType()
    {
        return _swordType;
    }


    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _swordType = SheetProcessor.ParseEnum<SwordType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.Sword2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.SwordPrefab3DArray);

        base.AutoCompletion();
    }
}
