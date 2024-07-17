using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ ���� ������������ ������� (������� ������ ��� ������ ���������)
/// </summary>
public class PlacedObjectSelectButtonsSystemUI : MonoBehaviour
{
    [SerializeField] private Transform _weaponSelectContainer; // ��������� ��� ������ ������ 
    [SerializeField] private Transform _itemSelectContainer; // ��������� ��� ������ ��������       
    [SerializeField] private Transform _moduleSelectContainer; // ��������� ��� ������ ������
    [SerializeField] private Button _weaponButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _weaponButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _itemButtonPanel;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _itemButtonSelectedImage; // ����������� ���������� ������ ��������
    [SerializeField] private Button _moduleButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _moduleButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private List<PlacedObjectTypeSO> _ignorePlacedObjectList; // ������ �������� ������� ���� ������������ ��� �������� ������ ���������


    private Transform[] _typeSelectContainerArray; // ������ ����������� ��� ������ ������ ��������� � �������
    private Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������
    //private Dictionary<PlacedObjectTypeSO, Transform> _buttonTransformDictionary; // ������� (��� ������������ ������� - ����, Transform- -��������)
    private PlacedObjectTypeListSO _placedObjectTypeListSO; // ������ ����� ������������ �������    
    private ScrollRect _scrollRect; //��������� ��������� ������
    private RenderMode _canvasRenderMode;
    private Camera _cameraInventoryUI;
    private TooltipUI _tooltipUI;
    private PickUpDropPlacedObject _pickUpDrop;


    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvasRenderMode = GetComponentInParent<Canvas>().renderMode;
        if (_canvasRenderMode == RenderMode.WorldSpace)// ���� ������ � ������� ������������ ��
        {
            _cameraInventoryUI = GetComponentInParent<Camera>(); // ��� ������� � ������� ������������ ����� ������������ ��������� �������������� ������
        }
        else
        {
            _cameraInventoryUI = null;
        }


        _placedObjectTypeListSO = Resources.Load<PlacedObjectTypeListSO>(typeof(PlacedObjectTypeListSO).Name);    // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                                  // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� BuildingTypeListSO (������ ����� ����) � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������
                                                                                                                  // _buttonTransformDictionary = new Dictionary<PlacedObjectTypeSO, Transform>(); // �������������� ����� �������
        _typeSelectContainerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer, _moduleSelectContainer };
        _buttonSelectedImageArray = new Image[] { _weaponButtonSelectedImage, _itemButtonSelectedImage, _moduleButtonSelectedImage };

        // ��������� � ������� ����� ������ ��� ������
        ShowContainer(_weaponSelectContainer);
        ShowSelectedButton(_weaponButtonSelectedImage);
    }

    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;

        Setup();
    }

    private void Setup()
    {
        CreatePlacedObjectSelectButtonsSystem(); // ������� ������ ����� ����������� ��������

        _weaponButtonPanel.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }); // _weaponButtonPanel.onClick.AddListener(delegate { ShowContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _itemButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_itemSelectContainer);
            ShowSelectedButton(_itemButtonSelectedImage);
        });

        _moduleButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_moduleSelectContainer);
            ShowSelectedButton(_moduleButtonSelectedImage);
        });
    }

    private void ShowSelectedButton(Image typeButtonSelectedImage) // �������� ���������� ������
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // ��������� ������ 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// ���� ��� ���������� ��� ����������� �� ������� ���
        }
    }

    private void ShowContainer(Transform typeSelectContainer) // �������� ��������� (� �������� �������� ������ ��������� ������)
    {
        foreach (Transform buttonContainer in _typeSelectContainerArray) // ��������� ������ �����������
        {
            if (buttonContainer == typeSelectContainer) // ���� ��� ���������� ��� ���������
            {
                buttonContainer.gameObject.SetActive(true); // ������� ���
                _scrollRect.content = (RectTransform)typeSelectContainer; // ��������� ���� ��������� ��� ������� ��� ���������
            }
            else // � ��������� ������
            {
                buttonContainer.gameObject.SetActive(false); // ��������
            }
        }
    }


    private void CreatePlacedObjectSelectButtonsSystem() // ������� ������� ������ ��� ������ ����������� ��������
    {
        foreach (Transform typeSelectContainer in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in typeSelectContainer)
            {
                Destroy(selectPlacedButton.gameObject); // ������ ������� ������ ������������� � Transform
            }
        }

        foreach (PlacedObjectTypeSO placedObjectTypeSO in _placedObjectTypeListSO.list) // ��������� ������ ����� ����������� ��������
        {
            if (_ignorePlacedObjectList.Contains(placedObjectTypeSO)) continue; // ��������� ������� ��� ������� �� ���� ��������� ������

            switch (placedObjectTypeSO) // � ����������� �� ���� �������� � ������ ���������
            {
                case GrappleTypeSO itemTypeSO:
                    CreatePlacedObjectButton(itemTypeSO, _weaponSelectContainer);// ������� ������ ����������� �������� � �������� � ���������
                    break;
                case ShootingWeaponTypeSO weaponTypeSO:
                    CreatePlacedObjectButton(weaponTypeSO, _weaponSelectContainer);// ������� ������ ����������� �������� � �������� � ���������
                    break;
                case GrenadeTypeSO moduleTypeSO:
                    CreatePlacedObjectButton(moduleTypeSO, _itemSelectContainer);// ������� ������ ����������� �������� � �������� � ���������
                    break;
            }
        }

        _scrollRect.verticalScrollbar.value = 1; // ���������� ��������� ������ � ����.
    }

    private void CreatePlacedObjectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform) // ������� ������ ����������� �������� � �������� � ���������
    {
        Transform buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), buttonTransform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������ 

        if (_canvasRenderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // ������ ��� �������� ������� ������� � ������� ����, ��� �� ��� �� ����������� �� ���������� �����
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        buttonTransform.GetComponent<Button>().onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            _pickUpDrop.CreatePlacedObject(buttonTransform.position, placedObjectTypeSO); // �������� ������ ������ � ������� ������                
        });

        MouseEnterExitEventsUI mouseEnterExitEventsUI = buttonTransform.GetComponent<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� �������
        {
            _tooltipUI.ShowAnchoredPlacedObjectTooltip(placedObjectTypeSO.GetPlacedObjectTooltip(), (RectTransform)buttonTransform, _cameraInventoryUI); // ��� ��������� �� ������ ������� ��������� � ��������� ����� � ������ ������� �������� ��� ������
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
        };

        //  _buttonTransformDictionary[placedObjectTypeSO] = buttonTransform; // �������� ������� ����� �������� (������� ���� ������� ���� ��������� ������ ���������� �� �������)
    }
}
