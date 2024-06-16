using System;
using UnityEngine;
/// <summary>
/// Обработчик Анимационных событий в Animation Event, т.к. напрямую через код это сделать НЕВОЗМОЖНО
/// </summary>
/// <remarks>
/// Прикрепить рядом с Animator
/// </remarks>
public class HandleAnimationEvents : MonoBehaviour 
{
    public event EventHandler OnAnimationTossGrenadeEventStarted;     // Действие В анимации "Бросок гранаты" стартовало событие  (в этот момент будем сосздавать гранату) // Это промежуточное событие между AnimationEvent и GrenadyAction в нем будем создовать гранату

    private Unit _unit;
    private Unit _targetUnit;
    private HealAction _healAction;

    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
    }

    private void Start()
    {       
        if (_unit != null) // Если юнит существует
        {
            _healAction = _unit.GetAction<HealAction>();// Попробуем на Юните получить компонент HealAction и если получиться сохраним в healAction

            _healAction.OnHealActionStarted += HealAction_OnHealActionStarted; // Подпишемся на событие           
        }
    }

    private void HealAction_OnHealActionStarted(object sender, Unit unit)
    {
        _targetUnit = unit;
    }


    
    private void InstantiateHealFXPrefab() // Вызываю в AnimationEvent на анимации молитвы StendUp
    {       
        Instantiate(GameAssets.Instance.healFXPrefab, _targetUnit.GetWorldPosition(), Quaternion.LookRotation(Vector3.up)); // Создадим префаб частиц для юнита которого исцеляем (Не забудь в инспекторе включить у частиц Stop Action - Destroy)
    }

    private void StartIntermediateEvent() // Вызываю в AnimationEvent на анимации бросания гранаты
    {
        OnAnimationTossGrenadeEventStarted?.Invoke(this, EventArgs.Empty); // Запустим событие В анимации "Бросок гранаты" стартовало событие (подписчик GrenadyAction)           
    }

    private void OnDestroy()
    {
        if (_healAction != null)
        {
            _healAction.OnHealActionStarted -= HealAction_OnHealActionStarted;// Отпишемя от события чтобы не вызывались функции в удаленных объектах.
        }
    }


}
