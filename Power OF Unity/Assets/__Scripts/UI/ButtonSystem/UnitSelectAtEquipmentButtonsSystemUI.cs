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
    [SerializeField] private Transform _unitsOnBarrackContainer; // Контейнер для Юнитов в БАРАКЕ
    [SerializeField] private Transform _unitsOnMissionContainer; // Контейнер для Юнитов на МИССИИ
    [Header("Текс вкладки для переключения")]
    [SerializeField] private TextMeshProUGUI _onBarrackHeaderText; // Текс заголовка
    [SerializeField] private TextMeshProUGUI _onMissionHeaderText; // Текс заголовка
     

    protected override void Awake()
    {
        base.Awake();

        _containerArray = new Transform[] { _unitsOnBarrackContainer, _unitsOnMissionContainer };
        _buttonSelectedImageArray = new Image[] { _barrackButtonSelectedImage, _missionButtonSelectedImage };
        _headerTextArray = new TextMeshProUGUI[] { _onBarrackHeaderText, _onMissionHeaderText };
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
        _canvas.enabled = active;
        if (active)
        {
            SetSelectedUnitAndShowTab();
        }
        else
        {
            _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
            ClearActiveButtonContainer();
        }
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
        ShowAndUpdateContainer(_unitsOnMissionContainer);
        ShowSelectedButton(_missionButtonSelectedImage);
        ShowSelectedHeaderText(_onMissionHeaderText);
    }
    private void ShowBarrackTab()
    {
        ShowAndUpdateContainer(_unitsOnBarrackContainer);
        ShowSelectedButton(_barrackButtonSelectedImage);
        ShowSelectedHeaderText(_onBarrackHeaderText);
    }


    protected override void CreateSelectButtonsSystemInActiveContainer()
    {
        if (_activeContainer == _unitsOnBarrackContainer)
            CreateUnitSelectButtonsSystem(_unitManager.GetUnitFriendOnBarrackList(),_unitsOnBarrackContainer);

        if (_activeContainer == _unitsOnMissionContainer)
            CreateUnitSelectButtonsSystem(_unitManager.GetUnitFriendOnMissionList(), _unitsOnMissionContainer);
    }

    /// <summary>
    /// Создать кнопки выбора юнитов, из переданного списка в переданном контейнере
    /// </summary>
    private void CreateUnitSelectButtonsSystem(List<Unit> unitList, Transform containerTransform)
    {
        for (int index = 0; index < unitList.Count; index++)
        {           
            UnitSelectAtEquipmentButtonUI unitSelectAtEquipmentButton = Instantiate(GameAssetsSO.Instance.unitSelectAtEquipmentButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
            unitSelectAtEquipmentButton.Init(unitList[index], _unitManager, index + 1);
        }
    }
}
