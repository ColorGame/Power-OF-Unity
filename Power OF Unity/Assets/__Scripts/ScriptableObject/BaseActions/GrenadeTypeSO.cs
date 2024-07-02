using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrenadeType")]

public class GrenadeTypeSO : PlacedObjectTypeSO
{
   

    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<GrenadeAction>();
    }
}
