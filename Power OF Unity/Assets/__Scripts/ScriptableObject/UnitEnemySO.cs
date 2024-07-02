
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitEnemy", menuName = "ScriptableObjects/UnitEnemy")]

public class UnitEnemySO : UnitTypeSO
{
    [SerializeField] private Transform _unitEnemyVisualPrefab;
    [Header("SO ��������� ��������(������) ������� ������� ����\n(��� ���������� ��� ������)")]
    [SerializeField] private PlacedObjectTypeSO _mainPlacedObjectTypeSO;
    [Header("������ SO �������������� ���������(������) ������� ������� ����")]
    [SerializeField] private PlacedObjectTypeSO[] _otherPlacedObjectTypeSOArray;

    public PlacedObjectTypeSO GetMainPlacedObjectTypeSO() { return _mainPlacedObjectTypeSO; }
    public PlacedObjectTypeSO[] GetOtherPlacedObjectTypeSOArray() {  return _otherPlacedObjectTypeSOArray; }
    public Transform GetUnitEnemyVisualPrefab() { return _unitEnemyVisualPrefab; }

}
