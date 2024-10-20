using UnityEngine;

public class MainMenuEntryPoint : MonoBehaviour, IEntryPoint, IStartScene
{
   [SerializeField] private MainMenuUI _mainMenuUI;

    public void Inject(DIContainer container)
    {              
        Init(container);
    }
    public void StartScene()
    {
        _mainMenuUI.StartAnimation();
    }

    private void Init(DIContainer container)
    {
       _mainMenuUI.Init(container.Resolve<OptionsSubMenuUIProvider>(),  container.Resolve<LoadGameSubMenuUIProvider>(), container.Resolve<ScenesService>(), container.Resolve<HashAnimationName>());
    }    

}
