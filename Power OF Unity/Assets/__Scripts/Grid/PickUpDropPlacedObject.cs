using System;
using UnityEngine;

/// <summary>
/// �������� �������� � ���������� ��������(� �����). ����� ���� ����� ���� �������������� � EquipmentGrid.<br/>
/// ����� ������ ������� �� ��������� � ������������� ��������� ����� ��������� � ����������� �����
/// </summary>
/// <remarks>
/// �� ����� ����������, � PlacedObject ���������� ����� ���������� � ������� ����� �������� �� �����.<br/>
/// �������� � ���� � ������� ItemSelectButtonsSystemUI
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour, IToggleActivity
{
    public event EventHandler OnGrabbedObjectGridExits; // ���������� ������ ������� �����   
    public event EventHandler<PlacedObjectGridParameters> OnGrabbedObjectGridPositionChanged; // ������� ������������ ������� �� ����� ���������� 

    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtEquipmentSystem; // ������ ������ �� ������� ����������   

    private LayerMask _equipmentLayerMask; // ��� ���������� ��������� ���� ��� Equipment // �������� �� ������� ��� ���� ��������� 
    private Canvas _canvasPickUpDrop;
    private Camera _camera;
    private RenderMode _canvasRenderMode;
    private PlacedObject _placedObject; // ����������� ������
    /// <summary>
    /// ����������� ������  ��� ������� ����� ����
    /// </summary>
    private PlacedObject _placedObjectMouseEnter;
    private Vector3 _offset; //�������� ������ ������� ������������ �����
    private Plane _planeForCanvasInWorldSpace; // ���������(�� ������� ����� ���������� ����������� �������) ��� ������� � ������� ������������    
    private Vector2Int _mouseGridPosition;  // �������� ������� ����

    /// <summary>
    /// �������� ������� (���������� ������ ������� �����), ����� �� ��������� ������� ������ ���� ������ �������������
    /// </summary>
    private bool _startEventOnGrabbedObjectGridExits = false;
    private bool _isActive = true;

    private GameInput _gameInput;
    private TooltipUI _tooltipUI;
    private EquipmentGrid _equipmentGrid;
    private UnitEquipmentSystem _unitEquipmentSystem;
    private UnitManager _unitManager;
    private Unit _selectedUnit;

    public void Init(GameInput gameInput, TooltipUI tooltipUI, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem, UnitManager unitManager)
    {
        _gameInput = gameInput;
        _tooltipUI = tooltipUI;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;
        _unitManager = unitManager;



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
    }

    public void SetActive(bool active)
    {
        if (_isActive == active) //���� ���������� ��������� ���� �� �������
            return;

        _isActive = active;

        _canvasPickUpDrop.enabled = active;
        if (active)
        {
            _gameInput.OnClickAction += GameInput_OnClickAction;
            _gameInput.OnClickRemoveAction += GameInput_OnClickRemoveAction;
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        }
        else
        {
            _gameInput.OnClickAction -= GameInput_OnClickAction;
            _gameInput.OnClickRemoveAction -= GameInput_OnClickRemoveAction;
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        }

    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        _selectedUnit = newSelectedUnit;
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
            mousePosition = _gameInput.GetMouseScreenPoint();
        else
            mousePosition = GetMousePositionOnPlane();


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
        else //���� �� ��� ������ (��� ��������) if(�� ��� �������� �������� ���������)  
        {

            if (_placedObject != null) // ���� ���� ���������� ������ 
            {
                DropAddPlacedObjectInResources();
            }
        }
    }

    private void GameInput_OnClickRemoveAction(object sender, EventArgs e)
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
            mousePosition = _gameInput.GetMouseScreenPoint();
        else
            mousePosition = GetMousePositionOnPlane();

        // ��������� �������� ��� ������ � ���-�����          
        if (_equipmentGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out mouseGridPosition)) // ���� ��� ������ �� 
        {
            if (_placedObject == null) // �� ���� ��� ���� �������� ��������
            {
                // ���� ��� �������� ���� ������� �� ������� �� ����� � ������� � �������
                if (_placedObjectMouseEnter != null)
                {
                    _unitEquipmentSystem.RemoveFromGridAndUnitEquipmentWithCheck(_placedObjectMouseEnter, returnInResourcesAndStartPosition: true);
                    // ���� ��������            
                }

            }
            else // ���� ���� ���������� ������ 
            {
                // ������� � ������ � ������� 
                DropAddPlacedObjectInResources();
                OnGrabbedObjectGridExits?.Invoke(this, EventArgs.Empty); // ��� �� �������� �������������� �������������� ����� ������
            }
        }
        else //���� �� ��� ������  (��������� ��������� ��� ������� ����)
        {
            if (_placedObject != null) // ���� ���� ���������� ������ 
            {
                // ������� � ������ � ������� ������� ������� ������
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
                case EquipmentSlot.HeadArmorSlot:
                case EquipmentSlot.BodyArmorSlot:

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
    /// ��������� �������� ������� (� ����� ���������)
    /// </summary>
    private void TryGrab()
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
                    _unitEquipmentSystem.RemoveFromGridAndUnitEquipmentWithCheck(_placedObject);
                    // ���� ��������                    
                }
            }
        }
        else
        {
            if (_placedObjectMouseEnter != null)
            {
                _placedObject = _placedObjectMouseEnter;
                _placedObject.Grab(); // ������� ���
                _offset = _placedObject.GetOffsetCenterFromAnchor() * _canvasPickUpDrop.scaleFactor;
                _unitEquipmentSystem.RemoveFromGridAndUnitEquipmentWithCheck(_placedObject);
                // ���� ��������            
            }
        }
    }


    /// <summary>
    /// C������� � ������� ����������� ������ � ������� �������
    /// </summary>
    private void DropAddPlacedObjectInResources()
    {
        _placedObject.Drop();
        _unitEquipmentSystem.ReturnPlacedObjectInResourcesAndStartPosition(_placedObject);
        ResetPlacedObject();
    }


    /// <summary>
    /// ��������� �������� ����������� �������
    /// </summary>
    private bool TryDrop(GridSystemXY<GridObjectEquipmentXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // ��������� �������� � ���������� �� �����       
        EquipmentSlot slotName = gridSystemXY.GetGridSlot(); // ������� ��� �����

        if (!placedObject.CanPlaceOnSlot(slotName)) //���� ������ ����� ��� � ������ ��� ����� ���������� ��� ������ ��
        {
            _tooltipUI.ShowShortTooltipFollowMouse("�������� ������ ����", new TooltipUI.TooltipTimer { timer = 2f });
            // ���� �������
            return false;
        }

        // �������� �� ������������� � ������� ���������� ����������
        if (!_unitEquipmentSystem.CompatibleWithOtherEquipment(placedObject))
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������������� ����������", new TooltipUI.TooltipTimer { timer = 0.8f });
            // ���� �������
            return false;
        }

        // ��� ����� ������� ����� ������� ������ ���� ������� ��������� gridPositionMouse = (0,0)
        switch (slotName)
        {
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:
            case EquipmentSlot.BodyArmorSlot:
            case EquipmentSlot.HeadArmorSlot:
                gridPositionMouse = new Vector2Int(0, 0);
                _unitEquipmentSystem.CleanSlotFromAnotherPlacedObject(slotName);
                break;
        }
        return TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
    }
    /// <summary>
    /// �������� �������� ����������� ������ � ������� �����
    /// </summary>   
    private bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectEquipmentXY> gridSystemXY)
    {
        bool drop = false;
        if (_equipmentGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {
            _unitEquipmentSystem.AddPlacedObjectAtUnitEquipment(_placedObject);
            ResetPlacedObject(); // ������� ������ ����������� ������
            // ���� �������� ����������           
            drop = true;
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse("�� ������� ����������", new TooltipUI.TooltipTimer { timer = 0.8f }); // ������� ��������� � ������� ����� ������ ����������� ���������
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
            //������� ����������� ������. ������� �� ������ ������ (���� ��� ����������� �.�. ������� �������������� ����� GameInput_OnClickAction ��� � ���������� ����������� ��������)
            DropAddPlacedObjectInResources();
        }
        else
        {
            _offset = placedObjectTypeSO.GetOffsetVisual�enterFromAnchor() * _canvasPickUpDrop.scaleFactor; // ����� ������ ��� �� �������� ����� ���� ������� �������� ������ ������� ������������ ����� � ������ ������� �������         
            _placedObject = PlacedObject.CreateInWorld(worldPosition - _offset, placedObjectTypeSO, _canvasPickUpDrop.transform, this); // �������� ������
            _placedObject.SetDropPositionWhenDeleted(worldPosition - _offset);
            _placedObject.Grab(); // ������� ���
        }
    }

    /// <summary>
    /// �������� ������� ���� �� ���������
    /// </summary>
    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint());//���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
        return ray.GetPoint(planeDistance); // ������� ����� �� ���� ��� ��� ��������� ���������
    }


    /// <summary>
    /// ������� ������ ����������� ������
    /// </summary>
    private void ResetPlacedObject()
    {
        _placedObject = null;
    }
    /// <summary>
    /// ���������� PlacedObject ��� ������� ����� ����
    /// </summary>
    public void SetPlacedObjectMouseEnter(PlacedObject placedObject)
    {
        _placedObjectMouseEnter = placedObject;
    }

    public Canvas GetCanvas() { return _canvasPickUpDrop; }

    /*  public Vector3 CalculateOffsetGrab() // �������� �������� �������
      {
          Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //���������� ���, ������ �� ������ ����� ����� ������ ��� ���������� ������ ���� 
          _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // ��������� ��� � ��������� � ������� ���������� ����� ����, ��� �� ���������� ���������.
          return _placedObject.transform.gridPosition - ray.GetPoint(planeDistance); // �������� �������� �� ������ ������� � ������  pivot �� �������.        
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
