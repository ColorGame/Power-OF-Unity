using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SwordType")]

public class SwordTypeSO : PlacedObjectTypeSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<SwordAction>();
    }
}
