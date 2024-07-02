using System;
using System.Collections.Generic;
using UnityEngine;


public class GrappleAction : BaseAction // ����� (Grapple Gun) // �������� ����� ��������� ������ ��� �������� ����� ������������
{  

    public static event EventHandler OnAnyUnitStunComboAction; // ����� ���� ������� � ����� ��������

    public event EventHandler<Unit> OnComboActionStarted;     // �������� ����� �������� 
    public event EventHandler<Unit> OnComboActionCompleted;   // �������� ����� �����������    

    public enum State
    {
        ComboSearchPartner, //����� �������� ��� �����
        ComboSearchEnemy,   //����� �����
        ComboStart,         //����� �����
        ComboAfter,         //����� �����
    }

    [SerializeField] private LayerMask _obstaclesDoorMousePlaneCoverLayerMask; //����� ���� ����������� ���� ������� Obstacles � DoorInteract � MousePlane(���) Cover// ����� �� ���� ������ � ���� ���������� ����� ����� -Obstacles, � �� ������ -DoorInteract //��� ����� ������� ������ �������� �������� �� Box collider ����� ����� ����� ����� ������������� ������� ���� 

    private State _state; // ��������� �����
    private float _stateTimer; //������ ���������
    private Unit _unitPartner; // ���� ������� � ������� ����� ������ �����
    private Unit _unitEnemy;  // ���� ����    
    private GridPositionXZ _targetPointEnemyGridPosition; // ����� ����������� �����
    private Transform _instantiateFXPrefab; // ��������� ������ ��������
    
    private RopeRanderer _ropeRandererUnit;
    private RopeRanderer _ropeRandererPartner;

    private int _searchEnemyPointCost = 2; // ��������� ������ �����
    private int _maxComboPartnerDistance = 1; //������������ ��������� ����� ��� ������ �������� � �������������� �����//����� ���������//
    private int _maxComboEnemyDistance = 5; //������������ ��������� ����� ��� ������ �����//����� ���������//
    private float zOffset = 0; // 

    

    public override void SetupForSpawn(Unit unit)
    {
       base.SetupForSpawn(unit);

        _state = State.ComboSearchPartner; // ��������� ��������� �� ��������� �.�. ���������� � ������ GetValidActionGridPositionList
        _ropeRandererUnit = _unit.GetUnitRope().GetRopeRanderer();
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
            case State.ComboSearchPartner:

                // ����������� � ������� ����� � ��� ����� �����
                float rotateSpeed = 10f;
                Vector3 unitPartnerDirection = (_unitPartner.GetTransformPosition() - _unit.GetTransformPosition()).normalized; // ����������� � �������� �����, ��������� ������
                _unit.SetTransformForward(Vector3.Slerp(_unit.GetTransform().forward, unitPartnerDirection, Time.deltaTime * rotateSpeed)); // ������ �����.
                break;

            case State.ComboSearchEnemy:

                HookShootin();

                break;

            case State.ComboStart:

                PullEnemy();

                break;

            case State.ComboAfter:
                break;
        }

        if (_stateTimer <= 0) // �� ��������� ������� ������� NextMusic() ������� � ���� ������� ���������� ���������. 
        {
            NextState(); //��������� ���������
        }

