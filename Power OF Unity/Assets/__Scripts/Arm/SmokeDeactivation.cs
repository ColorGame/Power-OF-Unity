using UnityEngine;
/// <summary>
/// Деактивация дыма от гранаты после нескольких ХОДОВ
/// </summary>
/// <remarks>
/// Должна лежать на префаде частиц дыма
/// </remarks>
public class SmokeDeactivation : MonoBehaviour 
{

    private int _startTurnNumber; // Номер очереди (хода) при старте 
    private int _currentTurnNumber; // Текущий номер очереди (хода) 
    private TurnSystem _turnSystem;

    private ParticleSystem _particleSystem;
    private CoverSmokeObject _coverSmokeObject; // Объект укрытия
    private float _rateOverTime = 50;// Скорость, с которой излучатель порождает новые частицы с течением времени (по умолчанию 200).
    private int _countTurnLowerEffect = 4; // Количество ходов для УМЕНЬШЕНИЯ эффекта
    private int _countTurnDestroyObjacte = 6; // Количество ходов для УНИЧТОЖЕНИЯ объекта.

    public void Init(TurnSystem turnSystem)
    {
        _turnSystem = turnSystem;

        _particleSystem = GetComponent<ParticleSystem>();
        _coverSmokeObject = GetComponent<CoverSmokeObject>();

        _coverSmokeObject.SetCoverSmokeType(CoverSmokeObject.CoverSmokeType.SmokeFull); // Установим укрытие дыма полным
        _startTurnNumber = _turnSystem.GetTurnNumber(); // Получим номер хода

        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; // Подпиш. на событие Ход Изменен
    }   

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        _currentTurnNumber = _turnSystem.GetTurnNumber(); // Получим ТЕКУЩИЙ номер хода;

        if (_currentTurnNumber - _startTurnNumber == _countTurnLowerEffect)
        {
            // на 4 ходу эффективность и защита падает на 50%
            var emission =  _particleSystem.emission;
            emission.rateOverTime = _rateOverTime; // Уменьшим количество создаваемых частиц
            _coverSmokeObject.SetCoverSmokeType(CoverSmokeObject.CoverSmokeType.SmokeHalf); // Установим укрытие дыма На половину
        }

        if (_currentTurnNumber - _startTurnNumber == _countTurnDestroyObjacte)
        {
            // на 6 ходу уничтожим дым
            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
