
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PickUpDrop : MonoBehaviour // �������� �������������� � �������� ���������
{

    public event EventHandler OnGrabbedObjectGridExits; // ���������� ������ ������� �����
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtGrid; // ������ �������� � ����� 
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtGrid; // ������ ������ �� �����       
    public event EventHandler<OnGrabbedObjectGridPositionChangedEventArgs> OnGrabbedObjectGridPositionChanged; // ������� ������������ ������� �� ����� ����������
    public class OnGrabbedObjectGridPositionChangedEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� ��������
    {
        public GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY; // �������� ������� ������� ����
        public Vector2Int newMouseGridPosition;  // ����� �������� ������� ����
        public PlacedObject placedObject; // ���������� ������
    }

    private LayerMask _inventoryLayerMask; // ��� ��������� ��������� ���� ��� Inventory // �������� �� ������� ��� ���� ��������� 
    private Transform _canvasInventoryWorld;
    private Camera _cameraInventoryUI;
    private GameInput _gameInput;
    private TooltipUI _tooltipUI;
    private InventoryGrid _inventoryGrid;

    private PlacedObject _placedObject; // ����������� ������    
    private Vector3 _offset; // �������� �� �����
    private PlacedObjectTypeSO _placedObjectTypeSO;
    private Plane _plane; // ��������� �� ������� ����� ���������� ����������� �������    
    private Vector2Int _mouseGridPosition;  // �������� ������� ����
    private bool _startEventOnGrabbedObjectGridExits = false; // �������� ������� (���������� ������ ������� �����), ����� �� ��������� ������� ������ ���� ������ �������������

    public void Initialize(GameInput gameInput, TooltipUI tooltipUI, InventoryGrid inventoryGrid)
    {
        _gameInput = gameInput;
        _tooltipUI = tooltipUI;
        _inventoryGrid = inventoryGrid;
    }

    private void Awake()
    {    
        _canvasInventoryWorld = GetComponentInParent<Canvas>().transform;
        _cameraInventoryUI = GetComponentInParent<Camera>();
    }


    private void Start()
    {
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_cameraInventoryUI); //������� � ���� �������� ������ ���� ������ ���������
        _plane = new Plane(_canvasInventoryWorld.forward, _canvasInventoryWorld.position); // �������� ��������� � ������� canvasInventory
        _inventoryLayerMask = LayerMask.GetMask("Inventory");
        _canvasInventoryWorld.GetComponent<Canvas>().worldCamera = _cameraInventoryUI; // ��������� ��� ������ ������ ������� ����� ��� ���������

        CoreEntryPoint.Instance.gameInput.OnClickAction += GameInput_OnClickAction;
        // _gameInput.OnClickAction += GameInput_OnClickAction;
    }

    private void GameInput_OnClickAction(object sender, EventArgs e) // ���� ��� ������ 
    {
        // ����� � �������� ������ ����� ������ � �����, �������� ���           
        if (_inventoryGrid.TryGetGridSystemGridPosition(GetMousePositionOnPlane(), out GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int gridPositionMouse)) // ���� ��� ������ �� ��������� �������� ��
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
                _placedObjectTypeSO = _placedObject.GetPlacedObjectTypeSO();

                // ���� ��������
                OnRemovePlacedObjectAtGrid?.Invoke(this, _placedObject); // �������� �������
            }
        }
    }

    public bool TryDrop(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // ��������� �������� � ���������� �� �����       
        GridName gridName = gridSystemXY.GetGridName(); // ������� ��� �����

        if (!placedObject.GetCanPlacedOnGridList().Contains(gridName)) //���� ����� ����� ��� � ������ ����� ��� ����� ���������� ��� ������ ��
        {
            _tooltipUI.ShowTooltipsFollowMouse("�������� ������ ����", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

            // ���� �������
            return false;
        }

        bool drop = false;
        switch (gridName)
        {
            case GridName.BagGrid1:
                if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
                {
                    placedObject.Drop();  // �������
                    placedObject.SetGridPositionAnchor(gridPositionMouse); // ��������� ����� �������� ������� �����
                    placedObject.SetGridSystemXY(gridSystemXY); //��������� ����� �� ������� �������� ��� �������

                    // ���� �������� ����������
                    OnAddPlacedObjectAtGrid?.Invoke(this, placedObject); // �������� �������
                    ResetPlacedObject(); // ������� ������ ����������� ������

                    drop = true;
                }
                else
                {
                    _tooltipUI.ShowTooltipsFollowMouse("�� ������� ����������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

                    // ���� �������
                    drop = false;
                }
                break;

            // ��� ����� ��������� � ���. ������ ��������� newMouseGridPosition (0,0)
            case GridName.MainWeaponGrid2:
            case GridName.OtherWeaponGrid3:
                if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(new Vector2Int(0, 0), placedObject, gridSystemXY))
                {
                    placedObject.Drop();  // �������
                    placedObject.SetGridPositionAnchor(new Vector2Int(0, 0)); // ��������� ����� �������� ������� �����
                    placedObject.SetGridSystemXY(gridSystemXY); //��������� ����� �� ������� �������� ��� �������

                    // ���� �������� ����������
                    OnAddPlacedObjectAtGrid?.Invoke(this, placedObject); // �������� �������
                    ResetPlacedObject(); // ������� ������ ����������� ������

                    drop = true;
                }
                else
                {
                    _tooltipUI.ShowTooltipsFollowMouse("�� ������� ����������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

                    // ���� �������
                    drop = false;
                }
                break;
        }
        return drop;
    }

    public void SetTargetPosition()
    {
        Vector3 mousePositionOnPlane = GetMousePositionOnPlane();
        Vector2Int zeroGridPosition = new Vector2Int(0, 0); // ������� ������� �����
        bool tryGetGridSystemGridPosition = _inventoryGrid.TryGetGridSystemGridPosition(mousePositionOnPlane, out GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int newMouseGridPosition);

        if (tryGetGridSystemGridPosition) // ���� ��� ������ �� ��������� �������� ��
        {
            GridName gridName = gridSystemXY.GetGridName(); // ������� ��� �����
            switch (gridName)
            {
                case GridName.BagGrid1:
                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionLowerLeft�ornerCell(newMouseGridPosition, gridSystemXY));
                    _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));

                    if (_mouseGridPosition != newMouseGridPosition || _mouseGridPosition == zeroGridPosition) // ���� �������� ������� �� ����� ���������� ��� ����� ������� ������� �� ...
                    {
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new OnGrabbedObjectGridPositionChangedEventArgs //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� �������� �������
                        {
                            gridSystemXY = gridSystemXY,
                            newMouseGridPosition = newMouseGridPosition,
                            placedObject = _placedObject
                        }); // �������� ������� � ���������

                        _mouseGridPosition = newMouseGridPosition; // ��������� ���������� ������� �� �����
                    }
                    break;

                // ��� ����� ��������� � ���. ������ ��������� TargetPosition � ������ �����
                case GridName.MainWeaponGrid2:
                case GridName.OtherWeaponGrid3:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //����� ������ ��� �� ������ ����� ���� ������� �������� ������� ������������ ��������
                    _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));

                    OnGrabbedObjectGridPositionChanged?.Invoke(this, new OnGrabbedObjectGridPositionChangedEventArgs //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� �������� �������
                    {
                        gridSystemXY = gridSystemXY,
                        newMouseGridPosition = zeroGridPosition,
                        placedObject = _placedObject
                    }); // �������� ������� � ���������                       
                    break;
            }
            _startEventOnGrabbedObjectGridExits = false; // ������� ��������
        }
        else // ���� �� ��� ������ �� ������ ������� �� �����
        {
            _placedObject.SetTargetPosition(mousePositionOnPlane - _offset);// ����� ������ ��� �� ������ ����� ���� ������� �������� ������� ������������ ��������
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
            _placedObject = PlacedObject.CreateInWorld(worldPosition, placedObjectTypeSO, _canvasInventoryWorld); // �������� ������
            _placedObject.Grab(); // ������� ���
            _offset = _placedObject.GetOffsetVisualFromParent(); // ����� ������ ��� �� ������ ����� ���� ������� �������� ������� ������������ ��������
            _placedObjectTypeSO = _placedObject.GetPlacedObjectTypeSO();
        }
    }

    public Vector3 GetMousePositionOnPlane() // �������� ������� ���� �� ���������
    {
        Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition());//���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
        _plane.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
        return ray.GetPoint(planeDistance); // ������� ����� �� ���� ��� ��� ��������� ���������
    }


    private void ResetPlacedObject() // ������� ������ ����������� ������
    {
        _placedObject = null;
    }


    /*  public Vector3 CalculateOffsetGrab() // �������� �������� �������
      {
          Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
          _plane.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
          return _placedObject.transform.position - ray.GetPoint(planeDistance); // �������� �������� �� ������ ������� � ������  pivot �� �������.        
      }

      public Vector3 GetMousePosition(LayerMask layerMask) // �������� ������� ���� (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������) // ��� ����������� ����
      {
          Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
          Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
          return raycastHit.point; // ���� ��� ������� � �������� �� Physics.Raycast ����� true, � raycastHit.point ������ "����� ����� � ������� ������������, ��� ��� ����� � ���������", � ���� false �� ����� ������� ����������� ������ ������ ��������(� ����� ������ ������ ������� ������).
      }

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int mouseGridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) // �������� ��������������� ������� ��������� ���� (��� ������)
      {
          Vector2Int rotationOffset = _placedObjectTypeSO.GetRotationOffset(_dir); // �������� ������� ���� �� ��������
          Vector3 placedObjectWorldPosition = InventoryGrid.Instance.GetWorldPositionCenter�ornerCell(mouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * InventoryGrid.Instance.GetCellSize();
          return placedObjectWorldPosition; // ������ ��������������� ��������� � ����� �����
      }

      public Quaternion GetPlacedObjectRotation() // ������� �������� ������������ �������
      {
          if (_placedObjectTypeSO != null)
          {
              return Quaternion.Euler(0, 0, _placedObjectTypeSO.GetRotationAngle(_dir));
          }
          else
          {
              return Quaternion.identity;
          }
      }*/

}
