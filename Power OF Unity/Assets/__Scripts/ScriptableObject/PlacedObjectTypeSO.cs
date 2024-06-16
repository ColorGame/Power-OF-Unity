using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, ������ ��� ���������� � ��������, ������� ����� ������� � ��������� � ���������. 
/// </summary>
/// <remarks>
/// abstract - ������ ������� ��������� ������� ������.
/// �� �������� ��, ������ �����, ������ ������������ ��������, ����� ��� "WeaponPlacedObjectSO"  "ItemPlacedObjectSO" ��� "ModulePlacedObjectSO" "EquipmentPlacedObjectSO".
/// </remarks>
public abstract class PlacedObjectTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver ��������� ��� ��������� �������� ������� ��� ������������ � ��������������.����� ������������ ��� �������� ID ��� ������������
                                                                                           //������������� ���������� ������� ������ ��������� ������� (� ��������� ����������� ���������) � �����. �������������� ��� ������� �������� ������������ � �� ������,
                                                                                           //������� ��������� � ������, �� ����� ������������ ��������� ������� � ������������ ���� ������ � ������ �����.
{
    [Tooltip("������������� ��������������� UUID ��� ����������/��������. �������� ��� ����, ���� �� ������ ������������� �����.")]
    [SerializeField] private string _itemID = null;
    [Tooltip("��� ������������ �������")]
    [SerializeField] private PlacedObjectType _placedObjectType;
    [Tooltip("������ ������������ �������")]
    [SerializeField] private Transform _prefab;
    [Tooltip("���������� ����� ������������ �������")]
    [SerializeField] private Transform _visual;
    [Tooltip("������� �������� ������ � ������ �")]
    [SerializeField] private int _widthX;
    [Tooltip("������� �������� ������ � ������ �")]
    [SerializeField] private int _heightY;
    [Tooltip("������ ������ ��������� �� ������� ����� ���������� ��� ������")]
    [SerializeField] private List<InventorySlot> _canPlacedOnSlotList;
    [Tooltip("��� ������������ ������� � �����������")]
    [Range(0, 50)][SerializeField] private int _weight;

    private BaseAction[] _baseActionArray; // ������ ������� �������� ������������� � ������� �������� �� ����� ���� ��������� (�������� GrappleComboAction � GrappleAction)

    // ������������ ���������
    static Dictionary<string, PlacedObjectTypeSO> placedObjectLookupCache; //������������ ������� ������ ������� ���� PlacedObjectTypeSO// ����������� ������� (����-ID ����� ��������, ��������)


    public virtual PlacedObjectTooltip GetPlacedObjectTooltip() // �������� ����������� ��������� ��� ������� ������������ ������� // virtual- ������������� � ����������� �������
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(_placedObjectType);
    }

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

    public Transform GetPrefab() { return _prefab; }

    /// <summary>
    /// ��������� �������� ������� ������������ ��������
    /// </summary>  
    public Vector3 GetOffsetVisualFromParent()
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

    public Transform GetVisual() { return _visual; }

    /// <summary>
    /// ������ ������ �� ������� ����� ���������� ��� ������
    /// </summary>
    public List<InventorySlot> GetCanPlacedOnSlotList() { return _canPlacedOnSlotList; }

    public BaseAction[] GetBaseActionsArray()
    {
        if(_baseActionArray.Length == 0) // ���� ��� �� ��������� �� 
        {
          return  _baseActionArray = _prefab.GetComponents<BaseAction>();
        }
        return _baseActionArray;
    }

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
