using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ������ ���� � ����� �������� �����
/// </summary>
public class UpperMenuBarOnUnitSetupUI : MonoBehaviour
{
    [Header("������ ��������� �������� ����")]
    [SerializeField] private Button _gameMenuButton;
    [Header("����� ���������� �����")]
    [SerializeField] private TextMeshProUGUI _coinCountText;
    [Header("������ ������� ����������� ������")]
    [SerializeField] private Button _unitManagerButtonButton;
    [SerializeField] private Image _selectedUnitManagerButtonImage; // ����������� ���������� ������ 
    [SerializeField] private Button _weaponButton;
    [SerializeField] private Image _selectedItemButtonImage;
    [SerializeField] private Button _armorButton;
    [SerializeField] private Image _selectedArmorButtonImage;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Image _selectedShopButtonImage;
    [SerializeField] private Button _missionButton;
    [SerializeField] private Image _selectedMissionButtonImage;

    private UnitPortfolioUI _unitPortfolioUI;
    private UnitSelectAtEquipmentButtonsSystemUI _unitSelectAtEquipmentButtonsSystemUI;
    private PickUpDropPlacedObject _pickUpDropPlacedObject;
    private UnitEquipmentSystem _unitEquipmentSystem;
    private ItemSelectButtonsSystemUI _itemSelectButtonsSystemUI;
    private ArmorSelectButtonsSystemUI _armorSelectButtonsSystemUI;
    private MarketUI _marketUI;
    private UnitSelectForManagementButtonsSystemUI _unitManagerTabUI;


    private Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������
    private IToggleActivity[] _toggleTabArray; // ������ ������� ������� ����� �����������

    private GameInput _gameInput;
    private GameMenuUIProvider _gameMenuUIProvider;
    private WarehouseManager _warehouseManager;

    private void Awake()
    {
        _buttonSelectedImageArray = new Image[]
        {
            _selectedUnitManagerButtonImage,
            _selectedItemButtonImage,
            _selectedArmorButtonImage,
            _selectedShopButtonImage,
            _selectedMissionButtonImage
        };
    }

    public void Init(
        GameInput gameInput,
        WarehouseManager warehouseManager,
        GameMenuUIProvider GameMenuUIProvider,
        UnitPortfolioUI unitPortfolioUI,
        UnitSelectAtEquipmentButtonsSystemUI unitSelectAtEquipmentButtonsSystemUI,
        PickUpDropPlacedObject pickUpDropPlacedObject,
        UnitEquipmentSystem unitEquipmentSystem,
        ItemSelectButtonsSystemUI itemSelectButtonsSystemUI,
        ArmorSelectButtonsSystemUI armorSelectButtonsSystemUI,
        MarketUI marketUI,
        UnitSelectForManagementButtonsSystemUI unitManagerTabUI)
    {
        _gameInput = gameInput;
        _warehouseManager = warehouseManager;
        _gameMenuUIProvider = GameMenuUIProvider;

        _unitPortfolioUI = unitPortfolioUI;
        _unitSelectAtEquipmentButtonsSystemUI = unitSelectAtEquipmentButtonsSystemUI;
        _pickUpDropPlacedObject = pickUpDropPlacedObject;
        _unitEquipmentSystem = unitEquipmentSystem;
        _itemSelectButtonsSystemUI = itemSelectButtonsSystemUI;
        _armorSelectButtonsSystemUI = armorSelectButtonsSystemUI;
        _marketUI = marketUI;
        _unitManagerTabUI = unitManagerTabUI;

        Setup();
    }

    private void Setup()
    {
        _gameMenuButton.onClick.AddListener(() =>
        {
            UnsubscribeAlternativeToggleVisible();
            _gameMenuUIProvider.LoadAndToggleVisible(SubscribeAlternativeToggleVisible).Forget();
        });

        _unitManagerButtonButton.onClick.AddListener(() => { ShowManagerTab(); });
        _weaponButton.onClick.AddListener(() => { ShowItemTab(); });
        _armorButton.onClick.AddListener(() => { ShowArmorTab(); });
        _shopButton.onClick.AddListener(() => { ShowShopTab(); });
        _missionButton.onClick.AddListener(() => { ShowMissionTab(); });

        _toggleTabArray = new IToggleActivity[]
       {
            _pickUpDropPlacedObject,
            _unitPortfolioUI,
            _unitSelectAtEquipmentButtonsSystemUI,
            _unitEquipmentSystem,
            _itemSelectButtonsSystemUI,
            _armorSelectButtonsSystemUI,
            _marketUI,
            _unitManagerTabUI
       };

        _coinCountText.text = $"{_warehouseManager.GetCountCoin().ToString("N0")}";
        _warehouseManager.OnChangCoinCount += WarehouseManager_OnChangCoinCount;

        SubscribeAlternativeToggleVisible();
        DisableAllToggleTabInArray();
        ShowItemTab();
    }       

