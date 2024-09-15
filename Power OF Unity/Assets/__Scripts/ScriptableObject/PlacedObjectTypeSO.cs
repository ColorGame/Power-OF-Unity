using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, ������ ��� ���������� � ��������, ������� ����� ������� � ��������� � ����������. 
/// </summary>
/// <remarks>
/// abstract - ������ ������� ��������� ������� ������.
/// �� �������� ��, ������ �����, ������ ������������ ��������, ����� ��� "SwordTypeSO"  "ShootingWeaponTypeSO" ��� "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>
public abstract class PlacedObjectTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver ��������� ��� ��������� �������� ������� ��� ������������ � ��������������.����� ������������ ��� �������� ID ��� ������������
                                                                                           //������������� ���������� ������� ������ ��������� ������� (� ��������� ����������� ���������) � �����. �������������� ��� ������� �������� ������������ � �� ������,
                                                                                           //������� ��������� � ������, �� ����� ������������ ��������� ������� � ������������ ���� ������ � ������ �����.
{
    [Header("������������� ��������������� UUID ��� ����������/��������.\n�������� ��� ����, ���� �� ������ ������������� �����.")]
    [SerializeField] private string _itemID = null;
    [Header("��� ������������ �������")]
    [SerializeField] private PlacedObjectType _placedObjectType;
    [Header("������ ������������ ������� 2D(��� Canvas)")]
    [SerializeField] private Transform _prefab2D;
    [Header("���������� ����� ������������ ������� 2D(��� ������ Canvas)")]
    [SerializeField] private Transform _visual2D;
    [Header("������� �������� ������ � ������ �")]
    [Range(1, 5)][SerializeField] private int _widthX;
    [Header("������� �������� ������ � ������ �")]
    [Range(1, 2)][SerializeField] private int _heightY;   
    [Header("������ ������ ���������� �� ������� ����� ���������� ��� ������")]
    [SerializeField] private List<EquipmentSlot> _canPlacedOnSlotList;
    [Header("��� ������������ ������� � �����������")]
    [Range(0, 50)][SerializeField] private int _weight;
    [Header("���� ������� � ������� �� �����")]
    [SerializeField] private uint _priceBuy ;
    [SerializeField] private uint _priceSell ;

    /* [Tooltip("������ ������������ ������� 3D/��� �������� � ���������� �� ������")]
     [SerializeField] private Transform _prefab3D;*/

    // ������������ ���������
    static Dictionary<string, PlacedObjectTypeWithActionSO> placedObjectLookupCache; //������������ ������� ������ ������� ���� PlacedObjectTypeWithActionSO// ����������� ������� (����-ID ����� ��������, ��������)


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
  //  public Transform GetPrefab3D() { return _prefab3D; }
    public Transform GetPrefab2D() { return _prefab2D; }
    public Transform GetVisual2D() { return _visual2D; }

    /// <summary>
    /// ��������� �������� ������ ������� ������������ �����
    /// </summary>  
    public Vector3 GetOffsetVisual�enterFromAnchor()
    {
        RectTransform rectTransformPrefab2D = (RectTransform)_prefab2D.transform;  
        Vector2 center = rectTransformPrefab2D.sizeDelta / 2;
               
        return center;
    }

    /// <summary>
    /// �������� ���������� ����� ������� �������� ������ � ������ � � ������ ��
    /// /// </summary>    
    public Vector2Int GetWidthXHeightYInCells() { return new Vector2Int(_widthX, _heightY); }  

    /// <summary>
    /// ������ ������ �� ������� ����� ���������� ��� ������
    /// </summary>
    public List<EquipmentSlot> GetCanPlacedOnSlotList() { return _canPlacedOnSlotList; }
       
    /// <summary>
    /// �������� ���� �������
    /// </summary>
    public uint GetPriceBuy() { return _priceBuy; }
    /// <summary>
    /// �������� ���� �������
    /// </summary>
    public uint GetPriceSell() { return _priceSell; }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // ������������ � ��������� ����� UUID, ���� �� ���� ��� ��� ������ ������ �������.
        if (string.IsNullOrWhiteSpace(_itemID))
        {
            _itemID = Guid.NewGuid().ToString();
        }

        // ������� ��� ������������ ������� � ���� ������
        if(name!=null)
        _placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(name);// ����������� ������ �� ������ � Enum ����<PlacedObjectType>        
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // ��������� ISerializationCallbackReceiver, �� ��� �� ����� ������ � ���� ������.
    }

}
