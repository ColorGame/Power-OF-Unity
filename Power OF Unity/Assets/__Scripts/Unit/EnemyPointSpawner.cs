using UnityEngine;

public class EnemyPointSpawner : MonoBehaviour
{
    [Header("Установим тип вражеского юнита \nкоторый будет здесь спавниться")]
    [SerializeField] private EnemyUnitType _enemyType;

    public EnemyUnitType GetEnemyUnitType() { return _enemyType; }
}
