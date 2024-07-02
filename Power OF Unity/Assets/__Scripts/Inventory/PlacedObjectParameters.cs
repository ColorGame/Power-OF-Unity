using System;
using UnityEngine;

/// <summary>
/// Параметры размещаемого объекта.
/// </summary>
[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct PlacedObjectParameters
{
    public InventorySlot slot;
    public PlacedObject placedObject;
    public PlacedObjectTypeSO placedObjectTypeSO;
    public Vector2Int gridPositioAnchor; // Сеточная позиция Якоря  

    /// <summary>
    /// Параметры размещаемого объекта. 
    /// </summary>
    public PlacedObjectParameters(InventorySlot slot, Vector2Int gridPositioAnchor, PlacedObjectTypeSO placedObjectTypeSO)
    {
        this.slot = slot;
        this.gridPositioAnchor = gridPositioAnchor;
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.placedObject = default;
    }

    /// <summary>
    /// Параметры размещаемого объекта. 
    /// </summary>
    public PlacedObjectParameters(InventorySlot slot, Vector2Int gridPositioAnchor, PlacedObject placedObject)
    {
        this.slot = slot;
        this.gridPositioAnchor = gridPositioAnchor;
        this.placedObject = placedObject;
        this.placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();
    }
}
