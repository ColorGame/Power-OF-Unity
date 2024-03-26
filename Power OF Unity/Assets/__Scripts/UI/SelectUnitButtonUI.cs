using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitActionSystem;

public class SelectUnitButtonUI : MonoBehaviour
{
    [SerializeField] private Transform _enemyUnitButonUIPrefab; // В инспекторе закинем префаб Кнопки
    [SerializeField] private Transform _enemyUnitButonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)
    [SerializeField] private Transform _friendlyUnitButonPrefab; // В инспекторе закинем префаб Кнопки
    [SerializeField] private Transform _friendlyUnitButonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)

    private Dictionary<Unit, FriendlyUnitButtonUI> _friendlyUnitButtonDictionary; // Ключ - юнит. Значение - кнопка для этого юнита
    private List<EnemyUnitButtonUI> _enemyUnitButonUIList; // Список кнопок Вражеских Юнитов   
    private UnitManager _unitManager;
    private TurnSystem _turnSystem;
    private UnitActionSystem _unitActionSystem;

    public void Initialize(UnitManager unitManager, TurnSystem turnSystem, UnitActionSystem unitActionSystem)
    {
        _unitManager = unitManager;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
    }

    private void Awake()
    {
        _friendlyUnitButtonDictionary = new Dictionary<Unit, FriendlyUnitButtonUI>();
        _enemyUnitButonUIList = new List<EnemyUnitButtonUI>();     
    }

    private void Start()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;// Событие Любой Юнит Умер И Удален из Списка
        _unitManager.OnAnyEnemyUnitSpawnedAndAddList += UnitManager_OnAnyEnemyUnitSpawnedAndAddList;// Любой вражеский юнит ражден и добавлен в Списка

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Изменен номер хода подписываемся на Event // Будет выполняться (изминение текста очков действий).
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Занятость Изменена
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; //Выбранный Юнит Изменен
        Unit.OnAnyFriendlyChangeHealth += Unit_OnAnyFriendlyChangeHealth; //У Любого дружественного Юнита изменилось здоровье   
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged; //Изменении очков действий у ЛЮБОГО(Any)

        CreateEnemyUnitButtons(); // Создать Кнопки для  Юнитов
        CreateFriendlyUnitButtons();
        UpdateSelectedVisual();
    }

    private void UnitManager_OnAnyEnemyUnitSpawnedAndAddList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        CreateEnemyUnitButtons();
    }

    private void Unit_OnAnyFriendlyChangeHealth(object sender, EventArgs e)
    {
        FriendlyUnitButtonUI friendlyUnitButonUI = _friendlyUnitButtonDictionary[sender as Unit];
        friendlyUnitButonUI.UpdateHealthBar();
    }
   

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) //sender - отправитель // Подписка должна иметь туже сигнатуру что и функция отправителя OnSelectedUnitChanged
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, OnUnitSystemEventArgs e)
    {
        if (e.selectedAction is GrappleAction comboAction) // Если выполняется Комбо Сделаем проверку состояний комбо
        {
            switch (comboAction.GetState())
            {
                case GrappleAction.State.ComboSearchEnemy: // Если ишу врага то кнопки должны быть скрытыми
                case GrappleAction.State.ComboStart:
                    return; // выходим и игнорируем код ниже
            }
        }

        foreach (FriendlyUnitButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values)// Переберем коллекцию, значения из словаря
        {
            friendlyUnitButonUI.HandleStateButton(e.isBusy);
        }
    }

    // ВНИМАНИЕ // Может возникнуть ошибка. Сброс очков в классе Unit и обновление текста очков в этом классе, СЛУШАЮТ одно и тоже событие. Что выполниться позже или раньше неизвестно, текст может обновиться раньше и показывать еще не сброшенные очки действий "0" а по факту их "2".
    // РЕШЕНИЕ 1 //- НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА UnitActionSystemUI , добавим в Project Settings/ Script Execution Order и поместим НИЖЕ Deafault Time в конец
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs empty) //Произошло изменение очков действий у ЛЮБОГО(Any) юнита а не только у выбранного. обновим их.
    {
        FriendlyUnitButtonUI friendlyUnitButonUI = _friendlyUnitButtonDictionary[sender as Unit];
        friendlyUnitButonUI.UpdateActionPoints();// Обнавление очков действий         
    }
    

    private void CreateEnemyUnitButtons() // Создать Кнопки для Врагов Юнитов
    {
        foreach (Transform buttonTransform in _enemyUnitButonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _enemyUnitButonUIList.Clear(); // Очистим сисок кнопок

        foreach (Unit unit in _unitManager.GetEnemyUnitList())// Переберем вражеских юнитов
        {
            if (unit.gameObject.activeSelf) // Если юнит активный то 
            {
                Transform actionButtonTransform = Instantiate(_enemyUnitButonUIPrefab, _enemyUnitButonContainerTransform); // Для каждого ЮНИТА создадим префаб кнопки и назначим родителя - Контейнер для кнопок
                EnemyUnitButtonUI enemyUnitButonUI = actionButtonTransform.GetComponent<EnemyUnitButtonUI>();// У кнопки найдем компонент EnemyUnitButtonUI
                enemyUnitButonUI.SetUnit(unit);//Назвать и Присвоить


                _enemyUnitButonUIList.Add(enemyUnitButonUI);// Добавим в список нашу кнопку
            }
        }
    }

    private void CreateFriendlyUnitButtons() // Создать Кнопки для Дружественныйх Юнитов
    {
        foreach (Transform buttonTransform in _friendlyUnitButonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _friendlyUnitButtonDictionary.Clear();

        foreach (Unit unit in _unitManager.GetMyUnitList())// Переберем дружественных юнитов
        {
            Transform actionButtonTransform = Instantiate(_friendlyUnitButonPrefab, _friendlyUnitButonContainerTransform); // Для каждого ЮНИТА создадим префаб кнопки и назначим родителя - Контейнер для кнопок
            FriendlyUnitButtonUI friendlyUnitButonUI = actionButtonTransform.GetComponent<FriendlyUnitButtonUI>();// У кнопки найдем компонент FriendlyUnitButtonUI
            friendlyUnitButonUI.SetUnit(unit);//Назвать и Присвоить

            _friendlyUnitButtonDictionary[unit] = friendlyUnitButonUI; // Ключ - ЮНИТ. Значение - FriendlyUnitButtonUI        
        }
    }

    private void UpdateSelectedVisual() //Обнавление визуализации выбора( при выборе кнопки включим рамку)
    {
        foreach (FriendlyUnitButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // Переберем коллекцию, значения из словаря
        {
            friendlyUnitButonUI.UpdateSelectedVisual();
        }
    }
    private void UpdateButtonVisibility() // Обновление визуализации кнопок в зависимости от того ЧЕЙ ХОД (прятать во время хода врага)
    {
        bool isBusy = !TurnSystem.Instance.IsPlayerTurn(); // Занято когда ходит враг (НЕ Я)

        foreach (FriendlyUnitButtonUI friendlyUnitButonUI in _friendlyUnitButtonDictionary.Values) // Переберем коллекцию, значения из словаря
        {
            friendlyUnitButonUI.HandleStateButton(isBusy);
        }
    }
}
