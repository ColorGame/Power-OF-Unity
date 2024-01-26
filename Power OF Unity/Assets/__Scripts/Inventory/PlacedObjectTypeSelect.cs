using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemTypeSO;

public class PlacedObjectTypeSelect : MonoBehaviour // Выбранный Тип Размещаемого Объекта // Создает кнопки для выбора инвенторя
{
    [SerializeField] private Transform _weaponSelectContainer; // Контейнер для выбора оружия 
    [SerializeField] private Transform _itemSelectContainer; // Контейнер для выбора предмета       
    [SerializeField] private Transform _moduleSelectContainer; // Контейнер для выбора модуля
    [SerializeField] private Button _weaponButtonPanel;  // Кнопка для включения панели оружия
    [SerializeField] private Button _itemButtonPanel;  // Кнопка для включения панели предмета
    [SerializeField] private Button _moduleButtonPanel;  // Кнопка для включения панели модуля
    [SerializeField] private List<PlacedObjectTypeSO> _ignorePlacedObjectList; // Список Объектов которые надо игнорировать при создании кнопок выделения


    private Transform[] _typeSelectContainerArray; // массив контейнеров для выбора оружия предметов и модулей
    private Dictionary<PlacedObjectTypeSO, Transform> _buttonTransformDictionary; // Словарь (Тип Размещаемого Объекта - ключ, Transform- -значение)
    private PlacedObjectTypeListSO _placedObjectTypeListSO; // Список типов Размещаемого Объекта    
    private ScrollRect _scrollRect; //Компонент прокрутки кнопок

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _placedObjectTypeListSO = Resources.Load<PlacedObjectTypeListSO>(typeof(PlacedObjectTypeListSO).Name);    // Загружает ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке ScriptableObjects).
                                                                                                                  // Что бы не ошибиться в имени пойдем другим путем. Создадим экземпляр BuildingTypeListSO (список будет один) и назавем также как и класс, потом для поиска SO будем извлекать имя класса которое совпадает с именем экземпляра
        _buttonTransformDictionary = new Dictionary<PlacedObjectTypeSO, Transform>(); // Инициализируем новый словарь
        _typeSelectContainerArray = new Transform[] { _weaponSelectContainer, _itemSelectContainer, _moduleSelectContainer };
    }

    private void Start()
    {
        CreateTypePlacedObjectButton(); // Создать Кнопки типов Размещаемых объектов


        _weaponButtonPanel.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowContainer(_weaponSelectContainer);
        }); // _weaponButtonPanel.onClick.AddListener(delegate { ShowContainer(_weaponSelectContainer); }); // Еще вариант объявления

        _itemButtonPanel.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowContainer(_itemSelectContainer);
        });

        _moduleButtonPanel.onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowContainer(_moduleSelectContainer);
        });
    }

    private void ShowContainer(Transform typeSelectContainer) // Показать контейнер (в аргумент передаем нужный контейнер кнопок)
    {
        foreach (Transform buttonContainer in _typeSelectContainerArray) // Переберем массив контейнеров
        {
            if (buttonContainer == typeSelectContainer) // Если это переданный нам контейнер
            {
                buttonContainer.gameObject.SetActive(true); // Включим его
            }
            else // В противном случае
            {
                buttonContainer.gameObject.SetActive(false); // Выключим
            }
        }
        _scrollRect.content = (RectTransform)typeSelectContainer; // Установим этот контейнер как контент для прокрутки
    }


    private void CreateTypePlacedObjectButton() // Создать Кнопки типов Размещаемых объектов
    {
        foreach (Transform typeSelectContainer in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in typeSelectContainer)
            {
                Destroy(selectPlacedButton.gameObject); // Удалим игровой объект прикрипленный к Transform
            }
        }

        foreach (PlacedObjectTypeSO placedObjectTypeSO in _placedObjectTypeListSO.list) // Переберем список Типов Размещаемых объектов
        {
            if (_ignorePlacedObjectList.Contains(placedObjectTypeSO)) continue; // Пропустим объекты для которых не надо создавать кнопки
                        
            switch (placedObjectTypeSO) // В зависимости от типа поместим в нужный контейнер
            {
                case WeaponTypeSO weaponTypeSO:
                    CreatePlacedObjectButton(weaponTypeSO, _weaponSelectContainer);// Создать Кнопки Размещаемых объектов и поместим в контейнер
                    break;

                case ItemTypeSO itemTypeSO:
                    CreatePlacedObjectButton(itemTypeSO, _itemSelectContainer);// Создать Кнопки Размещаемых объектов и поместим в контейнер
                    break;

                case ModuleTypeSO moduleTypeSO:
                    CreatePlacedObjectButton(moduleTypeSO, _moduleSelectContainer);// Создать Кнопки Размещаемых объектов и поместим в контейнер
                    break;
            }
        }
    }

    private void CreatePlacedObjectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform) // Создать Кнопки Размещаемых объектов и поместим в контейнер
    {
        Transform buttonTransform = Instantiate(GameAssets.Instance.placedObjectTypeButtonPrefab, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        Transform visualButton = Instantiate(placedObjectTypeSO.visual, buttonTransform); // Создадим Визуал кнопки в зависимости от типа размещаемого объекта и сделаем дочерним к кнопке 
        Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // Найдем все дочернии объекты визуала и изменим слой, что бы они не рендерились за гранимцами маски
        foreach (Transform child in childrenArray)
        {
            child.gameObject.layer = 13;
        }

        buttonTransform.GetComponent<Button>().onClick.AddListener(() => //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            PickUpDropSystem.Instance.CreatePlacedObject(buttonTransform.position, placedObjectTypeSO); // Создадим нужный объект в позиции кнопки                
        });

        MouseEnterExitEventsUI mouseEnterExitEventsUI = buttonTransform.GetComponent<MouseEnterExitEventsUI>(); // Найдем на кнопке компонент - События входа и выхода мышью 

        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // Подпишемся на событие
        {
            TooltipUI.Instance.Show(placedObjectTypeSO.GetToolTip() + "\n" +"характеристики"); // При наведении на кнопку покажем подсказку и передадим текст
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            TooltipUI.Instance.Hide(); // При отведении мыши скроем подсказку
        };

        _buttonTransformDictionary[placedObjectTypeSO] = buttonTransform; // Присвоим каждому ключу значение (Каждому типу объекта свой Трансформ кнопки созданного из шаблона)
    }
}
