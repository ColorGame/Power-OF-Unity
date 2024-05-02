using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Инвентарь юнита (Хранилище для предметов, которыми оснащен игрок). 
/// </summary>
/// <remarks>
/// Компонент должен быть прикриплен к юниту
/// </remarks>
public class UnitInventory : MonoBehaviour // Инвентарь юнита
{   

   

    [Header("ЗАПОЛНЯТЬ ТОЛЬКО ДЛЯ ВРАГА\n(Список оружия которым владеет ЮНИТ)")]
    [SerializeField] private List<PlacedObject> _placedObjectList; // Список Размещенных Объектов в Сетке Инвенторя.
   

   
    private Unit _unit;
    private UnitEquipment _unitEquipment;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _unitEquipment = GetComponent<UnitEquipment>();
    }

    private void Start()
    {
        if (_unit.IsEnemy()) // Если наш юнит враг то экипируем его объектами из списка
        {
            foreach (PlacedObject placedObject in _placedObjectList)
            {
                _unitEquipment.EquipArmor(placedObject);
            }
        }
    }

    /// <summary>
    /// Добавить полученный объект в Список "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>  
    public void AddPlacedObjectList(PlacedObject placedObject)
    {
        _placedObjectList.Add(placedObject);       
    }
    /// <summary>
    /// Удалить полученный объект из Списока "Размещенных Объектов в Сетке Инвенторя".
    /// </summary>  
    public void RemovePlacedObjectList(PlacedObject placedObject)
    {
        _placedObjectList.Remove(placedObject);     
    }


    /// <summary>
    /// Размещенный Объект в Сетке Инвенторя
    /// </summary>
    [Serializable]
    public struct PlacedObjectOnInventoryGrid
    {
        public GridSystemTiltedXY<GridObjectInventoryXY> gridSystemXY; //Сетка на которой добавили размещенный объект
        public Vector2Int gridPositioAnchor; // Сеточная позиция Якоря размещенного объекта
        public PlacedObjectTypeSO placedObjectTypeSO;
    }
    //private List<PlacedObjectOnInventoryGrid> _placedObjectOnInventoryGrid; // Список Размещенных Объектов в Сетке Инвенторя.
}
