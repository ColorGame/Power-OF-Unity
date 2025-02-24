using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ - ������ �������������� ����� �� ������� ������.
/// </summary>
public class UnitSelectAtLevelButtonUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _nameUnitText; // ��� �����
    [SerializeField] private TextMeshProUGUI _actionPointsText; // ���� ��������
    [SerializeField] private Image _selectedButtonVisualUI; // ����� �������� � ����. GameObject ��� �� ������ ��� �������� ����� ������ // � ���������� ���� �������� �����
    [SerializeField] private Image _healthBarImage; // � ���������� �������� ����� �������� "Bar"
    [SerializeField] private Image _backgroundImage; // � ���������� �������� ����� �������� "Bar"
    [SerializeField] private Image _actionImage; // � ���������� �������� ����� �������� "Bar"

    private Button _button; // ���� ������
    private Unit _unit;
    private UnitActionSystem _unitActionSystem;
    private Color _nameUnitTextColor;
    private Color _actionPointsTextColor;
    private Color _healthBarImageColor;
    private Color _backgroundImageColor;
    private Color _actionImageColor;


    public void Init(Unit unit, UnitActionSystem unitActionSystem, CameraFollow cameraFollow)
    {
        _unit = unit;
        _unitActionSystem = unitActionSystem;

        _nameUnitText.text = _unit.GetUnitTypeSO().GetName().ToUpper(); // ������� ��� � ��������� �����
        _actionPointsText.text = _unit.GetActionPointsSystem().GetActionPointsCount().ToString();
        _healthBarImage.fillAmount = _unit.GetHealthSystem().GetHealthNormalized();

        // �������� ����� ��������� ������
        _nameUnitTextColor = _nameUnitText.color;
        _actionPointsTextColor = _actionPointsText.color;
        _healthBarImageColor = _healthBarImage.color;
        _backgroundImageColor = _backgroundImage.color;
        _actionImageColor = _actionImage.color;

        _button = GetComponent<Button>();
        // �.�. ������ ��������� ����������� �� � ������� ����������� � ������� � �� � ����������
        //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        _button.onClick.AddListener(() =>
        {
            _unitActionSystem.SetSelectedUnit(_unit, _unit.GetAction<MoveAction>()); // ������� ����� ���������� � ���������� ��������
            cameraFollow.SetTargetUnit(_unit);
        });

        _unit.GetHealthSystem().OnDamageAndHealing += Unit_OnDamageAndHealing; // ���� � ����� ����� ���������� �������� �� �������
        _unit.GetActionPointsSystem().OnActionPointsChanged += Unit_OnActionPointsChanged; // ���� � ����� ����� ���������� ���� �������� �� �������
    }

    private void Unit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnDamageAndHealing(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    public void UpdateSelectedVisual(Unit selectedUnit) // (���������� �������) ��������� � ���������� ������������ ������.(���������� �������� ��� ������ ������ �������� ��������)
    {       
        _selectedButtonVisualUI.enabled = (selectedUnit == _unit);   // �������� ����� ���� ��� ��� ���� // ���� �� ��������� �� ������� false � ����� �����������       
    }
    private void UpdateActionPoints()
    {
        _actionPointsText.text = _unit.GetActionPointsSystem().GetActionPointsCount().ToString();
    }
    private void UpdateHealthBar()
    {
        _healthBarImage.fillAmount = _unit.GetHealthSystem().GetHealthNormalized();
    }

    //3//{ ������ ������ ������ ������ ����� ����� ���������
    public void HandleStateButton(bool isBusy) // ���������� ��������� ������
    {
        if (isBusy) // ���� �����
        {
            InteractableDesabled(); // ��������� ��������������
        }
        else
        {
            InteractableEnable(); // �������� ��������������
        }
    }

    private void InteractableEnable() // �������� ��������������
    {
        _button.interactable = true;
        // ����������� ������������ �����
        _nameUnitText.color = _nameUnitTextColor;
        _actionPointsText.color = _actionPointsTextColor;
        _healthBarImage.color = _healthBarImageColor;
        _backgroundImage.color = _backgroundImageColor;
        _actionImage.color = _actionImageColor;

        Unit selectedUnit = _unitActionSystem.GetSelectedUnit(); // ���������� ����
        UpdateSelectedVisual(selectedUnit); // ������� ����������� ����� ������ � ����������� �� ���������� �����
    }

    private void InteractableDesabled() // ��������� �������������� // ������ ����������� �� �������� � ������ ����(������������� � ���������� color  Desabled)
    {
        _button.interactable = false;

        Color nameUnitTextColor = _nameUnitTextColor; // �������� � ��������� ���������� ���� ������
        Color actionPointsTextColor = _actionPointsTextColor;
        Color healthBarImageColor = _healthBarImageColor;
        Color backgroundImageColor = _backgroundImageColor;
        Color actionImageColor = _actionImageColor;


        nameUnitTextColor.a = 0.1f; // ������� �������� ����� ������
        actionPointsTextColor.a = 0.1f;
        healthBarImageColor.a = 0.1f;
        backgroundImageColor.a = 0.1f;
        actionImageColor.a = 0.1f;

        _nameUnitText.color = nameUnitTextColor; // ������� ������� ���� ����� (���� ����������)
        _actionPointsText.color = actionPointsTextColor;
        _healthBarImage.color = healthBarImageColor;
        _backgroundImage.color = backgroundImageColor;
        _actionImage.color = actionImageColor;

        _selectedButtonVisualUI.enabled = (false); //�������� �����
    }   
    //3//}

    private void OnDestroy()
    {
        if (_unit != null)
        {
            _unit.GetHealthSystem().OnDamageAndHealing -= Unit_OnDamageAndHealing;
            _unit.GetActionPointsSystem().OnActionPointsChanged -= Unit_OnActionPointsChanged;
        }
    }
}
