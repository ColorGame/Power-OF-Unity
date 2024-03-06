using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;


namespace UnityEngine.InputSystem.UI
{
    /// <summary>
    /// <b>����� � ������� ��������� ��������� - NONE.</b>
    /// ���������, ������� ������� ����������� ���������� <see cref="Mouse"/>  � ��������� �� � ������� ������ � ����� ��������. ��� ����������
    /// ��������� ������ ����������� ����.
    /// </summary>
    /// <remarks>
    /// ���� ��������� ����� ������������ � UI, ������� ������������� ��� ����� � ������� ����, �.�. ������ ����������� ��������.
    /// ��������� <see cref="InputAction"/>s ����� ���������� � ����� � �������� � ����������� <see cref="cursorTransform"/>
    /// � �������������� ����������������� ���������� �������, �� ������ ������������ ���� ��������� ��� ���������� �������� ��������.
    ///
    /// �������� ��������, ��� ���� ��������� ���������� ��� �� ��������� ���� ����������������� ����������. ������ ����� �� ������� ����������� <see cref="Mouse"/>
    /// ����������, ������� ����� ����� ���� ������������ � ������ ����� (��������, � ������� <see cref="InputSystemUIInputModule"/>) ��� ��������� ���� � ������� ����/���������
    ///
    /// ����� �������� ��������, ��� ���� ��������� ��������� <see cref="Mouse"/> ���� ��������� �� ��� �� ������. ����� ���������, 
    /// ��������� ������ ���� �� ����� ��������� ��� ���� ������� ����������� ���� �����������.
    ///
    /// ������� ������ �� ���������� ����� � ��� �� �����, ��� � �������� ������ � ��� ���������, ��������� ������������� <see cref="InputState.Change"/>. 
    /// </remarks>
    /// <seealso cref="Gamepad"/>
    /// <seealso cref="Mouse"/>
    [AddComponentMenu("Input/Virtual Mouse")]

    public class VirtualMouseCustom : MonoBehaviour
    {
        /// <summary>
        /// �������������� ��������������, ������� ����� ��������� � ������������ � ������� ���������� ����.
        /// </summary>
        /// <value>������������ ��� ���������� � ������� ��������� ����.</value>
        /// <remarks>
        /// ��� �������, ���� � ��� ���� ������ ����������������� ����������, ������� ��������������� ������������ ������ ����. ������ �������� ���
        /// <c>VirtualMouseCustom</c> ��������� � <a href="https://docs.unity3d.com/Manual/script-Image.html">Image</a> 
        /// ���������� � ������������ <a href="https://docs.unity3d.com/ScriptReference/RectTransform.html">RectTransform</a>
        /// ��������� ��� ������� ����������������� ���������� �����. ����� ������ � ����� ����� ��������� ���������������� �������� ������� ����.
        /// </remarks>
        public RectTransform cursorTransform
        {
            get => m_CursorTransform;
            set => m_CursorTransform = value;
        }

        /// <summary>
        /// ������� �������� � ������� ������������ ������ �� ����� ���, ����� ��������������� ��� ��
        /// <see cref="stickAction"/> ����� 1.
        /// </summary>
        /// <value>�������� ���� � �������� � �������.</value>
        public float cursorSpeed
        {
            get => m_CursorSpeed;
            set => m_CursorSpeed = value;
        }

