using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ������� ������ - ������ ������� ���� UNIT
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

    protected void ShowSelectedHeaderText(TextMeshProUGUI selectHeaderText) // �������� ��������� ����� ����������
    {
        foreach (TextMeshProUGUI headerText in _headerTextArray) // ��������� ������ 
        {
            headerText.enabled = (headerText == selectHeaderText);// ���� ��� ���������� ��� ����������� �� ������� ���
        }
    }  

}
