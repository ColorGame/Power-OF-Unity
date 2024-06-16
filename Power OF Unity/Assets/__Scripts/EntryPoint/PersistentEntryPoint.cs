using UnityEngine;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Неразрушаемая точка входа с подсистемами в качестве дочерних, или внутри если они не наследуют MonoBehaviour.
/// </summary>.
/// <remarks>
/// Пока этот объект создает Bootstrapper, НО при БИЛДИНГЕ добавим его в Bootstrap сцену. 
/// </remarks>
public class PersistentEntryPoint : MonoBehaviour, IEntryPoint
{

    private VirtualMouseCustom _virtualMouseCustom;
    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private OptionsMenuUI _optionsMenuUI;
    private TooltipUI _tooltipUI;
    private GameInput _gameInput;

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
        _optionsMenuUI = GetComponentInChildren<OptionsMenuUI>(true);
        _tooltipUI = GetComponentInChildren<TooltipUI>(true);
    }

    private void Register(DIContainer rootContainer)
    {      
        _gameInput = rootContainer.RegisterSingleton(c => new GameInput()).CreateInstance();
        _soundManager = rootContainer.RegisterSingleton(c => _soundManager).CreateInstance();
        rootContainer.RegisterSingleton(c => new JsonSaveableEntity());
        rootContainer.RegisterSingleton(c => _virtualMouseCustom);
        rootContainer.RegisterSingleton(c => _musicManager);
        rootContainer.RegisterSingleton(c => _optionsMenuUI);
        rootContainer.RegisterSingleton(c => _tooltipUI);
        rootContainer.RegisterSingleton(c => new UnitManager(_tooltipUI));
    }

    private void Init()
    {
        _virtualMouseCustom.Init(_gameInput);
        _optionsMenuUI.Init(_gameInput, _soundManager, _musicManager);
        _tooltipUI.Init(_gameInput, _virtualMouseCustom);
    }
  
    
}

