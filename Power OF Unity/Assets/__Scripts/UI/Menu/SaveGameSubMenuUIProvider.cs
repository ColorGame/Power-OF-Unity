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
    private bool _isStartedLoad = false; // �������� ������?(��������� ������� ������� �� ������ � ��������� �����)

    /// <summary>
    /// ��������� � ��������� ���������. ��������� �������������� �������, ������� �������, ��� �������� ����
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action �loseDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_saveGameSubMenuUI == null)
        {
            _isStartedLoad = true;
            _saveGameSubMenuUI = await Load<SaveGameSubMenuUI>(AssetsConstants.SaveGameSubMenu);
            _saveGameSubMenuUI.Init(_gameInput, _hashAnimationName);

            if (�loseDelegate != null)
                _saveGameSubMenuUI.SetCloseDelegate(�loseDelegate);
            _saveGameSubMenuUI.SetUnloadDelegate(SetUnload);

            _saveGameSubMenuUI.ToggleVisible();
        }
    }

    private void SetUnload()
    {
        Unload();
        _saveGameSubMenuUI = null;
        _isStartedLoad = false;
    }
}
