#define PATHFINDING

using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Pathfinding.BlockManager;

// �������� �������� �� ��� ������, ���� ��� �������, ������ �� ����� �����, ��� ����� 'Path' ����������, � �� ������ ������ �����������
// ��� ������ ������ ������ �������������� � ������� ����� ��������, ������������ ����� ����



public class MoveAction : BaseAction // �������� ����������� ��������� ����� BaseAction // ������� � ��������� ����� // ����� �� ������ �����
{
    public static event EventHandler <Unit> OnAnyUnitPathComplete; // � ������ ����� �������� ���� // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
                                                            // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 

    public event EventHandler OnStartMoving; // ����� ��������� (����� ���� ������ �������� �� �������� ������� Event)
    public event EventHandler OnStopMoving; // ��������� �������� (����� ���� �������� �������� �� �������� ������� Event)  
    public event EventHandler<OnChangeFloorsStartedEventArgs> OnChangedFloorsStarted; // ������ ������ ����� 
    public class OnChangeFloorsStartedEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� �������� ������� ����� � ������� �������
    {
        public GridPositionXZ unitGridPosition; // ������ �������
        public GridPositionXZ targetGridPosition; // ���� �������
    }

    private int _moveDistance ; // ������������ ��������� �������� � �����

    private List<Vector3> _positionList; // ������� ������� ������ ���������� ���� (� ������������ �������)
    private int _currentPositionIndex; // ������� ������� ������
    private bool _isChangingFloors; // ��� ����� ������
    private float _differentFloorsTeleportTimer; // ������ ������������ �� ������ �����
    private float _differentFloorsTeleportTimerMax = .5f; // ������������ ������ ������������ �� ������ ����� (��� ����� ��������������� �������� ������ ��� �������)
    private List<GridPositionXZ> _validGridPositionList = new List<GridPositionXZ>(); // ������ ���������� �������� ������� ��� ��������

    
    

#if PATHFINDING
    private BlockManager.TraversalProvider _traversalProvider; // ���������(���������) ������
    private ABPath _path; // ���� 
    private ConstantPath _constantPath;  // �������� ����� ������ ������
    private List<SingleNodeBlocker> _obstaclesIgnoreList; // ������ ����������� ������� ����� ������������. ������� ���� ������ �����
    private SingleNodeBlocker _singleNodeBlocker; // ����������� ���� �� ����� �����
    private Seeker _seeker; 

    public override void SetupForSpawn(Unit unit)
    {
        base.SetupForSpawn(unit);   
        // ���������� ���������� ������ ���, ����� �� ���������� ��� ����, ������� ������������� SingleNodeBlocker
        // �� ����������� SingleNodeBlocker, �������������� ����� ����� (�� �� �����, ����������� ����� ����).
        _singleNodeBlocker = GetComponent<SingleNodeBlocker>();
        _singleNodeBlocker.BlockAtCurrentPosition(); // ���������� ���� ��� ����
        _obstaclesIgnoreList = new List<SingleNodeBlocker>() { _singleNodeBlocker };
        _traversalProvider = new TraversalProvider(BlockManager.Instance, BlockMode.AllExceptSelector, _obstaclesIgnoreList);

        _moveDistance = _unit.GetUnitTypeSO().GetBasicMoveDistance();

        _seeker = GetComponent<Seeker>();
        _seeker.pathCallback += Seeker_OnPathComplete; // ���������� ���� ��������

        
        _unit.GetUnitActionSystem().OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;  //���������� ��������� ���� �������
        _unit.GetUnitActionSystem().OnBusyChanged += UnitActionSystem_OnBusyChanged; //���������� �� ������� ��������� �������� 

        GeneratePossibleMoves(_unit); // ����������� ��������� ���� � ������� (A* Pathfinding Project4.2.18) ��� ������ �����
    }
   
#endif

