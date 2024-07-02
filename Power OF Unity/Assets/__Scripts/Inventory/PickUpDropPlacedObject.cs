
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �������� �������� � ���������� ��������(� �����).
/// </summary>
/// <remarks>
/// �� ����� ����������, � PlacedObject ���������� ����� ���������� � ������� ����� �������� �� �����.
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour // 
{
    private enum TypeCanvasRenderMode
    {
        ScreenSpaceOverlay,
        ScreenSpaceCamera,
        WorldSpace
    }

    public event EventHandler OnGrabbedObjectGridExits; // ���������� ������ ������� �����
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // ������ �������� � ����� ����������
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������   
    public event EventHandler<PlacedObjectParameters> OnGrabbedObjectGridPositionChanged; // ������� ������������ ������� �� ����� ���������� 

    private LayerMask _inventoryLayerMask; // ��� ��������� ��������� ���� ��� Inventory // �������� �� ������� ��� ���� ��������� 
    private Canvas _canvasInventory;
    private Camera _cameraInventoryUI;
    private RenderMode _canvasRenderMode;
    private PlacedObject _placedObject; // ����������� ������    
    private Vector3 _offset; // �������� �� ����� 
    private Plane _planeForCanvasInWorldSpace; // ���������(�� ������� ����� ���������� ����������� �������) ��� ������� � ������� ������������    
    private Vector2Int _mouseGridPosition;  // �������� ������� ����

    /// <summary>
    /// �������� ������� (���������� ������ ������� �����), ����� �� ��������� ������� ������ ���� ������ �������������
    /// </summary>
    private bool _startEventOnGrabbedObjectGridExits = false;

    private GameInput _gameInput;
    private TooltipUI _tooltipUI;
    private InventoryGrid _inventoryGrid;


    private void Awake()
    {
        _canvasInventory = GetComponentInParent<Canvas>();
        _cameraInventoryUI = GetComponentInParent<Camera>();
    }

    public void Init(GameInput gameInput, TooltipUI tooltipUI, InventoryGrid inventoryGrid)
    {
        _gameInput = gameInput;
        _tooltipUI = tooltipUI;
        _inventoryGrid = inventoryGrid;

        Setup();
    }

    private void Setup()
    {
        _canvasRenderMode = _canvasInventory.renderMode;
        if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ ��
        {
            _planeForCanvasInWorldSpace = new Plane(_canvasInventory.transform.forward, _canvasInventory.transform.position); // �������� ��������� � ������� canvasInventory
        }
        _inventoryLayerMask = LayerMask.GetMask("Inventory");

        _gameInput.OnClickAction += GameInput_OnClickAction;
    }

    private void GameInput_OnClickAction(object sender, EventArgs e) // ���� ��� ������ 
    {
        // ��������� ��������� ��������� �����
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = null;
        Vector2Int gridPositionMouse = new Vector2Int(0, 0);
        bool inGrid =false; 
        switch (_canvasRenderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                inGrid = _inventoryGrid.TryGetGridSystemGridPosition(_gameInput.GetMouseScreenPosition(), _canvasRenderMode, _cameraInventoryUI,out gridSystemXY,out gridPositionMouse);
                break;
            case RenderMode.ScreenSpaceCamera:
                inGrid = _inventoryGrid.TryGetGridSystemGridPosition(_gameInput.GetMouseScreenPosition(), _canvasRenderMode, _cameraInventoryUI, out gridSystemXY, out gridPositionMouse);
                break;
            case RenderMode.WorldSpace:
                inGrid = _inventoryGrid.TryGetGridSystemGridPosition(GetMousePositionOnPlane(), _canvasRenderMode, _cameraInventoryUI, out gridSystemXY, out gridPositionMouse);
                break;
        }
        // ����� � �������� ������ ����� ������ � �����, �������� ���           
        if (inGrid) // ���� ��� ������ �� 
        {
            if (_placedObject == null) // �� ���� ��� ���� �������� ��������, ����������� ��������
            {
                TryGrab();
            }
            else // ���������� ������� ������ �� �����
            {
                TryDrop(gridSystemXY, gridPositionMouse, _placedObject);
            }
        }
        else //���� �� ��� ������ 
        {
            if (_placedObject != null) // ���� ���� ���������� ������ 
            {
                _placedObject.Drop();
                _placedObject.SetMoveStartPosition(true); //�� ��������� ������ � ��������� ���������, ���������� � ������� ��� � ������ �������
                ResetPlacedObject(); // ������� ������ ����������� ������
            }
        }
    }

    private void Update()
    {
        if (_placedObject != null) // ���� ���� ����������� ������ ����� ��� ���������� �� ���������� ���� �� ��������� ���������
        {
            SetTargetPosition();
        }
    }
    /// <summary>
    /// ��������� �������� �������
    /// </summary>
    public void TryGrab()
    {
        Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _inventoryLayerMask)) // ������ true ���� ������� � ���������.
        {
            _placedObject = raycastHit.transform.GetComponentInParent<PlacedObject>();
            if (_placedObject != null) // ���� � �������� ������� � ������� ������ ���� PlacedObject �� ����� ��� �������� (�� ������ ����� ������ ������ � ��� ��� ��� �������)
            {
                _placedObject.Grab(); // ������� ���
                _inventoryGrid.RemovePlacedObjectAtGrid(_placedObject);// ������ �� ������� �������� �������               

                // ���� ��������
                OnRemovePlacedObjectAtInventoryGrid?.Invoke(this, _placedObject); // �������� �������
            }
        }
    }
    /// <summary>
    /// ��������� �������� ����������� �������
    /// </summary>
    public bool TryDrop(GridSystemXY<GridObjectInventoryXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // ��������� �������� � ���������� �� �����       
        InventorySlot gridName = gridSystemXY.GetGridSlot(); // ������� ��� �����

        if (!placedObject.GetCanPlacedOnGridList().Contains(gridName)) //���� ����� ����� ��� � ������ ����� ��� ����� ���������� ��� ������ ��
        {
            _tooltipUI.ShowTooltipsFollowMouse("�������� ������ ����", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

            // ���� �������
            return false;
        }

        bool drop = false;
        switch (gridName)
        {
            case InventorySlot.BagSlot:

                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;

            // ��� ����� ��������� � ���. ������ ��������� mouseGridPosition (0,0)
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponsSlot:

                gridPositionMouse = new Vector2Int(0, 0);
                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;
        }
        return drop;
    }
    /// <summary>
    /// �������� �������� ����������� ������ � ������� �����
    /// </summary>   
    private bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectInventoryXY> gridSystemXY)
    {
        bool drop = false;
        if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {
            placedObject.Drop();  // �������
            placedObject.SetGridPositionAnchor(gridPositionMouse); // ��������� ����� �������� ������� �����
            placedObject.SetGridSystemXY(gridSystemXY); //��������� ����� �� ������� �������� ��� �������

            // ���� �������� ����������
            OnAddPlacedObjectAtInventoryGrid?.Invoke(this, placedObject); // �������� ������� (�������� ����� � �� � InventoryGrid �.�. placedObject ��� ���� ��������� ����� ����)
            ResetPlacedObject(); // ������� ������ ����������� ������

            drop = true;
        }
        else
        {
            _tooltipUI.ShowTooltipsFollowMouse("�� ������� ����������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

            // ���� �������
            drop = false;
        }
        return drop;
    }

    private void SetTargetPosition()
    {        
        Vector2Int zeroGridPosition = new Vector2Int(0, 0); // ������� ������� �����

        // ��������� ��������� ��������� �����
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = null;
        Vector3 mousePosition = new Vector3(0, 0, 0);
        Vector2Int mouseGridPosition = new Vector2Int(0, 0);
        bool inGrid = false;
        switch (_canvasRenderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                mousePosition = _gameInput.GetMouseScreenPosition();
                inGrid = _inventoryGrid.TryGetGridSystemGridPosition(mousePosition, _canvasRenderMode, _cameraInventoryUI, out gridSystemXY, out mouseGridPosition);
                break;
            case RenderMode.ScreenSpaceCamera:
                mousePosition = _gameInput.GetMouseScreenPosition();
                inGrid = _inventoryGrid.TryGetGridSystemGridPosition(mousePosition, _canvasRenderMode, _cameraInventoryUI, out gridSystemXY, out mouseGridPosition);
                break;
            case RenderMode.WorldSpace:
                mousePosition = GetMousePositionOnPlane();
                inGrid = _inventoryGrid.TryGetGridSystemGridPosition(mousePosition, _canvasRenderMode, _cameraInventoryUI, out gridSystemXY, out mouseGridPosition);
                break;
        }
        if (inGrid) // ���� ��� ������ �� ��������� �������� ��
        {
            InventorySlot inventorySlot = gridSystemXY.GetGridSlot(); // ������� ���� ���������� � ���������
            switch (inventorySlot)
            {
                case InventorySlot.BagSlot:
                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionLowerLeft�ornerCell(mouseGridPosition, gridSystemXY));
                    _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));

                    if (_mouseGridPosition != mouseGridPosition || _mouseGridPosition == zeroGridPosition) // ���� �������� ������� �� ����� ���������� ��� ����� ������� ������� �� ...
                    {
                        //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� ��������� ������������ �������
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectParameters
                        {
                            slot = inventorySlot,
                            gridPositioAnchor = mouseGridPosition,
                            placedObject = _placedObject,
                        });

                        _mouseGridPosition = mouseGridPosition; // ��������� ���������� ������� �� �����
                    }
                    break;

                // ��� ����� ��������� � ���. ������ ��������� TargetPosition � ������ �����
                case InventorySlot.MainWeaponSlot:
                case InventorySlot.OtherWeaponsSlot:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //����� ������ ��� �� ������ ����� ���� ������� �������� ������� ������������ ��������
                    _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));

                    //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� ��������� ������������ �������
                    OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectParameters
                    {
                        slot = inventorySlot,
                        gridPositioAnchor = zeroGridPosition,
                        placedObject = _placedObject,
                    });
                    break;
            }
            _startEventOnGrabbedObjectGridExits = false; // ������� ��������
        }
        else // ���� �� ��� ������ �� ������ ������� �� �����
        {
            _placedObject.SetTargetPosition(mousePosition - _offset);// ����� ������ ��� �� ������ ����� ���� ������� �������� ������� ������������ ��������
            _placedObject.SetTargetRotation(Vector3.zero);

            _mouseGridPosition = zeroGridPosition; //������� �������� ������� - ����� ������ �������� ����� (����� ������ ����������� ���������, ��� ����� ���� ,� �� �� ������� ����� � ������� �����))

            if (!_startEventOnGrabbedObjectGridExits) // ���� �� �������� ������� �� �������� ���
            {
                OnGrabbedObjectGridExits?.Invoke(this, EventArgs.Empty);
                _startEventOnGrabbedObjectGridExits = true;
            }
        }
    }

    public void CreatePlacedObject(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO) //�������� ����������� ������ ��� ������� �� ������(� ��������� �������� ������� � ��� �������) 
    {
        // ����� �������� ������� ��������       
        if (_placedObject != null) // ���� ���� ���������� ������ 
        {
            //������� ����������� ������
            _placedObject.Drop();
            _placedObject.SetMoveStartPosition(true);
            ResetPlacedObject(); // ������� ������ ����������� ������ (_placedObject = null)
        }
        else
        {
            _placedObject = PlacedObject.CreateInWorld(worldPosition, placedObjectTypeSO, _canvasInventory.transform); // �������� ������
            _placedObject.Grab(); // ������� ���
            _offset = _placedObject.GetOffsetVisualFromParent(); // ����� ������ ��� �� ������ ����� ���� ������� �������� ������� ������������ ��������          
        }
    }

    public Vector3 GetMousePositionOnPlane() // �������� ������� ���� �� ���������
    {
        Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition());//���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
        return ray.GetPoint(planeDistance); // ������� ����� �� ���� ��� ��� ��������� ���������
    }

   

    private void ResetPlacedObject() // ������� ������ ����������� ������
    {
        _placedObject = null;
    }


    /*  public Vector3 CalculateOffsetGrab() // �������� �������� �������
      {
          Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
          _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
          return _placedObject.transform.position - ray.GetPoint(planeDistance); // �������� �������� �� ������ ������� � ������  pivot �� �������.        
      }

      public Vector3 GetMousePosition(LayerMask layerMask) // �������� ������� ���� (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������) // ��� ����������� ����
      {
          Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
          Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
          return raycastHit.point; // ���� ��� ������� � �������� �� Physics.Raycast ����� true, � raycastHit.point ������ "����� ����� � ������� ������������, ��� ��� ����� � ���������", � ���� false �� ����� ������� ����������� ������ ������ ��������(� ����� ������ ������ ������� ������).
      }

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int mouseGridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) // �������� ��������������� ������� ��������� ���� (��� ������)
      {
          Vector2Int rotationOffset = _placedObject.GetRotationOffset(_dir); // �������� ������� ���� �� ��������
          Vector3 placedObjectWorldPosition = InventoryGrid.Instance.GetWorldPositionCenter�ornerCell(mouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * InventoryGrid.Instance.GetCellSize();
          return placedObjectWorldPosition; // ������ ��������������� ��������� � ����� �����
      }

      public Quaternion GetPlacedObjectRotation() // ������� �������� ������������ �������
      {
          if (_placedObject != null)
          {
              return Quaternion.Euler(0, 0, _placedObject.GetRotationAngle(_dir));
          }
          else
          {
              return Quaternion.identity;
          }
      }*/

}
