using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� InventoryGridSystemVisual ��� ������� ����� ������� �� ���������, ��������� �� �����, ����� ���������� ������� ����������� ����� ����� ����������.
// (Project Settings/ Script Execution Order � �������� ���������� InventoryGridSystemVisual ���� Default Time)
public class InventoryGridSystemVisual : MonoBehaviour // �������� ������� ������������ ���������
{
    public static InventoryGridSystemVisual Instance { get; private set; }

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

    private InventoryGridSystemVisualSingle[][,] _inventoryGridSystemVisualSingleArray; // ������ ������� [���������� �����][����� (�), ������ (�)]
    private Dictionary<GridName, int> _gridNameIndexDictionary; // ������� (GridName - ����, int(������) -��������)
    private List<GridSystemTiltedXY<GridObjectInventoryXY>> _gridSystemTiltedXYList; // ������ �����


    private void Awake() //��� ��������� ������ Awake ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one InventoryGridSystemVisual!(��� ������, ��� ���� InventoryGridSystemVisual!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� InventoryGridSystemVisual ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    private void Start()
    {
        _gridNameIndexDictionary = new Dictionary<GridName, int>();

        // �������������� ������� ������ ����� ������� - ���������� �����
        _gridSystemTiltedXYList = InventoryGrid.Instance.GetGridSystemTiltedXYList(); // ������� ������ �����
        _inventoryGridSystemVisualSingleArray = new InventoryGridSystemVisualSingle[_gridSystemTiltedXYList.Count][,];

        // ��� ������ ����� ��������� ��������� ������ ���������
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++)
        {
            _inventoryGridSystemVisualSingleArray[i] = new InventoryGridSystemVisualSingle[_gridSystemTiltedXYList[i].GetWidth(), _gridSystemTiltedXYList[i].GetHeight()];
        }


        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // � ������
                {
                    Vector2Int gridPosition = new Vector2Int(x, y); // ������� �����
                    Vector3 rotation = InventoryGrid.Instance.GetRotationAnchorGrid(_gridSystemTiltedXYList[i]);
                    Transform AnchorGridTransform = InventoryGrid.Instance.GetAnchorGrid(_gridSystemTiltedXYList[i]);

                    Transform gridSystemVisualSingleTransform = Instantiate(GameAssets.Instance.inventoryGridSystemVisualSinglePrefab, InventoryGrid.Instance.GetWorldPositionCenter�ornerCell(gridPosition, _gridSystemTiltedXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // �������� ��� ������ � ������ ������� �����

                    _gridNameIndexDictionary[_gridSystemTiltedXYList[i].GetGridName()] = i; // �������� �����(��� ����� ��� ���� ��������) �������� (������ �������)
                    _inventoryGridSystemVisualSingleArray[i][x, y] = gridSystemVisualSingleTransform.GetComponent<InventoryGridSystemVisualSingle>(); // ��������� ��������� LevelGridSystemVisualSingle � ���������� ������ ��� x,y,y ��� ����� ������� �������.
                }
            }
        }

        PickUpDropSystem.Instance.OnAddPlacedObjectAtGrid += PickUpDropSystem_OnAddPlacedObjectAtGrid;
        PickUpDropSystem.Instance.OnRemovePlacedObjectAtGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
        PickUpDropSystem.Instance.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        PickUpDropSystem.Instance.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;
    }

    // ���������� ������ ������� �����
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)  // ���������� ������ ������� �����
    {
        SetDefaultState(); // ��������� ��������� ��������� ���� �����
    }

    // ������� ������������ ������� �� ����� ����������
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PickUpDropSystem.OnGrabbedObjectGridPositionChangedEventArgs e)
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
        GridName gridName = gridSystemTiltedXY.GetGridName(); // ��� �����

        switch (gridName)
        {
            case GridName.BagGrid1:
                int index = _gridNameIndexDictionary[gridName]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

                foreach (Vector2Int gridPosition in OccupiesGridPositionList) // ��������� ����������� �������� ������� �����
                {
                    //��������� ��������� ����� (� ���� isBusy = false(������ ��������) �� ��������� ���������� ��������)
                    _inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                }
                break;

            //������, �������� � ���. ����� ������, ����� ������ ����� � ���� �����, �.�. ��� ����� ����������� ������ ���� �������
            case GridName.MainWeaponGrid2:
            case GridName.OtherWeaponGrid3:

                index = _gridNameIndexDictionary[gridName]; //������ �� ������� ������ ����� � _inventoryGridSystemVisualSingleArray

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
        GridName gridName = gridSystemXY.GetGridName(); // ��� ����� ��� ���������� ��� � ����������� ��������
        switch (gridName)
        {
            case GridName.BagGrid1:

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
            case GridName.MainWeaponGrid2:
            case GridName.OtherWeaponGrid3:

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
