
using System;
using UnityEngine;

[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct InventoryGridParameters  // Создадим структуру можно в отдельном классе. Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#
{                    
    public InventorySlot slot;
    public int width;       //Ширина
    public int height;      //Высота    
    public  float cellSize;  // Размер ячейки
    public Transform anchorGridTransform; //Якорь трансформа сетки

    public InventoryGridParameters(InventorySlot gridName, int width, int height, float cellSize, Transform anchorGridTransform)
    {
        this.slot = gridName;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.anchorGridTransform = anchorGridTransform;
    }
}
