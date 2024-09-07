using System;
using UnityEngine;

/// <summary>
/// �������� �������� � ���������� ��������(� �����). ����� ���� ����� ���� �������������� � EquipmentGrid
/// </summary>
/// <remarks>
/// �� ����� ����������, � PlacedObject ���������� ����� ���������� � ������� ����� �������� �� �����.
/// �������� � ���� � ������� ItemSelectButtonsSystemUI
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour, IToggleActivity
{
    public event EventHandler OnGrabbedObjectGridExits; // ���������� ������ ������� �����
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtEquipmentGrid; // ������ �������� � ����� ����������
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtEquipmentGrid; // ������ ������ �� ����� ����������   
    public event EventHandler<PlacedObjectGridParameters> OnGrabbedObjectGridPositionChanged; // ������� ������������ ������� �� ����� ���������� 

    private LayerMask _equipmentLayerMask; // ��� ���������� ��������� ���� ��� Equipment // �������� �� ������� ��� ���� ��������� 
    private Canvas _canvasPickUpDrop;
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
    private EquipmentGrid _equipmentGrid;
    private UnitManager _unitManager;
    private Unit _selectedUnit;
    private WarehouseManager _warehouseManager;

    public void Init(GameInput gameInput, TooltipUI tooltipUI, EquipmentGrid equipmentGrid, UnitManager unitManager, WarehouseManager resourcesManager)
    {
        _gameInput = gameInput;
        _tooltipUI = tooltipUI;
        _equipmentGrid = equipmentGrid;
        _unitManager = unitManager;
        _warehouseManager = resourcesManager;


        Setup();
    }

    private void Setup()
    {
        _canvasPickUpDrop = GetComponent<Canvas>();
        _canvasRenderMode = _canvasPickUpDrop.renderMode;

        switch (_canvasRenderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                _camera = Camera.main;
                break;
            case RenderMode.ScreenSpaceCamera:
                _camera = GetComponentInParent<Camera>(); // ��� ������� ����� ������������ �������������� ������
                break;
            case RenderMode.WorldSpace:
                _planeForCanvasInWorldSpace = new Plane(_canvasPickUpDrop.transform.forward, _canvasPickUpDrop.transform.position); // �������� ��������� � ������� canvasEquipment
                _camera = GetComponentInParent<Camera>(); // ��� ������� � ������� ������������ ����� ������������ �������������� ������
                break;
        }

        _equipmentLayerMask = LayerMask.GetMask("Equipment");

        _selectedUnit = _unitManager.GetSelectedUnit();

        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        _selectedUnit = newSelectedUnit;
    }

    public void SetActive(bool active)
    {
        _canvasPickUpDrop.enabled = active;
        if (active)
        {
            _gameInput.OnClickAction += GameInput_OnClickAction;
        }
        else
        {
            _gameInput.OnClickAction -= GameInput_OnClickAction;
        }
    }


    private void GameInput_OnClickAction(object sender, EventArgs e) // ���� ��� ������ 
    {

        if (_selectedUnit == null) // ���� ��� ���������� ����� �� � ������ ����������� ����������
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������ �����", new TooltipUI.TooltipTimer { timer = 0.5f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            return; // ������� �����. ��� ����
        }

        // ��������� ��������� ��������� �����
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY;
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
        if (_equipmentGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out mouseGridPosition)) // ���� ��� ������ �� 
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
                DropAddPlacedObjectInResources();
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

    private void SetTargetPosition()
    {
        Vector2Int zeroGridPosition = Vector2Int.zero; // ������� ������� �����                
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY;
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

        if (_equipmentGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out newMouseGridPosition)) // ���� ��� ������ �� ��������� �������� ��
        {
            EquipmentSlot equipmentSlot = gridSystemXY.GetGridSlot(); // ������� ���� ���������� � ����������
            switch (equipmentSlot)
            {
                case EquipmentSlot.BagSlot:

                    _placedObject.SetTargetPosition(_equipmentGrid.GetWorldPositionLowerLeft�ornerCell(newMouseGridPosition, gridSystemXY));
                    if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ �� ����� � ��� �������
                    {
                        _placedObject.SetTargetRotation(_equipmentGrid.GetRotationAnchorGrid(gridSystemXY));
                    }

                    if (_mouseGridPosition != newMouseGridPosition || _mouseGridPosition == zeroGridPosition) // ���� �������� ������� �� ����� ���������� ��� ����� ������� ������� �� ...
                    {
                        //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� ��������� ������������ �������                     
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectGridParameters
                        {
                            slot = equipmentSlot,
                            gridPositioAnchor = newMouseGridPosition,
                            placedObject = _placedObject,
                        });

                        _mouseGridPosition = newMouseGridPosition; // ��������� ���������� ������� �� �����
                    }
                    break;

                // ��� ����� ��������� � ���. ������ ��������� TargetPosition � ������ �����
                case EquipmentSlot.MainWeaponSlot:
                case EquipmentSlot.OtherWeaponsSlot:
                case EquipmentSlot.ArmorHeadSlot:
                case EquipmentSlot.ArmorBodySlot:

                    _placedObject.SetTargetPosition(_equipmentGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //����� ������ ��� �� �������� ����� ���� ������� �������� ������ ������� ������������ �����
                    if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ �� ����� � ��� �������
                    {
                        _placedObject.SetTargetRotation(_equipmentGrid.GetRotationAnchorGrid(gridSystemXY));
                    }
                    else
                    {
                        _placedObject.SetTargetRotation(Vector3.zero);
                    }

                    //�������� - ������� ������� ���� �� ����� ���������� � ��������� ����� ��������� ������������ �������
                    OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectGridParameters
                    {
                        slot = equipmentSlot,
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

    /// <summary>
    /// ��������� �������� �������
    /// </summary>
    public void TryGrab()
    {
        if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ ��
        {
            Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _equipmentLayerMask)) // ������ true ���� ������� � ���������.
            {
                _placedObject = raycastHit.transform.GetComponentInParent<PlacedObject>();
                if (_placedObject != null) // ���� � �������� ������� � ������� ������ ���� PlacedObject �� ����� ��� �������� (�� ������ ����� ������ ������ � ��� ��� ��� �������)
                {
                    _placedObject.Grab(); // ������� ���
                    _equipmentGrid.RemovePlacedObjectAtGrid(_placedObject);// ������ �� ������� �������� �������               

                    // ���� ��������
                    OnRemovePlacedObjectAtEquipmentGrid?.Invoke(this, _placedObject); // �������� �������
                }
            }
        }
        else
        {
            if (_placedObjectMouseEnter != null)
            {
                _placedObject = _placedObjectMouseEnter;
                _placedObject.Grab(); // ������� ���
                _equipmentGrid.RemovePlacedObjectAtGrid(_placedObject);// ������ �� ������� �������� �������               
                _offset = _placedObject.GetOffsetCenterFromAnchor() * _canvasPickUpDrop.scaleFactor;
                // ���� ��������
                OnRemovePlacedObjectAtEquipmentGrid?.Invoke(this, _placedObject); // �������� �������
            }
        }
    }
    /// <summary>
    /// ��������� �������� ����������� �������
    /// </summary>
    public bool TryDrop(GridSystemXY<GridObjectEquipmentXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // ��������� �������� � ���������� �� �����       
        EquipmentSlot gridName = gridSystemXY.GetGridSlot(); // ������� ��� �����

        if (!placedObject.GetCanPlacedOnGridList().Contains(gridName)) //���� ����� ����� ��� � ������ ����� ��� ����� ���������� ��� ������ ��
        {
            _tooltipUI.ShowShortTooltipFollowMouse("�������� ������ ����", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������

            // ���� �������
            return false;
        }

        bool drop = false;
        switch (gridName)
        {
            case EquipmentSlot.BagSlot:

                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;

            // ��� ����� ��������� � ���. ������ ��������� newMouseGridPosition (0,0)
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:
            case EquipmentSlot.ArmorHeadSlot:
            case EquipmentSlot.ArmorBodySlot:               

                gridPositionMouse = new Vector2Int(0, 0);
                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;
        }
        return drop;
    }
    /// <summary>
    /// �������� �������� ����������� ������ � ������� �����
    /// </summary>   
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectEquipmentXY> gridSystemXY)
    {
        bool drop = false;
        if (_equipmentGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {
            // ���� �������� ����������
            OnAddPlacedObjectAtEquipmentGrid?.Invoke(this, placedObject); // �������� ������� (�������� ����� � �� � EquipmentGrid �.�. placedObject ��� ���� ��������� ����� ����)
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

    /// <summary>
    /// �������� ����������� ������ ��� ������� �� ������(� ��������� �������� ������� � ��� �������) 
    /// </summary>
    public void CreatePlacedObject(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO)
    {
        // ����� �������� ������� ��������    
        if (_selectedUnit == null) // ���� ��� ���������� ����� �� � ������� ����������� ����������
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������ �����", new TooltipUI.TooltipTimer { timer = 0.5f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            return; // ������� �����. ��� ����
        }


        if (_placedObject != null) // ���� ���� ���������� ������ 
        {
            //������� ����������� ������
            DropAddPlacedObjectInResources();
        }
        else
        {
            _offset = placedObjectTypeSO.GetOffsetVisual�enterFromAnchor() * _canvasPickUpDrop.scaleFactor; // ����� ������ ��� �� �������� ����� ���� ������� �������� ������ ������� ������������ ����� � ������ ������� �������         
            _placedObject = PlacedObject.CreateInWorld(worldPosition - _offset, placedObjectTypeSO, _canvasPickUpDrop.transform, this); // �������� ������
            _placedObject.Grab(); // ������� ���
        }
    }

    /// <summary>
    /// �������� ������� ���� �� ���������
    /// </summary>
    public Vector3 GetMousePositionOnPlane()
    {
        Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint());//���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
        return ray.GetPoint(planeDistance); // ������� ����� �� ���� ��� ��� ��������� ���������
    }
    /// <summary>
    /// C������� � ������� ����������� ������ � ������� �������
    /// </summary>
    private void DropAddPlacedObjectInResources()
    {
        _warehouseManager.AddCountPlacedObject(_placedObject.GetPlacedObjectTypeSO());
        _placedObject.Drop();
        _placedObject.SetFlagMoveStartPosition(true);
        ResetPlacedObject();
    }
    /// <summary>
    /// ������� ������ ����������� ������
    /// </summary>
    private void ResetPlacedObject()
    {
        _placedObject = null;
    }

    public void SetPlacedObjectMouseEnter(PlacedObject placedObject)
    {
        _placedObjectMouseEnter = placedObject;
    }

    public Canvas GetCanvas() { return _canvasPickUpDrop; }


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

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int newMouseGridPosition, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) // �������� ��������������� ������� ��������� ���� (��� ������)
      {
          Vector2Int rotationOffset = _placedObject.GetRotationOffset(_dir); // �������� ������� ���� �� ��������
          Vector3 placedObjectWorldPosition = EquipmentGrid.Instance.GetWorldPositionCenter�ornerCell(newMouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * EquipmentGrid.Instance.GetCellSizeWithScaleFactor();
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

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        _gameInput.OnClickAction -= GameInput_OnClickAction;
    }
}