        // Debug.Log(_state);
    }

    private void NextState() //������� ������������ ��������� (����� ������������ ��� ���������)
    {
        switch (_state)
        {
            case State.ComboSearchPartner:

                _instantiateFXPrefab = Instantiate(GameAssets.Instance.comboPartnerFXPrefab, _unit.GetTransformPosition()+ Vector3.up * 1.7f, Quaternion.identity); // �������� �������� ��������������
                _instantiateFXPrefab.LookAt(_unitPartner.GetTransformPosition() + Vector3.up * 1.7f); // � �������� � ������� ��������

                _state = State.ComboSearchEnemy;
                _unitPartner.GetAction<GrappleAction>().SetState(_state); // � ����� �������� ���� ������� ���������

                ActionComplete(); // �������� �������� ������ �������� � ������� ����  (�������� ������� ClearBusy ���������� �� ������ UnitActionSystem, � � UnitActionSystem_OnBusyChanged �� ������ ActionButtonSystemUI ������� ��� ��������, ���-�� ������ ���������� ������������ �� ������ ����� ��������)
                break;

            case State.ComboSearchEnemy:
                _state = State.ComboStart;
                ActionComplete();
                break;
            case State.ComboStart:
                _state = State.ComboAfter;
                _unitPartner.GetAction<GrappleAction>().SetState(_state);// � ����� �������� ���� ������� ���������, ��� �� �� ���� ����� �� ����� �����               
                
                float ComboAfterStateTime = 0.5f;
                _stateTimer = ComboAfterStateTime;
                break;

            case State.ComboAfter: // � ���� ��������� ������ UI ����������
                Destroy(_instantiateFXPrefab.gameObject);
                _unitPartner.GetUnitRope().HideRope();
                _unit.GetUnitRope().HideRope();

                _state = State.ComboSearchPartner;
                _unitPartner.GetAction<GrappleAction>().SetState(_state);// � ����� �������� ���� ������� ���������, ��� �� �� ���� ����� �� ����� �����
                ActionComplete(); // ������� ������� ������� �������� ���������
                break;
        }
    }

    private void HookShootin() // �������� ������
    {
        float rotateSpeed = 10f;
        Vector3 partnerEnemyDirection = (_unitEnemy.GetTransformPosition() - _unitPartner.GetTransformPosition()).normalized; // ����������� � �������� �����, ��������� ������
        Vector3 unitEnemyDirection = (_unitEnemy.GetTransformPosition() - _unit.GetTransformPosition()).normalized; // ����������� � �������� �����, ��������� ������

        // ��������� �������� � ������ �����
        _unitPartner.SetTransformForward( Vector3.Slerp(_unitPartner.GetTransform().forward, partnerEnemyDirection, Time.deltaTime * rotateSpeed)); // ������ �����.
        transform.forward = Vector3.Slerp(transform.forward, unitEnemyDirection, Time.deltaTime * rotateSpeed); // ������ �����.

        // ����� ���������� 
        if (Vector3.Dot(unitEnemyDirection, transform.forward) >= 0.95f) // ����� ���������� 1, ���� ��� ��������� � ����� � ��� �� �����������, -1, ���� ��� ��������� � ���������� ��������������� ������������, � ����, ���� ������� ���������������.
        {
            //�������� ��������
            Vector3 enemuAimPoint = _unitEnemy.GetHeadTransform().position; // ����� ������������ ������
            // ��������� ������� � ������� ����� (����� �������� � ��������� Z)
            _ropeRandererPartner.transform.LookAt(enemuAimPoint);
            _ropeRandererUnit.transform.LookAt(enemuAimPoint);

            float speedShootRope = 10;
            zOffset += Time.deltaTime * speedShootRope;

            if (zOffset <= Vector3.Distance(_ropeRandererPartner.transform.position, _unitEnemy.GetTransformPosition()))  // ���� ������� ��� �� �������� �� ����� ��
            {
                _ropeRandererPartner.RopeDraw(Vector3.forward * zOffset);// ������ �������       
            }
            if (zOffset <= Vector3.Distance(_ropeRandererUnit.transform.position, _unitEnemy.GetTransformPosition()))  // ���� ������� ��� �� �������� �� ����� ��
            {
                _ropeRandererUnit.RopeDraw(Vector3.forward * zOffset); // ������ �������               
            }

            if (zOffset >= Vector3.Distance(_unitPartner.GetTransformPosition(), _unitEnemy.GetTransformPosition()) &&
                zOffset >= Vector3.Distance(_unit.GetTransformPosition(), _unitEnemy.GetTransformPosition())) // ������� �������� �� �����
            {
                _unit.GetSoundManager().PlayOneShot(SoundName.HookShoot);
                NextState(); //��������� ���������
            }
        }
    }

    private void PullEnemy() // ����� �����
    {
        Vector3 targetPointEnemyWorldPosition = _unit.GetLevelGrid().GetWorldPosition(_targetPointEnemyGridPosition); // ������� ������� ���� ���� ����������� �����                

        Vector3 moveEnemyDirection = (targetPointEnemyWorldPosition - _unitEnemy.GetTransformPosition()).normalized; // ����������� ��������, ��������� ������

        float moveEnemySpead = 6f; //����� ���������//
        _unitEnemy.SetTransformPosition( _unitEnemy.GetTransformPosition() + moveEnemyDirection * moveEnemySpead * Time.deltaTime);

        // ��������� �������� � ����� � ������� �����
        _unitPartner.GetTransform().LookAt(_unitEnemy.GetTransform());
        _unit.GetTransform().LookAt(_unitEnemy.GetTransform());

        // ��������� ��������� �� �������� �� ����� � �� ����� �� �����
        float zDistancePartner = Vector3.Distance(_unitPartner.GetTransformPosition(), _unitEnemy.GetTransformPosition());
        _ropeRandererPartner.RopeDraw(Vector3.forward * zDistancePartner);// ������ ������� 
        float zDistanceUnit = Vector3.Distance(_unit.GetTransformPosition(), _unitEnemy.GetTransformPosition());
        _ropeRandererUnit.RopeDraw(Vector3.forward * zDistanceUnit); // ������ ������� 

        float stoppingDistance = 0.2f; // ��������� ��������� //����� ���������//
        if (Vector3.Distance(_unitEnemy.GetTransformPosition(), targetPointEnemyWorldPosition) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
        {
            float stunPercent = 0.3f; // ������� ���������
            _unitEnemy.GetActionPointsSystem().Stun(stunPercent); //����� ���������// �������
            _unit.GetSoundManager().PlayOneShot(SoundName.HookPull);
            NextState(); //��������� ���������
            _unitEnemy.UpdateGridPosition(); // ������� �������� ������� � ����� �������� ����������
        }
    }    

    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "����";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0, //�������� ������� �������� ��������. ����� ��������� ����� ���� ������ ������� ������� �� �����, 
        };
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList()// �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������                                                                     
    {
        List<GridPositionXZ> validGridPositionList = new List<GridPositionXZ>();

        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        int maxComboDistance = GetMaxActionDistance();
        for (int x = -maxComboDistance; x <= maxComboDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� maxComboDistance
        {
            for (int z = -maxComboDistance; z <= maxComboDistance; z++)
            {
                GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, 0); // ��������� �������� �������. ��� ������� ���������(0,0, 0-����) �������� ��� ���� 
                GridPositionXZ testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������
                Unit targetUnit = null;

                if (!_unit.GetLevelGrid().IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                switch (_state)
                {
                    default:
                    case State.ComboSearchPartner:

                        if (_unit.GetActionPointsSystem().GetActionPointsCount() < _searchEnemyPointCost) // ���� � ����� �� ������� ����� ��� ����������� �������� �� (�.�. ����� ��������� ������ �� �����, ���� ��� ����� �� � ����� ������)
                        {
                            return validGridPositionList; // ������ ������ ������
                        }

                        if (!_unit.GetLevelGrid().HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ 
                        {
                            // ������� ����� �����, ��� ������
                            continue;
                        }

                        // ���� ���� �������� �� �������� ������
                        targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� �������  // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����                        
                        if (targetUnit.IsEnemy() != _unit.IsEnemy()) // ���� ����������� �� � ���� ������� (���������� ���)
                        {
                            continue;
                        }

                        // �������� �������� �� ������������� �����
                        int actionPoint = targetUnit.GetActionPointsSystem().GetActionPointsCount(); // �������� ���� �������� � ������������ �����                
                        if (actionPoint < _searchEnemyPointCost)
                        {
                            // ���� � ���� ��������� ����� �������� �� �� ��� �� �������
                            continue;
                        }

                        if (targetUnit == _unit)
                        {
                            // � ����� ����� ����� ������ ������
                            continue;
                        }
                        break;

                    case State.ComboSearchEnemy:

                        // ��� ������� �������� ������ ������� ���� � �� �������
                        int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // ����� ���� ������������� ��������� �������� �������
                        if (testDistance > maxComboDistance) //������� ������ �� ����� � ���� ����� // ���� ���� � (0,0) �� ������ � ������������ (5,4) ��� �� ������� �������� 5+4>7
                        {
                            continue;
                        }

                        if (!_unit.GetLevelGrid().HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ 
                        {
                            // ������� ����� �����, ��� ������
                            continue;
                        }

                        // ���� ���� ����� �� �������� ������������� ������
                        targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                        if (targetUnit.IsEnemy() == _unit.IsEnemy()) // ���� ����������� � ����� ������� (���������� ���)
                        {
                            continue;
                        }

                        // �������� �� ����������������� ������ �� ���� 
                        Vector3 unitWorldPosition = _unit.GetLevelGrid().GetWorldPosition(unitGridPosition); // ��������� � ������� ���������� ���������� ��� �������� ������� �����  
                        Vector3 TestPositionDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized; //��������������� ������ ����������� �������� �����
                        float heightRaycast = 1.7f; // ������ �������� ���� �� ������ ������ (���� �� ����� ������ ���������)
                        if (Physics.Raycast(
                                unitWorldPosition + Vector3.up * heightRaycast,
                                TestPositionDirection,
                                Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                                _obstaclesDoorMousePlaneCoverLayerMask)) // ���� ��� ����� � ����������� �� (Raycast -������ bool ����������)
                        {
                            // �� �������������� ������������
                            continue;
                        }
                        break;

                    case State.ComboStart:

                        if (_unit.GetLevelGrid().HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� � �������. ����� ���������� ������������ ����� �� ������
                        {
                            // ��� ���� - ���� ����������
                            continue;
                        }

                        /*//���� � ���� ������� ��� ���� ���� GraphNode  ������ ��� GridPositionXZ ���������� ��� ������ (�����, ������) ��� ����� � �������)              
                        if (!_unit.GetLevelGrid().WalkableNode(testGridPosition))
                        {
                            continue; // ��������� ��� �������
                        }*/


                        // �������� �� ����������������� ������ �� ����
                        unitWorldPosition = _unit.GetLevelGrid().GetWorldPosition(unitGridPosition); // ��������� � ������� ���������� ���������� ��� �������� ������� �����
                        Vector3 testWorldPosition = _unit.GetLevelGrid().GetWorldPosition(testGridPosition);// 
                        TestPositionDirection = (testWorldPosition - unitWorldPosition).normalized; //��������������� ������ ����������� �������� �����
                        heightRaycast = 0.2f; // ������ �������� ���� ������� ����� ��� �� ������� � ������ �������. (�� ���� �������� ������ �.�. ������ ����� ���� ������ ������������ ������ �������� �������)
                        if (Physics.Raycast(
                                unitWorldPosition + Vector3.up * heightRaycast,
                                TestPositionDirection,
                                Vector3.Distance(unitWorldPosition, testWorldPosition),
                                _obstaclesDoorMousePlaneCoverLayerMask)) // ���� ��� ����� � ����������� �� (Raycast -������ bool ����������)
                        {
                            // �� �������������� ������������
                            continue;
                        }

                        //�������� �������� ������� ��� ������ ������ (���� ����������� ����� �������) // �������� � ������ � �������  if (_unit.GetLevelGrid().GetGridNode(testGridPosition) == null) 

                        /*if (PathfindingMonkey.Instance.GetGridPositionInAirList().Contains(testGridPosition))
                        {
                            continue;
                        }*/
                        break;
                }
                validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����

                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // ���������� ��������  (onActionComplete - �� ���������� ��������). � �������� ����� ���������� ������� Action 
    {
        SetupTakeActionFromState(gridPosition);
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� ��������� ������ � UPDATE// �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    private void SetupTakeActionFromState(GridPositionXZ gridPosition) //��������� ���������� �������� � ����������� �� ���������
    {
        switch (_state)
        {
            default:
            case State.ComboSearchPartner: // ������ ��������
                _unitPartner = _unit.GetLevelGrid().GetUnitAtGridPosition(gridPosition); // ������� ����� ��� �����  
                _ropeRandererPartner = _unitPartner.GetUnitRope().GetRopeRanderer(); // ������� � �������� ��������� �������
                float ComboSearchPartnerStateTime = 0.5f; //����� ��������.  ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� �������� ..//����� ���������//
                _stateTimer = ComboSearchPartnerStateTime;
                break;

            case State.ComboSearchEnemy:  // ���� ���� ����� ��                 
                _unitPartner.GetActionPointsSystem().SpendActionPoints(GetActionPointCost()); // ������ � �������� ���� �������� (� ���� ��� ������� � HandleSelectedAction() � ������ UnitActionSystem)
                _unitEnemy = _unit.GetLevelGrid().GetUnitAtGridPosition(gridPosition); // ��������� �����                
                _unitPartner.GetUnitRope().ShowRope();
                _unit.GetUnitRope().ShowRope();
                // ����� ����� ������� �.�. ��������� ������, �� ����� ���������� ������� ����� � ������ NextStste()
                float ComboSearchEnemyStateTime = 5f;
                _stateTimer = ComboSearchEnemyStateTime;

                // ��������� ������� ��� // ���������� ������� �� ����� �� ����� ����������� � ��������� ������
                break;

            case State.ComboStart:
                _targetPointEnemyGridPosition = gridPosition; // ������� ����� ���� ���� ����������� �����
                // ����� ����� ������� �.�. ��������� ������,  �� ����� ���������� ������� ����� � ������ NextStste()
                float ComboStartStateTime = 5f;
                _stateTimer = ComboStartStateTime;
                break;
        }
    }

    public override int GetActionPointCost() // ������������� ������� ������� // �������� ������ ����� �� �������� (��������� ��������)
    {
        switch (_state)
        {
            default:
            case State.ComboSearchPartner:
            case State.ComboStart: // ���� ��������� ��� ��������� �� �����, �� ����� ���������� ������� �� ����
                return 0; // ����� �������� ��� ����� ������ �� �����              
            case State.ComboSearchEnemy:
                return _searchEnemyPointCost;
        }
    }

    public override int GetMaxActionDistance()
    {
        int maxComboDistance;
        switch (_state)
        {
            default:
            case State.ComboSearchPartner:

                maxComboDistance = _maxComboPartnerDistance;
                break;

            case State.ComboSearchEnemy:
                maxComboDistance = _maxComboEnemyDistance;
                break;

            case State.ComboStart:
                maxComboDistance = _maxComboPartnerDistance;
                break;
        }
        return maxComboDistance;
    }
    public State GetState()
    {
        return _state;
    }
    private State SetState(State state)
    {
        return _state = state;
    }

 





    public override string GetToolTip()
    {
        return "����� �������� ����� 2  �����" + "\n" +
            "���� - " + _searchEnemyPointCost + "  ����������� ����� � �����" + "\n" +
            "���� - 0" + "\n" +
            "2 ����� � ���������� " + _maxComboEnemyDistance + " ������, ����� ����� � ����" + "\n" +
           "�������������� ������ � ��� � �����";
    }
}