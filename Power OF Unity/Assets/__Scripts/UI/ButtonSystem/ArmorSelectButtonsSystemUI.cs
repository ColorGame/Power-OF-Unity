using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора БРОНИ типа(PlacedObject)
/// </summary>
public class ArmorSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("Контейнеры для переключения")]
    [SerializeField] private RectTransform _headArmorSelectContainer; // Контейнер для выбора шлема 
    [SerializeField] private RectTransform _bodyArmorSelectContainer; // Контейнер для выбора бронежилета   
    [Header("Кнопки переключения")]
    [SerializeField] private Button _headArmorButton;  // Кнопка для включения панели оружия
    [SerializeField] private Image _headArmorButtonSelectedImage; // Изображение выделенной кнопки оружия
    [SerializeField] private Button _bodyArmorButton;  // Кнопка для включения панели предмета
    [SerializeField] private Image _bodyArmorButtonSelectedImage; // Изображение выделенной кнопки предмета


    private List<PlacedObjectSelectButtonUI> _headArmorPlacedObjectButtonList = new();
    private List<PlacedObjectSelectButtonUI> _bodyArmorPlacedObjectButtonList = new();

    protected override void Awake()
    {
        base.Awake();

        _containerButtonArray = new Transform[] { _headArmorSelectContainer, _bodyArmorSelectContainer };
        _buttonSelectedImageArray = new Image[] { _headArmorButtonSelectedImage, _bodyArmorButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _headArmorButton.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            SetAndShowContainer(_headArmorSelectContainer, _headArmorButtonSelectedImage, _headArmorPlacedObjectButtonList);
        }); // _headArmorButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // Еще вариант объявления

        _bodyArmorButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_bodyArmorSelectContainer, _bodyArmorButtonSelectedImage, _bodyArmorPlacedObjectButtonList);
        });
    }

    public override void SetActive(bool active)
    {
        if (_isActive == active) //Если предыдущее состояние тоже то выходим
            return;

        _isActive = active;

        _canvas.enabled = active;
        if (active)
        {
            SetAndShowContainer(_bodyArmorSelectContainer, _bodyArmorButtonSelectedImage, _bodyArmorPlacedObjectButtonList);
        }
        else
        {
            HideAllContainerArray();
        }
    }
  
    protected override void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer)
    {

        if (buttonContainer == _headArmorSelectContainer)
        {
            _headArmorPlacedObjectButtonList.Clear();
            foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
            {
                switch (placedObjectTypeSO)
                {
                    case HeadArmorTypeSO:
                        _headArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _headArmorSelectContainer));
                        break;
                }
            }
        }
        if (buttonContainer == _bodyArmorSelectContainer)
        {
            _bodyArmorPlacedObjectButtonList.Clear();
            foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
            {
                switch (placedObjectTypeSO)
                {
                    case BodyArmorTypeSO:
                        _bodyArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _bodyArmorSelectContainer));
                        break;
                }
            }
        }
    }

    protected override void CreateSelectButtonsSystemInAllContainer()
    {
        _headArmorPlacedObjectButtonList.Clear();
        _bodyArmorPlacedObjectButtonList.Clear();
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case HeadArmorTypeSO:
                    _headArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _headArmorSelectContainer));
                    break;
                case BodyArmorTypeSO:
                    _bodyArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _bodyArmorSelectContainer));
                    break;
            }
        }
    }

    /// <summary>
    /// Создать кнопку выбора размещенного объекта и поместить в контейнер
    /// </summary>
    private PlacedObjectSelectButtonUI CreatePlacedObjectSelectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssetsSO.Instance.placedObjectSelectButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке               

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeSO);
        return placedObjectSelectButton;
    }
}
