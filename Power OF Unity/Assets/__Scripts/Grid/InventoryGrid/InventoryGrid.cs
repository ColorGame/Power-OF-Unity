using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Сетка инверторя. В каждой ячейки создается GridObjectInventoryXY который хранит информ о размещенном на сетке объекте
/// </summary>
/// <remarks>
/// !!!Обращаться к InventoryGrid ЖЕЛАТЕЛЬНО через класс PickUpDropPlacedObject
/// </remarks>
public class InventoryGrid : MonoBehaviour
{

    private static float cellSize;  // Размер ячейки

    [SerializeField] private InventoryGridParameters[] _gridParametersArray; // Массив параметров сеток ЗАДАТЬ в ИНСПЕКТОРЕ
    [SerializeField] private Transform _gridDebugObjectPrefab; // Префаб отладки сетки 

    private TooltipUI _tooltipUI;  
    private Canvas _canvas;
    private List<PlacedObject> _placedObjectList = new List<PlacedObject>(); // Список размещенных объектов    
    private List<GridSystemXY<GridObjectInventoryXY>> _gridSystemXYList; //Список сеточнах систем .В дженерик предаем тип GridObjectInventoryXY    



    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();          
    }

    public void Init(TooltipUI tooltipUI)
    {       
        _tooltipUI = tooltipUI;

        Setup();
    }
   
    private void Setup()
    {       
        cellSize = _gridParametersArray[0].cellSize;// Для всех сеток масштаб ячейки одинвковый 

        _gridSystemXYList = new List<GridSystemXY<GridObjectInventoryXY>>(); // Инициализируем список              

        foreach (InventoryGridParameters gridParameters in _gridParametersArray)
        {
            GridSystemXY<GridObjectInventoryXY> gridSystem = new GridSystemXY<GridObjectInventoryXY>(_canvas.scaleFactor, gridParameters,   // ПОСТРОИМ СЕТКУ  и в каждой ячейки создадим объект типа GridObjectInventoryXY
                (GridSystemXY<GridObjectInventoryXY> g, Vector2Int gridPosition) => new GridObjectInventoryXY(g, gridPosition)); //в четвертом параметре аргумента зададим функцию ананимно через лямбду => new GridObjectUnitXZ(g, _gridPositioAnchor) И ПЕРЕДАДИМ ЕЕ ДЕЛЕГАТУ. (лямбда выражение можно вынести в отдельный метод)

            //gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // Создадим наш префаб в каждой ячейки
            _gridSystemXYList.Add(gridSystem); // Добавим в список созданную сетку          
        }


        // Задодим размер и тайлинг материала заднего фона

        //_background.localPosition = new Vector3(_widthBag / 2f, _heightBag / 2f, 0) * _cellSizeWithScaleFactor; // разместим задний фон посередине
        // _background.localScale = new Vector3(_widthBag, _heightBag, 0) * _cellSizeWithScaleFactor;
        //_background.GetComponent<Material>().mainTextureScale = new Vector2(_widthBag, _heightBag);// Сдеалем количество тайлов равным количеству ячеек по высоте и ширине   

        /* foreach (InventoryGridParameters gridParameters in _gridParametersArray)
         {
             RectTransform bagBackground = (RectTransform)gridParameters.anchorGridTransform.GetChild(0); //Получим задний фон сетки
             bagBackground.sizeDelta = new Vector2(gridParameters.width, gridParameters.height);
             bagBackground.localScale = Vector3.one * gridParameters.cellSize;
             bagBackground.GetComponent<RawImage>().uvRect = new Rect(0, 0, gridParameters.width, gridParameters.height);   // Сдеалем количество тайлов равным количеству ячеек по высоте и ширине   
         }*/
    }

    /// <summary>
    /// Попробуем, для заданной позиции Мыши, получим сеточную систему на Кэнвасе и в случае удачи вернем ее и сеточную позицию
    /// </summary>   
    public bool TryGetGridSystemGridPosition(Vector3 mousePosition, out GridSystemXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int mouseGridPosition)
    {
        gridSystemXY = null;
        mouseGridPosition = new Vector2Int();

        foreach (GridSystemXY<GridObjectInventoryXY> localGridSystem in _gridSystemXYList) // перберем список сеток
        {
            Vector2Int testGridPositionMouse = localGridSystem.GetGridPosition(mousePosition); //для данной localGridSystem получим сеточную позицию мышки

            if (localGridSystem.IsValidGridPosition(testGridPositionMouse)) // Если тестируемая сеточная позиция допустима, вернем ее и саму сеточную систему
            {
                gridSystemXY = localGridSystem;
                mouseGridPosition = testGridPositionMouse;
                break;
            }
        }
        //Способ для Canvas который в плоскости экрана, через функ. RectTransformUtility.ScreenPointToLocalPointInRectangle
        /*foreach (InventoryGridParameters inventoryGridParameter in _gridParametersArray)
        {
            Vector2 localPositionMouse;
            RectTransform rectTransformGrid = (RectTransform)inventoryGridParameter.anchorGridTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformGrid, mousePosition, camera, out localPositionMouse)) // Если мы попали на трансформ сетки, то вернем локальную позицию мыши на этом трансформе
            {
                gridSystemXY = GetGridSystemXY(inventoryGridParameter.slot);
                mouseGridPosition = gridSystemXY.GetGridPosition(localPositionMouse); // !!! НЕЛЬЗЯ ПЕРЕДОВАТЬ ЛОКАЛЬНЫЕ КООРДИНАТЫ
                break;
            }
        }*/

        if (gridSystemXY == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Попробую Добавить Размещаемый объект в Позицию Сетки
    /// </summary>   
    /// <remarks>Верну false если - Разместить нельзя. Если могу то добавлю  Размещаемый объект в GridObjectInventoryXY</remarks>
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionPlace, PlacedObject placedObject, GridSystemXY<GridObjectInventoryXY> gridSystemXY)
    {
        List<Vector2Int> gridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositionPlace); // Получим список сеточных позиций которые хочет занять объект
        bool canPlace = true;
        foreach (Vector2Int gridPosition in gridPositionList) // Сначало надо проверить каждую ячейку
        {
            if (!gridSystemXY.IsValidGridPosition(gridPosition)) // Если есть хоть одна НЕ допустимая позиция то 
            {
                canPlace = false; // Разместить нельзя
                break;
            }
            if (gridSystemXY.GetGridObject(gridPosition).HasPlacedObject()) // Если хоть на одной позиции уже есть размещенный объект то
            {
                canPlace = false; // Разместить нельзя
                break; //break завершит цикл полностью, continue просто пропустит текущую итерацию.
            }
        }

        if (canPlace)//Если Можем разместить на сетке
        {
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectInventoryXY который находится в gridPosition
                gridObject.AddPlacedObject(placedObject); // Добавить Размещаемый объект 
            }

            placedObject.Drop();  // Бросить
            placedObject.SetGridPositionAnchor(gridPositionPlace); // Установим новую сеточную позицию якоря
            placedObject.SetGridSystemXY(gridSystemXY); //Установим сетку на которую добавили наш оббъект

            _placedObjectList.Add(placedObject);
        }
        return canPlace;
    }
    /// <summary>
    /// Удаление Размещаемый объект из сетки (предпологается что он уже размещен в сетке)
    /// </summary>
    public void RemovePlacedObjectAtGrid(PlacedObject placedObject)
    {
        List<Vector2Int> gridPositionList = placedObject.GetOccupiesGridPositionList(); // Получим список сеточных позиций которые занимает объект
        GridSystemXY<GridObjectInventoryXY> gridSystemXY = placedObject.GetGridSystemXY(); // Получим сетку на которой он размещен 
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectInventoryXY для сетки в которой он находится
            gridObject.RemovePlacedObject(); // удалим Размещаемый объект 
        }
        _placedObjectList.Remove(placedObject);
    }

    /// <summary>
    /// Очистить инвентарь и удалить размещенные предметы
    /// </summary>
    public void ClearInventoryGridAndDestroyPlacedObjects()
    {
        for (int i = 0; i < _gridSystemXYList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // и высоту
                {
                    GridObjectInventoryXY gridObject = _gridSystemXYList[i].GetGridObject(new Vector2Int(x,y)); // Получим GridObjectInventoryXY для сетки в которой он находится
                    gridObject.RemovePlacedObject(); // удалим Размещаемый объект 
                }
            }
        }

        foreach (PlacedObject placedObject in _placedObjectList) 
        {           
            Destroy(placedObject.gameObject);
        }
        _placedObjectList.Clear();
    }

    public List<GridSystemXY<GridObjectInventoryXY>> GetGridSystemXYList() { return _gridSystemXYList; }

    // Получить сеточную позицию
    public Vector2Int GetGridPosition(Vector3 worldPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetGridPosition(worldPosition);

    /// <summary>
    /// Получим мировые координаты центра ячейки 
    /// </summary>    
    public Vector3 GetWorldPositionCenterСornerCell(Vector2Int gridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionCenterСornerCell(gridPosition) ;

    /// <summary>
    /// Получим мировые координаты центра Сетки 
    /// </summary>
    public Vector3 GetWorldPositionGridCenter(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionGridCenter();

    /// <summary>
    /// Получим мировые координаты нижнего левого угола ячейки 
    /// </summary>
    public Vector3 GetWorldPositionLowerLeftСornerCell(Vector2Int gridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionLowerLeftСornerCell(gridPosition);

    public Vector3 GetRotationAnchorGrid(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetRotationAnchorGrid();
    public Transform GetAnchorGrid(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetAnchorGrid();
    public int GetWidth(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWidth();
    public int GetHeight(GridSystemXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetHeight();
    public Canvas GetCanvas() { return _canvas; }
    public static float GetCellSize() { return cellSize ; }// (static обозначает что метод принадлежит классу а не кокому нибудь экземпляру)

    public GridSystemXY<GridObjectInventoryXY> GetGridSystemXY(InventorySlot inventorySlot) // Получить сетку для этого слота
    {
        foreach (GridSystemXY<GridObjectInventoryXY> localGridSystem in _gridSystemXYList) // перберем список сеток
        {
            if (localGridSystem.GetGridSlot() == inventorySlot)
            {
                return localGridSystem;
            }
        }

        return null;
    }

    /*
        public void Save()
        {
            _placedObjectList = new List<PlacedObjectParameters>();
            for (int i = 0; i < _gridSystemXYList.Count; i++) // переберем все сетки
            {
                for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
                {
                    for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // и высоту
                    {
                        Vector2Int gridPosition = new Vector2Int(x, y);
                        GridObjectInventoryXY gridObject = _gridSystemXYList[i].GetGridObject(gridPosition); // Получим сеточный объект для нашей позиции

                        if (gridObject.HasPlacedObject()) // Если в этой позиции есть размещенный объект то 
                        {
                            PlacedObjectParameters placedObjectParameters = new PlacedObjectParameters(
                                slot = _gridSystemXYList[i]
                                );
                            if (!_placedObjectList.Contains(gridObject.GetPlacedObject())) // Если этот объект еще не содержиться в списке то добавим его (PlacedObject может одновременно размещаться на нескольких GridObject, поэтому нужна проверка)
                            {
                                _placedObjectList.Add(gridObject.GetPlacedObject());
                            }
                        }
                    }
                }
            }

            // Создадим Список Добавленных Размещенных объектов
            List<PlacedObjectParameters> addPlacedObjectList = new List<PlacedObjectParameters>();
            foreach (PlacedObjectParameters placedObject in placedObjectList)
            {
                addPlacedObjectList.Add(new AddPlacedObject
                {
                    gridName = placedObject.GetGridSystemXY().GetGridSlot(),
                    gridPositioAnchor = placedObject.GetGridPositionAnchor(),
                    placedObject = placedObject.GetPlacedObjectTypeSO(),
                });
            }

            string json = JsonUtility.ToJson(new ListAddPlacedObject { addPlacedObjectList = addPlacedObjectList });

            PlayerPrefs.SetString("InventoryGridSystemSave", json); // Устанавливает единственное строковое значение для предпочтения, определяемого данным ключом. Вы можете использовать PlayerPrefs.getString для получения этого значени
            SaveSystem.Save("InventoryGridSystemSave", json, true); // Сохраним и перепишем последнее

            Debug.Log("Save!");
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey("InventoryGridSystemSave")) // Возвращает true, если заданное значение key существует в PlayerPrefs, в противном случае возвращает false.
            {
                string json = PlayerPrefs.GetString("InventoryGridSystemSave");
                json = SaveSystem.Load("InventoryGridSystemSave");

                ListAddPlacedObject listAddPlacedObject = JsonUtility.FromJson<ListAddPlacedObject>(json); // Загрузим список Добавленных Размещенных объектов

                foreach (AddPlacedObject addPlacedObject in listAddPlacedObject.addPlacedObjectList) // Переберем каждый Добавленный Размещенный объект в списке
                {
                    // Создадим и разместим сохраненный объект               
                    GridSystemXY<GridObjectInventoryXY> gridSystemXY = GetGridSystemXY(addPlacedObject.gridName); // Получим сетку для размещения
                    PlacedObject placedObject = PlacedObject.CreateInGrid(gridSystemXY, addPlacedObject.gridPositioAnchor, addPlacedObject.placedObject, _canvasInventory, this);
                    if (!_pickUpDropPlacedObject.TryDrop(gridSystemXY, addPlacedObject.gridPositioAnchor, placedObject)) // Если не удалось сбросить объект на сетку то
                    {
                        placedObject.DestroySelf(); // Уничтожим этот объект
                        _tooltipUI.ShowShortTooltipFollowMouse("не удалось загрузить сохранение", new TooltipUI.TooltipTimer { timer = 3f }); // Покажем подсказку и зададим новый таймер отображения подсказки                   
                    }
                }
            }
            Debug.Log("Load!");
        }*/


    /*public InventoryGridParameters[] GetGridParametersArray() //Получить Массив параметров сеток
    {
        return _gridParametersArray;
    }

    public PlacedObject GetPlacedObjectAtGridPosition(Vector2Int gridPosition, GridSystemXY<GridObjectInventoryXY> gridSystemXY) // Получить Размещаемый объект в этой сеточной позиции
    {
        GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectInventoryXY который находится в gridPositionPlace
        return gridObject.GetPlacedObject();
    }*/
}
