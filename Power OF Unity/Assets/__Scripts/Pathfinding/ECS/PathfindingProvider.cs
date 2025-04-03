using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// ��������� ������ ���� (��������� ����� �� ������ ����).����� ����� PathfindingSystem(DOTS) � MonoBehaviour ���������.<br/>
/// ������������ ������ � �������� � PathfindingSystem
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

        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingGridDate, PathfindingParams>().Build(_entityManager);// ������  �������� ������������ ��� ��������� � �������� � ������� ���� PathfindingGridDate
        _entityPathfinding = entityQuery.GetSingletonEntity(); //������� �������� ������ ����
        _pathfindingParams = _entityManager.GetComponentData<PathfindingParams>(_entityPathfinding);//������� ����� PathfindingParams

        SetPathfindingForUnit(_unitActionSystem.GetSelectedUnit());
    }

    private void MouseOnGameGrid_OnMouseGridPositionChanged(object sender, MouseOnGameGrid.OnMouseGridPositionChangedEventArgs e)
    {
        if (_unitActionSystem.GetIsBusy())
            return;
        if (_unitActionSystem.GetSelectedAction() is not MoveAction) //���� ��� �� MoveAction �� �������
            return;

        Debug.Log("������� ���� ����������.�������� ������ ����");
        /*EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<PathfindingParams>().Build(_entityManager);// ������  �������� ������������ ��� ��������� � �������� 

         _pathfindingParams.startGridPosition = _startGridPositionUnit;      
         _pathfindingParams.endPosition = e.newMouseGridPosition.ParseInt3();
         _pathfindingParams.isPathfindingComplete = false;
         _entityManager.SetComponentData(_entityPathfinding, _pathfindingParams); // ��� �������� _entityPathfinding ��������� PathfindingParams
         _entityManager.SetComponentEnabled<PathfindingParams>(_entityPathfinding, true); //���������� PathfindingParams ��� �� ��������� ������

         _startEvent=false; */

        /* if (_entityManager.GetComponentData<PathfindingParams>(_entityPathfinding).isPathfindingComplete)//�������� �� ����� ����.
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
        if (_selectedUnit != unit)//���� ������ ������ ���� �� �������� ��� ���� ����� ����
            SetPathfindingForUnit(unit);
    }
    /// <summary>
    /// ��������� ����� ���� ��� �����
    /// </summary>
    private void SetPathfindingForUnit(Unit unit)
    {
        _isPathfindingComplete = false;
        _selectedUnit = unit;
        _pathfindingParams.startGridPosition = _selectedUnit.GetGridPosition().ParseInt3();
        _pathfindingParams.maxMoveDistance = _selectedUnit.GetAction<MoveAction>().GetMaxActionDistance();
        _entityManager.SetComponentData(_entityPathfinding, _pathfindingParams); // ��� �������� _entityPathfinding ��������� PathfindingParams
        _entityManager.SetComponentEnabled<PathfindingParams>(_entityPathfinding, true); //���������� PathfindingParams ��� �� ��������� ������
    }

    public void PathfindingComplete(NativeParallelHashMap<int3, PathNode> dictionary)
    {
        _isPathfindingComplete = true;
        OnPathfindingComplete?.Invoke(this, dictionary);
    }   

    /// <summary>
    /// �������� �� ����� ����.
    /// </summary>
    public bool IsPathfindingComplete()
    {
        return _isPathfindingComplete;
    }



}
