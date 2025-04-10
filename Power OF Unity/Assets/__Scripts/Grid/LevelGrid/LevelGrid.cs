using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


// НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА LevelGrid, добавим в Project Settings/ Script Execution Order и поместим выполнение LevelGrid выше Default Time, чтобы LevelGrid запустился РАНЬШЕ до того как ктонибудь совершит поиск пути ( В StartScene() мы запускаем класс PathfindingMonkey - настроику поиска пути)

public class LevelGrid : MonoBehaviour // Основной скрипт который управляет СЕТКОЙ данного УРОВНЯ . Оснавная задача Присвоить или Получить определенного Юнита К заданной Позиции Сетки
{
    public event EventHandler<GridPositionXZ> OnAddUnitAtGridPosition; 
    public event EventHandler<GridPositionXZ> OnRemoveUnitAtGridPosition; 


    [SerializeField] private Transform _gridDebugObjectPrefab; // Префаб отладки сетки //Передоваемый тип должен совподать с типом аргумента метода CreateDebugObject
    [SerializeField] private GridParamsSO _levelGridPAramsSO;
    [SerializeField] private SceneName _sceneName;

    private LevelGridParameters _gridParameters;
    private List<GridSystemXZ<GridObjectUnitXZ>> _gridSystemList; //Список сеточнах систем .В дженерик предаем тип GridObjectUnitXZ

    private void Awake()
    {
        _gridParameters = _levelGridPAramsSO.GetLevelGridParameters(_sceneName);
        _gridSystemList = new List<GridSystemXZ<GridObjectUnitXZ>>(); // Инициализируем список

        for (int floor = 0; floor < _gridParameters.floorAmount; floor++) // Переберем этажи и на каждом построим сеточную систему
        {
            GridSystemXZ<GridObjectUnitXZ> gridSystem = new GridSystemXZ<GridObjectUnitXZ>(_gridParameters, // ПОСТРОИМ СЕТКУ и в каждой ячейки создадим объект типа GridObjectUnitXZ
                 (GridSystemXZ<GridObjectUnitXZ> gridSystem, GridPositionXZ gridPosition) => new GridObjectUnitXZ(gridSystem, gridPosition),transform.position, floor); //в 5 параметре аргумента зададим функцию ананимно через лямбду => new GridObjectUnitXZ(gridSystem, _gridPositioAnchor) И ПЕРЕДАДИМ ЕЕ ДЕЛЕГАТУ. (лямбда выражение можно вынести в отдельный метод)

         //  gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // Создадим наш префаб в каждой ячейки // Закоментировал т.к. PathfindingGridDebugObject будет выполнять базовыедействия вместо _gridDebugObjectPrefab

            _gridSystemList.Add(gridSystem); // Добавим в список созданную сетку
        }
    }  

    private GridSystemXZ<GridObjectUnitXZ> GetGridSystem(int floor) // Получить Сеточную систему для данного этажа
    {
        return _gridSystemList[floor];
    }    

