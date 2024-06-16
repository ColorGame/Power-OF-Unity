using UnityEngine;
/// <summary>
/// Точка спавна ВРАГА
/// </summary>
public class EnemyPointSpawner : MonoBehaviour
{
    [Header("Закинем UnitEnemySO вражеского юнита \nкоторый будет здесь спавниться")]
    [SerializeField] private UnitEnemySO _unitEnemySO;
    
   
    public UnitEnemySO GetUnitEnemySO() {  return _unitEnemySO; }
}
