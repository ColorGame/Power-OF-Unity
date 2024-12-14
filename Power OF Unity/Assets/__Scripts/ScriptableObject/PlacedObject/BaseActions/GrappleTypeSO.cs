using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrappleType")]

public class GrappleTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] GrappleType _grappleType;
    [Header("Это оружие для одной руки (для отображения с ЩИТОМ)")]
    [SerializeField] private bool _isOneHand=true;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<GrappleAction>();
    }
    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _grappleType);
    }

    public GrappleType GetGrappleType()
    {
       return _grappleType;
    }
    /// <summary>
    /// Это оружие для одной руки (для отображения с ЩИТОМ)
    /// </summary>
    public bool GetIsOneHand() { return _isOneHand; }
}
