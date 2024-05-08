using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/// <summary>
/// ��������� ������ � ������������� �������(Canvas)
/// </summary>
/// <remarks>
/// ������ ������ �� ������
/// </remarks>
public class SetupCameraAndAttachedCanvasesUI : MonoBehaviour
{
    private Canvas[] _canvasAttachedArray;
    private Camera _cameraInventoryUI;
    private void Awake()
    {
        _canvasAttachedArray = GetComponentsInChildren<Canvas>();
        _cameraInventoryUI = GetComponent<Camera>();
    }

    private void Start()
    {
        // ��� ����� ����� ���� ����� ��������� � ���� ������� ������
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_cameraInventoryUI); //������� � ���� �������� ������ ���� ������ ���������


        _cameraInventoryUI.cullingMask = LayerMask.GetMask("UI","Inventory","InventoryHidden"); // �������� ���� ����� ��� �����������

        foreach (Canvas canvas in _canvasAttachedArray)
        {
            canvas.worldCamera = _cameraInventoryUI; // ��������� ��� ������ ������ ������� ����� ��� ���������
        }      
    }
}
