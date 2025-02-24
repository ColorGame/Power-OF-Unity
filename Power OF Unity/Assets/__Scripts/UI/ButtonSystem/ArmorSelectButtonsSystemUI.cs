using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ������� ������ - ������ ����� ����(PlacedObject)
/// </summary>
public class ArmorSelectButtonsSystemUI : PlacedObjectSelectButtonsSystemUI
{
    [Header("���������� ��� ������������")]
    [SerializeField] private RectTransform _headArmorSelectContainer; // ��������� ��� ������ ����� 
    [SerializeField] private RectTransform _bodyArmorSelectContainer; // ��������� ��� ������ �����������   
    [Header("������ ������������")]
    [SerializeField] private Button _headArmorButton;  // ������ ��� ��������� ������ ������
    [SerializeField] private Image _headArmorButtonSelectedImage; // ����������� ���������� ������ ������
    [SerializeField] private Button _bodyArmorButton;  // ������ ��� ��������� ������ ��������
    [SerializeField] private Image _bodyArmorButtonSelectedImage; // ����������� ���������� ������ ��������


    private List<PlacedObjectSelectButtonUI> _headArmorPlacedObjectButtonList = new();
    private List<PlacedObjectSelectButtonUI> _bodyArmorPlacedObjectButtonList = new();

    protected override void Awake()
    {
        base.Awake();

        _containerButtonArray = new Transform[] { _headArmorSelectContainer, _bodyArmorSelectContainer };
        _buttonSelectedImageArray = new Image[] { _headArmorButtonSelectedImage, _bodyArmorButtonSelectedImage };
    }

    protected override void SetDelegateContainerSelectionButton()
    {
        _headArmorButton.onClick.AddListener(() => //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {
            SetAndShowContainer(_headArmorSelectContainer, _headArmorButtonSelectedImage, _headArmorPlacedObjectButtonList);
        }); // _headArmorButton.onClick.AddListener(delegate { ShowAndUpdateContainer(_unitOnMissionContainer); }); // ��� ������� ����������

        _bodyArmorButton.onClick.AddListener(() =>
        {
            SetAndShowContainer(_bodyArmorSelectContainer, _bodyArmorButtonSelectedImage, _bodyArmorPlacedObjectButtonList);
        });
    }

    public override void SetActive(bool active)
    {
        if (_isActive == active) //���� ���������� ��������� ���� �� �������
            return;

        _isActive = active;

        _canvas.enabled = active;
        if (active)
        {
            SetAndShowContainer(_bodyArmorSelectContainer, _bodyArmorButtonSelectedImage, _bodyArmorPlacedObjectButtonList);
        }
        else
        {
            HideAllContainerArray();
        }
    }
  
    protected override void CreateSelectButtonsSystemInContainer(RectTransform buttonContainer)
    {

        if (buttonContainer == _headArmorSelectContainer)
        {
            _headArmorPlacedObjectButtonList.Clear();
            foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
            {
                switch (placedObjectTypeSO)
                {
                    case HeadArmorTypeSO:
                        _headArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _headArmorSelectContainer));
                        break;
                }
            }
        }
        if (buttonContainer == _bodyArmorSelectContainer)
        {
            _bodyArmorPlacedObjectButtonList.Clear();
            foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
            {
                switch (placedObjectTypeSO)
                {
                    case BodyArmorTypeSO:
                        _bodyArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _bodyArmorSelectContainer));
                        break;
                }
            }
        }
    }

    protected override void CreateSelectButtonsSystemInAllContainer()
    {
        _headArmorPlacedObjectButtonList.Clear();
        _bodyArmorPlacedObjectButtonList.Clear();
        foreach (PlacedObjectTypeSO placedObjectTypeSO in _warehouseManager.GetAllPlacedObjectTypeSOList())
        {
            switch (placedObjectTypeSO)
            {
                case HeadArmorTypeSO:
                    _headArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _headArmorSelectContainer));
                    break;
                case BodyArmorTypeSO:
                    _bodyArmorPlacedObjectButtonList.Add(CreatePlacedObjectSelectButton(placedObjectTypeSO, _bodyArmorSelectContainer));
                    break;
            }
        }
    }

    /// <summary>
    /// ������� ������ ������ ������������ ������� � ��������� � ���������
    /// </summary>
    private PlacedObjectSelectButtonUI CreatePlacedObjectSelectButton(PlacedObjectTypeSO placedObjectTypeSO, Transform containerTransform)
    {
        PlacedObjectSelectButtonUI placedObjectSelectButton = Instantiate(GameAssetsSO.Instance.placedObjectSelectButton, containerTransform); // �������� ������ � ������� �������� � ���������
        Transform visualButton = Instantiate(placedObjectTypeSO.GetVisual2D(), placedObjectSelectButton.transform); // �������� ������ ������ � ����������� �� ���� ������������ ������� � ������� �������� � ������               

        placedObjectSelectButton.Init(_tooltipUI, _pickUpDrop, _warehouseManager, placedObjectTypeSO);
        return placedObjectSelectButton;
    }
}
