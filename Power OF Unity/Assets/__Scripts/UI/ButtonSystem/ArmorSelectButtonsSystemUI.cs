using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора БРОНИ типа(PlacedObject)
/// </summary>
public class ArmorSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("Контейнеры для переключения")]
    [SerializeField] private Transform _armorHeadSelectContainer; // Контейнер для выбора шлема 
    [SerializeField] private Transform _armorBodySelectContainer; // Контейнер для выбора бронежилета   
    [Header("Кнопки переключения")]
    [SerializeField] private Button _armorHeadButtonPanel;  // Кнопка для включения панели оружия
    [SerializeField] private Image _armorHeadButtonSelectedImage; // Изображение выделенной кнопки оружия
    [SerializeField] private Button _armorBodyButtonPanel;  // Кнопка для включения панели предмета
    [SerializeField] private Image _armorBodyButtonSelectedImage; // Изображение выделенной кнопки предмета



    protected override void Awake()
    {
        base.Awake();

        _typeSelectContainerArray = new Transform[] { _armorHeadSelectContainer, _armorBodySelectContainer };
        _buttonSelectedImageArray = new Image[] { _armorHeadButtonSelectedImage, _armorBodyButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _armorHeadButtonPanel.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowContainer(_armorHeadSelectContainer);
            ShowSelectedButton(_armorHeadButtonSelectedImage);
        }); // _armorHeadButtonPanel.onClick.AddListener(delegate { ShowContainer(_unitOnMissionContainer); }); // Еще вариант объявления

        _armorBodyButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_armorBodySelectContainer);
            ShowSelectedButton(_armorBodyButtonSelectedImage);
        });
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;

        if (active)
        {
            ShowContainer(_armorHeadSelectContainer);
            ShowSelectedButton(_armorHeadButtonSelectedImage);
        }
        else
        {
            ClearActiveButtonContainer();
        }
    }

    protected override void CreateSelectButtonsSystemInActiveContainer()
    {
        // Переберем список 
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _warehouseManager.GetPlacedObjectWithActionList())
        {
            switch (placedObjectTypeAndCount.placedObjectTypeSO)
            {
                case ArmorHeadTypeSO:               
                    if (_activeContainer == _armorHeadSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeAndCount, _armorHeadSelectContainer);
                    break;
               
                case ArmorBodyTypeSO:
                    if (_activeContainer == _armorBodySelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeAndCount, _armorBodySelectContainer);
                    break;
            }
        }
    }

    protected override void CreatePlacedObjectSelectButton(PlacedObjectTypeAndCount placedObjectTypeAndCount, Transform containerTransform)
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeAndCount.placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // Найдем все дочернии объекты визуала и изменим слой, что бы они не рендерились за гранимцами маски
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeAndCount);
    }
}
