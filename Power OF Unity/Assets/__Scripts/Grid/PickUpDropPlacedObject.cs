using System;
using UnityEngine;

/// <summary>
/// Поднятие Бросание и Размещение объектов(в сетке). Через этот класс идет взаимодействие с EquipmentGrid
/// </summary>
/// <remarks>
/// Во время размещения, в PlacedObject передается сетка размещения и позиция якоря предмета на сетки.
/// Работает в паре с классом ItemSelectButtonsSystemUI
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour, IToggleActivity
{
    public event EventHandler OnGrabbedObjectGridExits; // Захваченый объект покинул сетку
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtEquipmentGrid; // Объект добавлен в сетку Интвенторя
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtEquipmentGrid; // Объект удален из сетки Интвенторя   
    public event EventHandler<PlacedObjectGridParameters> OnGrabbedObjectGridPositionChanged; // позиция захваченного объекта на сетке изменилась 

    private LayerMask _equipmentLayerMask; // Для экипировки настроить слой как Equipment // Настроим на объекте где есть коллайдер 
    private Canvas _canvasPickUpDrop;
    private Camera _camera;
    private RenderMode _canvasRenderMode;
    private PlacedObject _placedObject; // Размещенный объект    
    private PlacedObject _placedObjectMouseEnter; // Размещенный объект  над которым вошла мышь
    private Vector3 _offset; //смещение центра визуала относительно якоря
    private Plane _planeForCanvasInWorldSpace; // плоскость(по которой будем перемещять захваченные объекты) для канваса в мировом пространстве    
    private Vector2Int _mouseGridPosition;  // сеточная позиция мыши

    /// <summary>
    /// Запущено событие (Захваченый объект покинул сетку), чтобы не запускать событие каждый кадр сделал переключатель
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
                _camera = GetComponentInParent<Camera>(); // Для канваса будем использовать дополнительную камеру
                break;
            case RenderMode.WorldSpace:
                _planeForCanvasInWorldSpace = new Plane(_canvasPickUpDrop.transform.forward, _canvasPickUpDrop.transform.position); // Создадим плоскость в позиции canvasEquipment
                _camera = GetComponentInParent<Camera>(); // Для канваса в мировом пространстве будем использовать дополнительную камеру
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


    private void GameInput_OnClickAction(object sender, EventArgs e) // Если мыш нажата 
    {

        if (_selectedUnit == null) // Если нет выбранного юнита то и некому настраивать экипировка
        {
            _tooltipUI.ShowShortTooltipFollowMouse("ВЫБЕРИ ЮНИТА", new TooltipUI.TooltipTimer { timer = 0.5f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            return; // Выходим игнор. код ниже
        }

        // Установим дефолтное состояние полей
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY;
        Vector2Int mouseGridPosition;
        Vector3 mousePosition;

        if (_canvasRenderMode != RenderMode.WorldSpace)// Если канвас НЕ в мировом пространстве то
        {
            mousePosition = _gameInput.GetMouseScreenPoint();
        }
        else
        {
            mousePosition = GetMousePositionOnPlane();
        }

        // Взять и положить объект можно только в сетке, проверим это           
        if (_equipmentGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out mouseGridPosition)) // Если над сеткой то 
        {
            if (_placedObject == null) // Не имея при себе никакого предмета, попытайтесь схватить
            {
                TryGrab();
            }
            else // Попытаемся сбросим объект на сетку
            {
                TryDrop(gridSystemXY, mouseGridPosition, _placedObject);
            }
        }
        else //Если НЕ над сеткой 
        {
            if (_placedObject != null) // Если Есть захваченый объект 
            {
                DropAddPlacedObjectInResources();
            }
        }
    }

    private void Update()
    {
        if (_placedObject != null) // Если есть захваченный объект будем его перемещать за указателем мыши по Canvas ил созданной плоскости
        {
            SetTargetPosition();
        }
    }

    private void SetTargetPosition()
    {
        Vector2Int zeroGridPosition = Vector2Int.zero; // Нулевая позиция сетки                
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY;
        Vector2Int newMouseGridPosition;
        Vector3 mousePosition;

        if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то
        {
            mousePosition = GetMousePositionOnPlane();
        }
        else
        {
            mousePosition = _gameInput.GetMouseScreenPoint();
        }

        if (_equipmentGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out newMouseGridPosition)) // Если над сеткой то попробуем получить ее
        {
            EquipmentSlot equipmentSlot = gridSystemXY.GetGridSlot(); // Получим слот размещения в экипировке
            switch (equipmentSlot)
            {
                case EquipmentSlot.BagSlot:

                    _placedObject.SetTargetPosition(_equipmentGrid.GetWorldPositionLowerLeftСornerCell(newMouseGridPosition, gridSystemXY));
                    if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то учтем и его поворот
                    {
                        _placedObject.SetTargetRotation(_equipmentGrid.GetRotationAnchorGrid(gridSystemXY));
                    }

                    if (_mouseGridPosition != newMouseGridPosition || _mouseGridPosition == zeroGridPosition) // Если сеточная позиция не равна предыдущей или равна нулевой позиции то ...
                    {
                        //запустим - Событие позиция мыши на сетке изменилось и передадим новою параметры размещаемого объекта                     
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectGridParameters
                        {
                            slot = equipmentSlot,
                            gridPositioAnchor = newMouseGridPosition,
                            placedObject = _placedObject,
                        });

                        _mouseGridPosition = newMouseGridPosition; // Перепишем предыдущую позицию на новую
                    }
                    break;

                // для сетки Основного и Доп. оружия установим TargetPosition в центре сетки
                case EquipmentSlot.MainWeaponSlot:
                case EquipmentSlot.OtherWeaponsSlot:
                case EquipmentSlot.ArmorHeadSlot:
                case EquipmentSlot.ArmorBodySlot:

                    _placedObject.SetTargetPosition(_equipmentGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //Чтобы объект был по середине сетки надо вычесть смещение центра визуала относительно якоря
                    if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то учтем и его поворот
                    {
                        _placedObject.SetTargetRotation(_equipmentGrid.GetRotationAnchorGrid(gridSystemXY));
                    }
                    else
                    {
                        _placedObject.SetTargetRotation(Vector3.zero);
                    }

                    //запустим - Событие позиция мыши на сетке изменилось и передадим новою параметры размещаемого объекта
                    OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectGridParameters
                    {
                        slot = equipmentSlot,
                        gridPositioAnchor = zeroGridPosition,
                        placedObject = _placedObject,
                    });
                    break;
            }
            _startEventOnGrabbedObjectGridExits = false; // Сбросим параметр
        }
        else // Если не над сеткой то просто следуем за мышью
        {
            _placedObject.SetTargetPosition(mousePosition - _offset);//Чтобы объект был по середине мышки надо вычесть смещение центра визуала относительно якоря
            _placedObject.SetTargetRotation(Vector3.zero);


            _mouseGridPosition = zeroGridPosition; //Сбросим сеточную позицию - Когда объект покидает сетку (чтобы визуал отображался корректно, при вводе мыши ,в ту же область сетки с который вышла))

            if (!_startEventOnGrabbedObjectGridExits) // Если не запущено событие то запустим его
            {
                OnGrabbedObjectGridExits?.Invoke(this, EventArgs.Empty);
                _startEventOnGrabbedObjectGridExits = true;
            }
        }
    }

    /// <summary>
    /// Попробуем СХВАТИТЬ предмет
    /// </summary>
    public void TryGrab()
    {
        if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то
        {
            Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _equipmentLayerMask)) // Вернет true если попадет в инвертарь.
            {
                _placedObject = raycastHit.transform.GetComponentInParent<PlacedObject>();
                if (_placedObject != null) // Если у родителя объекта в который попали есть PlacedObject то можно его схватить (на кнопка висит просто визуал и там нет род объекта)
                {
                    _placedObject.Grab(); // Схватим его
                    _equipmentGrid.RemovePlacedObjectAtGrid(_placedObject);// Удалим из текущей сеточной позиции               

                    // Звук поднятия
                    OnRemovePlacedObjectAtEquipmentGrid?.Invoke(this, _placedObject); // Запустим событие
                }
            }
        }
        else
        {
            if (_placedObjectMouseEnter != null)
            {
                _placedObject = _placedObjectMouseEnter;
                _placedObject.Grab(); // Схватим его
                _equipmentGrid.RemovePlacedObjectAtGrid(_placedObject);// Удалим из текущей сеточной позиции               
                _offset = _placedObject.GetOffsetCenterFromAnchor() * _canvasPickUpDrop.scaleFactor;
                // Звук поднятия
                OnRemovePlacedObjectAtEquipmentGrid?.Invoke(this, _placedObject); // Запустим событие
            }
        }
    }
    /// <summary>
    /// Попробуем СБРОСИТЬ захваченный предмет
    /// </summary>
    public bool TryDrop(GridSystemXY<GridObjectEquipmentXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // Попробуем сбросить и разместить на сетке       
        EquipmentSlot gridName = gridSystemXY.GetGridSlot(); // Получим имя сетки

        if (!placedObject.GetCanPlacedOnGridList().Contains(gridName)) //Если нашей сетки НЕТ в списке сеток где можно разместить наш объект то
        {
            _tooltipUI.ShowShortTooltipFollowMouse("попробуй другой слот", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки

            // Звук неудачи
            return false;
        }

        bool drop = false;
        switch (gridName)
        {
            case EquipmentSlot.BagSlot:

                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;

            // для сетки Основного и Доп. оружия установим newMouseGridPosition (0,0)
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
    /// Попробую Добавить Размещаемый объект в Позицию Сетки
    /// </summary>   
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectEquipmentXY> gridSystemXY)
    {
        bool drop = false;
        if (_equipmentGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {
            // Звук удачного размещения
            OnAddPlacedObjectAtEquipmentGrid?.Invoke(this, placedObject); // Запустим событие (запускаю здесь а не в EquipmentGrid т.к. placedObject еще надо настроить кодом выше)
            ResetPlacedObject(); // Обнулим взятый размещяемый объект

            drop = true;
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse("не удалось разместить", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки

            // Звук неудачи
            drop = false;
        }
        return drop;
    }

    /// <summary>
    /// Создадим размещаемый объект при нажатии на кнопку(в аргументе получаем позицию и тип объекта) 
    /// </summary>
    public void CreatePlacedObject(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO)
    {
        // Перед создание сделаем проверку    
        if (_selectedUnit == null) // Если нет выбранного юнита то и неукого настраивать экипировка
        {
            _tooltipUI.ShowShortTooltipFollowMouse("ВЫБЕРИ ЮНИТА", new TooltipUI.TooltipTimer { timer = 0.5f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            return; // Выходим игнор. код ниже
        }


        if (_placedObject != null) // Если Есть захваченый объект 
        {
            //Сбросим захваченный объект
            DropAddPlacedObjectInResources();
        }
        else
        {
            _offset = placedObjectTypeSO.GetOffsetVisualСenterFromAnchor() * _canvasPickUpDrop.scaleFactor; // Чтобы объект был по середине мышки надо вычесть смещение центра визуала относительно якоря и учесть масштаб канваса         
            _placedObject = PlacedObject.CreateInWorld(worldPosition - _offset, placedObjectTypeSO, _canvasPickUpDrop.transform, this); // Создадим объект
            _placedObject.Grab(); // Схватим его
        }
    }

    /// <summary>
    /// Получить позицию мыши на плоскости
    /// </summary>
    public Vector3 GetMousePositionOnPlane()
    {
        Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint());//Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
        return ray.GetPoint(planeDistance); // получим точку на луче где она пересекла плоскость
    }
    /// <summary>
    /// Cбросить и вернуть размещенный объект в разделе ресурсы
    /// </summary>
    private void DropAddPlacedObjectInResources()
    {
        _warehouseManager.AddCountPlacedObject(_placedObject.GetPlacedObjectTypeSO());
        _placedObject.Drop();
        _placedObject.SetFlagMoveStartPosition(true);
        ResetPlacedObject();
    }
    /// <summary>
    /// Обнулим взятый размещяемый объект
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


    /*  public Vector3 CalculateOffsetGrab() // вычислим смещение захвата
      {
          Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
          _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
          return _placedObject.transform.position - ray.GetPoint(planeDistance); // Вычислим смещение от точкой захвата и точкой  pivot на объекте.        
      }

      public Vector3 GetMousePosition(LayerMask layerMask) // Получить позицию мыши (static обозначает что метод принадлежит классу а не кокому нибудь экземпляру) // При одноэтажной игре
      {
          Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); // Луч от камеры в точку на экране где находиться курсор мыши
          Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
          return raycastHit.point; // Если луч попадет в колайдер то Physics.Raycast будет true, и raycastHit.point вернет "Точку удара в мировом пространстве, где луч попал в коллайдер", а если false то можно вернуть какоенибудь другое нужное значение(в нашем случае вернет нулевой вектор).
      }

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int newMouseGridPosition, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) // Получить Зафиксированное мировое положение мыши (над сеткой)
      {
          Vector2Int rotationOffset = _placedObject.GetRotationOffset(_dir); // Смещение объекта если он повернут
          Vector3 placedObjectWorldPosition = EquipmentGrid.Instance.GetWorldPositionCenterСornerCell(newMouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * EquipmentGrid.Instance.GetCellSizeWithScaleFactor();
          return placedObjectWorldPosition; // Вернет зафиксированное положение в узлах сетки
      }

      public Quaternion GetPlacedObjectRotation() // Получим вращение размещенного объекта
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
