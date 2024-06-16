using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Кнопка выбора вражеского юнита
/// </summary>
public class SelectUnitEnemyButtonUI : MonoBehaviour // Кнопка врага
{
    public static event EventHandler<Unit> OnAnyEnemylyUnitButtonPressed; // Нажата любая кнопка вражественного юнита // static - обозначает что event будет существовать для всего класса не зависимо от того скольго у нас созданно Юнитов.
                                                                          // Поэтому для прослушивания этого события слушателю не нужна ссылка на какую-либо конкретную единицу, они могут получить доступ к событию через класс, который затем запускает одно и то же событие для каждой единицы. 
    [SerializeField] private Button _button; // Сама кнопка      

    private Unit _enemyUnit;


    public void Init(Unit unit)
    {
        _enemyUnit = unit;

        // т.к. кнопки создаются динамически то и события настраиваем в скрипте а не в инспекторе
        //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        _button.onClick.AddListener(() =>
        {
            OnAnyEnemylyUnitButtonPressed?.Invoke(this, _enemyUnit); // Запустим событие и передадим выделенного юнита    
        });
    }


}