    private void WarehouseManager_OnChangCoinCount(object sender, uint count)
    {
        _coinCountText.text = $"{count.ToString("N0")}";
    }


    /// <summary>
    /// ����������� �� �������������� ������������ ��������� ���� (������ ��� ESC)
    /// </summary>
    protected void SubscribeAlternativeToggleVisible()
    {
        _gameInput.OnMenuAlternate += GameInput_OnMenuAlternate;
    }
    /// <summary>
    /// ���������� �� ��������������� ������������ ��������� ����
    /// </summary>
    protected void UnsubscribeAlternativeToggleVisible()
    {
        _gameInput.OnMenuAlternate -= GameInput_OnMenuAlternate;
    }
    private void GameInput_OnMenuAlternate(object sender, System.EventArgs e)
    {
        UnsubscribeAlternativeToggleVisible();
        _gameMenuUIProvider.LoadAndToggleVisible(SubscribeAlternativeToggleVisible).Forget();
    }
   
    private void ShowManagerTab()
    {
        ShowSelectedButton(_selectedUnitManagerButtonImage);
        ShowTabs(new HashSet<IToggleActivity>
        {
            _unitPortfolioUI,
            _unitManagerTabUI
        });
    }

    private void ShowItemTab()
    {
        ShowSelectedButton(_selectedItemButtonImage);
        _unitEquipmentSystem.SetActiveEquipmentGrid(EquipmentGrid.GridState.ItemGrid);
        ShowTabs(new HashSet<IToggleActivity>
        {
            _pickUpDropPlacedObject,
            _unitPortfolioUI,
            _unitSelectAtEquipmentButtonsSystemUI,
            _unitEquipmentSystem,
            _itemSelectButtonsSystemUI,
        });

    }
    private void ShowArmorTab()
    {
        ShowSelectedButton(_selectedArmorButtonImage);
        _unitEquipmentSystem.SetActiveEquipmentGrid(EquipmentGrid.GridState.ArmorGrid);
        ShowTabs(new HashSet<IToggleActivity>
        {
            _pickUpDropPlacedObject,
            _unitPortfolioUI,
            _unitSelectAtEquipmentButtonsSystemUI,
            _unitEquipmentSystem,
            _armorSelectButtonsSystemUI,
        });

    }
    private void ShowShopTab()
    {
        ShowSelectedButton(_selectedShopButtonImage);
        ShowTabs(new HashSet<IToggleActivity> { _marketUI });

    }
    private void ShowMissionTab()
    {
        ShowSelectedButton(_selectedMissionButtonImage);

        ShowTabs(new HashSet<IToggleActivity> { });
    }
    /// <summary>
    /// ��������� ��� ������������� ������� � �������
    /// </summary>
    private void DisableAllToggleTabInArray()
    {
        foreach (IToggleActivity tab in _toggleTabArray) 
        {
            tab.SetActive(false);
        } 
    }


    /// <summary>
    /// �������� ���������� �������
    /// </summary>
    /// <remarks>
    private void ShowTabs(HashSet<IToggleActivity> showArray)
    {
        foreach (IToggleActivity tab in _toggleTabArray)
        {
            if (showArray.Contains(tab))
            {
                tab.SetActive(true);               
            }
            else
            {
                tab.SetActive(false);
            }
        }     
    }

    private void ShowSelectedButton(Image typeButtonSelectedImage) // �������� ������������ ������ ������
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // ��������� ������ 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// ���� ��� ���������� ��� ����������� �� ������� ���
        }
    }

    private void OnDestroy()
    {
        if (_warehouseManager != null)
            _warehouseManager.OnChangCoinCount -= WarehouseManager_OnChangCoinCount;
        UnsubscribeAlternativeToggleVisible();
    }
}
