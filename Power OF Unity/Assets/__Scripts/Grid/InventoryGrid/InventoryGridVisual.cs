using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Добавим InventoryGridVisual для запуска после времени по умолчанию, поскольку мы хотим, чтобы визуальные эффекты запускались после всего остального.
// (Project Settings/ Script Execution Order и поместим выполнение InventoryGridVisual НИЖЕ Default Time)
public class InventoryGridVisual : MonoBehaviour // Сеточная система визуализации инвенторя
{
    
    [Serializable] // Чтобы созданная структура могла отображаться в инспекторе
    public struct GridVisualTypeMaterial    //Визуал сетки Тип Материала // Создадим структуру можно в отдельном классе. Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#
    {                                       //В данной структуре объединям состояние сетки с материалом
        public GridVisualType gridVisualType;
        public Material materialGrid;
    }

    public enum GridVisualType //Визуальные состояния сетки
    {
        Grey,
        White,
        Blue,
        BlueSoft,
        Red,
        RedSoft,
        Yellow,
        YellowSoft,
        Green,
        GreenSoft,
    }

    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList; // Список тип материала визуального состояния сетки Квадрат (Список из кастомного типа данных) визуального состояния сетки // В инспекторе под каждое состояние перетащить соответствующий материал сетки

    private InventoryGridVisualSingle[][,] _inventoryGridSystemVisualSingleArray; // Массив масивов [количество сеток][длина (Х), высота (У)]
    private Dictionary<InventorySlot, int> _gridNameIndexDictionary; // Словарь (InventorySlot - ключ, int(индекс) -значение)
    private List<GridSystemTiltedXY<GridObjectInventoryXY>> _gridSystemTiltedXYList; // список сеток

    private PickUpDrop _pickUpDrop;
    private InventoryGrid _inventoryGrid;

    
    public void Init(PickUpDrop pickUpDrop, InventoryGrid inventoryGrid)
    {
        _pickUpDrop = pickUpDrop;
        _inventoryGrid = inventoryGrid;        
    }


    private void Start()
    {
        _gridNameIndexDictionary = new Dictionary<InventorySlot, int>();

        // Инициализируем сначала первую часть массива - Количество сеток
        _gridSystemTiltedXYList = _inventoryGrid.GetGridSystemTiltedXYList(); // Получим список сеток
        _inventoryGridSystemVisualSingleArray = new InventoryGridVisualSingle[_gridSystemTiltedXYList.Count][,];

        // Для каждой сетки реализуем двумерный массив координат
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++)
        {
            _inventoryGridSystemVisualSingleArray[i] = new InventoryGridVisualSingle[_gridSystemTiltedXYList[i].GetWidth(), _gridSystemTiltedXYList[i].GetHeight()];
        }


        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // и высоту
                {
                    Vector2Int gridPosition = new Vector2Int(x, y); // позиция сетки
                    Vector3 rotation = _inventoryGrid.GetRotationAnchorGrid(_gridSystemTiltedXYList[i]);
                    Transform AnchorGridTransform = _inventoryGrid.GetAnchorGrid(_gridSystemTiltedXYList[i]);

                    Transform gridSystemVisualSingleTransform = Instantiate(GameAssets.Instance.inventoryGridSystemVisualSinglePrefab, _inventoryGrid.GetWorldPositionCenterСornerCell(gridPosition, _gridSystemTiltedXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // Создадим наш префаб в каждой позиции сетки

                    _gridNameIndexDictionary[_gridSystemTiltedXYList[i].GetGridSlot()] = i; // Присвоим ключу(имя Сетки под этим индексом) значение (индекс массива)
                    _inventoryGridSystemVisualSingleArray[i][x, y] = gridSystemVisualSingleTransform.GetComponent<InventoryGridVisualSingle>(); // Сохраняем компонент LevelGridVisualSingle в трехмерный массив где x,y,y это будут индексы массива.
                }
            }
        }

