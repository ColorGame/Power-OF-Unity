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
    private bool _isStartedLoad = false; // �������� ������?

    /// <summary>
    /// ��������� � ��������� ���������. ��������� �������������� �������, ������� �������, ��� �������� ����
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action �loseDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_loadGameSubMenuUI == null)
        {
            _isStartedLoad = true;
            _loadGameSubMenuUI = await Load<LoadGameSubMenuUI>(AssetsConstants.LoadGameSubMenu);
            _loadGameSubMenuUI.Init(_gameInput, _hashAnimationName);

            if (�loseDelegate != null)
                _loadGameSubMenuUI.SetCloseDelegate(�loseDelegate);
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