        /// <summary>
        /// ����������, ����� ������������� ������� ������������. ���� ������ �������� <see cref="CursorMode.SoftwareCursor"/>
        /// (�������� �� ���������), ����� <see cref="cursorGraphic"/> � <see cref="cursorTransform"/> ���������� ����������� ������
        /// ������� ��������������� � ������������ � ����������  <see cref="virtualMouse"/>. ���� ��� ����� ��������� ����������� �������� <see
        /// cref="CursorMode.HardwareCursorIfAvailable"/> � ������������ ����������� ���������� <see cref="Mouse"/> ��������� ���������� ���������� 
        /// ���� ����������� ���� � �������� ��� (����� ��� ����� �� ������������ ������� ����������). 
        /// ����� �� ����� ������������ <see cref="Mouse.WarpCursorPosition"/> ����� ����������� ��������� ������ ���� � ���������,
        /// ��������������� ��������� <see cref="virtualMouse"/>.  � ���� ������, <see cref="cursorGraphic"/>
        /// ����� ��������� � <see cref="cursorTransform"/> ����������� �� �����.
        /// </summary>
        /// <value> ������� �� �������� ��������� ������ ���� (���� �� ������������) � ������������ � ���������� ����������� ����.</value>
        /// <remarks>
        /// �������� ��������, ��� ���������� �� ����, ����� ����� ������������ ��� �������, ���������, ��� ���� � ������� ���� ����� ������� �� <see cref="virtualMouse"/>.
        ///
        /// �������� ��������, ��� ���� <see cref="CursorMode.HardwareCursorIfAvailable"/> ������������, ����������� ������ ��-�������� ������������, ���� ��� �����������
        /// <see cref="Mouse"/> ����������.
        /// </remarks>
        public CursorMode cursorMode
        {
            get => m_CursorMode;
            set
            {
                if (m_CursorMode == value)
                    return;

                // ���� �� ��������� ���, ���������, ��� �� ����� �������� ��������� ����.
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
        /// ����������� ������� UI, �������������� ������ ����.
        /// </summary>
        /// <value>����������� ������� ��� ������� ����������� ����.</value>
        /// <remarks>
        /// ���� <see cref="cursorMode"/> ����������� �������� <see cref="CursorMode.HardwareCursorIfAvailable"/>, ��� ����������� �����
        /// ���������.
        ///
        /// ����� ����, ���� ��������� ����������������� ���������� ������ ���������� <c>Canvas</c>, ������� ���������� ������� ������ ��� �������.
        /// �����, �� ������� ��������� ��� �����������, ����� ������ � ������� <c>GetComponentInParent</c>, � ����� <c>Canvas.pixelRect</c>
        /// ������ ������������ � �������� ������ ��������� ����������� �������.
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
        /// ��������� ��� ��������, ���������� �� <see cref="scrollWheelAction"/>.
        /// </summary>
        /// <value> ��������� ��� �������� ���������.</value>
        public float scrollSpeed
        {
            get => m_ScrollSpeed;
            set => m_ScrollSpeed = value;
        }

        /// <summary>
        /// ���������� ����������� ����, �� ������� ��������� ������ �������� ������.
        /// </summary>
        /// <value>��������� ����������� ���� ��� <c>null</c>.</value>
        /// <remarks>
        /// ��� ���������������� ������ ����� ����, ��� ��������� ��� ������� � ������ ���. �������� ��������, ���
        /// ��� ����������� ���������� ���������� �������� ����� ���������� ���������� ���������� ����
        /// �� ���������� �� ����� ��������� � �������, ���� ��������� �� �������.
        /// </remarks>
        public Mouse virtualMouse => m_VirtualMouse;

        /// <summary>
        /// ���� Vector2 stick, ������� ��������� �������� ����, �.�. <see cref="Pointer.position"/> ��
        /// <see cref="virtualMouse"/> � <a
        /// href="https://docs.unity3d.com/ScriptReference/RectTransform-anchoredPosition.html">anchoredPosition</a>
        /// �� <see cref="cursorTransform"/> (���� �����������).
        /// </summary>
        /// <value>���� � ������� ���������, ������� ��������� ���������� �������.</value>
        /// <remarks>
        /// ������ ��� ������ ���� ��������� � ��������� ����������, ����� ��� <see cref="Gamepad.leftStick"/> �/���
        /// <see cref="Gamepad.rightStick"/>.
        /// </remarks>
        public InputActionProperty stickAction
        {
            get => m_StickAction;
            set => SetAction(ref m_StickAction, value);
        }

        /// <summary>
        /// ������������ ���� ������, ������������, ����� <see cref="Mouse.leftButton"/> ������ ��
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>������� ������ ��� <see cref="Mouse.leftButton"/>.</value>
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
        /// ������������ ���� ������, ������������, ����� <see cref="Mouse.rightButton"/> ������ ��
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>������� ������ ��� <see cref="Mouse.rightButton"/>.</value>
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
        /// ������������ ���� ������, ������������, ����� <see cref="Mouse.middleButton"/> ������ ��
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>������� ������ ��� <see cref="Mouse.middleButton"/>.</value>
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
        /// ������������ ���� ������, ������������, ����� <see cref="Mouse.forwardButton"/> ������ ��
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>������� ������ ��� <see cref="Mouse.forwardButton"/>.</value>
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
        /// ������������ ���� ������, ������������, ����� <see cref="Mouse.forwardButton"/> ������ ��
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value>������� ������ ��� <see cref="Mouse.forwardButton"/>.</value>
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
        /// ������������ ���� �������� Vector2, ������������� �������� <see cref="Mouse.scroll"/> ��
        /// <see cref="virtualMouse"/>.
        /// </summary>
        /// <value> ���� ���  <see cref="Mouse.scroll"/>.</value>
        /// <remarks>
        /// � ������, ���� �� ������ ��������� ������ ������������ ���������, ������ ����������� <see cref="Composites.Vector2Composite"/>
        /// � ��������� ������ <c>Up</c> � <c>Down</c>, � <c>Left</c> � <c>Right</c>  ������� ��� �� � ���� �� ���������.
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
            // ������������� ��������� ����, ���� ��� ��������.
            if (m_CursorMode == CursorMode.HardwareCursorIfAvailable)
                TryEnableHardwareCursor();

            // �������� ���������� ����.
            if (m_VirtualMouse == null)
                m_VirtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            else if (!m_VirtualMouse.added)
                InputSystem.AddDevice(m_VirtualMouse);

            // ���������� ��������� ��������� �������.
            if (m_CursorTransform != null)
            {
                var position = m_CursorTransform.anchoredPosition;
                InputState.Change(m_VirtualMouse.position, position);
                m_SystemMouse?.WarpCursorPosition(position);
            }

            // ������������ � ���������� ������� ������.
            if (m_AfterInputUpdateDelegate == null)
                m_AfterInputUpdateDelegate = OnAfterInputUpdate;
            InputSystem.onAfterUpdate += m_AfterInputUpdateDelegate;

            // ������������� � ���������.
            if (m_ButtonActionTriggeredDelegate == null)
                m_ButtonActionTriggeredDelegate = OnButtonActionTriggered;
            SetActionCallback(m_LeftButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_RightButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_MiddleButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_ForwardButtonAction, m_ButtonActionTriggeredDelegate, true);
            SetActionCallback(m_BackButtonAction, m_ButtonActionTriggeredDelegate, true);

            // �������� ��������.
            m_StickAction.action?.Enable();
            m_LeftButtonAction.action?.Enable();
            m_RightButtonAction.action?.Enable();
            m_MiddleButtonAction.action?.Enable();
            m_ForwardButtonAction.action?.Enable();
            m_BackButtonAction.action?.Enable();
            m_ScrollWheelAction.action?.Enable();

            m_ScrollWheelAction.action.started += ScrollWheelAction_started;
            m_ScrollWheelAction.action.performed += ScrollWheelAction_performed; // ������� ���� ������ � ������� 1 ���.
            m_ScrollWheelAction.action.canceled += ScrollWheelAction_canceled;

            m_Canvas.sortingOrder = 100; //Canvas � ����� ������� �������� ���������� ������ ����� ������������ ��� Canvas � ����� ������ �������� ����������. 
            
            if(_gameInput!= null)
                _gameInput.OnGameDeviceChanged += GameInput_OnGameDeviceChanged;            

            ResetMouseToCenter();
            UpdateVisibility();
        }       

        protected void OnDisable()
        {
            // ������� ���������� ����.
            if (m_VirtualMouse != null && m_VirtualMouse.added)
                InputSystem.RemoveDevice(m_VirtualMouse);

            // ��������� ��������� ����.
            if (m_SystemMouse != null)
            {
                InputSystem.EnableDevice(m_SystemMouse);
                m_SystemMouse = null;
            }

            // ������� ���� �� input update.
            if (m_AfterInputUpdateDelegate != null)
                InputSystem.onAfterUpdate -= m_AfterInputUpdateDelegate;

            // ��������� ��������.
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

            // ���������� �� ��������.
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
            // ���� ���������� � OnEnable() �� ����� �� ������� Hide(){gameObject.SetActive(false);} � �������� ���� ������ �� �� ������������� ���������, � �� �� ������ ������ ��� ��������, 
            // � ��� �������� ����������� ���� ���� gameObject.SetActive(false) � ��� ����� ������� �� ������ �������� ���� ������  
            _gameInput.OnGameDeviceChanged -= GameInput_OnGameDeviceChanged;
        }

        private void ScrollWheelAction_canceled(InputAction.CallbackContext obj) { _pressedForWhile = false; }
        private void ScrollWheelAction_performed(InputAction.CallbackContext obj) { _pressedForWhile = true; }
        private void ScrollWheelAction_started(InputAction.CallbackContext obj)
        {
            // �������� ������ ���������.
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
        /// ���������� �������� ���������� ������, � ���������������� ������� � ����������� �����
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

            // ���� ��� ��������� ���� �� ������� ����� ����������� ���� � ������ � ������
            if (m_SystemMouse == null)
            {
                if (m_CursorGraphic != null)
                    m_CursorGraphic.enabled = true;
                return;
            }

            // �������� ��������� ��� ��� �� �� ������������ �������
            InputSystem.DisableDevice(m_SystemMouse);

            // ������������� �������
            if (m_VirtualMouse != null)
                m_SystemMouse.WarpCursorPosition(m_VirtualMouse.position.value);

            // ��������� ����������� ������� ����.
            if (m_CursorGraphic != null)
                m_CursorGraphic.enabled = false;
        }

        private void UpdateMotion()
        {
            if (m_VirtualMouse == null)
                return;

            transform.localScale = Vector3.one * (1f / m_Canvas.transform.localScale.x); // ����� ������� �������, ��� ����������� ������������ ������� ����������� ����       
            m_CursorTransform.localScale = Vector3.one * (m_Canvas.transform.localScale.x); // ������������� ������ ����

            // �������� ������� �������� �����.
            var stickAction = m_StickAction.action;
            if (stickAction == null)
                return;

            var stickValue = stickAction.ReadValue<Vector2>();
            if (Mathf.Approximately(0, stickValue.x) && Mathf.Approximately(0, stickValue.y)) // ���������� ��� �������� � ��������� ������� � ���������� true, ���� ��� ������.
            {
                // �������� ������������. ������� ���������� ����� � ���������� �������� �����
                m_LastTime = default;
                m_LastStickValue = default;
            }
            else
            {
                var currentTime = InputState.currentTime;
                if (Mathf.Approximately(0, m_LastStickValue.x) && Mathf.Approximately(0, m_LastStickValue.y)) // ���� ���������� �������� ����� ���� ������� �� �� ������ ������ ����
                {
                    // �������� ��������.
                    m_LastTime = currentTime;
                }

                // ��������� ������.
                var deltaTime = (float)(currentTime - m_LastTime);
                var delta = new Vector2(m_CursorSpeed * stickValue.x * deltaTime, m_CursorSpeed * stickValue.y * deltaTime);

                // �������� �������.
                var currentPosition = m_VirtualMouse.position.value;
                var newPosition = currentPosition + delta;

                ////�����: ��� �������� ������������ �������, ��������� ��� �� ���-������ ������?
                //��������� ������� ������. ����� ��������� ������
                // Vector2 anchoredPosition = _cursorVisual.anchoredPosition;
                /*float edgeSize = 20; // (���������� ��������) ������ �� ���� ������ 
                newPosition.x = Mathf.Clamp(newPosition.x, edgeSize, Screen.width - edgeSize);
                newPosition.y = Mathf.Clamp(newPosition.y, edgeSize, Screen.height - edgeSize);*/

                newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
                newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);


                ////�����: ��� ����, ��� � ��� ��� ������� ��� ���, ��������, ��� �������� �� ����� ����� �������������� ������� ��� �����������; ��������?
                InputState.Change(m_VirtualMouse.position, newPosition);
                InputState.Change(m_VirtualMouse.delta, delta);

                // �������� SoftwareCursor �������������� �������, ���� ������� �������.
                if (m_CursorTransform != null &&
                    (m_CursorMode == CursorMode.SoftwareCursor ||
                     (m_CursorMode == CursorMode.HardwareCursorIfAvailable && m_SystemMouse == null)))
                    m_CursorTransform.anchoredPosition = newPosition;

                m_LastStickValue = stickValue;
                m_LastTime = currentTime;

                // �������� ���������� ������.
                m_SystemMouse?.WarpCursorPosition(newPosition);
            }



            // �������� ������ ���������.
            var scrollAction = m_ScrollWheelAction.action;
            if (scrollAction != null && _pressedForWhile)
            {
                var scrollValue = scrollAction.ReadValue<Vector2>();
                scrollValue.x *= m_ScrollSpeed;
                scrollValue.y *= m_ScrollSpeed;

                InputState.Change(m_VirtualMouse.scroll, scrollValue);
            }
        }

