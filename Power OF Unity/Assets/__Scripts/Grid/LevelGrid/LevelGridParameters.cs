using System;
using UnityEngine;

[Serializable] // Чтобы созданная структура могла отображаться в инспекторе
public struct LevelGridParameters  // Создадим структуру можно в отдельном классе. Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#
{    
    public int width;       //Ширина
    public int height;      //Высота    
    public float cellSize;  // Размер ячейки
    public int floorAmount;// Количество Этажей
    public float floorHeight;// Высота этажа в уровне - это высота стенок  

    public LevelGridParameters(int width, int height, float cellSize, int floorAmount, float floorHeight)
    {        
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;
        this.floorHeight = floorHeight;
    }
}