using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject, хранит всю информацию о предмете, который можно создать и поместить в экипировка. 
/// </summary>
/// <remarks>
/// abstract - НЕЛЬЗЯ создать экземпляр данного класса.
/// На практике вы, скорее всего, будете использовать подкласс, такой как "SwordTypeSO"  "ShootingWeaponTypeSO" или "GrenadeTypeSO" "HealItemTypeSO".
/// </remarks>
public abstract class PlacedObjectTypeSO : ScriptableObject, ISerializationCallbackReceiver//ISerializationCallbackReceiver Интерфейс для получения обратных вызовов при сериализации и десериализации.Будем использовать для создания ID при сериализации
                                                                                           //Сериализацией называется процесс записи состояния объекта (с возможной последующей передачей) в поток. Десериализация это процесс обратный сериализации – из данных,
                                                                                           //которые находятся в потоке, мы можем восстановить состояние объекта и использовать этот объект в другом месте.
{
    [Header("Автоматически сгенерированный UUID для сохранения/загрузки.\nОчистите это поле, если вы хотите сгенерировать новое.")]
    [SerializeField] private string _itemID = null;
    [Header("Тип размещаемого объекта")]
    [SerializeField] protected PlacedObjectType _placedObjectType;
    [Header("Префаб размещаемого объекта 2D(для Canvas)")]
    [SerializeField] private Transform _prefab2D;
    [Header("Визуальная часть размещаемого объекта 2D(для кнопок Canvas)")]
    [SerializeField] private Transform _visual2D;
    [Header("Сколько занимает клеток в ширину Х")]
    [Range(1, 5)][SerializeField] private int _widthX;
    [Header("Сколько занимает клеток в высоту У")]
    [Range(1, 2)][SerializeField] private int _heightY;   
    [Header("Список слотов экипировки на которые можно разместить наш объект")]
    [SerializeField] private List<EquipmentSlot> _canPlacedOnSlotList;
    [Header("Вес размещаемого объекта в килограммах")]
    [Range(0, 50)][SerializeField] private int _weight;
    [Header("Цена ПОКУПКИ и ПРОДАЖИ на рынке")]
    [SerializeField] private uint _priceBuy ;
    [SerializeField] private uint _priceSell ;

    private HashSet<EquipmentSlot> _canPlacedOnSlotHashList;

    // КЭШИРОВАННОЕ СОСТАЯНИЕ
    static Dictionary<string, PlacedObjectTypeWithActionSO> placedObjectLookupCache; //кэшированный словарь поиска предмта типа PlacedObjectTypeWithActionSO// Статический словарь (Ключ-ID номер предмета, Значение)

    public virtual PlacedObjectTooltip GetPlacedObjectTooltip() // Получить всплывающую подсказку для данного размещенного объекта // virtual- переопределим в наследуемых классах
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
    public Transform GetPrefab2D() { return _prefab2D; }
    public Transform GetVisual2D() { return _visual2D; }

    /// <summary>
    /// Вычислить смещение центра визуала относительно якоря
    /// </summary>  
    public Vector3 GetOffsetVisualСenterFromAnchor()
    {
        RectTransform rectTransformPrefab2D = (RectTransform)_prefab2D.transform;  
        Vector2 center = rectTransformPrefab2D.sizeDelta / 2;
               
        return center;
    }

    /// <summary>
    /// Получить количество ЯЧЕЕК которое занимает объект в ширину Х и высоту Ую
    /// /// </summary>    
    public Vector2Int GetWidthXHeightYInCells() { return new Vector2Int(_widthX, _heightY); }  

    /// <summary>
    /// Список слотов на которые можно разместить наш объект
    /// </summary>
    public HashSet<EquipmentSlot> GetCanPlacedOnSlotList() 
    {
        if (_canPlacedOnSlotHashList == null)
        {
            _canPlacedOnSlotHashList = new HashSet<EquipmentSlot>(_canPlacedOnSlotList);
        }
        return _canPlacedOnSlotHashList; 
    }
       
    /// <summary>
    /// Получить цену покупки
    /// </summary>
    public uint GetPriceBuy() { return _priceBuy; }
    /// <summary>
    /// Получить цену продажи
    /// </summary>
    public uint GetPriceSell() { return _priceSell; }

   

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Сгенерируйте и сохраните новый UUID, если он пуст или там просто пустые пробелы.
        if (string.IsNullOrWhiteSpace(_itemID))
        {
            _itemID = Guid.NewGuid().ToString();
        }

        // Получим тип размещаемого объекта в этой строке
        if (name!=null)
            _placedObjectType = SheetProcessor.ParseEnum<PlacedObjectType>(name);// Преобразуем данные из ячейки в Enum типа<PlacedObjectType>        
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // Требуется ISerializationCallbackReceiver, но нам не нужно ничего с этим делать.
    }

}