        [Tooltip("�����, �������� �������� ����������� ������������ �������.")]
        [SerializeField] private Canvas m_Canvas; // �����, �������� �������� ����������� ������������ �������.       

        [Header("Cursor")]
        [Tooltip("������ �� ��������� ������������� ��������� ������� ����������� ������� ����, ���� ������� ��������. ���� ��, �� "
            + "����������� ������, �� ������� ��������� \"Cursor Graphic\", ����� �����.")]
        [SerializeField] private CursorMode m_CursorMode;
        [Tooltip("�������� \"CursorVisual\". ����������� �����������, �������������� ����������� ������. ��� ������, ���� ������������ ���������� ������ (��. \"Cursor Mode\").")]
        [SerializeField] private Graphic m_CursorGraphic;
        [Tooltip(" �������� \"CursorVisual\". �������������� ��� ������������ �������. ����� ����������� ������ � ��� ������, ���� ������������ ����������� ������ (��. \"Cursor Mode\"). ����������� �������"
            + " ��������� ������������ ��������� ��������������.")]
        [SerializeField] private RectTransform m_CursorTransform;

        [Header("Motion")]
        [Tooltip("�������� ����������� ������� � �������� � �������. �������������� �� ����� �� \"Stick Action\".")]
        [SerializeField] private float m_CursorSpeed = 400;
        [Tooltip("���������� �����������, ����������� � \"Scroll Wheel Action\" ��� ��������� �������� ���������� \"'scroll Wheel\" ����.")]
        [SerializeField] private float m_ScrollSpeed = 45;

