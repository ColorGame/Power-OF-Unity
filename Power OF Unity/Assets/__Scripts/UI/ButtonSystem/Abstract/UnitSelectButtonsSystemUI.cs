using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Система кнопок - выбора ОБЪЕКТА типа UNIT
/// </summary>
public abstract class UnitSelectButtonsSystemUI : ObjectSelectButtonsSystemUI
{    
    protected UnitManager _unitManager;
    protected TextMeshProUGUI[] _headerTextArray;

    public void Init(UnitManager unitManager,TooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;
        _unitManager = unitManager;
        Setup();
    }

    protected void ShowSelectedHeaderText(TextMeshProUGUI selectHeaderText) // Показать выбранный текст оглавления
    {
        foreach (TextMeshProUGUI headerText in _headerTextArray) // Переберем массив 
        {
            headerText.enabled = (headerText == selectHeaderText);// Если это переданное нам изображение то включим его
        }
    }  

}
