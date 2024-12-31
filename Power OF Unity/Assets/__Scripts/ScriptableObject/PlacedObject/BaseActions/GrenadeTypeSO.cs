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
}
