using System;
using UnityEngine;
using static Unit;

/// <summary>
/// �������� ����� ����� (��� MonoBehaviour)
/// </summary>
/// <remarks>
/// �������������� ���������� �����
/// </remarks>
public class Unit
{
    /// <summary>
    /// �������������� ����� �������, �����
    /// </summary>
    public enum Location
    {
        Barrack = 0,
        Mission = 1,
    }
    /// <summary>
    /// ��������� �����
    /// </summary>
    public enum UnitState
    {
        Idle = 0,
        Hold = 1,
    }


    public Unit(UnitTypeSO unitTypeSO, SoundManager soundManager, HashAnimationName hashAnimationName)
    {
        if (unitTypeSO is UnitEnemySO)
            _isEnemy = true;

        _unitTypeSO = unitTypeSO;
        _soundManager = soundManager;
        _healthSystem = new HealthSystem(unitTypeSO.GetBasicHealth(), _soundManager);
        _actionPointsSystem = new UnitActionPoints(this, unitTypeSO.GetBasicActionPoints());
        _unitEquipment = new UnitEquipment();
        _unitEquipsViewFarm = new UnitEquipViewFarm(this);
        _unitAnimator = new UnitAnimator(this, hashAnimationName);
        _unitPowerSystem = new UnitPowerSystem(unitTypeSO.GetBasicPower());
        _unitAccuracySystem = new UnitAccuracySystem(unitTypeSO.GetBasicAccuracy());

        _location = Location.Mission; // �� ��������� ��� ����� ���������� �� ������
        _completedMissionsCount = 0;
        _killedEnemiesCount = 0;
    }

    public static event EventHandler<Unit> OnAnyEnemyUnitSpawned; // ������� ����� ��������� ���������(���������) ����


    public static event EventHandler<Unit> OnAnyUnitDead; // ������� ����� ���� ����    


    readonly UnitTypeSO _unitTypeSO;
    readonly HealthSystem _healthSystem;
    readonly UnitActionPoints _actionPointsSystem;
    readonly UnitEquipment _unitEquipment;
    readonly UnitEquipViewFarm _unitEquipsViewFarm;
    readonly UnitAnimator _unitAnimator;
    readonly UnitPowerSystem _unitPowerSystem;
    readonly UnitAccuracySystem _unitAccuracySystem;
    readonly bool _isEnemy = false;

    private BaseAction[] _baseActionsArray; // ������ ������� ��������    
    private Transform _unitCoreTransform;

    private LevelGrid _levelGrid;
    private TurnSystem _turnSystem;
    readonly SoundManager _soundManager;
    private UnitActionSystem _unitActionSystem;
    private HandleAnimationEvents _handleAnimationEvents;
    private CameraFollow _cameraFollow;
    private PathfindingProvider _pathfindingProvider;
    private Rope _unitRope;
    private Transform _headTransform;

    private GridPositionXZ _gridPosition;
    private Location _location;
    private UnitState _unitState;

    private int _completedMissionsCount;
    private int _killedEnemiesCount;


    /// <summary>
    /// ��������� ����� ��� ������� �� ������. (�������� transform � gridPosition ...)
    /// </summary>   
    public virtual void SetupForSpawn(LevelGrid levelGrid, TurnSystem turnSystem, Transform unitCoreTransform, CameraFollow cameraFollow, UnitActionSystem unitActionSystem,PathfindingProvider pathfindingProvider)
    {
        // ����� Unit ����������, �������� ��� ��������� � ����� � ������� � GridObjectUnitXZ(�������� �����) � ������ ������         

        _levelGrid = levelGrid;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _unitCoreTransform = unitCoreTransform;
        _cameraFollow = cameraFollow;
        _pathfindingProvider = pathfindingProvider;
        _handleAnimationEvents = unitCoreTransform.GetComponentInChildren<HandleAnimationEvents>(true);
        _baseActionsArray = unitCoreTransform.GetComponents<BaseAction>();
        _headTransform = unitCoreTransform.Find("Head");
        _unitRope = unitCoreTransform.GetComponent<Rope>();
        _gridPosition = _levelGrid.GetGridPosition(unitCoreTransform.position); //������� �������� ������� � ����� ������
        _levelGrid.AddUnitAtGridPosition(_gridPosition, this); //������� ����� � ���� �����
        _unitState = UnitState.Hold;

        ISetupForSpawn[] iSetupForSpawnArray = unitCoreTransform.GetComponentsInChildren<ISetupForSpawn>(true);   // ������ ������������� � unitCoreTransform, ��� � ��� �������� ��������, ��� ������, ����������� ��������� ISetupForSpawn.             
        foreach (var iSetupForSpawn in iSetupForSpawnArray)
        {
            iSetupForSpawn.SetupForSpawn(this);
        }

        _healthSystem.SetupForSpawn();
        _actionPointsSystem.SetupForSpawn();

        _healthSystem.OnDead += HealthSystem_OnDead; // ������������� �� Event. ����� ����������� ��� ������ �����      

        if (_isEnemy)
            OnAnyEnemyUnitSpawned?.Invoke(this, this); // �������� ������� ����� ���������(���������) ��������� ����. ������� ��������� ������� ����� ����������� ��� ���� ��������� ��������� ������
    }

