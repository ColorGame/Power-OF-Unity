
using UnityEngine;



public class MainMenuEntryPoint : MonoBehaviour, IEntryPoint
{
    private MainMenuUI _mainMenuUI;

    public void Process(DIContainer container)
    {
        GetComponent();        
        Init(container);
    }

    private void GetComponent()
    {
        _mainMenuUI = GetComponentInChildren<MainMenuUI>(true);
    }

    private void Init(DIContainer container)
    {
        _mainMenuUI.Init(container.Resolve<OptionsMenuUI>(), container.Resolve<ScenesService>());
    }
    

    private void Start()
    {
        // Анимация меню
    }
}
