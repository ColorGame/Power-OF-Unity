using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_WithAction/GrappleType")]

public class GrappleTypeSO : PlacedObjectTypeWithActionSO
{
    [Header("��� ������������ �������")]
    [SerializeField] GrappleType _grappleType;
    [Header("��� ������ ��� ����� ���� (��� ����������� � �����)")]
    [SerializeField] private bool _isOneHand=true;
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<GrappleAction>();
    }
    /// <summary>
    /// �������� ����������� ��������� ��� ������� ������������ ������� 
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
    /// ��� ������ ��� ����� ���� (��� ����������� � �����)
    /// </summary>
    public bool GetIsOneHand() { return _isOneHand; }


    [ContextMenu("��������������")]
    protected override void AutoCompletion()
    {
        _grappleType = SheetProcessor.ParseEnum<GrappleType>(name);

        Search2DPrefabAndVisual(
          name,
          PlacedObjectGeneralListForAutoCompletionSO.Instance.Grapple2DArray);

        Search3DPrefab(name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.GrapplePrefab3DArray);

        _canPlacedOnSlotArray = new EquipmentSlot[] { EquipmentSlot.MainWeaponSlot,EquipmentSlot.OtherWeaponsSlot, EquipmentSlot.BagSlot };

        base.AutoCompletion();
    }
}
