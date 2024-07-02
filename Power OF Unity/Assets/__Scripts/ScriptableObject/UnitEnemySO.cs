
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitEnemy", menuName = "ScriptableObjects/UnitEnemy")]

public class UnitEnemySO : UnitTypeSO
{
    [SerializeField] private Transform _unitEnemyVisualPrefab;
    [Header("SO основного предмета(оружи€) которым владеет враг\n(дл€ экипировки при спавне)")]
    [SerializeField] private PlacedObjectTypeSO _mainPlacedObjectTypeSO;
    [Header("ћассив SO дополнительных предметов(оружи€) которым владеет враг")]
    [SerializeField] private PlacedObjectTypeSO[] _otherPlacedObjectTypeSOArray;

    public PlacedObjectTypeSO GetMainPlacedObjectTypeSO() { return _mainPlacedObjectTypeSO; }
    public PlacedObjectTypeSO[] GetOtherPlacedObjectTypeSOArray() {  return _otherPlacedObjectTypeSOArray; }
    public Transform GetUnitEnemyVisualPrefab() { return _unitEnemyVisualPrefab; }

}
