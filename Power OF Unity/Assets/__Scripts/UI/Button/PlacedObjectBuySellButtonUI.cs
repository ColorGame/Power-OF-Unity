using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Кнопки покупки и продажи объекта типа PlacedObject
/// </summary>
public class PlacedObjectBuySellButtonUI : MonoBehaviour
{
    [Tooltip("Имеющиеся количество объектов")]
    [SerializeField] private TextMeshProUGUI _countText;

    [Tooltip("Вкладка для ПОКУПКИ")]
    [SerializeField] private TextMeshProUGUI _buyPriceText;
    [SerializeField] private TextMeshProUGUI _buyCountText;
    [SerializeField] private TextMeshProUGUI _buySumText;
    [SerializeField] private Button _minusBuyCountButton;
    [SerializeField] private Button _plusBuyCountButton;

    [Tooltip("Вкладка для ПРОДАЖИ")]
    [SerializeField] private TextMeshProUGUI _sellPriceText;
    [SerializeField] private TextMeshProUGUI _sellCountText;
    [SerializeField] private TextMeshProUGUI _sellSumText;
    [SerializeField] private Button _minusSellCountButton;
    [SerializeField] private Button _plusSellCountButton;

    [Tooltip("Цвет для текста")]
    [SerializeField] private Color _red;
    [SerializeField] private Color _green;

    //uint>=0
    private uint _buyPrice;
    private uint _buyCount;   

    private uint _sellPrice;
    private uint _sellCount;   

    protected HashAnimationName _hashAnimationName;
    private Animator _animator;

    private MarketUI _marketUI;
    private WarehouseManager _warehouseManager;
    private PlacedObjectTypeSO _placedObjectTypeSO;  

    private void Awake()
    {
        if (TryGetComponent(out Animator animator))
        {
            _animator = animator;
        }
    }

    public void Init(MarketUI marketUI, WarehouseManager warehouseManager, PlacedObjectTypeSO placedObjectTypeSO, HashAnimationName hashAnimationName)
    {
        _marketUI = marketUI;
        _warehouseManager = warehouseManager;
        _hashAnimationName = hashAnimationName;               
        _placedObjectTypeSO = placedObjectTypeSO;
        Setup();
    }

    private void Setup()
    {
        _countText.text = _warehouseManager.GetCountPlacedObject(_placedObjectTypeSO).ToString();
        _buyPrice = _placedObjectTypeSO.GetPriceBuy();
        _sellPrice = _placedObjectTypeSO.GetPriceSell();
        _buyCount = 0;
        _sellCount = 0;

        // C: $1,234,567.89 // N: 1,234,567.89
        _buyPriceText.text = _buyPrice.ToString("C");
        _sellPriceText.text = _sellPrice.ToString("C");

        SetButtons();
        UpdateBuyText(); 
        UpdateSellText();

      //  _warehouseManager.OnChangCountPlacedObject += ResourcesManager_OnChangCountPlacedObject;
    }

    private void SetButtons()
    {
        _minusBuyCountButton.onClick.AddListener(() =>
        {
            MinusBuyCount();
        });
        _plusBuyCountButton.onClick.AddListener(() =>
        {
            PlusBuyCount();
        }); 
        _minusSellCountButton.onClick.AddListener(() =>
        {
            MinusSellCount();
        });
        _plusSellCountButton.onClick.AddListener(() =>
        {
            PlusSellCount();
        });
    }

    /// <summary>
    /// Убавить количество объектов для покупки
    /// </summary>
    private void MinusBuyCount()
    {       
        if (_buyCount > 0) 
        {
            _buyCount--;
            UpdateBuyText();
            //нет проверки на отриц. число т.к. uint>=0
        }           
    }
    /// <summary>
    /// Добавить количество объектов для покупки
    /// </summary>
    private void PlusBuyCount()
    {
        if (_marketUI.TryAddBuyCount(_placedObjectTypeSO))
        {
            _buyCount++;
            UpdateBuyText();
        }
    }

    /// <summary>
    /// Убавить количество объектов для продажи
    /// </summary>
    private void MinusSellCount()
    {
        if (_sellCount > 0)
        {
            _sellCount--;
            UpdateSellText();
            //нет проверки на отриц. число т.к. uint>=0
        }
    }    

    /// <summary>
    /// Добавить количество объектов для продажи
    /// </summary>
    private void PlusSellCount()
    {
       // if(_sellCount<_cou)
    }

    private void UpdateBuyText()
    {
        SetColorText(_buyCount, redText: _buySumText, greenText: _buyCountText);

        _buyCountText.text = _buyCount.ToString();
        _buySumText.text = (_buyCount * _buyPrice).ToString("C");
    }

    private void UpdateSellText()
    {
        SetColorText(_sellCount, redText: _sellCountText, greenText: _sellSumText);

        _sellCountText.text = _sellCount.ToString();
        _sellSumText.text = (_sellCount*_sellPrice).ToString("C");
    }
    /// <summary>
    /// Установить соответствующий цвет для переданного текста
    /// </summary>
    private void SetColorText(uint count, TextMeshProUGUI redText, TextMeshProUGUI greenText)
    {
        if (count == 0)
        {
            redText.color = Color.white;
            greenText.color = Color.white;
        }
        else
        {
            redText.color = _red;
            greenText.color = _green;
        }
    }


}
