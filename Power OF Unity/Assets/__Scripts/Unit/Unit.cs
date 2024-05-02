using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour // ���� ���� ����� �������� �� ������� �� ����� � ���� ��������, ��������� �����
{

    private const int ACTION_POINTS_MAX = 5; //���� ���������//

    // ��� // ������� 2 //� UnitActionSystemUI
    // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
    // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������,
    // ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 
    public static event EventHandler OnAnyActionPointsChanged;  // ��������� ����� �������� � ������(Any) ����� � �� ������ � ����������.                                                           
    public static event EventHandler OnAnyFriendlyChangeHealth; //� ������ �������������� ����� ���������� ��������   
    public static event EventHandler OnAnyEnemyUnitSpawned; // ������� ����� ��������� ���������(���������) ����
    public static event EventHandler OnAnyUnitDead; // ������� ����� ���� ����    


    [SerializeField] private bool _isEnemy; //� ���������� � ������� ����� ��������� �������

    // ������� ������
    private GridPositionXZ _gridPosition;
    private Health _healthSystem; 
    private BaseAction[] _baseActionsArray; // ������ ������� �������� // ����� ������������ ��� �������� ������   
    private List<BaseAction> _baseActionsOnEquipmentList; // ������ ������� �������� ������� �� ����������
    private Rope _unitRope;
    private int _actionPoints = ACTION_POINTS_MAX; // ���� ��������
    private float _penaltyStunPercent;  // �������� ������� ��������� (����� ��������� � ���� ���)
    private bool _stunned = false; // ����������(�� ��������� ����)
    private SingleNodeBlocker _singleNodeBlocker; // ����������� ���� �� ����� �����
    /*private int _startStunTurnNumber; //����� ������� (����) ��� ������ ������� ���������
    private int _durationStunEffectTurnNumber; // ����������������� ����������� ������� ���������� �����*/

    private TurnSystem _turnSystem;
    private LevelGrid _levelGrid;
    private bool _onLevel = false; // �� ������


    /// <summary>
    /// ��������� ����� ��� ������� �� ������. (�������� transform � gridPosition ...)
    /// </summary>   
    public void SetupUnitForSpawn(Transform PointSpawnTRansform, TurnSystem turnSystem, LevelGrid levelGrid)
    {
        // ����� Unit ����������, �������� ��� ��������� � ����� � ������� � GridObjectUnitXZ(�������� �����) � ������ ������
        transform.position = PointSpawnTRansform.position;
        transform.rotation = PointSpawnTRansform.rotation;

        _turnSystem = turnSystem;
        _levelGrid = levelGrid;

        _gridPosition = _levelGrid.GetGridPosition(PointSpawnTRansform.position); //������� �������� ������� � ����� ������
        _levelGrid.AddUnitAtGridPosition(_gridPosition, this); //������� ����� � ���� �����
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������. �� ������� ��� �������     

        GetComponent<FloorVisibility>().SetupUnitForSpawn();
        _onLevel = true;
    }
    /// <summary>
    ///  ��������� ����� ��� ��������� ������.
    /// </summary>
    public void SetupEndLevel()
    {
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void Awake()
    {
        _healthSystem = GetComponent<Health>();

        if (TryGetComponent<Rope>(out Rope unitRope))
        {
            _unitRope = unitRope;
        }

        _singleNodeBlocker = GetComponent<SingleNodeBlocker>();      
        _baseActionsArray = GetComponents<BaseAction>(); // _moveAction � _spinAction ����� ����� ��������� ������ ����� �������
    }

    private void OnEnable()
    {          
        _healthSystem.OnDead += HealthSystem_OnDead; // ������������� �� Event. ����� ����������� ��� ������ �����

        if (_isEnemy)
            OnAnyEnemyUnitSpawned?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ���������(���������) ��������� ����. ������� ��������� ������� ����� ����������� ��� ���� ��������� ��������� ������
    }

    private void OnDestroy()
    {       
        _healthSystem.OnDead -= HealthSystem_OnDead;
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (!_onLevel) return; // ���� �� �� ������� ������ �� �������
        // ����� �������������� ���� �������� ������� � MoveAction � GrappleAction
        GridPositionXZ newGridPosition = _levelGrid.GetGridPosition(transform.position); //������� ����� ������� ����� �� �����.
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
        return transform.position;
    }

    public BaseAction[] GetBaseActionsArray() // �������� ������ ������� ��������
    {
        return _baseActionsArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) // ��������� ��������� ���� ��������, ����� ��������� �������� // ���� ����� ��������� ������ ��� ������ ������
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) //�� ����� ��������� ���� ��������, ����� ��������� �������� ? 
    {
        if (_actionPoints >= baseAction.GetActionPointCost()) // ���� ����� �������� ������� ��...
        {
            return true; // ����� ��������� ��������
        }
        else
        {
            return false; // ��� ����� �� �������
        }

        /*// �������������� ������ ���� ����
        return _actionPoints >= baseAction.GetActionPointCost();*/
    }

    public void SpendActionPoints(int amount) //��������� ���� �������� (amount- ���������� ������� ���� ���������)
    {
        _actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� UnitActionSystemUI)
    }

    public int GetActionPoints() // �������� ���� ��������
    {
        return _actionPoints;
    }


    public void TurnSystem_OnTurnChanged(object sender, EventArgs empty) //��� ������� ������� ���� �������� �� ������������
    {
        if ((IsEnemy() && !_turnSystem.IsPlayerTurn()) || // ���� ��� ���� � ��� ������� (�� ������� ������) ��� ��� �� ����(�����) � ������� ������ ��...
            (!IsEnemy() && _turnSystem.IsPlayerTurn()))
        {
            _actionPoints = ACTION_POINTS_MAX;

            if (_penaltyStunPercent != 0)
            {
                _actionPoints -= Mathf.RoundToInt(_actionPoints * _penaltyStunPercent); // �������� �����
                _penaltyStunPercent = 0;
                SetStunned(false); // �������� ���������
            }

            //  ����� ������� ���������������� �� ���� ���
            /*int passedTurnNumber = _turnSystem.GetTurnNumber() - _startStunTurnNumber;// ������ ����� �� ������ ���������
            if (passedTurnNumber <= _durationStunEffectTurnNumber) // ���� ����� ������ ������ ��� ����� ������������ ��������� (������ ��������� ��� ���������)
            {
                _actionPoints -= Mathf.RoundToInt(_actionPoints * _penaltyStunPercent); // �������� �����
                _penaltyStunPercent = _penaltyStunPercent *0.3f; // �������� ����� ������� 30% �� ������������ (��� ���� ���� ��������� ������� ��������� �����)
            }
            if (passedTurnNumber > _durationStunEffectTurnNumber) //���� ����� ������ ������ ����������������� ���������
            {
                SetStunned(false); // �������� ���������
                _penaltyStunPercent = 0; // � ������� �����
            }  */

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� UnitActionSystemUI)
        }
    }

    public bool IsEnemy() // �������� ����
    {
        return _isEnemy;
    }

    public void Healing(int healingAmount) // ��������� (� �������� �������� �������� ����������������� ��������)
    {
        _healthSystem.Healing(healingAmount);
        if (!_isEnemy)// ���� �� ���� ��
        {
            OnAnyFriendlyChangeHealth?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Damage(int damageAmount) // ���� (� �������� �������� �������� �����������)
    {
        _healthSystem.Damage(damageAmount);
        if (!_isEnemy)// ���� �� ���� ��
        {
            OnAnyFriendlyChangeHealth?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Stun(float stunPercent) // �������� �� stunPercent(������� ���������)
    {
        SetStunned(true);
        _penaltyStunPercent = stunPercent; // ��������� �������� ���������

        /*// ����� ������� ���������������� �� ���� ���
        _startStunTurnNumber = _turnSystem.GetTurnNumber(); // ������� ��������� ����� ����              
        if (_actionPoints > 0) // ���� ����� ���� ������ ����
        {
            _durationStunEffectTurnNumber = 1; //����� ���������// ��������� ����� ������� ���� ��������� ���
        }
        if(_actionPoints<=0) // ���� ����� ���� ����
        {
            _durationStunEffectTurnNumber = 3; //����� ���������// ��������� ����� ������� ��������� 3 ���� (����� ��� �����)
        }*/
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.
    }


    private void HealthSystem_OnDead(object sender, EventArgs e) // ����� ����������� ��� ������ �����
    {
        _levelGrid.RemoveUnitAtGridPosition(_gridPosition, this); // ������ �� �������� ������� �������� �����

        Destroy(gameObject); // ��������� ������� ������ � �������� ���������� ������ ������

        // ������� ������ ��������� ����� ���� ������� ��� ���������� ����� ��� ���������� � UnitActionSystem    

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ������� ����. ������� ��������� ������� ����� ����������� ��� ������ �������� �����      
    }

    public float GetHealthNormalized() // �������� ��� ������
    {
        return _healthSystem.GetHealthNormalized();
    }
    public int GetHealth() // �������� ��� ������
    {
        return _healthSystem.GetHealth();
    }
    public int GetHealthMax() // �������� ��� ������
    {
        return _healthSystem.GetHealthMax();
    }
    public bool IsDead()
    {
        return _healthSystem.IsDead();
    }
    public Health GetHealthSystem()
    {
        return _healthSystem;
    }

    public Rope GetUnitRope()
    {
        return _unitRope;
    }

    public bool GetStunned()
    {
        return _stunned;
    }
    private void SetStunned(bool stunned)
    {
        _stunned = stunned;
    }
    public SingleNodeBlocker GetSingleNodeBlocker()
    {
        return _singleNodeBlocker;
    }
}
