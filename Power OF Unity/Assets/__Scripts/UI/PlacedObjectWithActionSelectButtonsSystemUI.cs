using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ ���� ������������ ������� c ���������(Action) (������� ������ ��� ������ ���������)
/// </summary>
public class PlacedObjectWithActionSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{
    [SerializeField] private Transform _weaponSelectContainer; // ��������� ��� ������ ������ 
    [SerializeField] private Transform _itemSelectContainer; // ��������� ��� ������ ��������   
    [SerializeField] private Button _weaponButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _weaponButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _itemButtonPanel;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _itemButtonSelectedImage; // ����������� ���������� ������ ��������
   // [SerializeField] private List<PlacedObjectTypeWithActionSO> _ignorePlacedObjectList; // ������ �������� ������� ���� ������������ ��� �������� ������ ���������


    private Transform[] _typeSelectContainerArray; // ������ ����������� ��� ������ ������ ��������� � �������
    private Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������
    //private Dictionary<PlacedObjectTypeWithActionSO, Transform> _buttonTransformDictionary; // ������� (��� ������������ ������� - ����, Transform- -��������)
    private PlacedObjectTypeListSO _placedObjectTypeListSO; // ������ ����� ������������ �������    
    private ScrollRect _scrollRect; //��������� ��������� ������
    private Canvas _canvas;
    private Camera _cameraInventoryUI;
    private TooltipUI _tooltipUI;
    private PickUpDropPlacedObject _pickUpDrop;


    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);

        switch (_canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                _cameraInventoryUI = null;
                break;
            case RenderMode.ScreenSpaceCamera:
                break;
            case RenderMode.WorldSpace:
                _cameraInventoryUI = GetComponentInParent<Camera>(); // ��� ������� � ������� ������������ ����� ������������ ��������� �������������� ������
                break;
        }     

        _placedObjectTypeListSO = Resources.Load<PlacedObjectTypeListSO>(typeof(PlacedObjectTypeListSO).Name);    // ��������� ������ ������������ ����, ���������� �� ������ path(����) � ����� Resources(��� ����� � ������ � ����� ScriptableObjects).
                                                                                                                  // ��� �� �� ��������� � ����� ������ ������ �����. �������� ��������� BuildingTypeListSO (������ ����� ����) � ������� ����� ��� � �����, ����� ��� ������ SO ����� ��������� ��� ������ ������� ��������� � ������ ����������
                                                                                                                  // _buttonTransformDictionary = new Dictionary<PlacedObjectTypeWithActionSO, Transform>(); // �������������� ����� �������
        _typeSelectContainerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer };
        _buttonSelectedImageArray = new Image[] { _weaponButtonSelectedImage, _itemButtonSelectedImage };               
    }

    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;

        Setup();
    }

    private void Setup()
    {
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
    }

    public void SetActive(bool active)
    {
       _canvas.enabled = active;
      
        if (active) 
        {
            CreatePlacedObjectSelectButtonsSystem(); // ������� ������ ����� ����������� ��������

            ShowContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }
        else
        {
            ClearButtonContainers();
        }
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
        ClearButtonContainers();

        foreach (PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO in _placedObjectTypeListSO.GetPlacedObjectWithActionList()) // ��������� ������ ����� ����������� ��������
        {
           // if (_ignorePlacedObjectList.Contains(PlacedObjectTypeWithActionSO)) continue; // ��������� ������� ��� ������� �� ���� ��������� ������

            switch (placedObjectTypeWithActionSO) // � ����������� �� ���� �������� � ������ ���������
            {
                case GrappleTypeSO grappleTypeSO:
                    CreatePlacedObjectButton(grappleTypeSO, _weaponSelectContainer);// ������� ������ ����������� �������� � �������� � ���������
                    break;
                case ShootingWeaponTypeSO shootingWeaponTypeSO:
                    CreatePlacedObjectButton(shootingWeaponTypeSO, _weaponSelectContainer);// ������� ������ ����������� �������� � �������� � ���������
                    break;
                case GrenadeTypeSO grenadeTypeSO:
                    CreatePlacedObjectButton(grenadeTypeSO, _itemSelectContainer);// ������� ������ ����������� �������� � �������� � ���������
                    break;
            }
        }

        _scrollRect.verticalScrollbar.value = 1; // ���������� ��������� ������ � ����.
    }

    private void ClearButtonContainers()
    {
        foreach (Transform typeSelectContainer in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in typeSelectContainer)
            {
                Destroy(selectPlacedButton.gameObject); // ������ ������� ������ ������������� � Transform
            }
        }
    }
    private void CreatePlacedObjectButton(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO, Transform containerTransform) // ������� ������ ����������� �������� � �������� � ���������
    {
        Transform buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeWithActionSO.GetVisual2D(), buttonTransform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������ 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // ������ ��� �������� ������� ������� � ������� ����, ��� �� ��� �� ����������� �� ���������� �����
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        buttonTransform.GetComponent<Button>().onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            _pickUpDrop.CreatePlacedObject(buttonTransform.position, placedObjectTypeWithActionSO); // �������� ������ ������ � ������� ������                
        });

        MouseEnterExitEventsUI mouseEnterExitEventsUI = buttonTransform.GetComponent<MouseEnterExitEventsUI>(); // ������ �� ������ ��������� - ������� ����� � ������ ����� 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� �������
        {
            _tooltipUI.ShowAnchoredPlacedObjectTooltip(placedObjectTypeWithActionSO.GetPlacedObjectTooltip(), (RectTransform)buttonTransform, _cameraInventoryUI); // ��� ��������� �� ������ ������� ��������� � ��������� ����� � ������ ������� �������� ��� ������
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
        };

        //  _buttonTransformDictionary[PlacedObjectTypeWithActionSO] = buttonTransform; // �������� ������� ����� �������� (������� ���� ������� ���� ��������� ������ ���������� �� �������)
    }

   
}
