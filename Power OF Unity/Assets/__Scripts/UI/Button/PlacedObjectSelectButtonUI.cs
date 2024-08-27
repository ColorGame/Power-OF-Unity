using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������ ������ ������������ �������
/// </summary>
public class PlacedObjectSelectButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _count;

    private Button _button;
    private TooltipUI _tooltipUI;
    private PickUpDropPlacedObject _pickUpDrop;
    private WarehouseManager _warehouseManager;

    private PlacedObjectTypeAndCount _placedObjectTypeAndCount;
    private PlacedObjectTypeSO _placedObjectTypeSO;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop, WarehouseManager warehouseManager, PlacedObjectTypeAndCount placedObjectTypeAndCount)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;
        _warehouseManager = warehouseManager;
        _placedObjectTypeAndCount = placedObjectTypeAndCount;
        _placedObjectTypeSO = placedObjectTypeAndCount.placedObjectTypeSO;
        Setup();
    }

    private void Setup()
    {
        _count.text = _placedObjectTypeAndCount.count.ToString();

        _warehouseManager.OnChangCountPlacedObject += ResourcesManager_OnChangCountPlacedObject;

        SetDelegateButton();
        SetTooltipButton();
    }
    /// <summary>
    /// ��� ��������� ���������� ����������� ��������
    /// </summary>
    private void ResourcesManager_OnChangCountPlacedObject(object sender, PlacedObjectTypeAndCount placedObjectTypeAndCount)
    {
        if (_placedObjectTypeSO == placedObjectTypeAndCount.placedObjectTypeSO) // ���� ��� ��� ��� �� ������� �����
            _count.text = placedObjectTypeAndCount.count.ToString();
    }

    /// <summary>
    /// ��������� ��� ������ ������� 
    /// </summary>
    private void SetDelegateButton()
    {
        _button.onClick.AddListener(() =>
        {
            if (_warehouseManager.TryDecreaseCountPlacedObjectType(_placedObjectTypeSO))
            {
                _pickUpDrop.CreatePlacedObject(transform.position, _placedObjectTypeSO);
            }
            else
            {
                _tooltipUI.ShowShortTooltipFollowMouse($"�� ������� {_placedObjectTypeSO.GetPlacedObjectTooltip().name}", new TooltipUI.TooltipTimer { timer = 1f }); // ������� ��������� � ������� ����� ������ ����������� ���������
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
        if(_warehouseManager!=null)
        _warehouseManager.OnChangCountPlacedObject -= ResourcesManager_OnChangCountPlacedObject;
    }
}
