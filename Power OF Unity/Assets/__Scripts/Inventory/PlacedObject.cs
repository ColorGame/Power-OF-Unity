using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Экземпляр размещенного объекта(создаем экземпляр на сетке, в миру).
/// </summary>
/// <remarks>
/// Прикрипить к перетаскиваемому объекту 
/// </remarks>
public class PlacedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Создадим экземпляр PlacedObject в стеке.
    /// </summary>
    public static PlacedObject CreateInGrid(PlacedObjectParameters placedObjectParameters, PickUpDropPlacedObject pickUpDropPlacedObject, InventoryGrid inventoryGrid) // (static обозначает что метод принадлежит классу а не кокому нибудь экземпляру)
    {
        PlacedObjectTypeSO placedObjectTypeSO = placedObjectParameters.placedObjectTypeSO;
        Vector2Int gridPositionAnchor = placedObjectParameters.gridPositioAnchor;
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = inventoryGrid.GetGridSystemXY(placedObjectParameters.slot);
        Canvas canvas = inventoryGrid.GetCanvas();

        Vector3 worldPosition = new Vector3();
        switch (placedObjectParameters.slot)
        {          
            case InventorySlot.BagSlot:
                 worldPosition = gridSystemXY.GetWorldPositionLowerLeftСornerCell(gridPositionAnchor);
                break;            
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponsSlot:
                Vector3 offset = placedObjectTypeSO.GetOffsetVisualСenterFromAnchor()* canvas.scaleFactor;
                worldPosition = gridSystemXY.GetWorldPositionGridCenter()- offset;
                break;
        }

        PlacedObject placedObject = CreateInWorld(worldPosition, placedObjectTypeSO, canvas.transform, pickUpDropPlacedObject);
        placedObject._gridSystemXY = gridSystemXY;
        placedObject._gridPositioAnchor = gridPositionAnchor;

        return placedObject;
    }

    /// <summary>
    /// Создадим экземпляр PlacedObject в мировом пространстве
    /// </summary>
    /// <remarks> Якорь(нижний левый край) созданного объекта будет = worldPosition</remarks>
    public static PlacedObject CreateInWorld(Vector3 worldPosition, PlacedObjectTypeSO placedObjectTypeSO, Transform parent, PickUpDropPlacedObject pickUpDropPlacedObject)
    {       
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab2D(), worldPosition , Quaternion.Euler(parent.rotation.eulerAngles.x, 0, 0), parent); //canvasContainer.rotation.eulerAngles.x- что бы был повернут как родитель
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._offsetVisualFromParent = placedObjectTypeSO.GetOffsetVisualСenterFromAnchor();
        placedObject._pickUpDropPlacedObject = pickUpDropPlacedObject;
        placedObject.Setup();

        return placedObject;
    }


    private PlacedObjectTypeSO _placedObjectTypeSO;
    private GridSystemXY<GridObjectInventoryXY> _gridSystemXY; // Сетка в которой разместилься наш объект
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private Vector2Int _gridPositioAnchor; // Сеточная позиция Якоря  
    private Vector3 _targetRotation;
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private Vector3 _scaleOriginal;
    private bool _grabbed; // Схвачен   
    private bool _moveStartPosition = false; // Переместить в начальную позицию        
    private Vector3 _offsetVisualFromParent;
    private List<InventorySlot> _canPlacedOnSlotList;// Слоты инвенторя где можно разместить наш объект   

    protected virtual void Setup()
    {
        _canPlacedOnSlotList = _placedObjectTypeSO.GetCanPlacedOnSlotList();
        _scaleOriginal = transform.localScale; // Сохраним оригинальный масштаб
        _startPosition = transform.position;  // Запомним стартовую позицию       
    }



    private void LateUpdate()
    {
        if (_grabbed) // Если объект взяли то
        {
            float moveSpeed = 20f;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
            if (_targetRotation != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime * 15f);
            }
        }

        if (_moveStartPosition) // Если надо переместить в начальную позиции и в конце уничтожим объект
        {
            float moveSpeed = 20f;
            transform.position = Vector3.Lerp(transform.position, _startPosition, Time.deltaTime * moveSpeed);

            float stoppingDistance = 0.1f; // Дистанция остановки //НУЖНО НАСТРОИТЬ//
            if (Vector3.Distance(transform.position, _startPosition) < stoppingDistance)  // Если растояние до целевой позиции меньше чем Дистанция остановки // Мы достигли цели        
            {
                _moveStartPosition = false; //прекратить движение Мы достигли _startPosition               
                Destroy(gameObject);
            }
        }
    }

    public void Grab() // Захватить
    {
        _grabbed = true;
        //поменяем оригинальный масштаб    
        float _scaleMultiplier = 1.1f; // множитель масштаба для эффекта увеличения при захвате
        transform.localScale = _scaleOriginal * _scaleMultiplier;
    }

    public void Drop() // Бросить
    {
        _grabbed = false;
        transform.localScale = _scaleOriginal;
    }

    public void SetTargetPosition(Vector3 targetPosition) { _targetPosition = targetPosition; }
    public void SetTargetRotation(Vector3 targetRotation) { _targetRotation = targetRotation; }
    public Vector3 GetOffsetVisualFromParent() { return _offsetVisualFromParent; }
    public Vector2Int GetGridPositionAnchor() { return _gridPositioAnchor; }
    public void SetGridPositionAnchor(Vector2Int gridPosition) { _gridPositioAnchor = gridPosition; }

    /// <summary>
    /// Получим сетку на которую добавили наш оббъект
    /// </summary>
    public GridSystemXY<GridObjectInventoryXY> GetGridSystemXY() { return _gridSystemXY; }

    /// <summary>
    /// Установим сетку на которую добавили наш оббъект
    /// </summary>
    public void SetGridSystemXY(GridSystemXY<GridObjectInventoryXY> gridSystemXY) { _gridSystemXY = gridSystemXY; }

    /// <summary>
    /// Получить список занимаемых позиций в сетке. (в аргумент передадим его актуальную сеточную позицию и направлени)
    /// </summary>
    public List<Vector2Int> GetOccupiesGridPositionList() { return _placedObjectTypeSO.GetGridPositionList(_gridPositioAnchor); }

    /// <summary>
    /// Получить список позиций в сетке, которые пытается занять. (в аргумент передадим сеточную позицию где хотим разместить)
    /// </summary>
    public List<Vector2Int> GetTryOccupiesGridPositionList(Vector2Int gridPosition) { return _placedObjectTypeSO.GetGridPositionList(gridPosition); }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() { return _placedObjectTypeSO; }
    public Vector3 GetStartPosition() { return _startPosition; }
    public void SetStartPosition(Vector3 startPosition) { _startPosition = startPosition; }
    /// <summary>
    /// Установить флаг, что надо перемещаться в стартовую позицию
    /// </summary>    
    public void SetFlagMoveStartPosition(bool moveStartPosition) { _moveStartPosition = moveStartPosition; }
        
    /// <summary>
    /// Получить Слот где можно разместить наш объект
    /// </summary>
    public List<InventorySlot> GetCanPlacedOnGridList() { return _canPlacedOnSlotList; }

    public PlacedObjectParameters GetPlacedObjectParameters()
    {
        return new PlacedObjectParameters(_gridSystemXY.GetGridSlot(), _gridPositioAnchor, _placedObjectTypeSO);
    }


    // для канваса в режиме ScrenSpace

    public void OnPointerEnter(PointerEventData eventData) /// Если мыш над этим объектом то передадим этот объект в PickUpDropPlacedObject
    {
        _pickUpDropPlacedObject.SetPlacedObjectMouseEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pickUpDropPlacedObject.SetPlacedObjectMouseEnter(null);
    }

}
