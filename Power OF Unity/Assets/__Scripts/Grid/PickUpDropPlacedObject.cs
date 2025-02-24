using System;
using UnityEngine;

/// <summary>
/// Поднятие Бросание и Размещение объектов(в сетке). Через этот класс идет взаимодействие с EquipmentGrid.<br/>
/// Когда объект схвачен он переходит в промежуточное состояние между РЕСУРСАМИ и ЭЕИПИРОВКОЙ юнита
/// </summary>
/// <remarks>
/// Во время размещения, в PlacedObject передается сетка размещения и позиция якоря предмета на сетки.<br/>
/// Работает в паре с классом ItemSelectButtonsSystemUI
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour, IToggleActivity
{
    public event EventHandler OnGrabbedObjectGridExits; // Захваченый объект покинул сетку   
    public event EventHandler<PlacedObjectGridParameters> OnGrabbedObjectGridPositionChanged; // позиция захваченного объекта на сетке изменилась 

    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtEquipmentSystem; // Объект удален из систему Интвенторя   

    private LayerMask _equipmentLayerMask; // Для экипировки настроить слой как Equipment // Настроим на объекте где есть коллайдер 
    private Canvas _canvasPickUpDrop;
    private Camera _camera;
    private RenderMode _canvasRenderMode;
    private PlacedObject _placedObject; // Размещенный объект
    /// <summary>
    /// Размещенный объект  над которым вошла мышь
    /// </summary>
    private PlacedObject _placedObjectMouseEnter;
    private Vector3 _offset; //смещение центра визуала относительно якоря
    private Plane _planeForCanvasInWorldSpace; // плоскость(по которой будем перемещять захваченные объекты) для канваса в мировом пространстве    
    private Vector2Int _mouseGridPosition;  // сеточная позиция мыши

    /// <summary>
    /// Запущено событие (Захваченый объект покинул сетку), чтобы не запускать событие каждый кадр сделал переключатель
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
                _camera = GetComponentInParent<Camera>(); // Для канваса будем использовать дополнительную камеру
                break;
            case RenderMode.WorldSpace:
                _planeForCanvasInWorldSpace = new Plane(_canvasPickUpDrop.transform.forward, _canvasPickUpDrop.transform.position); // Создадим плоскость в позиции canvasEquipment
                _camera = GetComponentInParent<Camera>(); // Для канваса в мировом пространстве будем использовать дополнительную камеру
                break;
        }

        _equipmentLayerMask = LayerMask.GetMask("Equipment");
        _selectedUnit = _unitManager.GetSelectedUnit();
    }

    public void SetActive(bool active)
    {
        if (_isActive == active) //Если предыдущее состояние тоже то выходим
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
            mousePosition = _gameInput.GetMouseScreenPoint();
        else
            mousePosition = GetMousePositionOnPlane();


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
        else //Если НЕ над сеткой (ДОП ПРОВЕРКА) if(не над кнопками созданмя предметов)  
        {

            if (_placedObject != null) // Если Есть захваченый объект 
            {
                DropAddPlacedObjectInResources();
            }
        }
    }

    private void GameInput_OnClickRemoveAction(object sender, EventArgs e)
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
            mousePosition = _gameInput.GetMouseScreenPoint();
        else
            mousePosition = GetMousePositionOnPlane();

        // Обработаю ситуации над СЕТКОЙ и ВНЕ-СЕТКИ          
        if (_equipmentGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out mouseGridPosition)) // Если над сеткой то 
        {
            if (_placedObject == null) // Не имея при себе никакого предмета
            {
                // Есди под курсором есть предмет то удалить из сетки и вернуть в РЕСУРСЫ
                if (_placedObjectMouseEnter != null)
                {
                    _unitEquipmentSystem.RemoveFromGridAndUnitEquipmentWithCheck(_placedObjectMouseEnter, returnInResourcesAndStartPosition: true);
                    // Звук поднятия            
                }

            }
            else // Если Есть захваченый объект 
            {
                // Сбросим и вернем в РЕСУРСЫ 
                DropAddPlacedObjectInResources();
                OnGrabbedObjectGridExits?.Invoke(this, EventArgs.Empty); // Что бы обновить подсвечиваемое предпологаемое место сброса
            }
        }
        else //Если НЕ над сеткой  (ДУБЛИРУЕТ поведение лев клавиши мыши)
        {
            if (_placedObject != null) // Если Есть захваченый объект 
            {
                // Сбросим и вернем в РЕСУРСЫ предмет который держим
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
                case EquipmentSlot.HeadArmorSlot:
                case EquipmentSlot.BodyArmorSlot:

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
    /// Попробуем СХВАТИТЬ предмет (с сетки инвенторя)
    /// </summary>
    private void TryGrab()
    {
        if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то
        {
            Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _equipmentLayerMask)) // Вернет true если попадет в инвертарь.
            {
                _placedObject = raycastHit.transform.GetComponentInParent<PlacedObject>();
                if (_placedObject != null) // Если у родителя объекта в который попали есть PlacedObject то можно его схватить (на кнопке висит просто визуал и там нет род объекта)
                {
                    _placedObject.Grab(); // Схватим его                                     
                    _unitEquipmentSystem.RemoveFromGridAndUnitEquipmentWithCheck(_placedObject);
                    // Звук поднятия                    
                }
            }
        }
        else
        {
            if (_placedObjectMouseEnter != null)
            {
                _placedObject = _placedObjectMouseEnter;
                _placedObject.Grab(); // Схватим его
                _offset = _placedObject.GetOffsetCenterFromAnchor() * _canvasPickUpDrop.scaleFactor;
                _unitEquipmentSystem.RemoveFromGridAndUnitEquipmentWithCheck(_placedObject);
                // Звук поднятия            
            }
        }
    }


    /// <summary>
    /// Cбросить и вернуть размещенный объект в разделе ресурсы
    /// </summary>
    private void DropAddPlacedObjectInResources()
    {
        _placedObject.Drop();
        _unitEquipmentSystem.ReturnPlacedObjectInResourcesAndStartPosition(_placedObject);
        ResetPlacedObject();
    }


    /// <summary>
    /// Попробуем СБРОСИТЬ захваченный предмет
    /// </summary>
    private bool TryDrop(GridSystemXY<GridObjectEquipmentXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // Попробуем сбросить и разместить на сетке       
        EquipmentSlot slotName = gridSystemXY.GetGridSlot(); // Получим имя слота

        if (!placedObject.CanPlaceOnSlot(slotName)) //Если нашего слота НЕТ в списке где можно разместить наш объект то
        {
            _tooltipUI.ShowShortTooltipFollowMouse("попробуй другой слот", new TooltipUI.TooltipTimer { timer = 2f });
            // Звук неудачи
            return false;
        }

        // Проверим на совместимость с другими предметами экипировки
        if (!_unitEquipmentSystem.CompatibleWithOtherEquipment(placedObject))
        {
            _tooltipUI.ShowShortTooltipFollowMouse("несовместимая экипировка", new TooltipUI.TooltipTimer { timer = 0.8f });
            // Звук неудачи
            return false;
        }

        // для слота который может принять только один предмет установим gridPositionMouse = (0,0)
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
    /// Попробую Добавить Размещаемый объект в Позицию Сетки
    /// </summary>   
    private bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectEquipmentXY> gridSystemXY)
    {
        bool drop = false;
        if (_equipmentGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {
            _unitEquipmentSystem.AddPlacedObjectAtUnitEquipment(_placedObject);
            ResetPlacedObject(); // Обнулим взятый размещяемый объект
            // Звук удачного размещения           
            drop = true;
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse("не удалось разместить", new TooltipUI.TooltipTimer { timer = 0.8f }); // Покажем подсказку и зададим новый таймер отображения подсказки
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
            //Сбросим захваченный объект. Оставил на всякий случай (ЭТОТ КОД НЕДОСТИЖИМЫ т.к. сначала обрабатывается метод GameInput_OnClickAction где и происходит сбрасывание предмета)
            DropAddPlacedObjectInResources();
        }
        else
        {
            _offset = placedObjectTypeSO.GetOffsetVisualСenterFromAnchor() * _canvasPickUpDrop.scaleFactor; // Чтобы объект был по середине мышки надо вычесть смещение центра визуала относительно якоря и учесть масштаб канваса         
            _placedObject = PlacedObject.CreateInWorld(worldPosition - _offset, placedObjectTypeSO, _canvasPickUpDrop.transform, this); // Создадим объект
            _placedObject.SetDropPositionWhenDeleted(worldPosition - _offset);
            _placedObject.Grab(); // Схватим его
        }
    }

    /// <summary>
    /// Получить позицию мыши на плоскости
    /// </summary>
    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint());//Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
        return ray.GetPoint(planeDistance); // получим точку на луче где она пересекла плоскость
    }


    /// <summary>
    /// Обнулим взятый размещяемый объект
    /// </summary>
    private void ResetPlacedObject()
    {
        _placedObject = null;
    }
    /// <summary>
    /// Установить PlacedObject над которым вошла мышь
    /// </summary>
    public void SetPlacedObjectMouseEnter(PlacedObject placedObject)
    {
        _placedObjectMouseEnter = placedObject;
    }

    public Canvas GetCanvas() { return _canvasPickUpDrop; }

    /*  public Vector3 CalculateOffsetGrab() // вычислим смещение захвата
      {
          Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
          _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
          return _placedObject.transform.gridPosition - ray.GetPoint(planeDistance); // Вычислим смещение от точкой захвата и точкой  pivot на объекте.        
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
