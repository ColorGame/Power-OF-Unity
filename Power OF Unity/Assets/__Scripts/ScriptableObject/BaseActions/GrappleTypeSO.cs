using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrappleType")]

public class GrappleTypeSO : PlacedObjectTypeWithActionSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<GrappleAction>();
    }
}
