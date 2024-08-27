using UnityEngine;
using UnityEngine.InputSystem.UI;

/// <summary>
/// ������������� ����� ����� � ������������ � �������� ��������, ��� ������ ���� ��� �� ��������� MonoBehaviour.
/// </summary>.
/// <remarks>
/// ���� ���� ������ ������� Bootstrapper, �� ��� �������� ������� ��� � Bootstrap �����. 
/// </remarks>
public class PersistentEntryPoint : MonoBehaviour, IEntryPoint
{

    private VirtualMouseCustom _virtualMouseCustom;
    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private OptionsSubMenuUI _optionsMenuUI; // ������ ������ � ��������� �� ��������
    private LoadGameSubMenuUI _loadGameSubMenuUI;
    private TooltipUI _tooltipUI;
    private GameInput _gameInput;
    private JsonSaveableEntity _jsonSaveableEntity;
    private WarehouseManager _warehouseManager;
    private UnitManager _unitManager;

    public void Process(DIContainer rootContainer)
    {
        GetComponent();
        Register(rootContainer);
        Init();
    }

    private void GetComponent()
    {
        _virtualMouseCustom = GetComponentInChildren<VirtualMouseCustom>(true);
        _musicManager = GetComponentInChildren<MusicManager>(true);
        _soundManager = GetComponentInChildren<SoundManager>(true);
        _optionsMenuUI = GetComponentInChildren<OptionsSubMenuUI>(true);
        _loadGameSubMenuUI = GetComponentInChildren<LoadGameSubMenuUI>(true);
        _tooltipUI = GetComponentInChildren<TooltipUI>(true);
        _gameInput = new GameInput();
        _jsonSaveableEntity = new JsonSaveableEntity();
        _warehouseManager = new WarehouseManager(_tooltipUI);
        _unitManager = new UnitManager(_tooltipUI, _soundManager, _warehouseManager); // ����� ���������� ���������� ����� ����� ��� ��������� � ������ �����
    }

    private void Register(DIContainer rootContainer)
    {
        rootContainer.RegisterSingleton(c => _gameInput);
        rootContainer.RegisterSingleton(c => _jsonSaveableEntity);
        rootContainer.RegisterSingleton(c => _virtualMouseCustom);
        rootContainer.RegisterSingleton(c => _soundManager);
        rootContainer.RegisterSingleton(c => _musicManager);
        rootContainer.RegisterSingleton(c => _optionsMenuUI);
        rootContainer.RegisterSingleton(c => _loadGameSubMenuUI);
        rootContainer.RegisterSingleton(c => _tooltipUI);
        rootContainer.RegisterSingleton(c => _warehouseManager);
        rootContainer.RegisterSingleton(c => _unitManager);
    }

    private void Init()
    {
        _virtualMouseCustom.Init(_gameInput);
        _optionsMenuUI.Init(_gameInput, _soundManager, _musicManager);
        _loadGameSubMenuUI.Init(_gameInput);
        _tooltipUI.Init(_gameInput, _virtualMouseCustom);        
    }


}

