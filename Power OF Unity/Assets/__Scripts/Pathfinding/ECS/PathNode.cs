using System;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Узел пути (создается в каждой ячейки сетки)
/// </summary>
public struct PathNode: IComponentData
{
    /// <summary>
    /// X,Y,Z (XZ-плоскость Y- этаж)
    /// </summary>
    public int3 gridPosition;
    public float3 worldPosition;

    public int index;
    /// <summary>
    /// Стоимость смещения от старта до текущей ячейки<br/>
    /// По диоганали вычисляется из теоремы пифагора. Длина гипотенузы в прямоугольном треугольнике(корень квадратный из суммы квадратов сторон)                         
    /// </summary>
    public int gCost;
    /// <summary>
    /// Эвристическая стоимость смещения до цели по кратчайшему пути (грубо говоря длина по прямой)<br/>
    /// Чем ближе к цели тем меньше будет это число
    /// </summary>
    public int hCost;
    /// <summary>
    /// Сумма g+h
    /// </summary>
    public int fCost;

    public bool isWalkable;
    public bool isInAir;

    /// <summary>
    /// пришел из индекса узла
    /// </summary>
    public int cameFromNodeIndex;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
