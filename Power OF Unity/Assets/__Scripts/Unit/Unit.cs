using System;
using UnityEngine;

/// <summary>
/// �������� ����� ����� ��� MonoBehaviour
/// </summary>
/// <remarks>
/// �������������� ���������� �����
/// </remarks>
public class Unit
{
    public Unit(UnitTypeSO unitTypeSO, SoundManager soundManager)
    {
        _soundManager = soundManager;
        _healthSystem = new Health(unitTypeSO.GetBasicHealth(), _soundManager);
        _actionPointsSystem = new UnitActionPoints(this);
        _unitInventory = new UnitInventory(this);
        _unitEquipment = new UnitEquipment(this);

        switch (unitTypeSO)
        {
            case UnitFriendSO unitFriendSO:
                _unitTypeSO = unitFriendSO;
                _unitAmorType = unitFriendSO.GetUnitArmorType();
                _isEnemy = false;
                break;
            case UnitEnemySO unitEnemySO:
                _unitTypeSO = unitEnemySO;
                _isEnemy = true;
                break;
        }
    }


    public static event EventHandler OnAnyEnemyUnitSpawned; // ������� ����� ��������� ���������(���������) ����
    public static event EventHandler OnAnyUnitDead; // ������� ����� ���� ����    

    // ������� ������
    private bool _isEnemy;
    private GridPositionXZ _gridPosition;

    readonly UnitTypeSO _unitTypeSO;
    readonly Health _healthSystem;
    readonly UnitActionPoints _actionPointsSystem;
    readonly UnitInventory _unitInventory;
    readonly UnitEquipment _unitEquipment;
    private BaseAction[] _baseActionsArray; // ������ ������� �������� 
    private UnitArmorType _unitAmorType; //��� ����� �����        
    private Transform _unitCoreTransform;

    private LevelGrid _levelGrid;
    private TurnSystem _turnSystem;
    readonly SoundManager _soundManager;
    private UnitActionSystem _unitActionSystem;
    private HandleAnimationEvents _handleAnimationEvents;
    private CameraFollow _cameraFollow;
    private Rope _unitRope;
    private Transform _headTransform;

    /// <summary>
    /// ��������� ����� ��� ������� �� ������. (�������� transform � gridPosition ...)
    /// </summary>   
    public void SetupForSpawn(LevelGrid levelGrid, TurnSystem turnSystem, Transform unitCoreTransform, CameraFollow cameraFollow, UnitActionSystem unitActionSystem)
    {
        // ����� Unit ����������, �������� ��� ��������� � ����� � ������� � GridObjectUnitXZ(�������� �����) � ������ ������         

        _levelGrid = levelGrid;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _unitCoreTransform = unitCoreTransform;
        _cameraFollow = cameraFollow;
        _handleAnimationEvents = unitCoreTransform.GetComponentInChildren<HandleAnimationEvents>(true);
        _baseActionsArray = unitCoreTransform.GetComponents<BaseAction>();
        _unitRope = unitCoreTransform.GetComponent<Rope>();
        _headTransform = unitCoreTransform.Find("Head");

        _gridPosition = _levelGrid.GetGridPosition(unitCoreTransform.position); //������� �������� ������� � ����� ������
        _levelGrid.AddUnitAtGridPosition(_gridPosition, this); //������� ����� � ���� �����

        ISetupForSpawn[] iSetupForSpawnArray = unitCoreTransform.GetComponentsInChildren<ISetupForSpawn>(true);   // ������ ������������� � unitCoreTransform, ��� � ��� �������� ��������, ��� ������, ����������� ��������� ISetupForSpawn.             
        foreach (var iSetupForSpawn in iSetupForSpawnArray)
        {
            iSetupForSpawn.SetupForSpawn(this);
        }

        _healthSystem.SetupForSpawn();
        _actionPointsSystem.SetupForSpawn();

        _healthSystem.OnDead += HealthSystem_OnDead; // ������������� �� Event. ����� ����������� ��� ������ �����       

        if (_isEnemy)
            OnAnyEnemyUnitSpawned?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ���������(���������) ��������� ����. ������� ��������� ������� ����� ����������� ��� ���� ��������� ��������� ������
    }

    /// <summary>
    ///  ��������� ����� ��� ������ �������� ��� ������ �� ������� �����.
    /// </summary>
    private void SetupOnDestroyAndQuit()
    {
        _levelGrid.GetGridPosition(GetTransformPosition());
        _levelGrid.RemoveUnitAtGridPosition(_gridPosition, this);
        _actionPointsSystem.SetupOnDestroyAndQuit();

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

    public bool IsEnemy() // �������� ����
    {
        return _isEnemy;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) // ����� ����������� ��� ������ �����
    {
        SetupOnDestroyAndQuit();
        UnityEngine.Object.Destroy(_unitCoreTransform.gameObject); // ��������� ������� ������ � �������� ���������� ������ ������

        // ������� ������ ��������� ����� ���� ������� ��� ���������� ����� ��� ���������� � UnitActionSystem    

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ������� ����. ������� ��������� ������� ����� ����������� ��� ������ �������� �����      
    }

    public Health GetHealthSystem() { return _healthSystem; }
    public UnitActionPoints GetActionPointsSystem() { return _actionPointsSystem; }
    public UnitInventory GetUnitInventory() { return _unitInventory; }
    public UnitEquipment GetUnitEquipment() { return _unitEquipment; }
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
    public UnitArmorType GetUnitArmorType() { return _unitAmorType; }
    public TurnSystem GetTurnSystem() { return _turnSystem; }
    public SoundManager GetSoundManager() { return _soundManager; }
    public UnitActionSystem GetUnitActionSystem() { return _unitActionSystem; }
    public LevelGrid GetLevelGrid() { return _levelGrid; }
    public HandleAnimationEvents GetHandleAnimationEvents() { return _handleAnimationEvents; }
    public CameraFollow GetCameraFollow() { return _cameraFollow; }
    public Rope GetUnitRope() { return _unitRope; }
    public Transform GetHeadTransform() { return _headTransform; }
}

