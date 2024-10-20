using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

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
    public async UniTask LoadAndToggleVisible(Action closeDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_loadGameSubMenuUI == null)
        {
            _isStartedLoad = true;
            _loadGameSubMenuUI = await Load<LoadGameSubMenuUI>(AssetsConstants.LoadGameSubMenu);
            _loadGameSubMenuUI.Init(_gameInput, _hashAnimationName);
            _loadGameSubMenuUI.ToggleVisible(new List<Action> { closeDelegate, SetUnload });
        }        
    }

    private void SetUnload()
    {
        Unload();
        _loadGameSubMenuUI = null;
        _isStartedLoad = false;
    }

}
