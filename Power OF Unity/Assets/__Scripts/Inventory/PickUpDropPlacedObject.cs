using System;
using UnityEngine;

/// <summary>
/// Поднятие Бросание и Размещение объектов(в сетке). Через этот класс идет взаимодействие с InventoryGrid
/// </summary>
/// <remarks>
/// Во время размещения, в PlacedObject передается сетка размещения и позиция якоря предмета на сетки.
/// </remarks>
public class PickUpDropPlacedObject : MonoBehaviour // 
{
    public event EventHandler OnGrabbedObjectGridExits; // Захваченый объект покинул сетку
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // Объект добавлен в сетку Интвенторя
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя   
    public event EventHandler<PlacedObjectParameters> OnGrabbedObjectGridPositionChanged; // позиция захваченного объекта на сетке изменилась 

    private LayerMask _inventoryLayerMask; // Для инвенторя настроить слой как Inventory // Настроим на объекте где есть коллайдер 
    private Canvas _canvasInventory;
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
        if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то
        {
            _planeForCanvasInWorldSpace = new Plane(_canvasInventory.transform.forward, _canvasInventory.transform.position); // Создадим плоскость в позиции canvasInventory
            _camera = GetComponentInParent<Camera>(); // Для канваса в мировом пространстве будем использовать дополнительную камеру
        }
        else
        {
            _camera = Camera.main;
        }
        _inventoryLayerMask = LayerMask.GetMask("Inventory");

