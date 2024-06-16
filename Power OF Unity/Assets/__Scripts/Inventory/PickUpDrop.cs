
using System;
using UnityEngine;


public class PickUpDrop : MonoBehaviour // Поднятие Перетаскивание и Бросание объектовЫ
{

    public event EventHandler OnGrabbedObjectGridExits; // Захваченый объект покинул сетку
    public event EventHandler<PlacedObject> OnAddPlacedObjectAtInventoryGrid; // Объект добавлен в сетку Интвенторя
    public event EventHandler<PlacedObject> OnRemovePlacedObjectAtInventoryGrid; // Объект удален из сетки Интвенторя   
    public event EventHandler<OnGrabbedObjectGridPositionChangedEventArgs> OnGrabbedObjectGridPositionChanged; // позиция захваченного объекта на сетке изменилась
    public class OnGrabbedObjectGridPositionChangedEventArgs : EventArgs // Расширим класс событий, чтобы в аргументе события передать
    {
        public GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY; // Сеточная система позиции мыши
        public Vector2Int newMouseGridPosition;  // Новая сеточная позиция мыши
        public PlacedObject placedObject; // размещаемы объект
    }

    private LayerMask _inventoryLayerMask; // Для инвенторя настроить слой как Inventory // Настроим на объекте где есть коллайдер 
    private Transform _canvasInventoryWorld;
    private Camera _cameraInventoryUI;

    private PlacedObject _placedObject; // Размещенный объект    
    private Vector3 _offset; // Смещение от мышки 
    private Plane _plane; // плоскость по которой будем перемещять захваченные объекты    
    private Vector2Int _mouseGridPosition;  // сеточная позиция мыши

    /// <summary>
    /// Запущено событие (Захваченый объект покинул сетку), чтобы не запускать событие каждый кадр сделал переключатель
    /// </summary>
    private bool _startEventOnGrabbedObjectGridExits = false; 

    private GameInput _gameInput;
    private TooltipUI _tooltipUI;
    private InventoryGrid _inventoryGrid;   

   
    private void Awake()
    {
        _canvasInventoryWorld = GetComponentInParent<Canvas>().transform;
        _cameraInventoryUI = GetComponentInParent<Camera>();
    }

    public void Init(GameInput gameInput, TooltipUI tooltipUI, InventoryGrid inventoryGrid)
    {
        _gameInput = gameInput;
        _tooltipUI = tooltipUI;
        _inventoryGrid = inventoryGrid;
    }

    private void Start()
    {      
        _plane = new Plane(_canvasInventoryWorld.forward, _canvasInventoryWorld.position); // Создадим плоскость в позиции canvasInventory
        _inventoryLayerMask = LayerMask.GetMask("Inventory");       

        _gameInput.OnClickAction += GameInput_OnClickAction;
    }

    private void GameInput_OnClickAction(object sender, EventArgs e) // Если мыш нажата 
    {
        // Взять и положить объект можно только в сетке, проверим это           
        if (_inventoryGrid.TryGetGridSystemGridPosition(GetMousePositionOnPlane(), out GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int gridPositionMouse)) // Если над сеткой то попробуем получить ее
        {
            if (_placedObject == null) // Не имея при себе никакого предмета, попытайтесь схватить
            {
                TryGrab();
            }
            else // Попытаемся сбросим объект на сетку
            {
                TryDrop(gridSystemXY, gridPositionMouse, _placedObject);
            }
        }
        else //Если НЕ над сеткой 
        {
            if (_placedObject != null) // Если Есть захваченый объект 
            {
                _placedObject.Drop();
                _placedObject.SetMoveStartPosition(true); //то перенести объект в стартовое положение, уничтожить и вернуть его в список товаров
                ResetPlacedObject(); // Обнулим взятый размещяемый объект
            }
        }
    }

