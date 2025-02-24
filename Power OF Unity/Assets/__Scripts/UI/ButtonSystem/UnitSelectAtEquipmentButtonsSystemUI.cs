using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора  Юнита, для настройки экипировки
/// </summary>
public class UnitSelectAtEquipmentButtonsSystemUI : UnitSelectButtonsSystemUI
{
    [Header("Кнопки для переключения вкладок,\nи изображение для выделенной кнопки")]
    [SerializeField] private Button _unitsOnBarrackButtonPanel;  // Кнопка для включения панели Юнитов в БАРАКЕ
    [SerializeField] private Image _barrackButtonSelectedImage; // Изображение выделенной кнопки 
    [SerializeField] private Button _unitsOnMissionButtonPanel;  // Кнопка для включения панели Юнитов на МИССИИ
    [SerializeField] private Image _missionButtonSelectedImage; // Изображение выделенной кнопки 
    [Header("Контейнеры для переключения")]
    [SerializeField] private RectTransform _unitsOnBarrackContainer; // Контейнер для Юнитов в БАРАКЕ
    [SerializeField] private RectTransform _unitsOnMissionContainer; // Контейнер для Юнитов на МИССИИ
    [Header("Текс вкладки для переключения")]
    [SerializeField] private TextMeshProUGUI _onBarrackHeaderText; // Текс заголовка
    [SerializeField] private TextMeshProUGUI _onMissionHeaderText; // Текс заголовка

    private List<UnitSelectAtEquipmentButtonUI> _activeUnitButtonList = new();
    private List<UnitSelectAtEquipmentButtonUI> _onBarrackUnitButtonList = new();
    private List<UnitSelectAtEquipmentButtonUI> _onMissionUnitButtonList = new();

    private bool _unitChanged = false;

    protected override void Awake()
    {
        base.Awake();

        _containerButtonArray = new Transform[] { _unitsOnBarrackContainer, _unitsOnMissionContainer };
        _buttonSelectedImageArray = new Image[] { _barrackButtonSelectedImage, _missionButtonSelectedImage };
        _headerTextArray = new TextMeshProUGUI[] { _onBarrackHeaderText, _onMissionHeaderText };
    }

    protected override void Setup()
    {
        base.Setup();
        _unitManager.OnAddRemoveUnitFromLocation += UnitManager_OnAddRemoveUnitFromLocation;
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _unitsOnMissionButtonPanel.onClick.AddListener(() =>//Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowMissionTab();
        });

        _unitsOnBarrackButtonPanel.onClick.AddListener(() =>
        {
            ShowBarrackTab();
        });
    }


    public override void SetActive(bool active)
    {
        if (_isActive == active) //Если предыдущее состояние тоже то выходим
            return;

        _isActive = active;

        _canvas.enabled = active;
        if (active)
        {
            SetSelectedUnitAndShowTab();
        }
        else
        {
            _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
            HideAllContainerArray();           
        }
    }

    private void UnitManager_OnAddRemoveUnitFromLocation(object sender, EventArgs e)
    {
        _unitChanged = true;
    }


    /// <summary>
    /// Установить Выбранного юнита, и показать соответствующую выкладку(база/миссия)
    /// </summary>
    private void SetSelectedUnitAndShowTab()
    {
        Unit selectedUnit = _unitManager.UpdateSelectedUnitAndReturn();

        if (selectedUnit == null) { return; }
        switch (selectedUnit.GetLocation()) // В зависимости от локации юнита включим нужную вкладку
        {
            case Unit.Location.Barrack:
                ShowBarrackTab();
                break;
            case Unit.Location.Mission:
                ShowMissionTab();
                break;
        }
    }

    private void ShowMissionTab()
    {
        SetActiveUnitButtonList(false);// Деактивируем прошлый активный контейнер с кнопками (если он был)
        _activeUnitButtonList = _onMissionUnitButtonList;//Назначим новый активный контейнер с кнопками

        if (_unitChanged)
        {
            _unitChanged =false;
            UpdateAllContainer();
        }

        ShowContainer(_unitsOnMissionContainer);
        SetActiveUnitButtonList(true);

        ShowSelectedButton(_missionButtonSelectedImage);
        ShowSelectedHeaderText(_onMissionHeaderText);
    }
    private void ShowBarrackTab()
    {
        SetActiveUnitButtonList(false);// Деактивируем прошлый активный контейнер с кнопками (если он был)
        _activeUnitButtonList = _onBarrackUnitButtonList;//Назначим новый активный контейнер с кнопками

        if (_unitChanged)
        {
            _unitChanged = false;
            UpdateAllContainer();
        }

        ShowContainer(_unitsOnBarrackContainer);
        SetActiveUnitButtonList(true);

        ShowSelectedButton(_barrackButtonSelectedImage);
        ShowSelectedHeaderText(_onBarrackHeaderText);
    }

    /// <summary>
    /// Настроим активный контейнер с кнопками
    /// </summary>
    private void SetActiveUnitButtonList(bool active)
    {
        foreach (UnitSelectAtEquipmentButtonUI button in _activeUnitButtonList)
        {
            button.SetActive(active);
        }
    }

    protected override void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer)
    {
        if (buttonContainer == _unitsOnBarrackContainer)
            _onBarrackUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitOnBarrackList(), _unitsOnBarrackContainer);
        if (buttonContainer == _unitsOnMissionContainer)
            _onMissionUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitOnMissionList(), _unitsOnMissionContainer);
    }

    protected override void CreateSelectButtonsSystemInAllContainer()
    {
        _onBarrackUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitOnBarrackList(), _unitsOnBarrackContainer);
        _onMissionUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitOnMissionList(), _unitsOnMissionContainer);
    }

    /// <summary>
    /// Получить список созданых кнопок выбора юнитов, из переданного списка<br/>
    /// </summary>
    private List<UnitSelectAtEquipmentButtonUI> GetCreatedUnitSelectButtonsList(List<Unit> unitList, Transform containerTransform)
    {
        List<UnitSelectAtEquipmentButtonUI> unitButtonList = new();
        for (int index = 0; index < unitList.Count; index++)
        {
            UnitSelectAtEquipmentButtonUI unitSelectAtEquipmentButton = Instantiate(GameAssetsSO.Instance.unitSelectAtEquipmentButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
            unitSelectAtEquipmentButton.Init(unitList[index], _unitManager, index + 1);
            unitButtonList.Add(unitSelectAtEquipmentButton);
        }
        return unitButtonList;
    }

    private void OnDestroy()
    {
        if (_unitManager != null)
            _unitManager.OnAddRemoveUnitFromLocation -= UnitManager_OnAddRemoveUnitFromLocation;
    }
}
