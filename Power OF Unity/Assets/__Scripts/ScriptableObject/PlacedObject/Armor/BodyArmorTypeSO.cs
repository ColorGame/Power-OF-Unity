using System;
using UnityEngine;

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
}
