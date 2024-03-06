using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;


namespace UnityEngine.InputSystem.UI
{
    /// <summary>
    /// <b>ВАЖНО В КНОПКАХ ВЫКЛЮЧИТЬ НАВИГАЦИЮ - NONE.</b>
    /// Компонент, который создает виртуальное устройство <see cref="Mouse"/>  и управляет им с помощью входов в стиле геймпада. Это эффективно
    /// добавляет курсор программной мыши.
    /// </summary>
    /// <remarks>
    /// Этот компонент можно использовать с UI, которые предназначены для ввода с помощью мыши, т.е. должны управляться курсором.
    /// Подключив <see cref="InputAction"/>s этого компонента к вводу с геймпада и направлению <see cref="cursorTransform"/>
    /// к преобразованию пользовательского интерфейса курсора, вы можете использовать этот компонент для управления экранным курсором.
    ///
    /// Обратите внимание, что этот компонент фактически сам не запускает ввод пользовательского интерфейса. Вместо этого он создает виртуальную <see cref="Mouse"/>
    /// устройство, которое затем может быть использовано в другом месте (например, с помощью <see cref="InputSystemUIInputModule"/>) где ожидается ввод с помощью мыши/указателя
    ///
    /// Также обратите внимание, что если платформа добавляет <see cref="Mouse"/> этот компонент на нее не влияет. Более конкретно, 
    /// системный курсор мыши не будет перемещен или иным образом использован этим компонентом.
    ///
    /// Входные данные от компонента видны в том же кадре, что и исходные данные о его действиях, благодаря использованию <see cref="InputState.Change"/>. 
    /// </remarks>
    /// <seealso cref="Gamepad"/>
    /// <seealso cref="Mouse"/>
    [AddComponentMenu("Input/Virtual Mouse")]

    public class VirtualMouseCustom : MonoBehaviour
    {
        /// <summary>
        /// Необязательное преобразование, которое будет обновлено в соответствии с текущим положением мыши.
        /// </summary>
        /// <value>Преобразуйте для обновления с помощью положения мыши.</value>
        /// <remarks>
        /// Это полезно, если у вас есть объект пользовательского интерфейса, который непосредственно представляет курсор мыши. Просто добавьте оба
        /// <c>VirtualMouseCustom</c> компонент и <a href="https://docs.unity3d.com/Manual/script-Image.html">Image</a> 
        /// компонента и подсоедините <a href="https://docs.unity3d.com/ScriptReference/RectTransform.html">RectTransform</a>
        /// компонент для объекта пользовательского интерфейса здесь. Затем объект в целом будет следовать сгенерированному движению курсора мыши.
        /// </remarks>
        public RectTransform cursorTransform
        {
            get => m_CursorTransform;
            set => m_CursorTransform = value;
        }

        /// <summary>
        /// Сколько пикселей в секунду перемещается курсор по одной оси, когда соответствующая ось от
        /// <see cref="stickAction"/> равен 1.
        /// </summary>
        /// <value>Скорость мыши в пикселях в секунду.</value>
        public float cursorSpeed
        {
            get => m_CursorSpeed;
            set => m_CursorSpeed = value;
        }

