using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_Armor/HeadArmorType")]
public class HeadArmorTypeSO : PlacedObjectTypeArmorSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] HeadArmorType _headArmorType;

    [Header("Список совместимой !!BODY ARMOR")]
    [SerializeField] private BodyArmorType[] _compatibleBodyArmorArray;


    /// <summary>
    /// Получить - Список совместимой брони
    /// </summary>
    public BodyArmorType[] GetCompatibleArmorList() { return _compatibleBodyArmorArray; }

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



    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {
        _headArmorType = SheetProcessor.ParseEnum<HeadArmorType>(name);
        Search2DPrefabAndVisual(
           name,
           PlacedObjectGeneralListForAutoCompletionSO.Instance.HeadArmor2DArray);

        _canPlacedOnSlotArray = new EquipmentSlot[] { EquipmentSlot.HeadArmorSlot };

        base.AutoCompletion();
    }
}
