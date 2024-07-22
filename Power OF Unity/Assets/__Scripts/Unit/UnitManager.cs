using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager // Менеджер (администратор) Юнитов
{
    public UnitManager(TooltipUI tooltipUI, SoundManager soundManager)
    {
        Init(tooltipUI, soundManager);
    }


    public event EventHandler OnAnyUnitDeadAndRemoveList; // Событие Любой Юнит Умер И Удален из Списка
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // Событие Любой вражеский юнит ражден и добавлен в Списка      
    public event EventHandler OnUnitChangedLocation; // Событие Юнит Изменил Локацию   

    private List<Unit> _unitFriendList = new List<Unit>();// ОБЩИЙ список  моих юнитов
    private List<Unit> _unitFriendOnMissionList = new List<Unit>();// список моих юнитов на МИССИИ
    private List<Unit> _unitFriendOnBarrackList = new List<Unit>();// список моих юнитов в КАЗАРМЕ. По умолчанию все юниты поступают в КАЗАРМУ
    private List<Unit> _unitFriendDeadList = new List<Unit>();// список моих ПОГИБШИХ юнитов 
    private List<Unit> _unitEnemyList = new List<Unit>();  // список Вражеских юнитов

    private TooltipUI _tooltipUI;
    private SoundManager _soundManager;


    private void Init(TooltipUI tooltipUI, SoundManager soundManager)
    {
        _tooltipUI = tooltipUI;
        _soundManager = soundManager;

        InitUnits(firstStart: true);

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // Подпишемся на событие (Любой Рожденный(созданный) Вражеский Юнит)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // Подпишемся на событие (Любой Мертвый Юнит)
    }

    public void InitUnits(bool firstStart)
    {
        if (firstStart)
        {
            UnitTypeBasicListSO unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);

            foreach (UnitTypeSO unitFriendTypeSO in unitTypeBasicListSO.list)
            {
                Unit unit = new Unit(unitFriendTypeSO, _soundManager);
                AddUnitFriendList(unit);
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

        //Debug.Log(unit + " dead"); // Для теста

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
        OnUnitChangedLocation?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Удалить юнита из текущей локации
    /// </summary>
    private void RemoveUnitFromCurrentLocation(Unit unit)
    {
        Unit.Location location = unit.GetLocation(); // В зависимости от локации удалим из нужного списка
        switch (location)
        {
            case Unit.Location.Barrack:
                _unitFriendOnBarrackList.Remove(unit);
                break;
            case Unit.Location.Mission:
                _unitFriendOnMissionList.Remove(unit);
                break;
        }
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

    public List<Unit> GetUnitFriendList() { return _unitFriendList; }
    public List<Unit> GetUnitFriendOnMissionList() { return _unitFriendOnMissionList; }
    public List<Unit> GetUnitFriendOnBarrackList() { return _unitFriendOnBarrackList; }
    public List<Unit> GetUnitEnemyList() { return _unitEnemyList; }
    public List<Unit> GetUnitFriendDeadList() { return _unitFriendDeadList; }

}