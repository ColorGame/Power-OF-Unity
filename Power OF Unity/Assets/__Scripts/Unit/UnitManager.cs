using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager // �������� (�������������) ������
{ 
    public UnitManager(TooltipUI tooltipUI, SoundManager soundManager) 
    {
        Init(tooltipUI, soundManager);
    }


    public event EventHandler OnAnyUnitDeadAndRemoveList; // ������� ����� ���� ���� � ������ �� ������
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // ������� ����� ��������� ���� ������ � �������� � ������      

    private List<Unit> _unitFriendList = new List<Unit>();// ������  ���� ������
    private List<Unit> _unitFriendOnMissionList = new List<Unit>();// ������ ���� ������ �� ������
    private List<Unit> _unitFriendDeadList = new List<Unit>();// ������ ���� �������� ������ 
    private List<Unit> _unitEnemyList = new List<Unit>();  // ������ ��������� ������
       
    private TooltipUI _tooltipUI;
    private SoundManager _soundManager;
   

    private void Init(TooltipUI tooltipUI, SoundManager soundManager)
    {
        _tooltipUI = tooltipUI;
        _soundManager = soundManager;        
                
        InitUnits(firstStart: true);

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // ���������� �� ������� (����� ���������(���������) ��������� ����)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // ���������� �� ������� (����� ������� ����)
    }

    public void InitUnits(bool firstStart)
    {
        if (firstStart)
        {
            UnitTypeBasicListSO unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);

            foreach (UnitTypeSO unitFriendTypeSO in unitTypeBasicListSO.list)
            {
                Unit unit = new Unit(unitFriendTypeSO, _soundManager);
                _unitFriendList.Add(unit);
            }
        }
        else
        {
            // ����������� ��������
        }
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
            _tooltipUI.ShowShortTooltipFollowMouse("��� 12 ���� ������", new TooltipUI.TooltipTimer { timer = 2f }); // ������� ��������� � ������� ����� ������ ����������� ���������
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