using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� InventoryGridVisual ��� ������� ����� ������� �� ���������, ��������� �� �����, ����� ���������� ������� ����������� ����� ����� ����������.
// (Project Settings/ Script Execution Order � �������� ���������� InventoryGridVisual ���� Default Time)
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
    private List<GridSystemTiltedXY<GridObjectInventoryXY>> _gridSystemTiltedXYList; // ������ �����

    private PickUpDrop _pickUpDrop;
    private InventoryGrid _inventoryGrid;

    
    public void Init(PickUpDrop pickUpDrop, InventoryGrid inventoryGrid)
    {
        _pickUpDrop = pickUpDrop;
        _inventoryGrid = inventoryGrid;        
    }


    private void Start()
    {
        _gridNameIndexDictionary = new Dictionary<InventorySlot, int>();

        // �������������� ������� ������ ����� ������� - ���������� �����
        _gridSystemTiltedXYList = _inventoryGrid.GetGridSystemTiltedXYList(); // ������� ������ �����
        _inventoryGridSystemVisualSingleArray = new InventoryGridVisualSingle[_gridSystemTiltedXYList.Count][,];

        // ��� ������ ����� ��������� ��������� ������ ���������
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++)
        {
            _inventoryGridSystemVisualSingleArray[i] = new InventoryGridVisualSingle[_gridSystemTiltedXYList[i].GetWidth(), _gridSystemTiltedXYList[i].GetHeight()];
        }


        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // � ������
                {
                    Vector2Int gridPosition = new Vector2Int(x, y); // ������� �����
                    Vector3 rotation = _inventoryGrid.GetRotationAnchorGrid(_gridSystemTiltedXYList[i]);
                    Transform AnchorGridTransform = _inventoryGrid.GetAnchorGrid(_gridSystemTiltedXYList[i]);

                    Transform gridSystemVisualSingleTransform = Instantiate(GameAssets.Instance.inventoryGridSystemVisualSinglePrefab, _inventoryGrid.GetWorldPositionCenter�ornerCell(gridPosition, _gridSystemTiltedXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // �������� ��� ������ � ������ ������� �����

                    _gridNameIndexDictionary[_gridSystemTiltedXYList[i].GetGridSlot()] = i; // �������� �����(��� ����� ��� ���� ��������) �������� (������ �������)
                    _inventoryGridSystemVisualSingleArray[i][x, y] = gridSystemVisualSingleTransform.GetComponent<InventoryGridVisualSingle>(); // ��������� ��������� LevelGridVisualSingle � ���������� ������ ��� x,y,y ��� ����� ������� �������.
                }
            }
        }

        _pickUpDrop.OnAddPlacedObjectAtInventoryGrid += PickUpDropSystem_OnAddPlacedObjectAtGrid;
        _pickUpDrop.OnRemovePlacedObjectAtInventoryGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
        _pickUpDrop.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDrop.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;
    }

    // ���������� ������ ������� �����
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)  // ���������� ������ ������� �����
    {
        SetDefaultState(); // ��������� ��������� ��������� ���� �����
    }

    // ������� ������������ ������� �� ����� ����������
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PickUpDrop.OnGrabbedObjectGridPositionChangedEventArgs e)
    {
        SetDefaultState(); // ��������� ��������� ��������� ���� �����
        ShowPossibleGridPositions(e.gridSystemXY, e.placedObject, e.newMouseGridPosition, GridVisualType.Yellow); //�������� ��������� �������� �������
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
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // � ������
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
        GridSystemTiltedXY<GridObjectInventoryXY> gridSystemTiltedXY = placedObject.GetGridSystemXY(); // �������� ������� ������� �������� ������
        List<Vector2Int> OccupiesGridPositionList = placedObject.GetOccupiesGridPositionList(); // ������ ���������� �������� �������
        InventorySlot inventorySlot = gridSystemTiltedXY.GetGridSlot(); // ���� �����

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
            case InventorySlot.OtherWeaponSlot:

                index = _gridNameIndexDictionary[inventorySlot]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

                for (int x = 0; x < _gridSystemTiltedXYList[index].GetWidth(); x++) // ��� ���������� ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemTiltedXYList[index].GetHeight(); y++)  // � ������
                    {
                        //��������� ��������� ����� (� ���� isBusy = false(������ ��������) �� ��������� ���������� ��������)
                        _inventoryGridSystemVisualSingleArray[index][x, y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                    }
                }

                break;
        }

    }

    // �������� ��������� �������� �������
    private void ShowPossibleGridPositions(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, PlacedObject placedObject, Vector2Int mouseGridPosition, GridVisualType gridVisualType)
    {
        InventorySlot gridName = gridSystemXY.GetGridSlot(); // ��� ����� ��� ���������� ��� � ����������� ��������
        switch (gridName)
        {
            case InventorySlot.BagSlot:

                int index = _gridNameIndexDictionary[gridName]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray
                List<Vector2Int> TryOccupiesGridPositionList = placedObject.GetTryOccupiesGridPositionList(mouseGridPosition); // ������ �������� ������� ������� ����� ������
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
            case InventorySlot.OtherWeaponSlot:

                index = _gridNameIndexDictionary[gridName]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

                for (int x = 0; x < _gridSystemTiltedXYList[index].GetWidth(); x++) // ��� ���������� ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemTiltedXYList[index].GetHeight(); y++)  // � ������
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
