using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/VisionItemType")]

public class VisionItemTypeSO : PlacedObjectTypeWithActionSO
{
    public override BaseAction GetAction(Unit unit)
    {
        return unit.GetAction<VisionAction>();
    }
}