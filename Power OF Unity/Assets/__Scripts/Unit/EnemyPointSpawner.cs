using UnityEngine;
/// <summary>
/// ����� ������ �����
/// </summary>
public class EnemyPointSpawner : MonoBehaviour
{
    [Header("������� UnitEnemySO ���������� ����� \n������� ����� ����� ����������")]
    [SerializeField] private UnitEnemySO _unitEnemySO;
    
   
    public UnitEnemySO GetUnitEnemySO() {  return _unitEnemySO; }
}
