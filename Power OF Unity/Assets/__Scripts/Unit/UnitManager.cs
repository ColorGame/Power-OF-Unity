using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager // �������� (�������������) ������
{
    public UnitManager(TooltipUI tooltipUI, SoundManager soundManager, WarehouseManager warehouseManager, HashAnimationName hashAnimationName)
    {
        Init(tooltipUI, soundManager, warehouseManager, hashAnimationName);
    }

    public event EventHandler<Unit> OnSelectedUnitChanged; // ������� ��������� ����
    public event EventHandler OnAnyUnitDeadAndRemoveList; // ������� ����� ���� ���� � ������ �� ������
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // ������� ����� ��������� ���� ������ � �������� � ������      
    public event EventHandler OnAddRemoveUnitFromLocation; // ������� ���������� �������� ����� �� �������

    private List<Unit> _unitFriendList = new List<Unit>();// ����� ������  ���� ������
    private List<Unit> _unitFriendOnMissionList = new List<Unit>();// ������ ���� ������ �� ������
    private List<Unit> _unitFriendOnBarrackList = new List<Unit>();// ������ ���� ������ � �������. �� ��������� ��� ����� ��������� � �������
    private List<Unit> _unitFriendDeadList = new List<Unit>();// ������ ���� �������� ������ 
    private List<Unit> _unitEnemyList = new List<Unit>();  // ������ ��������� ������
    private List<Unit> _hireUnitList = new List<Unit>();// ������ ������ ��� �����

    private TooltipUI _tooltipUI;
    private SoundManager _soundManager;
    private WarehouseManager _warehouseManager;

    private Unit _selectedUnit;


    private void Init(TooltipUI tooltipUI, SoundManager soundManager, WarehouseManager warehouseManager, HashAnimationName hashAnimationName)
    {
        _tooltipUI = tooltipUI;
        _soundManager = soundManager;
        _warehouseManager = warehouseManager;

        InitUnits(firstStart: true, hashAnimationName);

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // ���������� �� ������� (����� ���������(���������) ��������� ����)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // ���������� �� ������� (����� ������� ����)

    }

    private void InitUnits(bool firstStart , HashAnimationName hashAnimationName)
    {
        if (firstStart)
        {
            UnitTypeBasicListSO unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);

            foreach (UnitTypeSO unitFriendTypeSO in unitTypeBasicListSO.GetMyUnitsBasicList())
            {
                Unit unit = new Unit(unitFriendTypeSO, _soundManager, hashAnimationName);
                AddUnitFriendList(unit);
            }

            foreach (UnitTypeSO unitFriendTypeSO in unitTypeBasicListSO.GetHireUnitsBasiclist())
            {
                Unit hireUnit = new Unit(unitFriendTypeSO, _soundManager, hashAnimationName);
                AddHireUnitList(hireUnit);
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

        //Debug.Log(hireUnit + " dead"); // ��� �����

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _unitEnemyList.Remove(unit); // ������ ��� �� ������ ��������� ������           
        }
        else// ���� ���
        {
            RemoveUnitFriendList(unit);
            _unitFriendDeadList.Add(unit); // ������� � ��������
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // �������� ��������
    }

    /// <summary>
    /// �������� ������ ���������� �����
    /// </summary>
    public bool TryHireSelectedUnit()
    {
        if (_warehouseManager.TryMinusCoins(_selectedUnit.GetUnitTypeSO<UnitFriendSO>().GetPriceHiring()))
        {
            RemoveHireUnitList(_selectedUnit);
            AddUnitFriendList(_selectedUnit);
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// ������� ���������� �����
    /// </summary>
    public void DismissSelectedUnit()
    {
        // ��������� ���������� �����
        foreach (PlacedObjectGridParameters placedObjectGridParameters in _selectedUnit.GetUnitEquipment().GetEquipmentList())
        {
            _warehouseManager.PlusCountPlacedObject(placedObjectGridParameters.placedObjectTypeSO);
        }
        _selectedUnit.GetUnitEquipment().ClearEquipmentList();
        RemoveUnitFriendList(_selectedUnit);
        AddHireUnitList(_selectedUnit);
    }
    /// <summary>
    /// �������� � ������ ��� �����
    /// </summary>
    private void AddHireUnitList(Unit unit)
    {
        _hireUnitList.Add(unit);
    }
    /// <summary>
    /// ������� �� ������� ��� �����
    /// </summary>
    private void RemoveHireUnitList(Unit unit)
    {
        _hireUnitList.Remove(unit);
    }
    /// <summary>
    /// �������� ���� ��������� ����
    /// </summary>
    public void ClearSelectedUnit()
    {
        SetSelectedUnit(null);
    }

    /// <summary>
    /// �������� ����� � ����� ������ � ������� � �������
    /// </summary>
    private void AddUnitFriendList(Unit unit)
    {
        _unitFriendList.Add(unit);
        Unit.Location location = unit.GetLocation(); // ������� ������� �����
        AddUnitToLocation(unit, location);
    }

    /// <summary>
    /// ������� ����� �� ������ ������ � ������� �������
    /// </summary>
    private void RemoveUnitFriendList(Unit unit)
    {
        _unitFriendList.Remove(unit); // ������ ��� �� ������ ���� ������
        RemoveUnitFromCurrentLocation(unit);
    }

    /// <summary>
    /// �������� ������� �����
    /// </summary>
    public void ChangeLocation(Unit unit, Unit.Location newLocation)
    {
        RemoveUnitFromCurrentLocation(unit);
        unit.SetLocation(newLocation);
        AddUnitToLocation(unit, newLocation);       
    }

    /// <summary>
    /// ������� ����� �� ������� �������
    /// </summary>
    private void RemoveUnitFromCurrentLocation(Unit unit)
    {
        switch (unit.GetLocation()) // � ����������� �� ������� ������ �� ������� ������
        {
            case Unit.Location.Barrack:
                _unitFriendOnBarrackList.Remove(unit);
                break;
            case Unit.Location.Mission:
                _unitFriendOnMissionList.Remove(unit);
                break;
        }
        OnAddRemoveUnitFromLocation?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// �������� ����� � �������
    /// </summary>
    private void AddUnitToLocation(Unit unit, Unit.Location location)
    {
        switch (location)
        {
            case Unit.Location.Barrack:
                _unitFriendOnBarrackList.Add(unit);
                break;
            case Unit.Location.Mission:
                AddUnitFriendOnMissionList(unit);
                break;
        }
        OnAddRemoveUnitFromLocation?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// �������� ����� �� ������ (� ��������� �� ���������� ������ �� ������)
    /// </summary>
    private void AddUnitFriendOnMissionList(Unit unit)
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
    /// <summary>
    /// ����� ������  ���� ������ 
    /// </summary>
    public List<Unit> GetUnitFriendList() { return _unitFriendList; }
    /// <summary>
    /// ��� ����� ������� �� ������
    /// </summary>
    public List<Unit> GetUnitFriendOnMissionList() { return _unitFriendOnMissionList; }
    /// <summary>
    /// ��� ����� ������� �� ����
    /// </summary>
    public List<Unit> GetUnitFriendOnBarrackList() { return _unitFriendOnBarrackList; }
    public List<Unit> GetUnitEnemyList() { return _unitEnemyList; }
    public List<Unit> GetUnitFriendDeadList() { return _unitFriendDeadList; }
    /// <summary>
    /// ����� ��� �����
    /// </summary>
    public List<Unit> GetHireUnitList() { return _hireUnitList; }
    public void SetSelectedUnit(Unit selectedUnit)
    {
        _selectedUnit = selectedUnit;
        OnSelectedUnitChanged?.Invoke(this, _selectedUnit); // ������������� ������ ������ ����� ��� ��������� ���������� UnitSelectAtEquipmentButtonUI
    }
    public Unit GetSelectedUnit() { return _selectedUnit; }

    /// <summary>
    /// �������� ���������� ����� � ������� ��� (���� �� � ���� �������).
    /// </summary>
    /// <remarks>���� ��� ����� ������� �� ������ NULL</remarks>
    public Unit UpdateSelectedUnitAndReturn()
    {
        if (_selectedUnit != null & _unitFriendList.Contains(_selectedUnit)) // ���� ��� ���� �� ������� � �� ���
            SetSelectedUnit(_selectedUnit);
        else
            SetSelectedUnitFirstOneFromList();
        return _selectedUnit;
    }
    /// <summary>
    /// ���������� ���������� �����, ������� �� ������
    /// </summary>
    private void SetSelectedUnitFirstOneFromList()
    {
        if (_unitFriendList.Count != 0)
            SetSelectedUnit(_unitFriendList[0]);
        else
            SetSelectedUnit(null);
    }

}