using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static PathNodeSystem;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.CanvasScaler;
/// <summary>
/// Провайдер поиска пути (Поставщик услуг по поиску пути).Звено между PathfindingSystem(DOTS) и MonoBehaviour скриптами.<br/>
/// Обрабатывает запрос и передает в PathfindingSystem
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //Поместим в группу
[UpdateAfter(typeof(PathNodeSystem))] //Запустимся после PathNodeSystem 
public partial class PathfindingProviderSystem : SystemBase
{

    public event EventHandler OnPathfindingComplete;

    /// <summary>
    /// Словарь - допустимые позиции сетки и узлы пути <br/>
    /// PathNode - содержит инфу о пути
    /// </summary>
    private NativeHashMap<int3, PointsPath> _validGridPositionPointsPathDict; //Словарь. КЛЮЧ - допустимые сеточные позиции. ЗНАЧЕНИЕ - ТОЧКИ ПУТИ для этой сеточной позиции
    /// <summary>
    /// Список допустимых сеточных позиции ПЕРЕДВИЖЕНИЯ для выбранного юнита
    /// </summary>
    private List<GridPositionXZ> _validGridPositionMoveForSelectedUnitList = new List<GridPositionXZ>();

    private UnitActionSystem _unitActionSystem;
    private MouseOnGameGrid _mouseOnGameGrid;
    private LevelGrid _levelGrid;
    private Unit _selectedUnit;
    private bool _isPathfindingComplete = false;

    /// <summary>
    /// при инициализации поместить выше UnitActionSystem, LevelGrid, UnitSpawnerOnLevel
    /// </summary>
    public void Init(UnitActionSystem unitActionSystem, MouseOnGameGrid mouseOnGameGrid, LevelGrid levelGrid)
    {
        _unitActionSystem = unitActionSystem;
        _mouseOnGameGrid = mouseOnGameGrid;
        _levelGrid = levelGrid;
        Setup();
    }

    private void Setup()
    {
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        _mouseOnGameGrid.OnMouseGridPositionChanged += MouseOnGameGrid_OnMouseGridPositionChanged;
        _levelGrid.OnAddUnitAtGridPosition += LevelGrid_OnAddUnitAtGridPosition;
        _levelGrid.OnRemoveUnitAtGridPosition += LevelGrid_OnRemoveUnitAtGridPosition;

        /* _selectedUnit = _unitActionSystem.GetSelectedUnit();
         SetPathfindingForSelectedUnit();*/
    }

    private void LevelGrid_OnRemoveUnitAtGridPosition(object sender, GridPositionXZ e)
    {

    }

    private void LevelGrid_OnAddUnitAtGridPosition(object sender, GridPositionXZ e)
    {

    }

    private void MouseOnGameGrid_OnMouseGridPositionChanged(object sender, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        if (_unitActionSystem.GetIsBusy())
            return;
        if (_unitActionSystem.GetSelectedAction() is not MoveAction) //Если это не MoveAction то выходим
            return;

        //  Debug.Log("Позиция мыши изменилась.Запустил расчет пути");
    }

    private void UnitActionSystem_OnBusyChanged(object sender, UnitActionSystem.BusyChangedEventArgs e)
    {
        if (e.selectedAction is MoveAction && !e.isBusy) //Если юнит двигался и освободился(НЕ Занят) то 
        {
            if (_selectedUnit.GetActionPointsSystem().CanSpendActionPointsToTakeAction(e.selectedAction))//Если хватает очков для перемещения(is MoveAction) то расчитаем путь
            {
                Debug.Log("Установим поиск пути для юнита OnBusyChanged");
                SetPathfindingForSelectedUnit();
            }
            else
            {
                _validGridPositionMoveForSelectedUnitList.Clear();
                OnPathfindingComplete?.Invoke(this, EventArgs.Empty);
            }
        }
    }       

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit unit)
    {
        _selectedUnit = unit;

        if (unit.GetActionPointsSystem().CanSpendActionPointsToTakeAction(unit.GetAction<MoveAction>()))//Если хватает очков для перемещения то расчитаем путь
        {
          //  Debug.Log("Установим поиск пути для юнита OnSelectedUnitChanged");
            SetPathfindingForSelectedUnit();
        }
        else
        {
            _validGridPositionMoveForSelectedUnitList.Clear();
            OnPathfindingComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Установим поиск пути для выбранного юнита
    /// </summary>
    private void SetPathfindingForSelectedUnit()
    {
        _isPathfindingComplete = false;
        _validGridPositionMoveForSelectedUnitList.Clear();

        foreach ((RefRW<PathfindingParams> pathfindingParams, Entity pathfindingEntity) in SystemAPI.Query<RefRW<PathfindingParams>>().WithPresent<PathfindingParams>().WithEntityAccess())
        {
            pathfindingParams.ValueRW.startGridPosition = _selectedUnit.GetGridPosition().ParseInt3();
            pathfindingParams.ValueRW.maxMoveDistance = _selectedUnit.GetAction<MoveAction>().GetMaxActionDistance();

            SystemAPI.SetComponentEnabled<PathfindingParams>(pathfindingEntity, true); //Активируем PathfindingParams что бы запустить расчет
        }
    }

    protected override void OnUpdate()
    {
        // При включении ValidGridPositionPointsPathDict сохраним ссылку на словарь внутри него 
        foreach (RefRO<ValidGridPositionPointsPathDict> validGridPositionPathNodeDict in SystemAPI.Query<RefRO<ValidGridPositionPointsPathDict>>())
        {
            if (validGridPositionPathNodeDict.ValueRO.onRegister)
            {
                _validGridPositionPointsPathDict = validGridPositionPathNodeDict.ValueRO.dictionary;
               // Debug.Log($"validDict.onRegister = true");
            }
        }

        foreach (RefRO<PathfindingParams> pathfindingParams in SystemAPI.Query<RefRO<PathfindingParams>>().WithPresent<PathfindingParams>())
        {
            if (pathfindingParams.ValueRO.onPathfindingComplete)
            {
                PathfindingComplete();
               // Debug.Log($"PathfindingComplete");
            }
        }
    }


    /// <summary>
    /// Расчет пути завершен
    /// </summary>
    public void PathfindingComplete()
    {
        foreach (var collection in _validGridPositionPointsPathDict)
        {
            _validGridPositionMoveForSelectedUnitList.Add(new GridPositionXZ(collection.Key));
        }

        _isPathfindingComplete = true;
        OnPathfindingComplete?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Получить допустимые сеточные позиции передвижения для выбранного Юнита
    /// </summary>
    public List<GridPositionXZ> GetValidGridPositionMoveForSelectedUnit() { return _validGridPositionMoveForSelectedUnitList; }

    /// <summary>
    /// Получить ссылку на Словарь - допустимые позиции сетки и узлы пути
    /// </summary>
    public NativeHashMap<int3, PointsPath> GetvalidGridPositionPointsPathDict() { return _validGridPositionPointsPathDict; }

    /// <summary>
    /// Завершен ли поиск пути.
    /// </summary>
    public bool IsPathfindingComplete() { return _isPathfindingComplete; }

}
