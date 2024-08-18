using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeBasicListSO", menuName = "ScriptableObjects/UnitTypeBasicList")]

public class UnitTypeBasicListSO : ScriptableObject// ����� ����� ���� ��������� (���� ������)
{
    public List<UnitTypeSO> myUnitsBasiclist; // ������� ������ ���� ������
    public List<UnitTypeSO> hireUnitslist; // C����� ������ ��� �����
}
