using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpotterFireItemType")]

public class SpotterFireItemTypeSO : PlacedObjectTypeSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<SpotterFireAction>();
    }
}