    private void Update()
    {      
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

#if PATHFINDING
        if (!_seeker.IsDone())//���� ���� �� �������� 
        {
            return; // ������� � ���������� ��� ����            
        }
        
#endif
        // ���� ��������� �� ������ ����� �� _positionList, ������ ��������� ������ ����� targetPosition
        Vector3 targetPosition = _positionList[_currentPositionIndex]; // ������� �������� ����� ������� �� ����� � �������� ��������

        if (_isChangingFloors) // ���� ���� ������� ���� ��
        {
            // ������ ��������� � ������������
            // ��� ������� � ������, � ������� ���� ����� �����������������, ���������� ��� �� �� ������� � ������� ������ �� ������ �� ����������� (�� ������� ����� ��� ����)
            Vector3 targetSameFloorPosition = targetPosition; // ������� ������� ����� �� ����� = ������� ������
            targetSameFloorPosition.y = transform.position.y; // ������� ������� �� ��� � ��� � ������

            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized; // ����������� ��������

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotateSpeed);

            _differentFloorsTeleportTimer -= Time.deltaTime; //�������� ������ ������������ �� ������ �����
            if (_differentFloorsTeleportTimer < 0f) // �� ��������� ������� // ���������� ������������� ������ � ��������������� � ������� ��������� (� ����� � ������ ������� ������� ����� ����������� �������� ������)
            {
                _isChangingFloors = false;
                transform.position = targetPosition;
                _unit.UpdateGridPosition();
            }
        }
        else
        {
            // ������� ������ �����������

            Vector3 moveDirection = (targetPosition - transform.position).normalized; // ����������� ��������, ��������� ������

            float rotateSpeed = 10f; //��� ������ ��� �������
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed); // ������ �����. ������� Lerp �� - Slerp ���������� ������������� ����� ������������� a � b �� ����������� t. �������� t ��������� ���������� [0, 1]. ����������� ��� ��� �������� ��������, ������� ������ ������������� ����� ������ ������������ a � ������ ������������ b �� ������ �������� ��������� t. ���� �������� ��������� ������ � 0, �������� ������ ����� ������ � a, ���� ��� ������ � 1, �������� ������ ����� ������ � b.

            float moveSpead = 4f; //����� ���������//
            transform.position += moveDirection * moveSpead * Time.deltaTime;
        }

        float stoppingDistance = 0.2f; // ��������� ��������� //����� ���������//
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
        {
            _currentPositionIndex++; // �������� ������ �� �������
            _unit.UpdateGridPosition();

            if (_currentPositionIndex >= _positionList.Count) // ���� �� ����� �� ����� ������ �����...
            {
                _unit.GetSoundManager().SetLoop(false);
                _unit.GetSoundManager().Stop();

                OnStopMoving?.Invoke(this, EventArgs.Empty); //�������� ������� ��������� ��������
                _singleNodeBlocker.BlockAtCurrentPosition(); // ���������� ���� �� ����� ����� � ����������� ����������
                ActionComplete(); // ������� ������� ������� �������� ���������                
            }
            else
            {
                targetPosition = _positionList[_currentPositionIndex]; // ������� �������� ����� ������� �� ����� � �������� ��������

                GridPositionXZ targetGridPosition = _unit.GetLevelGrid().GetGridPosition(targetPosition); // ������� �������� ������� ������� �������
                GridPositionXZ unitGridPosition = _unit.GetLevelGrid().GetGridPosition(transform.position); // ������� �������� ������� �����                               
                
                if (targetGridPosition.floor != unitGridPosition.floor) // ���� ���� ������� �������� �� ��������� � ������ ����� �� ...              
                {
                    // ������ �����
                    _isChangingFloors = true;
                    _differentFloorsTeleportTimer = _differentFloorsTeleportTimerMax;

                    OnChangedFloorsStarted?.Invoke(this, new OnChangeFloorsStartedEventArgs // �������� ������� � ��������� �������� ������� ������ � ���� �������
                    {
                        unitGridPosition = unitGridPosition,
                        targetGridPosition = targetGridPosition,
                    });
                }
            }
        }
    }


    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // �������� � ������� �������. � �������� �������� �������� �������  � �������. ������� �� ��� �������� ����� ������� �������
    {
#if PATHFINDING
        _path = ABPath.Construct(transform.position, _unit.GetLevelGrid().GetWorldPosition(gridPosition)); // ��������
        _path.traversalProvider = _traversalProvider; // ��������� ������� ��� ������
        // ������������ ���� ��� �������
        _seeker.StartPath(_path);   // ��������
#else
        List<GridPosition> pathGridPositionList = PathfindingMonkey.Instance.FindPath(_unit.GetGridPosition(), gridPosition, out int pathLength); // ������� ������ ���� ������� ����� �� �������� ��������� ��������� ����� �� �������� (out int pathLength �������� ��� �� ��������������� ���������)

       // ���� ������������� ���������� ������ GridPosition � ������� ���������� Vector3
       _positionList = new List<Vector3>(); // �������������� ������ �������

        foreach (GridPosition pathGridPosition in pathGridPositionList) // ��������� ���������� ������ pathGridPositionList, ����������� �� � ������� ���������� � ������� � _positionList
        {
            _positionList.Add(_unit.GetLevelGrid().GetWorldPosition(pathGridPosition)); // ����������� pathGridPosition � ������� � ������� � _positionList
        }
#endif
        _unit.GetSoundManager().SetLoop(true);
        _unit.GetSoundManager().Play(SoundName.Move);
        _currentPositionIndex = 0; // �� ��������� ���������� � ����
        OnStartMoving?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ��������� 
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }


