using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// ����������� ��������� � ���������������� ���������� (������� ���������� � canvas ������� �� �������� � ��������, 
/// ������� ����������� ����� �������������� ������ ���� - ������� ���������. ��������� ������ ����� ������ Canvas)
/// </summary>
/// <remarks>
/// ������ ���� ���������� � PersistentEntryPoint
///  � background � text ��������� ���� ������ ������� Raycast target - ��� �� ��������� �� �������
/// </remarks>
public class TooltipUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _shortTooltipsText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    [SerializeField] private TextMeshProUGUI _nameText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    [SerializeField] private TextMeshProUGUI _descriptionText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    [SerializeField] private TextMeshProUGUI _detailsText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    [SerializeField] private TextMeshProUGUI _sideEffectsTooltipsText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    private ContentSizeFitter _contentSizeFitter; // ��������� ������� ��������
    private RectTransform _canvasTooltipRectTransform; // ��������� ������ 
    private RectTransform _tooltipRectTransform; // ��������� ����������� ��������� TooltipUI // � ���������� ��������� ����� �� ������ ����� ����, ��� ���������� (0, 0) ��� �� �� ��������� �������� �� ����� 
    //private RectTransform _backgroundRectTransform; // ��������� ������� ����
    private TooltipTimer _tooltipTimer; // ����� ����������� ��������� (����������� �����)
    private bool _followMouse; // ��������� �� �����   
    private float _widthTooltip; // ������ ��������� ��� PlacedObjectTooltip (����� ���������� ������������ ���������, ������� ������� �� �����. ������ ���������)


    private VirtualMouseCustomProvider _virtualMouseCustomProvider;
    private GameInput _gameInput;

    public void Init(GameInput gameInput, VirtualMouseCustomProvider Provider)
    {
        _virtualMouseCustomProvider = Provider;
        _gameInput = gameInput;

        Setup();
    }

    private void Setup()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.sortingOrder = 100; // ��������� ������� ���� ���������� ��� �� ��������� ������������ ����� ���� ��������

        _canvasTooltipRectTransform = (RectTransform)canvas.transform;
        _tooltipRectTransform = GetComponent<RectTransform>();
        _contentSizeFitter = GetComponent<ContentSizeFitter>();

        if (_shortTooltipsText == null)
            _shortTooltipsText = transform.Find("shortTooltipsText").GetComponent<TextMeshProUGUI>();
        if (_nameText == null)
            _nameText = transform.Find("nameText").GetComponent<TextMeshProUGUI>();
        if (_descriptionText == null)
            _descriptionText = transform.Find("descriptionText").GetComponent<TextMeshProUGUI>();
        if (_detailsText == null)
            _detailsText = transform.Find("detailsText").GetComponent<TextMeshProUGUI>();
        if (_sideEffectsTooltipsText == null)
            _sideEffectsTooltipsText = transform.Find("sideEffectsTooltipsText").GetComponent<TextMeshProUGUI>();

        _widthTooltip = _tooltipRectTransform.sizeDelta.x; // �������� ������������ ������
        //_backgroundRectTransform = transform.Find("background").CreateInstanceClass<RectTransform>();

        Hide(); // ������ ���������
    }

    private void OnEnable() // ��� ��������� ��������� 
    {
        _followMouse = false;
    }

    private void Update()
    {
        if (_followMouse)
            HandleFollowMouse();


        if (_tooltipTimer != null) // ���� ������ ����� �� - �������� ������
        {
            _tooltipTimer.timer -= Time.deltaTime;
            if (_tooltipTimer.timer <= 0) //�� ��������� ������� ������ ���������
            {
                _tooltipTimer = null;
                _followMouse=false;
                Hide();
            }
        }

    }

    /// <summary>
    /// ��������� ���������� �� �����
    /// </summary>
    private void HandleFollowMouse()
    {
        // � Canvas - ������ ���� ���� ��������������� Canvas Scale, ������� �� ��������� 1280*720. ��� ��������� �������� Game ����� ���������� ��������� Scale(�������) �� ������
        // ��� �� ��������� ��������� ����� �� ����� ���� ��������� Scale ������
        // Vector2 anchoredPosition = Input.mousePosition / _canvasTooltipRectTransform.localScale.x; //������� ���� ������� �� ������� ������ (����� ������������ ������ � ��������� �.�. Y Z �������� ���������������)

        Vector2 anchoredPosition;
        switch (_gameInput.GetActiveGameDevice())
        {
            default:
            case GameInput.GameDevice.KeyboardMouse:
                anchoredPosition = Input.mousePosition / _canvasTooltipRectTransform.localScale.x; //������� ���� ������� �� ������� ������ (����� ������������ ������ � ��������� �.�. Y Z �������� ���������������)
                break;
            case GameInput.GameDevice.Gamepad:
                anchoredPosition = _virtualMouseCustomProvider.GetVirtualMouseCustom().cursorTransform.anchoredPosition / _canvasTooltipRectTransform.localScale.x;
                break;
        }

        // ����������� ��� �� ��������� ������ ���������� �� ������
        if (anchoredPosition.x + _tooltipRectTransform.rect.width > _canvasTooltipRectTransform.rect.width) // ���� ������ ��������� ������� �� ������ ������� ������ �� ...
        {
            anchoredPosition.x = _canvasTooltipRectTransform.rect.width - _tooltipRectTransform.rect.width; // ����������� �� ������ ������� 
        }
        if (anchoredPosition.y + _tooltipRectTransform.rect.height > _canvasTooltipRectTransform.rect.height)
        {
            anchoredPosition.y = _canvasTooltipRectTransform.rect.height - _tooltipRectTransform.rect.height;
        }

        _tooltipRectTransform.anchoredPosition = anchoredPosition; // ������� �������� ����� "����������� ���������" � �������� ����
    }

    /// <summary>
    /// �������� �������� ����������� ���������, ������� ������� �� �����.
    /// </summary>
    /// <remarks>
    /// TooltipTimer - ����� ����������� ���������
    /// </remarks>
    public void ShowShortTooltipFollowMouse(string shortTooltipsText, TooltipTimer tooltipTimer = null)
    {
        _canvas.enabled = true;
        _tooltipTimer = tooltipTimer;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // ������ ��������� �� ����������� ������������ ��� ������ ������      

        EnableShortTooltipsText(); // �������� ����� �������� ����������� ���������

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // �������������� ���������� ����� � ���� �����

        _followMouse = true; // ��������� ����� ��������� �� �����
        HandleFollowMouse(); // ������� ������������ ���������
    }

    /// <summary>
    /// �������� �����������(�����������) ����������� ��������� � ����������� �������.
    /// </summary>
    /// <remarks>
    /// � �������� �������� ���� ,���� ���� ���������� ����������� ������� �� �������� � ��. 
    /// </remarks>
    public void ShowAnchoredPlacedObjectTooltip(PlacedObjectTooltip placedObjectTooltip, RectTransform slotRectTransform, Camera cameraSlotRender = null)
    {
        _canvas.enabled = true;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // �������� �������������� ����������� ������    
        _tooltipRectTransform.sizeDelta = new Vector2(_widthTooltip, 0); // ��������� ������ ��������� (������ - ����� ������������ �������������) 

        EnablePlacedObjectTooltip();

        _nameText.SetText(placedObjectTooltip.name);
        _nameText.ForceMeshUpdate();// �������������� ���������� ����� � ���� �����

        _descriptionText.SetText(placedObjectTooltip.description);
        _descriptionText.ForceMeshUpdate();

        _detailsText.SetText(placedObjectTooltip.details);
        _detailsText.ForceMeshUpdate();

        _sideEffectsTooltipsText.SetText(placedObjectTooltip.sideEffects);
        _sideEffectsTooltipsText.ForceMeshUpdate();

        _followMouse = false; // �������� ���������� �� �����. �� ����� ����������� �� �����
        _tooltipTimer = null; // ������� ������. �� ����� ����� ������ ��������� � �������� � ���� �� �������� �� � Update ��� ������ ������� ���������
        PositionTooltip(slotRectTransform, cameraSlotRender);
    }

    /// <summary>
    /// �������� �����������(�����������) �������� ����������� ���������
    /// </summary>
    /// <remarks>
    /// � �������� �������� ����, ���� ���� ���������� ����������� ������� �� �������� � ��. 
    /// </remarks>
    public void ShowAnchoredShortTooltip(string shortTooltipsText, RectTransform slotRectTransform, Camera cameraSlotRender = null)
    {
        _canvas.enabled = true;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // �������� �������������� ����������� ������     
        _tooltipRectTransform.sizeDelta = new Vector2(_widthTooltip, 0); // ��������� ������ ��������� (������ - ����� ������������ �������������)

        EnableShortTooltipsText();

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // �������������� ���������� ����� � ���� �����

        _followMouse = false; // �������� ���������� �� �����. �� ����� ����������� �� �����
        _tooltipTimer = null; // ������� ������. �� ����� ����� ������ ��������� � �������� � ���� �� �������� �� � Update ��� ������ ������� ���������

        PositionTooltip(slotRectTransform, cameraSlotRender);
    }
    /// <summary>
    /// �������� ����� �������� ����������� ������
    /// </summary>
    private void EnableShortTooltipsText()
    {
        _shortTooltipsText.enabled = true;

        _nameText.enabled = false;
        _descriptionText.enabled = false;
        _detailsText.enabled = false;
        _sideEffectsTooltipsText.enabled = false;
    }
    /// <summary>
    /// �������� ��������� � ����������� �������
    /// </summary>
    private void EnablePlacedObjectTooltip()
    {
        _shortTooltipsText.enabled = false;

        _nameText.enabled = true;
        _descriptionText.enabled = true;
        _detailsText.enabled = true;
        _sideEffectsTooltipsText.enabled = true;
    }

    /// <summary>
    /// ��������� ������� ��������� ������������ ����������� �����
    /// </summary>
    /// <remarks>
    /// ���� ���� ���������� ����������� ������� �� �������� �� � ��������. 
    /// </remarks>
    private void PositionTooltip(RectTransform slotTransform, Camera cameraSlotRender = null)
    {
        // ��������� ��� ����������� ���������� ����� � ������� ��������������� ���������.
        Canvas.ForceUpdateCanvases(); // ������������� �������� ���������� ���� canvas.

        Vector3[] tooltipCornerArray = new Vector3[4]; //������ - ���� ����������� ���������
        _tooltipRectTransform.GetWorldCorners(tooltipCornerArray); // �������� ���� ������ (���� ��������� ���������) �������� ���� ������������ �������������� � ������� ������������.

        Vector3[] slotCornerArray = new Vector3[4]; //������ - ���� ����� �� ������� ������� ���
        slotTransform.GetWorldCorners(slotCornerArray);

        bool below;
        bool right;
        if (cameraSlotRender == null)
        {
            // ��� ���� ����������� ��������� ������������ ������ ����� �����
            below = slotTransform.position.y > Screen.height / 2; // ����������� ���� -���� ����� ����� ���� �������� ������
            right = slotTransform.position.x < Screen.width / 2; // ����������� ������ -���� ����� ����� ����� �������� ������)
        }
        else
        {
            // ����������� ���������� ������������ ������ ������� �������� ���� ���� - slotTransform
            slotCornerArray = Array.ConvertAll(slotCornerArray, i => cameraSlotRender.WorldToScreenPoint(i));// ����������� � ����������� ������ ���������
            Vector3 slotPositionFromCamera = cameraSlotRender.WorldToScreenPoint(slotTransform.position);

            // ��� ���� ����������� ��������� ������������ ������ ����� �����
            below = slotPositionFromCamera.y > Screen.height / 2; // ����������� ���� -���� ����� ����� ���� �������� ������
            right = slotPositionFromCamera.x < Screen.width / 2; // ����������� ������ -���� ����� ����� ����� �������� ������)
        }

        int slotCornerIndex = GetCornerIndex(below, right); //������� ���� ����� � �������� �������� ���������
        int tooltipCornerIndex = GetCornerIndex(!below, !right); // ������� � ��������� ��������������� ����

        Vector3 offset = slotCornerArray[slotCornerIndex] - tooltipCornerArray[tooltipCornerIndex]; //������� �������� ����� ����� ������ - �� ������� ���� �������� ����� ���������

        Vector2 anchoredPosition = _tooltipRectTransform.anchoredPosition + (Vector2)offset / _canvasTooltipRectTransform.localScale.x;// �������� ��������������� �������� ��������� // _offset ������� �� ������� ������ (����� ������������ ������ � ��������� �.�. Y Z �������� ���������������)

        // ����������� ��� �� ��������� ������ ���������� �� ������. ����� �������������� ������ ������� � ������ �������, �.�. ������ � ����� �� �������� �� ����� ����� GetCornerIndex
        if (anchoredPosition.y + _tooltipRectTransform.rect.height > _canvasTooltipRectTransform.rect.height)// ���� ������ ��������� ������� �� ������� ������� ������ �� ...
        {
            anchoredPosition.y = _canvasTooltipRectTransform.rect.height - _tooltipRectTransform.rect.height;// ����������� �� ������� ������� 
        }
        if (anchoredPosition.y < 0)
        {
            anchoredPosition.y = 0;
        }

        _tooltipRectTransform.anchoredPosition = anchoredPosition; // ������� �������� ����� "����������� ���������"
    }

    private int GetCornerIndex(bool below, bool right) // ������� ������ ���� 
    {
        if (below && !right) return 0;          //������ �����
        else if (!below && !right) return 1;    //������� �����
        else if (!below && right) return 2;     //������� ������
        else return 3;                          //������ ������
    }

    public void Hide() // ������� ���������
    {
        _canvas.enabled = false;
    }


    public class TooltipTimer // �������� �����
    {
        public float timer;
    }

    /*Vector2 textSize = _shortTooltipsText.GetRenderedValues(false); // ������� ������ ������ (false ����� ��������� ��� �������)
       Vector2 padding = new Vector2(15, 15); // ���������� (��� �� ��� ������ �� ������) 
       _backgroundRectTransform.sizeDelta = textSize + padding; // ������� ������ ������� ���� � ����������� �� ����� ������*/
}
