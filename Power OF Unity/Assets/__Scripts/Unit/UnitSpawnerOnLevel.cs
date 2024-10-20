using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� � ����������� ������ �� ������ � PointSpawn.
/// </summary>
public class UnitSpawnerOnLevel : MonoBehaviour
{
    [SerializeField] private Transform _containerEnemyPointSpawner;
    [SerializeField] private Transform _containerFriendPointSpawner;

    [SerializeField] private Transform[] _enemyPointSpawnerArray;
    [Header("������ ������� ������ ���� = 12 \n(������������ ���������� ������ �� ������)")]
    [Tooltip("����� ����������� ������������� ������")]
    [SerializeField] private Transform[] _friendPointSpawnerArray = new Transform[Constant.COUNT_UNIT_ON_MISSION_MAX]; // ��������� ���������� ����� ������

    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private SoundManager _soundManager;   
    private LevelGrid _levelGrid;
    private UnitActionSystem _unitActionSystem;
    private CameraFollow _cameraFollow;

    /// <summary>
    /// �������������� ������������ ������� ����� ������ � ����������
    /// </summary>
    public void AutomaticCompleteTable()
    {
        if (_enemyPointSpawnerArray.Length == 0)
        {
            _enemyPointSpawnerArray = GetArray�hildTransform(_containerEnemyPointSpawner, _enemyPointSpawnerArray);
        }
        else { Debug.Log("������� -- Enemy Point Spawner Array -- ��� ���������!!!"); }

        if (_friendPointSpawnerArray.Length == 0 || _friendPointSpawnerArray[0]==null)
        {
            _friendPointSpawnerArray = GetArray�hildTransform(_containerFriendPointSpawner, _friendPointSpawnerArray);
        }
        else { Debug.Log("������� -- Friend Point Spawner Array -- ��� ���������!!!"); }
    }
    /// <summary>
    /// �������� ������ �������� Transform
    /// </summary>    
    private Transform[] GetArray�hildTransform(Transform containerPointSpawner, Transform[] transformArray)
    {
        transformArray = new Transform[containerPointSpawner.childCount];
        for (int index = 0; index < containerPointSpawner.childCount; index++)
        {
            transformArray[index] = containerPointSpawner.GetChild(index);
        }
        return transformArray;
    }


    public void Init(UnitManager unitManager, TurnSystem turnSystem, SoundManager soundManager, LevelGrid levelGrid, UnitActionSystem unitActionSystem, CameraFollow cameraFollow)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _soundManager = soundManager;       
        _levelGrid = levelGrid;
        _unitActionSystem = unitActionSystem;
        _cameraFollow = cameraFollow;

        UnitFriendSpawn();
        UnitEnemySpawn();
    }

    private void UnitFriendSpawn()
    {
        // �������� ������ ������ � UnitManager � ������� � ������ ����� ������       
        List<Unit> UnitFriendOnMissionList = _unitManager.GetUnitFriendOnMissionList();

        if (UnitFriendOnMissionList.Count <= _friendPointSpawnerArray.Length) // �������� ��� �� ���������� ������ �� ������ �� ��������� ���������� ����� ������
        {
            for (int index = 0; index < UnitFriendOnMissionList.Count; index++)
            {
                Transform pointSpawner = _friendPointSpawnerArray[index];
                Unit unitFriend = UnitFriendOnMissionList[index];
                UnitFriendSO unitFriendSO = unitFriend.GetUnitTypeSO<UnitFriendSO>();

                SetupAndInstantiateUnit(pointSpawner, unitFriendSO, unitFriend);
            }
        }
        else { Debug.Log("�� ������� ����� ������ ��� ���� ������!!!"); }
    }
    private void UnitEnemySpawn()
    {
        // ������� ��� � �������� ����� ��� ������ ����� ������ 
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            UnitEnemySO unitEnemySO = pointSpawner.GetComponent<EnemyPointSpawner>().GetUnitEnemySO(); // ������� UnitEnemySO ���������� ����� ��� ����� ������
                        
            Unit unitEnemy = new Unit(unitEnemySO, _soundManager); ;  // �������������� ������ �����         

            SetupAndInstantiateUnit(pointSpawner, unitEnemySO, unitEnemy);
            // ����� ����� ������� �������� � unitEnemy � ��������� ��� ��������� ���������, ��� ������� ����� ������ �����
        }
    }

    private void SetupAndInstantiateUnit(Transform pointSpawner, UnitTypeSO unitTypeSO, Unit unit)
    {        
        Transform unitCorePrefab = unitTypeSO.GetUnitCorePrefab();

        UnitView unitViewPrefab = null;
        switch (unitTypeSO)
        {
            case UnitFriendSO unitFriendSO:
                unitViewPrefab = unitFriendSO.GetUnitViewPrefab(unit.GetUnitEquipment().GetBodyArmor());
                break;
            case UnitEnemySO unitEnemySO:
                unitViewPrefab = unitEnemySO.GetUnitEnemyVisualPrefab();
                break;
        }

        Transform unitCoreTransform = Instantiate(unitCorePrefab, pointSpawner); // �������� ���� ����� � ����� ������        
        Instantiate(unitViewPrefab, unitCoreTransform); // �������� ������ � ���� �����    

        unit.SetupForSpawn( _levelGrid, _turnSystem, unitCoreTransform, _cameraFollow, _unitActionSystem);
    }
}
