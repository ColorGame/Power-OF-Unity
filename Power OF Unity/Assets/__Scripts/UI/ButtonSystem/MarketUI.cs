using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MarketState
{
    //NoneWeapon,
    Buy,
    Sell
}

/// <summary>
/// Рынок (экипировки и ресурсов) 
/// </summary>
/// <remarks>
/// Класса обязан сделать проверки на возможность ПОКУПКИ/ПРОДАЖИ
/// </remarks>
public class MarketUI : ObjectSelectButtonsSystemUI
{
    [Header("Контейнеры для переключения")]
    [SerializeField] private Transform _weaponSelectContainer; // Контейнер для выбора оружия 
    [SerializeField] private Transform _itemSelectContainer; // Контейнер для выбора предмета   
    [SerializeField] private Transform _armorHeadSelectContainer; // Контейнер для выбора шлема 
    [SerializeField] private Transform _armorBodySelectContainer; // Контейнер для выбора бронежилета   
    [SerializeField] private Transform _otherSelectContainer; // Контейнер для выбора прочих предметов   
    [Header("Кнопки переключения контейнеров")]
    [SerializeField] private Button _weaponButton;
    [SerializeField] private Image _weaponButtonSelectedImage;
    [SerializeField] private Button _itemButton;
    [SerializeField] private Image _itemButtonSelectedImage;
    [SerializeField] private Button _armorHeadButton;
    [SerializeField] private Image _armorHeadButtonSelectedImage;
    [SerializeField] private Button _armorBodyButton;
    [SerializeField] private Image _armorBodyButtonSelectedImage;
    [SerializeField] private Button _otherButton;
    [SerializeField] private Image _otherButtonSelectedImage;
    [Header("Кнопки переключения режима ПОКУПКИ/ПРОДАЖИ")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _buyButtonSelectedImage;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Image _sellButtonSelectedImage;
    [Header("Кнопка завершения сделки")]
    [SerializeField] private Button _сloseDealButton;
    [Header("Сумма всех ПОКУПОК/ПРОДАЖ")]
    [SerializeField] private TextMeshProUGUI _sumAllBuyText;
    [SerializeField] private TextMeshProUGUI _sumAllSellText;

    [Header("Цвет для текста")]
    [SerializeField] private Color _red;
    [SerializeField] private Color _green;

    private Dictionary<PlacedObjectTypeSO, uint> _buyObjectCountDictionary = new(); // Словарь - Количество объектов на покупку
    private Dictionary<PlacedObjectTypeSO, uint> _sellObjectCountDictionary = new();// Словарь - Количество объектов на продажу
    private List<PlacedObjectBuySellCountButtonUI> _activeBuySellCountButtonList = new(); // Список активных кнопок для ПОКУПКИ/ПРОДАЖИ
    private MarketState _marketState; // Хэшированое состояние маркета
    /// <summary>
    /// Сумма всех покупок (спишется со счета)
    /// </summary>
    private uint _sumAllBuy;
    /// <summary>
    /// Сумма всех продаж (зачислиться на сет)
    /// </summary>
    private uint _sumAllSell;

    private WarehouseManager _warehouseManager;
    private HashAnimationName _hashAnimationName;


    protected override void Awake()
    {
        base.Awake();
        ClearAndUpdateSumText();
        _marketState = MarketState.Buy;

        _containerArray = new Transform[]
        {
            _weaponSelectContainer,
            _itemSelectContainer,
            _armorHeadSelectContainer,
            _armorBodySelectContainer,
            _otherSelectContainer,
        };
        _buttonSelectedImageArray = new Image[]
        {
            _weaponButtonSelectedImage,
            _itemButtonSelectedImage,
            _armorHeadButtonSelectedImage,
            _armorBodyButtonSelectedImage,
            _otherButtonSelectedImage,           
        };
    }

    public void Init(TooltipUI tooltipUI, WarehouseManager warehouseManager, HashAnimationName hashAnimationName)
    {
        _tooltipUI = tooltipUI;
        _warehouseManager = warehouseManager;
        _hashAnimationName = hashAnimationName;
        Setup();
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _weaponButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_weaponSelectContainer, _weaponButtonSelectedImage);            
        });

        _itemButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_itemSelectContainer, _itemButtonSelectedImage);           
        });
        _armorHeadButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_armorHeadSelectContainer, _armorHeadButtonSelectedImage);           
        });

        _armorBodyButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_armorBodySelectContainer, _armorBodyButtonSelectedImage);            
        });

        // Кнопки переключения состояния BUY\SELL
        _buyButton.onClick.AddListener(() =>
        {
            SetMarketState(MarketState.Buy);
        });
        _sellButton.onClick.AddListener(() =>
        {
            SetMarketState(MarketState.Sell);
        });
        // Завершить сделку
        _сloseDealButton.onClick.AddListener(() =>
        {
            CloseDeal();
        });
    }

    

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;
        if (active)
        {
            SetAndShowContainer(_weaponSelectContainer, _weaponButtonSelectedImage);
        }
        else
        {
            ClearActiveButtonContainer();           
        }
    }

    private void SetAndShowContainer(Transform selectContainer, Image typeButtonSelectedImage)
    {
        ShowAndUpdateContainer(selectContainer);
        ShowSelectedButton(typeButtonSelectedImage);
        StartAnimationBuySellCountButton(_marketState);
    }

    /// <summary>
    /// Попробую добавить количество объектов для покупки
    /// </summary>
    public bool TryPlusBuyCount(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (CoinEnoughBuy(placedObjectTypeSO, number))
        {
            _buyObjectCountDictionary[placedObjectTypeSO] += number; // Добавим к текущему количеству
            _sumAllBuy += placedObjectTypeSO.GetPriceBuy() * number;
            UpdateSumAllBuyText();
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// Попробую убавить количество объектов для покупки
    /// </summary>
    public bool TryMinusBuyCount(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if ((int)(_buyObjectCountDictionary[placedObjectTypeSO] - number) >= 0)
        {
            _buyObjectCountDictionary[placedObjectTypeSO] -= number;
            _sumAllBuy -= placedObjectTypeSO.GetPriceBuy() * number;
            UpdateSumAllBuyText();
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// Попробую добавить количество объектов для продажи
    /// </summary>
    public bool TryPlusSellCount(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if (_sellObjectCountDictionary[placedObjectTypeSO] + number <= _warehouseManager.GetCountPlacedObject(placedObjectTypeSO)) // не можем продать больше чем имеем в наличии
        {
            _sellObjectCountDictionary[placedObjectTypeSO] += number;
            _sumAllSell += placedObjectTypeSO.GetPriceSell() * number;
            UpdateSumAllSellText();
            return true;
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse($"не хватает КОЛИЧЕСТВО", new TooltipUI.TooltipTimer { timer = 0.8f });
            return false;
        }
    }
    /// <summary>
    /// Попробую убавить количество объектов для продажи
    /// </summary>
    public bool TryMinusSellCount(PlacedObjectTypeSO placedObjectTypeSO, uint number = 1)
    {
        if ((int)(_sellObjectCountDictionary[placedObjectTypeSO] - number) >= 0)
        {
            _sellObjectCountDictionary[placedObjectTypeSO] -= number;
            _sumAllSell -= placedObjectTypeSO.GetPriceSell();
            UpdateSumAllSellText();
            return true;
        }
        else { return false; }
    }

    private void CloseDeal()
    {
        switch (_marketState)
        {
            case MarketState.Buy:
                Buy();
                break;
            case MarketState.Sell:
                Sell();
                break;
        }
    }

    private void Buy()
    {
        _warehouseManager.TryBuy(_sumAllBuy, _buyObjectCountDictionary);
    
        ClearBuySellCountInDictionary(_buyObjectCountDictionary);
        ClearAndUpdateSumText();
        ClearByuSellTextInButtonList();
    }

    private void Sell()
    {
        _warehouseManager.Sell(_sumAllSell, _sellObjectCountDictionary);        

        ClearBuySellCountInDictionary(_sellObjectCountDictionary);
        ClearAndUpdateSumText();
        ClearByuSellTextInButtonList();
    }

    protected override void CreateSelectButtonsSystemInActiveContainer()
    {   
        // Переберем список 
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case GrappleTypeSO:
                case ShootingWeaponTypeSO:
                case SwordTypeSO:
                    if (_activeContainer == _weaponSelectContainer)
                        CreatePlacedObjectBuySellCountButton(placedObjectTypeSO, _weaponSelectContainer);
                    break;

                case GrenadeTypeSO:
                case HealItemTypeSO:
                case ShieldItemTypeSO:
                case SpotterFireItemTypeSO:
                case CombatDroneTypeSO:
                    if (_activeContainer == _itemSelectContainer)
                        CreatePlacedObjectBuySellCountButton(placedObjectTypeSO, _itemSelectContainer);
                    break;

                case HeadArmorTypeSO:
                    if (_activeContainer == _armorHeadSelectContainer)
                        CreatePlacedObjectBuySellCountButton(placedObjectTypeSO, _armorHeadSelectContainer);
                    break;

                case BodyArmorTypeSO:
                    if (_activeContainer == _armorBodySelectContainer)
                        CreatePlacedObjectBuySellCountButton(placedObjectTypeSO, _armorBodySelectContainer);
                    break;
            }
        }
    }

    /// <summary>
    /// Создать Кнопки изменения количества покупки и продажи объекта типа PlacedObject
    /// </summary>
    private void CreatePlacedObjectBuySellCountButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
        PlacedObjectBuySellCountButtonUI buySellCountButton = Instantiate(GameAssets.Instance.placedObjectBuySellCountButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), buySellCountButton.transform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке               
        visualButton.GetComponentInChildren<Image>(true).raycastTarget = false;
        buySellCountButton.Init(this, _warehouseManager, placedObjectTypeSO, _hashAnimationName, _tooltipUI);

        _buyObjectCountDictionary[placedObjectTypeSO] = 0;
        _sellObjectCountDictionary[placedObjectTypeSO] = 0;
        _activeBuySellCountButtonList.Add(buySellCountButton);
    }

    protected override void ClearActiveButtonContainer()
    {
        base.ClearActiveButtonContainer();
        ClearActiveCollection();
    }
    private void ClearActiveCollection()
    {
        _buyObjectCountDictionary.Clear();
        _sellObjectCountDictionary.Clear();
        _activeBuySellCountButtonList.Clear();
        ClearAndUpdateSumText();
    }
    private void ClearAndUpdateSumText()
    {
        _sumAllBuy = 0;
        _sumAllSell = 0;
        UpdateSumAllBuyText();
        UpdateSumAllSellText();
    }
    private void SetMarketState(MarketState marketState)
    {
        if (_marketState == marketState)
        {
            return;
        }
        else
        {
            _marketState = marketState;
            UpdateMarketState();
        }
    }

    /// <summary>
    /// Очистим количество объектов на продажу/покупку, в словаре. Ключи остаются на месте.
    /// </summary>
    private void ClearBuySellCountInDictionary(Dictionary<PlacedObjectTypeSO, uint> dictionary)
    {
        var keyList = new List<PlacedObjectTypeSO>(dictionary.Keys); // Напрямую нельзя переберать коллекцию и делать в ней изменения
        foreach (PlacedObjectTypeSO placedObjectTypeSO in keyList)
        {
            dictionary[placedObjectTypeSO] = 0;
        }
    }

    private void UpdateMarketState()
    {
        switch (_marketState)
        {
            case MarketState.Buy:
                // Очистим неактивное окно
                ClearBuySellCountInDictionary(_sellObjectCountDictionary);
                ClearAndUpdateSumText();                
                StartAnimationBuySellCountButton(MarketState.Buy);

                _buyButtonSelectedImage.enabled = true;
                _sellButtonSelectedImage.enabled = false;

                break;
            case MarketState.Sell:
                // Очистим неактивное окно
                ClearBuySellCountInDictionary(_buyObjectCountDictionary);
                ClearAndUpdateSumText();
                StartAnimationBuySellCountButton(MarketState.Sell);

                _sellButtonSelectedImage.enabled = true;
                _buyButtonSelectedImage.enabled = false;

                break;
        }
    }
    

    /// <summary>
    /// Запуск анимации для кнопок, выбора количества объектов для покупки\продажи.
    /// </summary>
    private void StartAnimationBuySellCountButton(MarketState marketState)
    {
        foreach (PlacedObjectBuySellCountButtonUI buySellCountButton in _activeBuySellCountButtonList)
        {
            buySellCountButton.StartAnimationClearByuSellText(marketState);
        }
    }
    /// <summary>
    /// Очистить текст в списке _activeBuySellCountButtonList
    /// </summary>
    private void ClearByuSellTextInButtonList()
    {
        foreach (PlacedObjectBuySellCountButtonUI buySellCountButton in _activeBuySellCountButtonList)
        {
            buySellCountButton.ClearByuSellText();
        }
    }

    private void UpdateSumAllBuyText()
    {
        // C: $1,234,567.89 // N: 1,234,567.89
        _sumAllBuyText.text = $"-({_sumAllBuy.ToString("N0")})";
        SetColorText(_sumAllBuy, redText: _sumAllBuyText);
    }
    private void UpdateSumAllSellText()
    {
        _sumAllSellText.text = _sumAllSell.ToString("N0");
        SetColorText(_sumAllSell, greenText: _sumAllSellText);
    }

    /// <summary>
    /// Достаточно денег, чтобы купить?
    /// </summary>
    private bool CoinEnoughBuy(PlacedObjectTypeSO placedObjectTypeSO, uint number)
    {
        // теккущая сумма покупки + цена объекта (который хотим добавить) <= всех наших сбережений (если истина то у нас достаточно средств)
        if (_sumAllBuy + placedObjectTypeSO.GetPriceBuy() * number <= _warehouseManager.GetCountCoin())
        {
            return true;
        }
        else
        {
            _tooltipUI.ShowShortTooltipFollowMouse("Недостаточно СРЕДСТВ", new TooltipUI.TooltipTimer { timer = 0.8f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            return false;
        }      
       
    }

    /// <summary>
    /// Установить соответствующий цвет для переданного текста
    /// </summary>
    public void SetColorText(uint count, TextMeshProUGUI redText = null, TextMeshProUGUI greenText = null)
    {
        if (count == 0)
        {
            if (redText != null)
                redText.color = Color.white;
            if (greenText != null)
                greenText.color = Color.white;
        }
        else
        {
            if (redText != null)
                redText.color = _red;
            if (greenText != null)
                greenText.color = _green;
        }
    }


    public uint GetBuyCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _buyObjectCountDictionary[placedObjectTypeSO]; }
    public uint GetSellCountPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) { return _sellObjectCountDictionary[placedObjectTypeSO]; }
}
