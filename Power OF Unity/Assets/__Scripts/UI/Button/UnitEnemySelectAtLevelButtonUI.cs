using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Кнопка - выбора вражеского юнита на игровом уровне.
/// </summary>
public class UnitEnemySelectAtLevelButtonUI : MonoBehaviour // Кнопка врага
{
    private Button _button; // Сама кнопка     
    private Unit _enemyUnit;


    public void Init(Unit unit, CameraFollow cameraFollow)
    {
        _enemyUnit = unit;

        _button = GetComponent<Button>();
        // т.к. кнопки создаются динамически то и события настраиваем в скрипте а не в инспекторе
        //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        _button.onClick.AddListener(() =>
        {
            cameraFollow.SetTargetUnit(_enemyUnit);
        });
    }


}
