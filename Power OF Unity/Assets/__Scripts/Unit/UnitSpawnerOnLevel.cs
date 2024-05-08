using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
/// <summary>
/// Создает унитов на УРОВНЕ в PointSpawn.
/// </summary>
public class UnitSpawnerOnLevel : MonoBehaviour
{
    [SerializeField] private Transform _containerEnemyPointSpawner;
    [SerializeField] private Transform _containerFriendPointSpawner;

    [SerializeField] private Transform[] _enemyPointSpawnerArray;
    [Header("Размер массива должен быть = 12 \n(Максимальное количество юнитов на миссии)")]
    [Tooltip("Точки возраждения ДРУЖЕСТВЕННЫХ юнитов")]
    [SerializeField] private Transform[] _friendPointSpawnerArray;

    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private LevelGrid _levelGrid;

    /// <summary>
    /// Автоматическая комплектация таблицы через кнопку в ИНСПЕКТОРЕ
    /// </summary>
    public void AutomaticCompleteTable()
    {
        if (_enemyPointSpawnerArray.Length == 0)
        {
            _enemyPointSpawnerArray = GetArrayСhildTransform(_containerEnemyPointSpawner, _enemyPointSpawnerArray);
        }
        else { Debug.Log("ТАБЛИЦА -- Enemy Point Spawner Array -- уже заполнена!!!"); }

        if (_friendPointSpawnerArray.Length == 0)
        {
            _friendPointSpawnerArray = GetArrayСhildTransform(_containerFriendPointSpawner, _friendPointSpawnerArray);
        }
        else { Debug.Log("ТАБЛИЦА -- Friend Point Spawner Array -- уже заполнена!!!"); }
    }
    /// <summary>
    /// Получить массив дочерних Transform
    /// </summary>    
    private Transform[] GetArrayСhildTransform(Transform containerPointSpawner, Transform[] transformArray)
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

        // получить список юнитов у UnitManager и создать в каждой точке спавна       
        List<Unit> myUnitOnMissionList = _unitManager.GetMyUnitOnMissionList();

        if (myUnitOnMissionList.Count <= _friendPointSpawnerArray.Length) // Проверим что бы количество юнитов на миссии не превышало количество точек спавна
        {
            for (int index = 0; index < myUnitOnMissionList.Count; index++)
            {
                Unit myUnit = myUnitOnMissionList[index];
                Transform pointSpawner = _friendPointSpawnerArray[index];

                // Настроим юнита и передадим позицию точки спавна
                myUnit.SetupUnitForSpawn(pointSpawner, _turnSystem, _levelGrid);
                myUnit.GetAction<MoveAction>().SetupUnitForSpawn();
            }
        }
        else { Debug.Log("НЕ ХВАТАЕТ точек спавна для моих юнотов!!!"); }


        /* for (int index = 0; index < _friendPointSpawnerArray.Length; index++)
         {
             if (index< myUnitOnMissionList.Count) // Проверим индекс. (Точек спавна может быть больше чем ЮНИТОВ. )
             {
                 Unit myUnit = myUnitOnMissionList[index];
                 Transform pointSpawner = _friendPointSpawnerArray[index];

                 // Настроим юнита и передадим позицию точки спавна
                 myUnit.SetupUnitForSpawn(pointSpawner);
             } 
         }*/

        // Создать врагов из GameAssets
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            EnemyUnitType enemyUnitType = pointSpawner.GetComponent<EnemyPointSpawner>().GetEnemyUnitType(); // Получим тип вражеского юнита для этого спавна

            Transform enemyPrefab = Instantiate(GameAssets.Instance.GetEnemyPrefab(enemyUnitType)); // Создадим из префаба
            Unit enemyUnit = enemyPrefab.GetComponent<Unit>();//найдем на созданном префабе компонент Unit. Позицию настроим в след строке
            enemyUnit.SetupUnitForSpawn(pointSpawner, _turnSystem, _levelGrid);
            enemyUnit.GetAction<MoveAction>().SetupUnitForSpawn();
            // можно спавн сделать дочерним к enemyUnit и сохранять его последнее положение, или удалять после спавна юнита
        }

    }
}
