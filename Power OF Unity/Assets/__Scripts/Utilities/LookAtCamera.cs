
using UnityEngine;

public class LookAtCamera : MonoBehaviour // ����� �������� ����� �������� � ������� ������
{
    //[SerializeField] private bool _invert; // ��� 1 ������ �������� (������ ������� ���� ���� �������������)

    private Transform _cameraTransform;
    private bool _updateTransformStarted;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // �������� ��������� ������ (������ ������������� ��� �� ����� ������� ��������� ��������)
    }

    private void Start()
    {
        UpdateTransform();

        CameraMove.OnCameraRotateZoomStarted += CameraMove_OnCameraRotateZoomStarted;
        CameraMove.OnCameraRotateZoomCompleted += CameraMove_OnCameraRotateZoomCompleted;
    }

    private void OnDisable()
    {
        CameraMove.OnCameraRotateZoomStarted -= CameraMove_OnCameraRotateZoomStarted;
        CameraMove.OnCameraRotateZoomCompleted -= CameraMove_OnCameraRotateZoomCompleted;
    }
    private void CameraMove_OnCameraRotateZoomCompleted(object sender, System.EventArgs e)
    {
        _updateTransformStarted= false;
    }

    private void CameraMove_OnCameraRotateZoomStarted(object sender, System.EventArgs e)
    {
        _updateTransformStarted = true;
    }

    private void LateUpdate() // ����� ��������� ����� ���� Update ����� ������� ������������� ����� � ����� ����������� UI
    {
        /*// 1 �����//{ // � ������ ������ ������� UI ������ ������� �� ��������� �� ����� � ������� ��� ��������
        if (_invert) // ���� ����� ������� ��������������
        {
            Vector3 directionCamera = (_cameraTransform.position - transform.position).normalized; // �������������� ������ ������������ � ������� ������
            transform.LookAt(transform.position + directionCamera*(-1)); // ��������� � �������� - �������� �������� ����� ������� � ��������������� ������� ������� directionCamera (�� ����� ��� �� �������� �� ������ �� �����)
        }
        else
        {
            transform.LookAt(_cameraTransform);
        }// 1 �����//}*/

        // 2 ������ �����//{  � ������ ������ UI ����� ��������� ������ ������
       
        if (_updateTransformStarted) // ���� ��������� ��������� ������ ��� �������� ������
            UpdateTransform();
    }

    private void UpdateTransform()
    {
        transform.forward = _cameraTransform.forward; // ����� ������������ rotetion, �� ��� ���������� � ��� ������� ������� ��� Vector3
    }
}
