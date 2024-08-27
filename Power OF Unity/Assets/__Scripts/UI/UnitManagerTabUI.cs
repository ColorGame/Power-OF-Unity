using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;
/// <summary>
/// UI вкладки "Менеджер Юнитов". Изминение статуса юнитов(база/миссия), вербовка и увольнение
/// </summary>
public class UnitManagerTabUI : MonoBehaviour, IToggleActivity
{
    [Header("Кнопки для переключения вкладок,\nи изображение для выделенной кнопки")]
    [SerializeField] private Button _myUnitsButtonPanel;  // Кнопка для включения панели МОИ ЮНИТЫ
    [SerializeField] private Image _selectedImageMyUnitsButton; // Изображение выделенной кнопки 
    [SerializeField] private Button _hireUnitButtonPanel;  // Кнопка для включения панели НАЙМА ЮНИТОВ
    [SerializeField] private Image _selectedImageHireUnitButton; // Изображение выделенной кнопки 
    [Header("Контейнеры для переключения")]
    [SerializeField] private Transform _myUnitsContainer; // Контейнер МОИХ ЮНИТОВ
    [SerializeField] private Transform _hireUnitsContainer; // Контейнер ЮНИТОВ ДЛЯ НАЙМА
    [Header("Текс вкладки для переключения")]
    [SerializeField] private TextMeshProUGUI _myUnitsText; // Текст заголовка
    [SerializeField] private TextMeshProUGUI _hireUnitsText; // Текст заголовка
    [Header("Иконки в оглавлении таблицы")]
    [SerializeField] private Image _barrackImage;
    [SerializeField] private Image _missionImage;
    [SerializeField] private Image _priceImage;
    [Header("Кнопки НАНЯТЬ УВОЛИТЬ")]
    [SerializeField] private Transform _dismissButtonBar; // Уволить
    [SerializeField] private Button _dismissButton;
    [SerializeField] private Transform _hireButtonBar; // Нанять
    [SerializeField] private Button _hireButton;
    [Header("Панель расходов$")]
    [SerializeField] private Transform _expensesBar;
    [SerializeField] private TextMeshProUGUI _expensesText;

    private Canvas _canvas;
    private ScrollRect _scrollRect; //Компонент прокрутки кнопок    

