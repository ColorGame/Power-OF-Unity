using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������ ��������� ���������� ������� � ������� ������� ���� PlacedObject
/// </summary>
/// <remarks>
/// � ������� ��� ������� BUY\SELL ���������� � ��������� ���������
/// </remarks>
public class PlacedObjectBuySellCountButtonUI : MonoBehaviour
{
    public event EventHandler OnChangeBuyCount;
    public event EventHandler OnChangeSellCount;

    [Header("��������� ���������� ��������")]
    [SerializeField] private TextMeshProUGUI _inStockCountText;

    [Header("������� ��� �������")]
    [SerializeField] private TextMeshProUGUI _buyPriceText;
    [SerializeField] private TextMeshProUGUI _buyCountText;
    [SerializeField] private TextMeshProUGUI _buySumText;
    [SerializeField] private Button _minusBuyCountButton;
    [SerializeField] private Button _plusBuyCountButton;
    [SerializeField] private Image _buyTabSelectedImage;

    [Header("������� ��� �������")]
    [SerializeField] private TextMeshProUGUI _sellPriceText;
    [SerializeField] private TextMeshProUGUI _sellCountText;
    [SerializeField] private TextMeshProUGUI _sellSumText;
    [SerializeField] private Button _minusSellCountButton;
    [SerializeField] private Button _plusSellCountButton;
    [SerializeField] private Image _sellTabSelectedImage;

    [Header("��� ������ ������")]
    [SerializeField] private int _minHeightButton = 300;
    [Header("�������� ������")]
    [SerializeField] private Animator _animator;


    //uint>=0
    private uint _inStockCount; //���������� �� ������

    private uint _buyPrice;
    private uint _buyCount;
    private uint _buySum;

    private uint _sellPrice;
    private uint _sellCount;
    private uint _sellSum;

    private int _buyTabOpen;
    private int _buyTabFirstOpen;
    private int _sellTabOpen;
    private int _sellTabFirstOpen;
    private bool _firstStart = true;    

    private MarketUI _marketUI;
    private WarehouseManager _warehouseManager;
    private HashAnimationName _hashAnimationName;
    private PlacedObjectTypeSO _placedObjectTypeSO;
    private TooltipUI _tooltipUI;

   
    public void Init(MarketUI marketUI, WarehouseManager warehouseManager, PlacedObjectTypeSO placedObjectTypeSO, HashAnimationName hashAnimationName, TooltipUI tooltipUI)
    {
        _marketUI = marketUI;
        _warehouseManager = warehouseManager;
        _hashAnimationName = hashAnimationName;
        _placedObjectTypeSO = placedObjectTypeSO;
        _tooltipUI = tooltipUI;
        Setup();
    }

