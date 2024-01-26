using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemType")]

public class ItemTypeSO : PlacedObjectTypeSO
{
    
    public int damage; // Величина урона


    public override string GetToolTip()
    {
        return""+ name;
    }
}
