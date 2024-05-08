using System;
using System.Collections.Generic;
using UnityEngine;

// ��������  CoreEntryPoint ������� ���������� ������� UnitManager � Unit , ����� UnitManager ���������� ������ ��� �������� Unit
// (��� �� ������� ��������� ��������� � UnitManager � ����� ��������� ���� ������� � Unit)
// ����� ���� ������ Unit ��������� ������ �� ���� �� ���������� � ������ ������ �.�. ��� ��� �� ����������
public class UnitManager // �������� (�������������) ������
{
    private const int MAX_COUNT_UNIT_ON_MISSION = 12;// ������������ ���������� ������ �� ������

    public UnitManager() { }// ����������� ��� �� ��������� ���������� ��������� new T() �� ������ ���� ����


    public event EventHandler OnAnyUnitDeadAndRemoveList; // ������� ����� ���� ���� � ������ �� ������
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // ������� ����� ��������� ���� ������ � �������� � ������      

    private List<Unit> _myUnitList;// ������  ���� ������
    private List<Unit> _myUnitOnMissionList;// ������ ���� ������ �� ������
    private List<Unit> _myDeadUnitnList;// ������ ���� �������� ������ 
    private List<Unit> _enemyUnitList;  // ������ ��������� ������

    private UnitTypeBasicListSO _unitTypeBasicListSO; // ������ ��� �������� ������� ������
    private TooltipUI _tooltipUI;

    public void Initialize(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        // �������� ������������� �������        
        _myUnitList = new List<Unit>();
        _myUnitOnMissionList = new List<Unit>();
        _myDeadUnitnList = new List<Unit>();
        _enemyUnitList = new List<Unit>();

        _unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);    // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                         // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� UnitTypeBasicListSO � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // ���������� �� ������� (����� ���������(���������) ��������� ����)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // ���������� �� ������� (����� ������� ����)
    }


    private void Unit_OnAnyEnemyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        _enemyUnitList.Add(unit); // ������� ��� � ������ ��������� ������
        OnAnyEnemyUnitSpawnedAndAddList?.Invoke(this, EventArgs.Empty);
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        //Debug.Log(unit + " dead"); // ��� �����

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _enemyUnitList.Remove(unit); // ������ ��� �� ������ ��������� ������           
        }
        else// ���� ���
        {
            _myUnitList.Remove(unit); // ������ ��� �� ������ ���� ������
            _myDeadUnitnList.Add(unit); // ������� � ��������
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // �������� ��������
    }

    public void AddMyUnitList(Unit unit)
    {
        _myUnitList.Add(unit);
    }
    public void RemoveMyUnitList(Unit unit)
    {
        _myUnitList.Remove(unit);
    }

    public void AddMyUnitOnMissionList(Unit unit)
    {
        if (_myUnitOnMissionList.Count <= Constant.MAX_COUNT_UNIT_ON_MISSION) // �������� ���� ���������� ������ � ������ ������ ������������� �� ������� � ������ ����������� �����
        {
            _myUnitOnMissionList.Add(unit);
        }
        else
        {
            _tooltipUI.ShowTooltipsFollowMouse("��� 12 ���� ������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������
        }
    }
    public void RemoveMyUnitOnMissionList(Unit unit)
    {
        _myUnitOnMissionList.Remove(unit);
    }

    public List<Unit> GetMyUnitList()
    {
        return _myUnitList;
    }

    public List<Unit> GetMyUnitOnMissionList()
    {
        return _myUnitOnMissionList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return _enemyUnitList;
    }

    public List<Unit> GetMyDeadUnitnList()
    {
        return _myDeadUnitnList;
    }

    public int GetMaxCountUnitOnMission()
    {
        return MAX_COUNT_UNIT_ON_MISSION;
    }
}