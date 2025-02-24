using System;
using UnityEngine;

/// <summary>
/// ���� �������� �����
/// </summary>
public class UnitActionPoints
{
    /// <summary>
    /// ���� �������� �����
    /// </summary>
    public UnitActionPoints(Unit unit, int actionPoints)
    {
        _unit = unit;
        _actionPoints = actionPoints;
        _actionPointsFull = _actionPoints;
    }

    public event EventHandler OnActionPointsChanged;  // ��������� ����� ��������.         


    private int _actionPoints; // ���� ��������
    private int _actionPointsFull; //������ ���������� ����� ��������
    private float _penaltyStunPercent;  // �������� ������� ��������� (����� ��������� � ���� ���)
    private bool _stunned = false; // ����������(�� ��������� ����)
    private Unit _unit;

    /*private int _startStunTurnNumber; //����� ������� (����) ��� ������ ������� ���������
   private int _durationStunEffectTurnNumber; // ����������������� ����������� ������� ���������� �����*/

    private TurnSystem _turnSystem;

    public void SetupForSpawn()
    {
        _turnSystem = _unit.GetTurnSystem();
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // ������. �� ������� ��� �������    
    }

    /// <summary>
    ///  ��������� ����� ��� ������ ��� ��������.
    /// </summary>
    public void SetupOnDestroyAndQuit()
    {
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
    /// <summary>
    /// ��������� ��������� ���� ��������, ����� ��������� ��������
    /// </summary>
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) // ���� ����� ��������� ������ ��� ������ ������
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

    /// <summary>
    /// �� ����� ��������� ���� ��������, ����� ��������� �������� ? 
    /// </summary>
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
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
    /// <summary>
    /// ��������� ���� �������� (amount- ���������� ������� ���� ���������)
    /// </summary>
    public void SpendActionPoints(int amount)
    {
        _actionPoints -= amount;

        OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� ActionButtonSystemUI)
    }

    /// <summary>
    /// ��� ������� ������� ���� �������� �� �������
    /// </summary>
    public void TurnSystem_OnTurnChanged(object sender, EventArgs empty)
    {
        if ((_unit.GetIsEnemy() && !_turnSystem.IsPlayerTurn()) || // ���� ��� ���� � ��� ������� (�� ������� ������) ��� ��� �� ����(�����) � ������� ������ ��...
            (!_unit.GetIsEnemy() && _turnSystem.IsPlayerTurn()))
        {
            _actionPoints = _actionPointsFull;

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

            OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� ActionButtonSystemUI)
        }
    }
    /// <summary>
    /// �������� �� stunPercent(������� ���������)
    /// </summary>
    public void Stun(float stunPercent)
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
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.
    }
    /// <summary>
    /// �������� ���������� ����� ��������
    /// </summary>
    public int GetActionPointsCount() { return _actionPoints; }
    public int GetActionPointsCountFull() { return _actionPointsFull; }

    public bool GetStunned() { return _stunned; }
    private void SetStunned(bool stunned) { _stunned = stunned; }
}
