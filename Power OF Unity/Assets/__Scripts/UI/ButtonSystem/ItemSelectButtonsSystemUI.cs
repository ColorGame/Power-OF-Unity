using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ �������� ����(PlacedObject)
/// </summary>
public class ItemSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("���������� ��� ������������")]
    [SerializeField] private RectTransform _weaponSelectContainer; // ��������� ��� ������ ������ 
    [SerializeField] private RectTransform _itemSelectContainer; // ��������� ��� ������ ��������   
    [SerializeField] private RectTransform _weaponButtonFastTransitionContainer; // ��������� ��� ������ �������� �������� � ����
    [SerializeField] private RectTransform _itemButtonFastTransitionContainer; // ��������� ��� ������ �������� �������� � ����
    [Header("������ ������������")]
    [SerializeField] private Button _weaponButton;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _weaponButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _itemButton;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _itemButtonSelectedImage; // ����������� ���������� ������ ��������
    [Header("������ ��� �������� �������� �� ���� WEAPON �\nSO������ � �������� ���� ������ ������� ")]
    [SerializeField] private Button _grapplingButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _grapplingHookGun;
    [SerializeField] private Button _pistolButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _weaponPistol;
    [SerializeField] private Button _SMGButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _weaponSMG;
    [SerializeField] private Button _rifleButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _weaponRifle;
    [SerializeField] private Button _shotgunButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _weaponShotgun;
    [SerializeField] private Button _sniperButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _weaponSniper;
    [SerializeField] private Button _swordButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _sword;
    [Header("������ ��� �������� �������� �� ���� ITEM �\nSO������ � �������� ���� ������ ������� ")]
    [SerializeField] private Button _grenadeButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _grenade;
    [SerializeField] private Button _healButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _heal;
    [SerializeField] private Button _shieldButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _shield;
    [SerializeField] private Button _spotterFireButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _spotterFire;
    [SerializeField] private Button _combatDroneButtonPanel;
    [SerializeField] private PlacedObjectTypeSO _combatDrone;
    [Header("�������� �������� � �������� �������")]
    [SerializeField] private int _speed = 20;

    private bool _startMove = false;
    private Vector2 _targetPosition = default;

    private Transform[] _containerFastTransitionArray;

    private Dictionary<PlacedObjectTypeSO, RectTransform> _weaponPlacedObjectButtonTransformDict = new(); // ������� ���� - SO ������������ ������ �������� - ��������� ������
    private Dictionary<PlacedObjectTypeSO, RectTransform> _itemPlacedObjectButtonTransformDict = new(); // ������� ���� - SO ������������ �������� �������� - ��������� ������
      
    private List<PlacedObjectSelectButtonUI> _weaponPlacedObjectButtonList = new();
    private List<PlacedObjectSelectButtonUI> _itemPlacedObjectButtonList = new();

    protected override void Awake()
    {
        base.Awake();

        _containerButtonArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer };
        _containerFastTransitionArray = new Transform[] { _weaponButtonFastTransitionContainer, _itemButtonFastTransitionContainer };
        _buttonSelectedImageArray = new Image[] { _weaponButtonSelectedImage, _itemButtonSelectedImage };       
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _weaponButton.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            SetAndShowContainer(_weaponSelectContainer, _weaponButtonSelectedImage, _weaponPlacedObjectButtonList);
            ShowFastTransitionContainer(_weaponButtonFastTransitionContainer);
        }); // _headArmorButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _itemButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_itemSelectContainer, _itemButtonSelectedImage, _itemPlacedObjectButtonList);
            ShowFastTransitionContainer(_itemButtonFastTransitionContainer);
        });

        _grapplingButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_grapplingHookGun); });
        _pistolButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_weaponPistol); });
        _SMGButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_weaponSMG); });
        _rifleButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_weaponRifle); });
        _shotgunButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_weaponShotgun); });
        _sniperButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_weaponSniper); });
        _swordButtonPanel.onClick.AddListener(() => { SetTargetTransfromWeaponContainer(_sword); });

        _grenadeButtonPanel.onClick.AddListener(() => { SetTargetTransfromItemContainer(_grenade); });
        _healButtonPanel.onClick.AddListener(() => { SetTargetTransfromItemContainer(_heal); });
        _shieldButtonPanel.onClick.AddListener(() => { SetTargetTransfromItemContainer(_shield); });
        _spotterFireButtonPanel.onClick.AddListener(() => { SetTargetTransfromItemContainer(_spotterFire); });
        _combatDroneButtonPanel.onClick.AddListener(() => { SetTargetTransfromItemContainer(_combatDrone); });
    }

    public override void SetActive(bool active)
    {
        if (_isActive == active) //���� ���������� ��������� ���� �� �������
            return;

        _isActive = active;

        _canvas.enabled = active;
        if (active)
        {
            SetAndShowContainer(_weaponSelectContainer, _weaponButtonSelectedImage, _weaponPlacedObjectButtonList);
            ShowFastTransitionContainer(_weaponButtonFastTransitionContainer);
        }
        else
        {
            SetActiveButtonList(false);
            HideAllContainerArray();
        }
    }

    private void Update()
    {
        if (_startMove) // ���� ���� ����������� � ��������� ������� � � ����� ��������� ������
        {
            _activeContainer.anchoredPosition = Vector2.Lerp(_activeContainer.anchoredPosition, _targetPosition, Time.deltaTime * _speed);

            float stoppingDistance = 5f; // ��������� ��������� //����� ���������//
            if (Vector2.Distance(_activeContainer.anchoredPosition, _targetPosition) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
            {
                _startMove = false; //���������� �������� �� �������� _dropPositionWhenDeleted   
            }
        }
    }

   

    protected override void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer)
    {
        if (buttonContainer == _weaponSelectContainer)
        {
            _weaponPlacedObjectButtonList.Clear();
            _weaponPlacedObjectButtonTransformDict.Clear();
            foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
            {
                switch (placedObjectTypeSO)
                {
                    case GrappleTypeSO:
                    case ShootingWeaponTypeSO:
                    case SwordTypeSO:
                        PlacedObjectSelectButtonUI placedObjectSelectButton = CreatePlacedObjectSelectButton(placedObjectTypeSO, _weaponSelectContainer);
                        _weaponPlacedObjectButtonList.Add(placedObjectSelectButton);
                        _weaponPlacedObjectButtonTransformDict[placedObjectTypeSO] = placedObjectSelectButton.GetComponent<RectTransform>();
                        break;
                }
            }
        }
        if (buttonContainer == _itemSelectContainer)
        {
            _itemPlacedObjectButtonList.Clear();
            _itemPlacedObjectButtonTransformDict.Clear();
            foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
            {
                switch (placedObjectTypeSO)
                {
                    case GrenadeTypeSO:
                    case HealItemTypeSO:
                    case ShieldItemTypeSO:
                    case SpotterFireItemTypeSO:
                    case CombatDroneTypeSO:
                        PlacedObjectSelectButtonUI placedObjectSelectButton = CreatePlacedObjectSelectButton(placedObjectTypeSO, _itemSelectContainer);
                        _itemPlacedObjectButtonList.Add(placedObjectSelectButton);
                        _itemPlacedObjectButtonTransformDict[placedObjectTypeSO] = placedObjectSelectButton.GetComponent<RectTransform>();
                        break;
                }
            }
        }
    }

    protected override  void CreateSelectButtonsSystemInAllContainer()
    {
        _weaponPlacedObjectButtonList.Clear();
        _itemPlacedObjectButtonList.Clear();
        _weaponPlacedObjectButtonTransformDict.Clear();
        _itemPlacedObjectButtonTransformDict.Clear();
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case GrappleTypeSO:
                case ShootingWeaponTypeSO:
                case SwordTypeSO:
                    PlacedObjectSelectButtonUI placedObjectSelectButton =  CreatePlacedObjectSelectButton(placedObjectTypeSO, _weaponSelectContainer);
                    _weaponPlacedObjectButtonList.Add(placedObjectSelectButton);
                    _weaponPlacedObjectButtonTransformDict[placedObjectTypeSO] = placedObjectSelectButton.GetComponent<RectTransform>();
                    break;
                case GrenadeTypeSO:
                case HealItemTypeSO:
                case ShieldItemTypeSO:
                case SpotterFireItemTypeSO:
                case CombatDroneTypeSO:
                    placedObjectSelectButton =  CreatePlacedObjectSelectButton(placedObjectTypeSO, _itemSelectContainer);
                    _itemPlacedObjectButtonList.Add(placedObjectSelectButton);
                    _itemPlacedObjectButtonTransformDict[placedObjectTypeSO] = placedObjectSelectButton.GetComponent<RectTransform>();
                    break;
            }
        }
    }    

    /// <summary>
    /// ������� ������ ������ ������������ ������� � ��������� � ���������
    /// </summary>
    private PlacedObjectSelectButtonUI CreatePlacedObjectSelectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
       /* AsyncInstantiateOperation<PlacedObjectSelectButtonUI> buttonInstantiateOperation = InstantiateAsync(GameAssetsSO.Instance.placedObjectSelectButton, containerTransform);
        await buttonInstantiateOperation;
        PlacedObjectSelectButtonUI placedObjectSelectButton = buttonInstantiateOperation.Result[0];

        AsyncInstantiateOperation<Transform> visual2DInstantiateOperation = InstantiateAsync(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform);
        await visual2DInstantiateOperation;
        Transform visualButton = visual2DInstantiateOperation.Result[0];*/

         PlacedObjectSelectButtonUI placedObjectSelectButton =  Instantiate(GameAssetsSO.Instance.placedObjectSelectButton, containerTransform); // �������� ������ � ������� �������� � ���������
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
        return placedObjectSelectButton;
    }

    /// <summary>
    /// ���������� ������� ������� ��� ����������� WeaponSelectContainer
    /// </summary>
    private void SetTargetTransfromWeaponContainer(PlacedObjectTypeSO placedObjectTypeSO)
    {
        SetTargetTransfromSelectContainer(placedObjectTypeSO, _weaponPlacedObjectButtonTransformDict);
    }
    /// <summary>
    /// ���������� ������� ������� ��� ����������� ItemSelectContainer
    /// </summary>
    private void SetTargetTransfromItemContainer(PlacedObjectTypeSO placedObjectTypeSO)
    {
        SetTargetTransfromSelectContainer(placedObjectTypeSO, _itemPlacedObjectButtonTransformDict);
    }

    /// <summary>
    /// ���������� ������� ������� ��� ����������� SelectContainer<br/>
    /// � �������������� ����������� �������.
    /// </summary>
    private void SetTargetTransfromSelectContainer(PlacedObjectTypeSO placedObjectTypeSO, Dictionary<PlacedObjectTypeSO, RectTransform> placedObjectButtonTransformDict)
    {
        RectTransform buttonTransform = placedObjectButtonTransformDict[placedObjectTypeSO];
        float y = Mathf.Abs(buttonTransform.anchoredPosition.y) - buttonTransform.sizeDelta.y / 2;
        _targetPosition = new Vector2(0, y);
        _startMove = true;
    }

    private void ShowFastTransitionContainer(Transform fastTransitionContainer)
    {
        foreach (Transform containerTransform in _containerFastTransitionArray)
        {
            if (containerTransform == fastTransitionContainer)
                containerTransform.gameObject.SetActive(true);
            else
                containerTransform.gameObject.SetActive(false); // ��������
        }
    }

    public void StartAnimation()
    {

    }

    public Transform GetWeaponSelectContainerTransform() { return _weaponSelectContainer; }
}
