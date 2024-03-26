using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� //- �������� ������� ���������� ������� UnitManager � Unit , ������� � Project Settings/ Script Execution Order � �������� ���������� UnitManager ���� Default Time), ����� UnitManager ���������� ������ ��� �������� Unit
// (��� �� ������� ��������� ��������� � UnitManager � ����� ��������� ���� ������� � Unit)
// ����� ���� ������ Unit ��������� ������ �� ���� �� ���������� � ������ ������ �.�. ��� ��� �� ����������
public class UnitManager // �������� (�������������) ������
{
    private const int MAX_COUNT_UNIT_ON_MISSION = 12;// ������������ ���������� ������ �� ������

    public UnitManager() // ����������� ��� �� ��������� ���������� ��������� new T() �� ������ ���� ����
    {
    }

    public  event EventHandler OnAnyUnitDeadAndRemoveList; // ������� ����� ���� ���� � ������ �� ������
    public  event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // ������� ����� ��������� ���� ������ � �������� � ������

    private List<Unit> _myUnitList;// ������  ���� ������
    private List<Unit> _myUnitOnMissionList;// ������ ���� ������ �� ������
    private List<Unit> _enemyUnitList;  // ������ ��������� ������

    private TooltipUI _tooltipUI;  



    public void Initialize(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        // �������� ������������� �������        
        _myUnitList = new List<Unit>();
        _myUnitOnMissionList = new List<Unit>();
        _enemyUnitList = new List<Unit>();

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // ���������� �� ������� (����� ���������(���������) ����)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // ���������� �� ������� (����� ������� ����)
    }       


    private void Unit_OnAnyEnemyUnitSpawned(object sender, System.EventArgs e) // ��� �������� ������ ����������� �� �� �������
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        //Debug.Log(unit + " spawner"); // ��� �����

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _enemyUnitList.Add(unit); // ������� ��� � ������ ��������� ������
            OnAnyEnemyUnitSpawnedAndAddList?.Invoke(this, EventArgs.Empty);
        }
        else// ���� ���
        {
            _myUnitList.Add(unit); // ������� ��� � ������ ������������� ������
        }

    }
    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        //Debug.Log(unit + " dead"); // ��� �����

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _enemyUnitList.Remove(unit); // ������ ��� �� ������ ��������� ������           
        }
        else// ���� ���
        {
            _myUnitList.Remove(unit); // ������ ��� �� ������ ������������� ������
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // �������� ��������
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

    public int GetMaxCountUnitOnMission()
    {
        return MAX_COUNT_UNIT_ON_MISSION;
    }}