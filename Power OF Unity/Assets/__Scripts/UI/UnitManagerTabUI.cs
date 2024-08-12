using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI ������� "�������� ������". ��������� ������� ������(����/������), �������� � ����������
/// </summary>
public class UnitManagerTabUI : MonoBehaviour
{
    [SerializeField] private Button _myUnitsButtonPanel;  // ������ ��� ��������� ������ ��� �����
    [SerializeField] private Image _myUnitsButtonSelectedImage; // ����������� ���������� ������ 
    [SerializeField] private Button _hireUnitButtonPanel;  // ������ ��� ��������� ������ ����� ������
    [SerializeField] private Image _hireUnitButtonSelectedImage; // ����������� ���������� ������ 
    [SerializeField] private Transform _myUnitsContainer; // ��������� ���� ������
    [SerializeField] private TextMeshProUGUI _myUnitsText; // ����� ���������
    [SerializeField] private Transform _hireContainer; // ��������� ������ ��� �����
    [SerializeField] private TextMeshProUGUI _hireText; // ����� ���������

    private Transform[] _�ontainerArray; // ������ ����������� 
    private Image[] _buttonSelectedImageArray; // ������ ����������� ��� ��������� ������ ������
    private TextMeshProUGUI[] _headerContainerTextArray; // ������ ������� ��������� �����������
    private UnitManager _unitManager;
    private UnitInventorySystem _unitInventorySystem;
    private ScrollRect _scrollRect; //��������� ��������� ������    
   
    private TooltipUI _tooltipUI;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();       

        _�ontainerArray = new Transform[] { _hireContainer, _myUnitsContainer };
        _buttonSelectedImageArray = new Image[] { _hireUnitButtonSelectedImage, _myUnitsButtonSelectedImage };
        _headerContainerTextArray = new TextMeshProUGUI[] { _hireText, _myUnitsText };

        // ��� ����� ������������ ��� ������
        ShowContainer(_myUnitsContainer);
        ShowSelectedButton(_myUnitsButtonSelectedImage);
        ShowHeaderContainerText(_myUnitsText);
    }

    public void Init(UnitManager unitManager, UnitInventorySystem unitInventorySystem)
    {
        _unitManager = unitManager;
        _unitInventorySystem = unitInventorySystem;

        Setup();
    }

    private void Setup()
    {
        CreateUnitSelectButtonsSystem(); // ������� ������ ����� ����������� ��������

        _hireUnitButtonPanel.onClick.AddListener(() =>//������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_hireContainer);
            ShowSelectedButton(_hireUnitButtonSelectedImage);
            ShowHeaderContainerText(_hireText);
        });

        _myUnitsButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_myUnitsContainer);
            ShowSelectedButton(_myUnitsButtonSelectedImage);
            ShowHeaderContainerText(_myUnitsText);
        });


    }

    private void OnEnable()
    {
        if (_unitManager != null) // ��� ��������� ��������� ������� ������ ������ ����� (������ ��� ��� �� ����������� �.�. _unitManager= null)
        {
            CreateUnitSelectButtonsSystem(); // ������� ������ ����� ����������� ��������
        }
    }

    private void ShowContainer(Transform typeSelectContainer) // �������� ��������� (� �������� �������� ������ ��������� ������)
    {
        foreach (Transform buttonContainer in _�ontainerArray) // ��������� ������ �����������
        {
            if (buttonContainer == typeSelectContainer) // ���� ��� ���������� ��� ���������
            {
                buttonContainer.gameObject.SetActive(true); // ������� ���
                _scrollRect.content = (RectTransform)typeSelectContainer; // ��������� ���� ��������� ��� ������� ��� ���������
            }
            else // � ��������� ������
            {
                buttonContainer.gameObject.SetActive(false); // ��������
            }
        }
    }

    private void ShowSelectedButton(Image typeButtonSelectedImage) // �������� ������������ ������ ������
    {
        foreach (Image buttonSelectedImage in _buttonSelectedImageArray) // ��������� ������ 
        {
            buttonSelectedImage.enabled = (buttonSelectedImage == typeButtonSelectedImage);// ���� ��� ���������� ��� ����������� �� ������� ���
        }
    }

    private void ShowHeaderContainerText(TextMeshProUGUI typeHeaderContainerText) // �������� ����� ��������� ����������
    {
        foreach (TextMeshProUGUI headerContainerText in _headerContainerTextArray) // ��������� ������ 
        {
            headerContainerText.enabled = (headerContainerText == typeHeaderContainerText);// ���� ��� ���������� ��� ������ �� ������� ���
        }
    }

    private void CreateUnitSelectButtonsSystem() // ������� ������� ������ ��� ������ �����
    {
        foreach (Transform �ontainer in _�ontainerArray)
        {
            foreach (Transform unitSelectButton in �ontainer) // ��������� ��� ���������� � ����� ����������
            {
                Destroy(unitSelectButton.gameObject); // ������ ������� ������ ������������� � Transform
            }
        }

        List<Unit> unitFriendOnBarrackList = _unitManager.GetUnitFriendOnBarrackList();// ������  ���� ������ � �������
        List<Unit> unitFriendOnMissionList = _unitManager.GetUnitFriendOnMissionList();// ������  ���� ������ �� ������

        for (int index = 0; index < unitFriendOnBarrackList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendOnBarrackList[index], _myUnitsContainer, index + 1);
        }

        for (int index = 0; index < unitFriendOnMissionList.Count; index++)
        {
            CreateUnitSelectButton(unitFriendOnMissionList[index], _hireContainer, index + 1);
        }

        _scrollRect.verticalScrollbar.value = 1f; // ���������� ��������� ������ � ����.
    }


    private void CreateUnitSelectButton(Unit unit, Transform containerTransform, int index) // ������� ������ ����������� �������� � �������� � ���������
    {
        UnitSelectAtInventoryButton unitSelectAtInventoryButton = Instantiate(GameAssets.Instance.unitSelectAtInventoryButton, containerTransform); // �������� ������ � ������� �������� � ���������
        unitSelectAtInventoryButton.Init(unit, _unitInventorySystem, index);
    }
}
