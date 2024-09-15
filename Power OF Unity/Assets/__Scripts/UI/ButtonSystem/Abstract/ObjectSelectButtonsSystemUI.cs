using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора ОБЪЕКТА
/// </summary>
public abstract class ObjectSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{
    protected Transform[] _containerArray; // массив контейнеров
    protected Transform _activeContainer;
    protected Image[] _buttonSelectedImageArray; // массив изображений для веделения нужной кнопки      
    protected ScrollRect _scrollRect; //Компонент прокрутки кнопок
    protected Canvas _canvas;
    protected TooltipUI _tooltipUI;
  

    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);       
    }

    protected void Setup()
    {
        ClearAllContainer();
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
    ///  Показать и обновить контейнер (в аргумент передаем нужный контейнер кнопок)
    /// </summary>
    protected void ShowAndUpdateContainer(Transform selectContainer) 
    {
        ClearActiveButtonContainer(); // очистим предыдущий активный контейнер       

        foreach (Transform container in _containerArray) // Переберем массив контейнеров
        {
            if (container == selectContainer) // Если это переданный нам контейнер
            {
                _activeContainer = selectContainer; // назначим новый активный контейнер
                selectContainer.gameObject.SetActive(true); // Включим его
                CreateSelectButtonsSystemInActiveContainer();
                _scrollRect.content = (RectTransform)selectContainer; // Установим этот контейнер как контент для прокрутки
                _scrollRect.verticalScrollbar.value = 1; // переместим прокрутку панели в верх.
            }
            else // В противном случае
            {
                container.gameObject.SetActive(false); // Выключим
            }
        }
    }

    /// <summary>
    /// Очистим активный контейнер c кнопками
    /// </summary>
    protected virtual void ClearActiveButtonContainer()
    {
        if (_activeContainer != null)
        {
            foreach (Transform selectPlacedButton in _activeContainer)
            {
                Destroy(selectPlacedButton.gameObject); // Удалим игровой объект прикрипленный к Transform
            }
            _activeContainer = null;
        }
    }
    /// <summary>
    /// Очистить все контейнеры
    /// </summary>
    protected void ClearAllContainer()
    {
        foreach (Transform container in _containerArray)
        {
            foreach (Transform selectPlacedButton in container)
            {
                Destroy(selectPlacedButton.gameObject);
            }
        }
    }

    /// <summary>
    ///  Создать систему кнопок выбора(объектов) в активном контейнере
    /// </summary>
    protected abstract void CreateSelectButtonsSystemInActiveContainer();
}


