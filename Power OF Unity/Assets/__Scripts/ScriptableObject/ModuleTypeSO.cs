using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ModuleType")]

public class ModuleTypeSO : PlacedObjectTypeSO
{
   

    public override string GetToolTip()
    {
        return ""+ name;
    }
}