#if PATHFINDING

    private void Seeker_OnPathComplete(Path path)
    {
        if (path == _constantPath) // �������� ������ ��� ��������
        {
            // � �������� ������
            _validGridPositionList.Clear(); // ������� ����� �����
            foreach (GraphNode node in _constantPath.allNodes)
            {
                if (node == _constantPath.startNode)
                {
                    // ���� ������ �� ������� ����� ���� :(
                    continue;
                }

                GridPositionXZ nodeGridPosition = _unit.GetLevelGrid().GetGridPosition((Vector3)node.position);
                _validGridPositionList.Add(nodeGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����

                OnAnyUnitPathComplete?.Invoke(this, _unit); // �������� �������
            }
        }

        if (path == _path) // �������� ���� �� � �� �
        {
            // ������ ���� ��������
            _positionList = _path.vectorPath;
        }
            
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        if (_unit.GetUnitActionSystem().GetSelectedUnit() == _unit) // ���� ������ ���� ����
        {
          GeneratePossibleMoves(_unit); // ����������� ��������� ���� � ������� (A* Pathfinding Project4.2.18) ��� ������ �����
        }
    }
    private void UnitActionSystem_OnBusyChanged(object sender, EventArgs e)
    {
        if (_unit.GetUnitActionSystem().GetSelectedUnit() == _unit) // ���� ������ ���� ����
        {
            GeneratePossibleMoves(_unit); // ����������� ��������� ���� � ������� (A* Pathfinding Project4.2.18) ��� ������ �����
        }
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList() //�������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
    {       
        return _validGridPositionList;
    }

    public void GeneratePossibleMoves(Unit unit) // ����������� ��������� ���� � ������� (A* Pathfinding Project4.2.18) ��� ������ �����
    {
        _constantPath = ConstantPath.Construct(transform.position, _moveDistance * 1000 * Mathf.RoundToInt(_unit.GetLevelGrid().GetCellSize()));
        _constantPath.traversalProvider = _traversalProvider; // ��������� ������� ��� ������
        // ������������ ���� ��� �������
        _seeker.StartPath(_constantPath);

        // ������� ������������� ����� ������ �����
        /*GraphNode unitNode = AstarPath.active.GetNearest(unit.transform.position).node; // ������� ���� �����
        List<GraphNode> graphNodes = PathUtilities.BFS(unitNode, _moveDistance);

             _validGridPositionList.Clear(); // ������� ����� �����
        foreach (GraphNode node in graphNodes)
        {
            if (node == unitNode)
            {
                // ���� ������ �� ������� ����� ���� :(
                continue;
            }

            GridPositionXZ nodeGridPosition = _unit.GetLevelGrid().GetGridPosition((Vector3)node.position);
            _validGridPositionList.Add(nodeGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����

            OnAnyUnitPathComplete?.Invoke(this, _unit); // �������� �������
        }*/
    }


#else
    public override List<GridPosition> GetValidActionGridPositionList() //�������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
    {
        _validGridPositionList.Clear(); // ������� ����� �����

        GridPosition unitGridPosition = _unit.GetGridPosition(); // ������� ������� � �����
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� maxMoveDistance
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                for (int floor = -maxMoveDistance; floor <= maxMoveDistance; floor++)
                {

                    GridPosition offsetGridPosition = new GridPosition(x, z, floor); // ��������� �������� �������. ��� ������� ���������(0,0, floor-����) �������� ��� ���� 
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                    if (!_unit.GetLevelGrid().IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                    {
                        continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                    }

                    if (unitGridPosition == testGridPosition) // �������� �������� ������� ��� ���������� ��� ����
                    {
                        // ���� ������ �� ������� ����� ���� :(
                        continue;
                    }

                    if (!PathfindingMonkey.Instance.HasPath(unitGridPosition, testGridPosition)) //�������� �������� ������� ���� ������ ������ 
                    {
                        continue;
                    }

                    if (!PathfindingMonkey.Instance.IsWalkableGridPosition(testGridPosition)) //�������� �������� ������� ��� ������ ������ (���� ����������� ����� �������)
                    {
                        continue;
                    }

                    if (_unit.GetLevelGrid().HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ���������� ������ �����
                    {
                        // ������� ������ ������ ������ :(
                        continue;
                    }

                     int pathfindingDistanceMultiplier = 10; // ��������� ���������� ����������� ���� (� ������ PathfindingMonkey ������ ��������� �������� �� ������ � ��� ����� ����� 10 �� ��������� 14, ������� ������� ��� ��������� �� ���������� ������)
                    if (PathfindingMonkey.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier) //�������� �������� ������� - ���� ��������� �� ����������� ������ ������ ���������� ������� ���� ����� ���������� �� ���� ���
                    {
                        // ����� ���� ������� ������
                        continue;
                    }
                    _validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����
                                                                 //Debug.Log(testGridPosition);
                }
            }
        }                
        return _validGridPositionList;
    }
#endif

    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "��������";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        int targetCountAtPosition = _unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition); // � ����� ������ ������ ShootAction � ������� � ���� "�������� ���������� ����� �� �������"
                                                                                                           //� �����, ��� ����� ������� ���� �� ������ ����� �����, ������� ����� ������������ ������ � �������� ������������� ������� ������������ ������� �����. ����� �� ������ ������� ������ � ��������������� ���� � �� ��������� ���� �������� � ������ (Move � Shoot)
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtPosition * 10 + 40, //������ � ����� ������� ����������� ���������� ����� ����� � ����������. �������� ���� � ��� ���� ������� �����, � ������� ��� ���������� �����, � ������ ������� �����, � ������� ���� ���� ���������� ����, �� �������� �� ������ ������� �����, ��������� �������� �������� �������� �� ���������� ���������� �����.
        };
        // ��������� �������� ���������� ��� ������ ����� ����� ��������� ������ �������� ��������, ���� �������� ����� ���������� ����� 20%, ���� ����� �������� ����������� ����������� �������� �� ������, �� ������� ��� ���������� �����.
        // �� ����� �� ��������� �������������� ��� ������� �� ����������� ������, � ������� ������ ��������, ��� ������� �� ����������� ������ � ����� ������� ���������
        // ����� ���� ����� ������������, �����, �������, ��� ���������� ����� ������ ����� ��������� �����, ����������� ������ ��� ������� ��������� ��������.
    }

    //����� ������������ ���� ������� ����� ����������.
    //https://community.gamedev.tv/t/more-aggressive-enemy/220615?_gl=1*ueppqc*_ga*NzQ2MDMzMjI4LjE2NzY3MTQ0MDc.*_ga_2C81L26GR9*MTY3OTE1NDA5Ni4zMS4xLjE2NzkxNTQ1MjYuMC4wLjA.

    public override string GetToolTip()
    {
        return "���� - " + GetActionPointCost() + "\n" +
            "��������� - " + GetMaxActionDistance();
    }

    public override int GetMaxActionDistance()
    {
        return _moveDistance;
    }


#if PATHFINDING
    private void OnDestroy()
    {
        _seeker.pathCallback -= Seeker_OnPathComplete;
        _unit.GetUnitActionSystem().OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;  
    }
#endif
}
