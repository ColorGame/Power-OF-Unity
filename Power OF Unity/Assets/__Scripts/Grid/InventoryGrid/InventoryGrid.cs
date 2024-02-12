using System;
using System.Collections.Generic;
using UnityEngine;


public class InventoryGrid : MonoBehaviour // Сетка инверторя
{
    public static InventoryGrid Instance { get; private set; }

    [SerializeField] private GridParameters[] _gridParametersArray; // Массив параметров сеток ЗАДАТЬ в ИНСПЕКТОРЕ
    [SerializeField] private Transform _gridDebugObjectPrefab; // Префаб отладки сетки 

    private List<GridSystemTiltedXY<GridObjectInventoryXY>> _gridSystemTiltedXYList; //Список сеточнах систем .В дженерик предаем тип GridObjectInventoryXY    

    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one InventoryGrid!(Там больше, чем один InventoryGrid!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр InventoryGrid прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;

        _gridSystemTiltedXYList = new List<GridSystemTiltedXY<GridObjectInventoryXY>>(); // Инициализируем список              

        foreach (GridParameters gridParameters in _gridParametersArray)
        {
            GridSystemTiltedXY<GridObjectInventoryXY> gridSystem = new GridSystemTiltedXY<GridObjectInventoryXY>(gridParameters,   // ПОСТРОИМ СЕТКУ  и в каждой ячейки создадим объект типа GridObjectInventoryXY
                (GridSystemXY<GridObjectInventoryXY> g, Vector2Int gridPosition) => new GridObjectInventoryXY(g, gridPosition)); //в четвертом параметре аргумента зададим функцию ананимно через лямбду => new GridObjectUnitXZ(g, _gridPositioAnchor) И ПЕРЕДАДИМ ЕЕ ДЕЛЕГАТУ. (лямбда выражение можно вынести в отдельный метод)

            //gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // Создадим наш префаб в каждой ячейки
            _gridSystemTiltedXYList.Add(gridSystem); // Добавим в список созданную сетку
        }
    }

