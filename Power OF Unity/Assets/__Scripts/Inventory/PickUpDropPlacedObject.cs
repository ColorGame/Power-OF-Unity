using System;
using UnityEngine;

/// <summary>
/// �������� �������� � ���������� ��������(� �����). ����� ���� ����� ���� �������������� � InventoryGrid
/// </summary>
/// <remarks>
/// �� ����� ����������, � PlacedObject ���������� ����� ���������� � ������� ����� �������� �� �����.
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour // 
{
    public event EventHandler OnGrabbedObjectGridExits; // ���������� ������ ������� �����
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // ������ �������� � ����� ����������
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtInventoryGrid; // ������ ������ �� ����� ����������   
    public event EventHandler<PlacedObjectParameters> OnGrabbedObjectGridPositionChanged; // ������� ������������ ������� �� ����� ���������� 

    private LayerMask _inventoryLayerMask; // ��� ��������� ��������� ���� ��� Inventory // �������� �� ������� ��� ���� ��������� 
    private Canvas _canvasInventory;
    private Camera _camera;
    private RenderMode _canvasRenderMode;
    private PlacedObject _placedObject; // ����������� ������    
    private PlacedObject _placedObjectMouseEnter; // ����������� ������  ��� ������� ����� ����
    private Vector3 _offset; //�������� ������ ������� ������������ �����
    private Plane _planeForCanvasInWorldSpace; // ���������(�� ������� ����� ���������� ����������� �������) ��� ������� � ������� ������������    
    private Vector2Int _mouseGridPosition;  // �������� ������� ����

    /// <summary>
    /// �������� ������� (���������� ������ ������� �����), ����� �� ��������� ������� ������ ���� ������ �������������
    /// </summary>
    private bool _startEventOnGrabbedObjectGridExits = false;

    private GameInput _gameInput;
    private TooltipUI _tooltipUI;
    private InventoryGrid _inventoryGrid;

    public void Init(GameInput gameInput, TooltipUI tooltipUI, InventoryGrid inventoryGrid)
    {
        _gameInput = gameInput;
        _tooltipUI = tooltipUI;
        _inventoryGrid = inventoryGrid;

        Setup();
    }

    private void Setup()
    {
        _canvasInventory = GetComponentInParent<Canvas>();
        _canvasRenderMode = _canvasInventory.renderMode;
        if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ ��
        {
            _planeForCanvasInWorldSpace = new Plane(_canvasInventory.transform.forward, _canvasInventory.transform.position); // �������� ��������� � ������� canvasInventory
            _camera = GetComponentInParent<Camera>(); // ��� ������� � ������� ������������ ����� ������������ �������������� ������
        }
        else
        {
            _camera = Camera.main;
        }
        _inventoryLayerMask = LayerMask.GetMask("Inventory");

        _gameInput.OnClickAction += GameInput_OnClickAction;
    }

    private void GameInput_OnClickAction(object sender, EventArgs e) // ���� ��� ������ 
    {
        // ��������� ��������� ��������� �����
        GridSystemXY<GridObjectInventoryXY> gridSystemXY;
        Vector2Int mouseGridPosition;
        Vector3 mousePosition;

        if (_canvasRenderMode != RenderMode.WorldSpace)// ���� ������ �� � ������� ������������ ��
        {
            mousePosition = _gameInput.GetMouseScreenPoint();
        }
        else
        {
            mousePosition = GetMousePositionOnPlane();
        }

        // ����� � �������� ������ ����� ������ � �����, �������� ���           
        if (_inventoryGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out mouseGridPosition)) // ���� ��� ������ �� 
        {
            if (_placedObject == null) // �� ���� ��� ���� �������� ��������, ����������� ��������
            {
                TryGrab();
            }
            else // ���������� ������� ������ �� �����
            {
                TryDrop(gridSystemXY, mouseGridPosition, _placedObject);
            }
        }
        else //���� �� ��� ������ 
        {
            if (_placedObject != null) // ���� ���� ���������� ������ 
            {
                _placedObject.Drop();
                _placedObject.SetFlagMoveStartPosition(true); //�� ��������� ������ � ��������� ���������, ���������� � ������� ��� � ������ �������
                ResetPlacedObject(); // ������� ������ ����������� ������
            }
        }
    }

    private void Update()
    {
        if (_placedObject != null) // ���� ���� ����������� ������ ����� ��� ���������� �� ���������� ���� �� Canvas �� ��������� ���������
        {
            SetTargetPosition();
        }
    }
    /// <summary>
    /// ��������� �������� �������
    /// </summary>
    public void TryGrab()
    {
        if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ ��
        {
            Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
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
        else
        {
            if (_placedObjectMouseEnter != null)
            {
                _placedObject = _placedObjectMouseEnter;
                _placedObject.Grab(); // ������� ���
                _inventoryGrid.RemovePlacedObjectAtGrid(_placedObject);// ������ �� ������� �������� �������               
                _offset = _placedObject.GetOffsetVisualFromParent() * _canvasInventory.scaleFactor;
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
            _tooltipUI.ShowShortTooltipFollowMouse("�������� ������ ����", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

            // ���� �������
            return false;
        }

        bool drop = false;
        switch (gridName)
        {
            case InventorySlot.BagSlot:

                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;

            // ��� ����� ��������� � ���. ������ ��������� newMouseGridPosition (0,0)
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
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectInventoryXY> gridSystemXY)
    {
        bool drop = false;
        if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {

            // ���� �������� ����������
            OnAddPlacedObjectAtInventoryGrid?.Invoke(this, placedObject); // �������� ������� (�������� ����� � �� � InventoryGrid �.�. placedObject ��� ���� ��������� ����� ����)
            ResetPlacedObject(); // ������� ������ ����������� ������

            drop = true;
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse("�� ������� ����������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

            // ���� �������
            drop = false;
        }
        return drop;
    }

    private void SetTargetPosition()
    {
        Vector2Int zeroGridPosition = Vector2Int.zero; // ������� ������� �����                
        GridSystemXY<GridObjectInventoryXY> gridSystemXY;
        Vector2Int newMouseGridPosition;
        Vector3 mousePosition;

        if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ ��
        {
            mousePosition = GetMousePositionOnPlane();
        }
        else
        {
            mousePosition = _gameInput.GetMouseScreenPoint();
        }

        if (_inventoryGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out newMouseGridPosition)) // ���� ��� ������ �� ��������� �������� ��
        {
            InventorySlot inventorySlot = gridSystemXY.GetGridSlot(); // ������� ���� ���������� � ���������
            switch (inventorySlot)
            {
                case InventorySlot.BagSlot:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionLowerLeft�ornerCell(newMouseGridPosition, gridSystemXY));
                    if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ �� ����� � ��� �������
                    {
                        _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));
                    }

                    if (_mouseGridPosition != newMouseGridPosition || _mouseGridPosition == zeroGridPosition) // ���� �������� ������� �� ����� ���������� ��� ����� ������� ������� �� ...
                    {
                        //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� ��������� ������������ �������                     
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectParameters
                        {
                            slot = inventorySlot,
                            gridPositioAnchor = newMouseGridPosition,
                            placedObject = _placedObject,
                        });

                        _mouseGridPosition = newMouseGridPosition; // ��������� ���������� ������� �� �����
                    }
                    break;

                // ��� ����� ��������� � ���. ������ ��������� TargetPosition � ������ �����
                case InventorySlot.MainWeaponSlot:
                case InventorySlot.OtherWeaponsSlot:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //����� ������ ��� �� �������� ����� ���� ������� �������� ������ ������� ������������ �����
                    if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ �� ����� � ��� �������
                    {
                        _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));
                    }
                    else
                    {
                        _placedObject.SetTargetRotation(Vector3.zero);
                    }

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
            _placedObject.SetTargetPosition(mousePosition - _offset);//����� ������ ��� �� �������� ����� ���� ������� �������� ������ ������� ������������ �����
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
            _placedObject.SetFlagMoveStartPosition(true);
            ResetPlacedObject(); // ������� ������ ����������� ������ (_placedObject = null)
        }
        else
        {
            _offset = placedObjectTypeSO.GetOffsetVisual�enterFromAnchor() * _canvasInventory.scaleFactor; // ����� ������ ��� �� �������� ����� ���� ������� �������� ������ ������� ������������ ����� � ������ ������� �������         
            _placedObject = PlacedObject.CreateInWorld(worldPosition - _offset, placedObjectTypeSO, _canvasInventory.transform, this); // �������� ������
            _placedObject.Grab(); // ������� ���
        }
    }

    public Vector3 GetMousePositionOnPlane() // �������� ������� ���� �� ���������
    {
        Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint());//���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
        return ray.GetPoint(planeDistance); // ������� ����� �� ���� ��� ��� ��������� ���������
    }

    private void ResetPlacedObject() // ������� ������ ����������� ������
    {
        _placedObject = null;
    }

    public void SetPlacedObjectMouseEnter(PlacedObject placedObject)
    {
        _placedObjectMouseEnter = placedObject;
    }

    public Canvas GetCanvasInventory() { return _canvasInventory; }

    public GridSystemXY<GridObjectInventoryXY> GetGridSystemXY(InventorySlot inventorySlot) => _inventoryGrid.GetGridSystemXY(inventorySlot);


    /*  public Vector3 CalculateOffsetGrab() // �������� �������� �������
      {
          Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
          _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
          return _placedObject.transform.position - ray.GetPoint(planeDistance); // �������� �������� �� ������ ������� � ������  pivot �� �������.        
      }

      public Vector3 GetMousePosition(LayerMask layerMask) // �������� ������� ���� (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������) // ��� ����������� ����
      {
          Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
          Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
          return raycastHit.point; // ���� ��� ������� � �������� �� Physics.Raycast ����� true, � raycastHit.point ������ "����� ����� � ������� ������������, ��� ��� ����� � ���������", � ���� false �� ����� ������� ����������� ������ ������ ��������(� ����� ������ ������ ������� ������).
      }

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int newMouseGridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) // �������� ��������������� ������� ��������� ���� (��� ������)
      {
          Vector2Int rotationOffset = _placedObject.GetRotationOffset(_dir); // �������� ������� ���� �� ��������
          Vector3 placedObjectWorldPosition = InventoryGrid.Instance.GetWorldPositionCenter�ornerCell(newMouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * InventoryGrid.Instance.GetCellSizeWithScaleFactor();
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
