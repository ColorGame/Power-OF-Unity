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
    [SerializeField] private Button _armorHeadButton;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _armorHeadButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _armorBodyButton;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _armorBodyButtonSelectedImage; // ����������� ���������� ������ ��������



    protected override void Awake()
    {
        base.Awake();

        _containerArray = new Transform[] { _armorHeadSelectContainer, _armorBodySelectContainer };
        _buttonSelectedImageArray = new Image[] { _armorHeadButtonSelectedImage, _armorBodyButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _armorHeadButton.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            ShowAndUpdateContainer(_armorHeadSelectContainer);
            ShowSelectedButton(_armorHeadButtonSelectedImage);
        }); // _armorHeadButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _armorBodyButton.onClick.AddListener(() =>
        {
            ShowAndUpdateContainer(_armorBodySelectContainer);
            ShowSelectedButton(_armorBodyButtonSelectedImage);
        });
    }

    public override void SetActive(bool active)
    {
        _canvas.enabled = active;
        if (active)
        {
            ShowAndUpdateContainer(_armorHeadSelectContainer);
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
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case HeadArmorTypeSO:               
                    if (_activeContainer == _armorHeadSelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeSO, _armorHeadSelectContainer);
                    break;
               
                case BodyArmorTypeSO:
                    if (_activeContainer == _armorBodySelectContainer)
                        CreatePlacedObjectSelectButton(placedObjectTypeSO, _armorBodySelectContainer);
                    break;
            }
        }
    }

    /// <summary>
    /// ������� ������ ������ ������������ ������� � ��������� � ���������
    /// </summary>
    private void CreatePlacedObjectSelectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssets.Instance.placedObjectSelectButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������               

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeSO);
    }
}
