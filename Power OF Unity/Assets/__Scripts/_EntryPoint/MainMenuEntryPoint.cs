using UnityEngine;

public class MainMenuEntryPoint : MonoBehaviour, IEntryPoint
{
   [SerializeField] private MainMenuUI _mainMenuUI;

    public void Inject(DIContainer container)
    {              
        Init(container);
    }


    private void Init(DIContainer container)
    {
       _mainMenuUI.Init(container.Resolve<OptionsSubMenuUIProvider>(),  container.Resolve<LoadGameSubMenuUIProvider>(), container.Resolve<ScenesService>(), container.Resolve<HashAnimationName>());
    }    

}
