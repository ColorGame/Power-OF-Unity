using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, ������ ��� ���������� � ��������, ������� ����� ������� � ��������� � ���������. 
/// </summary>
/// <remarks>
/// abstract - ������ ������� ��������� ������� ������.
/// �� �������� ��, ������ �����, ������ ������������ ��������, ����� ��� "SwordTypeSO"  "ShootingWeaponTypeSO" ��� "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>
public abstract class PlacedObjectTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver ��������� ��� ��������� �������� ������� ��� ������������ � ��������������.����� ������������ ��� �������� ID ��� ������������
                                                                                           //������������� ���������� ������� ������ ��������� ������� (� ��������� ����������� ���������) � �����. �������������� ��� ������� �������� ������������ � �� ������,
                                                                                           //������� ��������� � ������, �� ����� ������������ ��������� ������� � ������������ ���� ������ � ������ �����.
{
    [Tooltip("������������� ��������������� UUID ��� ����������/��������. �������� ��� ����, ���� �� ������ ������������� �����.")]
    [SerializeField] private string _itemID = null;
    [Tooltip("��� ������������ �������")]
    [SerializeField] private PlacedObjectType _placedObjectType;
    [Tooltip("������ ������������ ������� 3D/��� �������� � ���������� �� ������")]
    [SerializeField] private Transform _prefab3D;
    [Tooltip("������ ������������ ������� 2D(��� Canvas)")]
    [SerializeField] private Transform _prefab2D;
    [Tooltip("���������� ����� ������������ ������� 2D(��� ������ Canvas)")]
    [SerializeField] private Transform _visual2D;
    [Tooltip("������� �������� ������ � ������ �")]
    [Range(1, 5)][SerializeField] private int _widthX;
    [Tooltip("������� �������� ������ � ������ �")]
    [Range(1, 2)][SerializeField] private int _heightY;   
    [Tooltip("������ ������ ��������� �� ������� ����� ���������� ��� ������")]
    [SerializeField] private List<InventorySlot> _canPlacedOnSlotList;
    [Tooltip("��� ������������ ������� � �����������")]
    [Range(0, 50)][SerializeField] private int _weight;


    // ������������ ���������
    static Dictionary<string, PlacedObjectTypeSO> placedObjectLookupCache; //������������ ������� ������ ������� ���� PlacedObjectTypeSO// ����������� ������� (����-ID ����� ��������, ��������)


    public virtual PlacedObjectTooltip GetPlacedObjectTooltip() // �������� ����������� ��������� ��� ������� ������������ ������� // virtual- ������������� � ����������� �������
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(_placedObjectType);
    }
    /// <summary>
    /// �������� ������� �������� ��� ������� PlacedObjectTypeSO
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����� � �������� ����� �������� BaseAction
    /// </remarks>
    public abstract BaseAction GetAction(Unit unit);

    /// <summary>
    /// ������ �������� ������� ������� �������� ������ ������������ ���������� �������� �������
    /// </summary>
    public List<Vector2Int> GetGridPositionList(Vector2Int gridPosition)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for (int x = 0; x < _widthX; x++)
        {
            for (int y = 0; y < _heightY; y++)
            {
                gridPositionList.Add(gridPosition + new Vector2Int(x, y));
            }
        }
        return gridPositionList;
    }

    public PlacedObjectType GetPlacedObjectType() { return _placedObjectType; }
    public Transform GetPrefab3D() { return _prefab3D; }
    public Transform GetPrefab2D() { return _prefab2D; }
    public Transform GetVisual2D() { return _visual2D; }

    /// <summary>
    /// ��������� �������� ������ ������� ������������ �����
    /// </summary>  
    public Vector3 GetOffsetVisual�enterFromAnchor()
    {
        float cellSize = InventoryGrid.GetCellSize();

        float x = cellSize * _widthX / 2; // ������ ������ ������� �� ���������� �����, ������� �������� ��� ������ �� � � ����� �������
        float y = cellSize * _heightY / 2;

        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// �������� ���������� ����� ������� �������� ������ � ������ � � ������ ��
    /// /// </summary>    
    public Vector2Int GetWidthXHeightYInCells() { return new Vector2Int(_widthX, _heightY); }  

    /// <summary>
    /// ������ ������ �� ������� ����� ���������� ��� ������
    /// </summary>
    public List<InventorySlot> GetCanPlacedOnSlotList() { return _canPlacedOnSlotList; }
       
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // ������������ � ��������� ����� UUID, ���� �� ���� ��� ��� ������ ������ �������.
        if (string.IsNullOrWhiteSpace(_itemID))
        {
            _itemID = Guid.NewGuid().ToString();
        }
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // ��������� ISerializationCallbackReceiver, �� ��� �� ����� ������ � ���� ������.
    }

}
