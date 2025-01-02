using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ ����� ��� �����������. ��������� ������� ������(����/������), �������� � ����������
/// </summary>
public class UnitSelectForManagementButtonsSystemUI : UnitSelectButtonsSystemUI
{
    [Header("������ ��� ������������ �������,\n� ����������� ��� ���������� ������")]
    [SerializeField] private Button _myUnitsButtonBar;  // ������ ��� ��������� ������ ��� �����
    [SerializeField] private Image _selectedImageMyUnitsButton; // ����������� ���������� ������ 
    [SerializeField] private Button _hireUnitButtonBar;  // ������ ��� ��������� ������ ����� ������
    [SerializeField] private Image _selectedImageHireUnitButton; // ����������� ���������� ������ 
    [Header("���������� ��� ������������")]
    [SerializeField] private Transform _myUnitsContainer; // ��������� ���� ������
    [SerializeField] private Transform _hireUnitsContainer; // ��������� ������ ��� �����
    [Header("���� ���������� ������� ��� ������������")]
    [SerializeField] private TextMeshProUGUI _myUnitsText; // ����� ���������
    [SerializeField] private TextMeshProUGUI _hireUnitsText; // ����� ���������
    [Header("������ � ���������� ������� ��� ������������\n� ��������� ������. ���������")]
    [SerializeField] private MouseEnterExitEventsUI _healthImage;
    [SerializeField] private MouseEnterExitEventsUI _actionPointsImage;
    [SerializeField] private MouseEnterExitEventsUI _powerImage;
    [SerializeField] private MouseEnterExitEventsUI _accuracyImage;
    [SerializeField] private Image _barrackImage;
    [SerializeField] private Image _missionImage;
    [SerializeField] private Image _priceImage;
    [Header("������ ������ �������")]
    [SerializeField] private Transform _dismissButtonBar; // �������
    [SerializeField] private Button _dismissButton;
    [SerializeField] private Transform _hireButtonBar; // ������
    [SerializeField] private Button _hireButton;
    [Header("������ ��������$")]
    [SerializeField] private Transform _debitedBar;
    [SerializeField] private TextMeshProUGUI _debitedText;

    private Image[] _headerImageArray;
    private Transform[] _transformBarArray;



    protected override void Awake()
    {
        base.Awake();

        _containerArray = new Transform[] { _myUnitsContainer, _hireUnitsContainer };
        _buttonSelectedImageArray = new Image[] { _selectedImageMyUnitsButton, _selectedImageHireUnitButton };
        _headerTextArray = new TextMeshProUGUI[] { _myUnitsText, _hireUnitsText };
        _headerImageArray = new Image[] { _missionImage, _barrackImage, _priceImage };
        _transformBarArray = new Transform[] { _dismissButtonBar, _hireButtonBar, _debitedBar };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _hireUnitButtonBar.onClick.AddListener(() =>//������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowHireUnitsTab();
        });

        _myUnitsButtonBar.onClick.AddListener(() =>
        {
            ShowMuUnitsTab();
        });

        _hireButton.onClick.AddListener(() =>
        {
            HireUnit();
        });

        _dismissButton.onClick.AddListener(() =>
        {
            DismissUnit();
        });


