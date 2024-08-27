using System;
using UnityEngine;


/// <summary>
/// �������� ������� // ����������� ����� C# // ����� ������������ ����������� ��� �������� ����� ����� ������� �� �� ��������� MonoBehaviour/
/// </summary>
/// <remarks>
/// <TGridObject> - Generic, ��� ���� ����� GridSystemXY ����� �������� �� ������ � GridSystemXY �� � � ��. ������������� �� ������ �������� �����
/// </remarks>
public class GridSystemXY<TGridObject> 
{   

    protected int _width;     // ������
    protected int _height;    // ������
    protected float _canvasScaleFactor;// ������� ������� �� ������� ��������� ����� (������ �� ������ ������)
    protected float _cellSizeWithScaleFactor;// ������ ������ � ������ �������� �������
    protected Transform _anchorGridTransform; // ����� �����
    protected TGridObject[,] _gridObjectArray; // ��������� ������ �������� �����
    protected Vector3 _offset�enterCell;// ������� �������� ��� �� ����� ������ �� ��������  � (0.0) transform.position �������� 
    protected EquipmentSlot _slot;

    public GridSystemXY(float canvasScaleFactor, EquipmentGridParameters gridParameters, Func<GridSystemXY<TGridObject>, Vector2Int, TGridObject> createGridObject)  // ����������� // Func - ��� ���������� ������� (������ �������� � ��������� ��� ���<TGridObject> ������� ���������� ��� ������� � ������� ��� createGridObject)
    {
        _width = gridParameters.width;
        _height = gridParameters.height;
        _canvasScaleFactor= canvasScaleFactor;
        _cellSizeWithScaleFactor = gridParameters.cellSize* canvasScaleFactor;
        _anchorGridTransform = gridParameters.anchorGridTransform;
        _slot = gridParameters.slot;
        _offset�enterCell = new Vector3(0.5f, 0.5f, 0) * _cellSizeWithScaleFactor; // ��������� �������� ��� ����� ����� , ����� ������ ����� (0,0) ���� � ������ ������� ������

        _gridObjectArray = new TGridObject[_width, _height]; // ������� ������ ����� ������������� �������� width �� height
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                _gridObjectArray[x, y] = createGridObject(this, gridPosition); // ������� ������� createGridObject � � �������� ��������� ���� GridSystemXY � ������� �����. ��������� ��� � ������ ������� ����� � ��������� ������ ��� x,y ��� ����� ������� �������.

                //Debug.DrawLine(GetWorldPositionCenter�ornerCell(gridPosition), GetWorldPositionCenter�ornerCell(gridPosition) + Vector3.right* .2f, Color.white, 1000); // ��� ����� �������� ��������� ����� � ������ ������ ������ �����
            }
        }       
    }

    public EquipmentSlot GetGridSlot() { return _slot; } // ������� ���� �����

    public virtual Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - _anchorGridTransform.position - _offset�enterCell; // ���������� ������� � 0 (������ �������� ����� ������������ ������ ��������� � �������� ������ ������� �������) 
        return new Vector2Int
            (
            Mathf.RoundToInt(localPosition.x / _cellSizeWithScaleFactor),  // ��������� Mathf.RoundToInt ��� �������������� float � int
            Mathf.RoundToInt(localPosition.y / _cellSizeWithScaleFactor)
            );
    }
    /// <summary>
    /// ������� ������� ���������� ������ �����
    /// </summary>
    public virtual Vector3 GetWorldPositionGridCenter() 
    {        
        // � ������� ����������� ����� ������� ������� �������� � ������ �������� �������
        float x = _anchorGridTransform.position.x+ _cellSizeWithScaleFactor * _width / 2; // ������ ������ ������� �� ���������� �����, ������� �������� ��� ������ �� � � ����� �������
        float y = _anchorGridTransform.position.y + _cellSizeWithScaleFactor * _height / 2;

        return new Vector3(x, y, 0);
    }
    /// <summary>
    /// ������� ������� ���������� ������ ������
    /// </summary>
    public virtual Vector3 GetWorldPositionCenter�ornerCell(Vector2Int gridPosition) 
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor + _anchorGridTransform.position + _offset�enterCell;   // ������� ���������� ����� ����� � ������ �� ��������, ������� �������� ����� ����� _anchorGridTransform � �������� ������� ������
                                                                                                                    // �� ����� ��� �� ����� ������ ��� ������ �������������� ������ _anchorGridTransform  � ����� ���� ����� �������� � ***Grid.transform.position                                                                                                              
    }
    /// <summary>
    /// ������� ������� ���������� ������� ������ ����� ������
    /// </summary>
    public virtual Vector3 GetWorldPositionLowerLeft�ornerCell(Vector2Int gridPosition) 
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0) * _cellSizeWithScaleFactor + _anchorGridTransform.position;   // ������� ���������� ����� ����� � ������ �� ��������, ������� �������� ����� ����� _anchorGridTransform                                                                                        
    }


    public void CreateDebugObject(Transform debugPrefab) // ������� ������ ������� ( public ��� �� ������� �� ������ Testing � ������� ������� �����)   // ��� Transform � GameObject ��������������� �.�. � ������ GameObject ���� Transform � � ������� Transform ���� ������������� GameObject
                                                         // � �������� ��� ������ ��� ����� Transform �������� �������. ���� � ��������� ������� ��� GameObject, ����� � ������, ���� �� �� ������ ����� ������� GameObject �������� ��� �������, ��� �������� ������ �������������� ��� "debugGameObject.Transform.LocalScale..."
                                                         // ������� ��� ��������� ���� � ��������� ��������� ��� Transform.
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y); // ������� �����

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPositionCenter�ornerCell(gridPosition), Quaternion.identity);  // �������� ��������� ����������� �������(debugPrefab) � ������ ������ ����� // �.�. ��� ���������� MonoBehaviour �� �� ����� �������� ������������ Instantiate ������ ����� GameObject.Instantiate
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>(); // � ���������� ������ ������� ��������� GridDebugObject
                gridDebugObject.SetGridObject(GetGridObject(gridPosition)); // �������� ����� SetGridObject() � �������� ���� ������� ����� ����������� � ������� _gridPositioAnchor // GetGridObject(_gridPositioAnchor) as GridObjectUnitXZ - �������� ��������� <TGridObject> ��� GridObjectUnitXZ
            }
        }
    }

    public TGridObject GetGridObject(Vector2Int gridPosition) // ������ ������� ������� ��������� � ������ ������� ����� .������� ��������� �.�. ����� ����������� �������� �� ���.
    {
        return _gridObjectArray[gridPosition.x, gridPosition.y]; // x,y ��� ������� ������� �� ������� ����� ������� ������ �������
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) // �������� �� ���������� �������� ��������  // ��������� ��� ���������� ��� �������� ������ 0 � ������ ������ � ������ ����� �����
    {
        return gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < _width &&
                gridPosition.y < _height;
    }

    public Vector3 GetRotationAnchorGrid()
    {
        return _anchorGridTransform.eulerAngles;
    }

    public Transform GetAnchorGrid()
    {
        return _anchorGridTransform;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }
    /// <summary>
    /// ������ ������ � ������ �������� CANVAS
    /// </summary>
    public float GetCellSizeWithScaleFactor()
    {
        return _cellSizeWithScaleFactor;
    }

}
