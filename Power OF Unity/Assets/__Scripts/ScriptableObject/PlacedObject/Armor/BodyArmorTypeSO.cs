using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectType_Armor/BodyArmorType")]
public class BodyArmorTypeSO  : PlacedObjectTypeArmorSO
{
    [Header("Тип размещаемого объекта")]
    [SerializeField] BodyArmorType _bodyArmorType;

    /// <summary>
    /// Получить всплывающую подсказку для данного размещенного объекта 
    /// </summary>
    public override PlacedObjectTooltip GetPlacedObjectTooltip()
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(this, _bodyArmorType);
    }

    public BodyArmorType GetBodyArmorType()
    {
        return _bodyArmorType;
    }



    [ContextMenu("Автозаполнение")]
    protected override void AutoCompletion()
    {             
        _bodyArmorType = SheetProcessor.ParseEnum<BodyArmorType>(name);

        Search2DPrefabAndVisual(
            name,            
            PlacedObjectGeneralListForAutoCompletionSO.Instance.BodyArmor2DArray);

        _canPlacedOnSlotArray = new EquipmentSlot[] { EquipmentSlot.BodyArmorSlot };
        base.AutoCompletion();

    }

    
}
