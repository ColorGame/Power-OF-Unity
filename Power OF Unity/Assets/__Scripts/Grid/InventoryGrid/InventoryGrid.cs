using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ����� ���������
/// </summary>
/// <remarks>
/// ��������� ����������� ������ � ������� ����� (��������� ����� PickUpDrop � GridObjectInventoryXY)
/// </remarks>
public class InventoryGrid : MonoBehaviour 
{

    private static float cellSize;  // ������ ������

   

    [SerializeField] private GridParameters[] _gridParametersArray; // ������ ���������� ����� ������ � ����������
    [SerializeField] private Transform _gridDebugObjectPrefab; // ������ ������� ����� 

    private TooltipUI _tooltipUI;
    private PickUpDrop _pickUpDrop;
    private Transform _canvasInventoryWorld;

    private List<GridSystemTiltedXY<GridObjectInventoryXY>> _gridSystemTiltedXYList; //������ �������� ������ .� �������� ������� ��� GridObjectInventoryXY    


    public void Initialize(PickUpDrop pickUpDrop, TooltipUI tooltipUI)
    {
        _pickUpDrop = pickUpDrop;
        _tooltipUI = tooltipUI;
    }

    private void Awake()
    {      
        _gridSystemTiltedXYList = new List<GridSystemTiltedXY<GridObjectInventoryXY>>(); // �������������� ������              

        foreach (GridParameters gridParameters in _gridParametersArray)
        {
            GridSystemTiltedXY<GridObjectInventoryXY> gridSystem = new GridSystemTiltedXY<GridObjectInventoryXY>(gridParameters,   // �������� �����  � � ������ ������ �������� ������ ���� GridObjectInventoryXY
                (GridSystemXY<GridObjectInventoryXY> g, Vector2Int gridPosition) => new GridObjectInventoryXY(g, gridPosition)); //� ��������� ��������� ��������� ������� ������� �������� ����� ������ => new GridObjectUnitXZ(g, _gridPositioAnchor) � ��������� �� ��������. (������ ��������� ����� ������� � ��������� �����)

            //gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // �������� ��� ������ � ������ ������
            _gridSystemTiltedXYList.Add(gridSystem); // ������� � ������ ��������� �����
        }

        _canvasInventoryWorld = GetComponentInParent<Canvas>().transform;
        cellSize = _gridSystemTiltedXYList[0].GetCellSize(); // ��� ���� ����� ������� ������ ����������  
    }

    private void Start()
    {
        // ������� ������ � ������� ��������� ������� ����

        //_background.localPosition = new Vector3(_widthBag / 2f, _heightBag / 2f, 0) * _cellSize; // ��������� ������ ��� ����������
        // _background.localScale = new Vector3(_widthBag, _heightBag, 0) * _cellSize;
        //_background.GetComponent<Material>().mainTextureScale = new Vector2(_widthBag, _heightBag);// ������� ���������� ������ ������ ���������� ����� �� ������ � ������   

        /* foreach (GridParameters gridParameters in _gridParametersArray)
         {
             RectTransform bagBackground = (RectTransform)gridParameters.anchorGridTransform.GetChild(0); //������� ������ ��� �����
             bagBackground.sizeDelta = new Vector2(gridParameters.width, gridParameters.height);
             bagBackground.localScale = Vector3.one * gridParameters.cellSize;
             bagBackground.GetComponent<RawImage>().uvRect = new Rect(0, 0, gridParameters.width, gridParameters.height);   // ������� ���������� ������ ������ ���������� ����� �� ������ � ������   
         }*/
    }

