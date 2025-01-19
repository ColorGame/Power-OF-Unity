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
    [SerializeField] private RectTransform _myUnitsContainer; // ��������� ���� ������
    [SerializeField] private RectTransform _hireUnitsContainer; // ��������� ������ ��� �����
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

    private Image[] _barrackMissionArray;
    private Transform[] _hireButtonDebitedArray;

    private List<UnitSelectAtManagementButtonUI> _activeUnitButtonList = new();
    private List<UnitSelectAtManagementButtonUI> _myUnitButtonList = new();
    private List<UnitSelectAtManagementButtonUI> _hireUnitButtonList = new();

    protected override void Awake()
    {
        base.Awake();

        _containerButtonArray = new Transform[] { _myUnitsContainer, _hireUnitsContainer };
        _buttonSelectedImageArray = new Image[] { _selectedImageMyUnitsButton, _selectedImageHireUnitButton };
        _headerTextArray = new TextMeshProUGUI[] { _myUnitsText, _hireUnitsText };
        _headerImageArray = new Image[] { _missionImage, _barrackImage, _priceImage };
        _transformBarArray = new Transform[] { _dismissButtonBar, _hireButtonBar, _debitedBar };

        _barrackMissionArray = new Image[] { _barrackImage, _missionImage };
        _hireButtonDebitedArray = new Transform[] { _hireButtonBar, _debitedBar };
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

        _hireButton.onClick.AddListener(() => { HireUnit(); });

        _dismissButton.onClick.AddListener(() => { DismissUnit(); });

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
        if (_isActive == active) //���� ���������� ��������� ���� �� �������
            return;

        _isActive = active;

        _canvas.enabled = active;
        if (active)
        {
            ShowMuUnitsTab();
        }
        else
        {
            HideAllContainerArray(); //��� ������ �� ���� ����������� �������� ��� ����
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
            //ClearActiveButtonContainer();
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
        if (!_unitManager.TryHireSelectedUnit())
        {
            _tooltipUI.ShowShortTooltipFollowMouse("������������ �������", new TooltipUI.TooltipTimer { timer = 0.8f }); // ������� ��������� � ������� ����� ������ ����������� ���������
            return; // ������� �����. ��� ����
        }

        SetActiveUnitButtonList(false);// ������������ �������� ��������� � �������� 
        UpdateAllContainer();
        SetActiveUnitButtonList(true);
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
        SetActiveUnitButtonList(false);// ������������ �������� ��������� � �������� 
        UpdateAllContainer();
        SetActiveUnitButtonList(true);
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    }


    private void ShowMuUnitsTab()
    {
        SetActiveUnitButtonList(false);// ������������ ������� �������� ��������� � �������� (���� �� ���)
        _activeUnitButtonList = _myUnitButtonList;//�������� ����� �������� ��������� � ��������
        ShowContainer(_myUnitsContainer);
        SetActiveUnitButtonList(true);

        ShowSelectedButton(_selectedImageMyUnitsButton);
        ShowSelectedHeaderText(_myUnitsText);
        ShowHeaderImage(_barrackMissionArray);
        ShowTransformBar(_dismissButtonBar);

        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
        _unitManager.ClearSelectedUnit();// ������� ����������� ����� ����� �������� 3D ������ � ���������
    }

    private void ShowHireUnitsTab()
    {
        SetActiveUnitButtonList(false);// ������������ ������� �������� ��������� � �������� (���� �� ���)
        _activeUnitButtonList = _hireUnitButtonList;//�������� ����� �������� ��������� � ��������
        ShowContainer(_hireUnitsContainer);
        SetActiveUnitButtonList(true);

        ShowSelectedButton(_selectedImageHireUnitButton);
        ShowSelectedHeaderText(_hireUnitsText);
        ShowHeaderImage(_priceImage);
        ShowTransformBar(_hireButtonDebitedArray);

        _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    }

    /// <summary>
    /// �������� �������� ��������� � ��������
    /// </summary>
    private void SetActiveUnitButtonList(bool active)
    {
        foreach (UnitSelectAtManagementButtonUI button in _activeUnitButtonList)
        {
            button.SetActive(active);
        }
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

    /*  protected override void CreateSelectButtonsSystemInActiveContainer()
      {
          if (_activeContainer == _myUnitsContainer)
              GetCreatedUnitSelectButtonsList(_unitManager.GetUnitFriendList(), _myUnitsContainer);//����� ������  ���� ������ 

          if (_activeContainer == _hireUnitsContainer)
              GetCreatedUnitSelectButtonsList(_unitManager.GetHireUnitList(), _hireUnitsContainer);// ������  ������ ��� �����    
      }*/

    protected override void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer)
    {
        if (buttonContainer == _myUnitsContainer)
            _myUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitFriendList(), _myUnitsContainer);//����� ������  ���� ������ 

        if (buttonContainer == _hireUnitsContainer)
            _hireUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetHireUnitList(), _hireUnitsContainer);// ������  ������ ��� �����    
    }
    protected override void CreateSelectButtonsSystemInAllContainer()
    {
        _myUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetUnitFriendList(), _myUnitsContainer);//����� ������  ���� ������        
        _hireUnitButtonList = GetCreatedUnitSelectButtonsList(_unitManager.GetHireUnitList(), _hireUnitsContainer);// ������  ������ ��� �����    
    }
    /// <summary>
    /// �������� ������ �������� ������ ������ ������, �� ����������� ������<br/>
    /// </summary>
    private List<UnitSelectAtManagementButtonUI> GetCreatedUnitSelectButtonsList(List<Unit> unitList, Transform containerTransform)
    {
        List<UnitSelectAtManagementButtonUI> unitButtonList = new();
        for (int index = 0; index < unitList.Count; index++)
        {
            UnitSelectAtManagementButtonUI unitSelectAtManagementButton = Instantiate(GameAssetsSO.Instance.unitSelectAtManagementButton, containerTransform); // �������� ������ � ������� �������� � ���������
            unitSelectAtManagementButton.Init(unitList[index], _unitManager, index + 1);
            unitButtonList.Add(unitSelectAtManagementButton);
        }
        return unitButtonList;
    }

    /// <summary>
    /// �������� ���������� ������ ������������ ���������
    /// </summary>
    private void ShowHeaderImage(IEnumerable<Image> headerImageEnumerable)
    {
        foreach (Image setHeaderImage in _headerImageArray) // ��������� ������ 
        {
            bool contains = false; // ����������� ��� ������� ��� � ������� ������ / ���� ������ ��� � ������� ������ �� ����������� ����  
            foreach (Image headerImage in headerImageEnumerable)
            {
                if (headerImage == setHeaderImage)
                {
                    contains = true;
                    break;// ���� ���� ���������� ������� �� �����
                }
            }
            setHeaderImage.enabled = contains;// ���� ����������� ���� � ���������� ������ �� ������� ���
        }
    }
    /// <summary>
    /// �������� ���������� ������������ ���������
    /// </summary>
    private void ShowHeaderImage(Image headerImage)
    {
        foreach (Image setHeaderImage in _headerImageArray) // ��������� ������ 
        {
            setHeaderImage.enabled = (setHeaderImage == headerImage);// ���� ��� ���������� ����������� ������ �� ������� ���
        }
    }

    /// <summary>
    /// �������� ���������� ������ �����������
    /// </summary>
    private void ShowTransformBar(IEnumerable<Transform> transformBarEnumerable)
    {
        foreach (Transform setTransform in _transformBarArray)
        {
            bool contains = false; // ����������� ��� ������� ��� � ������� ������ / ���� ������ ��� � ������� ������ �� ����������� ����  
            foreach (Transform transform in transformBarEnumerable)
            {
                if (transform == setTransform)
                {
                    contains = true;
                    break;// ���� ���� ���������� ������� �� �����
                }
            }
            setTransform.gameObject.SetActive(contains);// ���� setTransform ���� � ���������� ������ �� ������� ���
        }
    }
    /// <summary>
    /// �������� ���������� ���������
    /// </summary>
    private void ShowTransformBar(Transform transformBar)
    {
        foreach (Transform setTransform in _transformBarArray)
        {
            setTransform.gameObject.SetActive(setTransform == transformBar);// ���� ��� ���������� ��������� �� ������� ���
        }
    }

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
