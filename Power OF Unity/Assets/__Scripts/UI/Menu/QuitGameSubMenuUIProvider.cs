using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class QuitGameSubMenuUIProvider : LocalAssetLoader
{
    public QuitGameSubMenuUIProvider(GameInput gameInput, ScenesService scenesService, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _scenesService = scenesService;
        _hashAnimationName = hashAnimationName;
    }

    private GameInput _gameInput;
    private ScenesService _scenesService;
    private HashAnimationName _hashAnimationName;

    private QuitGameSubMenuUI _quitGameSubMenuUI;
    private bool _isStartedLoad = false; // Загрузка начата?(исключает двойное нажатие на кнопку и повторный вызов)

    /// <summary>
    /// Загрузить и переключи видимость. Принимает необязательный делегат, который вызовим, при закрытии меню
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action сloseDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_quitGameSubMenuUI == null)
        {
            _isStartedLoad = true;
            _quitGameSubMenuUI = await Load<QuitGameSubMenuUI>(AssetsConstants.QuitGameSubMenu);
            _quitGameSubMenuUI.Init(_gameInput, _scenesService, _hashAnimationName);

            if (сloseDelegate != null)
                _quitGameSubMenuUI.SetCloseDelegate(сloseDelegate);
            _quitGameSubMenuUI.SetUnloadDelegate(SetUnload);

            _quitGameSubMenuUI.ToggleVisible();
        }
    }

    private void SetUnload()
    {
        Unload();
        _quitGameSubMenuUI = null;
        _isStartedLoad = false;
    }
}
