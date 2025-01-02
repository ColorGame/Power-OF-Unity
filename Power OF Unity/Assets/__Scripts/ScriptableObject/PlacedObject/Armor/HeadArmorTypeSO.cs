using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_Armor/HeadArmorType")]
public class HeadArmorTypeSO : PlacedObjectTypeArmorSO
{
    [Header("��� ������������ �������")]
    [SerializeField] HeadArmorType _headArmorType;

    [Header("������ ����������� !!BODY ARMOR")]
    [SerializeField] private List<BodyArmorType> _compatibleBodyArmorList;

    private HashSet<BodyArmorType> _compatibleArmorHashList;

    /// <summary>
    /// �������� - ������ ����������� �����
    /// </summary>
    public HashSet<BodyArmorType> GetCompatibleArmorList()
    {
        if (_compatibleArmorHashList == null)
        {
            _compatibleArmorHashList = new HashSet<BodyArmorType>(_compatibleBodyArmorList);
        }
        return _compatibleArmorHashList;
    }

    /// <summary>
    /// �������� ����������� ��������� ��� ������� ������������ ������� 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _headArmorType);
    }

    public HeadArmorType GetHeadArmorType()
    {
        return _headArmorType;
    }



    [ContextMenu("��������������")]
    protected override void AutoCompletion()
    {
        _headArmorType = SheetProcessor.ParseEnum<HeadArmorType>(name);
        Search2DPrefabAndVisual(
           name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.HeadArmor2DArray);
        base.AutoCompletion();
    }
}
