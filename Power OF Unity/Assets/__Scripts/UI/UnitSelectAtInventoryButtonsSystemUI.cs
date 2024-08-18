using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора  Юнита, для настройки инвентаря
/// </summary>
public class UnitSelectAtInventoryButtonsSystemUI : MonoBehaviour, IToggleActivity
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
       
    private UnitManager _unitManager;
    private ScrollRect _scrollRect; //Компонент прокрутки кнопок    
    private Camera _cameraInventoryUI;
    private TooltipUI _tooltipUI;
    private Canvas _canvas;
   

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);
        RenderMode canvasRenderMode = _canvas.renderMode;
        if (canvasRenderMode == RenderMode.WorldSpace)// Если канвас в мировом пространстве то
        {
            _cameraInventoryUI = GetComponentInParent<Camera>(); // Для канваса в мировом пространстве будем использовать отдельную дополнительную камеру
        }
        else
        {
            _cameraInventoryUI = null;
        }
    }

    public void Init(UnitManager unitManager)
    {
        _unitManager = unitManager;

        Setup();
    }

    private void Setup()
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

    public void SetActive(bool active)
    {
        _canvas.enabled = active;
        if (active)
        {
            CreateButtons(); // Создать систему кнопок для выбора Юнита     
            SelectFirstUnitAndShowTab();
        }
        else
        {
            _unitManager.ClearSelectedUnit(); // Очистим выделенного юнита чтобы сбросить 3D модель и ПОРТФОЛИО
        }
    }


    /// <summary>
    /// Выбрать первого юнита из списка, и показать соответствующую выкладку(база/миссия)
    /// </summary>
    private void SelectFirstUnitAndShowTab()
    {     
        Unit selectedUnit = _unitManager.SelectAndReturnFirstUnitFromList();
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
        SetActiveUnitsOnMissionTab(true);
        SetActiveUnitsOnBarrackTab(false);
    }
    private void ShowBarrackTab()
    {
        SetActiveUnitsOnBarrackTab(true);
        SetActiveUnitsOnMissionTab(false);
    }
    private void SetActiveUnitsOnBarrackTab(bool active)
    {
        _barrackButtonSelectedImage.enabled = active;
        _onBarrackHeaderText.enabled = active;
        _unitsOnBarrackContainer.gameObject.SetActive(active);
        if (active) { _scrollRect.content = (RectTransform)_unitsOnBarrackContainer; } //Если активна то Установим этот контейнер как контент для прокрутки        
    }

    private void SetActiveUnitsOnMissionTab(bool active)
    {
        _missionButtonSelectedImage.enabled = active;
        _onMissionHeaderText.enabled = active;
        _unitsOnMissionContainer.gameObject.SetActive(active);
        if (active) { _scrollRect.content = (RectTransform)_unitsOnMissionContainer; } //Если активна то Установим этот контейнер как контент для прокрутки        
    }

    private void CreateButtons()
    {
        CreateUnitSelectOnMissionButtonsSystem();
        CreateUnitSelectOnBarrackButtonsSystem();
    }

    /// <summary>
    /// Создать кнопки выбора юнитов на ЗАДАНИИ
    /// </summary>
    private void CreateUnitSelectOnMissionButtonsSystem()
    {
        foreach (Transform unitSelectButton in _unitsOnMissionContainer) // Переберем все трансформы в нашем контейнере
        {
            Destroy(unitSelectButton.gameObject); // Удалим игровой объект прикрипленный к Transform
        }
        
        List<Unit> unitFriendOnMissionList = _unitManager.GetUnitFriendOnMissionList();// список  моих юнитов на миссии
        for (int index = 0; index < unitFriendOnMissionList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendOnMissionList[index], _unitsOnMissionContainer, index + 1);
        }
        _scrollRect.verticalScrollbar.value = 1f; // переместим прокрутку панели в верх.
    }
    /// <summary>
    /// Создать кнопки выбора юнитов на БАЗЕ
    /// </summary>
    private void CreateUnitSelectOnBarrackButtonsSystem()
    {
        foreach (Transform unitSelectButton in _unitsOnBarrackContainer) // Переберем все трансформы в нашем контейнере
        {
            Destroy(unitSelectButton.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        List<Unit> unitFriendOnBarrackList = _unitManager.GetUnitFriendOnBarrackList();// список  моих юнитов в казарме    
        for (int index = 0; index < unitFriendOnBarrackList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendOnBarrackList[index], _unitsOnBarrackContainer, index + 1);
        }      
        _scrollRect.verticalScrollbar.value = 1f; // переместим прокрутку панели в верх.
    }
    private void CreateUnitSelectButton(Unit unit, Transform containerTransform, int index) // Создать Кнопку Размещаемых объектов и поместим в контейнер
    {
        UnitSelectAtInventoryButton unitSelectAtInventoryButton = Instantiate(GameAssets.Instance.unitSelectAtInventoryButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        unitSelectAtInventoryButton.Init(unit, _unitManager, index);
    }
}
