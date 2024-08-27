using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ �������� ����(PlacedObject)
/// </summary>
public class ItemSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("���������� ��� ������������")]
    [SerializeField] private Transform _weaponSelectContainer; // ��������� ��� ������ ������ 
    [SerializeField] private Transform _itemSelectContainer; // ��������� ��� ������ ��������   
    [Header("������ ������������")]
    [SerializeField] private Button _weaponButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _weaponButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _itemButtonPanel;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _itemButtonSelectedImage; // ����������� ���������� ������ ��������



    protected override void Awake()
    {
        base.Awake();

        _typeSelectContainerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer };
        _buttonSelectedImageArray = new Image[] { _weaponButtonSelectedImage, _itemButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _weaponButtonPanel.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }); // _armorHeadButtonPanel.onClick.AddListener(delegate { ShowContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _itemButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_itemSelectContainer);
            ShowSelectedButton(_itemButtonSelectedImage);
        });
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;

        if (active)
        {
            ShowContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }
        else
        {
            ClearActiveButtonContainer();
        }
    }

    protected override void CreateSelectButtonsSystemInActiveContainer()
    {
        // ��������� ������ 
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _warehouseManager.GetPlacedObjectWithActionList())
        {
            switch (placedObjectTypeAndCount.placedObjectTypeSO)
            {
                case GrappleTypeSO:
                case ShootingWeaponTypeSO:
                case SwordTypeSO:
                    if (_activeContainer == _weaponSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeAndCount, _weaponSelectContainer);
                    break;

                case GrenadeTypeSO:
                case HealItemTypeSO:
                case ShieldItemTypeSO:
                case SpotterFireItemTypeSO:
                case VisionItemTypeSO:
                    if (_activeContainer == _itemSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeAndCount, _itemSelectContainer);
                    break;
            }
        }
    }
           
    protected override void CreatePlacedObjectSelectButton(PlacedObjectTypeAndCount placedObjectTypeAndCount, Transform containerTransform) 
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeAndCount.placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������ 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // ������ ��� �������� ������� ������� � ������� ����, ��� �� ��� �� ����������� �� ���������� �����
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeAndCount);       
    }   
}
