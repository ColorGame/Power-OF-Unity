using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;
/// <summary>
/// UI ������� "�������� ������". ��������� ������� ������(����/������), �������� � ����������
/// </summary>
public class UnitManagerTabUI : MonoBehaviour, IToggleActivity
{
    [Header("������ ��� ������������ �������,\n� ����������� ��� ���������� ������")]
    [SerializeField] private Button _myUnitsButtonPanel;  // ������ ��� ��������� ������ ��� �����
    [SerializeField] private Image _selectedImageMyUnitsButton; // ����������� ���������� ������ 
    [SerializeField] private Button _hireUnitButtonPanel;  // ������ ��� ��������� ������ ����� ������
    [SerializeField] private Image _selectedImageHireUnitButton; // ����������� ���������� ������ 
    [Header("���������� ��� ������������")]
    [SerializeField] private Transform _myUnitsContainer; // ��������� ���� ������
    [SerializeField] private Transform _hireUnitsContainer; // ��������� ������ ��� �����
    [Header("���� ������� ��� ������������")]
    [SerializeField] private TextMeshProUGUI _myUnitsText; // ����� ���������
    [SerializeField] private TextMeshProUGUI _hireUnitsText; // ����� ���������
    [Header("������ � ���������� �������")]
    [SerializeField] private Image _barrackImage;
    [SerializeField] private Image _missionImage;
    [SerializeField] private Image _priceImage;
    [Header("������ ������ �������")]
    [SerializeField] private Transform _dismissButtonBar; // �������
    [SerializeField] private Button _dismissButton;
    [SerializeField] private Transform _hireButtonBar; // ������
    [SerializeField] private Button _hireButton;
    [Header("������ ��������$")]
    [SerializeField] private Transform _expensesBar;
    [SerializeField] private TextMeshProUGUI _expensesText;

    private Canvas _canvas;
    private ScrollRect _scrollRect; //��������� ��������� ������    

    private UnitManager _unitManager;
    private TooltipUI _tooltipUI;



    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _canvas = GetComponentInParent<Canvas>(true);
    }

    public void Init(UnitManager unitManager, TooltipUI tooltipUI)
    {
        _unitManager = unitManager;
        _tooltipUI = tooltipUI;
        Setup();
    }

    private void Setup()
    {        
        _hireUnitButtonPanel.onClick.AddListener(() =>//������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowHireUnitsTab();
        });

        _myUnitsButtonPanel.onClick.AddListener(() =>
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
    }


    public void SetActive(bool active)
    {
        _canvas.enabled = active;

        if (active)
        {       
            ShowMuUnitsTab();
        }
        else
        {           
            _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
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
        _unitManager.HireSelectedUnit();
        UpdateHireUnitSelectButtonsSystem();
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
        UpdateMuUnitSelectButtonsSystem();
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    }


    private void ShowMuUnitsTab()
    {
        SetActiveMuUnitsTab(true);
        SetActiveHireUnitsTab(false);
        _unitManager.ClearSelectedUnit();// ������� ����������� ����� ����� �������� 3D ������ � ���������
    }

    private void ShowHireUnitsTab()
    {
        SetActiveHireUnitsTab(true);
        SetActiveMuUnitsTab(false);
        _unitManager.ClearSelectedUnit(); // ������� ����������� ����� ����� �������� 3D ������ � ���������
    }

    private void SetActiveMuUnitsTab(bool active)
    {
        _selectedImageMyUnitsButton.enabled = active;
        _myUnitsText.enabled = active;
        _barrackImage.enabled = active;
        _missionImage.enabled = active;
        _dismissButtonBar.gameObject.SetActive(active);
        _myUnitsContainer.gameObject.SetActive(active);
        if (active)
        {
            UpdateMuUnitSelectButtonsSystem();
            _scrollRect.content = (RectTransform)_myUnitsContainer; // ��������� ���� ��������� ��� ������� ��� ���������
        }
    }

    private void SetActiveHireUnitsTab(bool active)
    {
        _selectedImageHireUnitButton.enabled = active;
        _hireUnitsText.enabled = active;
        _priceImage.enabled = active;
        _hireButtonBar.gameObject.SetActive(active);
        _expensesBar.gameObject.SetActive(active);
        _hireUnitsContainer.gameObject.SetActive(active);
        if (active)
        {
            UpdateHireUnitSelectButtonsSystem();
            _scrollRect.content = (RectTransform)_hireUnitsContainer; // ��������� ���� ��������� ��� ������� ��� ���������
            _unitManager.OnSelectedUnitChanged += UnitManager_OnSelectedUnitChanged;
        }
        else
        {
            _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
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
            _expensesText.text = "";
        }
        else
        {
            int price = unit.GetUnitTypeSO<UnitFriendSO>().GetPriceHiring();
            _expensesText.text = $"-({price})";
        }       
    }

   
    /// <summary>
    /// �������� ������ ������ ������ ��� �����
    /// </summary>
    private void UpdateHireUnitSelectButtonsSystem()
    {
        foreach (Transform unitSelectButton in _hireUnitsContainer)
        {
            Destroy(unitSelectButton.gameObject);
        }

        List<Unit> unitHireList = _unitManager.GetHireUnitTypeSOList();// ������  ������ ��� �����               
        for (int index = 0; index < unitHireList.Count; index++)
        {
            CreateUnitSelectButton(unitHireList[index], _hireUnitsContainer, index + 1);
        }
        _scrollRect.verticalScrollbar.value = 1f; // ���������� ��������� ������ � ����.
    }
    /// <summary>
    /// �������� ������ ������ ���� ������
    /// </summary>
    private void UpdateMuUnitSelectButtonsSystem()
    {
        foreach (Transform unitSelectButton in _myUnitsContainer) // ��������� ��� ���������� � ����� ����������
        {
            Destroy(unitSelectButton.gameObject); // ������ ������� ������ ������������� � Transform
        }

        List<Unit> unitFriendList = _unitManager.GetUnitFriendList();//����� ������  ���� ������ 
        for (int index = 0; index < unitFriendList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendList[index], _myUnitsContainer, index + 1);
        }
        _scrollRect.verticalScrollbar.value = 1f; // ���������� ��������� ������ � ����.
    }

    private void CreateUnitSelectButton(Unit unit, Transform containerTransform, int index) // ������� ������ ����������� �������� � �������� � ���������
    {
        UnitSelectAtManagementButtonUI unitSelectAtManagementButton = Instantiate(GameAssets.Instance.unitSelectAtManagementButton, containerTransform); // �������� ������ � ������� �������� � ���������
        unitSelectAtManagementButton.Init(unit, _unitManager, index);
    }

    private void OnDestroy()
    {
        _unitManager.OnSelectedUnitChanged -= UnitManager_OnSelectedUnitChanged;
    }
}
