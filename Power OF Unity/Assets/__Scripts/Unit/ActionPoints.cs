using System;
using UnityEngine;

/// <summary>
/// ���� �������� �����
/// </summary>
public class ActionPoints
{
    /// <summary>
    /// ���� �������� �����
    /// </summary>
    public ActionPoints(Unit unit) 
    {
        _unit = unit;
    }

    public event EventHandler OnActionPointsChanged;  // ��������� ����� ��������.         


    private int _actionPointsCount = Constant.ACTION_POINTS_MAX; // ���� ��������
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
        if (_actionPointsCount >= baseAction.GetActionPointCost()) // ���� ����� �������� ������� ��...
        {
            return true; // ����� ��������� ��������
        }
        else
        {
            return false; // ��� ����� �� �������
        }

        /*// �������������� ������ ���� ����
        return _actionPointsCount >= baseAction.GetActionPointCost();*/
    }

    public void SpendActionPoints(int amount) //��������� ���� �������� (amount- ���������� ������� ���� ���������)
    {
        _actionPointsCount -= amount;

        OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� ActionButtonSystemUI)
    }

    public int GetActionPointsCount() // �������� ���� ��������
    {
        return _actionPointsCount;
    }


    public void TurnSystem_OnTurnChanged(object sender, EventArgs empty) //��� ������� ������� ���� �������� �� ������������
    {
        if ((_unit.IsEnemy() && !_turnSystem.IsPlayerTurn()) || // ���� ��� ���� � ��� ������� (�� ������� ������) ��� ��� �� ����(�����) � ������� ������ ��...
            (!_unit.IsEnemy() && _turnSystem.IsPlayerTurn()))
        {
            _actionPointsCount = Constant.ACTION_POINTS_MAX;

            if (_penaltyStunPercent != 0)
            {
                _actionPointsCount -= Mathf.RoundToInt(_actionPointsCount * _penaltyStunPercent); // �������� �����
                _penaltyStunPercent = 0;
                SetStunned(false); // �������� ���������
            }

            //  ����� ������� ���������������� �� ���� ���
            /*int passedTurnNumber = _turnSystem.GetTurnNumber() - _startStunTurnNumber;// ������ ����� �� ������ ���������
            if (passedTurnNumber <= _durationStunEffectTurnNumber) // ���� ����� ������ ������ ��� ����� ������������ ��������� (������ ��������� ��� ���������)
            {
                _actionPointsCount -= Mathf.RoundToInt(_actionPointsCount * _penaltyStunPercent); // �������� �����
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

    public void Stun(float stunPercent) // �������� �� stunPercent(������� ���������)
    {
        SetStunned(true);
        _penaltyStunPercent = stunPercent; // ��������� �������� ���������

        /*// ����� ������� ���������������� �� ���� ���
        _startStunTurnNumber = _turnSystem.GetTurnNumber(); // ������� ��������� ����� ����              
        if (_actionPointsCount > 0) // ���� ����� ���� ������ ����
        {
            _durationStunEffectTurnNumber = 1; //����� ���������// ��������� ����� ������� ���� ��������� ���
        }
        if(_actionPointsCount<=0) // ���� ����� ���� ����
        {
            _durationStunEffectTurnNumber = 3; //����� ���������// ��������� ����� ������� ��������� 3 ���� (����� ��� �����)
        }*/
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.
    }

    public bool GetStunned() { return _stunned; }
    private void SetStunned(bool stunned) { _stunned = stunned; }
}
