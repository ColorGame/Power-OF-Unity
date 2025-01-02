using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction // �������� ������� ��������� ����� BaseAction // ������� � ��������� ����� // ����� �� ������ �����
{    
    public event EventHandler<Unit> OnHealActionStarted;     // �������� ������� �������� (����� ������� ������ � ��������� ����) // � ������� ����� ���������� ����� �������� ����� (� HandleAnimationEvents ����� ��������� ��������)
    public event EventHandler<Unit> OnHealActionCompleted;   // �������� ������� ����������� (��������� ���� � ������� ������)



    private enum State
    {
        HealBefore, //�� ������� (�����������)
        HealAfter,  //����� �������
    }

    private int _healAmount = 50;
    private State _state; // ��������� �����
    private float _stateTimer; //������ ���������
    private Unit _targetUnit;// ���� �������� �����

    private int _maxHealDistance = 1; //������������ ��������� �������//����� ���������//


    protected override void Start()
    {
        HandleAnimationEvents handleAnimationEvents = _unit.GetHandleAnimationEvents();
        handleAnimationEvents.OnPrayingStendUpEventStarted += HandleAnimationEvents_OnPrayingStendUpEventStarted;
    }

   

    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        _stateTimer -= Time.deltaTime; // �������� ������ ��� ������������ ���������

        switch (_state) // ������������� ���������� ���� � ����������� �� _state
        {
            case State.HealBefore:

                if (_targetUnit != _unit) // ���� ����� ������� ����� �� ����������� � ��� �������
                {
                    Vector3 targetDirection = (_targetUnit.GetWorldPosition() - transform.position).normalized; // ����������� � �������� �����, ��������� ������
                    float rotateSpeed = 10f; //����� ���������//

                    transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotateSpeed); // ������ �����.
                }

                break;

            case State.HealAfter:
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
            case State.HealBefore:
                _state = State.HealAfter;
                float afterHealStateTime = 3f; // ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� ������� //����� ���������// (������ ��������� � ������������� ��������, ����� ������ ������������ � ������������ ������)
                _stateTimer = afterHealStateTime;

                Heal(); // �����

                break;

            case State.HealAfter:

                OnHealActionCompleted?.Invoke(this, _targetUnit);  // �������� ������� �������� ������� ����������� (��������� UnitAnimator, ��� ����� �������� ������)

                ActionComplete(); // ������� ������� ������� �������� ���������
                break;
        }

        //Debug.Log(_state);
    }
    private void HandleAnimationEvents_OnPrayingStendUpEventStarted(object sender, EventArgs e)
    {
        Instantiate(GameAssetsSO.Instance.healFXPrefab, _targetUnit.GetWorldPosition(), Quaternion.LookRotation(Vector3.up)); // �������� ������ ������ ��� ����� �������� �������� (�� ������ � ���������� �������� � ������ Stop Action - Destroy)
    }

    private void Heal() // �������
    {
       _unit.GetSoundManager().PlayOneShot(SoundName.Heal);
        _targetUnit.GetHealthSystem().Healing(_healAmount);
    }

    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "�������";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        float HealthNormalized = _unit.GetHealthSystem().GetHealthNormalized(); // ������� ��������������� �������� �����

        if (HealthNormalized <= 0.3) //���� �������� ������ ��� ����� 30% ��
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 150, //�������� ������� �������� �������� ����� �������. 
            };
        }
        else
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 50, //�������� ������� �������� ��������. ����� ��������� ������� ���� ������ ������� ������� �� �����, 
            };
        }

    }

    public override List<GridPositionXZ> GetValidActionGridPositionList()// �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
                                                                       // ���������� �������� ������� ��� �������� ������� ����� ������ ��� ����� ���� 
    {
        List<GridPositionXZ> validGridPositionList = new List<GridPositionXZ>();

        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxHealDistance; x <= _maxHealDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxComboDistance
        {
            for (int z = -_maxHealDistance; z <= _maxHealDistance; z++)
            {
                GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, 0); // ��������� �������� �������. ��� ������� ���������(0,0, 0-����) �������� ��� ���� 
                GridPositionXZ testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                if (!_unit.GetLevelGrid().IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                if (!_unit.GetLevelGrid().HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ (��� ����� ������ � ������� �� ����� �� ��������)
                {
                    // ������� ����� �����, ��� ������
                    continue;
                }

                Unit targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� 
                                                                                                // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                if (targetUnit.IsEnemy() != _unit.IsEnemy()) // ���� ����������� ���� ���� � ��� ���� ��� (���������� �������)
                {
                    // ��� ������������� � ������ "��������"
                    continue;
                }



                validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // (onActionComplete - �� ���������� ��������). � �������� ����� ���������� ������� Action 
                                                                                        // � ������ ������ �������� �������� ������� �� �� ���������� - GridPositionXZ _gridPositioAnchor - �� �������� ���� ��� ���� ����� ��������������� ��������� ������� ������� TakeAction.
                                                                                        // ���� ������ ������, ������� ���������� - public class BaseParameters{} 
                                                                                        // � ����������� � ������� ����� �������������� ��� ������� �������� -
                                                                                        // public SpinBaseParameters : BaseParameters{}
                                                                                        // ����� ������� - public override void TakeAction(BaseParameters baseParameters ,Action onActionComplete){
                                                                                        // SpinBaseParameters spinBaseParameters = (SpinBaseParameters)baseParameters;}
    {
        _targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(gridPosition); // ������� ����� �������� ����� �������� (��� ����� ���� � ��� ����)

        _state = State.HealBefore; // ���������� ��������� ���������� �� �������
        float beforeHealStateTime = 0.5f; //�� �������.  ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ���������� ����� �������� ..//����� ���������//
        _stateTimer = beforeHealStateTime;

        OnHealActionStarted?.Invoke(this, _targetUnit); // �������� ������� �������� ������� �������� � ��������� ���� ����� ��������� UnitAnimator (��������� ������)
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    public override int GetActionPointCost() // ������������� ������� ������� // �������� ������ ����� �� �������� (��������� ��������)
    {
        return 2;
    }

    public override int GetMaxActionDistance()
    {
        return _maxHealDistance;
    }
       
    private int GetHealAmount()
    {
        return _healAmount;
    }

    public override string GetToolTip()
    {
        return "���� - " + GetActionPointCost() + "\n" +
                "��������� - " + GetMaxActionDistance() + "\n" +
                "��������������� " + GetHealAmount() + " ��. �����" + "\n" +
                "������ ����� ����, � ������ ����� ����������";
    }


}