        [Space(10)]
        [Tooltip("�������� Vector2, ������� ���������� ������ �����/������ (X) � �����/���� (Y) �� ������.")]
        [SerializeField] private InputActionProperty m_StickAction;
        [Tooltip("�������� ������, ���������� ������ ����� ������� ����.")]
        [SerializeField] private InputActionProperty m_LeftButtonAction;
        [Tooltip("�������� ������, ���������� ������� ������ ����.")]
        [SerializeField] private InputActionProperty m_MiddleButtonAction;
        [Tooltip("�������� ������, ���������� ������ ������ ������� ����.")]
        [SerializeField] private InputActionProperty m_RightButtonAction;
        [Tooltip("�������� ������, ����������� ������� ������ ��������� ������ (������ �4) ������� ����.")]
        [SerializeField] private InputActionProperty m_ForwardButtonAction;
        [Tooltip("�������� ������, ����������� ������ �������� (������ �5) ������� ����.")]
        [SerializeField] private InputActionProperty m_BackButtonAction;
        [Tooltip("�������� Vector2, ������� ������������� � �������� \"scrollWheel\" ���� (�������������� �� \"Scroll Speed\"). � ������� \"Interactions\" �������� \"Hold\" � ��������� \"Hold Time = 1���\". ���� ������� ������ � ������� 1 ���, ����� �������� ��������� � \"Update\" �� ��������")]
        [SerializeField] private InputActionProperty m_ScrollWheelAction;

