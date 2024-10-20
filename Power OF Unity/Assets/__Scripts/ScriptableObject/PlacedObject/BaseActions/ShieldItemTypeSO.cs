using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShieldItemType")]

public class ShieldItemTypeSO : PlacedObjectTypeWithActionSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<ShieldAction>();
    }
}
