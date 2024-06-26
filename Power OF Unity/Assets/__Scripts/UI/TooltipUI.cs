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

    private RectTransform _canvasTooltipRectTransform; // ��������� ������ 
    private RectTransform _tooltipRectTransform; // ��������� ����������� ��������� TooltipUI // � ���������� ��������� ����� �� ������ ����� ����, ��� ���������� (0, 0) ��� �� �� ��������� �������� �� ����� 
    private ContentSizeFitter _contentSizeFitter; // ��������� ������� ��������
    private TextMeshProUGUI _shortTooltipsText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    private TextMeshProUGUI _nameText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    private TextMeshProUGUI _descriptionText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    private TextMeshProUGUI _detailsText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    private TextMeshProUGUI _sideEffectsTooltipsText; // ����� ��������� (��� TextMeshProUGUI ���� ������� � ������� UI)
    //private RectTransform _backgroundRectTransform; // ��������� ������� ����
    private TooltipTimer _tooltipTimer; // ����� ����������� ��������� (����������� �����)
    private bool _followMouse; // ��������� �� �����   
    private float _widthTooltip; // ������ ��������� ��� PlacedObjectTooltip (����� ���������� ������������ ���������, ������� ������� �� �����. ������ ���������)


    private VirtualMouseCustom _virtualMouseCustom;
    private GameInput _gameInput;

    public void Init( GameInput gameInput, VirtualMouseCustom virtualMouseCustom) 
    {
        _virtualMouseCustom = virtualMouseCustom;
        _gameInput = gameInput;
    }

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.sortingOrder = 100; // ��������� ������� ���� ���������� ��� �� ��������� ������������ ����� ���� ��������

        _canvasTooltipRectTransform = (RectTransform)canvas.transform;
        _tooltipRectTransform = GetComponent<RectTransform>();
        _contentSizeFitter = GetComponent<ContentSizeFitter>();
        _shortTooltipsText = transform.Find("shortTooltipsText").GetComponent<TextMeshProUGUI>();
        _nameText = transform.Find("nameText").GetComponent<TextMeshProUGUI>();
        _descriptionText = transform.Find("descriptionText").GetComponent<TextMeshProUGUI>();
        _detailsText = transform.Find("detailsText").GetComponent<TextMeshProUGUI>();
        _sideEffectsTooltipsText = transform.Find("sideEffectsTooltipsText").GetComponent<TextMeshProUGUI>();
        _widthTooltip = _tooltipRectTransform.sizeDelta.x; // �������� ������������ ������
        //_backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();

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
                anchoredPosition = _virtualMouseCustom.cursorTransform.anchoredPosition / _canvasTooltipRectTransform.localScale.x;
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
    /// �������� ����������� ���������, ������� ������� �� �����.
    /// </summary>
    /// <remarks>
    /// TooltipTimer - ����� ����������� ���������
    /// </remarks>
    public void ShowTooltipsFollowMouse(string shortTooltipsText, TooltipTimer tooltipTimer = null)
    {
        gameObject.SetActive(true);
        _tooltipTimer = tooltipTimer;
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // ������ ��������� �� ����������� ������������ ��� ������ ������

        EnableShortTooltipsText(); // �������� ����� �������� ����������� ���������

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // �������������� ���������� ����� � ���� �����

        _followMouse = true; // ��������� ����� ��������� �� �����
        HandleFollowMouse(); // ������� ������������ ���������
    }

    /// <summary>
    /// �������� ����������� ��������� � ����������� �������.
    /// </summary>
    /// <remarks>
    /// � �������� �������� ���� � ������ ������� �������� ���� ����
    /// </remarks>
    public void ShowPlacedObjectTooltip(PlacedObjectTooltip placedObjectTooltip, RectTransform slotRectTransform, Camera cameraSlotRender)
    {
        gameObject.SetActive(true);
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // �������� �������������� ����������� ������
        _tooltipRectTransform.sizeDelta = new Vector2(_widthTooltip, 0); // ��������� ������ ��������� (������ - ����� ������������ �������������) 

        EnablePlacedObjectTooltip();

        _nameText.SetText(placedObjectTooltip.name);
        _nameText.ForceMeshUpdate();

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
    /// �������� ����������� ����������� ���������
    /// </summary>
    /// <remarks>
    /// � �������� �������� ���� � ������ ������� �������� ���� ����, �� ��������� ��� 
    /// </remarks>
    public void ShowAnchoredTooltip(string shortTooltipsText, RectTransform slotRectTransform)
    {
        gameObject.SetActive(true);
        _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // �������� �������������� ����������� ������
        _tooltipRectTransform.sizeDelta = new Vector2(_widthTooltip, 0); // ��������� ������ ��������� (������ - ����� ������������ �������������)

        EnableShortTooltipsText();

        _shortTooltipsText.SetText(shortTooltipsText);
        _shortTooltipsText.ForceMeshUpdate(); // �������������� ���������� ����� � ���� �����

        _followMouse = false; // �������� ���������� �� �����. �� ����� ����������� �� �����
        _tooltipTimer = null; // ������� ������. �� ����� ����� ������ ��������� � �������� � ���� �� �������� �� � Update ��� ������ ������� ���������
               
        PositionTooltip(slotRectTransform);        
    }

    private void EnableShortTooltipsText()
    {
        _shortTooltipsText.gameObject.SetActive(true);

        _nameText.gameObject.SetActive(false);
        _descriptionText.gameObject.SetActive(false);
        _detailsText.gameObject.SetActive(false);
        _sideEffectsTooltipsText.gameObject.SetActive(false);
    }

    private void EnablePlacedObjectTooltip()
    {
        _shortTooltipsText.gameObject.SetActive(false);

        _nameText.gameObject.SetActive(true);
        _descriptionText.gameObject.SetActive(true);
        _detailsText.gameObject.SetActive(true);
        _sideEffectsTooltipsText.gameObject.SetActive(true);
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

        Vector2 anchoredPosition = _tooltipRectTransform.anchoredPosition + (Vector2)offset / _canvasTooltipRectTransform.localScale.x;// �������� ��������������� �������� ��������� // offset ������� �� ������� ������ (����� ������������ ������ � ��������� �.�. Y Z �������� ���������������)

        // ����������� ��� �� ��������� ������ ���������� �� ������. ����� �������������� ������ ������� � ������ �������, �.�. ������ � ����� �� �������� �� ����� ����� GetCornerIndex
        if (anchoredPosition.y + _tooltipRectTransform.rect.height > _canvasTooltipRectTransform.rect.height)// ���� ������ ��������� ������� �� ������� ������� ������ �� ...
        {
            anchoredPosition.y = _canvasTooltipRectTransform.rect.height - _tooltipRectTransform.rect.height;// ����������� �� ������� ������� 
        }
        if (anchoredPosition.y  < 0) 
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
        gameObject.SetActive(false);
    }


    public class TooltipTimer // �������� �����
    {
        public float timer;
    }

    /*Vector2 textSize = _shortTooltipsText.GetRenderedValues(false); // ������� ������ ������ (false ����� ��������� ��� �������)
       Vector2 padding = new Vector2(15, 15); // ���������� (��� �� ��� ������ �� ������) 
       _backgroundRectTransform.sizeDelta = textSize + padding; // ������� ������ ������� ���� � ����������� �� ����� ������*/
}
