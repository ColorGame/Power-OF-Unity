
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitEnemy", menuName = "ScriptableObjects/UnitEnemy")]

public class UnitEnemySO : UnitTypeSO
{
    [SerializeField] private UnitView _unitEnemyViewPrefab;
    [Header("SO основного предмета(оружи€) которым владеет враг\n(дл€ экипировки при спавне)")]
    [SerializeField] private PlacedObjectTypeWithActionSO _mainplacedObjectTypeWithActionSO;
    [Header("ћассив SO дополнительных предметов(оружи€) которым владеет враг")]
    [SerializeField] private PlacedObjectTypeWithActionSO[] _otherplacedObjectTypeWithActionSOArray;

    public PlacedObjectTypeWithActionSO GetMainplacedObjectTypeWithActionSO() { return _mainplacedObjectTypeWithActionSO; }
    public PlacedObjectTypeWithActionSO[] GetOtherplacedObjectTypeWithActionSOArray() {  return _otherplacedObjectTypeWithActionSOArray; }
    public UnitView GetUnitEnemyVisualPrefab() { return _unitEnemyViewPrefab; }

}
