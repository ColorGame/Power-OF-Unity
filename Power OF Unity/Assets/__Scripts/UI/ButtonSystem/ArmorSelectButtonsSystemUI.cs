using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ ����� ����(PlacedObject)
/// </summary>
public class ArmorSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("���������� ��� ������������")]
    [SerializeField] private Transform _armorHeadSelectContainer; // ��������� ��� ������ ����� 
    [SerializeField] private Transform _armorBodySelectContainer; // ��������� ��� ������ �����������   
    [Header("������ ������������")]
    [SerializeField] private Button _armorHeadButtonPanel;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _armorHeadButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _armorBodyButtonPanel;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _armorBodyButtonSelectedImage; // ����������� ���������� ������ ��������



    protected override void Awake()
    {
        base.Awake();

        _typeSelectContainerArray = new Transform[] { _armorHeadSelectContainer, _armorBodySelectContainer };
        _buttonSelectedImageArray = new Image[] { _armorHeadButtonSelectedImage, _armorBodyButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _armorHeadButtonPanel.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowContainer(_armorHeadSelectContainer);
            ShowSelectedButton(_armorHeadButtonSelectedImage);
        }); // _armorHeadButtonPanel.onClick.AddListener(delegate { ShowContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _armorBodyButtonPanel.onClick.AddListener(() =>
        {
            ShowContainer(_armorBodySelectContainer);
            ShowSelectedButton(_armorBodyButtonSelectedImage);
        });
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;

        if (active)
        {
            ShowContainer(_armorHeadSelectContainer);
            ShowSelectedButton(_armorHeadButtonSelectedImage);
        }
        else
        {
            ClearActiveButtonContainer();
        }
    }

    protected override void CreateSelectButtonsSystemInActiveContainer()
    {
        // ��������� ������ 
        foreach (PlacedObjectTypeAndCount placedObjectTypeAndCount in _warehouseManager.GetPlacedObjectWithActionList())
        {
            switch (placedObjectTypeAndCount.placedObjectTypeSO)
            {
                case ArmorHeadTypeSO:               
                    if (_activeContainer == _armorHeadSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeAndCount, _armorHeadSelectContainer);
                    break;
               
                case ArmorBodyTypeSO:
                    if (_activeContainer == _armorBodySelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeAndCount, _armorBodySelectContainer);
                    break;
            }
        }
    }

    protected override void CreatePlacedObjectSelectButton(PlacedObjectTypeAndCount placedObjectTypeAndCount, Transform containerTransform)
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeAndCount.placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������ 

        if (_canvas.renderMode == RenderMode.WorldSpace)
        {
            Transform[] childrenArray = visualButton.GetComponentsInChildren<Transform>(); // ������ ��� �������� ������� ������� � ������� ����, ��� �� ��� �� ����������� �� ���������� �����
            foreach (Transform child in childrenArray)
            {
                child.gameObject.layer = 13;
            }
        }

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeAndCount);
    }
}
