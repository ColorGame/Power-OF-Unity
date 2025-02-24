using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Кнопка выбора размещенного объекта
/// </summary>
public class PlacedObjectSelectButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _button;

    [Header("Мин высота кнопки")]
    [SerializeField] private int _minHeightButton = 300;

    private int _boundHieght = 50; // высота рамки

    private TooltipUI _tooltipUI;
    private PickUpDropPlacedObject _pickUpDrop;
    private WarehouseManager _warehouseManager;
    private PlacedObjectTypeSO _placedObjectTypeSO;


    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop, WarehouseManager warehouseManager, PlacedObjectTypeSO placedObjectTypeSO)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;
        _warehouseManager = warehouseManager;
        _placedObjectTypeSO = placedObjectTypeSO;
        Setup();
    }

    private void Setup()
    {
        UpdateCountText();
        _countText.transform.SetAsLastSibling(); // поместим в конце локального списка что бы отображаться поверх всех

        float heightImage = _placedObjectTypeSO.GetHeightImage2D(); // Получим высоту вложенного изображения 
        RectTransform rectTransformButton = GetComponent<RectTransform>();
        if (heightImage > _minHeightButton) // Если размер изображения больше то изменим высоту кнопки        
            rectTransformButton.sizeDelta = new Vector2(rectTransformButton.sizeDelta.x, heightImage + _boundHieght);
        else
            rectTransformButton.sizeDelta = new Vector2(rectTransformButton.sizeDelta.x, _minHeightButton);


        SetDelegateButton();
        SetTooltipButton();
    }

    public void SetActive(bool active)
    {
        if (_warehouseManager == null)
            return;

        if (active)
        {
            _warehouseManager.OnChangCountPlacedObject += WarehouseManager_OnChangCountPlacedObject;
            UpdateCountText();
        }
        else
        {
            _warehouseManager.OnChangCountPlacedObject -= WarehouseManager_OnChangCountPlacedObject;
        }
    }

    /// <summary>
    /// При изменении количества размещенных объектов
    /// </summary>
    private void WarehouseManager_OnChangCountPlacedObject(object sender, PlacedObjectTypeAndCount placedObjectTypeAndCount)
    {
        if (_placedObjectTypeSO == placedObjectTypeAndCount.placedObjectTypeSO) // Если это наш тип то обновим текст
            _countText.text = placedObjectTypeAndCount.count.ToString();
    }

    private void UpdateCountText()
    {
        _countText.text = _warehouseManager.GetCountPlacedObject(_placedObjectTypeSO).ToString();
    }

    /// <summary>
    /// Установим для кнопки делегат 
    /// </summary>
    private void SetDelegateButton()
    {
        _button.onClick.AddListener(() =>
        {
            if (_warehouseManager.TryMinusCountPlacedObjectType(_placedObjectTypeSO))
            {
                _pickUpDrop.CreatePlacedObject(transform.position, _placedObjectTypeSO);
            }
            else
            {
                _tooltipUI.ShowShortTooltipFollowMouse($"не хватает {_placedObjectTypeSO.GetPlacedObjectTooltip().name.ToUpper()}", new TooltipUI.TooltipTimer { timer = 1f }); // Покажем подсказку и зададим новый таймер отображения подсказки
            }
        });
    }

    /// <summary>
    /// Установим для кнопки всплывающую подсказку 
    /// </summary>
    private void SetTooltipButton()
    {
        MouseEnterExitEventsUI mouseEnterExitEventsUI = GetComponent<MouseEnterExitEventsUI>(); // Найдем на кнопке компонент - События входа и выхода мышью 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // Подпишемся на событие
        {
            _tooltipUI.ShowAnchoredPlacedObjectTooltip(_placedObjectTypeSO.GetPlacedObjectTooltip(), (RectTransform)transform); // При наведении на кнопку покажем подсказку и передадим текст и камеру которая рендерит эту кнопку
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // При отведении мыши скроем подсказку
        };
    }       

    private void OnDestroy()
    {
        if (_warehouseManager != null)
            _warehouseManager.OnChangCountPlacedObject -= WarehouseManager_OnChangCountPlacedObject;
    }
}
