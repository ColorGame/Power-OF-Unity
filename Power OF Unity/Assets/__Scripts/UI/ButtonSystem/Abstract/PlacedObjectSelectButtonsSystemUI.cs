using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Система кнопок - выбора ОБЪЕКТА типа PlacedObject
/// </summary>
public abstract class PlacedObjectSelectButtonsSystemUI : ObjectSelectButtonsSystemUI
{
    protected PickUpDropPlacedObject _pickUpDrop;
    protected WarehouseManager _warehouseManager;

    protected List<PlacedObjectSelectButtonUI> _activePlacedObjectButtonList = new();
    public void Init(TooltipUI tooltipUI, PickUpDropPlacedObject pickUpDrop, WarehouseManager warehouseManager)
    {
        _tooltipUI = tooltipUI;
        _pickUpDrop = pickUpDrop;
        _warehouseManager = warehouseManager;
        Setup();
    }

    protected void SetAndShowContainer(RectTransform selectContainer, Image showButtonSelectedImage, List<PlacedObjectSelectButtonUI> placedObjectButtonList)
    {
        SetActiveButtonList(false);
        _activePlacedObjectButtonList = placedObjectButtonList;//Назначим новый активный контейнер с кнопками
        ShowContainer(selectContainer);
        SetActiveButtonList(true);

        ShowSelectedButton(showButtonSelectedImage);
    }


    /// <summary>
    /// Настроим активный контейнер с кнопками
    /// </summary>
    protected void SetActiveButtonList(bool active)
    {
        if (_activePlacedObjectButtonList != null)
            foreach (PlacedObjectSelectButtonUI button in _activePlacedObjectButtonList)
            {
                button.SetActive(active);
            }
    }
   
}
