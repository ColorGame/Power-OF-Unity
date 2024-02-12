using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

// https://www.youtube.com/watch?v=j2XyzSAD4VU
public class VirtualCursorUI : MonoBehaviour // ������� ���� ���������� � ����� VirtualMouseCustom
{
    //[SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _cursorVisual;



    /*private VirtualMouseCustom _virtualMouseCustom;


    private void Awake()
    {
        _virtualMouseCustom = GetComponent<VirtualMouseCustom>();
    }*/
    private void Start()
    {
        // _canvas.sortingOrder = 100; //Canvas � ����� ������� �������� ���������� ������ ����� ������������ ��� Canvas � ����� ������ �������� ����������. 
        //  transform.SetAsLastSibling(); // ����������� ���� ������ � ����� ��������, ����� ������ ��������� ������ ������ ������ ������.
        GameInput.Instance.OnGameDeviceChanged += GameInput_OnGameDeviceChanged;

        ResetMouseToCenter();
        UpdateVisibility();
    }

    private void OnDestroy() // ���� ���������� � OnEnable() �� ����� �� ������� Hide() � �������� ���� ������ �� �� ������������� ���������, � �� �� ������ ������ ��� ��������
    {
        GameInput.Instance.OnGameDeviceChanged -= GameInput_OnGameDeviceChanged;
    }

    /* private void Update()
     {
           transform.localScale = Vector3.one * (1f / _canvas.transform.localScale.x); // ����� ������� �������, ��� ����������� ������������ ������� ����������� ����       
         _cursorVisual.localScale = Vector3.one * (_canvas.transform.localScale.x); // ������������� ������ ����

     }

     private void LateUpdate()
     {
          Vector2 anchoredPosition = _cursorVisual.anchoredPosition;
         *//* float edgeSize = 20; // (���������� ��������) ������ �� ���� ������ 
          newPosition.x = Mathf.Clamp(newPosition.x, edgeSize, Screen.width - edgeSize);
          newPosition.y = Mathf.Clamp(newPosition.y, edgeSize, Screen.height - edgeSize);*//*

         anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0, Screen.width);
         anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0, Screen.height);
         InputState.Change(_virtualMouseCustom.virtualMouse.position, anchoredPosition);
     }*/


    private void GameInput_OnGameDeviceChanged(object sender, System.EventArgs e)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (GameInput.Instance.GetActiveGameDevice() == GameInput.GameDevice.Gamepad)
        {
            ResetMouseToCenter();
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        try
        {
            gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError("VirtualCursorUI" + e.ToString());
        }
    }

    private void ResetMouseToCenter()
    {
        Vector2 centerPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _cursorVisual.anchoredPosition = centerPosition;
    }

}