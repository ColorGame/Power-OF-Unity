using System;
using UnityEngine;

/// <summary>
/// Наклоненная сеточная система :наследует стандартную.
/// </summary>
/// <remarks>
/// Учитывает наклон и поворот сетки.
/// </remarks>
public class GridSystemTiltedXY<TGridObject> : GridSystemXY<TGridObject>
{
    public GridSystemTiltedXY(float canvasScaleFactor, EquipmentGridParameters gridParameters, Func<GridSystemXY<TGridObject>, Vector2Int, TGridObject> createGridObject) : base(canvasScaleFactor,gridParameters, createGridObject)
    {
    }

    public override Vector2Int GetGridPosition(Vector3 worldPosition) // Получить сеточную позицию относительно якоря сетки (это будет локальным началом координат 0,0)
    {

        Vector3 localPosition = _anchorGridTransform.InverseTransformPoint(worldPosition) - _offsetСenterCell; // переведите обратно в 0 (удалите смещение) // InverseTransformPoint -Преобразует gridPosition мыши из мирового пространства в локальное (относительно _anchorGridTransform).  
        return new Vector2Int
            (
            Mathf.RoundToInt(localPosition.x / _cellSizeWithScaleFactor),  // Применяем Mathf.RoundToInt для преоброзования float в int
            Mathf.RoundToInt(localPosition.y / _cellSizeWithScaleFactor)
            );
    }

    public override Vector3 GetWorldPositionGridCenter() //  Получим мировые координаты центра Сетки (относительно  _anchorGridTransform)
    {
        Vector3 gridCenter = base.GetWorldPositionGridCenter();
        return _anchorGridTransform.TransformPoint(gridCenter); // transform.TransformPoint -Преобразует gridPosition нашей ячейки из локального пространства(_anchorGridTransform) в мировое пространство(учитывая наклон поворот _anchorGridTransform). т.к. _anchorGridTransform может находиться не в (0,0) и наклонен в мировом пространмтве
    }

    public override Vector3 GetWorldPositionCenterСornerCell(Vector2Int gridPosition) // Получим мировые координаты центра ячейки (относительно  _anchorGridTransform)
    {
        Vector3 localPositionXY = new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor + _offsetСenterCell;   // Получим локальные координаты нашей ячеки с учетом ее масштаба, относительно _anchorGridTransform
                                                                                                                    // мы хотим что бы центр ячейки был смещен относительного нашего _anchorGridTransform  и левый угол сетки совподал с ***Grid.transform.gridPosition (для удобства создания сетки, достаточно переместить объект ***Grid в нужное место) поэтому + _offsetСenterCell,
        return _anchorGridTransform.TransformPoint(localPositionXY); // transform.TransformPoint -Преобразует gridPosition нашей ячейки из локального пространства(_anchorGridTransform) в мировое пространство(учитывая наклон поворот _anchorGridTransform). т.к. _anchorGridTransform может находиться не в (0,0) и наклонен в мировом пространмтве
    }

    /// <summary>
    /// Получим мировые координаты нижнего левого угола ячейки (относительно нашей _anchorGridTransform)
    /// </summary>  
    /// <returns></returns>
    public override Vector3 GetWorldPositionLowerLeftСornerCell(Vector2Int gridPosition) 
    {
        Vector3 localPositionXY = new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor;   // Получим локальные координаты нашей ячеки с учетом ее масштаба, относительно _anchorGridTransform                                                                                                  
        return _anchorGridTransform.TransformPoint(localPositionXY); // transform.TransformPoint -Преобразует gridPosition нашей ячейки из локального пространства(_anchorGridTransform) в мировое пространство(учитывая наклон поворот _anchorGridTransform). т.к. _anchorGridTransform может находиться не в (0,0) и наклонен в мировом пространмтве
    }
}
