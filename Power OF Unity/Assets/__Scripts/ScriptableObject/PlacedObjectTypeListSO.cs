using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlacedObjectTypeListSO", menuName = "ScriptableObjects/PlacedObjectTypeList")]
public class PlacedObjectTypeListSO : ScriptableObject
{ // БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)

    public List<PlacedObjectTypeSO> list; // Список Типов Размещяемых объектов


    
}

