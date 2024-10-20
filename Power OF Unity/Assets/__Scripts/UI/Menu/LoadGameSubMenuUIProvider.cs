using Cysharp.Threading.Tasks;
using System;

public class LoadGameSubMenuUIProvider : LocalAssetLoader
{
    public LoadGameSubMenuUIProvider(GameInput gameInput, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;
    }
    private GameInput _gameInput;
    private HashAnimationName _hashAnimationName;

    private LoadGameSubMenuUI _loadGameSubMenuUI;
    private bool _isStartedLoad = false; // Загрузка начата?

    /// <summary>
    /// Загрузить и переключи видимость. Принимает необязательный делегат, который вызовим, при закрытии меню
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action сloseDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_loadGameSubMenuUI == null)
        {
            _isStartedLoad = true;
            _loadGameSubMenuUI = await Load<LoadGameSubMenuUI>(AssetsConstants.LoadGameSubMenu);
            _loadGameSubMenuUI.Init(_gameInput, _hashAnimationName);

            if (сloseDelegate != null)
                _loadGameSubMenuUI.SetCloseDelegate(сloseDelegate);
            _loadGameSubMenuUI.SetUnloadDelegate(SetUnload);

            _loadGameSubMenuUI.ToggleVisible();
        }        
    }

    private void SetUnload()
    {
        Unload();
        _loadGameSubMenuUI = null;
        _isStartedLoad = false;
    }

}
