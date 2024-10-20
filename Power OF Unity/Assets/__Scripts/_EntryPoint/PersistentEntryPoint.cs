using UnityEngine;

/// <summary>
/// Неразрушаемая точка входа с подсистемами в качестве дочерних, или внутри если они не наследуют MonoBehaviour.
/// </summary>.
public class PersistentEntryPoint : MonoBehaviour, IEntryPoint
{
    [SerializeField] private Camera _cameraUI;
    [SerializeField] private MusicManager _musicManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private TooltipUI _tooltipUI;
    private ScenesService _scenesService;
    private VirtualMouseCustomProvider _virtualMouseCustomProvider;    
    private GameInput _gameInput;
    private JsonSaveableEntity _jsonSaveableEntity;
    private WarehouseManager _warehouseManager;
    private UnitManager _unitManager;
    private HashAnimationName _hashAnimationName;
    private OptionsSubMenuUIProvider _optionsSubMenuUIProvider;
    private LoadGameSubMenuUIProvider _loadGameSubMenuUIProvider;
    private SaveGameSubMenuUIProvider _saveGameSubMenuUIProvider;
    private QuitGameSubMenuUIProvider _quitGameSubMenuUIProvider;
    private GameMenuUIProvider _gameMenuUIProvider;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Inject(DIContainer rootContainer)
    {
        CreateInstanceClass(rootContainer);
        Register(rootContainer);
        Init();
    }

    private void CreateInstanceClass(DIContainer container)
    {         
        _scenesService = new ScenesService(container.Resolve<LoadingScreenProvider>());
        _gameInput = new GameInput();
        _virtualMouseCustomProvider = new VirtualMouseCustomProvider(_gameInput);
        _jsonSaveableEntity = new JsonSaveableEntity();
        _warehouseManager = new WarehouseManager(_tooltipUI);
        _unitManager = new UnitManager(_tooltipUI, _soundManager, _warehouseManager); // После реализации сохранения можно будет его создавать в каждой сцене
        _hashAnimationName = new HashAnimationName();
        _optionsSubMenuUIProvider = new OptionsSubMenuUIProvider(_gameInput, _soundManager, _musicManager, _hashAnimationName);
        _loadGameSubMenuUIProvider = new LoadGameSubMenuUIProvider(_gameInput, _hashAnimationName);
        _saveGameSubMenuUIProvider = new SaveGameSubMenuUIProvider(_gameInput, _hashAnimationName);
        _quitGameSubMenuUIProvider = new QuitGameSubMenuUIProvider(_gameInput,_scenesService, _hashAnimationName);
        _gameMenuUIProvider = new GameMenuUIProvider(_gameInput, _hashAnimationName, _optionsSubMenuUIProvider, _quitGameSubMenuUIProvider, _saveGameSubMenuUIProvider, _loadGameSubMenuUIProvider);
    }

    private void Register(DIContainer rootContainer)
    {
        rootContainer.RegisterSingleton(c => _cameraUI);
        rootContainer.RegisterSingleton(c => _scenesService);
        rootContainer.RegisterSingleton(c => _gameInput);
        rootContainer.RegisterSingleton(c => _jsonSaveableEntity);
        rootContainer.RegisterSingleton(c => _virtualMouseCustomProvider);
        rootContainer.RegisterSingleton(c => _soundManager);
        rootContainer.RegisterSingleton(c => _musicManager);        
        rootContainer.RegisterSingleton(c => _tooltipUI);
        rootContainer.RegisterSingleton(c => _warehouseManager);
        rootContainer.RegisterSingleton(c => _unitManager);
        rootContainer.RegisterSingleton(c => _hashAnimationName);
        rootContainer.RegisterSingleton(c => _optionsSubMenuUIProvider);
        rootContainer.RegisterSingleton(c => _loadGameSubMenuUIProvider);
        rootContainer.RegisterSingleton(c => _saveGameSubMenuUIProvider);
        rootContainer.RegisterSingleton(c => _quitGameSubMenuUIProvider);
        rootContainer.RegisterSingleton(c => _gameMenuUIProvider);
    }

    private void Init()
    {       
        _tooltipUI.Init(_gameInput, _virtualMouseCustomProvider);
    }


}