    /// <summary>
    /// ��������� ������� �������� ������� ��� �������� ������� ���� � � ������ ����� ������ �� � �������� �������
    /// </summary>   
    public bool TryGetGridSystemGridPosition(Vector3 worldPositionMouse, out GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int gridPositionMouse)
    {
        gridSystemXY = null;
        gridPositionMouse = new Vector2Int(0, 0);

        foreach (GridSystemTiltedXY<GridObjectInventoryXY> localGridSystem in _gridSystemTiltedXYList) // �������� ������ �����
        {
            Vector2Int testGridPositionMouse = localGridSystem.GetGridPosition(worldPositionMouse); //��� ������ localGridSystem ������� �������� ������� �����

            if (localGridSystem.IsValidGridPosition(testGridPositionMouse)) // ���� ����������� �������� ������� ���������, ������ �� � ���� �������� �������
            {
                gridSystemXY = localGridSystem;
                gridPositionMouse = testGridPositionMouse;
                break;
            }
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
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY)
    {
        List<Vector2Int> gridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositionMouse); // ������� ������ �������� ������� ������� ����� ������ ������
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
                break;
            }
        }

        if (canPlace)//���� ����� ���������� �� �����
        {
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // ������� GridObjectInventoryXY ������� ��������� � gridPosition
                gridObject.AddPlacedObject(placedObject); // �������� ����������� ������ 
            }    
        }
        return canPlace;        
    }    

    public void RemovePlacedObjectAtGrid(PlacedObject placedObject) // �������� ����������� ������ �� ����� (�������������� ��� �� ��� �������� � �����)
    {
        List<Vector2Int> gridPositionList = placedObject.GetOccupiesGridPositionList(); // ������� ������ �������� ������� ������� �������� ������
        GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY = placedObject.GetGridSystemXY(); // ������� ����� �� ������� �� �������� 
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // ������� GridObjectInventoryXY ��� ����� � ������� �� ���������
            gridObject.RemovePlacedObject(placedObject); // ������ ����������� ������ 
        }
    }

    public List<GridSystemTiltedXY<GridObjectInventoryXY>> GetGridSystemTiltedXYList()
    {
        return _gridSystemTiltedXYList;
    }

    // �������� �������� �������
    public Vector2Int GetGridPosition(Vector3 worldPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetGridPosition(worldPosition);

    // ������� ������� ���������� ������ ������ (������������  ����� ***GridTransform)
    public Vector3 GetWorldPositionCenter�ornerCell(Vector2Int gridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionCenter�ornerCell(gridPosition);

    //  ������� ������� ���������� ������ ����� (������������  ����� ***GridTransform)
    public Vector3 GetWorldPositionGridCenter(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionGridCenter();

    /// <summary>
    /// ������� ������� ���������� ������� ������ ����� ������ (������������  ����� ***GridTransform)
    /// </summary>   
    /// <returns></returns>
    public Vector3 GetWorldPositionLowerLeft�ornerCell(Vector2Int gridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionLowerLeft�ornerCell(gridPosition);

    public Vector3 GetRotationAnchorGrid(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetRotationAnchorGrid();
    public Transform GetAnchorGrid(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetAnchorGrid();
    public int GetWidth(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWidth();
    public int GetHeight(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetHeight();
    public static float GetCellSize()// (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������)
    {
        return cellSize;
    }

    public GridSystemTiltedXY<GridObjectInventoryXY> GetGridSystemTiltedXY(GridName gridName) // �������� ����� �� �����
    {
        foreach (GridSystemTiltedXY<GridObjectInventoryXY> localGridSystem in _gridSystemTiltedXYList) // �������� ������ �����
        {
            if (localGridSystem.GetGridName() == gridName)
            {
                return localGridSystem;
            }
        }

        return null;
    }


    [Serializable]
    public struct AddPlacedObject // ���������� ����������� ������
    {
        public GridName gridName; //��� ����� �� ������� �������� ����������� ������
        public Vector2Int gridPositioAnchor; // �������� ������� �����
        public PlacedObjectTypeSO placedObjectTypeSO;
    }

    [Serializable]
    public struct ListAddPlacedObject // ������ ����������� ����������� ��������
    {
        public List<AddPlacedObject> addPlacedObjectList;
    }

    public void Save()
    {
        List<PlacedObject> placedObjectList = new List<PlacedObject>();
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // ��������� ��� �����
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // ��� ������ ����� ��������� �����
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // � ������
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    GridObjectInventoryXY gridObject = _gridSystemTiltedXYList[i].GetGridObject(gridPosition); // ������� �������� ������ ��� ����� �������

                    if (gridObject.HasPlacedObject()) // ���� � ���� ������� ���� ����������� ������ �� 
                    {
                        if (!placedObjectList.Contains(gridObject.GetPlacedObject())) // ���� ���� ������ ��� �� ����������� � ������ �� ������� ��� (PlacedObject ����� ������������ ����������� �� ���������� GridObject, ������� ����� ��������)
                        {
                            placedObjectList.Add(gridObject.GetPlacedObject());
                        }
                    }
                }
            }
        }

        // �������� ������ ����������� ����������� ��������
        List<AddPlacedObject> addPlacedObjectList = new List<AddPlacedObject>();
        foreach (PlacedObject placedObject in placedObjectList)
        {
            addPlacedObjectList.Add(new AddPlacedObject
            {
                gridName = placedObject.GetGridSystemXY().GetGridName(),
                gridPositioAnchor = placedObject.GetGridPositionAnchor(),
                placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO(),
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
                GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY = GetGridSystemTiltedXY(addPlacedObject.gridName); // ������� ����� ��� ����������
                PlacedObject placedObject = PlacedObject.CreateInGrid(gridSystemXY, addPlacedObject.gridPositioAnchor, addPlacedObject.placedObjectTypeSO, _canvasInventoryWorld , this);
                if (!_pickUpDrop.TryDrop(gridSystemXY, addPlacedObject.gridPositioAnchor, placedObject)) // ���� �� ������� �������� ������ �� ����� ��
                {
                    placedObject.DestroySelf(); // ��������� ���� ������
                    _tooltipUI.ShowTooltipsFollowMouse("�� ������� ��������� ����������", new TooltipUI.TooltipTimer { timer = 3f }); // ������� ��������� � ������� ����� ������ ����������� ���������                   
                }
            }
        }
        Debug.Log("Load!");
    }


    /*public GridParameters[] GetGridParametersArray() //�������� ������ ���������� �����
    {
        return _gridParametersArray;
    }

    public PlacedObject GetPlacedObjectAtGridPosition(Vector2Int gridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) // �������� ����������� ������ � ���� �������� �������
    {
        GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // ������� GridObjectInventoryXY ������� ��������� � gridPositionMouse
        return gridObject.GetPlacedObject();
    }*/
}
