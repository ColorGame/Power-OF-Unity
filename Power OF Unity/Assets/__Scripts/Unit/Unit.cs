using System;
using UnityEngine;

/// <summary>
/// Основной класс Юнита без MonoBehaviour
/// </summary>
/// <remarks>
/// Инициализирует подсистемы юнита
/// </remarks>
public class Unit
{
    public Unit(UnitTypeSO unitTypeSO, SoundManager soundManager)
    {
        _soundManager = soundManager;
        _healthSystem = new Health(unitTypeSO.GetBasicHealth(), _soundManager);
        _actionPointsSystem = new UnitActionPoints(this);
        _unitInventory = new UnitInventory(this);
        _unitEquipment = new UnitEquipment(this);

        switch (unitTypeSO)
        {
            case UnitFriendSO unitFriendSO:
                _unitTypeSO = unitFriendSO;
                _unitAmorType = unitFriendSO.GetUnitArmorType();
                _isEnemy = false;
                break;
            case UnitEnemySO unitEnemySO:
                _unitTypeSO = unitEnemySO;
                _isEnemy = true;
                break;
        }
    }


    public static event EventHandler OnAnyEnemyUnitSpawned; // Событие Любой Вражеский Рожденный(созданный) Юнит
    public static event EventHandler OnAnyUnitDead; // Событие Любой Юнит Умер    

    // Частные случаи
    private bool _isEnemy;
    private GridPositionXZ _gridPosition;

    readonly UnitTypeSO _unitTypeSO;
    readonly Health _healthSystem;
    readonly UnitActionPoints _actionPointsSystem;
    readonly UnitInventory _unitInventory;
    readonly UnitEquipment _unitEquipment;
    private BaseAction[] _baseActionsArray; // Массив базовых действий 
    private UnitArmorType _unitAmorType; //Тип брони юнита        
    private Transform _unitCoreTransform;

    private LevelGrid _levelGrid;
    private TurnSystem _turnSystem;
    readonly SoundManager _soundManager;
    private UnitActionSystem _unitActionSystem;
    private HandleAnimationEvents _handleAnimationEvents;
    private CameraFollow _cameraFollow;
    private Rope _unitRope;
    private Transform _headTransform;

    /// <summary>
    /// Настройка ЮНИТА при спанвне на Уровня. (Настроим transform и gridPosition ...)
    /// </summary>   
    public void SetupForSpawn(LevelGrid levelGrid, TurnSystem turnSystem, Transform unitCoreTransform, CameraFollow cameraFollow, UnitActionSystem unitActionSystem)
    {
        // Когда Unit спавниться, настроим его положение в сетке и добовим к GridObjectUnitXZ(объектам сетки) в данной ячейки         

        _levelGrid = levelGrid;
        _turnSystem = turnSystem;
        _unitActionSystem = unitActionSystem;
        _unitCoreTransform = unitCoreTransform;
        _cameraFollow = cameraFollow;
        _handleAnimationEvents = unitCoreTransform.GetComponentInChildren<HandleAnimationEvents>(true);
        _baseActionsArray = unitCoreTransform.GetComponents<BaseAction>();
        _unitRope = unitCoreTransform.GetComponent<Rope>();
        _headTransform = unitCoreTransform.Find("Head");

        _gridPosition = _levelGrid.GetGridPosition(unitCoreTransform.position); //Получим сеточную позицию в месте спавна
        _levelGrid.AddUnitAtGridPosition(_gridPosition, this); //Добавим юнита в нашу сетку

        ISetupForSpawn[] iSetupForSpawnArray = unitCoreTransform.GetComponentsInChildren<ISetupForSpawn>(true);   // Найдем прикрепленные к unitCoreTransform, или к его дочерним объектам, все классы, реализующие интерфейс ISetupForSpawn.             
        foreach (var iSetupForSpawn in iSetupForSpawnArray)
        {
            iSetupForSpawn.SetupForSpawn(this);
        }

        _healthSystem.SetupForSpawn();
        _actionPointsSystem.SetupForSpawn();

        _healthSystem.OnDead += HealthSystem_OnDead; // подписываемся на Event. Будет выполняться при смерти юнита       

        if (_isEnemy)
            OnAnyEnemyUnitSpawned?.Invoke(this, EventArgs.Empty); // Запустим событие Любой Рожденный(созданный) Вражеский Юнит. Событие статичное поэтому будет выполняться для всех вражеских созданных Юнитов
    }

