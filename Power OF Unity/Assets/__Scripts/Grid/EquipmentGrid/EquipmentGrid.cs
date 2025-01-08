using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Сетка экипировки юнита (оружия и брони). В каждой ячейки создается GridObjectEquipmentXY который хранит информ о размещенном на сетке объекте
/// </summary>
/// <remarks>
/// !!!Обращаться к EquipmentGrid ЖЕЛАТЕЛЬНО через класс PickUpDropPlacedObject/// 
/// </remarks>
public class EquipmentGrid : MonoBehaviour
{
    public enum GridState
    {
        ItemGrid,
        ArmorGrid
    }

    [SerializeField] private Canvas _canvasItemGrid;
    [SerializeField] private Canvas _canvasArmorGrid;
    [SerializeField] private EquipmentGridParameters[] _itemGridParametersArray; // Массив параметров сеток предметов
    [SerializeField] private EquipmentGridParameters[] _armorGridParametersArray; // Массив параметров сеток брони

    public event EventHandler<PlacedObject> OnAddInEquipmentGrid; // Объект добавлен в сетку Интвенторя
    public event EventHandler<PlacedObject> OnRemoveFromEquipmentGridAndHung; // Объект удален из сетки Интвенторя и повис над ней 
    public event EventHandler<PlacedObject> OnRemoveFromEquipmentGridAndMoveStartPosition; // Объект удален из сетки Интвенторя и ДВИЖЕтся в стартовую позиция

    private TooltipUI _tooltipUI;
    private List<GridSystemXY<GridObjectEquipmentXY>> _itemGridSystemXYList; //Список сеточнах систем .В дженерик предаем тип GridObjectEquipmentXY    
    private List<GridSystemXY<GridObjectEquipmentXY>> _armorGridSystemXYList; //Список сеточнах систем .В дженерик предаем тип GridObjectEquipmentXY    

    // активные списки с которыми работает скрипт 
    private GridState _gridState; // хэшированное состояние сетки
    private List<PlacedObject> _placedObjectActiveList = new();
    private List<GridSystemXY<GridObjectEquipmentXY>> _gridSystemActiveList;



    public void Init(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        Setup();
    }

    private void Setup()
    {
        _itemGridSystemXYList = InitEquipmentGrid(_itemGridParametersArray, _canvasItemGrid);
        _armorGridSystemXYList = InitEquipmentGrid(_armorGridParametersArray, _canvasArmorGrid);

        SetActiveGrid(GridState.ItemGrid);
    }

    private List<GridSystemXY<GridObjectEquipmentXY>> InitEquipmentGrid(EquipmentGridParameters[] gridParametersArray, Canvas canvas)
    {
        List<GridSystemXY<GridObjectEquipmentXY>> gridSystemXYList = new List<GridSystemXY<GridObjectEquipmentXY>>(); // Инициализируем список              

        foreach (EquipmentGridParameters gridParameters in gridParametersArray)
        {
            GridSystemXY<GridObjectEquipmentXY> gridSystem = new GridSystemXY<GridObjectEquipmentXY>(canvas.scaleFactor, gridParameters,   // ПОСТРОИМ СЕТКУ  и в каждой ячейки создадим объект типа GridObjectEquipmentXY
                (GridSystemXY<GridObjectEquipmentXY> g, Vector2Int gridPosition) => new GridObjectEquipmentXY(g, gridPosition)); //в четвертом параметре аргумента зададим функцию ананимно через лямбду => new GridObjectUnitXZ(g, _gridPositioAnchor) И ПЕРЕДАДИМ ЕЕ ДЕЛЕГАТУ. (лямбда выражение можно вынести в отдельный метод)

            gridSystemXYList.Add(gridSystem); // Добавим в список созданную сетку          
        }
        return gridSystemXYList;
    }
    /// <summary>
    /// Установить активную сетку
    /// </summary>
    public void SetActiveGrid(GridState stateGrid)
    {
        _gridState = stateGrid;
        switch (_gridState)
        {
            case GridState.ItemGrid:
                _gridSystemActiveList = _itemGridSystemXYList;
                break;

            case GridState.ArmorGrid:
                _gridSystemActiveList = _armorGridSystemXYList;
                break;
        }
    }

