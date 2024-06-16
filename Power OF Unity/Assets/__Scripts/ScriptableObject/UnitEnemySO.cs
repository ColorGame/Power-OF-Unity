
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitEnemy", menuName = "ScriptableObjects/UnitEnemy")]

public class UnitEnemySO : UnitTypeSO
{
    [SerializeField] private Transform _unitEnemyVisualPrefab;
    [Header("SO ��������� ��������(������) ������� ������� ����\n(��� ���������� ��� ������)")]
    [SerializeField] private PlacedObjectTypeSO _mainPlacedObjectTypeSO;
    [Header("������ SO �������������� ���������(������) ������� ������� ����\n(��� ��������� ������ ������� �������� ��� ������)")]
    [SerializeField] private PlacedObjectTypeSO[] _otherPlacedObjectTypeSOArray;

    private List<BaseAction> _baseActionList;


    public PlacedObjectTypeSO GetMainPlacedObjectTypeSO() { return _mainPlacedObjectTypeSO; }
    public PlacedObjectTypeSO[] GetOtherPlacedObjectTypeSOArray() {  return _otherPlacedObjectTypeSOArray; }
    public Transform GetUnitEnemyVisualPrefab() { return _unitEnemyVisualPrefab; }

    public List<BaseAction> GetBaseActionsList()
    {
        if (_baseActionList.Count == 0) // ���� ��� �� ��������� �� 
        {
            //��� ��������� ��������
            foreach (BaseAction baseAction in _mainPlacedObjectTypeSO.GetBaseActionsArray())
            {
                _baseActionList.Add(baseAction);
            }
            //��� �������������� ���������
            foreach (PlacedObjectTypeSO otherPlacedObjectTypeSO in _otherPlacedObjectTypeSOArray)
            {
                foreach (BaseAction baseAction in otherPlacedObjectTypeSO.GetBaseActionsArray())
                {
                    _baseActionList.Add(baseAction);
                }
            }
            return _baseActionList;
        }
        return _baseActionList;
    }

}
