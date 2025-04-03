using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MoveAction : BaseAction // �������� ����������� ��������� ����� BaseAction // ������� � ��������� ����� // ����� �� ������ �����
{
    public static event EventHandler<Unit> OnAnyUnitPathComplete; // � ������ ����� �������� ���� // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
                                                                  // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 

    public event EventHandler OnStartMoving; // ����� ��������� (����� ���� ������ �������� �� �������� ������� Event)
    public event EventHandler OnStopMoving; // ��������� �������� (����� ���� �������� �������� �� �������� ������� Event)  
    public event EventHandler<OnChangeFloorsStartedEventArgs> OnChangedFloorsStarted; // ������ ������ ����� 
    public struct OnChangeFloorsStartedEventArgs // �������� ����� �������, ����� � ��������� ������� �������� �������� ������� ����� � ������� �������
    {
        public GridPositionXZ unitGridPosition; // ������ �������
        public GridPositionXZ targetGridPosition; // ���� �������
    }

    private int _moveDistance; // ������������ ��������� �������� � �����
      
    private int _currentPositionIndex; // ������� ������� ������
    private bool _isStartChangingFloors = true; // ����� ������ ����
    private float _differentFloorsTeleportTimer; // ������ ������������ �� ������ �����
    private float _differentFloorsTeleportTimerMax = .5f; // ������������ ������ ������������ �� ������ ����� (��� ����� ��������������� �������� ������ ��� �������)
    private List<GridPositionXZ> _validGridPositionList = new List<GridPositionXZ>(); // ������ ���������� �������� ������� ��� ��������

    /// <summary>
    /// ������� - ���������� ������� ����� � ���� ���� <br/>
    /// PathNode - �������� ���� � ����
    /// </summary>
    private NativeParallelHashMap<int3, PathNode> _validGridPositionPathNodeDict; //�������. ���� - ���������� �������� �������. �������� - ���� ��� ���� �������� �������
    /// <summary>
    /// ������� ����� ����
    /// </summary>
    private int3 _gridPositionEndPath; 
    private UnitActionSystem _unitActionSystem;
    private PathfindingProvider _pathfindingProvider;

    public override void SetupForSpawn(Unit unit)
    {
        base.SetupForSpawn(unit);
        _unitActionSystem = _unit.GetUnitActionSystem();
        _pathfindingProvider = _unit.GetPathfindingProvider();
        _moveDistance = _unit.GetUnitTypeSO().GetBasicMoveDistance();

        _pathfindingProvider.OnPathfindingComplete += PathfindingProvider_OnPathfindingComplete; //���������� ���� ��������
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;  //���������� ��������� ���� �������
    }

    private void PathfindingProvider_OnPathfindingComplete(object sender, NativeParallelHashMap<int3, PathNode> validGridPositionPathNodeDict)
    {
        if (_unitActionSystem.GetSelectedUnit() == _unit) // ���� ������ ���� ����
        {
            _validGridPositionPathNodeDict = validGridPositionPathNodeDict;
           
            foreach (var collection in validGridPositionPathNodeDict)
            {
                _validGridPositionList.Add(new GridPositionXZ(collection.Key));
            }
        }
    }
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        if (selectedUnit == _unit) // ���� ������ ���� ����
        {
            _validGridPositionList.Clear(); // ������� ����� �����
        }
    }

    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        if (!_pathfindingProvider.IsPathfindingComplete())//���� ���� �� �������� 
        {
            return; // ������� � ���������� ��� ����            
        }

        // ���� ��������� �� ������ ����� �� pathWorldPositionList, ������ ��������� ������ ����� targetPosition
        Vector3 targetPosition = _validGridPositionPathNodeDict[_gridPositionEndPath].pathWorldPositionList[_currentPositionIndex]; // ������� �������� ����� ������� �� ����� � �������� ��������

        GridPositionXZ targetGridPosition = _unit.GetLevelGrid().GetGridPosition(targetPosition); // ������� �������� ������� ������� �������
        GridPositionXZ unitGridPosition = _unit.GetLevelGrid().GetGridPosition(transform.position); // ������� �������� ������� �����  
       
        if (targetGridPosition.floor != unitGridPosition.floor)// ���� ���� ������� �������� �� ��������� � ������ ����� �� ...      
        {
            if (_isStartChangingFloors)// ���� ������ ������ ������ ���� �� �������� ������ � �������� �������
            {               
                _isStartChangingFloors = false;
                _differentFloorsTeleportTimer = _differentFloorsTeleportTimerMax;

                OnChangedFloorsStarted?.Invoke(this, new OnChangeFloorsStartedEventArgs // �������� ������� � ��������� �������� ������� ������ � ���� �������
                {
                    unitGridPosition = unitGridPosition,
                    targetGridPosition = targetGridPosition,
                });
            }

            // ������ ��������� � ������������
            // ��� ������� � ������, � ������� ���� ����� �����������������, ���������� ��� �� �� ������� � ������� ������ �� ������ �� ����������� (�� ������� ����� ��� ����)
            Vector3 targetSameFloorPosition = targetPosition; // ������� ������� ����� �� ����� = ������� ������
            targetSameFloorPosition.y = transform.position.y; // ������� ������� �� ��� � ��� � ������

            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized; // ����������� ��������

            float rotateSpeed = 10f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateDirection), Time.deltaTime * rotateSpeed);

            _differentFloorsTeleportTimer -= Time.deltaTime; //�������� ������ ������������ �� ������ �����
            if (_differentFloorsTeleportTimer < 0f) // �� ��������� ������� // ���������� ������������� ������ � ��������������� � ������� ��������� (� ����� � ������ ������� ������� ����� ����������� �������� ������)
            {
                _isStartChangingFloors = true; //������� �������� ������ ����� ������
                transform.position = targetPosition;
                _unit.UpdateGridPosition();
            }
        }
        else
        {
            // ������� ������ �����������

            Vector3 moveDirection = (targetPosition - transform.position).normalized; // ����������� ��������, ��������� ������

            float rotateSpeed = 10f; //��� ������ ��� �������
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotateSpeed); // ������ �����. ������� Lerp �� - Slerp ���������� ������������� ����� ������������� a � b �� ����������� t. �������� t ��������� ���������� [0, 1]. ����������� ��� ��� �������� ��������, ������� ������ ������������� ����� ������ ������������ a � ������ ������������ b �� ������ �������� ��������� t. ���� �������� ��������� ������ � 0, �������� ������ ����� ������ � a, ���� ��� ������ � 1, �������� ������ ����� ������ � b.

            float moveSpead = 4f; //����� ���������//
            transform.position += moveDirection * moveSpead * Time.deltaTime;
        }

        float stoppingDistanceSQR = 0.4f; // ��������� ��������� � ������� ��� //����� ���������//
        if ((targetPosition-transform.position).sqrMagnitude < stoppingDistanceSQR)  // ���� ������� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
        {
            _currentPositionIndex--; // �������� ������ �� �������
            _unit.UpdateGridPosition();

            if (_currentPositionIndex == 0) // ���� �� ����� �� ����� ������ �����...
            {
                _unit.GetSoundManager().SetLoop(false);
                _unit.GetSoundManager().Stop();

                OnStopMoving?.Invoke(this, EventArgs.Empty); //�������� ������� ��������� ��������
                                                             // _singleNodeBlocker.BlockAtCurrentPosition(); // ���������� ���� �� ����� ����� � ����������� ����������
                ActionComplete(); // ������� ������� ������� �������� ���������                
            }           
        }
    }


    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // �������� � ������� �������. � �������� �������� �������� �������  � �������. ������� �� ��� �������� ����� ������� �������
    {
        _gridPositionEndPath =  gridPosition.ParseInt3();
        _currentPositionIndex = _validGridPositionPathNodeDict[_gridPositionEndPath].pathWorldPositionList.Length - 1;

        _unit.GetSoundManager().SetLoop(true);
        _unit.GetSoundManager().Play(SoundName.Move);
      
        OnStartMoving?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ��������� 
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    public override bool IsValidActionGridPosition(GridPositionXZ gridPosition) //(���������) �������� �� �������� ������� ���������� ��� ��������
    {
        return _pathfindingProvider.IsPathfindingComplete() ? _validGridPositionPathNodeDict.ContainsKey(gridPosition.ParseInt3()) : false;
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList()  // ������������� ������� �������
    {
        return _validGridPositionList;
    }

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



    private void OnDestroy()
    {
        _pathfindingProvider.OnPathfindingComplete -= PathfindingProvider_OnPathfindingComplete; 
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;  
    }

}
