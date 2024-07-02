using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ����� ���������. � ������ ������� ��������� GridObjectInventoryXY ������� ������ ������ � ����������� �� ����� �������
/// </summary>
/// <remarks>
/// �������������� ��������� ������ � InventoryGrid, ���������� ����� PickUpDropPlacedObject. 
/// </remarks>
public class InventoryGrid : MonoBehaviour
{

    private static float cellSize;  // ������ ������

    [SerializeField] private InventoryGridParameters[] _gridParametersArray; // ������ ���������� ����� ������ � ����������
    [SerializeField] private Transform _gridDebugObjectPrefab; // ������ ������� ����� 

    private TooltipUI _tooltipUI;
    private PickUpDropPlacedObject _pickUpDrop;
    private Transform _canvasInventoryWorld;
    private List<PlacedObject> _placedObjectList = new List<PlacedObject>(); // ������ ����������� ��������    
    private List<GridSystemXY<GridObjectInventoryXY>> _gridSystemXYList; //������ �������� ������ .� �������� ������� ��� GridObjectInventoryXY    



    private void Awake()
    {
        _gridSystemXYList = new List<GridSystemXY<GridObjectInventoryXY>>(); // �������������� ������              

        foreach (InventoryGridParameters gridParameters in _gridParametersArray)
        {
            GridSystemXY<GridObjectInventoryXY> gridSystem = new GridSystemXY<GridObjectInventoryXY>(gridParameters,   // �������� �����  � � ������ ������ �������� ������ ���� GridObjectInventoryXY
                (GridSystemXY<GridObjectInventoryXY> g, Vector2Int gridPosition) => new GridObjectInventoryXY(g, gridPosition)); //� ��������� ��������� ��������� ������� ������� �������� ����� ������ => new GridObjectUnitXZ(g, _gridPositioAnchor) � ��������� �� ��������. (������ ��������� ����� ������� � ��������� �����)

            //gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // �������� ��� ������ � ������ ������
            _gridSystemXYList.Add(gridSystem); // ������� � ������ ��������� �����          
        }

        _canvasInventoryWorld = GetComponentInParent<Canvas>().transform;
        cellSize = _gridSystemXYList[0].GetCellSize(); // ��� ���� ����� ������� ������ ����������  
    }

    public void Init(PickUpDropPlacedObject pickUpDrop, TooltipUI tooltipUI)
    {
        _pickUpDrop = pickUpDrop;
        _tooltipUI = tooltipUI;

        Setup();
    }

    private void Setup()
    {
        // ������� ������ � ������� ��������� ������� ����

        //_background.localPosition = new Vector3(_widthBag / 2f, _heightBag / 2f, 0) * _cellSize; // ��������� ������ ��� ����������
        // _background.localScale = new Vector3(_widthBag, _heightBag, 0) * _cellSize;
        //_background.GetComponent<Material>().mainTextureScale = new Vector2(_widthBag, _heightBag);// ������� ���������� ������ ������ ���������� ����� �� ������ � ������   

        /* foreach (InventoryGridParameters gridParameters in _gridParametersArray)
         {
             RectTransform bagBackground = (RectTransform)gridParameters.anchorGridTransform.GetChild(0); //������� ������ ��� �����
             bagBackground.sizeDelta = new Vector2(gridParameters.width, gridParameters.height);
             bagBackground.localScale = Vector3.one * gridParameters.cellSize;
             bagBackground.GetComponent<RawImage>().uvRect = new Rect(0, 0, gridParameters.width, gridParameters.height);   // ������� ���������� ������ ������ ���������� ����� �� ������ � ������   
         }*/
    }

    /// <summary>
    /// ���������, ��� �������� ������� ����, ������� �������� ������� �� ������� � � ������ ����� ������ �� � �������� �������
    /// </summary>   
    public bool TryGetGridSystemGridPosition(Vector3 mousePosition, RenderMode canvasRenderMode, Camera camera, out GridSystemXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int mouseGridPosition)
    {
        gridSystemXY = null;
        mouseGridPosition = new Vector2Int(0, 0);

        switch (canvasRenderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                foreach (InventoryGridParameters inventoryGridParameter in _gridParametersArray)
                {
                    Vector2 mouseLocalPosition;
                    RectTransform rectTransformGrid = (RectTransform)inventoryGridParameter.anchorGridTransform;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformGrid, mousePosition, null, out mouseLocalPosition)) // ���� �� ������ �� ��������� �����, �� ������ ��������� ������� ���� �� ���� ����������
                    {
                        gridSystemXY = GetGridSystemXY(inventoryGridParameter.slot);
                        mouseGridPosition = gridSystemXY.GetGridPosition(mouseLocalPosition);
                    }
                }
                break;

