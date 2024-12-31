using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/HealItemType")]

public class HealItemTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("��� ������������ �������")]
    [SerializeField] HealItemType _healItemType;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<HealAction>();
    }

    /// <summary>
    /// �������� ����������� ��������� ��� ������� ������������ ������� 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _healItemType);
    }

    public HealItemType GetHealItemType()
    {
        return _healItemType;
    }
}
