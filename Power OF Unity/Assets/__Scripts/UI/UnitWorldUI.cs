using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
///Мироврй интерфейс юнита (шкала здоровья). Лежит в canvas на юните
/// </summary>
public class UnitWorldUI : MonoBehaviour, ISetupForSpawn
{
    [SerializeField] private TextMeshProUGUI _actionPointsText; // Закинуть текс UI
    [SerializeField] private TextMeshProUGUI _hitPercentText; // Закинуть процент попадания
    [SerializeField] private TextMeshProUGUI _healthPointsText; // Закинуть текст здоровья
    [SerializeField] private Image _aimImage; // закинуть иконку прицела
    [SerializeField] private Image _stunnedImage; // закинуть иконку ОГЛУШЕНИЯ
    [SerializeField] private Image _healthBarImage; // в инспекторе закинуть шкалу здоровья "Bar"
    
    private Unit _unit;
    private HealthSystem _healthSystem;
    private UnitActionPoints _actionPointsSystem;
    private UnitActionSystem _unitActionSystem;
    private TurnSystem _turnSystem;

    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _unitActionSystem= _unit.GetUnitActionSystem();
        _turnSystem = _unit.GetTurnSystem();
        _healthSystem = _unit.GetHealthSystem();
        _actionPointsSystem = _unit.GetActionPointsSystem();


        _actionPointsSystem.OnActionPointsChanged += Unit_OnActionPointsChanged; //  изминение очков действий
        _healthSystem.OnDamageAndHealing += HealthSystem_OnDamageAndHealing; // Подпишемся на событие Получил повреждение или Вылечился.
        _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // Подпишемся Выбранное Действие Изменено
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; //Подпишемся Ход Изменен
        
        UpdateActionPointsText();
        UpdateHealthBar();
        HideHitPercent();
        UpdateStunnedState();
    }


    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateHitPercentText(); // Надо обновить текста Процент попадания т.к. в ЭТОМ ходе УКРЫТИЕ могло поменять свое состаяние
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) // Подпишемся Выбранное Действие Изменено
    {
        // Посмотрите, нужно ли показывать процент попадания в действии
        HideHitPercent();
        UpdateHitPercentText();
    }

    private void UpdateHitPercentText() // Обнавления текста Процент попадания
    {
        BaseAction baseAction = _unitActionSystem.GetSelectedAction();

        switch (baseAction)
        {
            case ShootAction shootAction:
                // Действие выстрела активно

                Unit selectedUnit = _unitActionSystem.GetSelectedUnit();

                if (_unit.GetType() != selectedUnit.GetType())
                {
                    // Этот ЮНИТ и Активный в разных групах

                    if (shootAction.IsValidActionGridPosition(_unit.GetGridPosition())) // Если этот юнит входит в список юнитов по которым я могу стрелять
                    {
                        // Это юнит выводит нас на правильную позицию для стрельбы
                        ShowHitPercent(shootAction.GetHitPercent(_unit));
                    }
                }
                break;
        }
    }
    private void UpdateActionPointsText() // Обнавления текста Очков Действия
    {
        _actionPointsText.text = _actionPointsSystem.GetActionPointsCount().ToString(); // Вернем очки действия юнита преобразуеи в строку и передадим в текст который отображается над юнитом
    }

    private void Unit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
        UpdateStunnedState();
    }
        
    private void UpdateHealthBar() // Обновления шкалы здоровья
    {
        _healthBarImage.fillAmount = _healthSystem.GetHealthNormalized();
        _healthPointsText.text = _healthSystem.GetHealth().ToString();

    }
    private void HealthSystem_OnDamageAndHealing(object sender, EventArgs e) // при наступления события обновим шкалу жизни
    {
        UpdateHealthBar();
    }

    private void ShowHitPercent(float hitChance)
    {
        _hitPercentText.enabled =true;
        _hitPercentText.text = Mathf.Round(hitChance * 100f) + "%";
        _aimImage.enabled = true;
    }

    private void HideHitPercent()
    {
        _hitPercentText.enabled = false;
        _aimImage.enabled = false;
    }

    private void UpdateStunnedState()
    {
        _stunnedImage.enabled = _unit.GetActionPointsSystem().GetStunned();
    }
        

    private void OnDestroy()
    {
        _actionPointsSystem.OnActionPointsChanged -= Unit_OnActionPointsChanged; 
        _healthSystem.OnDamageAndHealing -= HealthSystem_OnDamageAndHealing; 
        _unitActionSystem.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        _turnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
