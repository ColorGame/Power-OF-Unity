using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора ОБЪЕКТА
/// </summary>
public abstract class ObjectSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{
    protected Transform[] _containerButtonArray; // массив контейнеров
    protected RectTransform _activeContainer;
    protected Image[] _buttonSelectedImageArray; // массив изображений для выделения нужной кнопки      
    protected ScrollRect _scrollRect; //Компонент прокрутки кнопок
    protected Canvas _canvas;
    protected TooltipUI _tooltipUI;
    protected bool _isActive = true;
    protected bool _firstStart = true;


    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);
    }

    protected virtual void Setup()
    {
        // ClearAllContainer();
        SetDelegateContainerSelectionButton();
    }

    /// <summary>
    /// установить делегат кнопке выбора контейнера
    /// </summary>
    protected abstract void SetDelegateContainerSelectionButton();

    public abstract void SetActive(bool active);

    protected void ShowSelectedButton(Image typeButtonSelectedImage) // Показать выделенную кнопку
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // Переберем массив 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// Если это переданное нам изображение то включим его
        }
    }

    /// <summary>
    ///  Показать (в аргумент передаем нужный контейнер кнопок)<br/>
    ///  При ПЕРВОМ запуске происходит обновление всех контейнеров (создание)
    /// </summary>
    protected void ShowContainer(RectTransform buttonContainer)
    {
        if (_firstStart)
        {
            _firstStart = false;
            UpdateAllContainer(); // При первом запуске обновим все контейнеры
            Show(buttonContainer);
        }
        else
        {
            Show(buttonContainer);
        }
    }

    private void Show(RectTransform buttonContainer)
    {
        foreach (Transform container in _containerButtonArray) // Переберем массив контейнеров
        {
            if (container == buttonContainer) // Если это переданный нам контейнер
            {
                _activeContainer = buttonContainer; // назначим новый активный контейнер
                buttonContainer.gameObject.SetActive(true); // Включим его
                _scrollRect.content = buttonContainer; // Установим этот контейнер как контент для прокрутки
                _scrollRect.verticalScrollbar.value = 1; // переместим прокрутку панели в верх.
            }
            else // В противном случае
            {
                container.gameObject.SetActive(false); // Выключим
            }
        }
    }
    /// <summary>
    /// Обновим все контейнеры<br/>
    /// Удалим и создадим заново
    /// </summary>
    protected void UpdateAllContainer()
    {
        ClearAllContainer();
        CreateSelectButtonsSystemInAllContainer();
    }

    protected void UpdateContainer(RectTransform buttonContainer)
    {
        ClearButtonContainer(buttonContainer);
        CreateSelectButtonsSystemInContainer(buttonContainer);
    }

    /// <summary>
    ///  Скрыть ВСЕ контейнеры c кнопками
    /// </summary>
    protected void HideAllContainerArray()
    {
        foreach (Transform container in _containerButtonArray)
        {
            container.gameObject.SetActive(false);
        }
    }

    /* /// <summary>
     ///  Показать и обновить контейнер (в аргумент передаем нужный контейнер кнопок)
     /// </summary>
     protected void ShowAndUpdateContainer(RectTransform buttonContainer)
     {
         ClearActiveButtonContainer(); // очистим предыдущий активный контейнер       

         foreach (Transform container in _containerButtonArray) // Переберем массив контейнеров
         {
             if (container == buttonContainer) // Если это переданный нам контейнер
             {
                 _activeContainer = buttonContainer; // назначим новый активный контейнер
                 buttonContainer.gameObject.SetActive(true); // Включим его
                 CreateSelectButtonsSystemInActiveContainer();
                 _scrollRect.content = buttonContainer; // Установим этот контейнер как контент для прокрутки
                 _scrollRect.verticalScrollbar.value = 1; // переместим прокрутку панели в верх.
             }
             else // В противном случае
             {
                 container.gameObject.SetActive(false); // Выключим
             }
         }
     }*/

    /*    /// <summary>
        /// Очистим активный контейнер c кнопками
        /// </summary>
        protected virtual void ClearActiveButtonContainer()
        {
            if (_activeContainer != null)
            {
               // ClearButtonContainer(_activeContainer);
                _activeContainer = null;
            }
        }*/
    /// <summary>
    /// Очистить переданный контейнер с кнопками
    /// </summary>
    private void ClearButtonContainer(RectTransform buttonContainer)
    {
        foreach (Transform buttonTransform in buttonContainer)
        {
            Destroy(buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }
    }
    /// <summary>
    /// Очистить все контейнеры
    /// </summary>
    private  void ClearAllContainer()
    {
        foreach (Transform container in _containerButtonArray)
        {
            foreach (Transform selectPlacedButton in container)
            {
                Destroy(selectPlacedButton.gameObject);
            }
        }
    }

    /*/// <summary>
    ///  Создать систему кнопок выбора(объектов) в активном контейнере
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInActiveContainer();*/

    /// <summary>
    ///  Создать систему кнопок выбора(объектов) в переданном контейнере контейнере
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer);
    /// <summary>
    ///  Создать систему кнопок выбора(объектов) во всех контейнерах
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInAllContainer();
}


