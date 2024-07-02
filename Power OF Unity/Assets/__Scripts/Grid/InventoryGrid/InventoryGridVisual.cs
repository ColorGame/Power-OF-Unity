using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������ ���������� � ����� ���������
/// </summary>
/// <remarks>�������� � ����� PlacedObject</remarks>
public class InventoryGridVisual : MonoBehaviour // �������� ������� ������������ ���������
{
    
    [Serializable] // ����� ��������� ��������� ����� ������������ � ����������
    public struct GridVisualTypeMaterial    //������ ����� ��� ��������� // �������� ��������� ����� � ��������� ������. ������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#
    {                                       //� ������ ��������� ��������� ��������� ����� � ����������
        public GridVisualType gridVisualType;
        public Material materialGrid;
    }

    public enum GridVisualType //���������� ��������� �����
    {
        Grey,
        White,
        Blue,
        BlueSoft,
        Red,
        RedSoft,
        Yellow,
        YellowSoft,
        Green,
        GreenSoft,
    }

    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList; // ������ ��� ��������� ����������� ��������� ����� ������� (������ �� ���������� ���� ������) ����������� ��������� ����� // � ���������� ��� ������ ��������� ���������� ��������������� �������� �����

    private InventoryGridVisualSingle[][,] _inventoryGridSystemVisualSingleArray; // ������ ������� [���������� �����][����� (�), ������ (�)]
    private Dictionary<InventorySlot, int> _gridNameIndexDictionary; // ������� (InventorySlot - ����, int(������) -��������)
    private List<GridSystemXY<GridObjectInventoryXY>> _gridSystemXYList; // ������ �����

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private InventoryGrid _inventoryGrid;

    
    public void Init(PickUpDropPlacedObject pickUpDrop, InventoryGrid inventoryGrid)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _inventoryGrid = inventoryGrid;

