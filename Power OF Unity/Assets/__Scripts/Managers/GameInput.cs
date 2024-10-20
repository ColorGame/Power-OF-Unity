using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// ��� ����������� ������� ��������(��������, ����, ����������...) ������ ��������� ����� ���� �����.
/// �� ��������� MonoBehaviour. ���������� ������ singleton.
/// </summary>
public class GameInput   
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public GameInput()
    {
        Init();
    }
    /*
        private GameInput() { InitOnLoad(); } // ����������� private ��� �� ������ ���� �������  new GameInput() 

        // ��� Lazy ����������������. ������� ��������. ���, ��� ��� ����� �������, ��� �������� ������� ������������, ������� �������� ����������� Singleton, �������� ���������� ������-���������:
        private static readonly Lazy<GameInput> lazy =
            new Lazy<GameInput>(() => new GameInput());

        public static GameInput Instance { get { return lazy.Value; } }
    */

    public event EventHandler OnClickAction;
    public event EventHandler OnMenuAlternate;
    public event EventHandler OnCameraMovementStarted;
    public event EventHandler OnCameraMovementCanceled;
    public event EventHandler<float> OnCameraRotateAction;
    public event EventHandler<float> OnCameraZoomAction;

    public event EventHandler OnBindingRebind; // ��� ��������� ��������
    public event EventHandler<GameDevice> OnGameDeviceChanged;
   
    public enum GameDevice
    {
        KeyboardMouse,
        Gamepad,
    }

    public enum Binding // ��������
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Click,
        MenuAlternate,
        Gamepad_Click,
        Gamepad_MenuAlternate,
    }


    private PlayerInputActions _playerInpytActions; // ������� - ��������, �������� ������� (NEW INPUT SYSTEM)
    private GameDevice _activeGameDevice = GameDevice.KeyboardMouse;


    private void Init()
    {
        _playerInpytActions = new PlayerInputActions(); //�������� � �������� ����� ��������� ����� 

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) // ���������� �������� �����
        {
            _playerInpytActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        _playerInpytActions.Player.Enable(); // ������� ����� �������� (�� ������� �� Player � InputActions) � ������� ��, ��� �� ������������ ���� �����        

        InputSystem.onActionChange += InputSystem_onActionChange; // ����������� ����� ����� �������� ������ �����

        _playerInpytActions.Player.Click.performed += (Context) => { OnClickAction?.Invoke(this, EventArgs.Empty); };
        _playerInpytActions.Player.MenuAlternate.performed += (Context) => { OnMenuAlternate?.Invoke(this, EventArgs.Empty); };
        _playerInpytActions.Player.Scroll_CameraZoom.performed += (Context) => { OnCameraZoomAction?.Invoke(this, Context.ReadValue<float>()); };
        _playerInpytActions.Player.CameraRotate.performed += (Context) => { OnCameraRotateAction?.Invoke(this, Context.ReadValue<float>()); };

        _playerInpytActions.Player.CameraMovement.started += (Context) => { OnCameraMovementStarted?.Invoke(this, EventArgs.Empty); };
        _playerInpytActions.Player.CameraMovement.canceled += (Context) => { OnCameraMovementCanceled?.Invoke(this, EventArgs.Empty); };      
    }

    private void InputSystem_onActionChange(object arg1, InputActionChange inputActionChange)
    {
        if (inputActionChange == InputActionChange.ActionPerformed && arg1 is InputAction) // ���� ��� ����������� ��������(Performed �����������) 
        {
            InputAction inputAction = arg1 as InputAction;
            if (inputAction.activeControl.device.displayName == "VirtualMouse")
            {
                // ���������� ����������� ���
                return;
            }
            if (inputAction.activeControl.device is Gamepad) // ���� ��� Gamepad, �� ������� ������� ���������� �� Gamepad
            {
                if (_activeGameDevice != GameDevice.Gamepad)
                {
                    ChangeActiveGameDevice(GameDevice.Gamepad);
                }
            }
            else // � ��������� ������ ������� ������� ���������� �� KeyboardMouse
            {
                if (_activeGameDevice != GameDevice.KeyboardMouse)
                {
                    ChangeActiveGameDevice(GameDevice.KeyboardMouse);
                }
            }
        }
    }

    private void ChangeActiveGameDevice(GameDevice activeGameDevice)
    {
        _activeGameDevice = activeGameDevice; // ��������� �������� ����������
        Debug.Log("����� �������� ������� ������: " + activeGameDevice);

        Cursor.visible = activeGameDevice == GameDevice.KeyboardMouse;

        OnGameDeviceChanged?.Invoke(this, _activeGameDevice);
    }

    public GameDevice GetActiveGameDevice()
    {
        return _activeGameDevice;
    }
    /// <summary>
    ///  ������� ����� ���� �� ������ (������ �������� � ���������� �������� � �� � ������)
    /// </summary>
    public Vector2 GetMouseScreenPoint()
    {
        return Mouse.current.position.ReadValue();
    }

    public Vector2 GetCameraMoveVector() // �������� ������ �������� ������
    {
        return _playerInpytActions.Player.CameraMovement.ReadValue<Vector2>(); //������ � �������� �����, � ����� �������� (Player), � �������� �������� ������ (CameraMovement), � ��������� ��������� �������� ��������� �� ��� <Vector2>
    }


        /*     
        public bool IsEscButtonDownThisFrame() // ������ ������ Esc � ���� ����
        {
            return _playerInpytActions.Player.MenuAlternate.WasPressedThisFrame(); //��� ����� ���� ���� ���������� true ���� �� ���� ����� ���� ������ Esc        
        }

        public bool IsMouseButtonDownThisFrame() // ���� ������ ������ ���� � ���� ����
        {
            return _playerInpytActions.Player.Click.WasPressedThisFrame(); //��� ����� ���� ���� ���������� true ���� �� ���� ����� ���� ������ ����� ������ ����
        }

        public bool IsMouseButtonPressedThisFrame() // ���� ������ ������ ���� � ���� ����
        {
            return _playerInpytActions.Player.Click.IsPressed(); //���� ������ ���� ���� ���������� true ���� �� ���� ����� ���� ������ ����� ������ ����
        }
         */
   
}
