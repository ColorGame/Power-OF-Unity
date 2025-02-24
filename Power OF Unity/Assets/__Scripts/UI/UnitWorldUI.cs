using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
///������� ��������� ����� (����� ��������). ����� � canvas �� �����
/// </summary>
public class UnitWorldUI : MonoBehaviour, ISetupForSpawn
{
    [SerializeField] private TextMeshProUGUI _actionPointsText; // �������� ���� UI
    [SerializeField] private TextMeshProUGUI _hitPercentText; // �������� ������� ���������
    [SerializeField] private TextMeshProUGUI _healthPointsText; // �������� ����� ��������
    [SerializeField] private Image _aimImage; // �������� ������ �������
    [SerializeField] private Image _stunnedImage; // �������� ������ ���������
    [SerializeField] private Image _healthBarImage; // � ���������� �������� ����� �������� "Bar"
    
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


        _actionPointsSystem.OnActionPointsChanged += Unit_OnActionPointsChanged; //  ��������� ����� ��������
        _healthSystem.OnDamageAndHealing += HealthSystem_OnDamageAndHealing; // ���������� �� ������� ������� ����������� ��� ���������.
        _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // ���������� ��������� �������� ��������
        _turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged; //���������� ��� �������
        
        UpdateActionPointsText();
        UpdateHealthBar();
        HideHitPercent();
        UpdateStunnedState();
    }


    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateHitPercentText(); // ���� �������� ������ ������� ��������� �.�. � ���� ���� ������� ����� �������� ���� ���������
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) // ���������� ��������� �������� ��������
    {
        // ����������, ����� �� ���������� ������� ��������� � ��������
        HideHitPercent();
        UpdateHitPercentText();
    }

    private void UpdateHitPercentText() // ���������� ������ ������� ���������
    {
        BaseAction baseAction = _unitActionSystem.GetSelectedAction();

        switch (baseAction)
        {
            case ShootAction shootAction:
                // �������� �������� �������

                Unit selectedUnit = _unitActionSystem.GetSelectedUnit();

                if (_unit.GetType() != selectedUnit.GetType())
                {
                    // ���� ���� � �������� � ������ ������

                    if (shootAction.IsValidActionGridPosition(_unit.GetGridPosition())) // ���� ���� ���� ������ � ������ ������ �� ������� � ���� ��������
                    {
                        // ��� ���� ������� ��� �� ���������� ������� ��� ��������
                        ShowHitPercent(shootAction.GetHitPercent(_unit));
                    }
                }
                break;
        }
    }
    private void UpdateActionPointsText() // ���������� ������ ����� ��������
    {
        _actionPointsText.text = _actionPointsSystem.GetActionPointsCount().ToString(); // ������ ���� �������� ����� ����������� � ������ � ��������� � ����� ������� ������������ ��� ������
    }

    private void Unit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
        UpdateStunnedState();
    }
        
    private void UpdateHealthBar() // ���������� ����� ��������
    {
        _healthBarImage.fillAmount = _healthSystem.GetHealthNormalized();
        _healthPointsText.text = _healthSystem.GetHealth().ToString();

    }
    private void HealthSystem_OnDamageAndHealing(object sender, EventArgs e) // ��� ����������� ������� ������� ����� �����
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
