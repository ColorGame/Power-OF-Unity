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
    private PathfindingProvider _pathfindingProvider;

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


    public void Init(UnitManager unitManager, TurnSystem turnSystem, SoundManager soundManager, LevelGrid levelGrid, UnitActionSystem unitActionSystem, CameraFollow cameraFollow, HashAnimationName hashAnimationName, PathfindingProvider pathfindingProvider)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _soundManager = soundManager;       
        _levelGrid = levelGrid;
        _unitActionSystem = unitActionSystem;
        _cameraFollow = cameraFollow;
        _pathfindingProvider = pathfindingProvider;

        UnitSpawn();
      //  UnitEnemySpawn(hashAnimationName);
    }

    private void UnitSpawn()
    {
        // �������� ������ ������ � UnitManager � ������� � ������ ����� ������       
        List<Unit> UnitOnMissionList = _unitManager.GetUnitOnMissionList();

        if (UnitOnMissionList.Count <= _friendPointSpawnerArray.Length) // �������� ��� �� ���������� ������ �� ������ �� ��������� ���������� ����� ������
        {
            for (int index = 0; index < UnitOnMissionList.Count; index++)
            {
                Transform pointSpawner = _friendPointSpawnerArray[index];
                Unit unitFriend = UnitOnMissionList[index];                

                SetupAndInstantiateUnit(pointSpawner, unitFriend);
            }
        }
        else { Debug.Log("�� ������� ����� ������ ��� ���� ������!!!"); }
    }
    private void UnitEnemySpawn(HashAnimationName hashAnimationName)
    {
        // ������� ��� � �������� ����� ��� ������ ����� ������ 
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            UnitEnemySO unitEnemySO = pointSpawner.GetComponent<EnemyPointSpawner>().GetUnitEnemySO(); // ������� UnitEnemySO ���������� ����� ��� ����� ������

            Unit unitEnemy = new Unit(unitEnemySO, _soundManager, hashAnimationName); ;  // �������������� ������ �����         

            SetupAndInstantiateUnit(pointSpawner, unitEnemy);
            // ����� ����� ������� �������� � unitEnemy � ��������� ��� ��������� ���������, ��� ������� ����� ������ �����
        }
    }

    private void SetupAndInstantiateUnit(Transform pointSpawner,  Unit unit)
    {
        Transform unitCoreTransform = unit.GetUnitEquipsViewFarm().CreateCoreAndView(pointSpawner);      

        unit.SetupForSpawn(_levelGrid, _turnSystem, unitCoreTransform, _cameraFollow, _unitActionSystem, _pathfindingProvider);

    }
}
