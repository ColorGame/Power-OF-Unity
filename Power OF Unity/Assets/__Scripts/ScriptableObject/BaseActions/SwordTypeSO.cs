using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SwordType")]

public class SwordTypeSO : PlacedObjectTypeWithActionSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<SwordAction>();
    }
}
