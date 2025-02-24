
using UnityEngine;

public class LookAtCamera : MonoBehaviour, ISetupForSpawn // ����� �������� ����� �������� � ������� ������
{
    //[SerializeField] private bool _invert; // ��� 1 ������ �������� (������ ������� ���� ���� �������������)

    private Transform _cameraTransform;
    private bool _updateTransformStarted;
    private CameraFollow _cameraFollow;

    public void SetupForSpawn(Unit unit)
    {
        _cameraFollow = unit.GetCameraFollow();

        _cameraFollow.OnCameraRotateStarted += CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraRotateCompleted += CameraMove_OnCameraRotateZoomCompleted;
        
        _cameraFollow.OnCameraZoomStarted += CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraZoomCompleted += CameraMove_OnCameraRotateZoomCompleted;
    }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // �������� ��������� ������ (������ ������������� ��� �� ����� ������� ��������� ��������)
    }

    private void Start()
    {
        UpdateTransform();       
    }

    private void OnDestroy()
    {
        _cameraFollow.OnCameraRotateStarted -= CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraRotateCompleted -= CameraMove_OnCameraRotateZoomCompleted;
        
        _cameraFollow.OnCameraZoomStarted -= CameraMove_OnCameraRotateZoomStarted;
        _cameraFollow.OnCameraZoomCompleted -= CameraMove_OnCameraRotateZoomCompleted;
    }
    private void CameraMove_OnCameraRotateZoomCompleted(object sender, System.EventArgs e) { _updateTransformStarted = false; }
    private void CameraMove_OnCameraRotateZoomStarted(object sender, float e) { _updateTransformStarted = true; }
    private void CameraMove_OnCameraRotateZoomStarted(object sender, System.EventArgs e) { _updateTransformStarted = true; }

    private void LateUpdate() // ����� ��������� ����� ���� Update ����� ������� ������������� ����� � ����� ����������� UI
    {
        /*// 1 �����//{ // � ������ ������ ������� UI ������ ������� �� ��������� �� ����� � ������� ��� ��������
        if (_invert) // ���� ����� ������� ��������������
        {
            Vector3 directionCamera = (_cameraTransform.gridPosition - transform.gridPosition).normalized; // �������������� ������ ������������ � ������� ������
            transform.LookAt(transform.gridPosition + directionCamera*(-1)); // ��������� � �������� - �������� �������� ����� ������� � ��������������� ������� ������� directionCamera (�� ����� ��� �� �������� �� ������ �� �����)
        }
        else
        {
            transform.LookAt(_cameraTransform);
        }// 1 �����//}*/

        // 2 ������ �����//{  � ������ ������ UI ����� ��������� ������ ������

        if (_updateTransformStarted) // ����� ��������� ��������� ������ ��� �������� ������
            UpdateTransform();
    }

    private void UpdateTransform()
    {
        transform.forward = _cameraTransform.forward; // ����� ������������ rotetion, �� ��� ���������� � ��� ������� ������� ��� Vector3
    }
}
