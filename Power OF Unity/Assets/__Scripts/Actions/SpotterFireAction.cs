using System;
using System.Collections.Generic;
using UnityEngine;


public class SpotterFireAction : BaseAction // �������� �������������� ���� ��������� ����� BaseAction // ������� � ��������� ����� // ����� �� ������ �����
{

    public event EventHandler OnSpotterFireActionStarted;     // �������� �������������� ����  �������� // � ������� ����� ���������� ����� �������� ����� (� HandleAnimationEvents ����� ��������� ��������)
    public event EventHandler OnSpotterFireActionCompleted;   // �������� �������������� ����  ����������� (��������� ���� � ������� ������)



    private enum State
    {
        SpotterFireBefore, //�� 
        SpotterFireAfter,  //����� 
    }

       
    private List<Transform> _spotterFireFXList; // ������ ��������� ������ ����������
    private State _state; // ��������� �����    
    private Unit _partnerUnit;// ���� � ������� ������������ �����    

    private int _maxSpotterFireDistance = 1; //������������ ��������� ������ ����� ��� ������������� ����//����� ���������//

    protected override void Start()
    {
        base.Start();

        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // ��������� ���� �������
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        if (_partnerUnit != null) // ���� ���� �������
        {
            Unit selectedUnit = _unitActionSystem.GetSelectedUnit();
            if (selectedUnit != _partnerUnit) // ���� ���������� ���� �� ������� ��
            {
                _partnerUnit.GetAction<ShootAction>().�learSpotterFireUnit(); // �������� � �������� ���� ��������������� ����
                foreach (Transform spotterFireFX in _spotterFireFXList) // ������ �����
                {
                    Destroy(spotterFireFX.gameObject); // ��������� �����
                }
                _partnerUnit = null; // ������� ��������

                OnSpotterFireActionCompleted?.Invoke(this, EventArgs.Empty); // �������� ������� �������� ������������� ����������� ��������� UnitAnimator (�������� ������)
            }
        }
    }



    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        switch (_state) // ������������� ���������� ���� � ����������� �� _state
        {
            case State.SpotterFireBefore:

                NextState();
                break;

            case State.SpotterFireAfter:
                NextState();
                break;
        }
    }

    private void NextState() //������� ������������ ���������
    {
        switch (_state)
        {
            case State.SpotterFireBefore:

                _state = State.SpotterFireAfter;

                break;

            case State.SpotterFireAfter:

                ActionComplete(); // ������� ������� ������� �������� ���������

                break;
        }

        //Debug.Log(_state);
    }

    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "��������";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 40, //�������� ������� �������� ��������. ����� ��������� ������� ���� ������ ������� ������� �� �����, 
        };
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList()// �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
                                                                       // ���������� �������� ������� ��� �������� ������� ����� ������ ��� ����� ���� 
    {
        List<GridPositionXZ> validGridPositionList = new List<GridPositionXZ>();

        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxSpotterFireDistance; x <= _maxSpotterFireDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxComboDistance
        {
            for (int z = -_maxSpotterFireDistance; z <= _maxSpotterFireDistance; z++)
            {
                GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, 0); // ��������� �������� �������. ��� ������� ���������(0,0, 0-����) �������� ��� ���� 
                GridPositionXZ testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                if (!_levelGrid.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                if (!_levelGrid.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ (��� ����� ������ � ������� �� ����� �� ��������)
                {
                    // ������� ����� �����, ��� ������
                    continue;
                }

                Unit targetUnit = _levelGrid.GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� 
                                                                                                // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                if (targetUnit.IsEnemy() != _unit.IsEnemy()) // ���� ����������� ���� ���� � ��� ���� ��� (���������� �������)
                {
                    // ��� ������������� � ������ "��������"
                    continue;
                }

                // �������� �������� �� ������������� �����
                int actionPoint = targetUnit.GetActionPointsSystem().GetActionPointsCount(); // �������� ���� �������� � ������������ �����                
                if (actionPoint == 0)
                {
                    // �� ������ ��� �� �������
                    continue;
                }
                if (targetUnit == _unit)
                {
                    // � ����� ����� ������ ������
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
        _partnerUnit = _levelGrid.GetUnitAtGridPosition(gridPosition); // ������� ����� � �������� ����� �������������� �����
        _state = State.SpotterFireBefore; // ���������� ��������� ���������� 

        // ������ ����� ������������ � ������
        Transform unitAimPoinTransform = _unit.GetAction<ShootAction>().GetAimPoinTransform();
        Transform partnerAimPoinTransform = _partnerUnit.GetAction<ShootAction>().GetAimPoinTransform();
        _spotterFireFXList = new List<Transform> // �������� ����� � ������� � ������
        {
            Instantiate(GameAssets.Instance.spotterFireFXPrefab, unitAimPoinTransform.position, Quaternion.identity, unitAimPoinTransform),
            Instantiate(GameAssets.Instance.spotterFireFXPrefab, partnerAimPoinTransform.position, Quaternion.identity ,partnerAimPoinTransform)
        };

        _soundManager.PlayOneShot(SoundName.Spotter);

        _partnerUnit.GetAction<ShootAction>().SetSpotterFireUnit(_unit); // ��������� ��������, ����, ��� �������. ����
        _unitActionSystem.SetSelectedUnit(_partnerUnit, _partnerUnit.GetAction<ShootAction>()); //������� �������� ���������� � ������� �������� ��������


        OnSpotterFireActionStarted?.Invoke(this, EventArgs.Empty); // �������� ������� �������� ������������� �������� ��������� UnitAnimator (��������� ������)
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    public override int GetActionPointCost() // ������������� ������� ������� // �������� ������ ����� �� �������� (��������� ��������)
    {
        return 2;
    }
    public override int GetMaxActionDistance()
    {
        return _maxSpotterFireDistance;
    }     

    private void OnDestroy()
    {
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged; // ��������� ���� �������
    }

    public override string GetToolTip()
    {
        return "����� �������� ����� 2  �����" + "\n" +
            "���� - " + GetActionPointCost() + "  ����������� � ������� �����" + "\n" +
            "��������� - " + GetMaxActionDistance() + "\n" +
        "� ��������� ������������� ���� � ������ �������� �� 50%," + "\n" +
        " � �������� ����������� 100%)\r\n";
    }
}