    private void Start()
    {
        // Задодим размер и тайлинг материала заднего фона

        //_background.localPosition = new Vector3(_widthBag / 2f, _heightBag / 2f, 0) * _cellSize; // разместим задний фон посередине
        // _background.localScale = new Vector3(_widthBag, _heightBag, 0) * _cellSize;
        //_background.GetComponent<Material>().mainTextureScale = new Vector2(_widthBag, _heightBag);// Сдеалем количество тайлов равным количеству ячеек по высоте и ширине   

        /* foreach (GridParameters gridParameters in _gridParametersArray)
         {
             RectTransform bagBackground = (RectTransform)gridParameters.anchorGridTransform.GetChild(0); //Получим задний фон сетки
             bagBackground.sizeDelta = new Vector2(gridParameters.width, gridParameters.height);
             bagBackground.localScale = Vector3.one * gridParameters.cellSize;
             bagBackground.GetComponent<RawImage>().uvRect = new Rect(0, 0, gridParameters.width, gridParameters.height);   // Сдеалем количество тайлов равным количеству ячеек по высоте и ширине   
         }*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Load();
        }
    }

    public bool TryGetGridSystemGridPosition(Vector3 worldPositionMouse, out GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, out Vector2Int gridPositionMouse) //Попробуем Получим сеточную систему для заданной позиции Мыши и в случае удачи вернем ее и сеточную позицию
    {
        gridSystemXY = null;
        gridPositionMouse = new Vector2Int(0, 0);

        foreach (GridSystemTiltedXY<GridObjectInventoryXY> localGridSystem in _gridSystemTiltedXYList) // перберем список сеток
        {
            Vector2Int testGridPositionMouse = localGridSystem.GetGridPosition(worldPositionMouse); //для данной localGridSystem получим сеточную позицию мышки

            if (localGridSystem.IsValidGridPosition(testGridPositionMouse)) // Если тестируемая сеточная позиция допустима, вернем ее и саму сеточную систему
            {
                gridSystemXY = localGridSystem;
                gridPositionMouse = testGridPositionMouse;
                break;
            }
        }

        if (gridSystemXY == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionMouse, PlacedObject placedObject, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY)//Попробую Добавить Размещаемый объект в Позицию Сетки
    {
        List<Vector2Int> gridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositionMouse); // Получим список сеточных позиций которые хочет занять объект
        bool canPlace = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!gridSystemXY.IsValidGridPosition(gridPosition)) // Если есть хоть одна НЕ допустимая позиция то 
            {
                canPlace = false; // Разместить нельзя
                break;
            }
            if (gridSystemXY.GetGridObject(gridPosition).HasPlacedObject()) // Если хоть на одной позиции уже есть размещенный объект то
            {
                canPlace = false; // Разместить нельзя
                break;
            }
        }

        if (canPlace)//Если Можем разместить на сетке
        {
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectInventoryXY который находится в gridPosition
                gridObject.AddPlacedObject(placedObject); // Добавить Размещаемый объект 
            }
            return true;
        }
        else
        {
            return false;
        }
    }   

    public void RemovePlacedObjectAtGrid(PlacedObject placedObject) // Удаление Размещаемый объект из сетки (предпологается что он уже размещен в сетке)
    {
        List<Vector2Int> gridPositionList = placedObject.GetOccupiesGridPositionList(); // Получим список сеточных позиций которые занимает объект
        GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY = placedObject.GetGridSystemXY(); // Получим сетку на которой он размещен 
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectInventoryXY для сетки в которой он находится
            gridObject.RemovePlacedObject(placedObject); // удалим Размещаемый объект 
        }
    }

    public List<GridSystemTiltedXY<GridObjectInventoryXY>> GetGridSystemTiltedXYList()
    {
        return _gridSystemTiltedXYList;
    }

    // Получить сеточную позицию
    public Vector2Int GetGridPosition(Vector3 worldPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetGridPosition(worldPosition);

    // Получим мировые координаты центра ячейки (относительно  нашей ***GridTransform)
    public Vector3 GetWorldPositionCenterСornerCell(Vector2Int gridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionCenterСornerCell(gridPosition);

    //  Получим мировые координаты центра Сетки (относительно  нашей ***GridTransform)
    public Vector3 GetWorldPositionGridCenter(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionGridCenter();

    // Получим мировые координаты нижнего левого угола ячейки (относительно  нашей ***GridTransform)
    public Vector3 GetWorldPositionLowerLeftСornerCell(Vector2Int gridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWorldPositionLowerLeftСornerCell(gridPosition);

    public Vector3 GetRotationAnchorGrid(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetRotationAnchorGrid();
    public Transform GetAnchorGrid(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetAnchorGrid();
    public int GetWidth(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetWidth();
    public int GetHeight(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) => gridSystemXY.GetHeight();
    public float GetCellSize() => _gridSystemTiltedXYList[0].GetCellSize(); // Для всех сеток масштаб ячейки одинвковый    

    public GridSystemTiltedXY<GridObjectInventoryXY> GetGridSystemTiltedXY(GridName gridName) // Получить сетку по имени
    {
        foreach (GridSystemTiltedXY<GridObjectInventoryXY> localGridSystem in _gridSystemTiltedXYList) // перберем список сеток
        {
            if (localGridSystem.GetGridName() == gridName)
            {
                return localGridSystem;
            }
        }

        return null;
    }


    [Serializable]
    public struct AddPlacedObject // Добаленный Размещенный объект
    {
        public GridName gridName; //Имя Сетка на которой добавили размещенный объект
        public Vector2Int gridPositioAnchor; // Сеточная позиция Якоря
        public PlacedObjectTypeSO placedObjectTypeSO;
    }

    [Serializable]
    public struct ListAddPlacedObject // Список Добавленных Размещенных объектов
    {
        public List<AddPlacedObject> addPlacedObjectList;
    }

    public void Save()
    {
        List<PlacedObject> placedObjectList = new List<PlacedObject>();
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // и высоту
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    GridObjectInventoryXY gridObject = _gridSystemTiltedXYList[i].GetGridObject(gridPosition); // Получим сеточный объект для нашей позиции

                    if (gridObject.HasPlacedObject()) // Если в этой позиции есть размещенный объект то 
                    {
                        if (!placedObjectList.Contains(gridObject.GetPlacedObject())) // Если этот объект еще не содержиться в списке то добавим его (PlacedObject может одновременно размещаться на нескольких GridObject, поэтому нужна проверка)
                        {
                            placedObjectList.Add(gridObject.GetPlacedObject());
                        }
                    }
                }
            }
        }

        // Создадим Список Добавленных Размещенных объектов
        List<AddPlacedObject> addPlacedObjectList = new List<AddPlacedObject>();
        foreach (PlacedObject placedObject in placedObjectList)
        {
            addPlacedObjectList.Add(new AddPlacedObject
            {
                gridName = placedObject.GetGridSystemXY().GetGridName(),
                gridPositioAnchor = placedObject.GetGridPositionAnchor(),
                placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO(),
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
                Transform parentCanvas = PickUpDrop.Instance.GetCanvasInventoryWorld();
                GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY = GetGridSystemTiltedXY(addPlacedObject.gridName); // Получим сетку для размещения
                PlacedObject placedObject = PlacedObject.CreateInGrid(gridSystemXY, addPlacedObject.gridPositioAnchor, PlacedObjectTypeSO.Dir.Down, addPlacedObject.placedObjectTypeSO, parentCanvas);
                if (!PickUpDrop.Instance.TryDrop(gridSystemXY, addPlacedObject.gridPositioAnchor, placedObject)) // Если не удалось сбросить объект на сетку то
                {
                    placedObject.DestroySelf(); // Уничтожим этот объект
                    TooltipUI.Instance.Show("не удалось загрузить сохранение", new TooltipUI.TooltipTimer { timer = 3f }); // Покажем подсказку и зададим новый таймер отображения подсказки
                }
            }
        }
        Debug.Log("Load!");
    }


    /*public GridParameters[] GetGridParametersArray() //Получить Массив параметров сеток
    {
        return _gridParametersArray;
    }

    public PlacedObject GetPlacedObjectAtGridPosition(Vector2Int gridPosition, GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY) // Получить Размещаемый объект в этой сеточной позиции
    {
        GridObjectInventoryXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectInventoryXY который находится в gridPositionMouse
        return gridObject.GetPlacedObject();
    }*/
}
