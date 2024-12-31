using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора ПРЕДМЕТА типа(PlacedObject)
/// </summary>
public class ItemSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("Контейнеры для переключения")]
    [SerializeField] private Transform _weaponSelectContainer; // Контейнер для выбора оружия 
    [SerializeField] private Transform _itemSelectContainer; // Контейнер для выбора предмета   
    [Header("Кнопки переключения")]
    [SerializeField] private Button _weaponButton;  // Кнопка для включения панели оружия
    [SerializeField] private Image _weaponButtonSelectedImage; // Изображение выделенной кнопки оружия
    [SerializeField] private Button _itemButton;  // Кнопка для включения панели предмета
    [SerializeField] private Image _itemButtonSelectedImage; // Изображение выделенной кнопки предмета



    protected override void Awake()
    {
        base.Awake();

        _containerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer };
        _buttonSelectedImageArray = new Image[] { _weaponButtonSelectedImage, _itemButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _weaponButton.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowAndUpdateContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }); // _armorHeadButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // Еще вариант объявления

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
        // Переберем список 
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
    /// Создать кнопку выбора размещенного объекта и поместить в контейнер
    /// </summary>
    private void CreatePlacedObjectSelectButton(PlacedObjectTypeSO  placedObjectTypeSO, Transform containerTransform) 
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
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
    }   

    public Transform GetWeaponSelectContainerTransform() {  return _weaponSelectContainer; }
}
