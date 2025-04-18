using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction // ������� �������� ���
{
    public static event EventHandler<OnSwordEventArgs> OnAnySwordHit;   // ������� - ����� ����� ���� ����� (����� ����� ���� ������ ��������� ����� �� �������� ������� Event) // <Unit> ������� �������� �������� ����� ��� ����
                                                                        // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
                                                                        // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����,
                                                                        // ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 

    public event EventHandler OnSwordActionStarted;     // �������� ��� ��������
    public event EventHandler OnSwordActionCompleted;   // �������� ��� �����������
    public class OnSwordEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� ������ ������
    {
        public Unit targetUnit; // ������� ���� � ���� �������
        public Unit hittingUnit; // ���� ������� �������� ���� �����
    }
    private enum State
    {
        SwingingSwordBeforeHit, //����� ����� ����� ������
        SwingingSwordAfterHit,  //����� ���� ����� �����
    }


    private State _state; // ��������� �����
    private float _stateTimer; //������ ���������
    private Unit _targetUnit;// ���� � �������� �������� �������

    private int _swordDamage = 50; // ���� �� ����
    private int _maxSwordDistance = 1; //������������ ��������� ��������� ����� //����� ���������//



    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        _stateTimer -= Time.deltaTime; // �������� ������ ��� ������������ ���������

        switch (_state) // ������������� ���������� ���� � ����������� �� _state
        {
            case State.SwingingSwordBeforeHit:

                Vector3 aimDirection = (_targetUnit.GetWorldPosition() - transform.position).normalized; // ����������� ������������, ��������� ������
                float rotateSpeed = 10f; //����� ���������//

                transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed); // ������ �����.

                break;

            case State.SwingingSwordAfterHit:
                break;
        }

        if (_stateTimer <= 0) // �� ��������� ������� ������� NextMusic() ������� � ���� ������� ���������� ���������. �������� - � ���� ���� TypeGrenade.Aiming: ����� � case TypeGrenade.Aiming: ��������� �� TypeGrenade.Shooting;
        {
            NextState(); //��������� ���������
        }
    }

    private void NextState() //������� ������������ ���������
    {
        switch (_state)
        {
            case State.SwingingSwordBeforeHit:
                _state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 1f; // ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� ���� ����� ����� //����� ���������//
                _stateTimer = afterHitStateTime;
                SwordHit();
                break;

            case State.SwingingSwordAfterHit:

                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);  // �������� ������� �������� ��� �����������

                ActionComplete(); // ������� ������� ������� �������� ���������
                break;
        }

        //Debug.Log(_state);
    }

    private void SwordHit() // ���� �����
    {
        OnAnySwordHit?.Invoke(this, new OnSwordEventArgs // ������� ����� ��������� ������ OnShootEventArgs
        {
            targetUnit = _targetUnit,
            hittingUnit = _unit
        }); // �������� ������� ����� ����� ���� ����� � � �������� ��������� � ���� ������� � ��� �������� ���� (���������� ScreenShakeActions ��� ���������� ������ ������ � UnitRagdollSpawner- ��� ����������� ����������� ���������)

        _unit.GetSoundManager().PlayOneShot(SoundName.Sword);

        if (_targetUnit.GetActionPointsSystem().GetStunned() && !_unit.GetActionPointsSystem().GetStunned()) // ���� ���� ���������� � � ���
        {
            int maxDamage = Mathf.RoundToInt(_targetUnit.GetHealthSystem().GetHealthFull() * 0.8f); // ������� 90% ��� ���� ��������
            _targetUnit.GetHealthSystem().Damage(maxDamage);
        }
        else
        {
            _targetUnit.GetHealthSystem().Damage(_swordDamage); // ������� �������� ����� ������� ����     //����� ���������// � ���������� ����� ����� ���� ���������� �� ������
        }
    }


    public override string GetActionName()
    {
        return "���";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 150, // �������� ��� �������� ������������ //����� ���������//
        };
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList()// �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������                                                                       
    {
        List<GridPositionXZ> validGridPositionList = new List<GridPositionXZ>();

        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxSwordDistance; x <= _maxSwordDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxComboDistance
        {
            for (int z = -_maxSwordDistance; z <= _maxSwordDistance; z++)
            {
                GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, 0); // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPositionXZ testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                if (!_levelGrid.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                if (!_levelGrid.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ (��� ����� ������ � ������� �� ����� �� ��� �������)
                {
                    // ������� ����� �����, ��� ������
                    continue;
                }

                Unit targetUnit = _levelGrid.GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� 
                                                                                                // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                if (targetUnit.GetType() == _unit.GetType()) // ���� ����������� ���� ���� � ��� ���� ���� ���� �� (���� ��� ��� � ����� ������� �� ����� ������������ ���� ������)
                {
                    // ��� ������������� � ����� "�������"
                    continue;
                }

                validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete)  // ������������� TakeAction (��������� �������� (�����������). (������� onActionComplete - �� ���������� ��������). � ����� ������ �������� �������� ������� ClearBusy - �������� ���������
    {
        _targetUnit = _levelGrid.GetUnitAtGridPosition(gridPosition); // ������� ����� � �������� ������� � �������� ���

        _state = State.SwingingSwordBeforeHit; // ���������� ��������� ������������  ����� ����� ����� ������
        float beforeHitStateTime = 0.7f; //�� �����.  ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� ����� ����� ������ ..//����� ���������//
        _stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty); // �������� ������� �������� ��� �������� ��������� UnitAnimator

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������

    }
    public override int GetActionPointCost() // ������������� ������� ������� // �������� ������ ����� �� �������� (��������� ��������)
    {
        return 2;
    }

    public override int GetMaxActionDistance()
    {
        return _maxSwordDistance;
    }

    public Unit GetTargetUnit() // �������� _unitPartner
    {
        return _targetUnit;
    }
    private int GetSwordDamage()
    {
        return _swordDamage;
    }


    public override string GetToolTip()
    {
        return "���� - " + GetActionPointCost() + "\n" +
                "��������� - " + GetMaxActionDistance() + "\n" +
                "���� - " + GetSwordDamage() + "\n" +
                "������ �����" + "\n" +
                "� ����� � ������� ��������� ���������� 80%  ��� ���� ��������";
    }

}