    /// <summary>
    ///  ��������� ����� ��� ������ �������� ��� ������ �� ������� �����.
    /// </summary>
    private void SetupOnDestroyAndQuit()
    {
        _levelGrid.GetGridPosition(GetTransformPosition());
        _levelGrid.RemoveUnitAtGridPosition(_gridPosition, this);
        _actionPointsSystem.SetupOnDestroyAndQuit();


        _unitAnimator.SetupOnDestroyAndQuit();
        _unitEquipsViewFarm.SetupOnDestroyAndQuit();

        _healthSystem.OnDead -= HealthSystem_OnDead;
    }

    public void UpdateGridPosition()
    {
        GridPositionXZ newGridPosition = _levelGrid.GetGridPosition(GetTransformPosition()); //������� ����� ������� ����� �� �����.
        if (newGridPosition != _gridPosition) // ���� ����� ������� �� ����� ���������� �� ��������� �� ...
        {
            // ������� ��������� ����� �� �����
            GridPositionXZ oldGridPosition = _gridPosition; // �������� ������ ������� ��� �� �������� � event
            _gridPosition = newGridPosition; //������� ������� - ����� ������� ����������� �������

            _levelGrid.UnitMovedGridPosition(this, oldGridPosition, newGridPosition); //� UnitMovedGridPosition ��������� �������. ������� ��� ������ �������� � ������ . ����� �� ��������� ������� ����� ����������� � ���� ��� �� ���������
        }
    }

    public T GetAction<T>() where T : BaseAction //�������� ��� ��������� ������ ���� �������� �������� // �������� ����� � GENERICS � ��������� ������ �  BaseAction
    {
        foreach (BaseAction baseAction in _baseActionsArray) // ��������� ������ ������� ��������
        {
            if (baseAction is T) // ���� T ��������� � ����� ������ baseAction �� ...
            {
                return baseAction as T; // ������ ��� ������� �������� ��� � // (T)baseAction; - ��� ���� ����� ������
            }
        }
        return null; // ���� ��� ���������� �� ������ ����
    }

    public GridPositionXZ GetGridPosition() // �������� �������� �������
    {
        return _gridPosition;
    }

    public Vector3 GetWorldPosition() // �������� ������� �������
    {
        return GetTransformPosition();
    }

    public BaseAction[] GetBaseActionsArray() // �������� ������ ������� ��������
    {
        return _baseActionsArray;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) // ����� ����������� ��� ������ �����
    {
        SetupOnDestroyAndQuit();
        UnityEngine.Object.Destroy(_unitCoreTransform.gameObject); // ��������� ������� ������ � �������� ���������� ������ ������

        // ������� ������ ��������� ����� ���� ������� ��� ���������� ����� ��� ���������� � UnitActionSystem    

        OnAnyUnitDead?.Invoke(this, this); // �������� ������� ����� ������� ����. ������� ��������� ������� ����� ����������� ��� ������ �������� �����      
    }

    public HealthSystem GetHealthSystem() { return _healthSystem; }
    public UnitActionPoints GetActionPointsSystem() { return _actionPointsSystem; }
    public UnitEquipment GetUnitEquipment() { return _unitEquipment; }
    public UnitEquipViewFarm GetUnitEquipsViewFarm() { return _unitEquipsViewFarm; }
    public UnitPowerSystem GetUnitPowerSystem() { return _unitPowerSystem; }
    public UnitAccuracySystem GetUnitAccuracySystem() { return _unitAccuracySystem; }
    public bool GetIsEnemy() { return _isEnemy; }
    public Rope GetUnitRope() { return _unitRope; }
    public Vector3 GetTransformPosition() { return _unitCoreTransform.position; }
    public void SetTransformPosition(Vector3 position) { _unitCoreTransform.position = position; }
    public Transform GetTransform() { return _unitCoreTransform; }
    public void SetTransformForward(Vector3 forward) { _unitCoreTransform.forward = forward; }
    public T GetUnitTypeSO<T>() where T : UnitTypeSO //�������� ��� ��������� ������ ���������� �� UnitTypeSO
    {
        if (_unitTypeSO is T unit_T_SO)
        {
            return unit_T_SO;
        }
        return null; // ���� ��� ���������� �� ������ ����
    }
    public UnitTypeSO GetUnitTypeSO() { return _unitTypeSO; }
    public TurnSystem GetTurnSystem() { return _turnSystem; }
    public SoundManager GetSoundManager() { return _soundManager; }
    public UnitActionSystem GetUnitActionSystem() { return _unitActionSystem; }
    public PathfindingProvider GetPathfindingProvider() { return _pathfindingProvider; }
    public LevelGrid GetLevelGrid() { return _levelGrid; }
    public HandleAnimationEvents GetHandleAnimationEvents() { return _handleAnimationEvents; }
    public CameraFollow GetCameraFollow() { return _cameraFollow; }
    public Transform GetHeadTransform() { return _headTransform; }
    public UnitAnimator GetUnitAnimator() { return _unitAnimator; }
    public int GetCompletedMissionsCount() { return _completedMissionsCount; }
    public int GetKilledEnemiesCount() { return _killedEnemiesCount; }
    public void SetLocation(Location location) { _location = location; }
    public Location GetLocation() { return _location; }
    public void SetUnitState(UnitState unitState) { _unitState = unitState; }
    public UnitState GetUnitState() { return _unitState; }
}