        /// <summary>
        /// Определяет, какое представление курсора использовать. Если задано значение <see cref="CursorMode.SoftwareCursor"/>
        /// (значение по умолчанию), затем <see cref="cursorGraphic"/> и <see cref="cursorTransform"/> определите программный курсор
        /// который устанавливается в соответствии с положением  <see cref="virtualMouse"/>. Если для этого параметра установлено значение <see
        /// cref="CursorMode.HardwareCursorIfAvailable"/> и присутствует собственное устройство <see cref="Mouse"/> компонент перехватит управление 
        /// этим устройством мыши и отключит его (чтобы оно также не генерировало позицию обновления). 
        /// Затем он будет использовать <see cref="Mouse.WarpCursorPosition"/> чтобы переместить системный курсор мыши в положение,
        /// соответствующее положению <see cref="virtualMouse"/>.  В этом случае, <see cref="cursorGraphic"/>
        /// будет отключено и <see cref="cursorTransform"/> обновляться не будет.
        /// </summary>
        /// <value> Следует ли привести системный курсор мыши (если он присутствует) в соответствие с положением виртуальной мыши.</value>
        /// <remarks>
        /// Обратите внимание, что независимо от того, какой режим используется для курсора, ожидается, что ввод с помощью мыши будет получен из <see cref="virtualMouse"/>.
        ///
        /// Обратите внимание, что если <see cref="CursorMode.HardwareCursorIfAvailable"/> используется, программный курсор по-прежнему используется, если нет встроенного
        /// <see cref="Mouse"/> устройство.
        /// </remarks>
        public CursorMode cursorMode
        {
            get => m_CursorMode;
            set
            {
                if (m_CursorMode == value)
                    return;

                // Если мы выключаем его, убедитесь, что мы снова включили системную мышь.
                if (m_CursorMode == CursorMode.HardwareCursorIfAvailable && m_SystemMouse != null)
                {
                    InputSystem.EnableDevice(m_SystemMouse);
                    m_SystemMouse = null;
                }

                m_CursorMode = value;

                if (m_CursorMode == CursorMode.HardwareCursorIfAvailable)
                    TryEnableHardwareCursor();
                else if (m_CursorGraphic != null)
                    m_CursorGraphic.enabled = true;
            }
        }

        /// <summary>
        /// Графический элемент UI, представляющий курсор мыши.
        /// </summary>
        /// <value>Графический элемент для курсора программной мыши.</value>
        /// <remarks>
        /// Если <see cref="cursorMode"/> установлено значение <see cref="CursorMode.HardwareCursorIfAvailable"/>, это изображение будет
        /// отключено.
        ///
        /// Кроме того, этот компонент пользовательского интерфейса неявно определяет <c>Canvas</c>, который определяет область экрана для курсора.
        /// Холст, на котором находится это изображение, будет найден с помощью <c>GetComponentInParent</c>, а затем <c>Canvas.pixelRect</c>
        /// холста используется в качестве границ диапазона перемещения курсора.
        /// </remarks>
        /// <seealso cref="CursorMode.SoftwareCursor"/>
        public Graphic cursorGraphic
        {
            get => m_CursorGraphic;
            set
            {
                m_CursorGraphic = value;
                //TryFindCanvas();
            }
        }

        /// <summary>
        /// Множитель для значений, полученных от <see cref="scrollWheelAction"/>.
        /// </summary>
        /// <value> Множитель для значений прокрутки.</value>
        public float scrollSpeed
        {
            get => m_ScrollSpeed;
            set => m_ScrollSpeed = value;
        }

        /// <summary>
        /// Устройство виртуальной мыши, на которое компонент подает вводимые данные.
        /// </summary>
        /// <value>Экземпляр виртуальной мыши или <c>null</c>.</value>
        /// <remarks>
        /// Это инициализируется только после того, как компонент был включен в первый раз. Обратите внимание, что
        /// при последующем отключении компонента свойство будет продолжать возвращать устройство мыши
        /// но устройство не будет добавлено в систему, пока компонент не включен.
        /// </remarks>
        public Mouse virtualMouse => m_VirtualMouse;

        /// <summary>
        /// Ввод Vector2 stick, который управляет курсором мыши, т.е. <see cref="Pointer.position"/> на
        /// <see cref="virtualMouse"/> и <a
        /// href="https://docs.unity3d.com/ScriptReference/RectTransform-anchoredPosition.html">anchoredPosition</a>
        /// на <see cref="cursorTransform"/> (если установлено).
        /// </summary>
        /// <value>Ввод с помощью джойстика, который управляет положением курсора.</value>
        /// <remarks>
        /// Обычно это должно быть привязано к элементам управления, таким как <see cref="Gamepad.leftStick"/> и/или
        /// <see cref="Gamepad.rightStick"/>.
        /// </remarks>
        public InputActionProperty stickAction
        {
            get => m_StickAction;
            set => SetAction(ref m_StickAction, value);
        }

