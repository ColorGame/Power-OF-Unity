using Pathfinding;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ComboAction : BaseAction // ����� // �������� ����� ��������� ������ ��� �������� ����� ������������
{
    // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.                                                                                
    public static event EventHandler<OnComboEventArgs> OnAnyUnitComboStateChanged; // � ������ ����� ���������� ��������� �����         
    public class OnComboEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� ������ ������
    {
        public Unit partnerUnit; // ���� ������� �� ������� ���� �������� ���������
        public State state; // ���������
    }

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
    private RopeRanderer _ropeRandererParten;

    private int _searchEnemyPointCost = 2; // ��������� ������ �����
    private int _maxComboPartnerDistance = 1; //������������ ��������� ����� ��� ������ �������� � �������������� �����//����� ���������//
    private int _maxComboEnemyDistance = 5; //������������ ��������� ����� ��� ������ �����//����� ���������//
    private float zOffset = 0; // 

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _state = State.ComboSearchPartner; // ��������� ��������� �� ��������� �.�. ���������� � ������ GetValidActionGridPositionList
        _ropeRandererUnit = _unit.GetUnitRope().GetRopeRanderer();       

        ComboAction.OnAnyUnitComboStateChanged += ComboAction_OnAnyUnitComboStateChanged;      
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
                Vector3 unitPartnerDirection = (_unitPartner.transform.position - transform.position).normalized; // ����������� � �������� �����, ��������� ������
                transform.forward = Vector3.Slerp(transform.forward, unitPartnerDirection, Time.deltaTime * rotateSpeed); // ������ �����.
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

                _instantiateFXPrefab = Instantiate(GameAssets.Instance.comboPartnerFXPrefab, transform.position + Vector3.up * 1.7f, Quaternion.identity); // �������� �������� ��������������
                _instantiateFXPrefab.LookAt(_unitPartner.transform.position + Vector3.up * 1.7f); // � �������� � ������� ��������

                _state = State.ComboSearchEnemy;
                OnAnyUnitComboStateChanged?.Invoke(this, new OnComboEventArgs // � ����� �������� ���� ������� ���������, ��� �� �� ���� ��������� ��������� ���� �������� (��� GetActionPointCost() ������� �� ���������)
                {
                    partnerUnit = _unitPartner,
                    state = _state
                });
                ActionComplete(); // �������� �������� ������ �������� � ������� ����  (�������� ������� ClearBusy ���������� �� ������ UnitActionSystem, � � UnitActionSystem_OnBusyChanged �� ������ UnitActionSystemUI ������� ��� ��������, ���-�� ������ ���������� ������������ �� ������ ����� ��������)
                break;

            case State.ComboSearchEnemy:
                _state = State.ComboStart;
                ActionComplete();
                break;
            case State.ComboStart:
                _state = State.ComboAfter;
                OnAnyUnitComboStateChanged?.Invoke(this, new OnComboEventArgs // � ����� �������� ���� ������� ���������, ��� �� �� ���� ����� �� ����� �����
                {
                    partnerUnit = _unitPartner,
                    state = _state
                });
                float ComboAfterStateTime = 0.5f;
                _stateTimer = ComboAfterStateTime;
                break;

            case State.ComboAfter: // � ���� ��������� ������ UI ����������
                Destroy(_instantiateFXPrefab.GameObject());
                _unitPartner.GetUnitRope().HideRope();
                _unit.GetUnitRope().HideRope();

                _state = State.ComboSearchPartner;
                OnAnyUnitComboStateChanged?.Invoke(this, new OnComboEventArgs // � ����� �������� ���� ������� ���������, ��� �� �� ���� ����� �� ����� �����
                {
                    partnerUnit = _unitPartner,
                    state = _state
                });
                ActionComplete(); // ������� ������� ������� �������� ���������
                break;
        }
    }

    private void HookShootin() // �������� ������
    {
        float rotateSpeed = 10f;
        Vector3 partnerEnemyDirection = (_unitEnemy.transform.position - _unitPartner.transform.position).normalized; // ����������� � �������� �����, ��������� ������
        Vector3 unitEnemyDirection = (_unitEnemy.transform.position - transform.position).normalized; // ����������� � �������� �����, ��������� ������

        // ��������� �������� � ������ �����
        _unitPartner.transform.forward = Vector3.Slerp(_unitPartner.transform.forward, partnerEnemyDirection, Time.deltaTime * rotateSpeed); // ������ �����.
        transform.forward = Vector3.Slerp(transform.forward, unitEnemyDirection, Time.deltaTime * rotateSpeed); // ������ �����.

        // ����� ���������� 
        if (Vector3.Dot(unitEnemyDirection, transform.forward) >= 0.95f) // ����� ���������� 1, ���� ��� ��������� � ����� � ��� �� �����������, -1, ���� ��� ��������� � ���������� ��������������� ������������, � ����, ���� ������� ���������������.
        {
            //�������� ��������
            Vector3 enemuAimPoint = _unitEnemy.GetAction<ShootAction>().GetAimPoinTransform().position; // ����� ������������ ������
            // ��������� ������� � ������� ����� (����� �������� � ��������� Z)
            _ropeRandererParten.transform.LookAt(enemuAimPoint);
            _ropeRandererUnit.transform.LookAt(enemuAimPoint);

            float speedShootRope = 10;
            zOffset += Time.deltaTime * speedShootRope;

            if (zOffset <= Vector3.Distance(_ropeRandererParten.transform.position, _unitEnemy.transform.position))  // ���� ������� ��� �� �������� �� ����� ��
            {
                _ropeRandererParten.RopeDraw(Vector3.forward * zOffset);// ������ �������       
            }
            if (zOffset <= Vector3.Distance(_ropeRandererUnit.transform.position, _unitEnemy.transform.position))  // ���� ������� ��� �� �������� �� ����� ��
            {
                _ropeRandererUnit.RopeDraw(Vector3.forward * zOffset); // ������ �������               
            }

            if (zOffset >= Vector3.Distance(_unitPartner.transform.position, _unitEnemy.transform.position) &&
                zOffset >= Vector3.Distance(transform.position, _unitEnemy.transform.position)) // ������� �������� �� �����
            {
                SoundManager.Instance.PlaySoundOneShot(SoundManager.Sound.HookShoot);
                NextState(); //��������� ���������
            }
        }
    }

    private void PullEnemy() // ����� �����
    {
        Vector3 targetPointEnemyWorldPosition = LevelGrid.Instance.GetWorldPosition(_targetPointEnemyGridPosition); // ������� ������� ���� ���� ����������� �����                

        Vector3 moveEnemyDirection = (targetPointEnemyWorldPosition - _unitEnemy.transform.position).normalized; // ����������� ��������, ��������� ������

        float moveEnemySpead = 6f; //����� ���������//
        _unitEnemy.transform.position += moveEnemyDirection * moveEnemySpead * Time.deltaTime;

        // ��������� �������� � ����� � ������� �����
        _unitPartner.transform.LookAt(_unitEnemy.transform);
        transform.LookAt(_unitEnemy.transform);

        // ��������� ��������� �� �������� �� ����� � �� ����� �� �����
        float zDistancePartner = Vector3.Distance(_unitPartner.transform.position, _unitEnemy.transform.position);
        _ropeRandererParten.RopeDraw(Vector3.forward * zDistancePartner);// ������ ������� 
        float zDistanceUnit = Vector3.Distance(transform.position, _unitEnemy.transform.position);
        _ropeRandererUnit.RopeDraw(Vector3.forward * zDistanceUnit); // ������ ������� 

        float stoppingDistance = 0.2f; // ��������� ��������� //����� ���������//
        if (Vector3.Distance(_unitEnemy.transform.position, targetPointEnemyWorldPosition) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
        {
            float stunPercent = 0.3f; // ������� ���������
            _unitEnemy.Stun(stunPercent); //����� ���������// �������
            SoundManager.Instance.PlaySoundOneShot(SoundManager.Sound.HookPull);
            NextState(); //��������� ���������
        }
    }   

    private void ComboAction_OnAnyUnitComboStateChanged(object sender, OnComboEventArgs e)
    {
        if (e.partnerUnit == _unit) // ���� ������� ��� ����� - ��� � ��
        {
            SetState(e.state); // �������� ��� ���������
        };
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

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                switch (_state)
                {
                    default:
                    case State.ComboSearchPartner:

                        if (_unit.GetActionPoints() < _searchEnemyPointCost) // ���� � ����� �� ������� ����� ��� ����������� �������� �� (�.�. ����� ��������� ������ �� �����, ���� ��� ����� �� � ����� ������)
                        {
                            return validGridPositionList; // ������ ������ ������
                        }

                        if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ 
                        {
                            // ������� ����� �����, ��� ������
                            continue;
                        }

                        // ���� ���� �������� �� �������� ������
                        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� �������  // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����                        
                        if (targetUnit.IsEnemy() != _unit.IsEnemy()) // ���� ����������� �� � ���� ������� (���������� ���)
                        {
                            continue;
                        }

                        // �������� �������� �� ������������� �����
                        int actionPoint = targetUnit.GetActionPoints(); // �������� ���� �������� � ������������ �����                
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

                        if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ 
                        {
                            // ������� ����� �����, ��� ������
                            continue;
                        }

                        // ���� ���� ����� �� �������� ������������� ������
                        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                        if (targetUnit.IsEnemy() == _unit.IsEnemy()) // ���� ����������� � ����� ������� (���������� ���)
                        {
                            continue;
                        }

                        // �������� �� ����������������� ������ �� ���� 
                        Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition); // ��������� � ������� ���������� ���������� ��� �������� ������� �����  
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

                        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� � �������. ����� ���������� ������������ ����� �� ������
                        {
                            // ��� ���� - ���� ����������
                            continue;
                        }

                        /*//���� � ���� ������� ��� ���� ���� GraphNode  ������ ��� GridPositionXZ ���������� ��� ������ (�����, ������) ��� ����� � �������)              
                        if (!LevelGrid.Instance.WalkableNode(testGridPosition))
                        {
                            continue; // ��������� ��� �������
                        }*/


                        // �������� �� ����������������� ������ �� ����
                        unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition); // ��������� � ������� ���������� ���������� ��� �������� ������� �����
                        Vector3 testWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);// 
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

                        //�������� �������� ������� ��� ������ ������ (���� ����������� ����� �������) // �������� � ������ � �������  if (LevelGrid.Instance.GetGridNode(testGridPosition) == null) 

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
                _unitPartner = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition); // ������� ����� ��� �����  
                _ropeRandererParten = _unitPartner.GetUnitRope().GetRopeRanderer(); // ������� � �������� ��������� �������
                float ComboSearchPartnerStateTime = 0.5f; //����� ��������.  ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� �������� ..//����� ���������//
                _stateTimer = ComboSearchPartnerStateTime;
                break;

            case State.ComboSearchEnemy:  // ���� ���� ����� ��                 
                _unitPartner.SpendActionPoints(GetActionPointCost()); // ������ � �������� ���� �������� (� ���� ��� ������� � HandleSelectedAction() � ������ UnitActionSystem)
                _unitEnemy = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition); // ��������� �����                
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

    private int GetMaxComboEnemyDistance()
    {
        return _maxComboEnemyDistance;
    }

    private int GetSearchEnemyPointCost()
    {
        return _searchEnemyPointCost;
    }

    public override string GetToolTip()
    {
        return "����� �������� ����� 2  �����" + "\n" +
            "���� - " + GetSearchEnemyPointCost() + "  ����������� ����� � �����" + "\n" +
            "���� - 0" + "\n" +
            "2 ����� � ���������� " + GetMaxComboEnemyDistance() + " ������, ����� ����� � ����" + "\n" +
           "�������������� ������ � ��� � �����";
    }
}