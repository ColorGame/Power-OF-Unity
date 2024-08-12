using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI вкладки "Менеджер Юнитов". Изминение статуса юнитов(база/миссия), вербовка и увольнение
/// </summary>
public class UnitManagerTabUI : MonoBehaviour
{
    [SerializeField] private Button _myUnitsButtonPanel;  // Кнопка для включения панели МОИ ЮНИТЫ
    [SerializeField] private Image _myUnitsButtonSelectedImage; // Изображение выделенной кнопки 
    [SerializeField] private Button _hireUnitButtonPanel;  // Кнопка для включения панели НАЙМА ЮНИТОВ
    [SerializeField] private Image _hireUnitButtonSelectedImage; // Изображение выделенной кнопки 
    [SerializeField] private Transform _myUnitsContainer; // Контейнер МОИХ ЮНИТОВ
    [SerializeField] private TextMeshProUGUI _myUnitsText; // Текст заголовка
    [SerializeField] private Transform _hireContainer; // Контейнер ЮНИТОВ ДЛЯ НАЙМА
    [SerializeField] private TextMeshProUGUI _hireText; // Текст заголовка

    private Transform[] _сontainerArray; // массив контейнеров 
    private Image[] _buttonSelectedImageArray; // массив изображений для веделения нужной кнопки
    private TextMeshProUGUI[] _headerContainerTextArray; // Массив текстов заголовок контейнеров
    private UnitManager _unitManager;
    private UnitInventorySystem _unitInventorySystem;
    private ScrollRect _scrollRect; //Компонент прокрутки кнопок    
   
    private TooltipUI _tooltipUI;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();       

        _сontainerArray = new Transform[] { _hireContainer, _myUnitsContainer };
        _buttonSelectedImageArray = new Image[] { _hireUnitButtonSelectedImage, _myUnitsButtonSelectedImage };
        _headerContainerTextArray = new TextMeshProUGUI[] { _hireText, _myUnitsText };

        // МОИ ЮНИТЫ отображаются при старте
        ShowContainer(_myUnitsContainer);
        ShowSelectedButton(_myUnitsButtonSelectedImage);
        ShowHeaderContainerText(_myUnitsText);
    }

    public void Init(UnitManager unitManager, UnitInventorySystem unitInventorySystem)
    {
        _unitManager = unitManager;
        _unitInventorySystem = unitInventorySystem;

        Setup();
    }

    private void Setup()
    {
        CreateUnitSelectButtonsSystem(); // Создать Кнопки типов Размещаемых объектов

        _hireUnitButtonPanel.onClick.AddListener(() =>//Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {
            ShowContainer(_hireContainer);
            ShowSelectedButton(_hireUnitButtonSelectedImage);
            ShowHeaderContainerText(_hireText);
        });

        _myUnitsButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_myUnitsContainer);
            ShowSelectedButton(_myUnitsButtonSelectedImage);
            ShowHeaderContainerText(_myUnitsText);
        });


    }

    private void OnEnable()
    {
        if (_unitManager != null) // При повторном включении обновим кнопки выбора юнита (первый раз она не запуститься т.к. _unitManager= null)
        {
            CreateUnitSelectButtonsSystem(); // Создать Кнопки типов Размещаемых объектов
        }
    }

    private void ShowContainer(Transform typeSelectContainer) // Показать контейнер (в аргумент передаем нужный контейнер кнопок)
    {
        foreach (Transform buttonContainer in _сontainerArray) // Переберем массив контейнеров
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

    private void ShowSelectedButton(Image typeButtonSelectedImage) // Показать визуализацию выбора кнопки
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // Переберем массив 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// Если это переданное нам изображение то включим его
        }
    }

    private void ShowHeaderContainerText(TextMeshProUGUI typeHeaderContainerText) // Показать текст заголовка контейнера
    {
        foreach (TextMeshProUGUI headerContainerText in _headerContainerTextArray) // Переберем массив 
        {
            headerContainerText.enabled = (headerContainerText == typeHeaderContainerText);// Если это переданное нам объект то включим его
        }
    }

    private void CreateUnitSelectButtonsSystem() // Создать систему кнопок для выбора Юнита
    {
        foreach (Transform сontainer in _сontainerArray)
        {
            foreach (Transform unitSelectButton in сontainer) // Переберем все трансформы в нашем контейнере
            {
                Destroy(unitSelectButton.gameObject); // Удалим игровой объект прикрипленный к Transform
            }
        }

        List<Unit> unitFriendOnBarrackList = _unitManager.GetUnitFriendOnBarrackList();// список  моих юнитов в казарме
        List<Unit> unitFriendOnMissionList = _unitManager.GetUnitFriendOnMissionList();// список  моих юнитов на миссии

        for (int index = 0; index < unitFriendOnBarrackList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendOnBarrackList[index], _myUnitsContainer, index + 1);
        }

        for (int index = 0; index < unitFriendOnMissionList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendOnMissionList[index], _hireContainer, index + 1);
        }

        _scrollRect.verticalScrollbar.value = 1f; // переместим прокрутку панели в верх.
    }


    private void CreateUnitSelectButton(Unit unit, Transform containerTransform, int index) // Создать Кнопку Размещаемых объектов и поместим в контейнер
    {
        UnitSelectAtInventoryButton unitSelectAtInventoryButton = Instantiate(GameAssets.Instance.unitSelectAtInventoryButton, containerTransform); // Создадим кнопку и сделаем дочерним к контенеру
        unitSelectAtInventoryButton.Init(unit, _unitInventorySystem, index);
    }
}
