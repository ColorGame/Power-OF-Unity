using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора ПРЕДМЕТА типа(PlacedObject)
/// </summary>
public class ItemSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("Контейнеры для переключения")]
    [SerializeField] private RectTransform _weaponSelectContainer; // Контейнер для выбора оружия 
    [SerializeField] private RectTransform _itemSelectContainer; // Контейнер для выбора предмета   
    [SerializeField] private RectTransform _weaponButtonFastTransitionContainer; // Контейнер для кнопок быстрого перехода в меню
    [SerializeField] private RectTransform _itemButtonFastTransitionContainer; // Контейнер для кнопок быстрого перехода в меню
    [Header("Кнопки переключения")]
    [SerializeField] private Button _weaponButton;  // Кнопка для включения панели оружия
    [SerializeField] private Image _weaponButtonSelectedImage; // Изображение выделенной кнопки оружия
    [SerializeField] private Button _itemButton;  // Кнопка для включения панели предмета
    [SerializeField] private Image _itemButtonSelectedImage; // Изображение выделенной кнопки предмета
    [Header("Кнопки для быстрого перехода по меню WEAPON и\nSOобъект к которому надо быстро перейти ")]
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
    [Header("Кнопки для быстрого перехода по меню ITEM и\nSOобъект к которому надо быстро перейти ")]
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
    [Header("Скорость перехода к целевому объекту")]
    [SerializeField] private int _speed = 20;

    private bool _startMove = false;
    private Vector2 _targetPosition = default;

    private Transform[] _containerFastTransitionArray;

    private Dictionary<PlacedObjectTypeSO, RectTransform> _weaponPlacedObjectButtonTransformDict = new(); // Словарь КЛЮЧ - SO размещенного ОРУЖИЯ ЗНАЧЕНИЕ - трансформ кнопки
    private Dictionary<PlacedObjectTypeSO, RectTransform> _itemPlacedObjectButtonTransformDict = new(); // Словарь КЛЮЧ - SO размещенного ПРЕДМЕТА ЗНАЧЕНИЕ - трансформ кнопки
      
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
        _weaponButton.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            SetAndShowContainer(_weaponSelectContainer, _weaponButtonSelectedImage, _weaponPlacedObjectButtonList);
            ShowFastTransitionContainer(_weaponButtonFastTransitionContainer);
        }); // _headArmorButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // Еще вариант объявления

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
        if (_isActive == active) //Если предыдущее состояние тоже то выходим
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
        if (_startMove) // Если надо переместить в начальную позиции и в конце уничтожим объект
        {
            _activeContainer.anchoredPosition = Vector2.Lerp(_activeContainer.anchoredPosition, _targetPosition, Time.deltaTime * _speed);

            float stoppingDistance = 5f; // Дистанция остановки //НУЖНО НАСТРОИТЬ//
            if (Vector2.Distance(_activeContainer.anchoredPosition, _targetPosition) < stoppingDistance)  // Если растояние до целевой позиции меньше чем Дистанция остановки // Мы достигли цели        
            {
                _startMove = false; //прекратить движение Мы достигли _dropPositionWhenDeleted   
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
    /// Создать кнопку выбора размещенного объекта и поместить в контейнер
    /// </summary>
    private PlacedObjectSelectButtonUI CreatePlacedObjectSelectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
       /* AsyncInstantiateOperation<PlacedObjectSelectButtonUI> buttonInstantiateOperation = InstantiateAsync(GameAssetsSO.Instance.placedObjectSelectButton, containerTransform);
        await buttonInstantiateOperation;
        PlacedObjectSelectButtonUI placedObjectSelectButton = buttonInstantiateOperation.Result[0];

        AsyncInstantiateOperation<Transform> visual2DInstantiateOperation = InstantiateAsync(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform);
        await visual2DInstantiateOperation;
        Transform visualButton = visual2DInstantiateOperation.Result[0];*/

         PlacedObjectSelectButtonUI placedObjectSelectButton =  Instantiate(GameAssetsSO.Instance.placedObjectSelectButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // Найдем все дочернии объекты визуала и изменим слой, что бы они не рендерились за гранимцами маски
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeSO);
        return placedObjectSelectButton;
    }

    /// <summary>
    /// Установить целевую позицию для перемещения WeaponSelectContainer
    /// </summary>
    private void SetTargetTransfromWeaponContainer(PlacedObjectTypeSO placedObjectTypeSO)
    {
        SetTargetTransfromSelectContainer(placedObjectTypeSO, _weaponPlacedObjectButtonTransformDict);
    }
    /// <summary>
    /// Установить целевую позицию для перемещения ItemSelectContainer
    /// </summary>
    private void SetTargetTransfromItemContainer(PlacedObjectTypeSO placedObjectTypeSO)
    {
        SetTargetTransfromSelectContainer(placedObjectTypeSO, _itemPlacedObjectButtonTransformDict);
    }

    /// <summary>
    /// Установить целевую позицию для перемещения SelectContainer<br/>
    /// С использованием переданного словоря.
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
                containerTransform.gameObject.SetActive(false); // Выключим
        }
    }

    public void StartAnimation()
    {

    }

    public Transform GetWeaponSelectContainerTransform() { return _weaponSelectContainer; }
}
