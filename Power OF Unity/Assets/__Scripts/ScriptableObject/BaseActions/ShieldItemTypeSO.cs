using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShieldItemType")]

public class ShieldItemTypeSO : PlacedObjectTypeSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<ShieldAction>();
    }
}
