using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HealItemType")]

public class HealItemTypeSO : PlacedObjectTypeWithActionSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<HealAction>();
    }
}
