using System;
using UnityEngine;

[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct LevelGridParameters  // Создадим структуру можно в отдельном классе. Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#
{    
    public int width;       //Ширина
    public int height;      //Высота    
    public float cellSize;  // Размер ячейки
    public Transform anchorGridTransform; //Якорь трансформа сетки

    public LevelGridParameters(int width, int height, float cellSize, Transform anchorGridTransform)
    {        
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.anchorGridTransform = anchorGridTransform;
    }
}