    public List<Unit> GetUnitListAtGridPosition(GridPositionXZ gridPosition) // Получить Список Юнитов В заданной Позиции Сетки
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        return gridObject.GetUnitList();// получим юнита
    }

    public void AddUnitAtGridPosition(GridPositionXZ gridPosition, Unit unit) // Добавить определенного Юнита К заданной Позиции Сетки
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        gridObject.AddUnit(unit); // Добавить юнита 
        OnAddUnitAtGridPosition?.Invoke(this, gridPosition);
    }

    public void RemoveUnitAtGridPosition(GridPositionXZ gridPosition, Unit unit) // Удаление юнита из заданной позиции сетки
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        gridObject.RemoveUnit(unit); // удалим юнита
        OnRemoveUnitAtGridPosition?.Invoke(this, gridPosition);
    }

    public void UnitMovedGridPosition(Unit unit, GridPositionXZ fromGridPosition, GridPositionXZ toGridPosition) // Юнит Перемещен в Сеточной позиции из позиции fromGridPosition в позицию toGridPosition
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit); // Удалим юнита из прошлой позиции сетки
        AddUnitAtGridPosition(toGridPosition, unit);  // Добавим юнита к следующей позиции сетки
    }

    public int GetFloor(Vector3 worldPosition) // Получить этаж
    {
        return Mathf.RoundToInt(worldPosition.y / _gridParameters.floorHeight); // Поделим позицию по у на высоту этажа и округлим до целого тем самым получим этаж
    }

    // Что бы не раскрывать внутриние компоненты LevelGrid (и не делать публичным поле_gridSystem) но предоставить доступ к GridPositionXZ сделаем СКВОЗНУЮ функцию для доступа к GridPositionXZ
    public GridPositionXZ GetGridPosition(Vector3 worldPosition) // вернуть сеточную позицию для мировых координат
    {
        int floor = GetFloor(worldPosition); // узнаем этаж
        return GetGridSystem(floor).GetGridPosition(worldPosition); // Для этого этажа вернем сеточную позицию
    }

   /* /// <summary>
    /// Получить узел сетки - Если знаем, что координата находится внутри сетки, и хотим максимизировать производительность, то 
    /// будем напрямую искать узел во внутреннем массиве, что немного быстрее.
    /// </summary>
    public LevelGridNode GetGridNode(GridPositionXZ gridPosition) // Получить узел сетки (A* PathfindingGridDate Project4.2.18) для нашей сеточной позиции ()
    {
        int width = AstarPath.active.data.layerGridGraph.width;
        int depth = AstarPath.active.data.layerGridGraph.depth;
        *//*int layerCount = AstarPath.active.data.layerGridGraph.LayerCount;
        float nodeSize = AstarPath.active.data.layerGridGraph.nodeSize;     *//*

        LevelGridNode gridNode = (LevelGridNode)AstarPath.active.data.layerGridGraph.nodes[gridPosition.x + gridPosition.z * width + gridPosition.floor * width * depth];

        *//*// Проверял совпадение GraphNode и GridPositionXZ  
        Debug.Log("x " + _gridPositioAnchor.x + "y " + _gridPositioAnchor.y +"Floor " + _gridPositioAnchor._floor + 
            " node" + " x " + gridNode.XCoordinateInGrid + "y " + gridNode.ZCoordinateInGrid +"Layer " + gridNode.LayerCoordinateInGrid );*//*
        return gridNode;
    }*/

  /*  /// <summary>
    /// True, если узел доступен для прохода.
    /// </summary>
    public bool WalkableNode(GridPositionXZ gridPosition)
    {
       return GetGridNode(gridPosition).Walkable;
    }
*/
    public Vector3 GetWorldPosition(GridPositionXZ gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition); // Сквозная функция

    public bool IsValidGridPosition(GridPositionXZ gridPosition) // Является ли Допустимой Сеточной Позицией
    {
        if (gridPosition.floor < 0 || gridPosition.floor >= _gridParameters.floorAmount) // выходим за пределы наших этажей
        {
            return false;
        }
        else
        {
            return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition); // Сквозная функция для получения доступа к IsValidGridPosition из _itemGridSystemXYList
        }

    }
    public int GetWidth() => GetGridSystem(0).GetWidth(); // Все этажи имеют одинаковую форму поэто му берем 0 этаж
    public int GetHeight() => GetGridSystem(0).GetHeight();
    public float GetCellSize() => GetGridSystem(0).GetCellSize();
    public int GetFloorAmount() => _gridParameters.floorAmount;
    public float GetFloorHeght()=> _gridParameters.floorHeight;
    public Vector3 GetAnchorGrid() =>transform.position;

    public bool HasAnyUnitOnGridPosition(GridPositionXZ gridPosition) // Есть ли какой нибудь юнит на этой сеточной позиции
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPositionXZ gridPosition) // Получить Юнита в этой сеточной позиции
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        return gridObject.GetUnit();
    }


    // IInteractable Интерфейс Взаимодействия - позволяет в классе InteractAction взаимодействовать с любым объектом (дверь, сфера, кнопка...) - который реализует этот интерфейс
    public IInteractable GetInteractableAtGridPosition(GridPositionXZ gridPosition) // Получить Интерфейс Взаимодействия в этой сеточной позиции
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPosition(GridPositionXZ gridPosition, IInteractable interactable) // Установить полученный Интерфейс Взаимодействия в этой сеточной позиции
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        gridObject.SetInteractable(interactable);
    }
    public void ClearInteractableAtGridPosition(GridPositionXZ gridPosition) // Очистить Интерфейс Взаимодействия в этой сеточной позиции
    {
        GridObjectUnitXZ gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition); // Получим GridObjectUnitXZ который находится в _gridPositioAnchor
        gridObject.ClearInteractable(); // Очистить Интерфейс Взаимодействия в эточ сеточном объекте
    }

}
