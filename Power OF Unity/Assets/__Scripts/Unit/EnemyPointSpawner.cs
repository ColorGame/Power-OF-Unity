using UnityEngine;

public class EnemyPointSpawner : MonoBehaviour
{
    [Header("��������� ��� ���������� ����� \n������� ����� ����� ����������")]
    [SerializeField] private EnemyUnitType _enemyType;

    public EnemyUnitType GetEnemyUnitType() { return _enemyType; }
}
