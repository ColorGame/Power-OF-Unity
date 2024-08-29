using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Визуализация размещения в сетке предметов экипировки
/// </summary>
/// <remarks>Работает с типом PlacedObject</remarks>
public class ItemGridVisual : MonoBehaviour, IToggleActivity
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
        Orange,
    }

    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList; // Список тип материала визуального состояния сетки Квадрат (Список из кастомного типа данных) визуального состояния сетки // В инспекторе под каждое состояние перетащить соответствующий материал сетки

    private EquipmentGridVisualSingle[][,] _equipmentGridVisualSingleArray; // Массив масивов [количество сеток][длина (Х), высота (У)]
    private Dictionary<EquipmentSlot, int> _gridNameIndexDictionary; // Словарь (EquipmentSlot - ключ, int(индекс) -значение)
    private List<GridSystemXY<GridObjectEquipmentXY>> _gridSystemXYList; // список сеток

    private Canvas _canvasItemGrid;

    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private EquipmentGrid _equipmentGrid;
    private UnitEquipmentSystem _unitEquipmentSystem;


    public void Init(PickUpDropPlacedObject pickUpDrop, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;

        Setup();
    }


    private void Setup()
    {
        _canvasItemGrid = _equipmentGrid.GetCanvasItemGrid();
        _gridNameIndexDictionary = new Dictionary<EquipmentSlot, int>();

        // Инициализируем сначала первую часть массива - Количество сеток
        _gridSystemXYList = _equipmentGrid.GetItemGridSystemXYList(); // Получим список сеток
        _equipmentGridVisualSingleArray = new EquipmentGridVisualSingle[_gridSystemXYList.Count][,];

        // Для каждой сетки реализуем двумерный массив координат
        for (int i = 0; i < _gridSystemXYList.Count; i++)
        {
            _equipmentGridVisualSingleArray[i] = new EquipmentGridVisualSingle[_gridSystemXYList[i].GetWidth(), _gridSystemXYList[i].GetHeight()];
        }

        for (int i = 0; i < _gridSystemXYList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // и высоту
                {
                    Vector2Int gridPosition = new Vector2Int(x, y); // позиция сетки
                    Vector3 rotation = _equipmentGrid.GetRotationAnchorGrid(_gridSystemXYList[i]);
                    Transform AnchorGridTransform = _equipmentGrid.GetAnchorGrid(_gridSystemXYList[i]);

                    EquipmentGridVisualSingle equipmentGridVisualSingle;

                    switch (_canvasItemGrid.renderMode)
                    {
                        default:
                        case RenderMode.ScreenSpaceOverlay:
                        case RenderMode.ScreenSpaceCamera:
                            equipmentGridVisualSingle = Instantiate(GameAssets.Instance.EquipmentGridInVisualSingleScreenSpacePrefab, _equipmentGrid.GetWorldPositionCenterСornerCell(gridPosition, _gridSystemXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // Создадим наш префаб в каждой позиции сетки
                            break;
                        case RenderMode.WorldSpace:
                            equipmentGridVisualSingle = Instantiate(GameAssets.Instance.EquipmentGridInVisualSingleWorldSpacePrefab, _equipmentGrid.GetWorldPositionCenterСornerCell(gridPosition, _gridSystemXYList[i]), Quaternion.Euler(rotation), AnchorGridTransform); // Создадим наш префаб в каждой позиции сетки
                            break;
                    }

                    equipmentGridVisualSingle.Init(_gridSystemXYList[i].GetCellSizeWithScaleFactor());

                    _gridNameIndexDictionary[_gridSystemXYList[i].GetGridSlot()] = i; // Присвоим ключу(имя Сетки под этим индексом) значение (индекс массива)
                    _equipmentGridVisualSingleArray[i][x, y] = equipmentGridVisualSingle; // Сохраняем компонент LevelGridVisualSingle в трехмерный массив где x,y,y это будут индексы массива.
                }
            }
        }
    }

    public void SetActive(bool active)
    {
        _canvasItemGrid.enabled = active;
        if (active)
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared += UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
        }
        else
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid -= PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged -= PickUpDropManager_OnGrabbedObjectGridPositionChanged;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits -= PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared -= UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
        }
    }

    /// <summary>
    /// Инвентарь очищен
    /// </summary>    
    private void UnitEquipmentSystem_OnEquipmentGridsCleared(object sender, EventArgs e)
    {
        SetDefoltState(); // Установить дефолтное состояние        
    }
    /// <summary>
    /// Захваченый объект покинул сетку
    /// </summary>
    private void PickUpDropManager_OnGrabbedObjectGridExits(object sender, EventArgs e)
    {
        UpdateVisual();
    }
    /// <summary>
    /// позиция захваченного объекта на сетке изменилась
    /// </summary>
    private void PickUpDropManager_OnGrabbedObjectGridPositionChanged(object sender, PlacedObjectGridParameters e)
    {
        UpdateVisual();
        ShowPossibleGridPositions(e.slot, e.placedObject, e.gridPositioAnchor, GridVisualType.Orange); //показать возможные сеточные позиции
    }
    /// <summary>
    /// Объект удален из сетки и повис над ней
    /// </summary>
    private void PickUpDropSystem_OnRemovePlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Orange);
    }
    /// <summary>
    /// Объект добавлен в сетку 
    /// </summary>
    private void OnAddPlacedObjectAtGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }
    /// <summary>
    /// Обновить визуал
    /// </summary>
    private void UpdateVisual()
    {
        for (int i = 0; i < _gridSystemXYList.Count; i++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemXYList[i].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemXYList[i].GetHeight(); y++)  // и высоту
                {
                    if (!_equipmentGridVisualSingleArray[i][x, y].GetIsBusy()) // Если позиция не занята
                    {
                        _equipmentGridVisualSingleArray[i][x, y].Show(GetGridVisualTypeMaterial(GridVisualType.Grey));
                    }
                }
            }
        }
    }
    /// <summary>
    /// Установить дефолтное состояние
    /// </summary>
    private void SetDefoltState()
    {
        for (int index = 0; index < _gridSystemXYList.Count; index++) // переберем все сетки
        {
            for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // для каждой сетки переберем длину
            {
                for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // и высоту
                {
                    _equipmentGridVisualSingleArray[index][x, y].SetIsBusyAndMaterial(false, GetGridVisualTypeMaterial(GridVisualType.Grey));
                }
            }
        }
    }
    /// <summary>
    /// Установить занятость и материал
    /// </summary>
    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY = placedObject.GetGridSystemXY(); // Сеточная система которую занимает объект
        List<Vector2Int> OccupiesGridPositionList = placedObject.GetOccupiesGridPositionList(); // Список занимаемых сеточных позиций
        EquipmentSlot equipmentSlot = gridSystemXY.GetGridSlot(); // Слот сетки

        switch (equipmentSlot)
        {
            case EquipmentSlot.BagSlot:
                int index = _gridNameIndexDictionary[equipmentSlot]; //получу из словоря Индекс сетки в _equipmentGridVisualSingleArray

                foreach (Vector2Int gridPosition in OccupiesGridPositionList) // Переберем заниммаемые объектом позиции сетки
                {
                    //Установим занятость ячеек (и если isBusy = false(ячейка свободна) то установим переданный материал)
                    _equipmentGridVisualSingleArray[index][gridPosition.x, gridPosition.y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                }
                break;

            //Визуал, Основной и Доп. сетки оружия, будем менять сразу у всех ячеек, т.к. там может размещаться только один предмет
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:

                index = _gridNameIndexDictionary[equipmentSlot]; //получу из словоря Индекс сетки в _equipmentGridVisualSingleArray

                for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // для полученной сетки переберем длину
                {
                    for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // и высоту
                    {
                        //Установим занятость ячеек (и если isBusy = false(ячейка свободна) то установим переданный материал)
                        _equipmentGridVisualSingleArray[index][x, y].SetIsBusyAndMaterial(isBusy, GetGridVisualTypeMaterial(gridVisualType));
                    }
                }

                break;
        }

    }

    /// <summary>
    /// Показать возможные сеточные позиции
    /// </summary>
    private void ShowPossibleGridPositions(EquipmentSlot equipmentSlot, PlacedObject placedObject, Vector2Int gridPositioAnchor, GridVisualType gridVisualType)
    {
        GridSystemXY<GridObjectEquipmentXY> gridSystemXY = _equipmentGrid.GetActiveGridSystemXY(equipmentSlot); // Получим сетку для данного слота
        switch (equipmentSlot)
        {
            case EquipmentSlot.BagSlot:

                int index = _gridNameIndexDictionary[equipmentSlot]; //получу из словоря Индекс сетки в _equipmentGridVisualSingleArray                
                List<Vector2Int> TryOccupiesGridPositionList = placedObject.GetTryOccupiesGridPositionList(gridPositioAnchor); // Список сеточных позиций которые хотим занять
                foreach (Vector2Int gridPosition in TryOccupiesGridPositionList) // Переберем список позиций которые хоти занять
                {
                    if (gridSystemXY.IsValidGridPosition(gridPosition)) // Если позиция допустима то...
                    {
                        if (!_equipmentGridVisualSingleArray[index][gridPosition.x, gridPosition.y].GetIsBusy()) // Если позиция не занята
                        {
                            _equipmentGridVisualSingleArray[index][gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
                        }
                    }
                }
                break;
            //Когда объект над Основной и Доп. сетки оружия, будем показывать что он может занять сразу всю сетку, даже если он размером с одной ячейку
            case EquipmentSlot.MainWeaponSlot:
            case EquipmentSlot.OtherWeaponsSlot:

                index = _gridNameIndexDictionary[equipmentSlot]; //получу из словоря Индекс сетки в _equipmentGridVisualSingleArray

                for (int x = 0; x < _gridSystemXYList[index].GetWidth(); x++) // для полученной сетки переберем длину
                {
                    for (int y = 0; y < _gridSystemXYList[index].GetHeight(); y++)  // и высоту
                    {
                        if (_equipmentGridVisualSingleArray[index][x, y].GetIsBusy()) // Если хотябы одна позиция занята то выходим из цикла и игнор. код ниже
                        {
                            break;
                        }
                        _equipmentGridVisualSingleArray[index][x, y].Show(GetGridVisualTypeMaterial(gridVisualType));

                    }
                }
                break;
        }
    }
    /// <summary>
    /// (Вернуть Материал в зависимости от Состояния) Получить Тип Материала для Сеточной Визуализации в зависимости от переданного в аргумент Состояния Сеточной Визуализации
    /// </summary>
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList) // в цикле переберем Список тип материала визуального состояния сетки 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // Если  Состояние сетки(gridVisualType) совподает с переданным нам состояние то ..
            {
                return gridVisualTypeMaterial.materialGrid; // Вернем материал соответствующий данному состоянию сетки
            }
        }

        Debug.LogError("Не смог найти GridVisualTypeColor для GridVisualType " + gridVisualType); // Если не найдет соответсвий выдаст ошибку
        return null;
    }

    private void OnDestroy()
    {
        _unitEquipmentSystem = null; // Очистим ссылки чтобы класс обнулился
    }
}
