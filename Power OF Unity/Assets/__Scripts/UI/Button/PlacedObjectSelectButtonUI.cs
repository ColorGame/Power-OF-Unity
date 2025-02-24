using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������ ������ ������������ �������
/// </summary>
public class PlacedObjectSelectButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _button;

    [Header("��� ������ ������")]
    [SerializeField] private int _minHeightButton = 300;

    private int _boundHieght = 50; // ������ �����

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
        _countText.transform.SetAsLastSibling(); // �������� � ����� ���������� ������ ��� �� ������������ ������ ����

        float heightImage = _placedObjectTypeSO.GetHeightImage2D(); // ������� ������ ���������� ����������� 
        RectTransform rectTransformButton = GetComponent<RectTransform>();
        if (heightImage > _minHeightButton) // ���� ������ ����������� ������ �� ������� ������ ������        
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
    /// ��� ��������� ���������� ����������� ��������
    /// </summary>
    private void WarehouseManager_OnChangCountPlacedObject(object sender, PlacedObjectTypeAndCount placedObjectTypeAndCount)
    {
        if (_placedObjectTypeSO == placedObjectTypeAndCount.placedObjectTypeSO) // ���� ��� ��� ��� �� ������� �����
            _countText.text = placedObjectTypeAndCount.count.ToString();
    }

    private void UpdateCountText()
    {
        _countText.text = _warehouseManager.GetCountPlacedObject(_placedObjectTypeSO).ToString();
    }

    /// <summary>
    /// ��������� ��� ������ ������� 
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
                _tooltipUI.ShowShortTooltipFollowMouse($"�� ������� {_placedObjectTypeSO.GetPlacedObjectTooltip().name.ToUpper()}", new TooltipUI.TooltipTimer { timer = 1f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            }
        });
    }

    /// <summary>
    /// ��������� ��� ������ ����������� ��������� 
    /// </summary>
    private void SetTooltipButton()
    {
        MouseEnterExitEventsUI mouseEnterExitEventsUI = GetComponent<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� �������
        {
            _tooltipUI.ShowAnchoredPlacedObjectTooltip(_placedObjectTypeSO.GetPlacedObjectTooltip(), (RectTransform)transform); // ��� ��������� �� ������ ������� ��������� � ��������� ����� � ������ ������� �������� ��� ������
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
        };
    }       

    private void OnDestroy()
    {
        if (_warehouseManager != null)
            _warehouseManager.OnChangCountPlacedObject -= WarehouseManager_OnChangCountPlacedObject;
    }
}
