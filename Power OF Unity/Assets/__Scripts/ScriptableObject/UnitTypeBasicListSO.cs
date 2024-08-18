using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeBasicListSO", menuName = "ScriptableObjects/UnitTypeBasicList")]

public class UnitTypeBasicListSO : ScriptableObject// БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)
{
    public List<UnitTypeSO> myUnitsBasiclist; // Базовый список моих Юнитов
    public List<UnitTypeSO> hireUnitslist; // Cписок Юнитов для найма
}