            case RenderMode.ScreenSpaceCamera:
                foreach (InventoryGridParameters inventoryGridParameter in _gridParametersArray)
                {
                    Vector2 localPositionMouse;
                    RectTransform rectTransformGrid = (RectTransform)inventoryGridParameter.anchorGridTransform;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformGrid, mousePosition, camera, out localPositionMouse)) // ���� �� ������ �� ��������� �����, �� ������ ��������� ������� ���� �� ���� ����������
                    {
                        gridSystemXY = GetGridSystemXY(inventoryGridParameter.slot);
                        mouseGridPosition = gridSystemXY.GetGridPosition(localPositionMouse);
                    }
                }
                break;

            case RenderMode.WorldSpace:
                foreach (GridSystemXY<GridObjectInventoryXY> localGridSystem in _gridSystemXYList) // �������� ������ �����
                {
                    Vector2Int testGridPositionMouse = localGridSystem.GetGridPosition(mousePosition); //��� ������ localGridSystem ������� �������� ������� �����

                    if (localGridSystem.IsValidGridPosition(testGridPositionMouse)) // ���� ����������� �������� ������� ���������, ������ �� � ���� �������� �������
                    {
                        gridSystemXY = localGridSystem;
                        mouseGridPosition = testGridPositionMouse;
                        break;
                    }
                }
                break;
        }

        if (gridSystemXY == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }   

    /// <summary>
    /// �������� �������� ����������� ������ � ������� �����
    /// </summary>   
    /// <remarks>����� false ���� - ���������� ������. ���� ���� �� �������  ����������� ������ � GridObjectInventoryXY</remarks>
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionPlace, PlacedObject placedObject, GridSystemXY<GridObjectInventoryXY> gridSystemXY)
    {
        List<Vector2Int> gridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositionPlace); // ������� ������ �������� ������� ������� ����� ������ ������
        bool canPlace = true;
        foreach (Vector2Int gridPosition in gridPositionList) // ������� ���� ��������� ������ ������
        {
            if (!gridSystemXY.IsValidGridPosition(gridPosition)) // ���� ���� ���� ���� �� ���������� ������� �� 
            {
                canPlace = false; // ���������� ������
                break;
            }
            if (gridSystemXY.GetGridObject(gridPosition).HasPlacedObject()) // ���� ���� �� ����� ������� ��� ���� ����������� ������ ��
            {
                canPlace = false; // ���������� ������
                break; //break �������� ���� ���������, continue ������ ��������� ������� ��������.
            }
        }

        if (canPlace)//���� ����� ���������� �� �����
        {
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // ������� GridObjectInventoryXY ������� ��������� � gridPosition
                gridObject.AddPlacedObject(placedObject); // �������� ����������� ������ 
            }
            _placedObjectList.Add(placedObject);
        }
        return canPlace;
    }
    /// <summary>
    /// �������� ����������� ������ �� ����� (�������������� ��� �� ��� �������� � �����)
    /// </summary>
    public void RemovePlacedObjectAtGrid(PlacedObject placedObject)
    {
        List<Vector2Int> gridPositionList = placedObject.GetOccupiesGridPositionList(); // ������� ������ �������� ������� ������� �������� ������
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = placedObject.GetGridSystemXY(); // ������� ����� �� ������� �� �������� 
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // ������� GridObjectInventoryXY ��� ����� � ������� �� ���������
            gridObject.RemovePlacedObject(placedObject); // ������ ����������� ������ 
        }
        _placedObjectList.Remove(placedObject);
    }

    /// <summary>
    /// �������� ��������� � ������� ����������� ��������
    /// </summary>
    public void ClearInventoryGridAndDestroyPlacedObjects()
    {
        foreach (PlacedObject placedObject in _placedObjectList)
        {
            RemovePlacedObjectAtGrid(placedObject);
            Destroy(placedObject.gameObject);
        }
    }

    public List<GridSystemXY<GridObjectInventoryXY>> GetGridSystemXYList() { return _gridSystemXYList; }

    // �������� �������� �������
    public Vector2Int GetGridPosition(Vector3 worldPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetGridPosition(worldPosition);

    // ������� ������� ���������� ������ ������ (������������  ����� ***GridTransform)
    public Vector3 GetWorldPositionCenter�ornerCell(Vector2Int gridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionCenter�ornerCell(gridPosition);

    //  ������� ������� ���������� ������ ����� (������������  ����� ***GridTransform)
    public Vector3 GetWorldPositionGridCenter(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionGridCenter();

    /// <summary>
    /// ������� ������� ���������� ������� ������ ����� ������ (������������  ����� ***GridTransform)
    /// </summary>   
    /// <returns></returns>
    public Vector3 GetWorldPositionLowerLeft�ornerCell(Vector2Int gridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionLowerLeft�ornerCell(gridPosition);

    public Vector3 GetRotationAnchorGrid(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetRotationAnchorGrid();
    public Transform GetAnchorGrid(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetAnchorGrid();
    public int GetWidth(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWidth();
    public int GetHeight(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetHeight();
    public static float GetCellSize()// (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������)
    {
        return cellSize;
    }

    public GridSystemXY<GridObjectInventoryXY> GetGridSystemXY(InventorySlot inventorySlot) // �������� ����� ��� ����� �����
    {
        foreach (GridSystemXY<GridObjectInventoryXY> localGridSystem in _gridSystemXYList) // �������� ������ �����
        {
            if (localGridSystem.GetGridSlot() == inventorySlot)
            {
                return localGridSystem;
            }
        }

        return null;
    }

    /*
        public void Save()
        {
            _placedObjectList = new List<PlacedObjectParameters>();
            for (int i = 0; i < _gridSystemXYList.Count; i++) // ��������� ��� �����
            {
                for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
                {
                    for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // � ������
                    {
                        Vector2Int gridPosition = new Vector2Int(x, y);
                        GridObjectInventoryXY gridObject = _gridSystemXYList[i].GetGridObject(gridPosition); // ������� �������� ������ ��� ����� �������

                        if (gridObject.HasPlacedObject()) // ���� � ���� ������� ���� ����������� ������ �� 
                        {
                            PlacedObjectParameters placedObjectParameters = new PlacedObjectParameters(
                                slot = _gridSystemXYList[i]
                                );
                            if (!_placedObjectList.Contains(gridObject.GetPlacedObject())) // ���� ���� ������ ��� �� ����������� � ������ �� ������� ��� (PlacedObject ����� ������������ ����������� �� ���������� GridObject, ������� ����� ��������)
                            {
                                _placedObjectList.Add(gridObject.GetPlacedObject());
                            }
                        }
                    }
                }
            }

            // �������� ������ ����������� ����������� ��������
            List<PlacedObjectParameters> addPlacedObjectList = new List<PlacedObjectParameters>();
            foreach (PlacedObjectParameters placedObject in placedObjectList)
            {
                addPlacedObjectList.Add(new AddPlacedObject
                {
                    gridName = placedObject.GetGridSystemXY().GetGridSlot(),
                    gridPositioAnchor = placedObject.GetGridPositionAnchor(),
                    placedObject = placedObject.GetPlacedObjectTypeSO(),
                });
            }

            string json = JsonUtility.ToJson(new ListAddPlacedObject { addPlacedObjectList = addPlacedObjectList });

            PlayerPrefs.SetString("InventoryGridSystemSave", json); // ������������� ������������ ��������� �������� ��� ������������, ������������� ������ ������. �� ������ ������������ PlayerPrefs.getString ��� ��������� ����� �������
            SaveSystem.Save("InventoryGridSystemSave", json, true); // �������� � ��������� ���������

            Debug.Log("Save!");
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey("InventoryGridSystemSave")) // ���������� true, ���� �������� �������� key ���������� � PlayerPrefs, � ��������� ������ ���������� false.
            {
                string json = PlayerPrefs.GetString("InventoryGridSystemSave");
                json = SaveSystem.Load("InventoryGridSystemSave");

                ListAddPlacedObject listAddPlacedObject = JsonUtility.FromJson<ListAddPlacedObject>(json); // �������� ������ ����������� ����������� ��������

                foreach (AddPlacedObject addPlacedObject in listAddPlacedObject.addPlacedObjectList) // ��������� ������ ����������� ����������� ������ � ������
                {
                    // �������� � ��������� ����������� ������               
                    GridSystemXY<GridObjectInventoryXY> gridSystemXY = GetGridSystemXY(addPlacedObject.gridName); // ������� ����� ��� ����������
                    PlacedObject placedObject = PlacedObject.CreateInGrid(gridSystemXY, addPlacedObject.gridPositioAnchor, addPlacedObject.placedObject, _canvasInventory, this);
                    if (!_pickUpDropPlacedObject.TryDrop(gridSystemXY, addPlacedObject.gridPositioAnchor, placedObject)) // ���� �� ������� �������� ������ �� ����� ��
                    {
                        placedObject.DestroySelf(); // ��������� ���� ������
                        _tooltipUI.ShowTooltipsFollowMouse("�� ������� ��������� ����������", new TooltipUI.TooltipTimer { timer = 3f }); // ������� ��������� � ������� ����� ������ ����������� ���������                   
                    }
                }
            }
            Debug.Log("Load!");
        }*/


    /*public InventoryGridParameters[] GetGridParametersArray() //�������� ������ ���������� �����
    {
        return _gridParametersArray;
    }

    public PlacedObject GetPlacedObjectAtGridPosition(Vector2Int gridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) // �������� ����������� ������ � ���� �������� �������
    {
        GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // ������� GridObjectInventoryXY ������� ��������� � gridPositionPlace
        return gridObject.GetPlacedObject();
    }*/
}
