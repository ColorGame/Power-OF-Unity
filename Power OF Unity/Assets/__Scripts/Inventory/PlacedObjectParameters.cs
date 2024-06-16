using System;
using UnityEditor;
using UnityEngine;


[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct PlacedObjectParameters
{
    public InventorySlot slot;
    public PlacedObjectTypeSO placedObjectTypeSO;
    public Vector2Int gridPositioAnchor; // Сеточная позиция Якоря  


    public PlacedObjectParameters(InventorySlot slot, PlacedObjectTypeSO placedObjectTypeSO, Vector2Int gridPositioAnchor)
    {
        this.slot = slot;
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.gridPositioAnchor = gridPositioAnchor;        
    }

}
