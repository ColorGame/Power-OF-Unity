using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeBasicListSO", menuName = "ScriptableObjects/UnitTypeBasicList")]

public class UnitTypeBasicListSO : ScriptableObject// БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)
{
    public List<UnitTypeSO> list; // Базовый список типов Юнитов
}
