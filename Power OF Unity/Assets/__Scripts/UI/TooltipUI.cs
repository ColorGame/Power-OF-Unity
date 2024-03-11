using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Всплывающая подсказка в ПОЛЬЗОВАТЕЛЬСКОМ ИНТЕРФЕЙСЕ (порядок сортировки в canvas зависит от полжения в иерархии, 
/// вкладка находящияся внизу отрисовывается поверх всех - поэтому всплывающ. подсказку держим внизу списка Canvas)
/// </summary>
/// <remarks>
///  У background и text подсказки надо убрать галочку Raycast target - что бы подсказка не мерцала
/// </remarks>
public class TooltipUI : MonoBehaviour
{
    /// <summary>
    /// Ширина подсказки для PlacedObjectTooltip
    /// </summary>
    private const float WIDTH_PLACED_OBJECTP_TOOLTIP = 450f;

    private RectTransform _canvasTooltipRectTransform; // Трансформ холста 
    private RectTransform _tooltipRectTransform; // Трансформ всплывающей подсказки TooltipUI // В ИНСПЕКТОРЕ НАСТРОИТЬ ЯКОРЬ НА НИЖНИЙ ЛЕВЫЙ УГОЛ, это координаты (0, 0) что бы он правильно следовал за мышью 
    private ContentSizeFitter _contentSizeFitter; // Настройки размера контента
    private TextMeshProUGUI _shortTooltipsText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    private TextMeshProUGUI _nameText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    private TextMeshProUGUI _descriptionText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    private TextMeshProUGUI _detailsText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    private TextMeshProUGUI _sideEffectsTooltipsText; // Текст подсказки (тип TextMeshProUGUI надо выбрать и буквами UI)
    //private RectTransform _backgroundRectTransform; // Трансформ заднего фона
    private TooltipTimer _tooltipTimer; // Время отображения подсказки (расширяющий класс)
    private bool _followMouse; // Следовать за мышью   

    private void Awake()
    {
        _canvasTooltipRectTransform = (RectTransform)GetComponentInParent<Canvas>().transform;
        _tooltipRectTransform = GetComponent<RectTransform>();
        _contentSizeFitter = GetComponent<ContentSizeFitter>();
        _shortTooltipsText = transform.Find("shortTooltipsText").GetComponent<TextMeshProUGUI>();
        _nameText = transform.Find("nameText").GetComponent<TextMeshProUGUI>();
        _descriptionText = transform.Find("descriptionText").GetComponent<TextMeshProUGUI>();
        _detailsText = transform.Find("detailsText").GetComponent<TextMeshProUGUI>();
        _sideEffectsTooltipsText = transform.Find("sideEffectsTooltipsText").GetComponent<TextMeshProUGUI>();
        //_backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();

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
        Vector2 anchoredPosition = Input.mousePosition / _canvasTooltipRectTransform.localScale.x; //Позицию мыши Поделим на масштаб холста (Будем использовать только Х компонент т.к. Y Z меняются пропорционально)

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
    /// Показать короткую всплывающую подсказку.
    /// </summary>
    /// <remarks>
    /// TooltipTimer - Время отображения подсказки
    /// </remarks>
    public void ShowShortTooltips(string shortTooltipsText, TooltipTimer tooltipTimer = null)
    {
        gameObject.SetActive(true);
        _tooltipTimer = tooltipTimer;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // размер подсказки по горизонтали подстроиться под размер текста

        EnableShortTooltipsText(); // Включить текст коротких всплывающих подсказок

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // Принудительное обновление текса в этом кадре

        _followMouse = true; // Подсказка будет следовать за мышью
        HandleFollowMouse(); // Обновим расположение подсказки
    }

    /// <summary>
    /// Показать всплывающую подсказку о размещенном объекте.
    /// </summary>
    /// <remarks>
    /// В аргумент передать СЛОТ и КАМЕРУ которая рендерит этот слот
    /// </remarks>
    public void ShowPlacedObjectTooltip(PlacedObjectTooltip placedObjectTooltip, RectTransform slotRectTransform, Camera cameraSlotRender)
    {
        gameObject.SetActive(true);
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Отключим автоматическое выставление ширины
        _tooltipRectTransform.sizeDelta = new Vector2(WIDTH_PLACED_OBJECTP_TOOLTIP, 0); // Установим ширину подсказки (высота - будет выставляться автоматически)

        EnablePlacedObjectTooltip();

        _nameText.SetText(placedObjectTooltip.name);
        _nameText.ForceMeshUpdate();

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

    private void EnableShortTooltipsText()
    {
        _shortTooltipsText.gameObject.SetActive(true);

        _nameText.gameObject.SetActive(false);
        _descriptionText.gameObject.SetActive(false);
        _detailsText.gameObject.SetActive(false);
        _sideEffectsTooltipsText.gameObject.SetActive(false);
    }

    private void EnablePlacedObjectTooltip()
    {
        _shortTooltipsText.gameObject.SetActive(false);

        _nameText.gameObject.SetActive(true);
        _descriptionText.gameObject.SetActive(true);
        _detailsText.gameObject.SetActive(true);
        _sideEffectsTooltipsText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Настроика позиции подсказки относительно переданного слота
    /// </summary>
    private void PositionTooltip(RectTransform slotTransform, Camera cameraSlotRender) 
    {
        // Требуется для обеспечения обновления углов с помощью позиционирующих элементов.
        Canvas.ForceUpdateCanvases(); // Принудительно обновите содержимое всех canvas.

        Vector3[] tooltipCornerArray = new Vector3[4]; //Массив - углы всплывающей подсказки
        _tooltipRectTransform.GetWorldCorners(tooltipCornerArray); // Заполним этот массив (углы созданной подсказки) Получите углы вычисленного прямоугольника в мировом пространстве.
              
        Vector3[] slotCornerArray = new Vector3[4]; //Массив - углы слота на который наводим мыш
        slotTransform.GetWorldCorners(slotCornerArray);

        // Преобразуем координаты относительно камеры которая рендерит этот слот - slotTransform
        slotCornerArray = Array.ConvertAll(slotCornerArray, i => cameraSlotRender.WorldToScreenPoint(i));// Преобразуем и перезапишем массив координат
        Vector3 slotPositionFromCamera = cameraSlotRender.WorldToScreenPoint(slotTransform.position);

        // Где надо расположить подсказку относительно центра якоря слота
        bool below = slotPositionFromCamera.y > Screen.height / 2; // расположить НИЖЕ -если центр слота выше середины экрана
        bool right = slotPositionFromCamera.x < Screen.width / 2; // расположить ПРАВЕЕ -если центр слота левее середины экрана)

        int slotCornerIndex = GetCornerIndex(below, right); //Получим угол слота к которому прицепим подсказку
        int tooltipCornerIndex = GetCornerIndex(!below, !right); // Получим у подсказки противоположный угол

        Vector3 offset = slotCornerArray[slotCornerIndex] - tooltipCornerArray[tooltipCornerIndex]; //Текущее смещение между этими углами - на которое надо сместить якорь подсказки

        Vector2 anchoredPosition = _tooltipRectTransform.anchoredPosition + (Vector2)offset / _canvasTooltipRectTransform.localScale.x;// Вычислим предварительное смещение подсказки // offset Поделим на масштаб холста (Будем использовать только Х компонент т.к. Y Z меняются пропорционально)

        // Позаботимся что бы подсказка всегда оставалась на экране. Будем контролировать только верхнию и нижнию сторону, т.к. правую и левую мы выислили до этого через GetCornerIndex
        if (anchoredPosition.y + _tooltipRectTransform.rect.height > _canvasTooltipRectTransform.rect.height)// Если размер подсказки выходит за верхнию сторону холста то ...
        {
            anchoredPosition.y = _canvasTooltipRectTransform.rect.height - _tooltipRectTransform.rect.height;// Зафиксируем на верхней стороне 
        }
        if (anchoredPosition.y  < 0) 
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
        gameObject.SetActive(false);
    }


    public class TooltipTimer // РАСШИРИМ КЛАСС
    {
        public float timer;
    }

    /*Vector2 textSize = _shortTooltipsText.GetRenderedValues(false); // Получим размер текста (false чтобы учитывать все символы)
       Vector2 padding = new Vector2(15, 15); // Заполнение (что бы был отступ от текста) 
       _backgroundRectTransform.sizeDelta = textSize + padding; // Изменим размер заднего фона в зависимости от длины текста*/
}