        private Mouse m_VirtualMouse;
        private Mouse m_SystemMouse;
        private Action m_AfterInputUpdateDelegate;
        private Action<InputAction.CallbackContext> m_ButtonActionTriggeredDelegate;
        private double m_LastTime;
        private Vector2 m_LastStickValue;
        private GameInput _gameInput;

        /// <summary>
        /// ������ � ������� ���������� �������
        /// </summary>
        private bool _pressedForWhile;

        private void OnButtonActionTriggered(InputAction.CallbackContext context)
        {
            if (m_VirtualMouse == null)
                return;

            // �������� ���������� �������� �������� �������� ���������� ����������. �� �� ����� (����?) ������������ ��������� �����.�������� �� ���������
            // ��������� ���� ��������� ����������, ��������� �������� ���������� ��������� InputManager ������������ ������
            // ���������� ������� ������. ����, �� ������ �������� ������ ��������� ����� ����������� ����, ����� ���������
            // ������ ���, � ����� ������ �������������� ��� ���������.

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
                m_VirtualMouse.CopyState<MouseState>(out var mouseState);// ���������� ��������� ���������������.
                mouseState.WithButton(button.Value, isPressed); //������ ��������� ������ ������� �������� . ������������ ������ ���� � ������� �� ��������

                InputState.Change(m_VirtualMouse, mouseState); // ������� ��������� ����������� ���� �� �����.
            }
        }

        private static void SetActionCallback(InputActionProperty field, Action<InputAction.CallbackContext> callback, bool install = true)
        {
            var action = field.action;
            if (action == null)
                return;

            // ��� �� ����� ����������� �������� �����, ��������� ���� ������ ���� �������� ���������, �, �������������,
            // ��� ���������� ������ ���������� (1) � ���������� (0).

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
        /// ����������, ��� ����� ����������� ������ ����������� ����.
        /// </summary>
        /// <seealso cref="cursorMode"/>
        public enum CursorMode
        {
            /// <summary>
            /// ������ ����������� � ���� UI ����������. ������ <see cref="cursorGraphic"/>.
            /// </summary>
            SoftwareCursor,

            /// <summary>
            /// ���� ������������ ����������� ���������� <see cref="Mouse"/>  ��� ������ ����� �������������� � �����������
            /// ����������� ����� � �������  <see cref="Mouse.WarpCursorPosition"/>. ������ �� ����������� ������
            /// <see cref="cursorGraphic"/> ����� ���������.
            ///
            /// �������� ��������, ��� ���� ��� �����������  <see cref="Mouse"/> ��������� �������� � ��������
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

