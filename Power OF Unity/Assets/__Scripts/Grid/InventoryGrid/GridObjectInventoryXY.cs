using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


/// <summary>
/// Объект сетки (Хранит экземпляр размещенного объекта PlacedObject) 
/// </summary>
/// <remarks>
/// GridObjectInventoryXY создается в каждой ячейки сетки. Является оболочкой для хранения объектов
/// </remarks>
public class GridObjectInventoryXY 
{
    private GridSystemXY<GridObjectInventoryXY> _gridSystem; // Сеточная система .В дженерик предаем тип GridObjectUnitXZ// Частная сеточная система которая создала этот объект (это расширение например для сетки 2-го этажа)
    private Vector2Int _gridPosition; // Положение объекта в сетке
    private PlacedObject _placedObject; // Размещенный объект.
    private IInteractable _interactable; // IInteractable(интерфейс взаимодействия) (будет частью GridObjectUnitXZ как и юнит) ИНТЕРФЕЙС позволяет взаимодействовать с любым объектом (дверь, сфера, кнопка...) - который реализует этот интерфейс

    public GridObjectInventoryXY(GridSystemXY<GridObjectInventoryXY> gridSystem, Vector2Int gridPosition) // Конструктор 
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _placedObject = null;
    }

    public override string ToString() // Переопределим ToString(). Чтобы она возвращала позицию в сетке и юнита в этой ячейке (позже можно расширить диапазон возвращаемых данных)
    {                
        return _gridPosition.ToString() + "\n" +"!-"+ _placedObject;        
    }

    public void AddPlacedObject(PlacedObject placedObject) // добавим Размещаемый объект 
    {
        _placedObject = placedObject;
    }

    public PlacedObject GetPlacedObject() // получить Размещаемый объект 
    {
        return _placedObject;
    }

    public void RemovePlacedObject() // удалим Размещаемый объект 
    {
        _placedObject = null;
    } 

    public bool HasPlacedObject()
    {
        return _placedObject != null;
    }


}

