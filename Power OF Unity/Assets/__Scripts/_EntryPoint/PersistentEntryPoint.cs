using UnityEngine;

/// <summary>
/// Неразрушаемая точка входа с подсистемами в качестве дочерних, или внутри если они не наследуют MonoBehaviour.
/// </summary>.
public class PersistentEntryPoint : MonoBehaviour, IEntryPoint
{    
    [SerializeField] private MusicManager _musicManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private TooltipUI _tooltipUI;   
    private VirtualMouseCustomProvider _virtualMouseCustomProvider;    
    private GameInput _gameInput;
    private HashAnimationName _hashAnimationName;
    private JsonSaveableEntity _jsonSaveableEntity;
    private WarehouseManager _warehouseManager;
    private UnitManager _unitManager;
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
        _gameInput = new GameInput();
        _hashAnimationName = new HashAnimationName();
        _virtualMouseCustomProvider = new VirtualMouseCustomProvider(_gameInput);
        _jsonSaveableEntity = new JsonSaveableEntity();
        _warehouseManager = new WarehouseManager(_tooltipUI);
        _unitManager = new UnitManager(_tooltipUI, _soundManager, _warehouseManager, _hashAnimationName); // После реализации сохранения можно будет его создавать в каждой сцене
        _optionsSubMenuUIProvider = new OptionsSubMenuUIProvider(_gameInput, _soundManager, _musicManager, _hashAnimationName);
        _loadGameSubMenuUIProvider = new LoadGameSubMenuUIProvider(_gameInput, _hashAnimationName);
        _saveGameSubMenuUIProvider = new SaveGameSubMenuUIProvider(_gameInput, _hashAnimationName);
        _quitGameSubMenuUIProvider = new QuitGameSubMenuUIProvider(_gameInput, container.Resolve<ScenesService>(), _hashAnimationName);
        _gameMenuUIProvider = new GameMenuUIProvider(_gameInput, _hashAnimationName, _optionsSubMenuUIProvider, _quitGameSubMenuUIProvider, _saveGameSubMenuUIProvider, _loadGameSubMenuUIProvider);
    }

    private void Register(DIContainer container)
    {       
        container.RegisterSingleton(c => _gameInput);
        container.RegisterSingleton(c => _jsonSaveableEntity);
        container.RegisterSingleton(c => _virtualMouseCustomProvider);
        container.RegisterSingleton(c => _soundManager);
        container.RegisterSingleton(c => _musicManager);        
        container.RegisterSingleton(c => _tooltipUI);
        container.RegisterSingleton(c => _warehouseManager);
        container.RegisterSingleton(c => _unitManager);
        container.RegisterSingleton(c => _hashAnimationName);
        container.RegisterSingleton(c => _optionsSubMenuUIProvider);
        container.RegisterSingleton(c => _loadGameSubMenuUIProvider);
        container.RegisterSingleton(c => _saveGameSubMenuUIProvider);
        container.RegisterSingleton(c => _quitGameSubMenuUIProvider);
        container.RegisterSingleton(c => _gameMenuUIProvider);
    }

    private void Init()
    {       
        _tooltipUI.Init(_gameInput, _virtualMouseCustomProvider);
    }


}

