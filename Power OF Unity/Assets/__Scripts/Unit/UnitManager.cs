using System;
using System.Collections.Generic;
using UnityEngine;

// НАСТРОИМ  CoreEntryPoint ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА UnitManager и Unit , чтобы UnitManager запустился раньше чем сценарий Unit
// (что бы сначала запустить слушателя в UnitManager а потом запустить само событие в Unit)
// Иначе если скрипт Unit проснется раньше то юнит не добавиться в СПИСОК ЮНИТОВ т.к. тот еще не запустился
public class UnitManager // Менеджер (администратор) Юнитов
{
    private const int MAX_COUNT_UNIT_ON_MISSION = 12;// Максимальное количество юнитов на миссии

    public UnitManager() { }// Конструктор что бы отследить количество созданных new T() ОН ДОЛЖЕН БЫТЬ ОДИН


    public event EventHandler OnAnyUnitDeadAndRemoveList; // Событие Любой Юнит Умер И Удален из Списка
    public event EventHandler OnAnyEnemyUnitSpawnedAndAddList; // Событие Любой вражеский юнит ражден и добавлен в Списка      

    private List<Unit> _myUnitList;// список  моих юнитов
    private List<Unit> _myUnitOnMissionList;// список моих юнитов на Миссии
    private List<Unit> _myDeadUnitnList;// список моих погибших юнитов 
    private List<Unit> _enemyUnitList;  // список Вражеских юнитов

    private UnitTypeBasicListSO _unitTypeBasicListSO; // список для создания базовых юнитов
    private TooltipUI _tooltipUI;

    public void Initialize(TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        // Проведем инициализацию списков        
        _myUnitList = new List<Unit>();
        _myUnitOnMissionList = new List<Unit>();
        _myDeadUnitnList = new List<Unit>();
        _enemyUnitList = new List<Unit>();

        _unitTypeBasicListSO = Resources.Load<UnitTypeBasicListSO>(typeof(UnitTypeBasicListSO).Name);    // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                         // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр UnitTypeBasicListSO и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра

        Unit.OnAnyEnemyUnitSpawned += Unit_OnAnyEnemyUnitSpawned; // Подпишемся на событие (Любой Рожденный(созданный) Вражеский Юнит)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // Подпишемся на событие (Любой Мертвый Юнит)
    }


    private void Unit_OnAnyEnemyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        _enemyUnitList.Add(unit); // Добавим его в список Вражеских Юнитов
        OnAnyEnemyUnitSpawnedAndAddList?.Invoke(this, EventArgs.Empty);
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        //Debug.Log(unit + " dead"); // Для теста

        if (unit.IsEnemy()) // Если отправитель Враг то ...
        {
            _enemyUnitList.Remove(unit); // Удалим его из списка Вражеских Юнитов           
        }
        else// если нет
        {
            _myUnitList.Remove(unit); // Удалим его из списка моих Юнитов
            _myDeadUnitnList.Add(unit); // Добавим к погибшим
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // Запустим событьие
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

    public List<Unit> GetMyDeadUnitnList()
    {
        return _myDeadUnitnList;
    }

    public int GetMaxCountUnitOnMission()
    {
        return MAX_COUNT_UNIT_ON_MISSION;
    }
}