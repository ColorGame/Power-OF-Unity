using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager // Менеджер (администратор) Юнитов
{ 
    public UnitManager(TooltipUI tooltipUI) 
    {
        Init(tooltipUI);
    }


    public event EventHandler OnAnyUnitDeadAndRemoveList; // Событие Любой Юнит Умер И Удален из Списка
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // Событие Любой вражеский юнит ражден и добавлен в Списка      

    private List<Unit> _unitFriendList;// список  моих юнитов
    private List<Unit> _unitFriendOnMissionList;// список моих юнитов на Миссии
    private List<Unit> _unitFriendDeadList;// список моих погибших юнитов 
    private List<Unit> _unitEnemyList;  // список Вражеских юнитов

    private UnitTypeBasicListSO _unitTypeBasicListSO; // список для создания базовых юнитов
    private TooltipUI _tooltipUI;

    private void Init(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        // Проведем инициализацию списков        
        _unitFriendList = new List<Unit>();
        _unitFriendOnMissionList = new List<Unit>();
        _unitFriendDeadList = new List<Unit>();
        _unitEnemyList = new List<Unit>();

        _unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);    // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                         // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр UnitTypeBasicListSO и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // Подпишемся на событие (Любой Рожденный(созданный) Вражеский Юнит)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // Подпишемся на событие (Любой Мертвый Юнит)
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
            _unitFriendList.Remove(unit); // Удалим его из списка моих Юнитов
            _unitFriendDeadList.Add(unit); // Добавим к погибшим
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // Запустим событьие
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
        if (_unitFriendOnMissionList.Count <= Constant.COUNT_UNIT_ON_MISSION_MAX) // Проверим если количество юнитов в списке меньше МАКСТМАЛЬНОГО то добавим в список переданного юнита
        {
            _unitFriendOnMissionList.Add(unit);
        }
        else
        {
            _tooltipUI.ShowTooltipsFollowMouse("Все 12 мест заняты", new TooltipUI.TooltipTimer { timer = 2f }); // Покажем подсказку и зададим новый таймер отображения подсказки
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