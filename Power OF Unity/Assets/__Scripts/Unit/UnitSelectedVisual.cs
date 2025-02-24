using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour, ISetupForSpawn // Визуализация выбора юнита
{
    private Unit _unit; // Юнит к которому прикриплен данный визуа.
    private MeshRenderer _meshRenderer; // Будем включать и выкл. MeshRenderer что бы скрыть или показать наш визуальный объект
    private UnitActionSystem _unitActionSystem;

    private void Awake() //Для избежания ошибок Awake() Лучше использовать только для инициализации и настроийки объектов
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _unitActionSystem = _unit.GetUnitActionSystem();

        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // подписываемся на Event из UnitActionSystem (становимся слушателями). Обозначает что мы выполняем функцию UnitActionSystem_OnSelectedUnitChanged()
                                                                                           // Будет выполняться каждый раз когда мы меняем выбранного юнита.
        Unit selectedUnit = _unitActionSystem.GetSelectedUnit();
        UpdateVisual(selectedUnit); // Что бы при старте визуал был включен только у выбранного игрока
    }
    
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit) //sender - отправитель // Подписка должна иметь туже сигнатуру что и функция отправителя OnSelectedUnitChanged
    {
        UpdateVisual(selectedUnit);
    }

    private void UpdateVisual(Unit selectedUnit) // (Обновление визуала) Включение и выключение визуализации выбора.
    {
        _meshRenderer.enabled = (selectedUnit == _unit);// Если выбранный юнит равен юниту на котором лежит этот скрипт то включим круг       
    }

    private void OnDestroy() // Встроенная функция в MonoBehaviour и вызывается при уничтожении игрового объекта
    {
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged; // Отпишемя от события чтобы не вызывались функции в удаленных объектах.(Исключение MissingReferenceException: Объект типа 'MeshRenderer' был уничтожен, но вы все еще пытаетесь получить к нему доступ.)
    }
}
