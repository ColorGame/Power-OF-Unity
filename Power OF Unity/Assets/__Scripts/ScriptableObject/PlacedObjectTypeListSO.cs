using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlacedObjectTypeListSO", menuName = "ScriptableObjects/PlacedObjectTypeList")]
public class PlacedObjectTypeListSO : ScriptableObject
{ // БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)

    [SerializeField]private List<PlacedObjectTypeWithActionSO> _placedObjectWithActionList; // Список Типов Размещяемых объектов
    

    public List<PlacedObjectTypeWithActionSO> GetPlacedObjectWithActionList() { return _placedObjectWithActionList; }
}