        /// <summary>
        /// Опциональный ввод кнопки, определяющий, когда <see cref="Mouse.leftButton"/> нажата на
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Входные данные для <see cref="Mouse.leftButton"/>.</value>
        public InputActionProperty leftButtonAction
        {
            get => m_LeftButtonAction;
            set
            {
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_LeftButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetAction(ref m_LeftButtonAction, value);
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_LeftButtonAction, m_ButtonActionTriggeredDelegate, true);
            }
        }

        /// <summary>
        /// Опциональный ввод кнопки, определяющий, когда <see cref="Mouse.rightButton"/> нажата на
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Входные данные для <see cref="Mouse.rightButton"/>.</value>
        public InputActionProperty rightButtonAction
        {
            get => m_RightButtonAction;
            set
            {
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_RightButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetAction(ref m_RightButtonAction, value);
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_RightButtonAction, m_ButtonActionTriggeredDelegate, true);
            }
        }

        /// <summary>
        /// Опциональный ввод кнопки, определяющий, когда <see cref="Mouse.middleButton"/> нажата на
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Входные данные для <see cref="Mouse.middleButton"/>.</value>
        public InputActionProperty middleButtonAction
        {
            get => m_MiddleButtonAction;
            set
            {
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_MiddleButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetAction(ref m_MiddleButtonAction, value);
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_MiddleButtonAction, m_ButtonActionTriggeredDelegate, true);
            }
        }

        /// <summary>
        /// Опциональный ввод кнопки, определяющий, когда <see cref="Mouse.forwardButton"/> нажата на
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Входные данные для <see cref="Mouse.forwardButton"/>.</value>
        public InputActionProperty forwardButtonAction
        {
            get => m_ForwardButtonAction;
            set
            {
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_ForwardButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetAction(ref m_ForwardButtonAction, value);
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_ForwardButtonAction, m_ButtonActionTriggeredDelegate, true);
            }
        }

        /// <summary>
        /// Опциональный ввод кнопки, определяющий, когда <see cref="Mouse.forwardButton"/> нажата на
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>Входные данные для <see cref="Mouse.forwardButton"/>.</value>
        public InputActionProperty backButtonAction
        {
            get => m_BackButtonAction;
            set
            {
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_BackButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetAction(ref m_BackButtonAction, value);
                if (m_ButtonActionTriggeredDelegate != null)
                    SetActionCallback(m_BackButtonAction, m_ButtonActionTriggeredDelegate, true);
            }
        }

        /// <summary>
        /// Опциональный ввод значения Vector2, определяющего значение <see cref="Mouse.scroll"/> на
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value> Ввод для  <see cref="Mouse.scroll"/>.</value>
        /// <remarks>
        /// В случае, если вы хотите привязать только вертикальную прокрутку, просто используйте <see cref="Composites.Vector2Composite"/>
        /// с привязкой только <c>Up</c> и <c>Down</c>, а <c>Left</c> и <c>Right</c>  удалены или ни к чему не привязаны.
        /// </remarks>
        public InputActionProperty scrollWheelAction
        {
            get => m_ScrollWheelAction;
            set => SetAction(ref m_ScrollWheelAction, value);
        }
        
        public void Initialize(GameInput gameInput)
        {
            _gameInput = gameInput;
        }


        private void OnEnable()
        {   
            // Перехватывать системную мышь, если она включена.
            if (m_CursorMode == CursorMode.HardwareCursorIfAvailable)
                TryEnableHardwareCursor();

            // Добавьте устройство мыши.
            if (m_VirtualMouse == null)
                m_VirtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            else if (!m_VirtualMouse.added)
                InputSystem.AddDevice(m_VirtualMouse);

            // Установите начальное положение курсора.
            if (m_CursorTransform != null)
            {
                var position = m_CursorTransform.anchoredPosition;
                InputState.Change(m_VirtualMouse.position, position);
                m_SystemMouse?.WarpCursorPosition(position);
            }

            // Подключитесь к обновлению входных данных.
            if (m_AfterInputUpdateDelegate == null)
                m_AfterInputUpdateDelegate = OnAfterInputUpdate;
            InputSystem.onAfterUpdate += m_AfterInputUpdateDelegate;

            // Подключайтесь к действиям.
            if (m_ButtonActionTriggeredDelegate == null)
                m_ButtonActionTriggeredDelegate = OnButtonActionTriggered;
            SetActionCallback(m_LeftButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_RightButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_MiddleButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_ForwardButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_BackButtonAction, m_ButtonActionTriggeredDelegate, true);

            // Включите действия.
            m_StickAction.action?.Enable();
            m_LeftButtonAction.action?.Enable();
            m_RightButtonAction.action?.Enable();
            m_MiddleButtonAction.action?.Enable();
            m_ForwardButtonAction.action?.Enable();
            m_BackButtonAction.action?.Enable();
            m_ScrollWheelAction.action?.Enable();

            m_ScrollWheelAction.action.started += ScrollWheelAction_started;
            m_ScrollWheelAction.action.performed += ScrollWheelAction_performed; // Клавижа была нажата в течении 1 сек.
            m_ScrollWheelAction.action.canceled += ScrollWheelAction_canceled;

            m_Canvas.sortingOrder = 100; //Canvas с более высоким порядком сортировки всегда будет отображаться над Canvas с более низким порядком сортировки. 
            
            if(_gameInput!= null)
                _gameInput.OnGameDeviceChanged += GameInput_OnGameDeviceChanged;            

            ResetMouseToCenter();
            UpdateVisibility();
        }       

        protected void OnDisable()
        {
            // Удалите устройство мыши.
            if (m_VirtualMouse != null && m_VirtualMouse.added)
                InputSystem.RemoveDevice(m_VirtualMouse);

            // Отпустите системную мышь.
            if (m_SystemMouse != null)
            {
                InputSystem.EnableDevice(m_SystemMouse);
                m_SystemMouse = null;
            }

            // Удаляем себя из input update.
            if (m_AfterInputUpdateDelegate != null)
                InputSystem.onAfterUpdate -= m_AfterInputUpdateDelegate;

            // Отключите действия.
            m_StickAction.action?.Disable();
            m_LeftButtonAction.action?.Disable();
            m_RightButtonAction.action?.Disable();
            m_MiddleButtonAction.action?.Disable();
            m_ForwardButtonAction.action?.Disable();
            m_BackButtonAction.action?.Disable();
            m_ScrollWheelAction.action?.Disable();

            m_ScrollWheelAction.action.started -= ScrollWheelAction_started;
            m_ScrollWheelAction.action.performed -= ScrollWheelAction_performed;
            m_ScrollWheelAction.action.canceled -= ScrollWheelAction_canceled;

            // Отцепитесь от действий.
            if (m_ButtonActionTriggeredDelegate != null)
            {
                SetActionCallback(m_LeftButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetActionCallback(m_RightButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetActionCallback(m_MiddleButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetActionCallback(m_ForwardButtonAction, m_ButtonActionTriggeredDelegate, false);
                SetActionCallback(m_BackButtonAction, m_ButtonActionTriggeredDelegate, false);
            }

            m_LastTime = default;
            m_LastStickValue = default;           
        }
                       
        private void OnDestroy()
        {
            // Если отписаться в OnEnable() то когда мы вызовим Hide(){gameObject.SetActive(false);} и отключим этот объект то он автоматически отпишется, и мы не сможем больше его включить, 
            // а так подписка срабатывает даже если gameObject.SetActive(false) и при смене девайса мы сможем включить этот объект  
            _gameInput.OnGameDeviceChanged -= GameInput_OnGameDeviceChanged;
        }

        private void ScrollWheelAction_canceled(InputAction.CallbackContext obj) { _pressedForWhile = false; }
        private void ScrollWheelAction_performed(InputAction.CallbackContext obj) { _pressedForWhile = true; }
        private void ScrollWheelAction_started(InputAction.CallbackContext obj)
        {
            // Обновите колесо прокрутки.
            var scrollAction = m_ScrollWheelAction.action;
            if (scrollAction != null)
            {
                var scrollValue = scrollAction.ReadValue<Vector2>();
                scrollValue.x *= m_ScrollSpeed;
                scrollValue.y *= m_ScrollSpeed;


                InputState.Change(m_VirtualMouse.scroll, scrollValue);
            }
        }

        /// <summary>
        /// Попробуйте включить аппаратный курсор, и синхронизировать позицию с виртуальной мышью
        /// </summary>
        private void TryEnableHardwareCursor()
        {
            var devices = InputSystem.devices;
            for (var i = 0; i < devices.Count; ++i)
            {
                var device = devices[i];
                if (device.native && device is Mouse mouse)
                {
                    m_SystemMouse = mouse;
                    break;
                }
            }

            // Если нет системной мыши то включим крсор програмнной мыши и выйдем и метода
            if (m_SystemMouse == null)
            {
                if (m_CursorGraphic != null)
                    m_CursorGraphic.enabled = true;
                return;
            }

            // Отключим системную мыш что бы не генерировала события
            InputSystem.DisableDevice(m_SystemMouse);

            // Синхронизирую позицию
            if (m_VirtualMouse != null)
                m_SystemMouse.WarpCursorPosition(m_VirtualMouse.position.value);

            // Выключите изображение курсора мыши.
            if (m_CursorGraphic != null)
                m_CursorGraphic.enabled = false;
        }

        private void UpdateMotion()
        {
            if (m_VirtualMouse == null)
                return;

            transform.localScale = Vector3.one * (1f / m_Canvas.transform.localScale.x); // Учтем масштаб кэнваса, для правильного передвижения курсора виртуальной мыши       
            m_CursorTransform.localScale = Vector3.one * (m_Canvas.transform.localScale.x); // отмаштабируем визуал мыши

            // Считайте текущую значение стика.
            var stickAction = m_StickAction.action;
            if (stickAction == null)
                return;

            var stickValue = stickAction.ReadValue<Vector2>();
            if (Mathf.Approximately(0, stickValue.x) && Mathf.Approximately(0, stickValue.y)) // Сравнивает два значения с плавающей запятой и возвращает true, если они похожи.
            {
                // Движение прекратилось. обнулим предыдущее время и предыдущие значения стика
                m_LastTime = default;
                m_LastStickValue = default;
            }
            else
            {
                var currentTime = InputState.currentTime;
                if (Mathf.Approximately(0, m_LastStickValue.x) && Mathf.Approximately(0, m_LastStickValue.y)) // если предыдущие значение стика было нулевым то мы только начили ввод
                {
                    // Движение началось.
                    m_LastTime = currentTime;
                }

                // Вычислите дельту.
                var deltaTime = (float)(currentTime - m_LastTime);
                var delta = new Vector2(m_CursorSpeed * stickValue.x * deltaTime, m_CursorSpeed * stickValue.y * deltaTime);

                // Обновите позицию.
                var currentPosition = m_VirtualMouse.position.value;
                var newPosition = currentPosition + delta;

                ////ОБЗОР: что касается программного курсора, закрепите его на чем-нибудь другом?
                //Ограничим рамками экрана. можно настроить отступ
                // Vector2 anchoredPosition = _cursorVisual.anchoredPosition;
                /*float edgeSize = 20; // (количество пикселей) Отступ от края экрана 
                newPosition.x = Mathf.Clamp(newPosition.x, edgeSize, Screen.width - edgeSize);
                newPosition.y = Mathf.Clamp(newPosition.y, edgeSize, Screen.height - edgeSize);*/

                newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
                newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);


                ////ОБЗОР: тот факт, что у нас нет событий для них, означает, что действия не будут иметь идентификатора события для прохождения; проблема?
                InputState.Change(m_VirtualMouse.position, newPosition);
                InputState.Change(m_VirtualMouse.delta, delta);

                // Обновите SoftwareCursor преобразования курсора, если таковое имеется.
                if (m_CursorTransform != null &&
                    (m_CursorMode == CursorMode.SoftwareCursor ||
                     (m_CursorMode == CursorMode.HardwareCursorIfAvailable && m_SystemMouse == null)))
                    m_CursorTransform.anchoredPosition = newPosition;

                m_LastStickValue = stickValue;
                m_LastTime = currentTime;

                // Обновите аппаратный курсор.
                m_SystemMouse?.WarpCursorPosition(newPosition);
            }



            // Обновите колесо прокрутки.
            var scrollAction = m_ScrollWheelAction.action;
            if (scrollAction != null && _pressedForWhile)
            {
                var scrollValue = scrollAction.ReadValue<Vector2>();
                scrollValue.x *= m_ScrollSpeed;
                scrollValue.y *= m_ScrollSpeed;

                InputState.Change(m_VirtualMouse.scroll, scrollValue);
            }
        }

        [Tooltip("Холст, задающий диапазон перемещения программного курсора.")]
        [SerializeField] private Canvas m_Canvas; // Холст, задающий диапазон перемещения программного курсора.       

        [Header("Cursor")]
        [Tooltip("Должен ли компонент устанавливать положение курсора аппаратного курсора мыши, если таковой доступен. Если да, то "
            + "программный курсор, на который указывает \"Cursor Graphic\", будет скрыт.")]
        [SerializeField] private CursorMode m_CursorMode;
        [Tooltip("Закинуть \"CursorVisual\". Графическое изображение, представляющее программный курсор. Оно скрыто, если используется аппаратный курсор (см. \"Cursor Mode\").")]
        [SerializeField] private Graphic m_CursorGraphic;
        [Tooltip(" Закинуть \"CursorVisual\". Преобразование для программного курсора. Будет установлено только в том случае, если используется программный курсор (см. \"Cursor Mode\"). Перемещение курсора"
            + " обновляет закрепленное положение преобразования.")]
        [SerializeField] private RectTransform m_CursorTransform;

        [Header("Motion")]
        [Tooltip("Скорость перемещения курсора в пикселях в секунду. Масштабируется по вводу из \"Stick Action\".")]
        [SerializeField] private float m_CursorSpeed = 400;
        [Tooltip("Масштабный коэффициент, применяемый к \"Scroll Wheel Action\" при настройке элемента управления \"'scroll Wheel\" мыши.")]
        [SerializeField] private float m_ScrollSpeed = 45;

        [Space(10)]
        [Tooltip("Действие Vector2, которое перемещает курсор влево/вправо (X) и вверх/вниз (Y) по экрану.")]
        [SerializeField] private InputActionProperty m_StickAction;
        [Tooltip("Действие кнопки, вызывающее щелчок левой кнопкой мыши.")]
        [SerializeField] private InputActionProperty m_LeftButtonAction;
        [Tooltip("Действие кнопки, вызывающее средний щелчок мыши.")]
        [SerializeField] private InputActionProperty m_MiddleButtonAction;
        [Tooltip("Действие кнопки, вызывающее щелчок правой кнопкой мыши.")]
        [SerializeField] private InputActionProperty m_RightButtonAction;
        [Tooltip("Действие кнопки, запускающее нажатие кнопки перемотки вперед (кнопка №4) щелчком мыши.")]
        [SerializeField] private InputActionProperty m_ForwardButtonAction;
        [Tooltip("Действие кнопки, запускающее кнопку возврата (кнопка №5) щелчком мыши.")]
        [SerializeField] private InputActionProperty m_BackButtonAction;
        [Tooltip("Действие Vector2, которое преобразуется в действие \"scrollWheel\" мыши (масштабируется по \"Scroll Speed\"). В вкладке \"Interactions\" добавить \"Hold\" и выставить \"Hold Time = 1сек\". Если клавиша зажата в течении 1 сек, тогда начинаем считывать в \"Update\" ее значение")]
        [SerializeField] private InputActionProperty m_ScrollWheelAction;

        private Mouse m_VirtualMouse;
        private Mouse m_SystemMouse;
        private Action m_AfterInputUpdateDelegate;
        private Action<InputAction.CallbackContext> m_ButtonActionTriggeredDelegate;
        private double m_LastTime;
        private Vector2 m_LastStickValue;
        private GameInput _gameInput;

        /// <summary>
        /// Нажата в течение некоторого времени
        /// </summary>
        private bool _pressedForWhile;

        private void OnButtonActionTriggered(InputAction.CallbackContext context)
        {
            if (m_VirtualMouse == null)
                return;

            // Элементы управления кнопками являются битовыми элементами управления. Мы не можем (пока?) использовать состояние ввода.Изменить на состояние
            // изменение этих элементов управления, поскольку механизм обновления состояния InputManager поддерживает только
            // обновления области байтов. Итак, мы просто получаем полное состояние нашей виртуальной мыши, затем обновляем
            // кнопку там, а затем просто перезаписываем все состояние.

            var action = context.action;
            MouseButton? button = null;
            if (action == m_LeftButtonAction.action)
                button = MouseButton.Left;
            else if (action == m_RightButtonAction.action)
                button = MouseButton.Right;
            else if (action == m_MiddleButtonAction.action)
                button = MouseButton.Middle;
            else if (action == m_ForwardButtonAction.action)
                button = MouseButton.Forward;
            else if (action == m_BackButtonAction.action)
                button = MouseButton.Back;

            if (button != null)
            {
                var isPressed = context.control.IsPressed();
                m_VirtualMouse.CopyState<MouseState>(out var mouseState);// Скопируйте состояние ВиртуальнойМыши.
                mouseState.WithButton(button.Value, isPressed); //Меняем состояние кнопки которую нажимаем . Сопоставляем кнопку мыши с кнопкой на геймпаде

                InputState.Change(m_VirtualMouse, mouseState); // Заменим состояние виртуальной мыши на новое.
            }
        }

        private static void SetActionCallback(InputActionProperty field, Action<InputAction.CallbackContext> callback, bool install = true)
        {
            var action = field.action;
            if (action == null)
                return;

            // Нам не нужен выполненный обратный вызов, поскольку наши кнопки мыши являются двоичными, и, следовательно,
            // нас интересуют только запущенные (1) и отмененные (0).

            if (install)
            {
                action.started += callback;
                action.canceled += callback;
            }
            else
            {
                action.started -= callback;
                action.canceled -= callback;
            }
        }

        private static void SetAction(ref InputActionProperty field, InputActionProperty value)
        {
            var oldValue = field;
            field = value;

            if (oldValue.reference == null)
            {
                var oldAction = oldValue.action;
                if (oldAction != null && oldAction.enabled)
                {
                    oldAction.Disable();
                    if (value.reference == null)
                        value.action?.Enable();
                }
            }
        }

        private void OnAfterInputUpdate()
        {
            UpdateMotion();
        }

        /// <summary>
        /// Определяет, как будет представлен курсор виртуальной мыши.
        /// </summary>
        /// <seealso cref="cursorMode"/>
        public enum CursorMode
        {
            /// <summary>
            /// Курсор представлен в виде UI интерфейса. Видеть <see cref="cursorGraphic"/>.
            /// </summary>
            SoftwareCursor,

            /// <summary>
            /// Если присутствует собственное устройство <see cref="Mouse"/>  его курсор будет использоваться и управляться
            /// виртуальной мышью с помощью  <see cref="Mouse.WarpCursorPosition"/>. Ссылка на программный курсор
            /// <see cref="cursorGraphic"/> будет отключена.
            ///
            /// Обратите внимание, что если нет встроенного  <see cref="Mouse"/> поведение вернется к прежнему
            /// <see cref="SoftwareCursor"/>.
            /// </summary>
            HardwareCursorIfAvailable,
        }

        private void GameInput_OnGameDeviceChanged(object sender, System.EventArgs e)
        {
            UpdateVisibility();           
        }

        private void UpdateVisibility()
        {
            if (_gameInput.GetActiveGameDevice() == GameInput.GameDevice.Gamepad)
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
            m_CursorTransform.anchoredPosition = centerPosition;
        }
    }
}

