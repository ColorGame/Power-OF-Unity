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
/// ��������� ������ ���� (��������� ����� �� ������ ����).����� ����� PathfindingSystem(DOTS) � MonoBehaviour ���������.<br/>
/// ������������ ������ � �������� � PathfindingSystem
/// </summary>
[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //�������� � ������
[UpdateAfter(typeof(PathNodeSystem))] //���������� ����� PathNodeSystem 
public partial class PathfindingProviderSystem : SystemBase
{

    public event EventHandler OnPathfindingComplete;

    /// <summary>
    /// ������� - ���������� ������� ����� � ���� ���� <br/>
    /// PathNode - �������� ���� � ����
    /// </summary>
    private NativeHashMap<int3, PointsPath> _validGridPositionPointsPathDict; //�������. ���� - ���������� �������� �������. �������� - ����� ���� ��� ���� �������� �������
    /// <summary>
    /// ������ ���������� �������� ������� ������������ ��� ���������� �����
    /// </summary>
    private List<GridPositionXZ> _validGridPositionMoveForSelectedUnitList = new List<GridPositionXZ>();

    private UnitActionSystem _unitActionSystem;
    private MouseOnGameGrid _mouseOnGameGrid;
    private LevelGrid _levelGrid;
    private Unit _selectedUnit;
    private bool _isPathfindingComplete = false;

    /// <summary>
    /// ��� ������������� ��������� ���� UnitActionSystem, LevelGrid, UnitSpawnerOnLevel
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
        if (_unitActionSystem.GetSelectedAction() is not MoveAction) //���� ��� �� MoveAction �� �������
            return;

        //  Debug.Log("������� ���� ����������.�������� ������ ����");
    }

    private void UnitActionSystem_OnBusyChanged(object sender, UnitActionSystem.BusyChangedEventArgs e)
    {
        if (e.selectedAction is MoveAction && !e.isBusy) //���� ���� �������� � �����������(�� �����) �� 
        {
            if (_selectedUnit.GetActionPointsSystem().CanSpendActionPointsToTakeAction(e.selectedAction))//���� ������� ����� ��� �����������(is MoveAction) �� ��������� ����
            {
                Debug.Log("��������� ����� ���� ��� ����� OnBusyChanged");
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

        if (unit.GetActionPointsSystem().CanSpendActionPointsToTakeAction(unit.GetAction<MoveAction>()))//���� ������� ����� ��� ����������� �� ��������� ����
        {
          //  Debug.Log("��������� ����� ���� ��� ����� OnSelectedUnitChanged");
            SetPathfindingForSelectedUnit();
        }
        else
        {
            _validGridPositionMoveForSelectedUnitList.Clear();
            OnPathfindingComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// ��������� ����� ���� ��� ���������� �����
    /// </summary>
    private void SetPathfindingForSelectedUnit()
    {
        _isPathfindingComplete = false;
        _validGridPositionMoveForSelectedUnitList.Clear();

        foreach ((RefRW<PathfindingParams> pathfindingParams, Entity pathfindingEntity) in SystemAPI.Query<RefRW<PathfindingParams>>().WithPresent<PathfindingParams>().WithEntityAccess())
        {
            pathfindingParams.ValueRW.startGridPosition = _selectedUnit.GetGridPosition().ParseInt3();
            pathfindingParams.ValueRW.maxMoveDistance = _selectedUnit.GetAction<MoveAction>().GetMaxActionDistance();

            SystemAPI.SetComponentEnabled<PathfindingParams>(pathfindingEntity, true); //���������� PathfindingParams ��� �� ��������� ������
        }
    }

    protected override void OnUpdate()
    {
        // ��� ��������� ValidGridPositionPointsPathDict �������� ������ �� ������� ������ ���� 
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
    /// ������ ���� ��������
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
    /// �������� ���������� �������� ������� ������������ ��� ���������� �����
    /// </summary>
    public List<GridPositionXZ> GetValidGridPositionMoveForSelectedUnit() { return _validGridPositionMoveForSelectedUnitList; }

    /// <summary>
    /// �������� ������ �� ������� - ���������� ������� ����� � ���� ����
    /// </summary>
    public NativeHashMap<int3, PointsPath> GetvalidGridPositionPointsPathDict() { return _validGridPositionPointsPathDict; }

    /// <summary>
    /// �������� �� ����� ����.
    /// </summary>
    public bool IsPathfindingComplete() { return _isPathfindingComplete; }

}
