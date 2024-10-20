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

        if (_gameInput.GetActiveGameDevice() == GameInput.GameDevice.Gamepad) // ���� ����� �������� ���� � ��������
        {
            // ��������� ��������� � ����������������
            LoadAndInit().Forget(); // �������� � �����
        }
        _gameInput.OnGameDeviceChanged += GameInput_OnGameDeviceChanged; ;
    }

    private void GameInput_OnGameDeviceChanged(object sender, GameInput.GameDevice gameDevice)
    {
        if (gameDevice == GameInput.GameDevice.Gamepad)
        {
            // ��������� ��������� � ����������������
            LoadAndInit().Forget();            
        }
        else
        {
            // ��������� � ������� (���� �������� ������ � ������ VirtualMouseCustom)
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
