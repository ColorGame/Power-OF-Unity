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
    private Canvas _canvasArmorGrid;


    public void Init(PickUpDropPlacedObject pickUpDrop, EquipmentGrid equipmentGrid, UnitEquipmentSystem unitEquipmentSystem)
    {
        _pickUpDropPlacedObject = pickUpDrop;
        _equipmentGrid = equipmentGrid;
        _unitEquipmentSystem = unitEquipmentSystem;

        _canvasArmorGrid = _equipmentGrid.GetCanvasArmorGrid();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            // Включать будем только если активна нужная СЕТКА 
            if (_equipmentGrid.GetStateGrid() == EquipmentGrid.GridState.ArmorGrid)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        else
        {
            Disable();
        }
    }

    private void Enable()
    {
        _canvasArmorGrid.enabled = true;

        _equipmentGrid.OnAddInEquipmentGrid += EquipmentGrid_OnAddEquipmentGrid;
        _equipmentGrid.OnRemoveFromEquipmentGridAndHung += EquipmentGrid_OnRemoveFromEquipmentGridAndHung;
        _equipmentGrid.OnRemoveFromEquipmentGridAndMoveStartPosition += EquipmentGrid_OnRemoveFromEquipmentGridAndMoveStartPosition;

        _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged += PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDropPlacedObject.OnGrabbedObjectGridExits += PickUpDropManager_OnGrabbedObjectGridExits;

        _unitEquipmentSystem.OnEquipmentGridsCleared += UnitEquipmentSystem_OnEquipmentGridsCleared;       
    }

    private void Disable()
    {
        _canvasArmorGrid.enabled = false;

        _equipmentGrid.OnAddInEquipmentGrid -= EquipmentGrid_OnAddEquipmentGrid;
        _equipmentGrid.OnRemoveFromEquipmentGridAndHung -= EquipmentGrid_OnRemoveFromEquipmentGridAndHung;
        _equipmentGrid.OnRemoveFromEquipmentGridAndMoveStartPosition += EquipmentGrid_OnRemoveFromEquipmentGridAndMoveStartPosition;

        _pickUpDropPlacedObject.OnGrabbedObjectGridPositionChanged -= PickUpDropManager_OnGrabbedObjectGridPositionChanged;
        _pickUpDropPlacedObject.OnGrabbedObjectGridExits -= PickUpDropManager_OnGrabbedObjectGridExits;

        _unitEquipmentSystem.OnEquipmentGridsCleared -= UnitEquipmentSystem_OnEquipmentGridsCleared;       
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
       // UpdateVisual();
        ShowPossibleGridPositions(e.slot, e.placedObject, e.gridPositioAnchor, GridVisualType.Orange); //показать возможные сеточные позиции
    }
    /// <summary>
    /// Объект удален из сетки и повис над ней
    /// </summary>
    private void EquipmentGrid_OnRemoveFromEquipmentGridAndHung(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Orange);
    }

    /// <summary>
    /// Объект удален из сетки и движется в стартовую позицию
    /// </summary>
    private void EquipmentGrid_OnRemoveFromEquipmentGridAndMoveStartPosition(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, false, GridVisualType.Ocean);
    }
    /// <summary>
    /// Объект добавлен в сетку 
    /// </summary>
    private void EquipmentGrid_OnAddEquipmentGrid(object sender, PlacedObject placedObject)
    {
        SetIsBusyAndMaterial(placedObject, true);
    }
    /// <summary>
    /// Обновить визуал
    /// </summary>
    private void UpdateVisual()
    {
        SetVisualTypeOnFreeGridPosition(GridVisualType.Ocean);
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
    /// Установить занятость и тип визуализации
    /// </summary>
    private void SetIsBusyAndMaterial(PlacedObject placedObject, bool isBusy, GridVisualType gridVisualType = 0)
    {        
        EquipmentSlot equipmentSlot = placedObject.GetGridSystemXY().GetGridSlot(); // Слот сетки

        switch (equipmentSlot)
        {
            case EquipmentSlot.HeadArmorSlot:
                _armorHeadGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
            case EquipmentSlot.BodyArmorSlot:
                _armorBodyGridVisual.SetIsBusyAndColor(isBusy, GetColorTypeMaterial(gridVisualType));
                break;
        }
    }

    /// <summary>
    /// Показать возможные сеточные позиции
    /// </summary>
    private void ShowPossibleGridPositions(EquipmentSlot equipmentSlot, PlacedObject placedObject, Vector2Int gridPositioAnchor, GridVisualType gridVisualType)
    {       
        switch (equipmentSlot)
        {
            case EquipmentSlot.HeadArmorSlot:
                if (!_armorHeadGridVisual.GetIsBusy())// Если сетка не занята
                {
                    _armorHeadGridVisual.Show(GetColorTypeMaterial(gridVisualType));
                }
                break;
            case EquipmentSlot.BodyArmorSlot:
                if (!_armorBodyGridVisual.GetIsBusy())// Если сетка не занята
                {
                    _armorBodyGridVisual.Show(GetColorTypeMaterial(gridVisualType));
                }
                break;
        }
    }
    /// <summary>
    /// Установить переданный ТИП ВИЗУАЛИЗАЦИИ на свободные сеточные позиции
    /// </summary>
    private void SetVisualTypeOnFreeGridPosition(GridVisualType gridVisualType)
    {
        if (!_armorHeadGridVisual.GetIsBusy())// Если сетка не занята
        {
            _armorHeadGridVisual.Show(GetColorTypeMaterial(gridVisualType));
        }
        if (!_armorBodyGridVisual.GetIsBusy())// Если сетка не занята
        {
            _armorBodyGridVisual.Show(GetColorTypeMaterial(gridVisualType));
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
