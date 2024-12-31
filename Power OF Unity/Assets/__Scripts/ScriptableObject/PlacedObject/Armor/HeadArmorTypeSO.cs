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
}
