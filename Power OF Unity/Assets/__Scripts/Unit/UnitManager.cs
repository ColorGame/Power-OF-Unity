using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// РЕШЕНИЕ //- НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА UnitManager и Unit , добавим в Project Settings/ Script Execution Order и поместим выполнение UnitManager выше Default Time), чтобы UnitManager запустился раньше чем сценарий Unit
// (что бы сначала запустить слушателя в UnitManager а потом запустить само событие в Unit)
// Иначе если скрипт Unit проснется раньше то юнит не добавиться в СПИСОК ЮНИТОВ т.к. тот еще не запустился
public class UnitManager // Менеджер (администратор) Юнитов
{
    private const int MAX_COUNT_UNIT_ON_MISSION = 12;// Максимальное количество юнитов на миссии

    public UnitManager() // Конструктор что бы отследить количество созданных new T() ОН ДОЛЖЕН БЫТЬ ОДИН
    {
    }

    public  event EventHandler OnAnyUnitDeadAndRemoveList; // Событие Любой Юнит Умер И Удален из Списка
    public  event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // Событие Любой вражеский юнит ражден и добавлен в Списка

    private List<Unit> _myUnitList;// список  моих юнитов
    private List<Unit> _myUnitOnMissionList;// список моих юнитов на Миссии
    private List<Unit> _enemyUnitList;  // список Вражеских юнитов

    private TooltipUI _tooltipUI;  



    public void Initialize(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        // Проведем инициализацию списков        
        _myUnitList = new List<Unit>();
        _myUnitOnMissionList = new List<Unit>();
        _enemyUnitList = new List<Unit>();

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // Подпишемся на событие (Любой Рожденный(созданный) Юнит)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // Подпишемся на событие (Любой Мертвый Юнит)
    }       


    private void Unit_OnAnyEnemyUnitSpawned(object sender, System.EventArgs e) // При рождении юнитов распределим их по спискам
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        //Debug.Log(unit + " spawner"); // Для теста

        if (unit.IsEnemy()) // Если отправитель Враг то ...
        {
            _enemyUnitList.Add(unit); // Добавим его в список Вражеских Юнитов
            OnAnyEnemyUnitSpawnedAndAddList?.Invoke(this, EventArgs.Empty);
        }
        else// если нет
        {
            _myUnitList.Add(unit); // Добавим его в список Дружественных Юнитов
        }

    }
    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        //Debug.Log(unit + " dead"); // Для теста

        if (unit.IsEnemy()) // Если отправитель Враг то ...
        {
            _enemyUnitList.Remove(unit); // Удалим его из списка Вражеских Юнитов           
        }
        else// если нет
        {
            _myUnitList.Remove(unit); // Удалим его из списка Дружественных Юнитов
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // Запустим событьие
    }

    public void AddMyUnitOnMissionList(Unit unit)
    {
        if (_myUnitOnMissionList.Count <= Constant.MAX_COUNT_UNIT_ON_MISSION) // Проверим если количество юнитов в списке меньше МАКСТМАЛЬНОГО то добавим в список переданного юнита
        {
            _myUnitOnMissionList.Add(unit); 
        }
        else
        {
            _tooltipUI.ShowTooltipsFollowMouse("Все 12 мест заняты", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки
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