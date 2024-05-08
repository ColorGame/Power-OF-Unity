using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
/// <summary>
/// ������� ������ �� ������ � PointSpawn.
/// </summary>
public class UnitSpawnerOnLevel : MonoBehaviour
{
    [SerializeField] private Transform _containerEnemyPointSpawner;
    [SerializeField] private Transform _containerFriendPointSpawner;

    [SerializeField] private Transform[] _enemyPointSpawnerArray;
    [Header("������ ������� ������ ���� = 12 \n(������������ ���������� ������ �� ������)")]
    [Tooltip("����� ����������� ������������� ������")]
    [SerializeField] private Transform[] _friendPointSpawnerArray;

    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private LevelGrid _levelGrid;

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

        if (_friendPointSpawnerArray.Length == 0)
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


    public void Initialize(UnitManager unitManager, TurnSystem turnSystem, LevelGrid levelGrid)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _levelGrid = levelGrid;

        // �������� ������ ������ � UnitManager � ������� � ������ ����� ������       
        List<Unit> myUnitOnMissionList = _unitManager.GetMyUnitOnMissionList();

        if (myUnitOnMissionList.Count <= _friendPointSpawnerArray.Length) // �������� ��� �� ���������� ������ �� ������ �� ��������� ���������� ����� ������
        {
            for (int index = 0; index < myUnitOnMissionList.Count; index++)
            {
                Unit myUnit = myUnitOnMissionList[index];
                Transform pointSpawner = _friendPointSpawnerArray[index];

                // �������� ����� � ��������� ������� ����� ������
                myUnit.SetupUnitForSpawn(pointSpawner, _turnSystem, _levelGrid);
                myUnit.GetAction<MoveAction>().SetupUnitForSpawn();
            }
        }
        else { Debug.Log("�� ������� ����� ������ ��� ���� ������!!!"); }


        /* for (int index = 0; index < _friendPointSpawnerArray.Length; index++)
         {
             if (index< myUnitOnMissionList.Count) // �������� ������. (����� ������ ����� ���� ������ ��� ������. )
             {
                 Unit myUnit = myUnitOnMissionList[index];
                 Transform pointSpawner = _friendPointSpawnerArray[index];

                 // �������� ����� � ��������� ������� ����� ������
                 myUnit.SetupUnitForSpawn(pointSpawner);
             } 
         }*/

        // ������� ������ �� GameAssets
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            EnemyUnitType enemyUnitType = pointSpawner.GetComponent<EnemyPointSpawner>().GetEnemyUnitType(); // ������� ��� ���������� ����� ��� ����� ������

            Transform enemyPrefab = Instantiate(GameAssets.Instance.GetEnemyPrefab(enemyUnitType)); // �������� �� �������
            Unit enemyUnit = enemyPrefab.GetComponent<Unit>();//������ �� ��������� ������� ��������� Unit. ������� �������� � ���� ������
            enemyUnit.SetupUnitForSpawn(pointSpawner, _turnSystem, _levelGrid);
            enemyUnit.GetAction<MoveAction>().SetupUnitForSpawn();
            // ����� ����� ������� �������� � enemyUnit � ��������� ��� ��������� ���������, ��� ������� ����� ������ �����
        }

    }
}