    private UnitManager _unitManager;
    private TooltipUI _tooltipUI;



    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);
    }

    public void Init(UnitManager unitManager, TooltipUI tooltipUI)
    {
        _unitManager = unitManager;
        _tooltipUI = tooltipUI;
        Setup();
    }

    private void Setup()
    {        
        _hireUnitButtonPanel.onClick.AddListener(() =>//Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowHireUnitsTab();
        });

        _myUnitsButtonPanel.onClick.AddListener(() =>
        {
            ShowMuUnitsTab();
        });

        _hireButton.onClick.AddListener(() =>
        {
            HireUnit();
        });

        _dismissButton.onClick.AddListener(() =>
        {
            DismissUnit();
        });       
    }


    public void SetActive(bool active)
    {
        _canvas.enabled = active;

        if (active)
        {       
            ShowMuUnitsTab();
        }
        else
        {           
            _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
        }
    }
    /// <summary>
    /// Нанять
    /// </summary>
    private void HireUnit()
    {
        if (_unitManager.GetSelectedUnit() == null) 
        {
            _tooltipUI.ShowShortTooltipFollowMouse("ВЫБЕРИ ЮНИТА", new TooltipUI.TooltipTimer { timer = 0.5f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            return; // Выходим игнор. код ниже
        }
        _unitManager.HireSelectedUnit();
        UpdateHireUnitSelectButtonsSystem();
        _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }
    /// <summary>
    /// Уволить
    /// </summary>
    private void DismissUnit()
    {
        if (_unitManager.GetSelectedUnit() == null) 
        {
            _tooltipUI.ShowShortTooltipFollowMouse("ВЫБЕРИ ЮНИТА", new TooltipUI.TooltipTimer { timer = 0.5f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            return; // Выходим игнор. код ниже
        }
        _unitManager.DismissSelectedUnit();
        UpdateMuUnitSelectButtonsSystem();
        _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }


    private void ShowMuUnitsTab()
    {
        SetActiveMuUnitsTab(true);
        SetActiveHireUnitsTab(false);
        _unitManager.ClearSelectedUnit();// Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }

    private void ShowHireUnitsTab()
    {
        SetActiveHireUnitsTab(true);
        SetActiveMuUnitsTab(false);
        _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }

    private void SetActiveMuUnitsTab(bool active)
    {
        _selectedImageMyUnitsButton.enabled = active;
        _myUnitsText.enabled = active;
        _barrackImage.enabled = active;
        _missionImage.enabled = active;
        _dismissButtonBar.gameObject.SetActive(active);
        _myUnitsContainer.gameObject.SetActive(active);
        if (active)
        {
            UpdateMuUnitSelectButtonsSystem();
            _scrollRect.content = (RectTransform)_myUnitsContainer; // Установим этот контейнер как контент для прокрутки
        }
    }

    private void SetActiveHireUnitsTab(bool active)
    {
        _selectedImageHireUnitButton.enabled = active;
        _hireUnitsText.enabled = active;
        _priceImage.enabled = active;
        _hireButtonBar.gameObject.SetActive(active);
        _expensesBar.gameObject.SetActive(active);
        _hireUnitsContainer.gameObject.SetActive(active);
        if (active)
        {
            UpdateHireUnitSelectButtonsSystem();
            _scrollRect.content = (RectTransform)_hireUnitsContainer; // Установим этот контейнер как контент для прокрутки
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        }
        else
        {
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        }
    }

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        UpdatePriceText(selectedUnit);
    }
    private void UpdatePriceText(Unit unit)
    {
        if (unit == null)
        {
            _expensesText.text = "";
        }
        else
        {
            int price = unit.GetUnitTypeSO<UnitFriendSO>().GetPriceHiring();
            _expensesText.text = $"-({price})";
        }       
    }

   
    /// <summary>
    /// Обновить кнопки выбора юнитов для НАЙМА
    /// </summary>
    private void UpdateHireUnitSelectButtonsSystem()
    {
        foreach (Transform unitSelectButton in _hireUnitsContainer)
        {
            Destroy(unitSelectButton.gameObject);
        }

        List<Unit> unitHireList = _unitManager.GetHireUnitTypeSOList();// список  юнитов для найма               
        for (int index = 0; index < unitHireList.Count; index++)
        {
            CreateUnitSelectButton(unitHireList[index], _hireUnitsContainer, index + 1);
        }
        _scrollRect.verticalScrollbar.value = 1f; // переместим прокрутку панели в верх.
    }
    /// <summary>
    /// Обновить кнопки выбора моих юнитов
    /// </summary>
    private void UpdateMuUnitSelectButtonsSystem()
    {
        foreach (Transform unitSelectButton in _myUnitsContainer) // Переберем все трансформы в нашем контейнере
        {
            Destroy(unitSelectButton.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        List<Unit> unitFriendList = _unitManager.GetUnitFriendList();//Общий список  моих юнитов 
        for (int index = 0; index < unitFriendList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendList[index], _myUnitsContainer, index + 1);
        }
        _scrollRect.verticalScrollbar.value = 1f; // переместим прокрутку панели в верх.
    }

    private void CreateUnitSelectButton(Unit unit, Transform containerTransform, int index) // Создать Кнопку Размещаемых объектов и поместим в контейнер
    {
        UnitSelectAtManagementButtonUI unitSelectAtManagementButton = Instantiate(GameAssets.Instance.unitSelectAtManagementButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        unitSelectAtManagementButton.Init(unit, _unitManager, index);
    }

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
