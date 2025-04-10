using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовое Действие. Абстрактный клас 
/// </summary>
/// <remarks>
/// Все базовые дествия должны быть прикрепленны к Unit.
/// </remarks>
public abstract class BaseAction : MonoBehaviour, ISetupForSpawn
{
    public static event EventHandler OnAnyActionStart; //Запуск Любого Действия.
    public static event EventHandler OnAnyActionCompleted; //Завершении Любого Действия.

    protected bool _isActive; // Булевая переменная. Что бы исключить паралельное выполнение нескольких действий
    protected Unit _unit;
    protected LevelGrid _levelGrid;
    protected PlacedObjectTypeWithActionSO _placedObjectTypeWithActionSO;

    //Буду использовать встроенный делегат Action вместо - //public delegate void ActionCompleteDelegate(); //завершение действия // Объявляем делегат который не принимает аргумент и возвращает пустоту
    protected Action _onActionComplete; //(по завершении действия)// Объявляю делегат в пространстве имен - using System;
                                        //Сохраним наш делегат как обыкновенную переменную (в ней будет храниться функия которую мы передадим).
                                        //Action- встроенный делегат. Есть еще встроен. делегат Func<>. 
                                        //СВОЙСТВО Делегата. После выполнения функции в которую мы передали делегата, можно ПОЗЖЕ, в определенном месте кода, выполниться сохраненный делегат.
                                        //СВОЙСТВО Делегата. Может вызывать закрытую функцию из другого класса

    public virtual void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _levelGrid = _unit.GetLevelGrid();
    }
    protected virtual void Awake() // protected virtual- обозначает что можно переопределить в дочерних классах
    {

    }

    protected virtual void Start()
    {

    }
    /// <summary>
    /// Установим PlacedObjectTypeWithActionSO для данного действия (нужен для настройки полей).
    /// </summary>  
    /// <remarks>
    /// для MoveAction он будет null
    /// </remarks>
    public virtual void SetPlacedObjectTypeWithActionSO(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO)  
    {
        _placedObjectTypeWithActionSO = placedObjectTypeWithActionSO;
    }
    public abstract string GetActionName(); // Вернуть имя действия // abstract - вынуждает реализовывать в каждом подклассе и в базовом должно иметь пустое тело.

    public abstract string GetToolTip(); // Вернкть всплывающую подсказку

    public abstract int GetMaxActionDistance(); // Вернуть Дистанцию действия
    public abstract void TakeAction(GridPositionXZ gridPosition, Action onActionComplete); //Generic Применить Действие (Действовать) В аргумент передаем сеточную позицию под курсором и делегат onActionComplete (При Завершении Действия, в нашем случае это ClearBusy() // Очистить занятость или стать свободным - активировать кнопки UI ) 
    
    /// <summary>
    /// Получить Список Допустимых Сеточных Позиция для Действий
    /// </summary>
    public abstract List<GridPositionXZ> GetValidActionGridPositionList(); 

    public virtual bool IsValidActionGridPosition(GridPositionXZ gridPosition) //(Проверяем) Является ли Сеточная позиция Допустимой для Действия //Сделаем virtual- если понадобиться переопределить где нибудь
    {
        List<GridPositionXZ> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition); // Если наша _gridPositioAnchor содержиться в листе допустимых позиций, то вернется  истина
    }

    public virtual int GetActionPointCost() // Получить Расход Очков на Действие (Стоимость действия) //Сделаем virtual- если понадобиться переопределить где нибудь 
    {
        return 1; // По умолчанию стоимость всех действий =1
    }

    protected void ActionStart(Action onActionComplete) //Старт Действия В аргумент передаем делегат onActionComplete (При Завершении Действия) // В нашем коде мы запускаемэту функцию в методе TakeAction() и от него принимаем делегат
    {
        _isActive = true;
        _onActionComplete = onActionComplete; // Сохраним переданный делегат

        OnAnyActionStart?.Invoke(this, EventArgs.Empty); // Запустим событие  // ВАЖНО событи надо запускать после того как действие началось и все настроено //ПРОВЕРИМ ЧТОБЫ МЕТОД ActionStart() ВЫПОЛНЯЛСЯ ПОСЛЕ ТОГО КАК ВСЕ НАШИ ДЕЙСТВИЯ ВЫПОЛНЕНЫ для этого претащим этот метод в конец функции
    }

    protected void ActionComplete() //Действие завершено
    {
        _isActive = false;
        _onActionComplete(); // Вызовим сохраненый делегат.(в нашем случае это ClearBusy() // Очистить занятость или стать свободным - активировать кнопки UI  

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty); // Запустим событие
    }

    public Unit GetUnit() { return _unit; }
    public PlacedObjectTypeWithActionSO GetPlacedObjectTypeWithActionSO() { return _placedObjectTypeWithActionSO; }


    public EnemyAIAction GetBestEnemyAIAction() // Получим Лучшее действие Вражеского ИИ для выбранного действия
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>(); // Список Действий Вражеского ИИ

        List<GridPositionXZ> validActionGridPositionList = GetValidActionGridPositionList(); //Получить Список Допустимых Сеточных Позиция для Действий

        foreach (GridPositionXZ gridPosition in validActionGridPositionList) // Переберем все ячейки сетки из полученнного допустимого списка
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition); // Сгенерируем действия Вражеского ИИ для этого конкретого действия в этой позиции сетки
            enemyAIActionList.Add(enemyAIAction); // Добавим сгенерируемое действие в список (Для каждого действия будет свой список)
        }

        if (enemyAIActionList.Count > 0) // Проверим что список не пустой
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue); //ОТСОРТИРУЕМ. Требуется делегат который является нашей функцией сортировки(зададим ананимно через лямбду) // (Comparison<T> Делегат - Представляет метод, сравнивающий два объекта одного типа. Вернет значение -Знаковое целое число, которое определяет относительные значения параметров(инднкс в списке) a и b),  

            // ДЛЯ ПОНИМАНИЯ // Пример записания нашего делегата без лямды (https://stackoverflow.com/questions/74245814/can-someone-help-me-understand-anonymous-functions-in-c)
            /*int EnemyAIActionComparison(EnemyAIAction a, EnemyAIAction b)
            {
                if (a.actionValue > b.actionValue) return +1;
                if (a.actionValue < b.actionValue) return -1;
                return 0; // они равны
            }
            //Поскольку не имеет значения, насколько велик результат(имеет значение только знак), вы могли бы просто написать
            int EnemyAIActionComparison(EnemyAIAction x, EnemyAIAction y)
            {
                return x.actionValue - y.actionValue;
            }*/
            // ВЕРНЕМ элемент списка с наибольшим значением actionValue
            return enemyAIActionList[0]; // Лучшим будет тот, кто имеет нулевой индекс
        }
        else
        {
            // Нет возможных действий ИИ врага
            return null;
        }

    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition); //Получить действие вражеского ИИ и ОЦЕНКУ действия для _gridPositioAnchor // abstract - вынуждает реализовывать в каждом подклассе и в базовом должно иметь пустое тело.
}
