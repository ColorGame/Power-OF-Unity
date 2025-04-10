using System;
using Unity.Cinemachine;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;

    public event EventHandler OnCameraRotateStarted; // События На камере Поворот начался или завершен. Будет подписываться LookAtCamera 
    public event EventHandler OnCameraRotateCompleted;
    public event EventHandler<float> OnCameraZoomStarted; // События На камере Масштабирование началось или завершено. Будет подписываться LookAtCamera 
    public event EventHandler OnCameraZoomCompleted;

    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Collider cameraBoundsCollider; //Ссылка на коллайдер который ограничивает виртуальную камеру

    private GameInput _gameInput;
    private OptionsSubMenuUIProvider _optionsMenuUI;

  
    private CinemachineFollow _cinemachineTransposer; // Компонент на виртуальной камере с помощью которого будем реализовывать ZOOM
    private bool _edgeScrolling; // прокрутка по краям

    private Unit _targetUnit;
    private Vector3 _targetRotate; // Целевой поворот
    private Vector3 _targetZoomFollowOffset; // Целевое Увеличение Смещение следования       

    private int _rotateAngle = 45; // Угол поворота камеры
    private int _deltaOffsetZoom = 3; // дельта смещения при увеличении

    private bool _targetMoveStarted = false; //  целевое перемещение началось
    private bool _targetRotateStarted = false; // целевой поворот началось
    private bool _targetZoomStarted = false; // целевое увеличение началось
    private bool _moveStarted = false; // движение камеры началось

    private float _moveSpeed = 10f; // Скорость камеры
    private float _rotationSpeed = 5f;
    private float _zoomSpeed = 5f;

    public void Init(GameInput gameInput, OptionsSubMenuUIProvider optionsSubMenuUIProvider)
    {
        _gameInput = gameInput;
        _optionsMenuUI = optionsSubMenuUIProvider;

        _cinemachineTransposer = _cinemachineCamera.GetComponent<CinemachineFollow>(); // Получим и сохраним компонент CinemachineTransposer из виртуальной камеры, чтобы в дальнейшем изменять ее параметры для ZOOM камеры
        _targetZoomFollowOffset = _cinemachineTransposer.FollowOffset; // Смещение следования

        _gameInput.OnCameraMovementStarted += GameInput_OnCameraMovementStarted;
        _gameInput.OnCameraMovementCanceled += GameInput_OnCameraMovementCanceled;

        _gameInput.OnCameraRotateAction += GameInput_OnCameraRotateAction;
        _gameInput.OnCameraZoomAction += GameInput_OnCameraZoomAction;

        _optionsMenuUI.OnEdgeScrollingChangeProvaider += OptionsMenuUI_OnEdgeScrollingChangeProvaider;

        _edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // Загрузим сохраненый параметр _edgeScrolling и если это 1 то будет истина если не=1 то будет ложь (из PlayerPrefs.GetInt нельзя тегать булевые параметры поэтому используем строку)
    }

    private void OptionsMenuUI_OnEdgeScrollingChangeProvaider(object sender, bool e) { SetEdgeScrolling(e); }

    private void GameInput_OnCameraZoomAction(object sender, float e)
    {
        // Считаем переданную величину от игрового ввода и изменим Zoom на _deltaOffsetZoom
        if (e < 0)
            _targetZoomFollowOffset.y -= _deltaOffsetZoom;
        if (e > 0)
            _targetZoomFollowOffset.y += _deltaOffsetZoom;

        _targetZoomStarted = true;
        OnCameraZoomStarted?.Invoke(this, _targetZoomFollowOffset.y);
    }

    private void GameInput_OnCameraRotateAction(object sender, float e)
    {
        // Считаем переданную величину от игрового ввода и изменим поворот по оси У на _rotateAngle
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
    /// Прокрутка по краям экрана.
    /// </summary>
    private void EdgeScrolling()
    {
        if (!_edgeScrolling) return;// Если прокрутка по краям НЕ активированна то выходим

        Vector2 mousePosition = _gameInput.GetMouseScreenPoint();
        Vector2 inputMoveDirection = Vector2.zero;
        float edgeScrollingSize = 5; // (количество пикселей) Отступ от края экрана где начинается движение камеры
        if (mousePosition.x > Screen.width - edgeScrollingSize) // если мыш больше высоты экран - отступ от края
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

    private void HandleMovement() // Обработка Ручного движение
    {
        if (_moveStarted)
        {
            Vector2 inputMoveDirection = _gameInput.GetCameraMoveVector(); // Направление вводимого движенияи (обнуляем перед каждой трансформащией)        
            MovementInBounds(inputMoveDirection);
        }
    }

    private void MovementInBounds(Vector2 inputMoveDirection)
    {
        //Чтобы Движение учитывало вращение преобразуем вектор inputMoveDirection в moveVector
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x; // Применим локальное смещение. Локальным вектор forward(y) изменим на inputMoveDirection.y, а Локальным вектор right(x) изменим на inputMoveDirection.x
        Vector3 targetPosition = transform.position + moveVector; //расчитаем целевую позицию в которую хотим  переместить наш объект

        //ограничим движение
        targetPosition.x = Mathf.Clamp(targetPosition.x,
            cameraBoundsCollider.bounds.min.x,
            cameraBoundsCollider.bounds.max.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z,
            cameraBoundsCollider.bounds.min.z,
            cameraBoundsCollider.bounds.max.z);

        // Debug.Log( cameraBoundsCollider.bounds.min);         
        transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSpeed * Time.deltaTime); // Переместим в расчитаную позицию
    }

    private void HandleRotation() // Ручной поворот
    {
        if (_targetRotateStarted)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotate), Time.deltaTime * _rotationSpeed);

            if (Quaternion.Dot(transform.rotation, Quaternion.Euler(_targetRotate)) == 1) // Точка возвращает 1, если они указывают в одном и том же направлении, -1, если они указывают в совершенно противоположных направлениях, и ноль, если векторы перпендикулярны.
            {
                transform.rotation = Quaternion.Euler(_targetRotate); // т.к. есть небольшие погрешности
                _targetRotateStarted = false;
                OnCameraRotateCompleted?.Invoke(this, EventArgs.Empty);
                //  Debug.Log("Dot__RotateCompleted ВЫПОЛНЕНО");
            }
        }
    }

    private void HandleZoom() // Ручное масштабирование
    {
        if (_targetZoomStarted)
        {
            _targetZoomFollowOffset.y = Mathf.Clamp(_targetZoomFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);// ограничим значения масштабирования

            _cinemachineTransposer.FollowOffset.y = Mathf.Lerp(_cinemachineTransposer.FollowOffset.y, _targetZoomFollowOffset.y, Time.deltaTime * _zoomSpeed); // Загружаем наши измененые значения, Для плавности используем Lerp

            float tolerance = 0.009f; // Допуск //НУЖНО НАСТРОИТЬ//
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

            float stoppingDistance = 0.2f; // Дистанция остановки //НУЖНО НАСТРОИТЬ//
            if (Vector3.Distance(transform.position, _targetUnit.GetWorldPosition()) < stoppingDistance)  // Если растояние до целевой позиции меньше чем Дистанция остановки // Мы достигли цели        
            {
                _targetMoveStarted = false;
            }
        }
    }

    /// <summary>
    /// Установить булевое значение для - прокрутки по краям
    /// </summary>
    public void SetEdgeScrolling(bool edgeScrolling) { _edgeScrolling = edgeScrolling; }

    public void SetTargetUnit(Unit targetUnit)
    {
        _targetUnit = targetUnit;
        _targetMoveStarted = true;
    }

}
