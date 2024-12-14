using System;
using UnityEngine;

/// <summary>
/// Сеточные параметры размещаемого объекта.
/// </summary>
[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct PlacedObjectGridParameters
{
    public EquipmentSlot slot;
    /// <summary>
    /// Экземпляр размещенного объекта
    /// </summary>
    public PlacedObject placedObject;
    public PlacedObjectTypeSO placedObjectTypeSO;
    /// <summary>
    /// Сеточная позиция Якоря(нижний левый угол объекта) 
    /// </summary>
    public Vector2Int gridPositioAnchor; // Сеточная позиция Якоря  

    /// <summary>
    /// Параметры размещаемого объекта
    /// </summary>
    public PlacedObjectGridParameters(EquipmentSlot slot, Vector2Int gridPositioAnchor, PlacedObjectTypeSO placedObjectTypeSO = null, PlacedObject placedObject = null)
    {
        this.slot = slot;
        this.gridPositioAnchor = gridPositioAnchor;
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.placedObject = placedObject;
    }


}
