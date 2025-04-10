using System;
using Unity.Cinemachine;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;

    public event EventHandler OnCameraRotateStarted; // ������� �� ������ ������� ������� ��� ��������. ����� ������������� LookAtCamera 
    public event EventHandler OnCameraRotateCompleted;
    public event EventHandler<float> OnCameraZoomStarted; // ������� �� ������ ��������������� �������� ��� ���������. ����� ������������� LookAtCamera 
    public event EventHandler OnCameraZoomCompleted;

    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Collider cameraBoundsCollider; //������ �� ��������� ������� ������������ ����������� ������

    private GameInput _gameInput;
    private OptionsSubMenuUIProvider _optionsMenuUI;

  
    private CinemachineFollow _cinemachineTransposer; // ��������� �� ����������� ������ � ������� �������� ����� ������������� ZOOM
    private bool _edgeScrolling; // ��������� �� �����

    private Unit _targetUnit;
    private Vector3 _targetRotate; // ������� �������
    private Vector3 _targetZoomFollowOffset; // ������� ���������� �������� ����������       

    private int _rotateAngle = 45; // ���� �������� ������
    private int _deltaOffsetZoom = 3; // ������ �������� ��� ����������

    private bool _targetMoveStarted = false; //  ������� ����������� ��������
    private bool _targetRotateStarted = false; // ������� ������� ��������
    private bool _targetZoomStarted = false; // ������� ���������� ��������
    private bool _moveStarted = false; // �������� ������ ��������

    private float _moveSpeed = 10f; // �������� ������
    private float _rotationSpeed = 5f;
    private float _zoomSpeed = 5f;

    public void Init(GameInput gameInput, OptionsSubMenuUIProvider optionsSubMenuUIProvider)
    {
        _gameInput = gameInput;
        _optionsMenuUI = optionsSubMenuUIProvider;

        _cinemachineTransposer = _cinemachineCamera.GetComponent<CinemachineFollow>(); // ������� � �������� ��������� CinemachineTransposer �� ����������� ������, ����� � ���������� �������� �� ��������� ��� ZOOM ������
        _targetZoomFollowOffset = _cinemachineTransposer.FollowOffset; // �������� ����������

        _gameInput.OnCameraMovementStarted += GameInput_OnCameraMovementStarted;
        _gameInput.OnCameraMovementCanceled += GameInput_OnCameraMovementCanceled;

        _gameInput.OnCameraRotateAction += GameInput_OnCameraRotateAction;
        _gameInput.OnCameraZoomAction += GameInput_OnCameraZoomAction;

        _optionsMenuUI.OnEdgeScrollingChangeProvaider += OptionsMenuUI_OnEdgeScrollingChangeProvaider;

        _edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // �������� ���������� �������� _edgeScrolling � ���� ��� 1 �� ����� ������ ���� ��=1 �� ����� ���� (�� PlayerPrefs.GetInt ������ ������ ������� ��������� ������� ���������� ������)
    }

    private void OptionsMenuUI_OnEdgeScrollingChangeProvaider(object sender, bool e) { SetEdgeScrolling(e); }

    private void GameInput_OnCameraZoomAction(object sender, float e)
    {
        // ������� ���������� �������� �� �������� ����� � ������� Zoom �� _deltaOffsetZoom
        if (e < 0)
            _targetZoomFollowOffset.y -= _deltaOffsetZoom;
        if (e > 0)
            _targetZoomFollowOffset.y += _deltaOffsetZoom;

        _targetZoomStarted = true;
        OnCameraZoomStarted?.Invoke(this, _targetZoomFollowOffset.y);
    }

    private void GameInput_OnCameraRotateAction(object sender, float e)
    {
        // ������� ���������� �������� �� �������� ����� � ������� ������� �� ��� � �� _rotateAngle
        if (e < 0)
            _targetRotate.y -= _rotateAngle;
        if (e > 0)
            _targetRotate.y += _rotateAngle;

        _targetRotateStarted = true;
        OnCameraRotateStarted?.Invoke(this, EventArgs.Empty);
    }


    private void GameInput_OnCameraMovementStarted(object sender, System.EventArgs e) { _moveStarted = true; }
    private void GameInput_OnCameraMovementCanceled(object sender, System.EventArgs e) { _moveStarted = false; }


    private void Update()
    {
        EdgeScrolling();

        HandleMovement();
        HandleRotation();
        HandleZoom();

        MoveSelectedUnit();
    }

    /// <summary>
    /// ��������� �� ����� ������.
    /// </summary>
    private void EdgeScrolling()
    {
        if (!_edgeScrolling) return;// ���� ��������� �� ����� �� ������������� �� �������

        Vector2 mousePosition = _gameInput.GetMouseScreenPoint();
        Vector2 inputMoveDirection = Vector2.zero;
        float edgeScrollingSize = 5; // (���������� ��������) ������ �� ���� ������ ��� ���������� �������� ������
        if (mousePosition.x > Screen.width - edgeScrollingSize) // ���� ��� ������ ������ ����� - ������ �� ����
        {
            inputMoveDirection.x = +1f;
        }
        if (mousePosition.x < edgeScrollingSize)
        {
            inputMoveDirection.x = -1f;
        }
        if (mousePosition.y > Screen.height - edgeScrollingSize)
        {
            inputMoveDirection.y = +1f;
        }
        if (mousePosition.y < edgeScrollingSize)
        {
            inputMoveDirection.y = -1f;
        }

        MovementInBounds(inputMoveDirection);
    }

    private void HandleMovement() // ��������� ������� ��������
    {
        if (_moveStarted)
        {
            Vector2 inputMoveDirection = _gameInput.GetCameraMoveVector(); // ����������� ��������� ��������� (�������� ����� ������ ��������������)        
            MovementInBounds(inputMoveDirection);
        }
    }

    private void MovementInBounds(Vector2 inputMoveDirection)
    {
        //����� �������� ��������� �������� ����������� ������ inputMoveDirection � moveVector
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x; // �������� ��������� ��������. ��������� ������ forward(y) ������� �� inputMoveDirection.y, � ��������� ������ right(x) ������� �� inputMoveDirection.x
        Vector3 targetPosition = transform.position + moveVector; //��������� ������� ������� � ������� �����  ����������� ��� ������

        //��������� ��������
        targetPosition.x = Mathf.Clamp(targetPosition.x,
            cameraBoundsCollider.bounds.min.x,
            cameraBoundsCollider.bounds.max.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z,
            cameraBoundsCollider.bounds.min.z,
            cameraBoundsCollider.bounds.max.z);

        // Debug.Log( cameraBoundsCollider.bounds.min);         
        transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSpeed * Time.deltaTime); // ���������� � ���������� �������
    }

    private void HandleRotation() // ������ �������
    {
        if (_targetRotateStarted)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotate), Time.deltaTime * _rotationSpeed);

            if (Quaternion.Dot(transform.rotation, Quaternion.Euler(_targetRotate)) == 1) // ����� ���������� 1, ���� ��� ��������� � ����� � ��� �� �����������, -1, ���� ��� ��������� � ���������� ��������������� ������������, � ����, ���� ������� ���������������.
            {
                transform.rotation = Quaternion.Euler(_targetRotate); // �.�. ���� ��������� �����������
                _targetRotateStarted = false;
                OnCameraRotateCompleted?.Invoke(this, EventArgs.Empty);
                //  Debug.Log("Dot__RotateCompleted ���������");
            }
        }
    }

    private void HandleZoom() // ������ ���������������
    {
        if (_targetZoomStarted)
        {
            _targetZoomFollowOffset.y = Mathf.Clamp(_targetZoomFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);// ��������� �������� ���������������

            _cinemachineTransposer.FollowOffset.y = Mathf.Lerp(_cinemachineTransposer.FollowOffset.y, _targetZoomFollowOffset.y, Time.deltaTime * _zoomSpeed); // ��������� ���� ��������� ��������, ��� ��������� ���������� Lerp

            float tolerance = 0.009f; // ������ //����� ���������//
            if (Mathf.Abs(_targetZoomFollowOffset.y - _cinemachineTransposer.FollowOffset.y) <= tolerance)
            {
                _cinemachineTransposer.FollowOffset.y = _targetZoomFollowOffset.y;
                _targetZoomStarted = false;
                OnCameraZoomCompleted?.Invoke(this, EventArgs.Empty);
                // Debug.Log("DELTA_ZoomCompleted");
            }
        }
    }

    private void MoveSelectedUnit()
    {
        if (_targetMoveStarted)
        {
            transform.position = Vector3.Lerp(transform.position, _targetUnit.GetWorldPosition(), Time.deltaTime * _moveSpeed);

            float stoppingDistance = 0.2f; // ��������� ��������� //����� ���������//
            if (Vector3.Distance(transform.position, _targetUnit.GetWorldPosition()) < stoppingDistance)  // ���� ��������� �� ������� ������� ������ ��� ��������� ��������� // �� �������� ����        
            {
                _targetMoveStarted = false;
            }
        }
    }

    /// <summary>
    /// ���������� ������� �������� ��� - ��������� �� �����
    /// </summary>
    public void SetEdgeScrolling(bool edgeScrolling) { _edgeScrolling = edgeScrolling; }

    public void SetTargetUnit(Unit targetUnit)
    {
        _targetUnit = targetUnit;
        _targetMoveStarted = true;
    }

}
