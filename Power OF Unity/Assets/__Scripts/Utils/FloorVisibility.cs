using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������� ����� // ������ ������ �� ���� �������� ������� ����� ������
/// </summary>
/// <remarks>
/// ���� �������� �������� ������� ������������ ����� ����� �� ����� �������� ������������ ��������
/// </remarks>
public class FloorVisibility : MonoBehaviour
{
    [Header("������������ ��������� �����. \n��� ����� ��������� �������")]
    [SerializeField] private bool _dynamicFloorPosition = false; // ������������ ������� ����� (��� �������� ������� ����� ������������ � ������ ���� ����������)
    [Header("Renderer ������� ���� ������������ \n��� ��������� � ���������� ������������ ��������.")]
    [SerializeField] private List<Renderer> _ignoreRendererList; // ������ Renderer ������� ���� ������������ ��� ��������� � ���������� ������������ �������� // ��� ���������� � �������� ����� �� ����� � �������� ���� ������ ���������� � ���������

    private static LevelGrid _levelGrid;
    private static CameraFollow _cameraFollow;

    private Renderer[] _rendererArray; // ������ Renderer �������� ��������
    private Canvas _canvas;
    private int _floor; // ����    
    private bool _cameraZoomActionStarted = false; // �������� �������� ���������� ������
    private float _cameraHeight;

    private static bool _isInit = false;

    private void Awake()
    {
        _rendererArray = GetComponents<Renderer>();
    }

    public static void Init(LevelGrid levelGrid, CameraFollow cameraFollow)
    {
        _levelGrid = levelGrid;
        _cameraFollow = cameraFollow;

        _isInit = true;
    }

    private void Start()
    {
      //  SetupOnStart();
    }

    private void SetupOnStart()
    {
        _floor = _levelGrid.GetFloor(transform.position); // ������� ���� ��� ����� �������(������� �� ������� ����� ������) 

        if (_floor == 0 && !_dynamicFloorPosition) // ���� ���� �� ������� ���������� ������� � ������� ���������� ������ �������  �  ��������� ����������� �� ���������� (��� �������� ������) ��...
        {
            Destroy(this); // ��������� ���� ������ ��� �� �� ������ ��� �� ������� Update
        }
        _cameraFollow.OnCameraZoomStarted += CameraFollow_OnCameraZoomStarted;
        _cameraFollow.OnCameraZoomCompleted += CameraFollow_OnCameraZoomCompleted;
    }

    private void OnDestroy()
    {
        _cameraFollow.OnCameraZoomStarted -= CameraFollow_OnCameraZoomStarted;
        _cameraFollow.OnCameraZoomCompleted -= CameraFollow_OnCameraZoomCompleted;
    }
    private void CameraFollow_OnCameraZoomStarted(object sender, float e) { _cameraZoomActionStarted = true; _cameraHeight = e; }
    private void CameraFollow_OnCameraZoomCompleted(object sender, System.EventArgs e) { _cameraZoomActionStarted = false; }

    private void Update()
    {
        if (!_isInit) return;
        if (!_cameraZoomActionStarted) return; // ���� ������ �� ������ Zoom �� ������� �� ������� (���������� �������� ���������� ��� ���������� ZOOM)

        float floorHeightOffset = 3f; // �������� ������ ����� // ��� �������� ����������� ������
        bool showObject = _cameraHeight > _levelGrid.GetFloorHeght() * _floor + floorHeightOffset; // ������������ ������ ��� ������� ( ���� ������ ������ ������ ������ ����� * �� ����� ����� + ��������)

        if (showObject || _floor == 0) // ���� ����� �������� ������ ��� ���� ������� (��� �� ���� ������ ������ ��������� ������ cameraHeight, ����� �� ������� ����� �� �����������)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show() // ��������
    {
        foreach (Renderer renderer in _rendererArray) // ��������� ������
        {
            if (_ignoreRendererList.Contains(renderer)) continue; // ���� ������ � ������ ���������� �� ��������� ���
            renderer.enabled = true;
        }
        if (_canvas != null)
        {
            _canvas.enabled = true;
        }
    }

    private void Hide() // ������
    {
        foreach (Renderer renderer in _rendererArray)
        {
            if (_ignoreRendererList.Contains(renderer)) continue; // ���� ������ � ������ ���������� �� ��������� ���
            renderer.enabled = false;
        }
        if (_canvas != null)
        {
            _canvas.enabled = false;
        }
    }

    private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
    {
        _floor = e.targetGridPosition.floor; // ������� ���� � ������ �����
    }

}