    /// <summary>
    /// Попробуем, для заданной позиции Мыши, получим сеточную систему на Кэнвасе и в случае удачи вернем ее и сеточную позицию
    /// </summary>   
    public bool TryGetGridSystemGridPosition(Vector3 mousePosition, out GridSystemXY<GridObjectEquipmentXY> gridSystemXY, out Vector2Int mouseGridPosition)
    {
        gridSystemXY = null;
        mouseGridPosition = new Vector2Int();

        foreach (GridSystemXY<GridObjectEquipmentXY> localGridSystem in _gridSystemActiveList) // перберем активный список сеток
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
        /*foreach (EquipmentGridParameters equipmentGridParameter in _itemGridParametersArray)
        {
            Vector2 localPositionMouse;
            RectTransform rectTransformGrid = (RectTransform)equipmentGridParameter.anchorGridTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformGrid, mousePosition, camera, out localPositionMouse)) // Если мы попали на трансформ сетки, то вернем локальную позицию мыши на этом трансформе
            {
                gridSystemXY = GetActiveGridSystemXY(equipmentGridParameter.slot);
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
    /// <remarks>Верну false если - Разместить нельзя. Если могу то добавлю  Размещаемый объект в GridObjectEquipmentXY</remarks>
    public bool TryAddPlacedObjectAtGridPosition(Vector2Int gridPositionPlace, PlacedObject placedObject, GridSystemXY<GridObjectEquipmentXY> gridSystemXY)
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
                GridObjectEquipmentXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectEquipmentXY который находится в gridPosition
                gridObject.AddPlacedObject(placedObject); // Добавить Размещаемый объект 
            }

            placedObject.Drop();  // Бросить
            placedObject.SetGridPositionAnchor(gridPositionPlace); // Установим новую сеточную позицию якоря
            placedObject.SetGridSystemXY(gridSystemXY); //Установим сетку на которую добавили наш оббъект

            _placedObjectActiveList.Add(placedObject);

            OnAddInEquipmentGrid?.Invoke(this, placedObject); // Запустим событие 
        }
        return canPlace;
    }
    /// <summary>
    /// Удаление Размещаемый объект из сетки (предпологается что он уже размещен в сетке)
    /// </summary>
    public void RemovePlacedObjectAtGrid(PlacedObject placedObject, bool moveStartPosition = false)
    {
        List<Vector2Int> gridPositionList = placedObject.GetOccupiesGridPositionList(); // Получим список сеточных позиций которые занимает объект
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY = placedObject.GetGridSystemXY(); // Получим сетку на которой он размещен 
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            GridObjectEquipmentXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectEquipmentXY для сетки в которой он находится
            gridObject.RemovePlacedObject(); // удалим Размещаемый объект 
        }
        _placedObjectActiveList.Remove(placedObject);


        if (moveStartPosition)
            OnRemoveFromEquipmentGridAndMoveStartPosition?.Invoke(this, placedObject); // Запустим событие
        else
            OnRemoveFromEquipmentGridAndHung?.Invoke(this, placedObject); // Запустим событие
    }

    /// <summary>
    /// Очистить экипировка и удалить размещенные предметы
    /// </summary>
    public void ClearEquipmentGridAndDestroyPlacedObjects()
    {
        for (int i = 0; i < _gridSystemActiveList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemActiveList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemActiveList[i].GetHeight(); y++)  // и высоту
                {
                    GridObjectEquipmentXY gridObject = _gridSystemActiveList[i].GetGridObject(new Vector2Int(x, y)); // Получим GridObjectEquipmentXY для сетки в которой он находится
                    gridObject.RemovePlacedObject(); // удалим Размещаемый объект 
                }
            }
        }

        foreach (PlacedObject placedObject in _placedObjectActiveList)
        {
            Destroy(placedObject.gameObject);
        }
        _placedObjectActiveList.Clear();
    }

    public List<GridSystemXY<GridObjectEquipmentXY>> GetItemGridSystemXYList() { return _itemGridSystemXYList; }

    /// <summary>
    /// Попробую вернуть размещенный объект по типу <T>
    /// </summary>
    public bool TryGetPlacedObjectByType<T>(out PlacedObject placedObjectTypeT) where T : PlacedObjectTypeSO
    {
        foreach (PlacedObject placedObject in _placedObjectActiveList)
        {
            if (placedObject.GetPlacedObjectTypeSO() is T)
            {
                placedObjectTypeT = placedObject;
                return true;
            }
        }
        placedObjectTypeT = null;
        return false;
    }

    // Получить сеточную позицию
    public Vector2Int GetGridPosition(Vector3 worldPosition, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) => gridSystemXY.GetGridPosition(worldPosition);

    /// <summary>
    /// Получим мировые координаты центра ячейки 
    /// </summary>    
    public Vector3 GetWorldPositionCenterСornerCell(Vector2Int gridPosition, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) => gridSystemXY.GetWorldPositionCenterСornerCell(gridPosition);

    /// <summary>
    /// Получим мировые координаты центра Сетки 
    /// </summary>
    public Vector3 GetWorldPositionGridCenter(GridSystemXY<GridObjectEquipmentXY> gridSystemXY) => gridSystemXY.GetWorldPositionGridCenter();

    /// <summary>
    /// Получим мировые координаты нижнего левого угола ячейки 
    /// </summary>
    public Vector3 GetWorldPositionLowerLeftСornerCell(Vector2Int gridPosition, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) => gridSystemXY.GetWorldPositionLowerLeftСornerCell(gridPosition);

    public Vector3 GetRotationAnchorGrid(GridSystemXY<GridObjectEquipmentXY> gridSystemXY) => gridSystemXY.GetRotationAnchorGrid();
    public Transform GetAnchorGrid(GridSystemXY<GridObjectEquipmentXY> gridSystemXY) => gridSystemXY.GetAnchorGrid();

    /// <summary>
    /// Получить АКТИВНКЮ сетку для этого слота
    /// </summary><remarks>Если нет активных вернет null</remarks>
    public GridSystemXY<GridObjectEquipmentXY> GetActiveGridSystemXY(EquipmentSlot equipmentSlot)
    {
        foreach (GridSystemXY<GridObjectEquipmentXY> localGridSystem in _gridSystemActiveList) // перберем активный список сеток
        {
            if (localGridSystem.GetGridSlot() == equipmentSlot)
            {
                return localGridSystem;
            }
        }
        return null;
    }

    /// <summary>
    /// Получить сетку для этого слота 
    /// </summary><remarks>(!!СЕТКА МОЖЕТ БЫТЬ НЕ АКТИВНА)</remarks>
    public GridSystemXY<GridObjectEquipmentXY> GetGridSystemXY(EquipmentSlot equipmentSlot)
    {
        foreach (GridSystemXY<GridObjectEquipmentXY> localGridSystem in _itemGridSystemXYList) // перберем список сеток предметов
        {
            if (localGridSystem.GetGridSlot() == equipmentSlot)
            {
                return localGridSystem;
            }
        }
        foreach (GridSystemXY<GridObjectEquipmentXY> localGridSystem in _armorGridSystemXYList) // перберем список сеток брони
        {
            if (localGridSystem.GetGridSlot() == equipmentSlot)
            {
                return localGridSystem;
            }
        }
        return null;
    }

    public Canvas GetCanvasItemGrid() { return _canvasItemGrid; }
    public Canvas GetCanvasArmorGrid() { return _canvasArmorGrid; }
    public GridState GetStateGrid() { return _gridState; }
    /*
        public void Save()
        {
            _itemPlacedObjectList = new List<PlacedObjectGridParameters>();
            for (int i = 0; i < _itemGridSystemXYList.Count; i++) // переберем все сетки
            {
                for (int x = 0; x < _itemGridSystemXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
                {
                    for (int y = 0; y < _itemGridSystemXYList[i].GetHeight(); y++)  // и высоту
                    {
                        Vector2Int gridPosition = new Vector2Int(x, y);
                        GridObjectEquipmentXY gridObject = _itemGridSystemXYList[i].GetGridObject(gridPosition); // Получим сеточный объект для нашей позиции

                        if (gridObject.HasPlacedObject()) // Если в этой позиции есть размещенный объект то 
                        {
                            PlacedObjectGridParameters placedObjectParameters = new PlacedObjectGridParameters(
                                slot = _itemGridSystemXYList[i]
                                );
                            if (!_itemPlacedObjectList.Contains(gridObject.GetPlacedObject())) // Если этот объект еще не содержиться в списке то добавим его (PlacedObject может одновременно размещаться на нескольких GridObject, поэтому нужна проверка)
                            {
                                _itemPlacedObjectList.Add(gridObject.GetPlacedObject());
                            }
                        }
                    }
                }
            }

            // Создадим Список Добавленных Размещенных объектов
            List<PlacedObjectGridParameters> addPlacedObjectList = new List<PlacedObjectGridParameters>();
            foreach (PlacedObjectGridParameters placedObject in placedObjectList)
            {
                addPlacedObjectList.Add(new AddPlacedObject
                {
                    gridName = placedObject.GetActiveGridSystemXY().GetGridSlot(),
                    gridPositioAnchor = placedObject.GetGridPositionAnchor(),
                    placedObject = placedObject.GetPlacedObjectTypeSO(),
                });
            }

            string json = JsonUtility.ToJson(new ListAddPlacedObject { addPlacedObjectList = addPlacedObjectList });

            PlayerPrefs.SetString("EquipmentGridSystemSave", json); // Устанавливает единственное строковое значение для предпочтения, определяемого данным ключом. Вы можете использовать PlayerPrefs.getString для получения этого значени
            SaveSystem.Save("EquipmentGridSystemSave", json, true); // Сохраним и перепишем последнее

            Debug.Log("Save!");
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey("EquipmentGridSystemSave")) // Возвращает true, если заданное значение key существует в PlayerPrefs, в противном случае возвращает false.
            {
                string json = PlayerPrefs.GetString("EquipmentGridSystemSave");
                json = SaveSystem.Load("EquipmentGridSystemSave");

                ListAddPlacedObject listAddPlacedObject = JsonUtility.FromJson<ListAddPlacedObject>(json); // Загрузим список Добавленных Размещенных объектов

                foreach (AddPlacedObject addPlacedObject in listAddPlacedObject.addPlacedObjectList) // Переберем каждый Добавленный Размещенный объект в списке
                {
                    // Создадим и разместим сохраненный объект               
                    GridSystemXY<GridObjectEquipmentXY> gridSystemXY = GetActiveGridSystemXY(addPlacedObject.gridName); // Получим сетку для размещения
                    PlacedObject placedObject = PlacedObject.CreateInGrid(gridSystemXY, addPlacedObject.gridPositioAnchor, addPlacedObject.placedObject, _canvasPickUpDrop, this);
                    if (!_pickUpDropPlacedObject.TryDrop(gridSystemXY, addPlacedObject.gridPositioAnchor, placedObject)) // Если не удалось сбросить объект на сетку то
                    {
                        placedObject.DestroySelf(); // Уничтожим этот объект
                        _tooltipUI.ShowShortTooltipFollowMouse("не удалось загрузить сохранение", new TooltipUI.TooltipTimer { timer = 3f }); // Покажем подсказку и зададим новый таймер отображения подсказки                   
                    }
                }
            }
            Debug.Log("Load!");
        }*/


    /*public EquipmentGridParameters[] GetGridParametersArray() //Получить Массив параметров сеток
    {
        return _itemGridParametersArray;
    }

    public PlacedObject GetPlacedObjectAtGridPosition(Vector2Int gridPosition, GridSystemXY<GridObjectEquipmentXY> gridSystemXY) // Получить Размещаемый объект в этой сеточной позиции
    {
        GridObjectEquipmentXY gridObject = gridSystemXY.GetGridObject(gridPosition); // Получим GridObjectEquipmentXY который находится в gridPositionPlace
        return gridObject.GetPlacedObject();
    }*/
}
