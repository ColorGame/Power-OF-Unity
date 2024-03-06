using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour // ��������� ����� // ������ ������ �� ���� �������� ������� ����� ������ // ���� �������� �������� ������� ������������ ����� ����� �� ����� �������� ������������ ��������
{
    [SerializeField] private bool dynamicFloorPosition = false; // ������������ ������� ����� (��� �������� ������� ����� ������������ � ������ ���� ����������) // ��� ����� � ���������� ���� ��������� �������
    [SerializeField] private List<Renderer> ignoreRendererList; // ������ Renderer ������� ���� ������������ ��� ��������� � ���������� ������������ �������� // ��� ���������� � �������� ����� �� ����� � �������� ���� ������ ���������� � ���������

    private Renderer[] _rendererArray; // ������ Renderer �������� ��������
    private Canvas _canvas;
    private int floor; // ����    
    private bool _cameraZoomActionStarted; // �������� �������� ���������� ������
    private float _cameraHeight;

    private void Awake()
    {
        _rendererArray = GetComponentsInChildren<Renderer>(true); // ������ ��������� Renderer � ���� �������� �������� ���� ���������� � �������� � ������
        _canvas = GetComponentInChildren<Canvas>(true); // ���� �� ������� ��� �� ������ null, ���� ����� ��������    
        if (TryGetComponent(out MoveAction moveAction)) // ���� �� ������� ���� ���� ��������� �� ���������� �� �������
        {
            moveAction.OnChangedFloorsStarted += MoveAction_OnChangedFloorsStarted;
        }
    }

    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position); // ������� ���� ��� ����� �������(������ �� ������� ����� ������) 

        if (floor == 0 && !dynamicFloorPosition) // ���� ���� �� ������� ���������� ������� � ������� ���������� ������ �������  �  ��������� ����������� �� ���������� (��� �������� ������) ��...
        {
            Destroy(this); // ��������� ���� ������ ��� �� �� ������ ��� �� ������� Update
        }

        CameraFollow.OnCameraZoomStarted += CameraFollow_OnCameraZoomStarted;
        CameraFollow.OnCameraZoomCompleted += CameraFollow_OnCameraZoomCompleted;
    }
       

    private void OnDestroy()
    {
        CameraFollow.OnCameraZoomStarted -= CameraFollow_OnCameraZoomStarted;
        CameraFollow.OnCameraZoomCompleted -= CameraFollow_OnCameraZoomCompleted;
    }
    private void CameraFollow_OnCameraZoomStarted(object sender, float e) { _cameraZoomActionStarted = true; _cameraHeight = e; }
    private void CameraFollow_OnCameraZoomCompleted(object sender, System.EventArgs e) { _cameraZoomActionStarted = false; }

    private void Update()
    {
        if (!_cameraZoomActionStarted) return; // ���� ������ �� ������ Zoom �� ������� �� �������

        float floorHeightOffset = 3f; // �������� ������ ����� // ��� �������� ����������� ������
        bool showObject = _cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset; // ������������ ������ ��� ������� ( ���� ������ ������ ������ ������ ����� * �� ����� ����� + ��������)

        if (showObject || floor == 0) // ���� ����� �������� ������ ��� ���� ������� (��� �� ���� ������ ������ ��������� ������ cameraHeight, ����� �� ������� ����� �� �����������)
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
            if (ignoreRendererList.Contains(renderer)) continue; // ���� ������ � ������ ���������� �� ��������� ���
            renderer.enabled = true;
        }
        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(true);
        }
    }

    private void Hide() // ������
    {
        foreach (Renderer renderer in _rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue; // ���� ������ � ������ ���������� �� ��������� ���
            renderer.enabled = false;
        }
        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(false);
        }
    }

    private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
    {
        floor = e.targetGridPosition.floor; // ������� ���� � ������ �����
    }
}
