using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class OptionsSubMenuUIProvider : LocalAssetLoader
{
    public OptionsSubMenuUIProvider(GameInput gameInput, SoundManager soundManager, MusicManager musicManager, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _soundManager = soundManager;
        _musicManager = musicManager;
        _hashAnimationName = hashAnimationName;

    }
    private GameInput _gameInput;
    private SoundManager _soundManager;
    private MusicManager _musicManager;
    private HashAnimationName _hashAnimationName;

    private OptionsSubMenuUI _optionsSubMenuUI;
    private bool _isStartedLoad = false; // �������� ������?(��������� ������� ������� �� ������ � ��������� �����)

    /// <summary>
    /// ��������� � ��������� ���������. ��������� �������������� �������, ������� �������, ��� �������� ����
    /// </summary>
    public async UniTask LoadAndToggleVisible(Action �loseDelegate = null)
    {
        if (_isStartedLoad == true)
            return;

        if (_optionsSubMenuUI == null)
        {
            _isStartedLoad = true;
            _optionsSubMenuUI = await Load<OptionsSubMenuUI>(AssetsConstants.OptionsSubMenu);
            _optionsSubMenuUI.Init(_gameInput, _soundManager, _musicManager, _hashAnimationName);

            if (�loseDelegate != null)
                _optionsSubMenuUI.SetCloseDelegate(�loseDelegate);
            _optionsSubMenuUI.SetUnloadDelegate(SetUnload);

            _optionsSubMenuUI.ToggleVisible();
        }       
    }

    private void SetUnload()
    {
        Unload();
        _optionsSubMenuUI = null;
        _isStartedLoad = false;
    }

}