        Setup();
    }


    private void Setup()
    {
        _gridNameIndexDictionary = new Dictionary<InventorySlot, int>();

        // �������������� ������� ������ ����� ������� - ���������� �����
        _gridSystemXYList = _inventoryGrid.GetGridSystemXYList(); // ������� ������ �����
        _inventoryGridSystemVisualSingleArray = new InventoryGridVisualSingle[_gridSystemXYList.Count][,];

        // ��� ������ ����� ��������� ��������� ������ ���������
        for (int i = 0; i < _gridSystemXYList.Count; i++)
        {
            _inventoryGridSystemVisualSingleArray[i] = new InventoryGridVisualSingle[_gridSystemXYList[i].GetWidth(), _gridSystemXYList[i].GetHeight()];
        }


        for (int i = 0; i < _gridSystemXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // � ������
                {
                    Vector2Int gridPosition = new Vector2Int(x, y); // ������� �����
                    Vector3 rotation = _inventoryGrid.GetRotationAnchorGrid(_gridSystemXYList[i]);
                    Transform AnchorGridTransform = _inventoryGrid.GetAnchorGrid(_gridSystemXYList[i]);

                    Transform gridSystemVisualSingleTransform = Instantiate(GameAssets.Instance.inventoryGridSystemVisualSinglePrefab, _inventoryGrid.GetWorldPositionCenter�ornerCell(gridPosition, _gridSystemXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // �������� ��� ������ � ������ ������� �����

                    _gridNameIndexDictionary[_gridSystemXYList[i].GetGridSlot()] = i; // �������� �����(��� ����� ��� ���� ��������) �������� (������ �������)
                    _inventoryGridSystemVisualSingleArray[i][x, y] = gridSystemVisualSingleTransform.GetComponent<InventoryGridVisualSingle>(); // ��������� ��������� LevelGridVisualSingle � ���������� ������ ��� x,y,y ��� ����� ������� �������.
                }
            }
        }

        _pickUpDropPlacedObject.OnAddPlacedObjectAtInventoryGrid += PickUpDropSystem_OnAddPlacedObjectAtGrid;
        _pickUpDropPlacedObject.OnRemovePlacedObjectAtInventoryGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
        _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;
    }

    // ���������� ������ ������� �����
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)  // ���������� ������ ������� �����
    {
        SetDefaultState(); // ��������� ��������� ��������� ���� �����
    }

    // ������� ������������ ������� �� ����� ����������
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PlacedObjectParameters e)
    {
        SetDefaultState(); // ��������� ��������� ��������� ���� �����
        ShowPossibleGridPositions(e.slot, e.placedObject, e.gridPositioAnchor, GridVisualType.Yellow); //�������� ��������� �������� �������
    }

    // ������ ������ �� �����
    private void PickUpDropSystem_OnRemovePlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Yellow);
    }

    // ������ �������� � ����� 
    private void PickUpDropSystem_OnAddPlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }

    private void SetDefaultState() // ���������� ��������� ��������� �����
    {
        for (int i = 0; i < _gridSystemXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // � ������
                {
                    if (!_inventoryGridSystemVisualSingleArray[i][x, y].GetIsBusy()) // ���� ������� �� ������
                    {
                        _inventoryGridSystemVisualSingleArray[i][x, y].Show(GetGridVisualTypeMaterial(GridVisualType.Grey));
                    }
                }
            }
        }
    }

    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = placedObject.GetGridSystemXY(); // �������� ������� ������� �������� ������
        List<Vector2Int> OccupiesGridPositionList = placedObject.GetOccupiesGridPositionList(); // ������ ���������� �������� �������
        InventorySlot inventorySlot = gridSystemXY.GetGridSlot(); // ���� �����

        switch (inventorySlot)
        {
            case InventorySlot.BagSlot:
                int index = _gridNameIndexDictionary[inventorySlot]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

                foreach (Vector2Int gridPosition in OccupiesGridPositionList) // ��������� ����������� �������� ������� �����
                {
                    //��������� ��������� ����� (� ���� isBusy = false(������ ��������) �� ��������� ���������� ��������)
                    _inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                }
                break;

            //������, �������� � ���. ����� ������, ����� ������ ����� � ���� �����, �.�. ��� ����� ����������� ������ ���� �������
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponsSlot:

                index = _gridNameIndexDictionary[inventorySlot]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

                for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // ��� ���������� ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // � ������
                    {
                        //��������� ��������� ����� (� ���� isBusy = false(������ ��������) �� ��������� ���������� ��������)
                        _inventoryGridSystemVisualSingleArray[index][x, y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                    }
                }

                break;
        }

    }
        
    /// <summary>
    /// �������� ��������� �������� �������
    /// </summary>
    private void ShowPossibleGridPositions(InventorySlot inventorySlot, PlacedObject placedObject, Vector2Int gridPositioAnchor, GridVisualType gridVisualType)
    {       
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = _inventoryGrid.GetGridSystemXY(inventorySlot); // ������� ����� ��� ������� �����
        switch (inventorySlot)
        {
            case InventorySlot.BagSlot:

                int index = _gridNameIndexDictionary[inventorySlot]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray                
                List<Vector2Int> TryOccupiesGridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositioAnchor); // ������ �������� ������� ������� ����� ������
                foreach (Vector2Int gridPosition in TryOccupiesGridPositionList) // ��������� ������ ������� ������� ���� ������
                {
                    if (gridSystemXY.IsValidGridPosition(gridPosition)) // ���� ������� ��������� ��...
                    {
                        if (!_inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].GetIsBusy()) // ���� ������� �� ������
                        {
                            _inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
                        }
                    }
                }
                break;
            //����� ������ ��� �������� � ���. ����� ������, ����� ���������� ��� �� ����� ������ ����� ��� �����, ���� ���� �� �������� � ����� ������
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponsSlot:

                index = _gridNameIndexDictionary[inventorySlot]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

                for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // ��� ���������� ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // � ������
                    {
                        if (_inventoryGridSystemVisualSingleArray[index][x, y].GetIsBusy()) // ���� ������ ���� ������� ������ �� ������� �� ����� � �����. ��� ����
                        {
                            break;
                        }                                                   
                        _inventoryGridSystemVisualSingleArray[index][x, y].Show(GetGridVisualTypeMaterial(gridVisualType));
                       
                    }
                }
                break;
        }
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) //(������� �������� � ����������� �� ���������) �������� ��� ��������� ��� �������� ������������ � ����������� �� ����������� � �������� ��������� �������� ������������
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList) // � ����� ��������� ������ ��� ��������� ����������� ��������� ����� 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // ����  ��������� �����(gridVisualType) ��������� � ���������� ��� ��������� �� ..
            {
                return gridVisualTypeMaterial.materialGrid; // ������ �������� ��������������� ������� ��������� �����
            }
        }

        Debug.LogError("�� ���� ����� GridVisualTypeMaterial ��� GridVisualType " + gridVisualType); // ���� �� ������ ����������� ������ ������
        return null;
    }
}
