using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Система кнопок - выбора ПРЕДМЕТА типа(PlacedObject)
/// </summary>
public abstract class PlacedObjectSelectButtonsSystemUI : MonoBehaviour, IToggleActivity
{


    protected Transform[] _typeSelectContainerArray; // массив контейнеров для выбора типа (PlacedObject)
    protected Transform _activeContainer;
    protected Image[] _buttonSelectedImageArray; // массив изображений для веделения нужной кнопки      
    protected ScrollRect _scrollRect; //Компонент прокрутки кнопок
    protected Canvas _canvas;
    protected Camera _cameraEquipmentUI;
    protected TooltipUI _tooltipUI;
    protected PickUpDropPlacedObject _pickUpDrop;
    protected WarehouseManager _warehouseManager;

    protected virtual void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);

        switch (_canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                _cameraEquipmentUI = null;
                break;
            case RenderMode.ScreenSpaceCamera:
            case RenderMode.WorldSpace:
                _cameraEquipmentUI = GetComponentInParent<Camera>(); // Для канваса в мировом пространстве будем использовать отдельную дополнительную камеру
                break;
        }
    }

    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop, WarehouseManager warehouseManager)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;
        _warehouseManager = warehouseManager;
        Setup();
    }

    private void Setup()
    {
        ClearAllContainer();
        SetDelegateContainerSelectionButton();
    }

    /// <summary>
    /// установить делегат кнопке выбора контейнера
    /// </summary>
    protected virtual void SetDelegateContainerSelectionButton() { }

    public virtual void SetActive(bool active) { }

    protected void ShowSelectedButton(Image typeButtonSelectedImage) // Показать выделенную кнопку
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // Переберем массив 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// Если это переданное нам изображение то включим его
        }
    }

    /// <summary>
    ///  Показать контейнер (в аргумент передаем нужный контейнер кнопок)
    /// </summary>
    protected void ShowContainer(Transform typeSelectContainer) 
    {
        ClearActiveButtonContainer(); // очистим предыдущий активный контейнер       

        foreach (Transform container in _typeSelectContainerArray) // Переберем массив контейнеров
        {
            if (container == typeSelectContainer) // Если это переданный нам контейнер
            {
                _activeContainer = typeSelectContainer; // назначим новый активный контейнер
                typeSelectContainer.gameObject.SetActive(true); // Включим его
                CreateSelectButtonsSystemInActiveContainer();
                _scrollRect.content = (RectTransform)typeSelectContainer; // Установим этот контейнер как контент для прокрутки
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
    protected void ClearActiveButtonContainer()
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
    private void ClearAllContainer()
    {
        foreach (Transform container in _typeSelectContainerArray)
        {
            foreach (Transform selectPlacedButton in container)
            {
                Destroy(selectPlacedButton.gameObject);
            }
        }
    }

    /// <summary>
    ///  Создать систему кнопок выбора(Размещаемых объектов) в активном контейнере
    /// </summary>
    protected virtual void CreateSelectButtonsSystemInActiveContainer() { }

    /// <summary>
    /// Создать кнопку выбора размещенного объекта и поместить в контейнер
    /// </summary>
    protected virtual void CreatePlacedObjectSelectButton(PlacedObjectTypeAndCount PlacedObjectResourcesParameters, Transform containerTransform) { }
}


