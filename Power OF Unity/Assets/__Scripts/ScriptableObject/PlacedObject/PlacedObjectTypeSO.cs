using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// ScriptableObject, ������ ��� ���������� � ��������, ������� ����� ������� � ��������� � ����������. 
/// </summary>
/// <remarks>
/// abstract - ������ ������� ��������� ������� ������.
/// �� �������� ��, ������ �����, ������ ������������ ��������, ����� ��� "SwordTypeSO"  "ShootingWeaponTypeSO" ��� "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>
public abstract class PlacedObjectTypeSO : ScriptableObject//, ISerializationCallbackReceiver//ISerializationCallbackReceiver ��������� ��� ��������� �������� ������� ��� ������������ � ��������������.����� ������������ ��� �������� ID ��� ������������
                                                           //������������� ���������� ������� ������ ��������� ������� (� ��������� ����������� ���������) � �����. �������������� ��� ������� �������� ������������ � �� ������,
                                                           //������� ��������� � ������, �� ����� ������������ ��������� ������� � ������������ ���� ������ � ������ �����.
{
    /*[Header("������������� ��������������� UUID ��� ����������/��������.\n�������� ��� ����, ���� �� ������ ������������� �����.")]
    [SerializeField] private string _itemID = null;   */
    [Header("������ ������������ ������� 2D(��� Canvas)")]
    [SerializeField] protected Transform _prefab2D;
    [Header("���������� ����� ������������ ������� 2D(��� ������ Canvas)")]
    [SerializeField] protected Transform _visual2D;
    [Header("������ ����������� 2D(��� ������ Canvas)")]
    [SerializeField] protected float _heightImage2D;
    [Header("������� �������� ������ � ������ �")]
    [Range(1, 5)][SerializeField] private int _widthX;
    [Header("������� �������� ������ � ������ �")]
    [Range(1, 5)][SerializeField] private int _heightY;
    [Header("������ ������ ���������� �� ������� ����� ���������� ��� ������")]
    [SerializeField] private List<EquipmentSlot> _canPlacedOnSlotList;
    [Header("��� ������������ ������� � �����������")]
    [Range(0, 50)][SerializeField] private int _weight;
    [Header("���� ������� � ������� �� �����")]
    [SerializeField] private uint _priceBuy;
    [SerializeField] private uint _priceSell;

    private HashSet<EquipmentSlot> _canPlacedOnSlotHashList;

    // ������������ ���������
    static Dictionary<string, PlacedObjectTypeWithActionSO> placedObjectLookupCache; //������������ ������� ������ ������� ���� PlacedObjectTypeWithActionSO// ����������� ������� (����-ID ����� ��������, ��������)

    public abstract PlacedObjectTooltip GetPlacedObjectTooltip(); // �������� ����������� ��������� ��� ������� ������������ ������� // ��� ������� ���� ���� ����������


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

    public Transform GetPrefab2D() { return _prefab2D; }
    public Transform GetVisual2D() { return _visual2D; }
    public float GetHeightImage2D() { return _heightImage2D; }

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
    public HashSet<EquipmentSlot> GetCanPlacedOnSlotList()
    {
        if (_canPlacedOnSlotHashList == null)
        {
            _canPlacedOnSlotHashList = new HashSet<EquipmentSlot>(_canPlacedOnSlotList);
        }
        return _canPlacedOnSlotHashList;
    }

    /// <summary>
    /// �������� ���� �������
    /// </summary>
    public uint GetPriceBuy() { return _priceBuy; }
    /// <summary>
    /// �������� ���� �������
    /// </summary>
    public uint GetPriceSell() { return _priceSell; }



    /*  void  ISerializationCallbackReceiver.OnBeforeSerialize()
      {
          // ������������ � ��������� ����� UUID, ���� �� ���� ��� ��� ������ ������ �������.
          if (string.IsNullOrWhiteSpace(_itemID))
          {
              _itemID = Guid.NewGuid().ToString();
          }

         *//* // ������� ��� ������������ ������� � ���� ������
          
      }
      void ISerializationCallbackReceiver.OnAfterDeserialize()
      {
          // ��������� ISerializationCallbackReceiver, �� ��� �� ����� ������ � ���� ������.
      }*/

    protected virtual void AutoCompletion()
    {
        string name = _visual2D.name; // ����� ����� ������������ ������� ������*������ (3�2). ���������
        char height = name[name.Length - 1];// ������� 1-� ������ � ����� - ��� ������
        char width = name[name.Length - 3]; // ������� 3-� ������ � ����� - ��� ������
        if (int.TryParse(width.ToString(), out int result))
            _widthX = result;
        if (int.TryParse(height.ToString(), out result))
            _heightY = result;

        _heightImage2D = _visual2D.GetComponentInChildren<Image>().rectTransform.sizeDelta.y; // ������� ������ ���������� ����������� 

    }

    protected void Search2DPrefabAndVisual(string nameFail, GameObject[] gameObjectArray)
    {
        List<GameObject> prefab2DList = new();
        List<GameObject> visual2DList = new();

        foreach (GameObject gameObject in gameObjectArray)
        {
            if (gameObject.name.Contains("Visual"))
            {
                visual2DList.Add(gameObject);
            }
            else
            {
                prefab2DList.Add(gameObject);
            }
        }

        int prefabDeleteLastCharName = 7; // ���������� �������� ��� �������� � ����� ������� (����� ������ 2D_1X1 ) 
        int visualDeleteLastCharName = 13;// ���������� �������� ��� �������� � ����� ������� (����� ������ 2DVisual_1X1 )

        foreach (GameObject go in prefab2DList)
        {
            string prefabName = go.name.Remove(go.name.Length - prefabDeleteLastCharName); // ������� ��� ������� ��� ��������� 6 ��������
            if (nameFail.Equals(prefabName, StringComparison.OrdinalIgnoreCase)) // ������� ��� SO � ��������� ��� ����� ��������
            {
                _prefab2D = go.transform;
            }
        }
        foreach (GameObject go in visual2DList)
        {
            string visualName = go.name.Remove(go.name.Length - visualDeleteLastCharName); // ������� ��� ������� ��� ��������� 13 ��������
            if (nameFail.Equals(visualName, StringComparison.OrdinalIgnoreCase)) // ������� ��� SO � ��������� ��� ����� ��������
            {
                _visual2D = go.transform;
            }
        }

        if (_prefab2D == null)
        {
            Debug.Log($"�� ������� ��������� (_prefab2D). ������� ��� {name}");
        }
        if (_visual2D == null)
        {
            Debug.Log($"�� ������� ��������� (_visual2D). ������� ��� {name}");
        }
    }

}
