﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAction : BaseAction // Щит
{

    private float _totalSpinAmount; // Общая сумма вращения
    private int _maxSpinDistance = 1; //Максимальная дистанция

    private void Update()
    {
        if (!_isActive) // Если не активны то ...
        {
            return; // выходим и игнорируем код ниже
        }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0); // Вращаем юнита вокруг оси У

        _totalSpinAmount += spinAddAmount; // Каждый кадр добовлям угол на который мы повернули

        if (_totalSpinAmount >= 360f) // При полном повороте на 360 градусов...
        {
            ActionComplete(); // Вызовим базовую функцию ДЕЙСТВИЕ ЗАВЕРШЕНО
        }
    }


    // Переопределим TakeAction (Применить Действие (Действовать)) // Мы переименовали Spin в TakeAction и добавили в аргумент GridPositionXZ
    public override void TakeAction(GridPositionXZ gridPosition, Action onActionComplete) // (onActionComplete - по завершении действия). В аргумент будем передовать делегат Action 
                                                                                          // В данном методе добавлен аргумент который мы не используем - GridPositionXZ _gridPositioAnchor - он добавлен лишь для того чтобы соответствовать сигнатуре Базовой функции TakeAction.
                                                                                          // Есть другой способ, создать оттдельный -
                                                                                          // public class BaseParameters{} 
                                                                                          // и наследуемый в котором можно переопределить наш базовый параметр -
                                                                                          // public SpinBaseParameters : BaseParameters{}
                                                                                          // тогда запишем - public override void TakeAction(BaseParameters baseParameters ,Action onActionComplete){
                                                                                          // SpinBaseParameters spinBaseParameters = (SpinBaseParameters)baseParameters;}
    {
        _totalSpinAmount = 0f; // При вызове Spin() обнуляем сумарный поворот                               

        ActionStart(onActionComplete); // Вызовим базовую функцию СТАРТ ДЕЙСТВИЯ // Вызываем этот метод в конце после всех настроек т.к. в этом методе есть EVENT и он должен запускаться после всех настроек
    }

    public override string GetActionName() // Присвоить базовое действие //целиком переопределим базовую функцию
    {
        return "разворот";
    }

    public override List<GridPositionXZ> GetValidActionGridPositionList() // Получить Список Допустимых Сеточных Позиция для Действий // переопределим базовую функцию
                                                                          // Допустимая сеточная позиция для Действия Вращения будет ячейка где стоит юнит 
    {
        GridPositionXZ unitGridPosition = _unit.GetGridPosition(); // Получим сеточную позицию юнита 

        return new List<GridPositionXZ> // Создадим список и добавим в нее сеточную позицию юнита, а затем вернем ее
        {
            unitGridPosition
        };
    }

    public override int GetActionPointCost() // Переопределим базовую функцию // Получить Расход Очков на Действие (Стоимость действия)
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //Получить действие вражеского ИИ // Переопределим абстрактный базовый метод
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0, //Поставим низкое значение действия. Будет выполнять вращение если ничего другого сделать не может, 
        };
    }

    public override string GetToolTip()
    {
        return "цена - " + GetActionPointCost() + "\n" +
                "дальность - " + GetMaxActionDistance();
    }

    public override int GetMaxActionDistance()
    {
        return _maxSpinDistance;
    }
}
