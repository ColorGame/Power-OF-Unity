using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour // ������������ �������� �����
{

    private enum State // ��������� �����
    {
        WaitingForEnemyTurn,// �������� ���������� ����(���� ������)
        TakingTurn,         // ��������� ��� (������� ���)
        Busy,               // �������
    }

    private State _state; // ������� ���������
    private float _timer;
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;

    public void Init(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
    }

    private void Awake()
    {
        _state = State.WaitingForEnemyTurn; // ��������� ��������� "�������� ���������� ����" �� ���������, �.�. ����� ����� ������ ������
    }


    private void Start()
    {
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ���������� �� ������� ��� �������
    }

    private void Update()
    {
        if (_turnSystem.IsPlayerTurn()) // ��������� ��� ��� ����� ���� ����� ����� �� ���������� ���������� (������ �������� �� �����)
        {
            return;
        }

        switch (_state) // ������� ��������
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0) // ������ ���� ����� �� ���������� ��������� �������� ...
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))   //�������� �� ����� ����������� ��������� �������� ���������� ������������� ���������.� �������� �������� ������� ������� ������ ��� ����� � �������� ��������� TakingTurn,
                                                                    // � ���� ����� �� ��������� _timer ���������� �������� ���� �� ��������� ��� ��������� �������� ��
                    {
                        _state = State.Busy; // ���������� �� ��������� "�������" ���� ���� ��� ��������
                    }
                    else
                    {
                        // � ������ ��� �������� ������� ����� ���������,  ��������� ��� ��������
                        _turnSystem.NextTurn(); // �������� � ���������� ����
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn() // ���������� ��������� ��������� ��� (������� ���)
    {
        _timer = 0.5f; // ����� �� ���� ���������
        _state = State.TakingTurn; // �������� ��������� ��������� ��� (������� ���)
    }

    private void TurnSystem_OnTurnChanged(object seder, EventArgs emptu) // �� ����� ����� ���� ��������� ������
    {
        if (!_turnSystem.IsPlayerTurn()) // �������� ��� ��� �� ��� ������
        {
            _state = State.TakingTurn; // �������� ��������� ��������� ��� (������� ���)
            _timer = 2;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)   // ����������� ��������� �������� ���������� ������������� ���������. � �������� �������� ������� onEnemyAIActionComplete (�������� ���������� ������������� ��������� ���������)
                                                                        // ������� �� ������ ������ � �������� ��������� ��������
    {        
        foreach (Unit enemyUnit in _unitManager.GetUnitEnemyList()) // � ����� ��������� ������ � ������ ������
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))  // �������� ����� �� �� ����������� �������� �������� ��
            {
                return true;
            }
        }
        return false; // ���� ������ �� ������ �� ������ ����
    }

    //������� ����� �� � ������ ����������
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) // ��������� �������� ���������� ������������� ��������� ��� enemyUnit. � �������� �������� ������� onEnemyAIActionComplete (�������� ���������� ������������� ��������� ���������) � ���������� �����
    {
        EnemyAIAction bestEnemyAIAction = null; // ������� �������� ���������� �� �� ��������� = null 
        BaseAction bestBaseAction = null; // ������ ������� �������� �� ��������� = null 

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionsList()) // ��������� ������ ������� �������� � ������ ������
        {
            if (!enemyUnit.GetActionPointsSystem().CanSpendActionPointsToTakeAction(baseAction)) // ������� � enemyUnit �� ����� ��������� ���� ��������, ����� ��������� �������� ? ���� �� ����� ��...
            {
                // ���� �� ����� ��������� ���� ��������� ��� ��������
                continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
            }

            if (bestEnemyAIAction == null) // � ������ ����� ������ �������� ���������� �� �� �����������, ������� ��������� 1-� �������� � ������ ������ �� ���������
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction(); // ������� ������ �������� ���������� �� ��� ������� �������� ��������
                bestBaseAction = baseAction; //������(������������) ������� �������� �������� ������
            }
            else // ���� ��� ����������� "������ �������� ���������� ��" �� ������� � ������� � ������� ����� �����
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction(); // ����������� ������ �������� ���������� ��
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) // ���� ����������� �������� �� ������� � �� �������� �������� ������ ����������� �������� �� ��������� ��� ������
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }
        //��������� ���������� ����� ��������
        if (bestEnemyAIAction != null && enemyUnit.GetActionPointsSystem().TrySpendActionPointsToTakeAction(bestBaseAction)) // ��������� ��������� ���� ��������, ����� ��������� �������� 
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete); //� ���������� �������� ������� ����� "��������� �������� (�����������)" � ��������� � ������� ������� SetStateTakingTurn  ������� ������ ��� ����� � �������� ��������� TakingTurn
            _unitActionSystem.SetSelectedAction(bestBaseAction); // ������� ��� �������� ����������
            return true; // ���� ����� ��� �� ������� �� ������ ������
        }
        else
        {
            return false; // ���������� ���������� � ������� ���� 
        }        
    }
}
