using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MoveAction : BaseAction // Действие перемещения НАСЛЕДУЕТ класс BaseAction // ВЫделим в отдельный класс // Лежит на каждом юните
{
    public static event EventHandler<Unit> OnAnyUnitPathComplete; // У любого Юнита Вычислен Путь // static - обозначает что event будет существовать для всего класса не зависимо от того скольго у нас созданно Юнитов.
                                                                  // Поэтому для прослушивания этого события слушателю не нужна ссылка на какую-либо конкретную единицу, они могут получить доступ к событию через класс, который затем запускает одно и то же событие для каждой единицы. 

    public event EventHandler OnStartMoving; // Начал двигаться (когда юнит начнет движение мы запустим событие Event)
    public event EventHandler OnStopMoving; // Прекратил движение (когда юнит законсит движение мы запустим событие Event)  
    public event EventHandler<OnChangeFloorsStartedEventArgs> OnChangedFloorsStarted; // Начали менять этажи 
    public struct OnChangeFloorsStartedEventArgs // Расширим класс событий, чтобы в аргументе события передать Сеточную позицию Юнита и Целевой позиции
    {
        public GridPositionXZ unitGridPosition; // Откуда прыгаем
        public GridPositionXZ targetGridPosition; // КУда прыгаем
    }

    private int _moveDistance; // Максимальная дистанция движения в сетке
      
    private int _currentPositionIndex; // Текущая Позиция Индекс
    private bool _isStartChangingFloors = true; // Начал менять этаж
    private float _differentFloorsTeleportTimer; // Таймер телепортации на разные этажи
    private float _differentFloorsTeleportTimerMax = .5f; // Максимальный таймер телепортации на разные этажи (это время воспроизведения анимации прыжка или падения)
    private List<GridPositionXZ> _validGridPositionList = new List<GridPositionXZ>(); // Список Допустимых Сеточных Позиция для Действий

    /// <summary>
    /// Словарь - допустимые позиции сетки и узлы пути <br/>
    /// PathNode - содержит инфу о пути
    /// </summary>
    private NativeParallelHashMap<int3, PathNode> _validGridPositionPathNodeDict; //Словарь. КЛЮЧ - допустимые сеточные позиции. ЗНАЧЕНИЕ - узел для этой сеточной позиции
    /// <summary>
    /// Позиция конца пути
    /// </summary>
    private int3 _gridPositionEndPath; 
    private UnitActionSystem _unitActionSystem;
    private PathfindingProvider _pathfindingProvider;

    public override void SetupForSpawn(Unit unit)
    {
        base.SetupForSpawn(unit);
        _unitActionSystem = _unit.GetUnitActionSystem();
        _pathfindingProvider = _unit.GetPathfindingProvider();
        _moveDistance = _unit.GetUnitTypeSO().GetBasicMoveDistance();

        _pathfindingProvider.OnPathfindingComplete += PathfindingProvider_OnPathfindingComplete; //подпишемся путь вычислен
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;  //подпишемся Выбранный Юнит Изменен
    }

    private void PathfindingProvider_OnPathfindingComplete(object sender, NativeParallelHashMap<int3, PathNode> validGridPositionPathNodeDict)
    {
        if (_unitActionSystem.GetSelectedUnit() == _unit) // Если выбран этот юнит
        {
            _validGridPositionPathNodeDict = validGridPositionPathNodeDict;
           
            foreach (var collection in validGridPositionPathNodeDict)
            {
                _validGridPositionList.Add(new GridPositionXZ(collection.Key));
            }
        }
    }
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        if (selectedUnit == _unit) // Если выбран этот юнит
        {
            _validGridPositionList.Clear(); // Очистим сисок путей
        }
    }

    private void Update()
    {
        if (!_isActive) // Если не активны то ...
        {
            return; // выходим и игнорируем код ниже
        }

        if (!_pathfindingProvider.IsPathfindingComplete())//Если путь не расчитан 
        {
            return; // выходим и игнорируем код ниже            
        }

        // Буду двигаться по списку ячеек из pathWorldPositionList, каждая следующая ячейка будет targetPosition
        Vector3 targetPosition = _validGridPositionPathNodeDict[_gridPositionEndPath].pathWorldPositionList[_currentPositionIndex]; // Целевой позицией будет позиция из листа с заданным индексом

        GridPositionXZ targetGridPosition = _unit.GetLevelGrid().GetGridPosition(targetPosition); // Получим сеточную позицию Целевой позиции
        GridPositionXZ unitGridPosition = _unit.GetLevelGrid().GetGridPosition(transform.position); // Получим сеточную позицию Юнита  
       
        if (targetGridPosition.floor != unitGridPosition.floor)// Если этаж Целевой позииции не совпадает с этажом Юнита то ...      
        {
            if (_isStartChangingFloors)// Если только начали менять этаж то настроим таймер и запустим событие
            {               
                _isStartChangingFloors = false;
                _differentFloorsTeleportTimer = _differentFloorsTeleportTimerMax;

                OnChangedFloorsStarted?.Invoke(this, new OnChangeFloorsStartedEventArgs // Запустим события и передадим сеточные позиции откуда и куда прыгаем
                {
                    unitGridPosition = unitGridPosition,
                    targetGridPosition = targetGridPosition,
                });
            }

            // Логика остановки и телепортации
            // При подходе к ячейки, с которой юнит будет телепортироваться, необходимо что бы он смотрел в сторону прыжка но только по горизонтали (Не смотрел вверх или вниз)
            Vector3 targetSameFloorPosition = targetPosition; // Целевая позиция этого же Этажа = Целевой позици
            targetSameFloorPosition.y = transform.position.y; // Изменим позицию по оси У как у игрока

            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized; // Направление поворота

            float rotateSpeed = 10f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateDirection), Time.deltaTime * rotateSpeed);

            _differentFloorsTeleportTimer -= Time.deltaTime; //ЗАПУСТИМ Таймер телепортации на разные этажи
            if (_differentFloorsTeleportTimer < 0f) // По истечению таймера // Переключим переключатель этажей и Телепортируемся в целевое положение (а также в момент отсчета таймера будет происходить анимация прыжка)
            {
                _isStartChangingFloors = true; //Сбросим параметр старта смены этажей
                transform.position = targetPosition;
                _unit.UpdateGridPosition();
            }
        }
        else
        {
            // Обычная логика перемещения

            Vector3 moveDirection = (targetPosition - transform.position).normalized; // Направление движения, еденичный вектор

            float rotateSpeed = 10f; //Чем больше тем быстрее
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotateSpeed); // поворт юнита. ЗАМЕНИЛ Lerp на - Slerp Сферически интерполирует между кватернионами a и b по соотношению t. Параметр t ограничен диапазоном [0, 1]. Используйте это для создания поворота, который плавно интерполирует между первым кватернионом a и вторым кватернионом b на основе значения параметра t. Если значение параметра близко к 0, выходные данные будут близки к a, если оно близко к 1, выходные данные будут близки к b.

            float moveSpead = 4f; //НУЖНО НАСТРОИТЬ//
            transform.position += moveDirection * moveSpead * Time.deltaTime;
        }

        float stoppingDistanceSQR = 0.4f; // Дистанция остановки в степени два //НУЖНО НАСТРОИТЬ//
        if ((targetPosition-transform.position).sqrMagnitude < stoppingDistanceSQR)  // Если квадрат растояние до целевой позиции меньше чем Дистанция остановки // Мы достигли цели        
        {
            _currentPositionIndex--; // уменьшим индекс на еденицу
            _unit.UpdateGridPosition();

            if (_currentPositionIndex == 0) // Если мы дошли до конца списка тогда...
            {
                _unit.GetSoundManager().SetLoop(false);
                _unit.GetSoundManager().Stop();

                OnStopMoving?.Invoke(this, EventArgs.Empty); //Запустим событие Прекратил движение
                                                             // _singleNodeBlocker.BlockAtCurrentPosition(); // Заблокирую узел на новом месте и разблокирую предыдущий
                ActionComplete(); // Вызовим базовую функцию ДЕЙСТВИЕ ЗАВЕРШЕНО                
            }           
        }
    }


    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // Движение к целевой позиции. В аргумент передаем сеточную позицию  и делегат. Вызываю ее для передачи новой целевой позиции
    {
        _gridPositionEndPath =  gridPosition.ParseInt3();
        _currentPositionIndex = _validGridPositionPathNodeDict[_gridPositionEndPath].pathWorldPositionList.Length - 1;

        _unit.GetSoundManager().SetLoop(true);
        _unit.GetSoundManager().Play(SoundName.Move);
      
        OnStartMoving?.Invoke(this, EventArgs.Empty); // Запустим событие Начал двигаться 
        ActionStart(onActionComplete); // Вызовим базовую функцию СТАРТ ДЕЙСТВИЯ // Вызываем этот метод в конце после всех настроек т.к. в этом методе есть EVENT и он должен запускаться после всех настроек
    }

    public override bool IsValidActionGridPosition(GridPositionXZ gridPosition) //(Проверяем) Является ли Сеточная позиция Допустимой для Действия
    {
        return _pathfindingProvider.IsPathfindingComplete() ? _validGridPositionPathNodeDict.ContainsKey(gridPosition.ParseInt3()) : false;
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList()  // переопределим базовую функцию
    {
        return _validGridPositionList;
    }

    public override string GetActionName() // Присвоить базовое действие //целиком переопределим базовую функцию
    {
        return "движение";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //Получить действие вражеского ИИ // Переопределим абстрактный базовый метод
    {
        int targetCountAtPosition = _unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition); // У юнита вернем скрипт ShootAction и вызовим у него "Получить Количество Целей На Позиции"
                                                                                                           //Я думаю, что самым простым было бы просто иметь метод, который будет подсчитывать врагов в пределах определенного радиуса определенной позиции сетки. Тогда вы можете указать радиус в сериализованном поле и не связывать одно действие с другим (Move и Shoot)
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtPosition * 10 + 40, //Ячейка с самым большим количеством стреляемых целей будет в ПРИОРИТЕТЕ. Например если у вас есть позиция сетки, в которой нет стреляемых целей, и другая позиция сетки, в которой есть одна стреляемая цель, ИИ перейдет на вторую позицию сетки, поскольку значение действия основано на количестве стреляемых целей.
        };
        // ВОЗМОЖНЫЕ ВАРИАНТЫ УСЛОЖНЕНИЯ Эта логика может легко учитывать другие факторы… например, если здоровье юнита составляет менее 20%, юнит может пожелать рассмотреть возможность перехода на плитку, на которой НЕТ стреляемых целей.
        // Вы могли бы назначить дополнительный вес плиткам со стреляемыми целями, у которых меньше здоровья, чем плиткам со стреляемыми целями с более высоким здоровьем…
        // Здесь есть много возможностей, помня, конечно, что добавление такой логики может увеличить время, необходимое врагам для расчета наилучших действий.
    }

    //Враги преследовали моих игроков более агрессивно.
    //https://community.gamedev.tv/t/more-aggressive-enemy/220615?_gl=1*ueppqc*_ga*NzQ2MDMzMjI4LjE2NzY3MTQ0MDc.*_ga_2C81L26GR9*MTY3OTE1NDA5Ni4zMS4xLjE2NzkxNTQ1MjYuMC4wLjA.

    public override string GetToolTip()
    {
        return "цена - " + GetActionPointCost() + "\n" +
            "дальность - " + GetMaxActionDistance();
    }

    public override int GetMaxActionDistance()
    {
        return _moveDistance;
    }



    private void OnDestroy()
    {
        _pathfindingProvider.OnPathfindingComplete -= PathfindingProvider_OnPathfindingComplete; 
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;  
    }

}
