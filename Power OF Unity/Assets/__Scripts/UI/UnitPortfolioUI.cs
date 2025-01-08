using System;
using TMPro;
using UnityEngine;

public class UnitPortfolioUI : MonoBehaviour, IToggleActivity
{
    [Header("Здоровье")]
    [SerializeField] private Transform _healthBar;
    [SerializeField] private TextMeshProUGUI _healthNumberText;
    [Header("Ед. времени")]
    [SerializeField] private Transform _actionPointsBar;
    [SerializeField] private TextMeshProUGUI _actionPointsNumberText;
    [Header("Сила")]
    [SerializeField] private Transform _powerBar;
    [SerializeField] private TextMeshProUGUI _powerNumberText;
    [Header("Точность")]
    [SerializeField] private Transform _accuracyBar;
    [SerializeField] private TextMeshProUGUI _accuracyNumberText;
    [Header("Количество завершенных миссий и убитых врагов")]
    [SerializeField] private TextMeshProUGUI _completedMissionsCountText;
    [SerializeField] private TextMeshProUGUI _killedEnemiesCountText;
    [Header("Контейнер для создания аватара")]
    [SerializeField] private Transform _avatarContainer;
    [Header("Имя юнита")]
    [SerializeField] private TextMeshProUGUI _unitNameText;

    private UnitManager _unitManager;
    private Canvas _canvas;
    private float _maxValue = 100f;
    private int _speed = 10;
    private Unit _selectedUnit;

    private Vector3 _targetHealthBarScale;
    private Vector3 _targetActionPointsBarScale;
    private Vector3 _targetPowerBarScale;
    private Vector3 _targetAccuracyBarScale;

    private float _updateTimer;
    private float _updateTimerMax = 3f;

    public void Init(UnitManager unitManager)
    {
        _unitManager = unitManager;

        Setup();
    }

    private void Setup()
    {
        _canvas = GetComponentInParent<Canvas>(true);
        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;

        _selectedUnit = _unitManager.GetSelectedUnit();
        UpdatePortfolio();
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit newSelectedUnit)
    {
        _selectedUnit = newSelectedUnit;
        UpdatePortfolio();
    }

    public void SetActive(bool active)
    {
        _canvas.enabled = active;
    }
   
    private void ClearPortfolio()
    {
        _unitNameText.text = "";

        foreach (Transform attachTransform in _avatarContainer) // Очистим контейнер
        {
            Destroy(attachTransform.gameObject);
        }

        _healthNumberText.text = "0";
        _targetHealthBarScale = new Vector3(0, 1f, 1f);

        _actionPointsNumberText.text = "0";
        _targetActionPointsBarScale = new Vector3(0, 1f, 1f);

        _powerNumberText.text = "0";
        _targetPowerBarScale = new Vector3(0, 1f, 1f);

        _accuracyNumberText.text = "0";
        _targetAccuracyBarScale = new Vector3(0, 1f, 1f);

        _completedMissionsCountText.text = "0";
        _killedEnemiesCountText.text = "0";
        _updateTimer = _updateTimerMax;
    }

    private void Update()
    {
        _updateTimer -= Time.deltaTime; // Запустим таймер для обновления

        if (_updateTimer <= 0)
        {
            return;
        }

        UpdateScaleBar(_healthBar, _targetHealthBarScale);
        UpdateScaleBar(_actionPointsBar, _targetActionPointsBarScale);
        UpdateScaleBar(_powerBar, _targetPowerBarScale);
        UpdateScaleBar(_accuracyBar, _targetAccuracyBarScale);
    }

    private void UpdatePortfolio()
    {
        if (_selectedUnit != null)
        {
            UpdateName();
            UpdateAvatar();
            UpdateNubberSetTargetScaleBar();
            _updateTimer = _updateTimerMax;
        }
        else
        {
            ClearPortfolio();
        }
    }

    private void UpdateName()
    {
        _unitNameText.text = _selectedUnit.GetUnitTypeSO().GetName();
    }
    private void UpdateAvatar()
    {
        foreach (Transform attachTransform in _avatarContainer) // Очистим контейнер
        {
            Destroy(attachTransform.gameObject);
        }
        Transform avatarTransform = Instantiate(_selectedUnit.GetUnitTypeSO<UnitFriendSO>().GetUnitAvatarPortfolioViewPrefab(), _avatarContainer);
        avatarTransform.localScale /= _canvas.scaleFactor; //Установим масштаб аватара - Разделим текущий масштаб на масштаб родительского канваса
    }
    /// <summary>
    /// Обновим цифры и установим целевой масштаб панели
    /// </summary>
    private void UpdateNubberSetTargetScaleBar()
    {
        int healthFull = _selectedUnit.GetHealthSystem().GetHealthFull();
        _healthNumberText.text = healthFull.ToString();
        _targetHealthBarScale = new Vector3(healthFull / _maxValue, 1f, 1f);

        int actionPointsFul = _selectedUnit.GetActionPointsSystem().GetActionPointsCountFull();
        _actionPointsNumberText.text = actionPointsFul.ToString();
        _targetActionPointsBarScale = new Vector3(actionPointsFul / _maxValue, 1f, 1f);

        int powerFull = _selectedUnit.GetUnitPowerSystem().GetPower();
        _powerNumberText.text = powerFull.ToString();
        _targetPowerBarScale = new Vector3(powerFull / _maxValue, 1f, 1f);

        int accuracyFull = _selectedUnit.GetUnitAccuracySystem().GetAccuracy();
        _accuracyNumberText.text = accuracyFull.ToString();
        _targetAccuracyBarScale = new Vector3(accuracyFull / _maxValue, 1f, 1f);

        _completedMissionsCountText.text = _selectedUnit.GetCompletedMissionsCount().ToString();
        _killedEnemiesCountText.text = _selectedUnit.GetKilledEnemiesCount().ToString();
    }

    private void UpdateScaleBar(Transform barTransform, Vector3 targetScale)
    {
        barTransform.localScale = Vector3.Lerp(barTransform.localScale, targetScale, Time.deltaTime * _speed);
    }

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }

   
}
