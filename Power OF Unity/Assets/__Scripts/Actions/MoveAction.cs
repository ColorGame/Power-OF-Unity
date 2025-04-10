using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static PathNodeSystem;

public class MoveAction : BaseAction // �������� ����������� ��������� ����� BaseAction // ������� � ��������� ����� // ����� �� ������ �����
{
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
    private PathfindingProviderSystem _pathfindingProviderSystem;

    public override void SetupForSpawn(Unit unit)
    {
        base.SetupForSpawn(unit);
        _unitActionSystem = _unit.GetUnitActionSystem();
        _pathfindingProviderSystem = _unit.GetPathfindingProvider();
        _moveDistance = _unit.GetUnitTypeSO().GetBasicMoveDistance();

        _pathfindingProviderSystem.OnPathfindingComplete += PathfindingProvider_OnPathfindingComplete; //���������� ���� ��������
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;  //���������� ��������� ���� �������      
    }


    private void PathfindingProvider_OnPathfindingComplete(object sender, EventArgs e)
    {
        if (_unitActionSystem.GetSelectedUnit() == _unit) // ���� ������ ���� ����
        {
            _validGridPositionList = _pathfindingProviderSystem.GetValidGridPositionMoveForSelectedUnit();
            _validGridPositionPathNodeDict = _pathfindingProviderSystem.GetvalidGridPositionPathNodeDict();
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

        if (!_pathfindingProviderSystem.IsPathfindingComplete())//���� ���� �� �������� 
        {
            return; // ������� � ���������� ��� ����            
        }

        // ���� ��������� �� ������ ����� �� pathWorldPositionList, ������ ��������� ������ ����� targetPosition
        Vector3 targetPosition = _validGridPositionPathNodeDict[_gridPositionEndPath].pathWorldPositionList[_currentPositionIndex]; // ������� �������� ����� ������� �� ����� � �������� ��������

        GridPositionXZ targetGridPosition = _levelGrid.GetGridPosition(targetPosition); // ������� �������� ������� ������� �������
        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� �������� ������� �����  

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
                _unit.UpdateGridPosition(targetGridPosition); //��� ����� ����� ������� �������� �������
            }
        }
        else
        {
            // ������� ������ �����������
            Vector3 moveDirection = (targetPosition - transform.position).normalized; // ����������� ��������, ��������� ������

            float rotateSpeed = 10f; //��� ������ ��� �������   
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotateSpeed); // ������ �����. ������� Lerp �� - Slerp ���������� ������������� ����� ������������� a � b �� ����������� t. �������� t ��������� ���������� [0, 1]. ����������� ��� ��� �������� ��������, ������� ������ ������������� ����� ������ ������������ a � ������ ������������ b �� ������ �������� ��������� t. ���� �������� ��������� ������ � 0, �������� ������ ����� ������ � a, ���� ��� ������ � 1, �������� ������ ����� ������ � b.

            float moveSpead = 8f; //����� ���������//
            transform.position += moveDirection * moveSpead * Time.deltaTime;
        }

        float stoppingDistanceSQR = 0.05f; // ��������� ��������� � ������� ��� //����� ���������//
        float distSQR = (targetPosition - transform.position).sqrMagnitude;
        if (distSQR < stoppingDistanceSQR)  // ���� ������� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
        {
            _currentPositionIndex--; // �������� ������ �� �������
            _unit.UpdateGridPosition(targetGridPosition); 

            if (_currentPositionIndex < 0) // ���� �� ������ ��� ���� ����...
            {
                _unit.GetSoundManager().SetLoop(false);
                _unit.GetSoundManager().Stop();

                OnStopMoving?.Invoke(this, EventArgs.Empty); //�������� ������� ��������� ��������                                                          
                ActionComplete(); // ������� ������� ������� �������� ���������                
            }
        }
    }


    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // �������� � ������� �������. � �������� �������� �������� �������  � �������. ������� �� ��� �������� ����� ������� �������
    {
        _gridPositionEndPath = gridPosition.ParseInt3();
        _currentPositionIndex = _validGridPositionPathNodeDict[_gridPositionEndPath].pathWorldPositionList.Length - 1;

        _unit.GetSoundManager().SetLoop(true);
        _unit.GetSoundManager().Play(SoundName.Move);

        OnStartMoving?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ��������� 
        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    public override bool IsValidActionGridPosition(GridPositionXZ gridPosition) //(���������) �������� �� �������� ������� ���������� ��� ��������
    {
        return _pathfindingProviderSystem.IsPathfindingComplete() ? _validGridPositionPathNodeDict.ContainsKey(gridPosition.ParseInt3()) : false;
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
        _pathfindingProviderSystem.OnPathfindingComplete -= PathfindingProvider_OnPathfindingComplete;
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }

}
