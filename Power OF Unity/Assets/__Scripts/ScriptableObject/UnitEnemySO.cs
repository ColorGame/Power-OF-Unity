
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitEnemy", menuName = "ScriptableObjects/UnitEnemy")]

public class UnitEnemySO : UnitTypeSO
{
    [SerializeField] private UnitView _unitEnemyViewPrefab;
    [Header("SO ��������� ��������(������) ������� ������� ����\n(��� ���������� ��� ������)")]
    [SerializeField] private PlacedObjectTypeWithActionSO _mainplacedObjectTypeWithActionSO;
    [Header("������ SO �������������� ���������(������) ������� ������� ����")]
    [SerializeField] private PlacedObjectTypeWithActionSO[] _otherplacedObjectTypeWithActionSOArray;

    public PlacedObjectTypeWithActionSO GetMainplacedObjectTypeWithActionSO() { return _mainplacedObjectTypeWithActionSO; }
    public PlacedObjectTypeWithActionSO[] GetOtherplacedObjectTypeWithActionSOArray() {  return _otherplacedObjectTypeWithActionSOArray; }
    public UnitView GetUnitEnemyVisualPrefab() { return _unitEnemyViewPrefab; }

}
