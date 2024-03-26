using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
/// <summary>
/// ������� ������ �� ������� � PointSpawn.
/// </summary>
public class UnitSpawner : MonoBehaviour
{   
    
    [SerializeField] private Transform[] _enemyPointSpawnerArray;
    [Header("������ ������� ������ ���� = 12 \n(������������ ���������� ������ �� ������)")]
    [Tooltip("����� ����������� ������������� ������")]
    [SerializeField] private Transform[] _friendPointSpawnerArray;

    private UnitManager _unitManager;

    /// <summary>
    /// �������������� ������������ ������� ����� ������ � ����������
    /// </summary>
    public void AutomaticCompleteTable()
    {
        if (_enemyPointSpawnerArray.Length==0)
        {
            Transform containerEnemyPointSpawner = transform.Find("ContainerEnemyPointSpawner");
            _enemyPointSpawnerArray = GetArray�hildTransform(containerEnemyPointSpawner, _enemyPointSpawnerArray);
        }
        else { Debug.Log("������� -- Enemy Point Spawner Array -- ��� ���������!!!"); }
        
        if (_friendPointSpawnerArray.Length == 0)
        {
            Transform containerFriendPointSpawner = transform.Find("ContainerFriendPointSpawner");
            _friendPointSpawnerArray = GetArray�hildTransform(containerFriendPointSpawner, _friendPointSpawnerArray);
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


    public void Initialize(UnitManager unitManager)
    {
        _unitManager = unitManager;

        // �������� ������ ������ � UnitManager � ������� � ������ ����� ������       
        List<Unit> myUnitOnMissionList = _unitManager.GetMyUnitOnMissionList();

        for (int index = 0; index < myUnitOnMissionList.Count; index++)
        {
            Unit myUnit = myUnitOnMissionList[index];
            Transform pointSpawner = _friendPointSpawnerArray[index];

            // �������� ����� � ��������� ������� ����� ������
            myUnit.SetupTransformAndGridPosition(pointSpawner);
        }

        /* for (int index = 0; index < _friendPointSpawnerArray.Length; index++)
         {
             if (index< myUnitOnMissionList.Count) // �������� ������. (����� ������ ����� ���� ������ ��� ������. )
             {
                 Unit myUnit = myUnitOnMissionList[index];
                 Transform pointSpawner = _friendPointSpawnerArray[index];

                 // �������� ����� � ��������� ������� ����� ������
                 myUnit.SetupTransformAndGridPosition(pointSpawner);
             } 
         }*/

        // ������� ������ �� GameAssets
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            EnemyUnitType enemyUnitType = pointSpawner.GetComponent<EnemyPointSpawner>().GetEnemyUnitType(); // ������� ��� ���������� ����� ��� ����� ������

            Unit enemyUnit = Instantiate(GameAssets.Instance.GetEnemyPrefab(enemyUnitType)).GetComponent<Unit>(); // �������� �� ������� � ������ ��������� Unit. ������� �������� � ���� ������
            enemyUnit.SetupTransformAndGridPosition(pointSpawner);

            // ����� ����� ������� �������� � enemyUnit � ��������� ��� ��������� ���������, ��� ������� ����� ������ �����
        }

    }
}
