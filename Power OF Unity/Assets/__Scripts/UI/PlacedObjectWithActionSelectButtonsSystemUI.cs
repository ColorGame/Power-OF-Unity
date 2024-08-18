using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора Типа Размещаемого Объекта c ДЕЙСТВИЕМ(Action) (Создает кнопки для выбора инвенторя)
/// </summary>
public class PlacedObjectWithActionSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{
    [SerializeField] private Transform _weaponSelectContainer; // Контейнер для выбора оружия 
    [SerializeField] private Transform _itemSelectContainer; // Контейнер для выбора предмета   
    [SerializeField] private Button _weaponButtonPanel;  // Кнопка для включения панели оружия
    [SerializeField] private Image _weaponButtonSelectedImage; // Изображение выделенной кнопки оружия
    [SerializeField] private Button _itemButtonPanel;  // Кнопка для включения панели предмета
    [SerializeField] private Image _itemButtonSelectedImage; // Изображение выделенной кнопки предмета
   // [SerializeField] private List<PlacedObjectTypeWithActionSO> _ignorePlacedObjectList; // Список Объектов которые надо игнорировать при создании кнопок выделения


    private Transform[] _typeSelectContainerArray; // массив контейнеров для выбора оружия предметов и модулей
    private Image[] _buttonSelectedImageArray; // массив изображений для веделения нужной кнопки
    //private Dictionary<PlacedObjectTypeWithActionSO, Transform> _buttonTransformDictionary; // Словарь (Тип Размещаемого Объекта - ключ, Transform- -значение)
    private PlacedObjectTypeListSO _placedObjectTypeListSO; // Список типов Размещаемого Объекта    
    private ScrollRect _scrollRect; //Компонент прокрутки кнопок
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
                _cameraInventoryUI = GetComponentInParent<Camera>(); // Для канваса в мировом пространстве будем использовать отдельную дополнительную камеру
                break;
        }     

        _placedObjectTypeListSO = Resources.Load<PlacedObjectTypeListSO>(typeof(PlacedObjectTypeListSO).Name);    // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                                  // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр BuildingTypeListSO (список будет один) и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра
                                                                                                                  // _buttonTransformDictionary = new Dictionary<PlacedObjectTypeWithActionSO, Transform>(); // Инициализируем новый словарь
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
        _weaponButtonPanel.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }); // _weaponButtonPanel.onClick.AddListener(delegate { ShowContainer(_unitOnMissionContainer); }); // Еще вариант объявления

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
            CreatePlacedObjectSelectButtonsSystem(); // Создать Кнопки типов Размещаемых объектов

            ShowContainer(_weaponSelectContainer);
            ShowSelectedButton(_weaponButtonSelectedImage);
        }
        else
        {
            ClearButtonContainers();
        }
    }

    private void ShowSelectedButton(Image typeButtonSelectedImage) // Показать выделенную кнопку
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // Переберем массив 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// Если это переданное нам изображение то включим его
        }
    }

    private void ShowContainer(Transform typeSelectContainer) // Показать контейнер (в аргумент передаем нужный контейнер кнопок)
    {
        foreach (Transform buttonContainer in _typeSelectContainerArray) // Переберем массив контейнеров
        {
            if (buttonContainer == typeSelectContainer) // Если это переданный нам контейнер
            {
                buttonContainer.gameObject.SetActive(true); // Включим его
                _scrollRect.content = (RectTransform)typeSelectContainer; // Установим этот контейнер как контент для прокрутки
            }
            else // В противном случае
            {
                buttonContainer.gameObject.SetActive(false); // Выключим
            }
        }
    }


    private void CreatePlacedObjectSelectButtonsSystem() // Создать систему кнопок для выбора Размещаемых объектов
    {
        ClearButtonContainers();

        foreach (PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO in _placedObjectTypeListSO.GetPlacedObjectWithActionList()) // Переберем список Типов Размещаемых объектов
        {
           // if (_ignorePlacedObjectList.Contains(PlacedObjectTypeWithActionSO)) continue; // Пропустим объекты для которых не надо создавать кнопки

            switch (placedObjectTypeWithActionSO) // В зависимости от типа поместим в нужный контейнер
            {
                case GrappleTypeSO grappleTypeSO:
                    CreatePlacedObjectButton(grappleTypeSO, _weaponSelectContainer);// Создать Кнопки Размещаемых объектов и поместим в контейнер
                    break;
                case ShootingWeaponTypeSO shootingWeaponTypeSO:
                    CreatePlacedObjectButton(shootingWeaponTypeSO, _weaponSelectContainer);// Создать Кнопки Размещаемых объектов и поместим в контейнер
                    break;
                case GrenadeTypeSO grenadeTypeSO:
                    CreatePlacedObjectButton(grenadeTypeSO, _itemSelectContainer);// Создать Кнопки Размещаемых объектов и поместим в контейнер
                    break;
            }
        }

        _scrollRect.verticalScrollbar.value = 1; // переместим прокрутку панели в верх.
    }

    private void ClearButtonContainers()
    {
        foreach (Transform typeSelectContainer in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in typeSelectContainer)
            {
                Destroy(selectPlacedButton.gameObject); // Удалим игровой объект прикрипленный к Transform
            }
        }
    }
    private void CreatePlacedObjectButton(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO, Transform containerTransform) // Создать Кнопки Размещаемых объектов и поместим в контейнер
    {
        Transform buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeWithActionSO.GetVisual2D(), buttonTransform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // Найдем все дочернии объекты визуала и изменим слой, что бы они не рендерились за гранимцами маски
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        buttonTransform.GetComponent<Button>().onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            _pickUpDrop.CreatePlacedObject(buttonTransform.position, placedObjectTypeWithActionSO); // Создадим нужный объект в позиции кнопки                
        });

        MouseEnterExitEventsUI mouseEnterExitEventsUI = buttonTransform.GetComponent<MouseEnterExitEventsUI>(); // Найдем на кнопке компонент - События входа и выхода мышью 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // Подпишемся на событие
        {
            _tooltipUI.ShowAnchoredPlacedObjectTooltip(placedObjectTypeWithActionSO.GetPlacedObjectTooltip(), (RectTransform)buttonTransform, _cameraInventoryUI); // При наведении на кнопку покажем подсказку и передадим текст и камеру которая рендерит эту кнопку
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // При отведении мыши скроем подсказку
        };

        //  _buttonTransformDictionary[PlacedObjectTypeWithActionSO] = buttonTransform; // Присвоим каждому ключу значение (Каждому типу объекта свой Трансформ кнопки созданного из шаблона)
    }

   
}
