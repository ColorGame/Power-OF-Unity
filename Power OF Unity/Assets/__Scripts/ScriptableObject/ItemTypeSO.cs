using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemType")]

public class ItemTypeSO : PlacedObjectTypeSO
{
    
    public int damage; // �������� �����


    public override string GetToolTip()
    {
        return""+ name;
    }
}