    /// <summary>
    ///  Настройка ЮНИТА при смерти удалении или выходе из игровой сцены.
    /// </summary>
    private void SetupOnDestroyAndQuit()
    {
        _levelGrid.GetGridPosition(GetTransformPosition());
        _levelGrid.RemoveUnitAtGridPosition(_gridPosition, this);
        _actionPointsSystem.SetupOnDestroyAndQuit();

        _healthSystem.OnDead -= HealthSystem_OnDead;
    }

    public void UpdateGridPosition()
    {
        GridPositionXZ newGridPosition = _levelGrid.GetGridPosition(GetTransformPosition()); //Получим новую позицию юнита на сетке.
        if (newGridPosition != _gridPosition) // Если новая позиция на сетке отличается от последней то ...
        {
            // Изменем положение юнита на сетке
            GridPositionXZ oldGridPosition = _gridPosition; // Сохраним старую позицию что бы передать в event
            _gridPosition = newGridPosition; //Обновим позицию - Новая позиция становиться текущей

            _levelGrid.UnitMovedGridPosition(this, oldGridPosition, newGridPosition); //в UnitMovedGridPosition запускаем Событие. Поэтому эту строку поместим в КОНЦЕЦ . Иначе мы запускаем событие сетка обнавляется а юнит еще не перемещен
        }
    }

    public T GetAction<T>() where T : BaseAction //Фунцкция для получения любого типа базового действия // Создадим метод с GENERICS и ограничим типами в  BaseAction
    {
        foreach (BaseAction baseAction in _baseActionsArray) // Переберем Массив базовых действий
        {
            if (baseAction is T) // если T совподает с каким нибудь baseAction то ...
            {
                return baseAction as T; // Вернем это базовое действие КАК Т // (T)baseAction; - еще один метод записи
            }
        }
        return null; // Если нет совпадений то вернем ноль
    }

    public GridPositionXZ GetGridPosition() // Получить сеточную позицию
    {
        return _gridPosition;
    }

    public Vector3 GetWorldPosition() // Получить мировую позицию
    {
        return GetTransformPosition();
    }

    public BaseAction[] GetBaseActionsArray() // Получить Список базовых действий
    {
        return _baseActionsArray;
    }

    public bool IsEnemy() // Раскроем поле
    {
        return _isEnemy;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) // Будет выполняться при смерти юнита
    {
        SetupOnDestroyAndQuit();
        UnityEngine.Object.Destroy(_unitCoreTransform.gameObject); // Уничтожим игровой объект к которому прикриплен данный скрипт

        // Вслучае смерти активного Юнита надо предать ход следующему юниту это происходит в UnitActionSystem    

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty); // Запустим событие Любой Мертвый Юнит. Событие статичное поэтому будет выполняться для любого мертвого Юнита      
    }

    public Health GetHealthSystem() { return _healthSystem; }
    public UnitActionPoints GetActionPointsSystem() { return _actionPointsSystem; }
    public UnitInventory GetUnitInventory() { return _unitInventory; }
    public UnitEquipment GetUnitEquipment() { return _unitEquipment; }
    public Vector3 GetTransformPosition() { return _unitCoreTransform.position; }
    public void SetTransformPosition(Vector3 position) { _unitCoreTransform.position = position; }
    public Transform GetTransform() { return _unitCoreTransform; }
    public void SetTransformForward(Vector3 forward) { _unitCoreTransform.forward = forward; }
    public T GetUnitTypeSO<T>() where T : UnitTypeSO //Фунцкция для получения любого наследника от UnitTypeSO
    {
        if (_unitTypeSO is T unit_T_SO)
        {
            return unit_T_SO;
        }
        return null; // Если нет совпадений то вернем ноль
    }
    public UnitTypeSO GetUnitTypeSO() { return _unitTypeSO; }
    public UnitArmorType GetUnitArmorType() { return _unitAmorType; }
    public TurnSystem GetTurnSystem() { return _turnSystem; }
    public SoundManager GetSoundManager() { return _soundManager; }
    public UnitActionSystem GetUnitActionSystem() { return _unitActionSystem; }
    public LevelGrid GetLevelGrid() { return _levelGrid; }
    public HandleAnimationEvents GetHandleAnimationEvents() { return _handleAnimationEvents; }
    public CameraFollow GetCameraFollow() { return _cameraFollow; }
    public Rope GetUnitRope() { return _unitRope; }
    public Transform GetHeadTransform() { return _headTransform; }
}

