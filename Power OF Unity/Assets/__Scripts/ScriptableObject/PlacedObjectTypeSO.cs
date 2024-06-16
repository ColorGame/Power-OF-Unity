using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, хранит всю информацию о предмете, который можно создать и поместить в инвентарь. 
/// </summary>
/// <remarks>
/// abstract - НЕЛЬЗЯ создать экземпляр данного класса.
/// На практике вы, скорее всего, будете использовать подкласс, такой как "WeaponPlacedObjectSO"  "ItemPlacedObjectSO" или "ModulePlacedObjectSO" "EquipmentPlacedObjectSO".
/// </remarks>
public abstract class PlacedObjectTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver Интерфейс для получения обратных вызовов при сериализации и десериализации.Будем использовать для создания ID при сериализации
                                                                                           //Сериализацией называется процесс записи состояния объекта (с возможной последующей передачей) в поток. Десериализация это процесс обратный сериализации – из данных,
                                                                                           //которые находятся в потоке, мы можем восстановить состояние объекта и использовать этот объект в другом месте.
{
    [Tooltip("Автоматически сгенерированный UUID для сохранения/загрузки. Очистите это поле, если вы хотите сгенерировать новое.")]
    [SerializeField] private string _itemID = null;
    [Tooltip("Тип размещаемого объекта")]
    [SerializeField] private PlacedObjectType _placedObjectType;
    [Tooltip("Префаб размещаемого объекта")]
    [SerializeField] private Transform _prefab;
    [Tooltip("Визуальная часть размещаемого объекта")]
    [SerializeField] private Transform _visual;
    [Tooltip("Сколько занимает клеток в ширину Х")]
    [SerializeField] private int _widthX;
    [Tooltip("Сколько занимает клеток в высоту У")]
    [SerializeField] private int _heightY;
    [Tooltip("Список слотов инвенторя на которые можно разместить наш объект")]
    [SerializeField] private List<InventorySlot> _canPlacedOnSlotList;
    [Tooltip("Вес размещаемого объекта в килограммах")]
    [Range(0, 50)][SerializeField] private int _weight;

    private BaseAction[] _baseActionArray; // Список базовых действий прикрипленных к данному предмету их может быть несколько (например GrappleComboAction и GrappleAction)

    // КЭШИРОВАННОЕ СОСТАЯНИЕ
    static Dictionary<string, PlacedObjectTypeSO> placedObjectLookupCache; //кэшированный словарь поиска предмта типа PlacedObjectTypeSO// Статический словарь (Ключ-ID номер предмета, Значение)


    public virtual PlacedObjectTooltip GetPlacedObjectTooltip() // Получить всплывающую подсказку длф данного размещенного объекта // virtual- переопределим в наследуемых классах
    {
        return PlacedObjectTypeBaseStatsSO.Instance.GetTooltipPlacedObject(_placedObjectType);
    }

    /// <summary>
    /// Список сеточных позиций которые занимает объект относительно переданной сеточной позиции
    /// </summary>
    public List<Vector2Int> GetGridPositionList(Vector2Int gridPosition)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for (int x = 0; x < _widthX; x++)
        {
            for (int y = 0; y < _heightY; y++)
            {
                gridPositionList.Add(gridPosition + new Vector2Int(x, y));
            }
        }
        return gridPositionList;
    }

    public PlacedObjectType GetPlacedObjectType() { return _placedObjectType; }

    public Transform GetPrefab() { return _prefab; }

    /// <summary>
    /// Вычислить смещение визуала относительно родителя
    /// </summary>  
    public Vector3 GetOffsetVisualFromParent()
    {
        float cellSize = InventoryGrid.GetCellSize();

        float x = cellSize * _widthX / 2; // Размер ячейки умножим на количество ячеек, которое занимает наш объект по Х и делим пополам
        float y = cellSize * _heightY / 2;

        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Получить количество ЯЧЕЕК которое занимает объект в ширину Х и высоту Ую
    /// /// </summary>    
    public Vector2Int GetWidthXHeightYInCells() { return new Vector2Int(_widthX, _heightY); }

    public Transform GetVisual() { return _visual; }

    /// <summary>
    /// Список слотов на которые можно разместить наш объект
    /// </summary>
    public List<InventorySlot> GetCanPlacedOnSlotList() { return _canPlacedOnSlotList; }

    public BaseAction[] GetBaseActionsArray()
    {
        if(_baseActionArray.Length == 0) // Если еще не заполнена то 
        {
          return  _baseActionArray = _prefab.GetComponents<BaseAction>();
        }
        return _baseActionArray;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Сгенерируйте и сохраните новый UUID, если он пуст или там просто пустые пробелы.
        if (string.IsNullOrWhiteSpace(_itemID))
        {
            _itemID = Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // Требуется ISerializationCallbackReceiver, но нам не нужно ничего с этим делать.
    }

}
