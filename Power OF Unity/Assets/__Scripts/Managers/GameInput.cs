using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Все манипуляции внешних девайсов(джойстик, мышь, клавиатура...) должны проходить через этот класс.
/// Не наследует MonoBehaviour. Реализован патерн singleton.
/// </summary>
public class GameInput   
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public GameInput()
    {
        Init();
    }
    /*
        private GameInput() { InitOnLoad(); } // Конструктор private что бы нельзя было создать  new GameInput() 

        // Тип Lazy потокобезопасная. Ленивая загрузка. Все, что вам нужно сделать, это передать делегат конструктору, который вызывает конструктор Singleton, которому передается лямбда-выражение:
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

    public event EventHandler OnBindingRebind; // При повторной привязке
    public event EventHandler<GameDevice> OnGameDeviceChanged;
   
    public enum GameDevice
    {
        KeyboardMouse,
        Gamepad,
    }

    public enum Binding // Привязки
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


    private PlayerInputActions _playerInpytActions; // Объявим - Действия, вводимые игроком (NEW INPUT SYSTEM)
    private GameDevice _activeGameDevice = GameDevice.KeyboardMouse;


    private void Init()
    {
        _playerInpytActions = new PlayerInputActions(); //Создадим и сохраним новый экземпляр Ввода 

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) // Сохраненая привязка ввода
        {
            _playerInpytActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        _playerInpytActions.Player.Enable(); // Откроем карту действий (мы назвали ее Player в InputActions) и включим ее, что бы использовать этот актив        

        InputSystem.onActionChange += InputSystem_onActionChange; // Срабатывает когда игрок коснется любого ввода

        _playerInpytActions.Player.Click.performed += (Context) => { OnClickAction?.Invoke(this, EventArgs.Empty); };
        _playerInpytActions.Player.MenuAlternate.performed += (Context) => { OnMenuAlternate?.Invoke(this, EventArgs.Empty); };
        _playerInpytActions.Player.Scroll_CameraZoom.performed += (Context) => { OnCameraZoomAction?.Invoke(this, Context.ReadValue<float>()); };
        _playerInpytActions.Player.CameraRotate.performed += (Context) => { OnCameraRotateAction?.Invoke(this, Context.ReadValue<float>()); };

        _playerInpytActions.Player.CameraMovement.started += (Context) => { OnCameraMovementStarted?.Invoke(this, EventArgs.Empty); };
        _playerInpytActions.Player.CameraMovement.canceled += (Context) => { OnCameraMovementCanceled?.Invoke(this, EventArgs.Empty); };      
    }

    private void InputSystem_onActionChange(object arg1, InputActionChange inputActionChange)
    {
        if (inputActionChange == InputActionChange.ActionPerformed && arg1 is InputAction) // Если это выполненное действие(Performed Выполненный) 
        {
            InputAction inputAction = arg1 as InputAction;
            if (inputAction.activeControl.device.displayName == "VirtualMouse")
            {
                // Игнорируем Виртуальную мыш
                return;
            }
            if (inputAction.activeControl.device is Gamepad) // Если это Gamepad, то заменим ативное устройство на Gamepad
            {
                if (_activeGameDevice != GameDevice.Gamepad)
                {
                    ChangeActiveGameDevice(GameDevice.Gamepad);
                }
            }
            else // В ПРОТИВНОМ СЛУЧАЕ заменим ативное устройство на KeyboardMouse
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
        _activeGameDevice = activeGameDevice; // Установим активное устройство
        Debug.Log("Новый Активный Игровой Девайс: " + activeGameDevice);

        Cursor.visible = activeGameDevice == GameDevice.KeyboardMouse;

        OnGameDeviceChanged?.Invoke(this, _activeGameDevice);
    }

    public GameDevice GetActiveGameDevice()
    {
        return _activeGameDevice;
    }
    /// <summary>
    ///  Вернуть точку мыши на экране (вернет значение в количестве пикселей а не в метрах)
    /// </summary>
    public Vector2 GetMouseScreenPoint()
    {
        return Mouse.current.position.ReadValue();
    }

    public Vector2 GetCameraMoveVector() // Получить Вектор Движения Камеры
    {
        return _playerInpytActions.Player.CameraMovement.ReadValue<Vector2>(); //Зайдем в менеджер ввода, в Карту действий (Player), в действие Движение камеры (CameraMovement), и прочитаем преданные значения определив их тип <Vector2>
    }


        /*     
        public bool IsEscButtonDownThisFrame() // Нажата кнопка Esc в этот кадр
        {
            return _playerInpytActions.Player.MenuAlternate.WasPressedThisFrame(); //Был Нажат Этот Кадр возвращает true если на этом кадре была нажата Esc        
        }

        public bool IsMouseButtonDownThisFrame() // Была Нажата кнопка мыши в этот кадр
        {
            return _playerInpytActions.Player.Click.WasPressedThisFrame(); //Был Нажат Этот Кадр возвращает true если на этом кадре была нажата левая кнопка мыши
        }

        public bool IsMouseButtonPressedThisFrame() // Была Зажата кнопка мыши в этот кадр
        {
            return _playerInpytActions.Player.Click.IsPressed(); //Была Зажата Этот Кадр возвращает true если на этом кадре была Зажата левая кнопка мыши
        }
         */
   
}
