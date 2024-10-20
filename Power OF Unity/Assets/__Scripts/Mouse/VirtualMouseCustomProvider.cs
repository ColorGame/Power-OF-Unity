using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem.UI;

public class VirtualMouseCustomProvider: LocalAssetLoader
{
    public VirtualMouseCustomProvider(GameInput gameInput)
    {
        Init(gameInput);
    }

    private GameInput _gameInput;
    private VirtualMouseCustom _virtualMouseCustom;

    private void Init(GameInput gameInput)
    {
        _gameInput = gameInput;

        if (_gameInput.GetActiveGameDevice() == GameInput.GameDevice.Gamepad) // Если игрок начинает игру с геймпада
        {
            // загрузить асинхроно и инициализировать
            LoadAndInit().Forget(); // запустил и забыл
        }
        _gameInput.OnGameDeviceChanged += GameInput_OnGameDeviceChanged; ;
    }

    private void GameInput_OnGameDeviceChanged(object sender, GameInput.GameDevice gameDevice)
    {
        if (gameDevice == GameInput.GameDevice.Gamepad)
        {
            // загрузить асинхроно и инициализировать
            LoadAndInit().Forget();            
        }
        else
        {
            // выгрузить и удалить (надо изменить логику в классе VirtualMouseCustom)
            _virtualMouseCustom.Unload();
            Unload();
        }
    }

    private async UniTask LoadAndInit()
    {
        _virtualMouseCustom = await Load<VirtualMouseCustom>(AssetsConstants.VirtualMouseCustom);
        _virtualMouseCustom.InitOnLoad();
    }

    public VirtualMouseCustom GetVirtualMouseCustom() { return _virtualMouseCustom; }
}
