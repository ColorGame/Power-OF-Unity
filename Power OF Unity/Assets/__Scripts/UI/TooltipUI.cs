using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Всплывающая подсказка в ПОЛЬЗОВАТЕЛЬСКОМ ИНТЕРФЕЙСЕ (порядок сортировки в canvas зависит от полжения в иерархии, 
/// вкладка находящияся внизу отрисовывается поверх всех - поэтому всплывающ. подсказку держим внизу списка Canvas)
/// </summary>
/// <remarks>
/// Должен быть прикриплен к PersistentEntryPoint
///  У background и text подсказки надо убрать галочку Raycast target - что бы подсказка не мерцала
/// </remarks>
public class TooltipUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _shortTooltipsText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    [SerializeField] private TextMeshProUGUI _nameText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    [SerializeField] private TextMeshProUGUI _descriptionText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    [SerializeField] private TextMeshProUGUI _detailsText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    [SerializeField] private TextMeshProUGUI _sideEffectsTooltipsText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    private ContentSizeFitter _contentSizeFitter; // Настройки размера контента
    private RectTransform _canvasTooltipRectTransform; // Трансформ холста 
    private RectTransform _tooltipRectTransform; // Трансформ всплывающей подсказки TooltipUI // В ИНСПЕКТОРЕ НАСТРОИТЬ ЯКОРЬ НА НИЖНИЙ ЛЕВЫЙ УГОЛ, это координаты (0, 0) что бы он правильно следовал за мышью 
    //private RectTransform _backgroundRectTransform; // Трансформ заднего фона
    private TooltipTimer _tooltipTimer; // Время отображения подсказки (расширяющий класс)
    private bool _followMouse; // Следовать за мышью   
    private float _widthTooltip; // Ширина подсказки для PlacedObjectTooltip (когда включается всплывающуая подсказка, которая следует за мышью. ширина сбивается)


    private VirtualMouseCustomProvider _virtualMouseCustomProvider;
    private GameInput _gameInput;

    public void Init(GameInput gameInput, VirtualMouseCustomProvider Provider)
    {
        _virtualMouseCustomProvider = Provider;
        _gameInput = gameInput;

        Setup();
    }

    private void Setup()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.sortingOrder = 100; // Установим большой слой сортировки что бы подсказки отображались повер всех канвасов

        _canvasTooltipRectTransform = (RectTransform)canvas.transform;
        _tooltipRectTransform = GetComponent<RectTransform>();
        _contentSizeFitter = GetComponent<ContentSizeFitter>();

        if (_shortTooltipsText == null)
            _shortTooltipsText = transform.Find("shortTooltipsText").GetComponent<TextMeshProUGUI>();
        if (_nameText == null)
            _nameText = transform.Find("nameText").GetComponent<TextMeshProUGUI>();
        if (_descriptionText == null)
            _descriptionText = transform.Find("descriptionText").GetComponent<TextMeshProUGUI>();
        if (_detailsText == null)
            _detailsText = transform.Find("detailsText").GetComponent<TextMeshProUGUI>();
        if (_sideEffectsTooltipsText == null)
            _sideEffectsTooltipsText = transform.Find("sideEffectsTooltipsText").GetComponent<TextMeshProUGUI>();

        _widthTooltip = _tooltipRectTransform.sizeDelta.x; // сохраним оригинальную ширину
        //_backgroundRectTransform = transform.Find("background").CreateInstanceClass<RectTransform>();

        Hide(); // скроем подсказки
    }

    private void OnEnable() // При включении подсказки 
    {
        _followMouse = false;
    }

    private void Update()
    {
        if (_followMouse)
            HandleFollowMouse();


        if (_tooltipTimer != null) // Если задано время то - запустим таймер
        {
            _tooltipTimer.timer -= Time.deltaTime;
            if (_tooltipTimer.timer <= 0) //По истечении времени скроем подсказку
            {
                _tooltipTimer = null;
                _followMouse=false;
                Hide();
            }
        }

    }

    /// <summary>
    /// Обработка следования за мышью
    /// </summary>
    private void HandleFollowMouse()
    {
        // У Canvas - холста есть свое масштабирование Canvas Scale, которое мы настроили 1280*720. При изминении размеров Game сцены происходит изменение Scale(масштаб) на холсте
        // что бы подсказка следовала четко за мышью надо учитывать Scale холста
        // Vector2 anchoredPosition = Input.mousePosition / _canvasTooltipRectTransform.localScale.x; //Позицию мыши Поделим на масштаб холста (Будем использовать только Х компонент т.к. Y Z меняются пропорционально)

        Vector2 anchoredPosition;
        switch (_gameInput.GetActiveGameDevice())
        {
            default:
            case GameInput.GameDevice.KeyboardMouse:
                anchoredPosition = Input.mousePosition / _canvasTooltipRectTransform.localScale.x; //Позицию мыши Поделим на масштаб холста (Будем использовать только Х компонент т.к. Y Z меняются пропорционально)
                break;
            case GameInput.GameDevice.Gamepad:
                anchoredPosition = _virtualMouseCustomProvider.GetVirtualMouseCustom().cursorTransform.anchoredPosition / _canvasTooltipRectTransform.localScale.x;
                break;
        }

        // Позаботимся что бы подсказка всегда оставалась на экране
        if (anchoredPosition.x + _tooltipRectTransform.rect.width > _canvasTooltipRectTransform.rect.width) // Если размер подсказки выходит за правую сторону холста то ...
        {
            anchoredPosition.x = _canvasTooltipRectTransform.rect.width - _tooltipRectTransform.rect.width; // Зафиксируем на правой стороне 
        }
        if (anchoredPosition.y + _tooltipRectTransform.rect.height > _canvasTooltipRectTransform.rect.height)
        {
            anchoredPosition.y = _canvasTooltipRectTransform.rect.height - _tooltipRectTransform.rect.height;
        }

        _tooltipRectTransform.anchoredPosition = anchoredPosition; // Сместим положени якоря "всплывающей подсказки" в полжение мыши
    }

    /// <summary>
    /// Показать короткую всплывающую подсказку, которая следует за мышью.
    /// </summary>
    /// <remarks>
    /// TooltipTimer - Время отображения подсказки
    /// </remarks>
    public void ShowShortTooltipFollowMouse(string shortTooltipsText, TooltipTimer tooltipTimer = null)
    {
        _canvas.enabled = true;
        _tooltipTimer = tooltipTimer;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // размер подсказки по горизонтали подстроиться под размер текста      

        EnableShortTooltipsText(); // Включить текст коротких всплывающих подсказок

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // Принудительное обновление текса в этом кадре

        _followMouse = true; // Подсказка будет следовать за мышью
        HandleFollowMouse(); // Обновим расположение подсказки
    }

    /// <summary>
    /// Показать привязанную(заякаренную) всплывающую подсказку о размещенном объекте.
    /// </summary>
    /// <remarks>
    /// В аргумент передать СЛОТ ,Если слот рендерится СПЕЦИАЛЬНОЙ камерой то предадим и ее. 
    /// </remarks>
    public void ShowAnchoredPlacedObjectTooltip(PlacedObjectTooltip placedObjectTooltip, RectTransform slotRectTransform, Camera cameraSlotRender = null)
    {
        _canvas.enabled = true;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Отключим автоматическое выставление ширины    
        _tooltipRectTransform.sizeDelta = new Vector2(_widthTooltip, 0); // Установим ширину подсказки (высота - будет выставляться автоматически) 

        EnablePlacedObjectTooltip();

        _nameText.SetText(placedObjectTooltip.name);
        _nameText.ForceMeshUpdate();// Принудительное обновление текса в этом кадре

        _descriptionText.SetText(placedObjectTooltip.description);
        _descriptionText.ForceMeshUpdate();

        _detailsText.SetText(placedObjectTooltip.details);
        _detailsText.ForceMeshUpdate();

        _sideEffectsTooltipsText.SetText(placedObjectTooltip.sideEffects);
        _sideEffectsTooltipsText.ForceMeshUpdate();

        _followMouse = false; // Отключим следованию за мыщью. Мы будем фиксировать на углах
        _tooltipTimer = null; // Обнулим таймер. До этого могла гореть подсказка с таймером и если не обнулить то в Update она скроет текущую подсказку
        PositionTooltip(slotRectTransform, cameraSlotRender);
    }

    /// <summary>
    /// Показать привязанную(заякаренную) Короткую всплывающую подсказку
    /// </summary>
    /// <remarks>
    /// В аргумент передать СЛОТ, Если слот рендерится СПЕЦИАЛЬНОЙ камерой то предадим и ее. 
    /// </remarks>
    public void ShowAnchoredShortTooltip(string shortTooltipsText, RectTransform slotRectTransform, Camera cameraSlotRender = null)
    {
        _canvas.enabled = true;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Отключим автоматическое выставление ширины     
        _tooltipRectTransform.sizeDelta = new Vector2(_widthTooltip, 0); // Установим ширину подсказки (высота - будет выставляться автоматически)

        EnableShortTooltipsText();

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // Принудительное обновление текса в этом кадре

        _followMouse = false; // Отключим следованию за мыщью. Мы будем фиксировать на углах
        _tooltipTimer = null; // Обнулим таймер. До этого могла гореть подсказка с таймером и если не обнулить то в Update она скроет текущую подсказку

        PositionTooltip(slotRectTransform, cameraSlotRender);
    }
    /// <summary>
    /// Включить текст короткой всплывающей подски
    /// </summary>
    private void EnableShortTooltipsText()
    {
        _shortTooltipsText.enabled = true;

        _nameText.enabled = false;
        _descriptionText.enabled = false;
        _detailsText.enabled = false;
        _sideEffectsTooltipsText.enabled = false;
    }
    /// <summary>
    /// Включить подсказку о размещенном объекте
    /// </summary>
    private void EnablePlacedObjectTooltip()
    {
        _shortTooltipsText.enabled = false;

        _nameText.enabled = true;
        _descriptionText.enabled = true;
        _detailsText.enabled = true;
        _sideEffectsTooltipsText.enabled = true;
    }

    /// <summary>
    /// Настроика позиции подсказки относительно переданного слота
    /// </summary>
    /// <remarks>
    /// Если слот рендерится СПЕЦИАЛЬНОЙ камерой то предадим ее в аргумент. 
    /// </remarks>
    private void PositionTooltip(RectTransform slotTransform, Camera cameraSlotRender = null)
    {
        // Требуется для обеспечения обновления углов с помощью позиционирующих элементов.
        Canvas.ForceUpdateCanvases(); // Принудительно обновите содержимое всех canvas.

        Vector3[] tooltipCornerArray = new Vector3[4]; //Массив - углы всплывающей подсказки
        _tooltipRectTransform.GetWorldCorners(tooltipCornerArray); // Заполним этот массив (углы созданной подсказки) Получите углы вычисленного прямоугольника в мировом пространстве.

        Vector3[] slotCornerArray = new Vector3[4]; //Массив - углы слота на который наводим мыш
        slotTransform.GetWorldCorners(slotCornerArray);

        bool below;
        bool right;
        if (cameraSlotRender == null)
        {
            // Где надо расположить подсказку относительно центра якоря слота
            below = slotTransform.position.y > Screen.height / 2; // расположить НИЖЕ -если центр слота выше середины экрана
            right = slotTransform.position.x < Screen.width / 2; // расположить ПРАВЕЕ -если центр слота левее середины экрана)
        }
        else
        {
            // Преобразуем координаты относительно камеры которая рендерит этот слот - slotTransform
            slotCornerArray = Array.ConvertAll(slotCornerArray, i => cameraSlotRender.WorldToScreenPoint(i));// Преобразуем и перезапишем массив координат
            Vector3 slotPositionFromCamera = cameraSlotRender.WorldToScreenPoint(slotTransform.position);

            // Где надо расположить подсказку относительно центра якоря слота
            below = slotPositionFromCamera.y > Screen.height / 2; // расположить НИЖЕ -если центр слота выше середины экрана
            right = slotPositionFromCamera.x < Screen.width / 2; // расположить ПРАВЕЕ -если центр слота левее середины экрана)
        }

        int slotCornerIndex = GetCornerIndex(below, right); //Получим угол слота к которому прицепим подсказку
        int tooltipCornerIndex = GetCornerIndex(!below, !right); // Получим у подсказки противоположный угол

        Vector3 offset = slotCornerArray[slotCornerIndex] - tooltipCornerArray[tooltipCornerIndex]; //Текущее смещение между этими углами - на которое надо сместить якорь подсказки

        Vector2 anchoredPosition = _tooltipRectTransform.anchoredPosition + (Vector2)offset / _canvasTooltipRectTransform.localScale.x;// Вычислим предварительное смещение подсказки // _offset Поделим на масштаб холста (Будем использовать только Х компонент т.к. Y Z меняются пропорционально)

        // Позаботимся что бы подсказка всегда оставалась на экране. Будем контролировать только верхнию и нижнию сторону, т.к. правую и левую мы выислили до этого через GetCornerIndex
        if (anchoredPosition.y + _tooltipRectTransform.rect.height > _canvasTooltipRectTransform.rect.height)// Если размер подсказки выходит за верхнию сторону холста то ...
        {
            anchoredPosition.y = _canvasTooltipRectTransform.rect.height - _tooltipRectTransform.rect.height;// Зафиксируем на верхней стороне 
        }
        if (anchoredPosition.y < 0)
        {
            anchoredPosition.y = 0;
        }

        _tooltipRectTransform.anchoredPosition = anchoredPosition; // Сместим положени якоря "всплывающей подсказки"
    }

    private int GetCornerIndex(bool below, bool right) // Получим нужный угол 
    {
        if (below && !right) return 0;          //нижний левый
        else if (!below && !right) return 1;    //верхний левый
        else if (!below && right) return 2;     //верхний правый
        else return 3;                          //нижний правый
    }

    public void Hide() // Скрытие подсказки
    {
        _canvas.enabled = false;
    }


    public class TooltipTimer // РАСШИРИМ КЛАСС
    {
        public float timer;
    }

    /*Vector2 textSize = _shortTooltipsText.GetRenderedValues(false); // Получим размер текста (false чтобы учитывать все символы)
       Vector2 padding = new Vector2(15, 15); // Заполнение (что бы был отступ от текста) 
       _backgroundRectTransform.sizeDelta = textSize + padding; // Изменим размер заднего фона в зависимости от длины текста*/
}
