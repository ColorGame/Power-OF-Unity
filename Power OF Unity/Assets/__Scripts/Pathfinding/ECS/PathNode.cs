using Unity.Collections;
using Unity.Mathematics;

/// <summary>
/// Узел пути (создается в каждой ячейки сетки)
/// </summary>
public struct PathNode 
{
    /// <summary>
    /// X,Y,Z (XZ-плоскость Y- этаж)
    /// </summary>
    public int3 gridPosition;
    public float3 worldPosition;
    /// <summary>
    /// Индекс узла в массиве
    /// </summary>
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
    /// <summary>
    /// Можно пройти по этому узлу?
    /// </summary>
    public bool isWalkable;
    /// <summary>
    /// Этот узел в воздухе?
    /// </summary>
    public bool isInAir;
    /// <summary>
    /// Список мировых позиций от начала пути до этого узла
    /// </summary>
    public NativeList<float3> pathWorldPositionList;
    /// <summary>
    /// пришел из индекса узла(сосед)
    /// </summary>
    public int cameFromNodeIndex;
    /// <summary>
    /// gCost + hCost
    /// </summary>
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
