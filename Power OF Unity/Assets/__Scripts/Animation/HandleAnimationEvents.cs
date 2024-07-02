using System;
using UnityEngine;
/// <summary>
/// Обработчик Анимационных событий. На Animation Event НЕВОЗМОЖНО подписаться через код, поэтому создаем дублирующее событие и подписываемя на него 
/// </summary>
/// <remarks>
/// Прикрепить рядом с Animator 
/// </remarks>
public class HandleAnimationEvents : MonoBehaviour 
{
    public event EventHandler OnAnimationTossGrenadeEventStarted;  // В анимации "Бросок гранаты" стартовало событие  (в этот момент будем сосздавать гранату) // Это промежуточное событие между AnimationEvent и GrenadyAction 
    public event EventHandler OnPrayingStendUpEventStarted; // В анимации "Молящийся Встает" стартовало событие (в этот момент будем сосздавать частички исцеления) // Это промежуточное событие между AnimationEvent и HealAction 
    
    
    private void InstantiateHealFXPrefab() // Вызываю в AnimationEvent на анимации молитвы StendUp
    {
        OnPrayingStendUpEventStarted?.Invoke(this, EventArgs.Empty);
       
    }

    private void StartIntermediateEvent() // Вызываю в AnimationEvent на анимации бросания гранаты
    {
        OnAnimationTossGrenadeEventStarted?.Invoke(this, EventArgs.Empty); // Запустим событие В анимации "Бросок гранаты" стартовало событие (подписчик GrenadyAction)           
    }   


}
