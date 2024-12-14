using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/VisionItemType")]

public class VisionItemTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] VisionItemType _visionItemType;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<VisionAction>();
    }

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _visionItemType);
    }

    public VisionItemType GetVisionItemType()
    {
        return _visionItemType;
    }
}