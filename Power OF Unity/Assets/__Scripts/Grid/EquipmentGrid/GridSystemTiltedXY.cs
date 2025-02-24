using System;
using UnityEngine;

/// <summary>
/// ����������� �������� ������� :��������� �����������.
/// </summary>
/// <remarks>
/// ��������� ������ � ������� �����.
/// </remarks>
public class GridSystemTiltedXY<TGridObject> : GridSystemXY<TGridObject>
{
    public GridSystemTiltedXY(float canvasScaleFactor, EquipmentGridParameters gridParameters, Func<GridSystemXY<TGridObject>, Vector2Int, TGridObject> createGridObject) : base(canvasScaleFactor,gridParameters, createGridObject)
    {
    }

    public override Vector2Int GetGridPosition(Vector3 worldPosition) // �������� �������� ������� ������������ ����� ����� (��� ����� ��������� ������� ��������� 0,0)
    {

        Vector3 localPosition = _anchorGridTransform.InverseTransformPoint(worldPosition) - _offset�enterCell; // ���������� ������� � 0 (������� ��������) // InverseTransformPoint -����������� gridPosition ���� �� �������� ������������ � ��������� (������������ _anchorGridTransform).  
        return new Vector2Int
            (
            Mathf.RoundToInt(localPosition.x / _cellSizeWithScaleFactor),  // ��������� Mathf.RoundToInt ��� �������������� float � int
            Mathf.RoundToInt(localPosition.y / _cellSizeWithScaleFactor)
            );
    }

    public override Vector3 GetWorldPositionGridCenter() //  ������� ������� ���������� ������ ����� (������������  _anchorGridTransform)
    {
        Vector3 gridCenter = base.GetWorldPositionGridCenter();
        return _anchorGridTransform.TransformPoint(gridCenter); // transform.TransformPoint -����������� gridPosition ����� ������ �� ���������� ������������(_anchorGridTransform) � ������� ������������(�������� ������ ������� _anchorGridTransform). �.�. _anchorGridTransform ����� ���������� �� � (0,0) � �������� � ������� ������������
    }

    public override Vector3 GetWorldPositionCenter�ornerCell(Vector2Int gridPosition) // ������� ������� ���������� ������ ������ (������������  _anchorGridTransform)
    {
        Vector3 localPositionXY = new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor + _offset�enterCell;   // ������� ��������� ���������� ����� ����� � ������ �� ��������, ������������ _anchorGridTransform
                                                                                                                    // �� ����� ��� �� ����� ������ ��� ������ �������������� ������ _anchorGridTransform  � ����� ���� ����� �������� � ***Grid.transform.gridPosition (��� �������� �������� �����, ���������� ����������� ������ ***Grid � ������ �����) ������� + _offset�enterCell,
        return _anchorGridTransform.TransformPoint(localPositionXY); // transform.TransformPoint -����������� gridPosition ����� ������ �� ���������� ������������(_anchorGridTransform) � ������� ������������(�������� ������ ������� _anchorGridTransform). �.�. _anchorGridTransform ����� ���������� �� � (0,0) � �������� � ������� ������������
    }

    /// <summary>
    /// ������� ������� ���������� ������� ������ ����� ������ (������������ ����� _anchorGridTransform)
    /// </summary>  
    /// <returns></returns>
    public override Vector3 GetWorldPositionLowerLeft�ornerCell(Vector2Int gridPosition) 
    {
        Vector3 localPositionXY = new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor;   // ������� ��������� ���������� ����� ����� � ������ �� ��������, ������������ _anchorGridTransform                                                                                                  
        return _anchorGridTransform.TransformPoint(localPositionXY); // transform.TransformPoint -����������� gridPosition ����� ������ �� ���������� ������������(_anchorGridTransform) � ������� ������������(�������� ������ ������� _anchorGridTransform). �.�. _anchorGridTransform ����� ���������� �� � (0,0) � �������� � ������� ������������
    }
}