    private void Setup()
    {       
        float heightImage = _placedObjectTypeSO.GetHeightImage2D(); // ������� ������ ���������� ����������� 
        RectTransform rectTransformButton = GetComponent<RectTransform>();
        if (heightImage > _minHeightButton) // ���� ������ ����������� ������ �� ������� ������ ������        
            rectTransformButton.sizeDelta = new Vector2(rectTransformButton.sizeDelta.x, heightImage);
        else
            rectTransformButton.sizeDelta = new Vector2(rectTransformButton.sizeDelta.x, _minHeightButton);

        _inStockCount = _warehouseManager.GetCountPlacedObject(_placedObjectTypeSO);
        _buyPrice = _placedObjectTypeSO.GetPriceBuy();
        _sellPrice = _placedObjectTypeSO.GetPriceSell();

        _inStockCountText.transform.SetAsLastSibling(); // �������� � ����� ���������� ������ ��� �� ������������ ������ ����      


        // C: $1,234,567.89 // N: 1,234,567.89
        _buyPriceText.text = $"{_buyPrice.ToString("N0")} $";
        _sellPriceText.text = $"{_sellPrice.ToString("N0")} $";
               
        SetDelegateButton();
        SetTooltipButton();
        SetAnimationOpenClose();
        ClearByuSellText();
        UpdateInStockCountText();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            _warehouseManager.OnChangCountPlacedObject += ResourcesManager_OnChangCountPlacedObject;

            _inStockCount = _warehouseManager.GetCountPlacedObject(_placedObjectTypeSO);
            UpdateInStockCountText();
        }
        else
        {
            _warehouseManager.OnChangCountPlacedObject -= ResourcesManager_OnChangCountPlacedObject;
        }
    }
   
    /// <summary>
    /// ��� ��������� ���������� ����������� ��������
    /// </summary>
    private void ResourcesManager_OnChangCountPlacedObject(object sender, PlacedObjectTypeAndCount placedObjectTypeAndCount)
    {
        if (_placedObjectTypeSO == placedObjectTypeAndCount.placedObjectTypeSO) // ���� ��� ��� ��� �� ������� �����
        {
            _inStockCount = placedObjectTypeAndCount.count;
            UpdateInStockCountText();
        }
    }

    private void SetDelegateButton()
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
    /// ��������� ��� ������ ����������� ��������� 
    /// </summary>
    private void SetTooltipButton()
    {
        MouseEnterExitEventsUI mouseEnterExitEventsUI = GetComponentInChildren<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� �������
        {
            _tooltipUI.ShowAnchoredPlacedObjectTooltip(_placedObjectTypeSO.GetPlacedObjectTooltip(), (RectTransform)transform); // ��� ��������� �� ������ ������� ��������� � ��������� ����� � ������ ������� �������� ��� ������
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
        };
    }


    /// <summary>
    /// �������� ���������� �������� ��� �������
    /// </summary>
    private void PlusBuyCount()
    {
        if (_marketUI.TryPlusBuyCount(_placedObjectTypeSO))
            UpdateBuyText(_marketUI.GetBuyCountPlacedObject(_placedObjectTypeSO));
    }
    /// <summary>
    /// ������� ���������� �������� ��� �������
    /// </summary>
    private void MinusBuyCount()
    {
        if (_marketUI.TryMinusBuyCount(_placedObjectTypeSO))
            UpdateBuyText(_marketUI.GetBuyCountPlacedObject(_placedObjectTypeSO));
    }
    /// <summary>
    /// �������� ���������� �������� ��� �������
    /// </summary>
    private void PlusSellCount()
    {
        if (_marketUI.TryPlusSellCount(_placedObjectTypeSO))
            UpdateSellText(_marketUI.GetSellCountPlacedObject((_placedObjectTypeSO)));
    }
    /// <summary>
    /// ������� ���������� �������� ��� �������
    /// </summary>
    private void MinusSellCount()
    {
        if (_marketUI.TryMinusSellCount(_placedObjectTypeSO))
            UpdateSellText(_marketUI.GetSellCountPlacedObject((_placedObjectTypeSO)));
    }

    public void ClearByuSellText()
    {
        UpdateBuyText(0);
        UpdateSellText(0);
    }

    private void UpdateBuyText(uint buyCount)
    {
        _buyCount = buyCount;
        _buySum = _buyCount * _buyPrice;

        _buyCountText.text = _buyCount.ToString();
        _buySumText.text = $"{_buySum.ToString("N0")} $";

        _marketUI.SetColorText(_buyCount, redText: _buySumText, greenText: _buyCountText);
        _buyTabSelectedImage.enabled = _buyCount == 0 ? false : true;
    }

    private void UpdateSellText(uint sellCount)
    {
        _sellCount = sellCount;
        _sellSum = _sellCount * _sellPrice;

        _sellCountText.text = _sellCount.ToString();
        _sellSumText.text = $"{_sellSum.ToString("N0")} $";

        _marketUI.SetColorText(_sellCount, redText: _sellCountText, greenText: _sellSumText);
        _sellTabSelectedImage.enabled = _sellCount == 0 ? false : true;
    }

    private void UpdateInStockCountText()
    {
        _inStockCountText.text = _inStockCount.ToString();
    }

    private void SetAnimationOpenClose()
    {
        _buyTabOpen = _hashAnimationName.PlacedObjectBuyTabOpen;
        _sellTabOpen = _hashAnimationName.PlacedObjectSellTabOpen;
        _buyTabFirstOpen = _hashAnimationName.PlacedObjectBuyTabFirstOpen;
        _sellTabFirstOpen = _hashAnimationName.PlacedObjectSellTabFirstOpen;
    }
    /// <summary>
    /// ������ �������� � ������� ����� �������/�������
    /// </summary>
    public void StartAnimationClearByuSellText(MarketState marketState)
    {
        switch (marketState)
        {
            case MarketState.Buy:
                if (_firstStart)
                {
                    StartAnimation(_buyTabFirstOpen);
                    _firstStart = false;
                }
                else
                {
                    StartAnimation(_buyTabOpen);
                }
                break;

            case MarketState.Sell:
                if (_firstStart)
                {
                    StartAnimation(_sellTabFirstOpen);
                    _firstStart = false;
                }
                else
                {
                    StartAnimation(_sellTabOpen);
                }
                break;
        }
        ClearByuSellText();
    }

    private void StartAnimation(int hashNameAnimation)
    {       
        _animator.CrossFade(hashNameAnimation, 0);         
    }

    private void OnDestroy()
    {
        if (_warehouseManager != null)
            _warehouseManager.OnChangCountPlacedObject -= ResourcesManager_OnChangCountPlacedObject;
    }

}
