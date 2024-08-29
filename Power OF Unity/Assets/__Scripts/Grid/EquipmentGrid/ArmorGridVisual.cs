using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Визуализация размещения в сетке БРОНИ 
/// </summary>
/// <remarks>Работает с типом PlacedObject</remarks>
public class ArmorGridVisual : MonoBehaviour
{
    [Serializable] // Чтобы созданная структура могла отображаться в инспекторе
    public struct GridVisualTypeColor    //Визуал сетки Тип Материала // Создадим структуру можно в отдельном классе. Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#
    {                                       //В данной структуре объединям состояние сетки с материалом
        public GridVisualType gridVisualType;
        public Color colorlGrid;
    }

    public enum GridVisualType //Визуальные состояния сетки
    {
        Ocean,
        Orange,
    }

    [SerializeField] private List<GridVisualTypeColor> _gridVisualColorList; // Список цветов для визуализации сетки
    [SerializeField] private EquipmentGridVisualSingleSetColor _armorHeadGridVisual;
    [SerializeField] private EquipmentGridVisualSingleSetColor _armorBodyGridVisual;


    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private EquipmentGrid _equipmentGrid;
    private UnitEquipmentSystem _unitEquipmentSystem;


    public void Init(PickUpDropPlacedObject pickUpDrop, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;
    }



    public void SetActive(bool active)
    {
        if (active)
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid += PickUpDropSystem_OnRemovePlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;

            _unitEquipmentSystem.OnEquipmentGridsCleared += UnitEquipmentSystem_OnEquipmentGridsCleared;
            _unitEquipmentSystem.OnAddPlacedObjectAtEquipmentGrid += OnAddPlacedObjectAtGrid;
        }
        else
        {
            _pickUpDropPlacedObject.OnAddPlacedObjectAtEquipmentGrid -= OnAddPlacedObjectAtGrid;
            _pickUpDropPlacedObject.OnRemovePlacedObjectAtEquipmentGrid -= PickUpDropSystem_OnRemovePlacedObjectAtGrid;
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
        if (!_armorHeadGridVisual.GetIsBusy())// Если сетка не занята
        {
            _armorHeadGridVisual.Show(GetColorTypeMaterial(GridVisualType.Ocean));
        }
        if (!_armorBodyGridVisual.GetIsBusy())// Если сетка не занята
        {
            _armorBodyGridVisual.Show(GetColorTypeMaterial(GridVisualType.Ocean));
        }
    }
    /// <summary>
    /// Установить дефолтное состояние
    /// </summary>
    private void SetDefoltState()
    {
        _armorHeadGridVisual.SetIsBusyAndColor(false, GetColorTypeMaterial(GridVisualType.Ocean));
        _armorBodyGridVisual.SetIsBusyAndColor(false, GetColorTypeMaterial(GridVisualType.Ocean));
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
            case EquipmentSlot.ArmorHeadSlot:
                _armorHeadGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
            case EquipmentSlot.ArmorBodySlot:
                _armorBodyGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
        }
    }


    /// <summary>
    /// (Вернуть Цвет в зависимости от Состояния) Получить Цвет для Сеточной Визуализации в зависимости от переданного в аргумент Состояния Сеточной Визуализации
    /// </summary>
    private Color GetColorTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeColor gridVisualTypeMaterial in _gridVisualColorList) // в цикле переберем Список тип материала визуального состояния сетки 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // Если  Состояние сетки(gridVisualType) совподает с переданным нам состояние то ..
            {
                return gridVisualTypeMaterial.colorlGrid; // Вернем материал соответствующий данному состоянию сетки
            }
        }

        Debug.LogError("Не смог найти GridVisualTypeColor для GridVisualType " + gridVisualType); // Если не найдет соответсвий выдаст ошибку
        return new Color(0, 0, 0, 0);
    }

    private void OnDestroy()
    {
        _unitEquipmentSystem = null; // Очистим ссылки чтобы класс обнулился
    }
}
