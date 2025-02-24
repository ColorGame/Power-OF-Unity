using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Этот клас важен и он должен просыпаться самым первым. Настроим его, добавим в Project Settings/ Script Execution Order и поместим выше Deafault Time
public class UnitActionSystem : MonoBehaviour // Система действий юнита (ОБРАБОТКА ВЫБОРА ДЕЙСТВИЯ ЮНИТА)
{
    public event EventHandler<Unit> OnSelectedUnitChanged; // Выбранный Юнит Изменен (когда поменяется выбранный юнит мы запустим событие Event) <Unit>-новый выбранный юнит
    public event EventHandler OnSelectedActionChanged; // Выбранное Действие Изменено (когда меняется активное действие в блоке кнопок мы запустим событие Event)  
    public event EventHandler OnGameOver; // Конец игры

    public event EventHandler<OnUnitSystemEventArgs> OnBusyChanged; // Занятость Изменена (когда меняется значение _isBusy, мы запустим событие Event, и передаем ее в аргументе) в <> -generic этот тип будем вторым аргументом

    public class OnUnitSystemEventArgs : EventArgs // Расширим класс событий, чтобы в аргументе события передать нужных юнитов
    {
        public bool isBusy;
        public BaseAction selectedAction; // выбранное действие
    }


    private LayerMask _unitLayerMask; // маска слоя юнитов (появится в ИНСПЕКТОРЕ) НАДО ВЫБРАТЬ Units
    private Unit _selectedUnit; // Выбранный юнит (ПО УМОЛЧАНИЮ).Ниже сделаем общедоступный метод который будет возвращать ВЫБРАННОГО ЮНИТА

    private GameInput _gameInput;
    private BaseAction _selectedAction; // Выбранное Действие// Будем передовать в Button   
    private bool _isBusy; // Занят (булевая переменная для исключения одновременных действий)
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private LevelGrid _levelGrid;
    private MouseOnGameGrid _mouseOnGameGrid;

    public void Init(GameInput gameInput, UnitManager unitManager, TurnSystem turnSystem, LevelGrid levelGrid, MouseOnGameGrid mouseOnGameGrid)
    {
        _gameInput = gameInput;
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _levelGrid = levelGrid;
        _mouseOnGameGrid = mouseOnGameGrid;

        Setup();
    }

    private void Setup()
    {
        _unitLayerMask = LayerMask.GetMask("Units");
        _selectedUnit = _unitManager.GetUnitOnMissionList()[0];
        SetSelectedUnit(_selectedUnit, _selectedUnit.GetAction<MoveAction>()); // Присвоить(Установить) выбранного юнита, Установить Выбранное Действие,   // При старте в _targetUnit передается юнит по умолчанию 


        // Пока сделал в старте так как возникает гонка
        _gameInput.OnClickAction += GameInput_OnClickAction; // Подпишемся на событие клик по мыши или геймпаду
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList; //Подпишемся на событие Любой Юнит Умер И Удален из Списка
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Подпишемся Ход Изменен
    }


