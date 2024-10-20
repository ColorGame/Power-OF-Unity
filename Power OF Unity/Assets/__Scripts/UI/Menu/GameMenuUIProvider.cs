using Cysharp.Threading.Tasks;
using Pathfinding.Ionic.Zip;
using System;
using System.Collections.Generic;

public class GameMenuUIProvider : LocalAssetLoader
{
    public GameMenuUIProvider(GameInput gameInput, HashAnimationName hashAnimationName, OptionsSubMenuUIProvider optionsSubMenuUIProvider, QuitGameSubMenuUIProvider quitGameSubMenuUIProvider, SaveGameSubMenuUIProvider saveGameSubMenuUIProvider, LoadGameSubMenuUIProvider loadGameSubMenuUIProvider )
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;
        _optionsSubMenuUIProvider = optionsSubMenuUIProvider;
        _quitGameSubMenuUIProvider = quitGameSubMenuUIProvider;
        _saveGameSubMenuUIProvider = saveGameSubMenuUIProvider;
        _loadGameSubMenuUIProvider = loadGameSubMenuUIProvider;
    }

    private GameInput _gameInput;
    private HashAnimationName _hashAnimationName;
    private OptionsSubMenuUIProvider _optionsSubMenuUIProvider;
    private QuitGameSubMenuUIProvider _quitGameSubMenuUIProvider;
    private SaveGameSubMenuUIProvider _saveGameSubMenuUIProvider;
    private LoadGameSubMenuUIProvider _loadGameSubMenuUIProvider;

    private GameMenuUI _gameMenuUI;
    private bool _isStartedLoad = false; // �������� ������?(��������� ������� ������� �� ������ � ��������� �����)

    /// <summary>
    /// ��������� � ��������� ���������. ��������� �������������� �������, ������� �������, ��� �������� ����
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action �loseDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_gameMenuUI == null)
        {
            _isStartedLoad = true;
            _gameMenuUI = await Load<GameMenuUI>(AssetsConstants.GameMenu);
            _gameMenuUI.Init(_gameInput, _hashAnimationName, _optionsSubMenuUIProvider, _quitGameSubMenuUIProvider, _saveGameSubMenuUIProvider, _loadGameSubMenuUIProvider);
            
            if(�loseDelegate!=null)
                _gameMenuUI.SetCloseDelegate(�loseDelegate);
            _gameMenuUI.SetUnloadDelegate(SetUnload);

            _gameMenuUI.ToggleVisible();
        }
    }

    private void SetUnload()
    {
        Unload();
        _gameMenuUI = null;
        _isStartedLoad = false;
    }

}
