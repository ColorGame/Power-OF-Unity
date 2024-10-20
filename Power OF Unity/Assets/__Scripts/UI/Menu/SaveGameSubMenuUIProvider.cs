using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class SaveGameSubMenuUIProvider : LocalAssetLoader
{
    public SaveGameSubMenuUIProvider(GameInput gameInput, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;
    }

    private GameInput _gameInput;
    private HashAnimationName _hashAnimationName;

    private SaveGameSubMenuUI _saveGameSubMenuUI;
    private bool _isStartedLoad = false; // Загрузка начата?(исключает двойное нажатие на кнопку и повторный вызов)

    /// <summary>
    /// Загрузить и переключи видимость. Принимает необязательный делегат, который вызовим, при закрытии меню
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action closeDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_saveGameSubMenuUI == null)
        {
            _isStartedLoad = true;
            _saveGameSubMenuUI = await Load<SaveGameSubMenuUI>(AssetsConstants.SaveGameSubMenu);
            _saveGameSubMenuUI.Init(_gameInput, _hashAnimationName);

            var closeDelegateList = new List<Action>();
            if (closeDelegate != null)
            {
                closeDelegateList.Add(closeDelegate);            
            }
            closeDelegateList.Add(SetUnload);

            _saveGameSubMenuUI.ToggleVisible(closeDelegateList);
        }
    }

    private void SetUnload()
    {
        Unload();
        _saveGameSubMenuUI = null;
        _isStartedLoad = false;
    }
}
