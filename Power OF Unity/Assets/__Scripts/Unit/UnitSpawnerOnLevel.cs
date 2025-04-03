using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Создает и НАСТРАИВАЕТ унитов на УРОВНЕ в PointSpawn.
/// </summary>
public class UnitSpawnerOnLevel : MonoBehaviour
{
    [SerializeField] private Transform _containerEnemyPointSpawner;
    [SerializeField] private Transform _containerFriendPointSpawner;

    [SerializeField] private Transform[] _enemyPointSpawnerArray;
    [Header("Размер массива должен быть = 12 \n(Максимальное количество юнитов на миссии)")]
    [Tooltip("Точки возраждения ДРУЖЕСТВЕННЫХ юнитов")]
    [SerializeField] private Transform[] _friendPointSpawnerArray = new Transform[Constant.COUNT_UNIT_ON_MISSION_MAX]; // Ограничим количество точек спавна

    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private SoundManager _soundManager;   
    private LevelGrid _levelGrid;
    private UnitActionSystem _unitActionSystem;
    private CameraFollow _cameraFollow;
    private PathfindingProvider _pathfindingProvider;

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

        if (_friendPointSpawnerArray.Length == 0 || _friendPointSpawnerArray[0]==null)
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
        // получить список юнитов у UnitManager и создать в каждой точке спавна       
        List<Unit> UnitOnMissionList = _unitManager.GetUnitOnMissionList();

        if (UnitOnMissionList.Count <= _friendPointSpawnerArray.Length) // Проверим что бы количество юнитов на миссии не превышало количество точек спавна
        {
            for (int index = 0; index < UnitOnMissionList.Count; index++)
            {
                Transform pointSpawner = _friendPointSpawnerArray[index];
                Unit unitFriend = UnitOnMissionList[index];                

                SetupAndInstantiateUnit(pointSpawner, unitFriend);
            }
        }
        else { Debug.Log("НЕ ХВАТАЕТ точек спавна для моих юнотов!!!"); }
    }
    private void UnitEnemySpawn(HashAnimationName hashAnimationName)
    {
        // получим тип и создадим юнита для каждой точки спавна 
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            UnitEnemySO unitEnemySO = pointSpawner.GetComponent<EnemyPointSpawner>().GetUnitEnemySO(); // Получим UnitEnemySO вражеского юнита для этого спавна

            Unit unitEnemy = new Unit(unitEnemySO, _soundManager, hashAnimationName); ;  // инициализируем нового юнита         

            SetupAndInstantiateUnit(pointSpawner, unitEnemy);
            // можно спавн сделать дочерним к unitEnemy и сохранять его последнее положение, или удалять после спавна юнита
        }
    }

    private void SetupAndInstantiateUnit(Transform pointSpawner,  Unit unit)
    {
        Transform unitCoreTransform = unit.GetUnitEquipsViewFarm().CreateCoreAndView(pointSpawner);      

        unit.SetupForSpawn(_levelGrid, _turnSystem, unitCoreTransform, _cameraFollow, _unitActionSystem, _pathfindingProvider);

    }
}