    private void OnDisable()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList -= UnitManager_OnAnyUnitDeadAndRemoveList; //Подпишемся на событие Любой Юнит Умер И Удален из Списка
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged; // Подпишемся Ход Изменен
        _gameInput.OnClickAction -= GameInput_OnClickAction; // Подпишемся на событие клик по мыши или геймпаду
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (_turnSystem.IsPlayerTurn()) // Если ход Игрока то
        {
            List<Unit> myUnitList = _unitManager.GetUnitList(); // Вернем список моих юнитов
            if (myUnitList.Count > 0) // Если есть живые то передаем выделению первому по списку юниту
            {
                SetSelectedUnit(myUnitList[0], myUnitList[0].GetAction<MoveAction>());
            }
        };
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        if (_selectedUnit.GetHealthSystem().IsDead()) // Если выделенный юнит отъехал то ...
        {
            List<Unit> friendlyUnitList = _unitManager.GetUnitList(); // Вернем список дружественных юнитов
            if (friendlyUnitList.Count > 0) // Если есть живые то передаем выделению первому по списку юниту
            {
                SetSelectedUnit(friendlyUnitList[0], friendlyUnitList[0].GetAction<MoveAction>());
            }
            else // Если нет никого в живых то КОНЕЦ
            {
                OnGameOver?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void GameInput_OnClickAction(object sender, EventArgs e)
    {
        if (_isBusy) // Если занят ... то остановить выполнение
        {
            return;
        }

        if (!_turnSystem.IsPlayerTurn()) // Проверяем это очередь игрока если нет то остановить выполнение
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())  // Проверим, наведен ли указатель мыши на элементе пользовательского интерфейса  
                                                            // Встроенная в юнити функция событий. (current - Возвращает текущую систему событий.) (IsPointerOverGameObject() -наведение указателя (мыши) на игровой объект)
        {
            return; // Если указатель мыши над кнопкой(UI), ТОГДА останавливаем метод , что бы во время кликанья по кнопке, юнит не пошол в место сетки которая находиться под кнопкой
        }
        if (TryHandleUnitSelection()) // Попытка обработки выбора юнита
        {
            return; //Если мы выбрали юнита то TryHandleUnitSelection() вернет true. ТОГДА останавливаем метод, чтобы во время кликанья по юниту, которого хотим выбрать, предыдущий выбранный юнит не шел в место клика мышки
        }

        HandleSelectedAction(); // Обработать выбранное действие      
    }

    public void HandleSelectedAction() // Обработать выбранное действие
    {
        GridPositionXZ mouseGridPosition = _levelGrid.GetGridPosition(_mouseOnGameGrid.GetPositionOnlyHitVisible()); // Преобразуем позицию мыши из мирового в сеточную.

        if (!_selectedAction.IsValidActionGridPosition(mouseGridPosition)) // Проверяем для нашего выбранного действия, сеточную позицию мыши на допустимость действий . Если не допустимо то...
        {
            return; // Остановить выполнение  //Добавление ! и return; помогает раскрвть скобки if()
        }

        if (!_selectedUnit.GetActionPointsSystem().TrySpendActionPointsToTakeAction(_selectedAction)) // для Выбранного Юнита ПОПРОБУЕМ Потратить Очки Действия, Чтобы Выполнить выбранное Действие. Если не можем то...
        {
            return; // Остановить выполнение
        }

        SetBusy(); // Установить Занятый
        _selectedAction.TakeAction(mouseGridPosition, ClearBusy); //У выбранного действия вызовим метод "Применить Действие (Действовать)" и передадим в делегат функцию ClearBusy
    }

    private void SetBusy() // Установить Занятый
    {
        _isBusy = true;
        OnBusyChanged?.Invoke(this, new OnUnitSystemEventArgs // создаем новый экземпляр класса OnUnitSystemEventArgs
        {
            isBusy = _isBusy,
            selectedAction = _selectedAction,
        });
    }

    private void ClearBusy() // Очистить занятость или стать свободным
    {
        _isBusy = false;
        OnBusyChanged?.Invoke(this, new OnUnitSystemEventArgs // создаем новый экземпляр класса OnUnitSystemEventArgs
        {
            isBusy = _isBusy,
            selectedAction = _selectedAction,
        });
    }

    private bool TryHandleUnitSelection() // Попытка обработки выбора юнита
    {
        if (_selectedAction is GrappleAction comboAction) //Если Выбранное Действие GrappleAction то
        {
            switch (comboAction.GetState()) // Получим состояние КОМБО 
            {
                case GrappleAction.State.ComboSearchEnemy:
                case GrappleAction.State.ComboStart:
                    // В этих режимах нельзя выбирать другого юнита
                    return false;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(_gameInput.GetMouseScreenPoint()); // Луч от камеры в точку где находиться курсор мыши
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask)) // Вернет true если во что-то попадет. Т.к. указана маска взаимодействия то реагировать будет только на юнитов
        {   // Проверим есть ли на объекте в который мы попали компонент  <Unit>
            if (raycastHit.transform.TryGetComponent(out UnitCoreView unitCore)) // ПРЕИМУЩЕСТВО TryGetComponent перед CreateInstanceClass в том что НЕ НАДО делать нулевую проверку. TryGetComponent - возвращает true, если компонент < > найден. Возвращает компонент указанного типа, если он существует.
            {
                Unit unit = unitCore.GetUnit();
                if (unit == _selectedUnit) // Данная проверка позволяет нажимать на выбранного юнита для выполнения _selectedAction (нажимать сквозь выбранного юнита на сеточную позиция спрятанную за ним) Если эти строки убрать то вместо выполнения _selectedAction мы просто опять выберим юнита.
                {
                    // ЭТОТ ЮНИТ УЖЕ ВЫБРАН
                    return false;
                }

                if (unit.GetType() != _selectedUnit.GetType()) // Если луч попал во врага (их типы не совподают)
                {
                    // ЭТО ВРАГ ЕГО ВЫБИРАТЬ НЕ НАДО
                    return false;
                }
                SetSelectedUnit(unit, unit.GetAction<MoveAction>()); // Объект (Юнит) в который попал луч становиться ВЫБРАННЫМ.
                return true;
            }
        }
        // Не смог выбрать юнита
        return false;
    }

    public void SetSelectedUnit(Unit unit, BaseAction baseAction) // Присвоить(Установить) выбранного юнита,и Установить базовое Действие, И запускаем событие   
    {
        _selectedUnit = unit; // аргумент переданный в этот метод становиться ВЫБРАННЫМ юнитом.

        SetSelectedAction(baseAction); // Получим компонент "MoveAction"  нашего Выбранного юнита (по умолчанию при старте базовым действием бедет MoveAction). Сохраним в переменную _selectedAction через функцию SetSelectedAction()

        OnSelectedUnitChanged?.Invoke(this, _selectedUnit); // "?"- проверяем что !=0. Invoke вызвать (this-ссылка на объект который запускает событие "отправитель" а класс UnitSelectedVisual и ActionButtonSystemUI будет его прослушивать "обрабатывать" для этого ему нужна ссылка на _targetUnit)
    }

    public void SetSelectedAction(BaseAction baseAction) //Установить Выбранное Действие, И запускаем событие  
    {
        _selectedAction = baseAction;
       
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); // "?"- проверяем что !=0. Invoke вызвать (this-ссылка на объект который запускает событие "отправитель" а класс ActionButtonSystemUI  LevelGridVisual будет его прослушивать "обрабатывать")
    }
    public BaseAction GetSelectedAction() { return _selectedAction; }// Вернуть выбранное действие
    public Unit GetSelectedUnit() { return _selectedUnit; }



}
