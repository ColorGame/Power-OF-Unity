using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_Armor/HeadArmorType")]
public class HeadArmorTypeSO : PlacedObjectTypeArmorSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] HeadArmorType _headArmorType;

    [Header("Список совместимой !!BODY ARMOR")]
    [SerializeField] private List<BodyArmorType> _compatibleBodyArmorList;

    private HashSet<BodyArmorType> _compatibleArmorHashList;

    /// <summary>
    /// Получить - Список совместимой брони
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
    /// Получить всплывающую подсказку для данного размещенного объекта 
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
