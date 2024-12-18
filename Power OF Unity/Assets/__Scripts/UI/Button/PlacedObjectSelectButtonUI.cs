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

    private Button _button;
    private TooltipUI _tooltipUI;
    private PickUpDropPlacedObject _pickUpDrop;
    private WarehouseManager _warehouseManager;       
    private PlacedObjectTypeSO _placedObjectTypeSO;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

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
        _countText.text = _warehouseManager.GetCountPlacedObject(_placedObjectTypeSO).ToString();

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
            _countText.text = placedObjectTypeAndCount.count.ToString();
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
        if(_warehouseManager!=null)
        _warehouseManager.OnChangCountPlacedObject -= ResourcesManager_OnChangCountPlacedObject;
    }
}
