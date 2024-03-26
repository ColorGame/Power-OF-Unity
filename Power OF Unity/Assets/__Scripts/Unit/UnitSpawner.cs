using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
/// <summary>
/// Создает унитов на локации в PointSpawn.
/// </summary>
public class UnitSpawner : MonoBehaviour
{   
    
    [SerializeField] private Transform[] _enemyPointSpawnerArray;
    [Header("Размер массива должен быть = 12 \n(Максимальное количество юнитов на миссии)")]
    [Tooltip("Точки возраждения ДРУЖЕСТВЕННЫХ юнитов")]
    [SerializeField] private Transform[] _friendPointSpawnerArray;

    private UnitManager _unitManager;

    /// <summary>
    /// Автоматическая комплектация таблицы через кнопку в ИНСПЕКТОРЕ
    /// </summary>
    public void AutomaticCompleteTable()
    {
        if (_enemyPointSpawnerArray.Length==0)
        {
            Transform containerEnemyPointSpawner = transform.Find("ContainerEnemyPointSpawner");
            _enemyPointSpawnerArray = GetArrayСhildTransform(containerEnemyPointSpawner, _enemyPointSpawnerArray);
        }
        else { Debug.Log("ТАБЛИЦА -- Enemy Point Spawner Array -- уже заполнена!!!"); }
        
        if (_friendPointSpawnerArray.Length == 0)
        {
            Transform containerFriendPointSpawner = transform.Find("ContainerFriendPointSpawner");
            _friendPointSpawnerArray = GetArrayСhildTransform(containerFriendPointSpawner, _friendPointSpawnerArray);
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


    public void Initialize(UnitManager unitManager)
    {
        _unitManager = unitManager;

        // получить список юнитов у UnitManager и создать в каждой точке спавна       
        List<Unit> myUnitOnMissionList = _unitManager.GetMyUnitOnMissionList();

        for (int index = 0; index < myUnitOnMissionList.Count; index++)
        {
            Unit myUnit = myUnitOnMissionList[index];
            Transform pointSpawner = _friendPointSpawnerArray[index];

            // Настроим юнита и передадим позицию точки спавна
            myUnit.SetupTransformAndGridPosition(pointSpawner);
        }

        /* for (int index = 0; index < _friendPointSpawnerArray.Length; index++)
         {
             if (index< myUnitOnMissionList.Count) // Проверим индекс. (Точек спавна может быть больше чем ЮНИТОВ. )
             {
                 Unit myUnit = myUnitOnMissionList[index];
                 Transform pointSpawner = _friendPointSpawnerArray[index];

                 // Настроим юнита и передадим позицию точки спавна
                 myUnit.SetupTransformAndGridPosition(pointSpawner);
             } 
         }*/

        // Создать врагов из GameAssets
        foreach (Transform pointSpawner in _enemyPointSpawnerArray)
        {
            EnemyUnitType enemyUnitType = pointSpawner.GetComponent<EnemyPointSpawner>().GetEnemyUnitType(); // Получим тип вражеского юнита для этого спавна

            Unit enemyUnit = Instantiate(GameAssets.Instance.GetEnemyPrefab(enemyUnitType)).GetComponent<Unit>(); // Создадим из префаба и найдем компонент Unit. Позицию настроим в след строке
            enemyUnit.SetupTransformAndGridPosition(pointSpawner);

            // можно спавн сделать дочерним к enemyUnit и сохранять его последнее положение, или удалять после спавна юнита
        }

    }
}
