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
    [SerializeField] private Button _weaponButton;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _weaponButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _itemButton;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _itemButtonSelectedImage; // ����������� ���������� ������ ��������



    protected override void Awake()
    {
        base.Awake();

        _containerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer };
        _buttonSelectedImageArray = new Image[] { _weaponButtonSelectedImage, _itemButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _weaponButton.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowAndUpdateContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }); // _armorHeadButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _itemButton.onClick.AddListener(() =>
        {
            ShowAndUpdateContainer(_itemSelectContainer);
            ShowSelectedButton(_itemButtonSelectedImage);
        });
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;
        if (active)
        {
            ShowAndUpdateContainer(_weaponSelectContainer);
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
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case GrappleTypeSO:
                case ShootingWeaponTypeSO:
                case SwordTypeSO:
                    if (_activeContainer == _weaponSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeSO, _weaponSelectContainer);
                    break;

                case GrenadeTypeSO:
                case HealItemTypeSO:
                case ShieldItemTypeSO:
                case SpotterFireItemTypeSO:
                case CombatDroneTypeSO:
                    if (_activeContainer == _itemSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeSO, _itemSelectContainer);
                    break;
            }
        }
    }

    /// <summary>
    /// ������� ������ ������ ������������ ������� � ��������� � ���������
    /// </summary>
    private void CreatePlacedObjectSelectButton(PlacedObjectTypeSO  placedObjectTypeSO, Transform containerTransform) 
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������ 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // ������ ��� �������� ������� ������� � ������� ����, ��� �� ��� �� ����������� �� ���������� �����
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeSO);       
    }   

    public Transform GetWeaponSelectContainerTransform() {  return _weaponSelectContainer; }
}
