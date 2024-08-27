using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitTypeBasicListSO", menuName = "ScriptableObjects/UnitTypeBasicList")]
public class UnitTypeBasicListSO : ScriptableObject// ����� ����� ���� ��������� (���� ������)
{
    [SerializeField] private List<UnitTypeSO> myUnitsBasiclist; // ������� ������ ���� ������
    [SerializeField] private List<UnitTypeSO> hireUnitsBasiclist; // ������� c����� ������ ��� �����

    public List<UnitTypeSO> GetMyUnitsBasicList() {  return myUnitsBasiclist; }
    public List<UnitTypeSO> GetHireUnitsBasiclist() {  return myUnitsBasiclist; }
}
