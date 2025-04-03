using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// Провайдер поиска пути (Поставщик услуг по поиску пути).Звено между PathfindingSystem(DOTS) и MonoBehaviour скриптами.<br/>
/// Обрабатывает запрос и передает в PathfindingSystem
/// </summary>
public class PathfindingProvider
{
    public static PathfindingProvider Instance { get; private set; }

    public PathfindingProvider()
    {
        Instance = this;
    }

    public event EventHandler<NativeParallelHashMap<int3, PathNode>> OnPathfindingComplete;


    private UnitActionSystem _unitActionSystem;
    private MouseOnGameGrid _mouseOnGameGrid;
    private Unit _selectedUnit;
    private bool _isPathfindingComplete = false;

    private PathfindingParams _pathfindingParams;
    private Entity _entityPathfinding;
    private EntityManager _entityManager;

    public void Init(UnitActionSystem unitActionSystem, MouseOnGameGrid mouseOnGameGrid)
    {
        _unitActionSystem = unitActionSystem;
        _mouseOnGameGrid = mouseOnGameGrid;
        Setup();
    }

    private void Setup()
    {
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        _mouseOnGameGrid.OnMouseGridPositionChanged += MouseOnGameGrid_OnMouseGridPositionChanged;

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingGridDate, PathfindingParams>().Build(_entityManager);// способ  Создадия конструктора для обращения к сущности у которых есть PathfindingGridDate
        _entityPathfinding = entityQuery.GetSingletonEntity(); //Получим сущность ПОИСКА ПУТИ
        _pathfindingParams = _entityManager.GetComponentData<PathfindingParams>(_entityPathfinding);//Получим копию PathfindingParams

        SetPathfindingForUnit(_unitActionSystem.GetSelectedUnit());
    }

    private void MouseOnGameGrid_OnMouseGridPositionChanged(object sender, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        if (_unitActionSystem.GetIsBusy())
            return;
        if (_unitActionSystem.GetSelectedAction() is not MoveAction) //Если это не MoveAction то выходим
            return;

        Debug.Log("Позиция мыши изменилась.Запустил расчет пути");
        /*EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingParams>().Build(_entityManager);// способ  Создадия конструктора для обращения к сущности 

         _pathfindingParams.startGridPosition = _startGridPositionUnit;      
         _pathfindingParams.endPosition = e.newMouseGridPosition.ParseInt3();
         _pathfindingParams.isPathfindingComplete = false;
         _entityManager.SetComponentData(_entityPathfinding, _pathfindingParams); // Для сущности _entityPathfinding перепишем PathfindingParams
         _entityManager.SetComponentEnabled<PathfindingParams>(_entityPathfinding, true); //Активируем PathfindingParams что бы запустить расчет

         _startEvent=false; */

        /* if (_entityManager.GetComponentData<PathfindingParams>(_entityPathfinding).isPathfindingComplete)//Завершен ли поиск пути.
         {



             *//* foreach (PathWorldPositionBufferElement bufferElement in _entityManager.GetBuffer<PathWorldPositionBufferElement>(_entityPathfinding, true))
              {
                  pathWorldPosition.Add(bufferElement.worldPosition);
              }*//*

             //   OnPathfindingComplete?.Invoke(this, pathWorldPosition);

         }*/
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit unit)
    {
        if (_selectedUnit != unit)//Если выбран другой юнит то выполним для него поиск пути
            SetPathfindingForUnit(unit);
    }
    /// <summary>
    /// Установим поиск пути для юнита
    /// </summary>
    private void SetPathfindingForUnit(Unit unit)
    {
        _isPathfindingComplete = false;
        _selectedUnit = unit;
        _pathfindingParams.startGridPosition = _selectedUnit.GetGridPosition().ParseInt3();
        _pathfindingParams.maxMoveDistance = _selectedUnit.GetAction<MoveAction>().GetMaxActionDistance();
        _entityManager.SetComponentData(_entityPathfinding, _pathfindingParams); // Для сущности _entityPathfinding перепишем PathfindingParams
        _entityManager.SetComponentEnabled<PathfindingParams>(_entityPathfinding, true); //Активируем PathfindingParams что бы запустить расчет
    }

    public void PathfindingComplete(NativeParallelHashMap<int3, PathNode> dictionary)
    {
        _isPathfindingComplete = true;
        OnPathfindingComplete?.Invoke(this, dictionary);
    }   

    /// <summary>
    /// Завершен ли поиск пути.
    /// </summary>
    public bool IsPathfindingComplete()
    {
        return _isPathfindingComplete;
    }



}
