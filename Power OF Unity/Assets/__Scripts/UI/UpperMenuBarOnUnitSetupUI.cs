using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Верхняя панель меню в сцене нстройки юнита
/// </summary>
public class UpperMenuBarOnUnitSetupUI : MonoBehaviour
{
    [Header("Кнопка включения игрового меню")]
    [SerializeField] private Button _gameMenuButton;
    [Header("Текст количества монет")]
    [SerializeField] private TextMeshProUGUI _coinCountText;
    [Header("Кнопки верхней центральной панели")]
    [SerializeField] private Button _unitManagerButtonButton;
    [SerializeField] private Image _selectedUnitManagerButtonImage; // Изображение выделенной кнопки 
    [SerializeField] private Button _weaponButton;
    [SerializeField] private Image _selectedWeaponButtonImage;
    [SerializeField] private Button _armorButton;
    [SerializeField] private Image _selectedArmorButtonImage;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Image _selectedShopButtonImage;
    [SerializeField] private Button _missionButton;
    [SerializeField] private Image _selectedMissionButtonImage;

    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSelectAtInventoryButtonsSystemUI _unitSelectAtInventoryButtonsSystemUI;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitInventorySystem _unitInventorySystem;
    private PlacedObjectWithActionSelectButtonsSystemUI _placedObjectSelectButtonsSystemUI;
    private UnitManagerTabUI _unitManagerTabUI;


    private Image[] _buttonSelectedImageArray; // массив изображений для веделения нужной кнопки
    private IToggleActivity[] _toggleTabArray; // Массив вкладок которые будем переключать

    private GameInput _gameInput;
    private GameMenuUI _gameMenuUI;

    private void Awake()
    {
        _buttonSelectedImageArray = new Image[]
        {
            _selectedUnitManagerButtonImage,
            _selectedWeaponButtonImage,
            _selectedArmorButtonImage,
            _selectedShopButtonImage,
            _selectedMissionButtonImage
        };

       
    }

    public void Init(
        GameInput gameInput,
        GameMenuUI gameMenuUI,
        UnitPortfolioUI unitPortfolioUI,
        UnitSelectAtInventoryButtonsSystemUI unitSelectAtInventoryButtonsSystemUI,
        PickUpDropPlacedObject pickUpDropPlacedObject,
        UnitInventorySystem unitInventorySystem,
        PlacedObjectWithActionSelectButtonsSystemUI placedObjectSelectButtonsSystemUI,
        UnitManagerTabUI unitManagerTabUI)
    {
        _gameInput = gameInput;
        _gameMenuUI = gameMenuUI;

        _unitPortfolioUI = unitPortfolioUI;
        _unitSelectAtInventoryButtonsSystemUI = unitSelectAtInventoryButtonsSystemUI;
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitInventorySystem = unitInventorySystem;
        _placedObjectSelectButtonsSystemUI = placedObjectSelectButtonsSystemUI;
        _unitManagerTabUI = unitManagerTabUI;



        Setup();
    }

    private void Setup()
    {
        _gameMenuButton.onClick.AddListener(() =>
        {
            UnsubscribeAlternativeToggleVisible();
            _gameMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
        });

        _unitManagerButtonButton.onClick.AddListener(() => { ShowManagerTab(); });
        _weaponButton.onClick.AddListener(() => { ShowWeaponTab(); });
        _armorButton.onClick.AddListener(() => { ShowArmorTab(); });
        _shopButton.onClick.AddListener(() => { ShowShopTab(); });
        _missionButton.onClick.AddListener(() => { ShowMissionTab(); });

        _toggleTabArray = new IToggleActivity[]
       {
            _pickUpDropPlacedObject,
            _unitPortfolioUI,
            _unitInventorySystem,
            _placedObjectSelectButtonsSystemUI,
            _unitSelectAtInventoryButtonsSystemUI,
            _unitManagerTabUI
       };

        SubscribeAlternativeToggleVisible();
        ShowWeaponTab();
    }

    /// <summary>
    /// Подписаться на альтернативное переключение видимости меню (обычно это ESC)
    /// </summary>
    protected void SubscribeAlternativeToggleVisible()
    {
        _gameInput.OnMenuAlternate += GameInput_OnMenuAlternate;
    }
    /// <summary>
    /// Отписаться от альтернативного переключение видимости меню
    /// </summary>
    protected void UnsubscribeAlternativeToggleVisible()
    {
        _gameInput.OnMenuAlternate -= GameInput_OnMenuAlternate;
    }
    private void GameInput_OnMenuAlternate(object sender, System.EventArgs e)
    {
        UnsubscribeAlternativeToggleVisible();
        _gameMenuUI.ToggleVisible(SubscribeAlternativeToggleVisible);
    }

    private void ShowManagerTab()
    {
        ShowSelectedButton(_selectedUnitManagerButtonImage);
        ShowTabs(new IToggleActivity[]
        {
            _unitPortfolioUI,
            _unitManagerTabUI
        });
    }

    private void ShowWeaponTab()
    {
        ShowSelectedButton(_selectedWeaponButtonImage);
        ShowTabs(new IToggleActivity[]
        {
            _pickUpDropPlacedObject,
            _unitPortfolioUI,
            _unitSelectAtInventoryButtonsSystemUI,
            _unitInventorySystem,
            _placedObjectSelectButtonsSystemUI,
        });

    }
    private void ShowArmorTab()
    {
        ShowSelectedButton(_selectedArmorButtonImage);
        ShowTabs(new IToggleActivity[]
        {
            _unitPortfolioUI,
            _unitSelectAtInventoryButtonsSystemUI,
            _pickUpDropPlacedObject,
        });

    }
    private void ShowShopTab()
    {
        ShowSelectedButton(_selectedShopButtonImage);
        ShowTabs(new IToggleActivity[] { });

    }
    private void ShowMissionTab()
    {
        ShowSelectedButton(_selectedMissionButtonImage);

        ShowTabs(new IToggleActivity[] {  } );
    }

    /// <summary>
    /// Показать переданные вкладки
    /// </summary>
    /// <remarks>
    /// Сначала все отключаем, потом активируем переданные. Такая система позваляет сбрасывать и обновлять все поля вкладок
    /// </remarks>
    private void ShowTabs(IToggleActivity[] showArray)
    {
        foreach (IToggleActivity tab in _toggleTabArray) 
        {
            tab.SetActive(false);            
        }
        foreach (IToggleActivity tab in showArray)
        {
            tab.SetActive(true);
        }

    }

    private void ShowSelectedButton(Image typeButtonSelectedImage) // Показать визуализацию выбора кнопки
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // Переберем массив 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// Если это переданное нам изображение то включим его
        }
    }
}
