using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitTypeBasicListSO", menuName = "ScriptableObjects/UnitTypeBasicList")]
public class UnitTypeBasicListSO : ScriptableObject// БУДЕТ ВСЕГО ОДИН ЭКЗЕМПЛЯР (один список)
{
    [SerializeField] private List<UnitTypeSO> myUnitsBasiclist; // Базовый список моих Юнитов
    [SerializeField] private List<UnitTypeSO> hireUnitsBasiclist; // Базовый cписок Юнитов для найма

    public List<UnitTypeSO> GetMyUnitsBasicList() {  return myUnitsBasiclist; }
    public List<UnitTypeSO> GetHireUnitsBasiclist() {  return myUnitsBasiclist; }
}