        _gameInput.OnClickAction += GameInput_OnClickAction;
    }

    private void GameInput_OnClickAction(object sender, EventArgs e) // Если мыш нажата 
    {
        // Установим дефолтное состояние полей
        GridSystemXY<GridObjectInventoryXY> gridSystemXY;
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
        if (_inventoryGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out mouseGridPosition)) // Если над сеткой то 
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
                _placedObject.Drop();
                _placedObject.SetFlagMoveStartPosition(true); //то перенести объект в стартовое положение, уничтожить и вернуть его в список товаров
                ResetPlacedObject(); // Обнулим взятый размещяемый объект
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
    /// <summary>
    /// Попробуем СХВАТИТЬ предмет
    /// </summary>
    public void TryGrab()
    {
        if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то
        {
            Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _inventoryLayerMask)) // Вернет true если попадет в инвертарь.
            {
                _placedObject = raycastHit.transform.GetComponentInParent<PlacedObject>();
                if (_placedObject != null) // Если у родителя объекта в который попали есть PlacedObject то можно его схватить (на кнопка висит просто визуал и там нет род объекта)
                {
                    _placedObject.Grab(); // Схватим его
                    _inventoryGrid.RemovePlacedObjectAtGrid(_placedObject);// Удалим из текущей сеточной позиции               

                    // Звук поднятия
                    OnRemovePlacedObjectAtInventoryGrid?.Invoke(this, _placedObject); // Запустим событие
                }
            }
        }
        else
        {
            if (_placedObjectMouseEnter != null)
            {
                _placedObject = _placedObjectMouseEnter;
                _placedObject.Grab(); // Схватим его
                _inventoryGrid.RemovePlacedObjectAtGrid(_placedObject);// Удалим из текущей сеточной позиции               
                _offset = _placedObject.GetOffsetVisualFromParent() * _canvasInventory.scaleFactor;
                // Звук поднятия
                OnRemovePlacedObjectAtInventoryGrid?.Invoke(this, _placedObject); // Запустим событие
            }
        }
    }
    /// <summary>
    /// Попробуем СБРОСИТЬ захваченный предмет
    /// </summary>
    public bool TryDrop(GridSystemXY<GridObjectInventoryXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // Попробуем сбросить и разместить на сетке       
        InventorySlot gridName = gridSystemXY.GetGridSlot(); // Получим имя сетки

        if (!placedObject.GetCanPlacedOnGridList().Contains(gridName)) //Если нашей сетки НЕТ в списке сеток где можно разместить наш объект то
        {
            _tooltipUI.ShowShortTooltipFollowMouse("попробуй другой слот", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки

            // Звук неудачи
            return false;
        }

        bool drop = false;
        switch (gridName)
        {
            case InventorySlot.BagSlot:

                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;

            // для сетки Основного и Доп. оружия установим newMouseGridPosition (0,0)
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponsSlot:

                gridPositionMouse = new Vector2Int(0, 0);
                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;
        }
        return drop;
    }
    /// <summary>
    /// Попробую Добавить Размещаемый объект в Позицию Сетки
    /// </summary>   
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemXY<GridObjectInventoryXY> gridSystemXY)
    {
        bool drop = false;
        if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {

            // Звук удачного размещения
            OnAddPlacedObjectAtInventoryGrid?.Invoke(this, placedObject); // Запустим событие (запускаю здесь а не в InventoryGrid т.к. placedObject еще надо настроить кодом выше)
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

    private void SetTargetPosition()
    {
        Vector2Int zeroGridPosition = Vector2Int.zero; // Нулевая позиция сетки                
        GridSystemXY<GridObjectInventoryXY> gridSystemXY;
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

        if (_inventoryGrid.TryGetGridSystemGridPosition(mousePosition, out gridSystemXY, out newMouseGridPosition)) // Если над сеткой то попробуем получить ее
        {
            InventorySlot inventorySlot = gridSystemXY.GetGridSlot(); // Получим слот размещения в инвентаре
            switch (inventorySlot)
            {
                case InventorySlot.BagSlot:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionLowerLeftСornerCell(newMouseGridPosition, gridSystemXY));
                    if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то учтем и его поворот
                    {
                        _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));
                    }

                    if (_mouseGridPosition != newMouseGridPosition || _mouseGridPosition == zeroGridPosition) // Если сеточная позиция не равна предыдущей или равна нулевой позиции то ...
                    {
                        //запустим - Событие позиция мыши на сетке изменилось и передадим новою параметры размещаемого объекта                     
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectParameters
                        {
                            slot = inventorySlot,
                            gridPositioAnchor = newMouseGridPosition,
                            placedObject = _placedObject,
                        });

                        _mouseGridPosition = newMouseGridPosition; // Перепишем предыдущую позицию на новую
                    }
                    break;

                // для сетки Основного и Доп. оружия установим TargetPosition в центре сетки
                case InventorySlot.MainWeaponSlot:
                case InventorySlot.OtherWeaponsSlot:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //Чтобы объект был по середине сетки надо вычесть смещение центра визуала относительно якоря
                    if (_canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то учтем и его поворот
                    {
                        _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));
                    }
                    else
                    {
                        _placedObject.SetTargetRotation(Vector3.zero);
                    }

                    //запустим - Событие позиция мыши на сетке изменилось и передадим новою параметры размещаемого объекта
                    OnGrabbedObjectGridPositionChanged?.Invoke(this, new PlacedObjectParameters
                    {
                        slot = inventorySlot,
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

    public void CreatePlacedObject(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO) //Создадим размещаемый объект при нажатии на кнопку(в аргументе получаем позицию и тип объекта) 
    {
        // Перед создание сделаем проверку       
        if (_placedObject != null) // Если Есть захваченый объект 
        {
            //Сбросим захваченный объект
            _placedObject.Drop();
            _placedObject.SetFlagMoveStartPosition(true);
            ResetPlacedObject(); // Обнулим взятый размещяемый объект (_placedObject = null)
        }
        else
        {
            _offset = placedObjectTypeSO.GetOffsetVisualСenterFromAnchor() * _canvasInventory.scaleFactor; // Чтобы объект был по середине мышки надо вычесть смещение центра визуала относительно якоря и учесть масштаб канваса         
            _placedObject = PlacedObject.CreateInWorld(worldPosition - _offset, placedObjectTypeSO, _canvasInventory.transform, this); // Создадим объект
            _placedObject.Grab(); // Схватим его
        }
    }

    public Vector3 GetMousePositionOnPlane() // Получить позицию мыши на плоскости
    {
        Ray ray = _camera.ScreenPointToRay(_gameInput.GetMouseScreenPoint());//Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
        _planeForCanvasInWorldSpace.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
        return ray.GetPoint(planeDistance); // получим точку на луче где она пересекла плоскость
    }

    private void ResetPlacedObject() // Обнулим взятый размещяемый объект
    {
        _placedObject = null;
    }

    public void SetPlacedObjectMouseEnter(PlacedObject placedObject)
    {
        _placedObjectMouseEnter = placedObject;
    }

    public Canvas GetCanvasInventory() { return _canvasInventory; }

    public GridSystemXY<GridObjectInventoryXY> GetGridSystemXY(InventorySlot inventorySlot) => _inventoryGrid.GetGridSystemXY(inventorySlot);


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

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int newMouseGridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) // Получить Зафиксированное мировое положение мыши (над сеткой)
      {
          Vector2Int rotationOffset = _placedObject.GetRotationOffset(_dir); // Смещение объекта если он повернут
          Vector3 placedObjectWorldPosition = InventoryGrid.Instance.GetWorldPositionCenterСornerCell(newMouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * InventoryGrid.Instance.GetCellSizeWithScaleFactor();
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

}
