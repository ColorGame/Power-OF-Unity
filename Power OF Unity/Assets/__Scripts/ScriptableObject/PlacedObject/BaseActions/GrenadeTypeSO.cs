using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrenadeType")]

public class GrenadeTypeSO : PlacedObjectTypeWithActionSO
{
   

    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<GrenadeAction>();
    }
}
