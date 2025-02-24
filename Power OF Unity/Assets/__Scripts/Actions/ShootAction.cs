using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ShootAction : BaseAction
{

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;  // ������� - ����� ����� �������� (����� ����� ���� ������ �������� �� �������� ������� Event) // <Unit> ������� �������� �������� ����� ��� ����
                                                                    // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
                                                                    // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����,
                                                                    // ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 

    public event EventHandler<OnShootEventArgs> OnShoot; // ����� �������� (����� ���� ������ �������� �� �������� ������� Event) // <OnShootEventArgs> ������� �������� �������� ����� ��� ����

    public class OnShootEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� ������ ������
    {
        public Unit targetUnit; // ������� ���� � ���� ��������
        public Unit shootingUnit; // ����������� ���� ��� ��� ��������
    }

    private enum State
    {
        Aiming,     // ������������
        Shooting,   // ��������
        Cooloff,    // ��������� (��������� �������� ������ ��� �� �������� ��������)
    }

    
    [SerializeField] private Transform _shootPointTransform; // � ���������� �������� ����� �������� ����� �� ��������    
    [SerializeField] private Transform _targetAnimationAim; //���� ��� �������� ������������ 
    [SerializeField] private Rig _aimRig; // �������� ������������ �����

    private LayerMask _obstaclesDoorMousePlaneCoverLayerMask;//����� ���� ����������� ���� ������� Obstacles � Door � MousePlane(���) Cover// ����� �� ���� ������ � ���� ���������� ����� ����� -Obstacles, � �� ������ -DoorInteract //��� ����� ������� ������ �������� �������� �� Box collider ����� ����� ����� ����� ������������� ������� ���� // Cover ������ ������������� ���� ����� �������� ���� ������� (���� ��������� ��� ������� Cover)
    private LayerMask _smokeCoverLayerMask; //����� ���� ���� � ��������� (�������� � ����������) ���� ������� Smoke � Cover // ����� �� ���� ��������� � ���� �� �������,  ���������� ��������������� ����� �����t 
    private LayerMask _coverLayerMask; // ����� ���� �������
    private LayerMask _smokeLayerMask; // ����� ���� ���   

    private int _numberShotInOneAction; // ���������� ��������� �� ���� ��������
    private float _delayShoot; //�������� ����� ����������
    private int _maxShootDistance ;
    private float _percentageShootDistanceIncrease;// ������� ���������� ��������� �������� 
    private int _shootDamage; // �������� �����
    private float _percentageShootDamageIncrease;//������� ���������� ����� �� �������� 
    private State _state; // ��������� �����
    private float _stateTimer; //������ ���������
    private Unit _targetUnit; // ���� � �������� �������� �������
    private Vector3 _targetUnitAimPointPosition; // ����� ������������ � �����
    private bool _canShootBullet; // ����� �������� �����    
    private float _timerShoot; //������ ��������
    private int _counterShoot; // ������� ���������
    private bool _hit; // ����� ��� ��������
    private float _hitPercent; // ������� ���������
    private float _cellSize;// ������ ������
    private float _aimRigWeight;// ��� �������� ������������ �����

    private bool _haveSpotterFire = false; // ���� �������������� ���� (�� ��������� ���)
    private Unit _spotterFireUnit; // ���� �������������� ����    

    protected override void Start()
    {
        base.Start();

        _obstaclesDoorMousePlaneCoverLayerMask = LayerMask.GetMask("Obstacles", "Door", "MousePlane", "Cover");
        _smokeCoverLayerMask = LayerMask.GetMask("Smoke", "Cover");
        _coverLayerMask = LayerMask.GetMask("Cover");
        _smokeLayerMask = LayerMask.GetMask("Smoke");

        _hitPercent = 1f; //��������� ������� ��������� ������������ 100%
        _cellSize = _unit.GetLevelGrid().GetCellSize();
       // _targetAnimationAim.position = _shootPointTransform.position; // ��� ������ ���� ��� �������� ������������  ����� ����� ������������ �� ����� �����, ��� �� �� ���� ������ ��������� �����������
    }

    public override void SetPlacedObjectTypeWithActionSO(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO)
    {
        base.SetPlacedObjectTypeWithActionSO (placedObjectTypeWithActionSO);
        _numberShotInOneAction = ((ShootingWeaponTypeSO)placedObjectTypeWithActionSO).GetNumberShotInOneAction();
    }

    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        _stateTimer -= Time.deltaTime; // �������� ������ ��� ������������ ���������
        _timerShoot -= Time.deltaTime;// �������� ������ ��� ���������� � � ����������

        switch (_state) // ������������� ���������� ���� � ����������� �� _state
        {
            case State.Aiming:

                Vector3 aimDirection = (_targetUnit.GetTransformPosition() - transform.position).normalized; // ����������� ������������, ��������� ������
                aimDirection.y = 0; // ����� ���� �� ���������� ��� �������� (�.�. ������ ����� �������������� ������ �� ��������� x,y)

                float rotateSpeed = 10f; //����� ���������// ��� ������ ��� �������
                transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed); // ������ �����.                               

                _aimRig.weight = Mathf.Lerp(_aimRig.weight, _aimRigWeight, Time.deltaTime * 20f); // ������ ������� ��� ��� �������� ������������

                if (_haveSpotterFire) // ���� ���� �������������� �� ��������� � ���
                {
                    Vector3 SpotterUnitEnemyDirection = (_targetUnit.GetTransformPosition() - _spotterFireUnit.GetTransformPosition()).normalized; // ����������� �� ��������������� � �����
                    SpotterUnitEnemyDirection.y = 0;

                    _spotterFireUnit.GetTransform().forward = Vector3.Slerp(_spotterFireUnit.GetTransform().forward, SpotterUnitEnemyDirection, Time.deltaTime * rotateSpeed); // ������ ���������������.    
                }
                break;

            case State.Shooting:

                if (_canShootBullet && _timerShoot <= 0) // ���� ���� �������� ����� � ������ ����� ...
                {
                    Shoot();
                    _timerShoot = _delayShoot; // ��������� ������ = �������� ����� ����������
                    _counterShoot += 1; // �������� � �������� ��������� 1 
                }

                if (_counterShoot >= _numberShotInOneAction || _targetUnit.GetHealthSystem().IsDead()) //����� ������� 3 ���� ��� ����� ���� ����
                {
                    _canShootBullet = false;
                    _counterShoot = 0; //������� ������� ����
                }

                break;

            case State.Cooloff: // ���� ������ �� ����� �������� �������� ��������� ��� �������
                _aimRig.weight = Mathf.Lerp(_aimRig.weight, _aimRigWeight, Time.deltaTime * 20f); // ������ ������� ��� ��� �������� ������������
                break;
        }

        if (_stateTimer <= 0) // �� ��������� ������� _stateTimer ������� NextMusic() ������� � ���� ������� ���������� ���������. �������� - � ���� ���� TypeGrenade.Aiming: ����� � case TypeGrenade.Aiming: ��������� �� TypeGrenade.Shooting;
        {
            NextState(); //��������� ���������
        }
    }

    

    private void NextState() //������� ������������ ���������
    {
        switch (_state)
        {
            case State.Aiming:
                _state = State.Shooting;
                float shootingStateTime = _numberShotInOneAction * _delayShoot + 0.5f; // ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ������� = ���������� ��������� * ����� ��������
                _stateTimer = shootingStateTime;
                _targetAnimationAim.position = _targetUnitAimPointPosition; //���� ��� �������� ������������  ����� ����� ������������ �� �����
                break;
            case State.Shooting:
                _state = State.Cooloff;
                float cooloffStateTime = 0.5f; // ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ���������� //���� ���������// ����������������� �������� ��������(��������� ������))
                _stateTimer = cooloffStateTime;
                _aimRigWeight = 0; // �������� ��� ��� �������� ������������
                break;
            case State.Cooloff:
                _targetAnimationAim.position = _shootPointTransform.position; // ���� ��� �������� ������������  ����� ����� ������������ �� ����� �����, ��� �� ��� ������ ���� �������� ���� ������� � ������ �����������
                ActionComplete(); // ������� ������� ������� �������� ���������
                break;
        }

        //Debug.Log(_state);
    }

    public override void TakeAction(GridPositionXZ targetGridPosition, Action onActionComplete) // ���������� ��������
    {
        _targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(targetGridPosition); // ������� ����� � �������� ������� � �������� ���                               
        _targetUnitAimPointPosition = _targetUnit.GetHeadTransform().position; // ������� ������������ �������� �����. 

        _state = State.Aiming; // ���������� ��������� ������������ 
        float aimingStateTime = 0.5f; //����������������� ��������� ������������ //����� ���������//
        _stateTimer = aimingStateTime;

        _canShootBullet = true;

        _aimRigWeight = 1; // �������� ��� ��� �������� ������������           

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� � ��������� ������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    private void Shoot() // �������
    {
        //������� ������������� ���� ��� ������ �����. �� ��� ������ ��� ��������� �� ������� ScreenShake. ������� ��������� �� �������
        //ScreenShake.Instance.Shake(5);
        OnAnyShoot?.Invoke(this, new OnShootEventArgs // ������� ����� ��������� ������ OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootingUnit = _unit
        }); // �������� ������� ����� ����� �������� � � �������� ��������� � ���� �������� � ��� �������� (���������� ScreenShakeActions ��� ���������� ������ ������ � UnitRagdollSpawner- ��� ����������� ����������� ���������)

        OnShoot?.Invoke(this, new OnShootEventArgs // ������� ����� ��������� ������ OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootingUnit = _unit
        }); // �������� ������� ����� �������� � � �������� ��������� � ���� �������� � ��� �������� (UnitAnimator-���������)               

        // ��������� ��������� ��� ������
        _hit = UnityEngine.Random.Range(0, 1f) < GetHitPercent(_targetUnit);

        Transform bulletProjectilePrefabTransform = Instantiate(GameAssetsSO.Instance.bulletProjectilePrefab, _shootPointTransform.position, Quaternion.identity); // �������� ������ ���� � ����� ��������
        BulletProjectile bulletProjectile = bulletProjectilePrefabTransform.GetComponent<BulletProjectile>(); // ������ ��������� BulletProjectile ��������� ����

        _unit.GetSoundManager().PlayOneShot(SoundName.Shoot); // ������������� ���� 
        if (_hit) // ���� ������ ��
        {
            bulletProjectile.Setup(_targetUnitAimPointPosition, _hit); // � �������� ������� ������� ������������ �������� �����
            _targetUnit.GetHealthSystem().Damage(GetShootDamage()); // ��� ����� ����� ����� 5. � ���������� ����� ����� ���� ���������� �� ������ //���� ���������//
        }
        else // ���� ������
        {
            //�������� ������� �� X Y Z
            _targetUnitAimPointPosition += Vector3.one * UnityEngine.Random.Range(-0.8f, 0.8f);
            bulletProjectile.Setup(_targetUnitAimPointPosition, _hit); // � �������� ������� ������� ������� �������� �����. 
        }
    }

    public float GetHitPercent(Unit enemyUnit) // �������� ������� ��������� �� ����� (� �������� �������� �����)
    {
        _hitPercent = 1f; //��������� ������� ��������� ������������ 100%

        if (_haveSpotterFire) // ���� ���� �������������� �� ����� ����
        {
            return _hitPercent;
        }
        // �������� ��� CoverSmokeObject �� ���� ��������

        Vector3 unitWorldPosition = _unit.GetWorldPosition(); // ������� ������� ���������� �����
        Vector3 enemyUnitWorldPosition = enemyUnit.GetWorldPosition(); //������� ������� ���������� ����������
        Vector3 shototDirection = (enemyUnitWorldPosition - unitWorldPosition).normalized; //��������������� ������ ����������� ��������
        float heightRaycast = 0.5f; // ������ �������� ���� (������ ������ ��� �� ������� � Half ������������ ���������)              
        float maxPenaltyAccuracy = 0; // ������������ ����� ������������
        Collider ignoreCoverSmokeCollider = null; // ������������ CoverSmokeObject

        // ��������� ��� � ������ ����� �� ��������� 1.5 ������ � ������ ��� ������� ������. ���� ������ �� ���������� ����� �� ����� �������(��� ����� ������������ �.�. �� �� �������� �� ���� � ������ ������ �����������). ���� ������ �� ���������� ����� �� ����� �������
        if (Physics.Raycast(
                 unitWorldPosition + Vector3.up * heightRaycast,
                 shototDirection,
                 out RaycastHit hitCoverInfo,
                 _cellSize * 1.5f,
                 _coverLayerMask))
        {
            ignoreCoverSmokeCollider = hitCoverInfo.collider;
        }
        /*Debug.DrawRay(unitWorldPosition + Vector3.up * heightRaycast,
                 shototDirection * (_cellSizeWithScaleFactor *1.5f),
                 Color.white,
                 100);*/

        // ��������� ��� CoverSmokeObject � ������� maxPenaltyAccuracy
        RaycastHit[] raycastHitArray = Physics.RaycastAll(
                unitWorldPosition + Vector3.up * heightRaycast,
                shototDirection,
                Vector3.Distance(unitWorldPosition, enemyUnitWorldPosition),
                _smokeCoverLayerMask); // �������� ������ ���� ��������� ����. Obstacles ���� ��������� �.�. ����� ���� ������ ��������

        /*Debug.DrawRay(unitWorldPosition + Vector3.up * heightRaycast,
                shototDirection * Vector3.Distance(unitWorldPosition, enemyUnitWorldPosition),
                 Color.green,
                 999);*/

        foreach (RaycastHit raycastHit in raycastHitArray) // ��������� ��� ���������� ������ � ������� ������������ ����� ������������ maxPenaltyAccuracy
        {
            Collider coverSmokeCollider = raycastHit.collider; // ������� ��������, � ������� ������.

            if (coverSmokeCollider == ignoreCoverSmokeCollider)
            {
                //��������� ��������� ������� ���� ������������(��� ������� �� ���� ��� ������)
                continue;
            }
            CoverSmokeObject coverSmokeObject = coverSmokeCollider.GetComponent<CoverSmokeObject>(); // ������� �� ���������, � ������� ������, ��������� CoverSmokeObject - ������ ������� ��� ���
            float penaltyAccuracy = coverSmokeObject.GetPenaltyAccuracy(); // ������� ����� ������������
            if (penaltyAccuracy > maxPenaltyAccuracy)
            {
                maxPenaltyAccuracy = penaltyAccuracy;
            }
        }

        // ��������� ��� �� ������� ����� ��� �� ��������� ���� � ���� (��� �� ����� �������� ������ ���������(������ �����))
        if (Physics.Raycast(
                 enemyUnitWorldPosition + Vector3.up * heightRaycast,
                 shototDirection * (-1),
                 out RaycastHit hitSmokeInfo,
                 Vector3.Distance(unitWorldPosition, enemyUnitWorldPosition),
                 _smokeLayerMask)) // ��������� ������ ���
        {
            CoverSmokeObject coverSmokeObject = hitSmokeInfo.collider.GetComponent<CoverSmokeObject>(); // ������� �� ���������, � ������� ������, ��������� CoverSmokeObject - ������ �������
            float penaltyAccuracy = coverSmokeObject.GetPenaltyAccuracy(); // ������� ����� ������������
            if (penaltyAccuracy > maxPenaltyAccuracy)
            {
                maxPenaltyAccuracy = penaltyAccuracy;
            }
        }
        return _hitPercent -= maxPenaltyAccuracy;
    }

    public override string GetActionName() // ������� ��� ��� ������
    {
        return "�������";
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList() //�������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
    {
        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����
        return GetValidActionGridPositionList(unitGridPosition);
    }

    //���������� �� ������ ���� ����������.
    public List<GridPositionXZ> GetValidActionGridPositionList(GridPositionXZ unitGridPosition) //�������� ������ ���������� �������� ������� ��� ��������.
                                                                                                //�������� ������ ���������� ����� ������ ������� �����
                                                                                                //� �������� �������� �������� ������� �����                                                                                            
    {
        List<GridPositionXZ> validGridPositionList = new List<GridPositionXZ>();

        int maxShootDistance = GetMaxActionDistance();
        for (int x = -maxShootDistance; x <= maxShootDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� maxShootDistance
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                for (int floor = -maxShootDistance; floor <= maxShootDistance; floor++)
                {

                    GridPositionXZ offsetGridPosition = new GridPositionXZ(x, z, floor); // ��������� �������� �������. ��� ������� ���������(0,0, 0-����) �������� ��� ���� 
                    GridPositionXZ testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                    if (!_unit.GetLevelGrid().IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                    {
                        continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                    }
                    // ��� ������� �������� ������� ���� � �� �������
                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // ����� ���� ������������� ��������� �������� �������
                    if (testDistance > maxShootDistance) //������� ������ �� ����� � ���� ����� // ���� ���� � (0,0) �� ������ � ������������ (5,4) ��� �� ������� �������� 5+4>7
                    {
                        continue;
                    }

                    if (!_unit.GetLevelGrid().HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ (��� ����� ������ � ������� �� ����� �� ��� �������)
                    {
                        // ������� ����� �����, ��� ������
                        continue;
                    }

                    Unit targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� 
                                                                                                      // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                    if (targetUnit.GetType() == _unit.GetType()) // ���� ����������� ���� ���� � ��� ���� ���� ���� �� (���� ��� ��� � ����� ������� �� ����� ������������ ���� ������)
                    {
                        // ��� ������������� � ����� "�������"
                        continue;
                    }

                    // �������� �� ����������������� �� ���� , Cover ������ ������������� ���� ����� �������� ���� ������� (���� ��������� ��� ������� Cover)
                    Vector3 unitWorldPosition = _unit.GetLevelGrid().GetWorldPosition(unitGridPosition); // ��������� � ������� ���������� ���������� ��� �������� ������� �����  
                    Vector3 shototDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized; //��������������� ������ ����������� ��������
                    Vector3 shootPoint = _shootPointTransform.position - unitWorldPosition; // ������� ���������� �� ����� �������� �� ��������� ����� (��������� ��� 2 �����)

                    float reserveHeight = 0.15f; // ������ �� ������ ���� ���� ����� �������� ()
                    if (Physics.Raycast(
                            //_shootPointTransform.gridPosition + Vector3.up * reserveHeight,
                            unitWorldPosition + Vector3.up * (shootPoint.y + reserveHeight), 
                            shototDirection,
                            Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                            _obstaclesDoorMousePlaneCoverLayerMask)) // ���� ��� ����� � ����������� �� (Raycast -������ bool ����������)
                    {
                        // �� �������������� ������������
                        continue;
                    }

                    /*Debug.DrawRay(unitWorldPosition + Vector3.up * (shootPoint.y + reserveHeight),
                        shototDirection * Vector3.Distance(unitWorldPosition, startUnit.GetWorldPositionCenter�ornerCell()),
                        Color.red,
                        999);*/

                    validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����
                                                                 //Debug.Log(testGridPosition);
                }
            }
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� ��  ��� ���������� ��� �������� �������// ������������� ����������� ������� ����� //EnemyAIAction ������ � ������ ���������� ������� �������, ���� ������ - ��������� ������ ������ � ����������� �� ��������� ����� ������� ��� �����
    {
        Unit targetUnit = _unit.GetLevelGrid().GetUnitAtGridPosition(gridPosition); // ������� ����� ��� ���� ������� ��� ���� ����

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            //actionValue = 100 +Mathf.RoundToInt(1- startUnit.GetHealthNormalized()) *100,  // ��������� ������ ��� �������� �� ������ ������������� ������ .
            // �������� ���� ���� ��������� ������ �� GetHealthNormalized() ������ 1  ����� (1-1)*100 = 0 � ����� actionValue ���������� ������� 100
            // �� ���� �������� �������� ����� �� GetHealthNormalized() ������ 0,5 ����� (1-0,5)*100 = 50 � actionValue ������ ������ 150 ����� ������� ���������� �������� 
            // ������ �� �������� ����� � ������ ������ ������������ ��������
            // �������� ������ ������
            actionValue = 100 + Mathf.RoundToInt(AttackScore(targetUnit))
        };
    }

    public float AttackScore(Unit unit) // ������ ����� // � ������ ������� ���� ������, �� ����� ��� ������ ��������� ��������, �������� ��� ��� ��� ����� �������
    {
        int health = unit.GetHealthSystem().GetHealth();
        int healthFull = unit.GetHealthSystem().GetHealthFull();

        float unitPerHealthPoint = 100 / healthFull;  //��� ���� ��������, ��� ���� ����� ���� ����
        return (healthFull - health) * unitPerHealthPoint + unitPerHealthPoint; // ���� ���� � ���� ����� ������������ �������� (health=healthFull), �� ������ ����� ������� ���� � ������� ������������ ��������� (������ ������ �������� � ������ �������).
                                                                               // ���� ����� ����������� � ���� ������ ���������� �������� �������� 20 �� � ������� healthFull=100 � � ������� 120 �� �������� ������� �.�. � ���� ������ ������������ �������� � �� ������� ������ ����� ��������
    }

    public void SetSpotterFireUnit(Unit spotterFireUnit) // ���������� ��������������� ����
    {
        _spotterFireUnit = spotterFireUnit;
        _haveSpotterFire = true;

    }

    public void �learSpotterFireUnit()// �������� ���� ��������������� ����
    {
        _spotterFireUnit = null;
        _haveSpotterFire = false;
    }

    public int GetTargetCountAtPosition(GridPositionXZ gridPosition) // �������� ���������� ����� �� �������
    {
        return GetValidActionGridPositionList(gridPosition).Count; // ������� ���������� ����� �� ������ ���������� �����
    }

    public Unit GetTargetUnit() // �������� _targetUnit
    {
        return _targetUnit;
    }
    public override int GetMaxActionDistance() // �������� maxShootDistance
    {
        if (_haveSpotterFire)
        {
            return _maxShootDistance + Mathf.RoundToInt(_maxShootDistance * _percentageShootDistanceIncrease);
        }
        else
        {
            return _maxShootDistance;
        }

    }
    private int GetShootDamage()
    {
        if (_haveSpotterFire)
        {
            return _shootDamage + Mathf.RoundToInt(_shootDamage * _percentageShootDamageIncrease);
        }
        else
        {
            return _shootDamage;
        }
    }

    public override string GetToolTip()
    {
        return "���� - " + GetActionPointCost() + " (�� 3 �������� ��������)" + "\n" +
            "��������� - " + GetMaxActionDistance() + "\n" +
            "���� - " + GetShootDamage() + "\n" +
            "����������� � ��� ���� ����� ��������" + "\n" +
           "�������� ����������� ��������� ��  50%";
    }
}
