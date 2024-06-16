using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager // �������� (�������������) ������
{ 
    public UnitManager(TooltipUI tooltipUI) 
    {
        Init(tooltipUI);
    }


    public event EventHandler OnAnyUnitDeadAndRemoveList; // ������� ����� ���� ���� � ������ �� ������
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // ������� ����� ��������� ���� ������ � �������� � ������      

    private List<Unit> _unitFriendList;// ������  ���� ������
    private List<Unit> _unitFriendOnMissionList;// ������ ���� ������ �� ������
    private List<Unit> _unitFriendDeadList;// ������ ���� �������� ������ 
    private List<Unit> _unitEnemyList;  // ������ ��������� ������

    private UnitTypeBasicListSO _unitTypeBasicListSO; // ������ ��� �������� ������� ������
    private TooltipUI _tooltipUI;

    private void Init(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        // �������� ������������� �������        
        _unitFriendList = new List<Unit>();
        _unitFriendOnMissionList = new List<Unit>();
        _unitFriendDeadList = new List<Unit>();
        _unitEnemyList = new List<Unit>();

        _unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);    // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                         // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� UnitTypeBasicListSO � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // ���������� �� ������� (����� ���������(���������) ��������� ����)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // ���������� �� ������� (����� ������� ����)
    }


    private void Unit_OnAnyEnemyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        _unitEnemyList.Add(unit); // ������� ��� � ������ ��������� ������
        OnAnyEnemyUnitSpawnedAndAddList?.Invoke(this, EventArgs.Empty);
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        //Debug.Log(unit + " dead"); // ��� �����

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _unitEnemyList.Remove(unit); // ������ ��� �� ������ ��������� ������           
        }
        else// ���� ���
        {
            _unitFriendList.Remove(unit); // ������ ��� �� ������ ���� ������
            _unitFriendDeadList.Add(unit); // ������� � ��������
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // �������� ��������
    }

    public void AddUnitFriendList(Unit unit)
    {
        _unitFriendList.Add(unit);
    }
    public void RemoveUnitFriendList(Unit unit)
    {
        _unitFriendList.Remove(unit);
    }

    public void AddUnitFriendOnMissionList(Unit unit)
    {
        if (_unitFriendOnMissionList.Count <= Constant.COUNT_UNIT_ON_MISSION_MAX) // �������� ���� ���������� ������ � ������ ������ ������������� �� ������� � ������ ����������� �����
        {
            _unitFriendOnMissionList.Add(unit);
        }
        else
        {
            _tooltipUI.ShowTooltipsFollowMouse("��� 12 ���� ������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������
        }
    }
    public void RemoveUnitFriendOnMissionList(Unit unit)
    {
        _unitFriendOnMissionList.Remove(unit);
    }

    public List<Unit> GetUnitFriendList()
    {
        return _unitFriendList;
    }

    public List<Unit> GetUnitFriendOnMissionList()
    {
        return _unitFriendOnMissionList;
    }

    public List<Unit> GetUnitEnemyList()
    {
        return _unitEnemyList;
    }

    public List<Unit> GetUnitFriendDeadList()
    {
        return _unitFriendDeadList;
    }

}