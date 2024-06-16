using System;
using System.Collections.Generic;
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
        _healthSystem = new Health(_unitTypeSO.GetBasicHealth(), soundManager);
        _actionPointsSystem = new ActionPoints(this);
        _baseActionsList = new List<BaseAction>();

        switch (unitTypeSO)
        {
            case UnitFriendSO unitFriendSO:
                _unitTypeSO = unitFriendSO;
                _unitAmorType = unitFriendSO.GetUnitArmorType();
                _isEnemy = false;
                break;
            case UnitEnemySO unitEnemySO:
                _unitTypeSO = unitEnemySO;
                _baseActionsList = unitEnemySO.GetBaseActionsList();
                _isEnemy = true;
                break;
        }

        _unitInventory = new UnitInventory(this);
        _unitEquipment = new UnitEquipment(this);
                
        _soundManager = soundManager;
    }


    public static event EventHandler OnAnyEnemyUnitSpawned; // ������� ����� ��������� ���������(���������) ����
    public static event EventHandler OnAnyUnitDead; // ������� ����� ���� ����    

    // ������� ������
    private bool _isEnemy;
    private GridPositionXZ _gridPosition;

    private UnitTypeSO _unitTypeSO;
    private Health _healthSystem;
    private ActionPoints _actionPointsSystem;
    private UnitInventory _unitInventory;
    private UnitEquipment _unitEquipment;
    private List<BaseAction> _baseActionsList; // ������ ������� �������� // ����� ������������ ��� �������� ������   
    private UnitArmorType _unitAmorType; //��� ����� �����        
    private Transform _unitCoreTransform;

    private LevelGrid _levelGrid;
    private TurnSystem _turnSystem;
    private SoundManager _soundManager;
    private UnitActionSystem _unitActionSystem;

    /// <summary>
    /// ��������� ����� ��� ������� �� ������. (�������� transform � gridPosition ...)
    /// </summary>   
    public void SetupForSpawn( LevelGrid levelGrid, TurnSystem turnSystem, Transform unitCoreTransform, CameraFollow cameraFollow, UnitActionSystem unitActionSystem)
    {
        // ����� Unit ����������, �������� ��� ��������� � ����� � ������� � GridObjectUnitXZ(�������� �����) � ������ ������         
        
        _levelGrid = levelGrid;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _gridPosition = _levelGrid.GetGridPosition(unitCoreTransform.position); //������� �������� ������� � ����� ������
        _levelGrid.AddUnitAtGridPosition(_gridPosition, this); //������� ����� � ���� �����
        _unitCoreTransform = unitCoreTransform;

        MoveAction moveAction = unitCoreTransform.GetComponent<MoveAction>();
        UnitWorldUI unitWorldUI = unitCoreTransform.GetComponentInChildren<UnitWorldUI>(true);
        LookAtCamera lookAtCamera = unitCoreTransform.GetComponentInChildren<LookAtCamera>(true);
        HandleAnimationEvents handleAnimationEvents = unitCoreTransform.GetComponentInChildren<HandleAnimationEvents>(true);
        UnitRagdollSpawner unitRagdollSpawner = unitCoreTransform.GetComponentInChildren<UnitRagdollSpawner>(true);

        _baseActionsList.Add(moveAction); // ������� ������������ � ������� ��������
        moveAction.SetupForSpawn(this, unitActionSystem, _unitTypeSO.GetBasicMoveDistance());
        unitWorldUI.SetupForSpawn(this, unitActionSystem, _turnSystem);
        lookAtCamera.SetupForSpawn(cameraFollow);
        handleAnimationEvents.SetupForSpawn(this);
        unitRagdollSpawner.SetupForSpawn(this);
        _healthSystem.SetupForSpawn();
        _actionPointsSystem.SetupForSpawn(_turnSystem);

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

    public void AddBaseActionsList(BaseAction baseAction)
    {
        _baseActionsList.Add(baseAction);
    }
    public void RemoveBaseActionsList(BaseAction baseAction)
    {
        _baseActionsList.Remove(baseAction);
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
        foreach (BaseAction baseAction in _baseActionsList) // ��������� ������ ������� ��������
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

    public List<BaseAction> GetBaseActionsList() // �������� ������ ������� ��������
    {
        return _baseActionsList;
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
    public ActionPoints GetActionPointsSystem() { return _actionPointsSystem; }
    public UnitInventory GetUnitInventory() { return _unitInventory; }
    public UnitEquipment GetUnitEquipment() { return _unitEquipment; }
    public Vector3 GetTransformPosition() { return _unitCoreTransform.position; }
    public void SetTransformPosition(Vector3 position) {  _unitCoreTransform.position = position; }
    public Transform GetTransform() { return _unitCoreTransform; }
    public void SetTransformForward(Vector3 forward) {  _unitCoreTransform.forward = forward; }
    public T GetUnitTypeSO<T>() where T : UnitTypeSO //�������� ��� ��������� ������ ���������� �� UnitTypeSO
    {
        if (_unitTypeSO is T unit_T_SO)
        {
            return unit_T_SO;
        }
        return null; // ���� ��� ���������� �� ������ ����
    }
    public UnitArmorType GetUnitArmorType() { return _unitAmorType; }
    public TurnSystem GetTurnSystem() { return _turnSystem; }
    public SoundManager GetSoundManager() { return _soundManager; }
    public UnitActionSystem GetUnitActionSystem() { return _unitActionSystem; } 
    public LevelGrid GetLevelGrid() { return _levelGrid;}

}

