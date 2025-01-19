using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора юнита для менеджмента. Изминение статуса юнитов(база/миссия), вербовка и увольнение
/// </summary>
public class UnitSelectForManagementButtonsSystemUI : UnitSelectButtonsSystemUI
{
    [Header("Кнопки для переключения вкладок,\nи изображение для выделенной кнопки")]
    [SerializeField] private Button _myUnitsButtonBar;  // Кнопка для включения панели МОИ ЮНИТЫ
    [SerializeField] private Image _selectedImageMyUnitsButton; // Изображение выделенной кнопки 
    [SerializeField] private Button _hireUnitButtonBar;  // Кнопка для включения панели НАЙМА ЮНИТОВ
    [SerializeField] private Image _selectedImageHireUnitButton; // Изображение выделенной кнопки 
    [Header("Контейнеры для переключения")]
    [SerializeField] private RectTransform _myUnitsContainer; // Контейнер МОИХ ЮНИТОВ
    [SerializeField] private RectTransform _hireUnitsContainer; // Контейнер ЮНИТОВ ДЛЯ НАЙМА
    [Header("Текс ОГЛАВЛЕНИЯ вкладки для переключения")]
    [SerializeField] private TextMeshProUGUI _myUnitsText; // Текст заголовка
    [SerializeField] private TextMeshProUGUI _hireUnitsText; // Текст заголовка
    [Header("Иконки в оглавлении таблицы для переключения\nи настройкм всплыв. подсказки")]
    [SerializeField] private MouseEnterExitEventsUI _healthImage;
    [SerializeField] private MouseEnterExitEventsUI _actionPointsImage;
    [SerializeField] private MouseEnterExitEventsUI _powerImage;
    [SerializeField] private MouseEnterExitEventsUI _accuracyImage;
    [SerializeField] private Image _barrackImage;
    [SerializeField] private Image _missionImage;
    [SerializeField] private Image _priceImage;
    [Header("Кнопки НАНЯТЬ УВОЛИТЬ")]
    [SerializeField] private Transform _dismissButtonBar; // Уволить
    [SerializeField] private Button _dismissButton;
    [SerializeField] private Transform _hireButtonBar; // Нанять
    [SerializeField] private Button _hireButton;
    [Header("Панель расходов$")]
    [SerializeField] private Transform _debitedBar;
    [SerializeField] private TextMeshProUGUI _debitedText;

    private Image[] _headerImageArray;
    private Transform[] _transformBarArray;

    private Image[] _barrackMissionArray;
    private Transform[] _hireButtonDebitedArray;

    private List<UnitSelectAtManagementButtonUI> _activeUnitButtonList = new();
    private List<UnitSelectAtManagementButtonUI> _myUnitButtonList = new();
    private List<UnitSelectAtManagementButtonUI> _hireUnitButtonList = new();

    protected override void Awake()
    {
        base.Awake();

        _containerButtonArray = new Transform[] { _myUnitsContainer, _hireUnitsContainer };
        _buttonSelectedImageArray = new Image[] { _selectedImageMyUnitsButton, _selectedImageHireUnitButton };
        _headerTextArray = new TextMeshProUGUI[] { _myUnitsText, _hireUnitsText };
        _headerImageArray = new Image[] { _missionImage, _barrackImage, _priceImage };
        _transformBarArray = new Transform[] { _dismissButtonBar, _hireButtonBar, _debitedBar };

        _barrackMissionArray = new Image[] { _barrackImage, _missionImage };
        _hireButtonDebitedArray = new Transform[] { _hireButtonBar, _debitedBar };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _hireUnitButtonBar.onClick.AddListener(() =>//Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowHireUnitsTab();
        });

        _myUnitsButtonBar.onClick.AddListener(() =>
        {
            ShowMuUnitsTab();
        });

        _hireButton.onClick.AddListener(() => { HireUnit(); });

        _dismissButton.onClick.AddListener(() => { DismissUnit(); });

