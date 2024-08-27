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
    private Camera _cameraEquipmentUI;
    private void Awake()
    {
        _canvasAttachedArray = GetComponentsInChildren<Canvas>();
        _cameraEquipmentUI = GetComponent<Camera>();
    }

    private void Start()
    {
        // ��� ����� ����� ���� ����� ��������� � ���� ������� ������
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_cameraEquipmentUI); //������� � ���� �������� ������ ���� ������ ����������


        _cameraEquipmentUI.cullingMask = LayerMask.GetMask("UI","Equipment","EquipmentHidden"); // �������� ���� ����� ��� �����������

        foreach (Canvas canvas in _canvasAttachedArray)
        {
            canvas.worldCamera = _cameraEquipmentUI; // ��������� ��� ������ ������ ������� ����� ��� ���������
        }      
    }
}
