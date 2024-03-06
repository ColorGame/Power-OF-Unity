using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Размещенный объект. Создаем и размещаем объект на сетке
/// </summary>
/// <remarks>
/// Прикрипить к перетаскиваемому объекту 
/// </remarks>
public class PlacedObject : MonoBehaviour 
{
    public static PlacedObject CreateInGrid(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, Vector2Int gridPosition, PlacedObjectTypeSO placedObjectTypeSO, Transform parent) // (static обозначает что метод принадлежит классу а не кокому нибудь экземпляру)
    {        
        Vector3 offset = placedObjectTypeSO.GetOffsetVisualFromParent(); // вычислим смещение чтобы в дальнейшем создавать объекты в центре worldPosition
        Vector3 worldPosition = InventoryGrid.Instance.GetWorldPositionLowerLeftСornerCell(gridPosition, gridSystemXY);
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab(), worldPosition, Quaternion.Euler(parent.rotation.eulerAngles.x, 0, 0), parent); //parent.rotation.eulerAngles.x- что бы был повернут как родитель
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
     
        placedObject._offsetVisualFromParent = offset;       
        placedObject.Setup();        

        return placedObject;
    }

    public static PlacedObject CreateInWorld(Vector3 worldPosition,  PlacedObjectTypeSO placedObjectTypeSO, Transform parent)
    {
        Vector3 offset = placedObjectTypeSO.GetOffsetVisualFromParent(); // вычислим смещение чтобы создать объект в центре worldPosition
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab(), worldPosition - offset, Quaternion.Euler(parent.rotation.eulerAngles.x, 0, 0), parent); //parent.rotation.eulerAngles.x- что бы был повернут как родитель

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;
       
        placedObject._offsetVisualFromParent = offset;        
        placedObject.Setup();

        return placedObject;
    }


    public static PlacedObject CreateInCanvas(Transform parent, Vector2 anchoredPosition, Vector2Int gridPosition, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.GetPrefab(), parent);       
        placedObjectTransform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._placedObjectTypeSO = placedObjectTypeSO;       
 
        placedObject.Setup();

        return placedObject;
    }

    

    private PlacedObjectTypeSO _placedObjectTypeSO;
    private GridSystemTiltedXY<GridObjectInventoryXY> _gridSystemXY; // Сетка в которой разместилься наш объект
    private Vector2Int _gridPositioAnchor; // Сеточная позиция Якоря  
    private Vector3 _targetRotation;
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private Vector3 _scaleOriginal;
    private bool _grabbed; // Схвачен   
    private bool _moveStartPosition = false; // Переместить в начальную позицию    
    private Transform _visual;
    private Vector3 _offsetVisualFromParent;
    private BaseAction _baseAction; // Базовое действие которое выполняет наш объект
    private List<GridName> _canPlacedOnGridList;// Сетки где можно разместить наш объект

    private void Start()
    {
        if(TryGetComponent(out BaseAction baseAction)) // Если на объекте есть Базовое действие то вернем его
        {
            _baseAction = baseAction;
        }
    }

    private void LateUpdate()
    {
        if (_grabbed) // Если объект взяли то
        {
            float moveSpeed = 20f;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotation), Time.deltaTime * 15f);
            //transform.rotation = Quaternion.Lerp(transform.rotation, PickUpDrop.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);// Плавно повернем объект
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

    protected virtual void Setup()
    {
        _canPlacedOnGridList = _placedObjectTypeSO.GetCanPlacedOnGridList();
        _visual = transform.GetChild(0); //Получим визуальный объект   
        _visual.localPosition = _offsetVisualFromParent;    // Установим в середину занимаемых ячеек наш визуальный объект (Если забыли установить вручную в префабе инвенторя)           
        _scaleOriginal = transform.localScale; // Сохраним оригинальный масштаб
        _startPosition = transform.position;  // Запомним стартовую позицию        
    }

    public Vector3 GetOffsetVisualFromParent()
    {
        return _offsetVisualFromParent;
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

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public void SetTargetRotation(Vector3 targetRotation)
    {
        _targetRotation = targetRotation;
    }  

    public Vector2Int GetGridPositionAnchor()
    {
        return _gridPositioAnchor;
    }

    public void SetGridPositionAnchor(Vector2Int gridPosition)
    {
        _gridPositioAnchor = gridPosition;
    }

    public GridSystemTiltedXY<GridObjectInventoryXY> GetGridSystemXY() //Получим сетку на которую добавили наш оббъект
    {
        return _gridSystemXY;
    }

    public void SetGridSystemXY(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) //Установим сетку на которую добавили наш оббъект
    {
        _gridSystemXY = gridSystemXY;
    }

    public List<Vector2Int> GetOccupiesGridPositionList() // Получить список занимаемых позиций в сетке. 
    {
        return _placedObjectTypeSO.GetGridPositionList(_gridPositioAnchor); // (в аргумент передадим его актуальную сеточную позицию и направлени)
    }


    public List<Vector2Int> GetTryOccupiesGridPositionList(Vector2Int gridPosition) //Получить список позиций в сетке, которые пытается занять. (в аргумент передадим сеточную позицию где хотим разместить)
    {
        return _placedObjectTypeSO.GetGridPositionList(gridPosition);
    }
       

    /*public override string ToString()
    {
        return _placedObjectTypeSO.nameString;
    }*/

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return _placedObjectTypeSO;
    }

    public Vector3 GetStartPosition()
    {
        return _startPosition;
    }

    public void SetStartPosition(Vector3 startPosition)
    {
        _startPosition = startPosition;
    }

    public void SetMoveStartPosition(bool moveStartPosition) // Установить надо перемещаться в стартовую позицию
    {
        _moveStartPosition = moveStartPosition;
    }    

    public virtual void DestroySelf() // Уничтожить себя
    {
        Destroy(gameObject);
    }

    public BaseAction GetBaseAction() // Получить Базовое действие предмета 
    {
        return _baseAction;
    }

    public List<GridName> GetCanPlacedOnGridList() //Получить Сетки где можно разместить наш объект
    {
        return _canPlacedOnGridList;
    }    
}
