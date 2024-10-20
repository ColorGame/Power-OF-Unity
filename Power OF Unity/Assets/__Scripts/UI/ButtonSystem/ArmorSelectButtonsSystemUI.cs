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
    [SerializeField] private Button _armorHeadButton;  // Кнопка для включения панели оружия
    [SerializeField] private Image _armorHeadButtonSelectedImage; // Изображение выделенной кнопки оружия
    [SerializeField] private Button _armorBodyButton;  // Кнопка для включения панели предмета
    [SerializeField] private Image _armorBodyButtonSelectedImage; // Изображение выделенной кнопки предмета



    protected override void Awake()
    {
        base.Awake();

        _containerArray = new Transform[] { _armorHeadSelectContainer, _armorBodySelectContainer };
        _buttonSelectedImageArray = new Image[] { _armorHeadButtonSelectedImage, _armorBodyButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _armorHeadButton.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowAndUpdateContainer(_armorHeadSelectContainer);
            ShowSelectedButton(_armorHeadButtonSelectedImage);
        }); // _armorHeadButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // Еще вариант объявления

        _armorBodyButton.onClick.AddListener(() =>
        {
            ShowAndUpdateContainer(_armorBodySelectContainer);
            ShowSelectedButton(_armorBodyButtonSelectedImage);
        });
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;
        if (active)
        {
            ShowAndUpdateContainer(_armorHeadSelectContainer);
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
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case HeadArmorTypeSO:               
                    if (_activeContainer == _armorHeadSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeSO, _armorHeadSelectContainer);
                    break;
               
                case BodyArmorTypeSO:
                    if (_activeContainer == _armorBodySelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeSO, _armorBodySelectContainer);
                    break;
            }
        }
    }

    /// <summary>
    /// Создать кнопку выбора размещенного объекта и поместить в контейнер
    /// </summary>
    private void CreatePlacedObjectSelectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке               

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeSO);
    }
}
