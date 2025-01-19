using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager // Менеджер (администратор) Юнитов
{
    public UnitManager(TooltipUI tooltipUI, SoundManager soundManager, WarehouseManager warehouseManager, HashAnimationName hashAnimationName)
    {
        Init(tooltipUI, soundManager, warehouseManager, hashAnimationName);
    }

    public event EventHandler<Unit> OnSelectedUnitChanged; // Изменен выбранный юнит
    public event EventHandler OnAnyUnitDeadAndRemoveList; // Событие Любой Юнит Умер И Удален из Списка
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // Событие Любой вражеский юнит ражден и добавлен в Списка      
    public event EventHandler OnAddRemoveUnitFromLocation; // Событие Добавление удаление юнита из локации

    private List<Unit> _unitFriendList = new List<Unit>();// ОБЩИЙ список  моих юнитов
    private List<Unit> _unitFriendOnMissionList = new List<Unit>();// список моих юнитов на МИССИИ
    private List<Unit> _unitFriendOnBarrackList = new List<Unit>();// список моих юнитов в КАЗАРМЕ. По умолчанию все юниты поступают в КАЗАРМУ
    private List<Unit> _unitFriendDeadList = new List<Unit>();// список моих ПОГИБШИХ юнитов 
    private List<Unit> _unitEnemyList = new List<Unit>();  // список Вражеских юнитов
    private List<Unit> _hireUnitList = new List<Unit>();// список Юнитов для НАЙМА

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

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // Подпишемся на событие (Любой Рожденный(созданный) Вражеский Юнит)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // Подпишемся на событие (Любой Мертвый Юнит)

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
            // Реализовать загрузку
        }
    }



    private void Unit_OnAnyEnemyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        _unitEnemyList.Add(unit); // Добавим его в список Вражеских Юнитов
        OnAnyEnemyUnitSpawnedAndAddList?.Invoke(this, EventArgs.Empty);
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        //Debug.Log(hireUnit + " dead"); // Для теста

        if (unit.IsEnemy()) // Если отправитель Враг то ...
        {
            _unitEnemyList.Remove(unit); // Удалим его из списка Вражеских Юнитов           
        }
        else// если нет
        {
            RemoveUnitFriendList(unit);
            _unitFriendDeadList.Add(unit); // Добавим к погибшим
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // Запустим событьие
    }

    /// <summary>
    /// Попробую Нанять выбранного юнита
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
    /// Уволить выбранного юнита
    /// </summary>
    public void DismissSelectedUnit()
    {
        // Переберем экипировку юнита
        foreach (PlacedObjectGridParameters placedObjectGridParameters in _selectedUnit.GetUnitEquipment().GetEquipmentList())
        {
            _warehouseManager.PlusCountPlacedObject(placedObjectGridParameters.placedObjectTypeSO);
        }
        _selectedUnit.GetUnitEquipment().ClearEquipmentList();
        RemoveUnitFriendList(_selectedUnit);
        AddHireUnitList(_selectedUnit);
    }
    /// <summary>
    /// Добавить в список для найма
    /// </summary>
    private void AddHireUnitList(Unit unit)
    {
        _hireUnitList.Add(unit);
    }
    /// <summary>
    /// Удалить из списока для найма
    /// </summary>
    private void RemoveHireUnitList(Unit unit)
    {
        _hireUnitList.Remove(unit);
    }
    /// <summary>
    /// Очистить поле выбранный юнит
    /// </summary>
    public void ClearSelectedUnit()
    {
        SetSelectedUnit(null);
    }

    /// <summary>
    /// Добавить Юнита в общий список и добавим в локацию
    /// </summary>
    private void AddUnitFriendList(Unit unit)
    {
        _unitFriendList.Add(unit);
        Unit.Location location = unit.GetLocation(); // Получим локацию юнита
        AddUnitToLocation(unit, location);
    }

    /// <summary>
    /// Удалить юнита из общего списка и текущей локации
    /// </summary>
    private void RemoveUnitFriendList(Unit unit)
    {
        _unitFriendList.Remove(unit); // Удалим его из списка моих Юнитов
        RemoveUnitFromCurrentLocation(unit);
    }

    /// <summary>
    /// Поменять локацию юнита
    /// </summary>
    public void ChangeLocation(Unit unit, Unit.Location newLocation)
    {
        RemoveUnitFromCurrentLocation(unit);
        unit.SetLocation(newLocation);
        AddUnitToLocation(unit, newLocation);       
    }

    /// <summary>
    /// Удалить юнита из текущей локации
    /// </summary>
    private void RemoveUnitFromCurrentLocation(Unit unit)
    {
        switch (unit.GetLocation()) // В зависимости от локации удалим из нужного списка
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
    /// Добавить юнита в локацию
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
    /// Добавить Юнита на МИССИЮ (С проверкой на количество юнитов на миссии)
    /// </summary>
    private void AddUnitFriendOnMissionList(Unit unit)
    {
        if (_unitFriendOnMissionList.Count <= Constant.COUNT_UNIT_ON_MISSION_MAX) // Проверим если количество юнитов в списке меньше МАКСТМАЛЬНОГО то добавим в список переданного юнита
        {
            _unitFriendOnMissionList.Add(unit);
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse("Все 12 мест заняты", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки
        }
    }
    /// <summary>
    /// Общий список  моих юнитов 
    /// </summary>
    public List<Unit> GetUnitFriendList() { return _unitFriendList; }
    /// <summary>
    /// Мои юниты которые на МИССИИ
    /// </summary>
    public List<Unit> GetUnitFriendOnMissionList() { return _unitFriendOnMissionList; }
    /// <summary>
    /// Мои юниты которые на БАЗЕ
    /// </summary>
    public List<Unit> GetUnitFriendOnBarrackList() { return _unitFriendOnBarrackList; }
    public List<Unit> GetUnitEnemyList() { return _unitEnemyList; }
    public List<Unit> GetUnitFriendDeadList() { return _unitFriendDeadList; }
    /// <summary>
    /// Юниты для найма
    /// </summary>
    public List<Unit> GetHireUnitList() { return _hireUnitList; }
    public void SetSelectedUnit(Unit selectedUnit)
    {
        _selectedUnit = selectedUnit;
        OnSelectedUnitChanged?.Invoke(this, _selectedUnit); // Подписываятся кнопки выбора юнита для настройки экипировки UnitSelectAtEquipmentButtonUI
    }
    public Unit GetSelectedUnit() { return _selectedUnit; }

    /// <summary>
    /// Обновить выбранного юнита и вернуть его (если он в моей команде).
    /// </summary>
    /// <remarks>Если все юниты погибли то вернем NULL</remarks>
    public Unit UpdateSelectedUnitAndReturn()
    {
        if (_selectedUnit != null & _unitFriendList.Contains(_selectedUnit)) // Если это юнит не нулевой и он мой
            SetSelectedUnit(_selectedUnit);
        else
            SetSelectedUnitFirstOneFromList();
        return _selectedUnit;
    }
    /// <summary>
    /// Установить выбранного юнита, первого из списка
    /// </summary>
    private void SetSelectedUnitFirstOneFromList()
    {
        if (_unitFriendList.Count != 0)
            SetSelectedUnit(_unitFriendList[0]);
        else
            SetSelectedUnit(null);
    }

}