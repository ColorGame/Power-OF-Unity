using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ �������� ����(PlacedObject)
/// </summary>
public abstract class PlacedObjectSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{


    protected Transform[] _typeSelectContainerArray; // ������ ����������� ��� ������ ���� (PlacedObject)
    protected Transform _activeContainer;
    protected Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������      
    protected ScrollRect _scrollRect; //��������� ��������� ������
    protected Canvas _canvas;
    protected Camera _cameraEquipmentUI;
    protected TooltipUI _tooltipUI;
    protected PickUpDropPlacedObject _pickUpDrop;
    protected WarehouseManager _warehouseManager;

    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);

        switch (_canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                _cameraEquipmentUI = null;
                break;
            case RenderMode.ScreenSpaceCamera:
            case RenderMode.WorldSpace:
                _cameraEquipmentUI = GetComponentInParent<Camera>(); // ��� ������� � ������� ������������ ����� ������������ ��������� �������������� ������
                break;
        }
    }

    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop, WarehouseManager warehouseManager)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;
        _warehouseManager = warehouseManager;
        Setup();
    }

    private void Setup()
    {
        ClearAllContainer();
        SetDelegateContainerSelectionButton();
    }

    /// <summary>
    /// ���������� ������� ������ ������ ����������
    /// </summary>
    protected virtual void SetDelegateContainerSelectionButton() { }

    public virtual void SetActive(bool active) { }

    protected void ShowSelectedButton(Image typeButtonSelectedImage) // �������� ���������� ������
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // ��������� ������ 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// ���� ��� ���������� ��� ����������� �� ������� ���
        }
    }

    /// <summary>
    ///  �������� ��������� (� �������� �������� ������ ��������� ������)
    /// </summary>
    protected void ShowContainer(Transform typeSelectContainer) 
    {
        ClearActiveButtonContainer(); // ������� ���������� �������� ���������       

        foreach (Transform container in _typeSelectContainerArray) // ��������� ������ �����������
        {
            if (container == typeSelectContainer) // ���� ��� ���������� ��� ���������
            {
                _activeContainer = typeSelectContainer; // �������� ����� �������� ���������
                typeSelectContainer.gameObject.SetActive(true); // ������� ���
                CreateSelectButtonsSystemInActiveContainer();
                _scrollRect.content = (RectTransform)typeSelectContainer; // ��������� ���� ��������� ��� ������� ��� ���������
                _scrollRect.verticalScrollbar.value = 1; // ���������� ��������� ������ � ����.
            }
            else // � ��������� ������
            {
                container.gameObject.SetActive(false); // ��������
            }
        }
    }

    /// <summary>
    /// ������� �������� ��������� c ��������
    /// </summary>
    protected void ClearActiveButtonContainer()
    {
        if (_activeContainer != null)
        {
            foreach (Transform selectPlacedButton in _activeContainer)
            {
                Destroy(selectPlacedButton.gameObject); // ������ ������� ������ ������������� � Transform
            }
            _activeContainer = null;
        }
    }
    /// <summary>
    /// �������� ��� ����������
    /// </summary>
    private void ClearAllContainer()
    {
        foreach (Transform container in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in container)
            {
                Destroy(selectPlacedButton.gameObject);
            }
        }
    }

    /// <summary>
    ///  ������� ������� ������ ������(����������� ��������) � �������� ����������
    /// </summary>
    protected virtual void CreateSelectButtonsSystemInActiveContainer() { }

    /// <summary>
    /// ������� ������ ������ ������������ ������� � ��������� � ���������
    /// </summary>
    protected virtual void CreatePlacedObjectSelectButton(PlacedObjectTypeAndCount PlacedObjectResourcesParameters, Transform containerTransform) { }
}