    private void Update()
    {
        if (_placedObject != null) // Если есть захваченный объект будем его перемещать за указателем мыши по созданной плоскости
        {
            SetTargetPosition();
        }
    }
    /// <summary>
    /// Попробуем СХВАТИТЬ предмет
    /// </summary>
    public void TryGrab()
    {
        Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
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
    /// <summary>
    /// Попробуем СБРОСИТЬ захваченный предмет
    /// </summary>
    public bool TryDrop(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, Vector2Int gridPositionMouse, PlacedObject placedObject)
    {
        // Попробуем сбросить и разместить на сетке       
        InventorySlot gridName = gridSystemXY.GetGridSlot(); // Получим имя сетки

        if (!placedObject.GetCanPlacedOnGridList().Contains(gridName)) //Если нашей сетки НЕТ в списке сеток где можно разместить наш объект то
        {
            _tooltipUI.ShowTooltipsFollowMouse("попробуй другой слот", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки

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
            case InventorySlot.OtherWeaponSlot:

                gridPositionMouse = new Vector2Int(0, 0);
                drop = TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY);
                break;
        }
        return drop;
    }
    /// <summary>
    /// Попробую Добавить Размещаемый объект в Позицию Сетки
    /// </summary>   
    private bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY)
    {
        bool drop = false;
        if (_inventoryGrid.TryAddPlacedObjectAtGridPosition(gridPositionMouse, placedObject, gridSystemXY))
        {
            placedObject.Drop();  // Бросить
            placedObject.SetGridPositionAnchor(gridPositionMouse); // Установим новую сеточную позицию якоря
            placedObject.SetGridSystemXY(gridSystemXY); //Установим сетку на которую добавили наш оббъект

            // Звук удачного размещения
            OnAddPlacedObjectAtInventoryGrid?.Invoke(this, placedObject); // Запустим событие (запускаю здесь а не в InventoryGrid т.к. placedObject еще надо настроить кодом выше)
            ResetPlacedObject(); // Обнулим взятый размещяемый объект

            drop = true;
        }
        else
        {
            _tooltipUI.ShowTooltipsFollowMouse("не удалось разместить", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки

            // Звук неудачи
            drop = false;
        }
        return drop;
    }

    public void SetTargetPosition()
    {
        Vector3 mousePositionOnPlane = GetMousePositionOnPlane();
        Vector2Int zeroGridPosition = new Vector2Int(0, 0); // Нулевая позиция сетки
        bool tryGetGridSystemGridPosition = _inventoryGrid.TryGetGridSystemGridPosition(mousePositionOnPlane, out GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int newMouseGridPosition);

        if (tryGetGridSystemGridPosition) // Если над сеткой то попробуем получить ее
        {
            InventorySlot gridName = gridSystemXY.GetGridSlot(); // Получим имя сетки
            switch (gridName)
            {
                case InventorySlot.BagSlot:
                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionLowerLeftСornerCell(newMouseGridPosition, gridSystemXY));
                    _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));

                    if (_mouseGridPosition != newMouseGridPosition || _mouseGridPosition == zeroGridPosition) // Если сеточная позиция не равна предыдущей или равна нулевой позиции то ...
                    {
                        OnGrabbedObjectGridPositionChanged?.Invoke(this, new OnGrabbedObjectGridPositionChangedEventArgs //запустим - Событие позиция мыши на сетке изменилось и передадим новою сеточную позицию
                        {
                            gridSystemXY = gridSystemXY,
                            newMouseGridPosition = newMouseGridPosition,
                            placedObject = _placedObject
                        }); // Создадим событие и передадим

                        _mouseGridPosition = newMouseGridPosition; // Перепишем предыдущую позицию на новую
                    }
                    break;

                // для сетки Основного и Доп. оружия установим TargetPosition в центре сетки
                case InventorySlot.MainWeaponSlot:
                case InventorySlot.OtherWeaponSlot:

                    _placedObject.SetTargetPosition(_inventoryGrid.GetWorldPositionGridCenter(gridSystemXY) - _offset); //Чтобы объект был по центру сетки надо вычесть смещение визуала относительно родителя
                    _placedObject.SetTargetRotation(_inventoryGrid.GetRotationAnchorGrid(gridSystemXY));

                    OnGrabbedObjectGridPositionChanged?.Invoke(this, new OnGrabbedObjectGridPositionChangedEventArgs //запустим - Событие позиция мыши на сетке изменилось и передадим новою сеточную позицию
                    {
                        gridSystemXY = gridSystemXY,
                        newMouseGridPosition = zeroGridPosition,
                        placedObject = _placedObject
                    }); // Создадим событие и передадим                       
                    break;
            }
            _startEventOnGrabbedObjectGridExits = false; // Сбросим параметр
        }
        else // Если не над сеткой то просто следуем за мышью
        {
            _placedObject.SetTargetPosition(mousePositionOnPlane - _offset);// Чтобы объект был по центру мышки надо вычесть смещение визуала относительно родителя
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
            _placedObject.SetMoveStartPosition(true);
            ResetPlacedObject(); // Обнулим взятый размещяемый объект (_placedObject = null)
        }
        else
        {
            _placedObject = PlacedObject.CreateInWorld(worldPosition, placedObjectTypeSO, _canvasInventoryWorld); // Создадим объект
            _placedObject.Grab(); // Схватим его
            _offset = _placedObject.GetOffsetVisualFromParent(); // Чтобы объект был по центру мышки надо вычесть смещение визуала относительно родителя          
        }
    }

    public Vector3 GetMousePositionOnPlane() // Получить позицию мыши на плоскости
    {
        Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition());//Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
        _plane.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
        return ray.GetPoint(planeDistance); // получим точку на луче где она пересекла плоскость
    }


    private void ResetPlacedObject() // Обнулим взятый размещяемый объект
    {
        _placedObject = null;
    }


    /*  public Vector3 CalculateOffsetGrab() // вычислим смещение захвата
      {
          Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); //Возвращает луч, идущий от камеры через точку экрана где находиться курсор мыши 
          _plane.Raycast(ray, out float planeDistance); // Пересечем луч и плоскость и получим расстояние вдоль луча, где он пересекает плоскость.
          return _placedObject.transform.position - ray.GetPoint(planeDistance); // Вычислим смещение от точкой захвата и точкой  pivot на объекте.        
      }

      public Vector3 GetMousePosition(LayerMask layerMask) // Получить позицию мыши (static обозначает что метод принадлежит классу а не кокому нибудь экземпляру) // При одноэтажной игре
      {
          Ray ray = _cameraInventoryUI.ScreenPointToRay(_gameInput.GetMouseScreenPosition()); // Луч от камеры в точку на экране где находиться курсор мыши
          Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
          return raycastHit.point; // Если луч попадет в колайдер то Physics.Raycast будет true, и raycastHit.point вернет "Точку удара в мировом пространстве, где луч попал в коллайдер", а если false то можно вернуть какоенибудь другое нужное значение(в нашем случае вернет нулевой вектор).
      }

      public Vector3 GetMouseWorldSnappedPosition(Vector2Int mouseGridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) // Получить Зафиксированное мировое положение мыши (над сеткой)
      {
          Vector2Int rotationOffset = _placedObjectTypeSO.GetRotationOffset(_dir); // Смещение объекта если он повернут
          Vector3 placedObjectWorldPosition = InventoryGrid.Instance.GetWorldPositionCenterСornerCell(mouseGridPosition, gridSystemXY) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * InventoryGrid.Instance.GetCellSize();
          return placedObjectWorldPosition; // Вернет зафиксированное положение в узлах сетки
      }

      public Quaternion GetPlacedObjectRotation() // Получим вращение размещенного объекта
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
