using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������ ���������� � ����� ��������� ����������
/// </summary>
/// <remarks>�������� � ����� PlacedObject</remarks>
public class ItemGridVisual : MonoBehaviour, IToggleActivity
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
        Orange,
    }

    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList; // ������ ��� ��������� ����������� ��������� ����� ������� (������ �� ���������� ���� ������) ����������� ��������� ����� // � ���������� ��� ������ ��������� ���������� ��������������� �������� �����

    private EquipmentGridVisualSingle[][,] _equipmentGridVisualSingleArray; // ������ ������� [���������� �����][����� (�), ������ (�)]
    private Dictionary<EquipmentSlot, int> _gridNameIndexDictionary; // ������� (EquipmentSlot - ����, int(������) -��������)
    private List<GridSystemXY<GridObjectEquipmentXY>> _gridSystemXYList; // ������ �����

    private Canvas _canvasItemGrid;

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private EquipmentGrid _equipmentGrid;
    private UnitEquipmentSystem _unitEquipmentSystem;


    public void Init(PickUpDropPlacedObject pickUpDrop, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;

        Setup();
    }


    private void Setup()
    {
        _canvasItemGrid = _equipmentGrid.GetCanvasItemGrid();
        _gridNameIndexDictionary = new Dictionary<EquipmentSlot, int>();

        // �������������� ������� ������ ����� ������� - ���������� �����
        _gridSystemXYList = _equipmentGrid.GetItemGridSystemXYList(); // ������� ������ �����
        _equipmentGridVisualSingleArray = new EquipmentGridVisualSingle[_gridSystemXYList.Count][,];

        // ��� ������ ����� ��������� ��������� ������ ���������
        for (int i = 0; i < _gridSystemXYList.Count; i++)
        {
            _equipmentGridVisualSingleArray[i] = new EquipmentGridVisualSingle[_gridSystemXYList[i].GetWidth(), _gridSystemXYList[i].GetHeight()];
        }

        for (int i = 0; i < _gridSystemXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // � ������
                {
                    Vector2Int gridPosition = new Vector2Int(x, y); // ������� �����
                    Vector3 rotation = _equipmentGrid.GetRotationAnchorGrid(_gridSystemXYList[i]);
                    Transform AnchorGridTransform = _equipmentGrid.GetAnchorGrid(_gridSystemXYList[i]);

                    EquipmentGridVisualSingle equipmentGridVisualSingle;

                    switch (_canvasItemGrid.renderMode)
                    {
                        default:
                        case RenderMode.ScreenSpaceOverlay:
                        case RenderMode.ScreenSpaceCamera:
                            equipmentGridVisualSingle = Instantiate(GameAssets.Instance.EquipmentGridInVisualSingleScreenSpacePrefab, _equipmentGrid.GetWorldPositionCenter�ornerCell(gridPosition, _gridSystemXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // �������� ��� ������ � ������ ������� �����
                            break;
                        case RenderMode.WorldSpace:
                            equipmentGridVisualSingle = Instantiate(GameAssets.Instance.EquipmentGridInVisualSingleWorldSpacePrefab, _equipmentGrid.GetWorldPositionCenter�ornerCell(gridPosition, _gridSystemXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // �������� ��� ������ � ������ ������� �����
                            break;
                    }

                    equipmentGridVisualSingle.Init(_gridSystemXYList[i].GetCellSizeWithScaleFactor());

                    _gridNameIndexDictionary[_gridSystemXYList[i].GetGridSlot()] = i; // �������� �����(��� ����� ��� ���� ��������) �������� (������ �������)
                    _equipmentGridVisualSingleArray[i][x, y] = equipmentGridVisualSingle; // ��������� ��������� LevelGridVisualSingle � ���������� ������ ��� x,y,y ��� ����� ������� �������.
                }
            }
        }
    }

    public void SetActive(bool active)
    {
        _canvasItemGrid.enabled = active;
        if (active)
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared += UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
        }
        else
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid -= PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged -= PickUpDropManager_OnGrabbedObjectGridPositionChanged;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits -= PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared -= UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
        }
    }

    /// <summary>
    /// ��������� ������
    /// </summary>    
    private void UnitEquipmentSystem_OnEquipmentGridsCleared(object sender, EventArgs e)
    {
        SetDefoltState(); // ���������� ��������� ���������        
    }
    /// <summary>
    /// ���������� ������ ������� �����
    /// </summary>
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)
    {
        UpdateVisual();
    }
    /// <summary>
    /// ������� ������������ ������� �� ����� ����������
    /// </summary>
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PlacedObjectGridParameters e)
    {
        UpdateVisual();
        ShowPossibleGridPositions(e.slot, e.placedObject, e.gridPositioAnchor, GridVisualType.Orange); //�������� ��������� �������� �������
    }
    /// <summary>
    /// ������ ������ �� ����� � ����� ��� ���
    /// </summary>
    private void PickUpDropSystem_OnRemovePlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Orange);
    }
    /// <summary>
    /// ������ �������� � ����� 
    /// </summary>
    private void OnAddPlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }
    /// <summary>
    /// �������� ������
    /// </summary>
    private void UpdateVisual()
    {
        for (int i = 0; i < _gridSystemXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // � ������
                {
                    if (!_equipmentGridVisualSingleArray[i][x, y].GetIsBusy()) // ���� ������� �� ������
                    {
                        _equipmentGridVisualSingleArray[i][x, y].Show(GetGridVisualTypeMaterial(GridVisualType.Grey));
                    }
                }
            }
        }
    }
    /// <summary>
    /// ���������� ��������� ���������
    /// </summary>
    private void SetDefoltState()
    {
        for (int index = 0; index < _gridSystemXYList.Count; index++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // � ������
                {
                    _equipmentGridVisualSingleArray[index][x, y].SetIsBusyAndMaterial(false, GetGridVisualTypeMaterial(GridVisualType.Grey));
                }
            }
        }
    }
    /// <summary>
    /// ���������� ��������� � ��������
    /// </summary>
    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY = placedObject.GetGridSystemXY(); // �������� ������� ������� �������� ������
        List<Vector2Int> OccupiesGridPositionList = placedObject.GetOccupiesGridPositionList(); // ������ ���������� �������� �������
        EquipmentSlot equipmentSlot = gridSystemXY.GetGridSlot(); // ���� �����

        switch (equipmentSlot)
        {
            case EquipmentSlot.BagSlot:
                int index = _gridNameIndexDictionary[equipmentSlot]; //������ �� ������� ������ ����� � _equipmentGridVisualSingleArray

                foreach (Vector2Int gridPosition in OccupiesGridPositionList) // ��������� ����������� �������� ������� �����
                {
                    //��������� ��������� ����� (� ���� isBusy = false(������ ��������) �� ��������� ���������� ��������)
                    _equipmentGridVisualSingleArray[index][gridPosition.x, gridPosition.y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                }
                break;

            //������, �������� � ���. ����� ������, ����� ������ ����� � ���� �����, �.�. ��� ����� ����������� ������ ���� �������
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:

                index = _gridNameIndexDictionary[equipmentSlot]; //������ �� ������� ������ ����� � _equipmentGridVisualSingleArray

                for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // ��� ���������� ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // � ������
                    {
                        //��������� ��������� ����� (� ���� isBusy = false(������ ��������) �� ��������� ���������� ��������)
                        _equipmentGridVisualSingleArray[index][x, y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                    }
                }

                break;
        }

    }

    /// <summary>
    /// �������� ��������� �������� �������
    /// </summary>
    private void ShowPossibleGridPositions(EquipmentSlot equipmentSlot, PlacedObject placedObject, Vector2Int gridPositioAnchor, GridVisualType gridVisualType)
    {
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY = _equipmentGrid.GetActiveGridSystemXY(equipmentSlot); // ������� ����� ��� ������� �����
        switch (equipmentSlot)
        {
            case EquipmentSlot.BagSlot:

                int index = _gridNameIndexDictionary[equipmentSlot]; //������ �� ������� ������ ����� � _equipmentGridVisualSingleArray                
                List<Vector2Int> TryOccupiesGridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositioAnchor); // ������ �������� ������� ������� ����� ������
                foreach (Vector2Int gridPosition in TryOccupiesGridPositionList) // ��������� ������ ������� ������� ���� ������
                {
                    if (gridSystemXY.IsValidGridPosition(gridPosition)) // ���� ������� ��������� ��...
                    {
                        if (!_equipmentGridVisualSingleArray[index][gridPosition.x, gridPosition.y].GetIsBusy()) // ���� ������� �� ������
                        {
                            _equipmentGridVisualSingleArray[index][gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
                        }
                    }
                }
                break;
            //����� ������ ��� �������� � ���. ����� ������, ����� ���������� ��� �� ����� ������ ����� ��� �����, ���� ���� �� �������� � ����� ������
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:

                index = _gridNameIndexDictionary[equipmentSlot]; //������ �� ������� ������ ����� � _equipmentGridVisualSingleArray

                for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // ��� ���������� ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // � ������
                    {
                        if (_equipmentGridVisualSingleArray[index][x, y].GetIsBusy()) // ���� ������ ���� ������� ������ �� ������� �� ����� � �����. ��� ����
                        {
                            break;
                        }
                        _equipmentGridVisualSingleArray[index][x, y].Show(GetGridVisualTypeMaterial(gridVisualType));

                    }
                }
                break;
        }
    }
    /// <summary>
    /// (������� �������� � ����������� �� ���������) �������� ��� ��������� ��� �������� ������������ � ����������� �� ����������� � �������� ��������� �������� ������������
    /// </summary>
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList) // � ����� ��������� ������ ��� ��������� ����������� ��������� ����� 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // ����  ��������� �����(gridVisualType) ��������� � ���������� ��� ��������� �� ..
            {
                return gridVisualTypeMaterial.materialGrid; // ������ �������� ��������������� ������� ��������� �����
            }
        }

        Debug.LogError("�� ���� ����� GridVisualTypeColor ��� GridVisualType " + gridVisualType); // ���� �� ������ ����������� ������ ������
        return null;
    }

    private void OnDestroy()
    {
        _unitEquipmentSystem = null; // ������� ������ ����� ����� ���������
    }
}
