using UnityEngine;
using Cinemachine;
using System;


public class CameraMove : Singleton<CameraMove>
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;

    public static event EventHandler OnCameraRotateZoomStarted; // События На камере Поворот и масштабирование начались или завершены. Будет подписываться LookAtCamera 
    public static event EventHandler OnCameraRotateZoomCompleted;

    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private Collider cameraBoundsCollider; //Ссылка на коллайдер который ограничивает виртуальную камеру

    private CinemachineTransposer _cinemachineTransposer;
    private bool _edgeScrolling; // прокрутка по краям
                                  
    private Unit _targetUnit;
    private Vector3 _targetRotate; // Целевой поворот
    private Vector3 _targetZoomFollowOffset; // Целевое Увеличение Смещение следования       

    private int _rotateAngle = 45; // Угол поворота камеры
    private int _deltaOffsetZoom = 3; // дельта смещения при увеличении

    private bool _targetMoveCompleted = true; //  целевое перемещение завершено
    private bool _targetRotateCompleted = true; // целевой поворот завершен
    private bool _targetZoomCompleted = true; // целевое увеличение завершено
    private bool _moveStarted = false; // движение камеры началось

    private float _moveSpeed = 10f; // Скорость камеры
    private float _rotationSpeed = 5f;
    private float _zoomSpeed = 5f;

    protected override void Awake()
    {
        base.Awake(); //Выполним присваивание в базовом методе

        _edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // Загрузим сохраненый параметр _edgeScrolling и если это 1 то будет истина если не=1 то будет ложь (из PlayerPrefs.GetInt нельзя тегать булевые параметры поэтому используем строку)
    }

    private void Start()
    {
        _cinemachineTransposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>(); // Получим и сохраним компонент CinemachineTransposer из виртуальной камеры, чтобы в дальнейшем изменять ее параметры для ZOOM камеры

        _targetZoomFollowOffset = _cinemachineTransposer.m_FollowOffset; // Смещение следования

        FriendlyUnitButtonUI.OnAnyFriendlyUnitButtonPressed += UpdateSelectedUnit; //подпишемся Нажата любая кнопка дружественного юнита 
        EnemyUnitButtonUI.OnAnyEnemylyUnitButtonPressed += UpdateSelectedUnit; //подпишемся Нажата любая кнопка вражественного юнита

        GameInput.Instance.OnCameraMovementStarted += GameInput_OnCameraMovementStarted;
        GameInput.Instance.OnCameraMovementCanceled += GameInput_OnCameraMovementCanceled;

        GameInput.Instance.OnCameraRotateAction += GameInput_OnCameraRotateAction;
        GameInput.Instance.OnCameraZoomAction += GameInput_OnCameraZoomAction;

    }

    private void GameInput_OnCameraZoomAction(object sender, float e)
    {
        // Считаем переданную величину от игрового ввода и изменим Zoom на _deltaOffsetZoom
        if (e < 0)
            _targetZoomFollowOffset.y -= _deltaOffsetZoom;
        if (e > 0)
            _targetZoomFollowOffset.y += _deltaOffsetZoom;

        _targetZoomCompleted = false;  // У нас новые вводные значит мы Не достигли целевого увеличения
        OnCameraRotateZoomStarted?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnCameraRotateAction(object sender, float e)
    {
        // Считаем переданную величину от игрового ввода и изменим поворот по оси У на _rotateAngle
        if (e < 0)
            _targetRotate.y -= _rotateAngle;
        if (e > 0)
            _targetRotate.y += _rotateAngle;

        _targetRotateCompleted = false; // У нас новые вводные значит мы Не достигли целевого поворота
        OnCameraRotateZoomStarted?.Invoke(this, EventArgs.Empty);
    }


    private void GameInput_OnCameraMovementStarted(object sender, System.EventArgs e) { _moveStarted = true; }
    private void GameInput_OnCameraMovementCanceled(object sender, System.EventArgs e) { _moveStarted = false; }


    private void UpdateSelectedUnit(object sender, Unit targetUnit)
    {
        _targetUnit = targetUnit;
        _targetMoveCompleted = false;
       
    }

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

        Vector2 mousePosition = GameInput.Instance.GetMouseScreenPosition();
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
        if (!_moveStarted) return; // Если не начали перемещать камеру то выходим

        Vector2 inputMoveDirection = GameInput.Instance.GetCameraMoveVector(); // Направление вводимого движенияи (обнуляем перед каждой трансформащией)        

        MovementInBounds(inputMoveDirection);
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
        if (!_targetRotateCompleted)// Если целевой поворот НЕ достигнута то выполняем движенее к ней
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_targetRotate), Time.deltaTime * _rotationSpeed);

            if (Quaternion.Dot(transform.rotation, Quaternion.Euler(_targetRotate)) == 1) // Точка возвращает 1, если они указывают в одном и том же направлении, -1, если они указывают в совершенно противоположных направлениях, и ноль, если векторы перпендикулярны.
            {
                transform.rotation = Quaternion.Euler(_targetRotate); // т.к. есть небольшие погрешности
                _targetRotateCompleted = true;
                OnCameraRotateZoomCompleted?.Invoke(this, EventArgs.Empty);
                //  Debug.Log("Dot__RotateCompleted ВЫПОЛНЕНО");
            }
        }
    }

    private void HandleZoom() // Ручное масштабирование
    {
        if (!_targetZoomCompleted)// Если целевое увеличение НЕ достигнуто то выполняем движенее к ней
        {
            _targetZoomFollowOffset.y = Mathf.Clamp(_targetZoomFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);// ограничим значения масштабирования

            _cinemachineTransposer.m_FollowOffset.y = Mathf.Lerp(_cinemachineTransposer.m_FollowOffset.y, _targetZoomFollowOffset.y, Time.deltaTime * _zoomSpeed); // Загружаем наши измененые значения, Для плавности используем Lerp

            float tolerance = 0.009f; // Допуск //НУЖНО НАСТРОИТЬ//
            if (Mathf.Abs(_targetZoomFollowOffset.y - _cinemachineTransposer.m_FollowOffset.y) <= tolerance)
            {
                _cinemachineTransposer.m_FollowOffset.y = _targetZoomFollowOffset.y;
                _targetZoomCompleted = true;
                OnCameraRotateZoomCompleted?.Invoke(this, EventArgs.Empty);
                // Debug.Log("DELTA_ZoomCompleted");
            }            
        }
    }

    private void MoveSelectedUnit()
    {
        if (!_targetMoveCompleted) // Если цель НЕ достигнута то выполняем движенее к ней
        {
            transform.position = Vector3.Lerp(transform.position, _targetUnit.transform.position, Time.deltaTime * _moveSpeed);

            float stoppingDistance = 0.2f; // Дистанция остановки //НУЖНО НАСТРОИТЬ//
            if (Vector3.Distance(transform.position, _targetUnit.transform.position) < stoppingDistance)  // Если растояние до целевой позиции меньше чем Дистанция остановки // Мы достигли цели        
            {
                _targetMoveCompleted = true;
            }
        }
    }

    public float GetCameraHeight() // Получить высоту камеры смещения
    {
        return _targetZoomFollowOffset.y;
    }

    public void SetEdgeScrolling(bool edgeScrolling) // Установить булевое значение для - прокрутки по краям
    {
        this._edgeScrolling = edgeScrolling;
        PlayerPrefs.SetInt("edgeScrolling", edgeScrolling ? 1 : 0); // Сохраним полученное значение в память (если _edgeScrolling истина то установим 1 если ложь установим 0 )
    }

    public bool GetEdgeScrolling() // Вернуть булевое значение для - прокрутки по краям
    {
        return _edgeScrolling;
    }

}