        SetTooltip(_healthImage, "��������");
        SetTooltip(_actionPointsImage, "������ �������");
        SetTooltip(_powerImage, "����");
        SetTooltip(_accuracyImage, "��������");
        SetTooltip(_barrackImage.GetComponent<MouseEnterExitEventsUI>(), "��������� �� ����");
        SetTooltip(_missionImage.GetComponent<MouseEnterExitEventsUI>(), "��������� �� �������");   
        SetTooltip(_priceImage.GetComponent<MouseEnterExitEventsUI>(), "��������� �����");   
    }

    private void SetTooltip(MouseEnterExitEventsUI mouseEnterExitEventsUI, string text)
    {
        mouseEnterExitEventsUI.OnMouseEnter += (object sender, EventArgs e) => // ���������� �� �������
        {
            _tooltipUI.ShowShortTooltipFollowMouse(text, new TooltipUI.TooltipTimer { timer = 0.8f }); // ������� ��������� � ������� ����� ������ ����������� ���������
        };
        mouseEnterExitEventsUI.OnMouseExit += (object sender, EventArgs e) =>
        {
            _tooltipUI.Hide(); // ��� ��������� ���� ������ ���������
        };
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;
        if (active)
        {
            ShowMuUnitsTab();
        }
        else
        {          
            ClearActiveButtonContainer();

        }
    }


    /// <summary>
    /// ������
    /// </summary>
    private void HireUnit()
    {
        if (_unitManager.GetSelectedUnit() == null)
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������ �����", new TooltipUI.TooltipTimer { timer = 0.5f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            return; // ������� �����. ��� ����
        }
       if(!_unitManager.TryHireSelectedUnit())
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������������ �������", new TooltipUI.TooltipTimer { timer = 0.8f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            return; // ������� �����. ��� ����
        }
        ShowAndUpdateContainer(_hireUnitsContainer);
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    }
    /// <summary>
    /// �������
    /// </summary>
    private void DismissUnit()
    {
        if (_unitManager.GetSelectedUnit() == null)
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������ �����", new TooltipUI.TooltipTimer { timer = 0.5f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            return; // ������� �����. ��� ����
        }
        _unitManager.DismissSelectedUnit();
        ShowAndUpdateContainer(_myUnitsContainer);
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    }


    private void ShowMuUnitsTab()
    {
        ShowAndUpdateContainer(_myUnitsContainer);
        ShowSelectedButton(_selectedImageMyUnitsButton);
        ShowSelectedHeaderText(_myUnitsText);
        ShowHeaderImageList(new HashSet<Image> { _barrackImage, _missionImage });
        ShowTransformBarList(new HashSet<Transform> { _dismissButtonBar });                      
       
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        _unitManager.ClearSelectedUnit();// ������� ����������� ����� ����� �������� 3D ������ � ���������
    }

    private void ShowHireUnitsTab()
    {
        ShowAndUpdateContainer(_hireUnitsContainer);
        ShowSelectedButton(_selectedImageHireUnitButton);
        ShowSelectedHeaderText(_hireUnitsText);
        ShowHeaderImageList(new HashSet<Image> { _priceImage });
        ShowTransformBarList(new HashSet<Transform> { _hireButtonBar,_debitedBar });

        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    } 
       

    private void UnitManager_OnSelectedUnitChanged(object sender, Unit selectedUnit)
    {
        UpdatePriceText(selectedUnit);
    }
    private void UpdatePriceText(Unit unit)
    {
        if (unit == null)
        {
            _debitedText.text = "";
        }
        else
        {
            uint price = unit.GetUnitTypeSO<UnitFriendSO>().GetPriceHiring();
            _debitedText.text = $"-({price.ToString("N0")})";
        }
    }

    protected override void CreateSelectButtonsSystemInActiveContainer()
    {
        if (_activeContainer == _myUnitsContainer)
            CreateUnitSelectButtonsSystem(_unitManager.GetUnitFriendList(), _myUnitsContainer);//����� ������  ���� ������ 

        if (_activeContainer == _hireUnitsContainer)
            CreateUnitSelectButtonsSystem(_unitManager.GetHireUnitList(), _hireUnitsContainer);// ������  ������ ��� �����    
    }

    /// <summary>
    /// ������� ������ ������ ������, �� ����������� ������ � ���������� ����������
    /// </summary>
    private void CreateUnitSelectButtonsSystem(List<Unit> unitList, Transform containerTransform)
    {
        for (int index = 0; index < unitList.Count; index++)
        {
            UnitSelectAtManagementButtonUI unitSelectAtManagementButton = Instantiate(GameAssetsSO.Instance.unitSelectAtManagementButton, containerTransform); // �������� ������ � ������� �������� � ���������
            unitSelectAtManagementButton.Init(unitList[index], _unitManager, index + 1);            
        }
    }

    /// <summary>
    /// �������� ���������� ������ ������������ ���������
    /// </summary>
    private void ShowHeaderImageList(HashSet<Image> headerImageList) 
    {
        foreach (Image headerImage in _headerImageArray) // ��������� ������ 
        {
            headerImage.enabled = (headerImageList.Contains(headerImage));// ���� ����������� ���� � ���������� ������ �� ������� ���
        }
    }
    
    /// <summary>
    /// �������� ���������� ������ �����������
    /// </summary>
    private void ShowTransformBarList(HashSet<Transform> transformBar)
    {
        foreach (Transform transform in _transformBarArray)
        {
            transform.gameObject.SetActive(transformBar.Contains(transform));// ���� transform ���� � ���������� ������ �� ������� ���
        }
    }

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