        _pickUpDrop.OnAddPlacedObjectAtInventoryGrid += PickUpDropSystem_OnAddPlacedObjectAtGrid;
        _pickUpDrop.OnRemovePlacedObjectAtInventoryGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
        _pickUpDrop.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDrop.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;
    }

    // Захваченый объект покинул сетку
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)  // Захваченый объект покинул сетку
    {
        SetDefaultState(); // Установим дефолтное состояние всех сеток
    }

    // позиция захваченного объекта на сетке изменилась
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PickUpDrop.OnGrabbedObjectGridPositionChangedEventArgs e)
    {
        SetDefaultState(); // Установим дефолтное состояние всех сеток
        ShowPossibleGridPositions(e.gridSystemXY, e.placedObject, e.newMouseGridPosition, GridVisualType.Yellow); //показать возможные сеточные позиции
    }

    // Объект удален из сетки
    private void PickUpDropSystem_OnRemovePlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Yellow);
    }

    // Объект добавлен в сетку 
    private void PickUpDropSystem_OnAddPlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }

    private void SetDefaultState() // Установить дефолтное состояние сеток
    {
        for (int i = 0; i < _gridSystemTiltedXYList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemTiltedXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemTiltedXYList[i].GetHeight(); y++)  // и высоту
                {
                    if (!_inventoryGridSystemVisualSingleArray[i][x, y].GetIsBusy()) // Если позиция не занята
                    {
                        _inventoryGridSystemVisualSingleArray[i][x, y].Show(GetGridVisualTypeMaterial(GridVisualType.Grey));
                    }
                }
            }
        }
    }

    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {
        GridSystemTiltedXY<GridObjectInventoryXY> gridSystemTiltedXY = placedObject.GetGridSystemXY(); // Сеточная система которую занимает объект
        List<Vector2Int> OccupiesGridPositionList = placedObject.GetOccupiesGridPositionList(); // Список занимаемых сеточных позиций
        InventorySlot inventorySlot = gridSystemTiltedXY.GetGridSlot(); // Слот сетки

        switch (inventorySlot)
        {
            case InventorySlot.BagSlot:
                int index = _gridNameIndexDictionary[inventorySlot]; //получу из словоря Индекс сетки в _inventoryGridSystemVisualSingleArray

                foreach (Vector2Int gridPosition in OccupiesGridPositionList) // Переберем заниммаемые объектом позиции сетки
                {
                    //Установим занятость ячеек (и если isBusy = false(ячейка свободна) то установим переданный материал)
                    _inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                }
                break;

            //Визуал, Основной и Доп. сетки оружия, будем менять сразу у всех ячеек, т.к. там может размещаться только один предмет
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponSlot:

                index = _gridNameIndexDictionary[inventorySlot]; //получу из словоря Индекс сетки в _inventoryGridSystemVisualSingleArray

                for (int x = 0; x < _gridSystemTiltedXYList[index].GetWidth(); x++) // для полученной сетки переберем длину
                {
                    for (int y = 0; y < _gridSystemTiltedXYList[index].GetHeight(); y++)  // и высоту
                    {
                        //Установим занятость ячеек (и если isBusy = false(ячейка свободна) то установим переданный материал)
                        _inventoryGridSystemVisualSingleArray[index][x, y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                    }
                }

                break;
        }

    }

    // показать возможные сеточные позиции
    private void ShowPossibleGridPositions(GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY, PlacedObject placedObject, Vector2Int mouseGridPosition, GridVisualType gridVisualType)
    {
        InventorySlot gridName = gridSystemXY.GetGridSlot(); // Имя сетки где находиться мыш с захваченным объектом
        switch (gridName)
        {
            case InventorySlot.BagSlot:

                int index = _gridNameIndexDictionary[gridName]; //получу из словоря Индекс сетки в _inventoryGridSystemVisualSingleArray
                List<Vector2Int> TryOccupiesGridPositionList = placedObject.GetTryOccupiesGridPositionList(mouseGridPosition); // Список сеточных позиций которые хотим занять
                foreach (Vector2Int gridPosition in TryOccupiesGridPositionList) // Переберем список позиций которые хоти занять
                {
                    if (gridSystemXY.IsValidGridPosition(gridPosition)) // Если позиция допустима то...
                    {
                        if (!_inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].GetIsBusy()) // Если позиция не занята
                        {
                            _inventoryGridSystemVisualSingleArray[index][gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
                        }
                    }
                }
                break;
            //Когда объект над Основной и Доп. сетки оружия, будем показывать что он может занять сразу всю сетку, даже если он размером с одной ячейку
            case InventorySlot.MainWeaponSlot:
            case InventorySlot.OtherWeaponSlot:

                index = _gridNameIndexDictionary[gridName]; //получу из словоря Индекс сетки в _inventoryGridSystemVisualSingleArray

                for (int x = 0; x < _gridSystemTiltedXYList[index].GetWidth(); x++) // для полученной сетки переберем длину
                {
                    for (int y = 0; y < _gridSystemTiltedXYList[index].GetHeight(); y++)  // и высоту
                    {
                        if (_inventoryGridSystemVisualSingleArray[index][x, y].GetIsBusy()) // Если хотябы одна позиция занята то выходим из цикла и игнор. код ниже
                        {
                            break;
                        }                                                   
                        _inventoryGridSystemVisualSingleArray[index][x, y].Show(GetGridVisualTypeMaterial(gridVisualType));
                       
                    }
                }
                break;
        }


    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) //(Вернуть Материал в зависимости от Состояния) Получить Тип Материала для Сеточной Визуализации в зависимости от переданного в аргумент Состояния Сеточной Визуализации
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList) // в цикле переберем Список тип материала визуального состояния сетки 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // Если  Состояние сетки(gridVisualType) совподает с переданным нам состояние то ..
            {
                return gridVisualTypeMaterial.materialGrid; // Вернем материал соответствующий данному состоянию сетки
            }
        }

        Debug.LogError("Не смог найти GridVisualTypeMaterial для GridVisualType " + gridVisualType); // Если не найдет соответсвий выдаст ошибку
        return null;
    }
}