        SetTooltip(_healthImage, "здоровье");
        SetTooltip(_actionPointsImage, "едениц времени");
        SetTooltip(_powerImage, "сила");
        SetTooltip(_accuracyImage, "точность");
        SetTooltip(_barrackImage.GetComponent<MouseEnterExitEventsUI>(), "отправить на базу");
        SetTooltip(_missionImage.GetComponent<MouseEnterExitEventsUI>(), "отправить на задание");
        SetTooltip(_priceImage.GetComponent<MouseEnterExitEventsUI>(), "стоимость найма");
    }

    private void SetTooltip(MouseEnterExitEventsUI mouseEnterExitEventsUI, string text)
    {
        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // Подпишемся на событие
        {
            _tooltipUI.ShowShortTooltipFollowMouse(text, new TooltipUI.TooltipTimer { timer = 0.8f }); // Покажем подсказку и зададим новый таймер отображения подсказки
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // При отведении мыши скроем подсказку
        };
    }

    public override void SetActive(bool active)
    {
        if (_isActive == active) //Если предыдущее состояние тоже то выходим
            return;

        _isActive = active;

        _canvas.enabled = active;
        if (active)
        {
            ShowMuUnitsTab();
        }
        else
        {
            HideAllContainerArray(); //при выходе из меню менеджмента выключим все окна
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
            //ClearActiveButtonContainer();
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
        if (!_unitManager.TryHireSelectedUnit())
        {
            _tooltipUI.ShowShortTooltipFollowMouse("Недостаточно СРЕДСТВ", new TooltipUI.TooltipTimer { timer = 0.8f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            return; // Выходим игнор. код ниже
        }

        SetActiveUnitButtonList(false);// Деактивируем активный контейнер с кнопками 
        UpdateAllContainer();
        SetActiveUnitButtonList(true);
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
        SetActiveUnitButtonList(false);// Деактивируем активный контейнер с кнопками 
        UpdateAllContainer();
        SetActiveUnitButtonList(true);
        _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }


    private void ShowMuUnitsTab()
    {
        SetActiveUnitButtonList(false);// Деактивируем прошлый активный контейнер с кнопками (если он был)
        _activeUnitButtonList = _myUnitButtonList;//Назначим новый активный контейнер с кнопками
        ShowContainer(_myUnitsContainer);
        SetActiveUnitButtonList(true);

        ShowSelectedButton(_selectedImageMyUnitsButton);
        ShowSelectedHeaderText(_myUnitsText);
        ShowHeaderImage(_barrackMissionArray);
        ShowTransformBar(_dismissButtonBar);

        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        _unitManager.ClearSelectedUnit();// Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }

    private void ShowHireUnitsTab()
    {
        SetActiveUnitButtonList(false);// Деактивируем прошлый активный контейнер с кнопками (если он был)
        _activeUnitButtonList = _hireUnitButtonList;//Назначим новый активный контейнер с кнопками
        ShowContainer(_hireUnitsContainer);
        SetActiveUnitButtonList(true);

        ShowSelectedButton(_selectedImageHireUnitButton);
        ShowSelectedHeaderText(_hireUnitsText);
        ShowHeaderImage(_priceImage);
        ShowTransformBar(_hireButtonDebitedArray);

        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
    }

    /// <summary>
    /// Настроим активный контейнер с кнопками
    /// </summary>
    private void SetActiveUnitButtonList(bool active)
    {
        foreach (UnitSelectAtManagementButtonUI button in _activeUnitButtonList)
        {
            button.SetActive(active);
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
            _debitedText.text = "";
        }
        else
        {
            uint price = unit.GetUnitTypeSO<UnitFriendSO>().GetPriceHiring();
            _debitedText.text = $"-({price.ToString("N0")})";
        }
    }

    /*  protected override void CreateSelectButtonsSystemInActiveContainer()
      {
          if (_activeContainer == _myUnitsContainer)
              GetCreatedUnitSelectButtonsList(_unitManager.GetUnitFriendList(), _myUnitsContainer);//Общий список  моих юнитов 

          if (_activeContainer == _hireUnitsContainer)
              GetCreatedUnitSelectButtonsList(_unitManager.GetHireUnitList(), _hireUnitsContainer);// список  юнитов для найма    
      }*/

    protected override void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer)
    {
        if (buttonContainer == _myUnitsContainer)
            _myUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitFriendList(), _myUnitsContainer);//Общий список  моих юнитов 

        if (buttonContainer == _hireUnitsContainer)
            _hireUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetHireUnitList(), _hireUnitsContainer);// список  юнитов для найма    
    }
    protected override void CreateSelectButtonsSystemInAllContainer()
    {
        _myUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitFriendList(), _myUnitsContainer);//Общий список  моих юнитов        
        _hireUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetHireUnitList(), _hireUnitsContainer);// список  юнитов для найма    
    }
    /// <summary>
    /// Получить список созданых кнопок выбора юнитов, из переданного списка<br/>
    /// </summary>
    private List<UnitSelectAtManagementButtonUI> GetCreatedUnitSelectButtonsList(List<Unit> unitList, Transform containerTransform)
    {
        List<UnitSelectAtManagementButtonUI> unitButtonList = new();
        for (int index = 0; index < unitList.Count; index++)
        {
            UnitSelectAtManagementButtonUI unitSelectAtManagementButton = Instantiate(GameAssetsSO.Instance.unitSelectAtManagementButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
            unitSelectAtManagementButton.Init(unitList[index], _unitManager, index + 1);
            unitButtonList.Add(unitSelectAtManagementButton);
        }
        return unitButtonList;
    }

    /// <summary>
    /// Показать переданный список изображенией заголовка
    /// </summary>
    private void ShowHeaderImage(IEnumerable<Image> headerImageEnumerable)
    {
        foreach (Image setHeaderImage in _headerImageArray) // Переберем массив 
        {
            bool contains = false; // предположим что объекта нет в искомом списке / Если найдем его в искомом списке то перезапишем флаг  
            foreach (Image headerImage in headerImageEnumerable)
            {
                if (headerImage == setHeaderImage)
                {
                    contains = true;
                    break;// Если есть совпадение выходим из цикла
                }
            }
            setHeaderImage.enabled = contains;// Если изображение есть в переданном списке то включим его
        }
    }
    /// <summary>
    /// Показать переданное изображенией заголовка
    /// </summary>
    private void ShowHeaderImage(Image headerImage)
    {
        foreach (Image setHeaderImage in _headerImageArray) // Переберем массив 
        {
            setHeaderImage.enabled = (setHeaderImage == headerImage);// Если это переданное изображение списке то включим его
        }
    }

    /// <summary>
    /// Показать переданный список трансформов
    /// </summary>
    private void ShowTransformBar(IEnumerable<Transform> transformBarEnumerable)
    {
        foreach (Transform setTransform in _transformBarArray)
        {
            bool contains = false; // предположим что объекта нет в искомом списке / Если найдем его в искомом списке то перезапишем флаг  
            foreach (Transform transform in transformBarEnumerable)
            {
                if (transform == setTransform)
                {
                    contains = true;
                    break;// Если есть совпадение выходим из цикла
                }
            }
            setTransform.gameObject.SetActive(contains);// Если setTransform есть в переданном списке то включим его
        }
    }
    /// <summary>
    /// Показать переданный трансформ
    /// </summary>
    private void ShowTransformBar(Transform transformBar)
    {
        foreach (Transform setTransform in _transformBarArray)
        {
            setTransform.gameObject.SetActive(setTransform == transformBar);// Если это переданный трансформ то включим его
        }
    }

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
