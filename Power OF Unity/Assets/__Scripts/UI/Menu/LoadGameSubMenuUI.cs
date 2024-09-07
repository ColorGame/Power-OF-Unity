using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Под меню настроики ЗАГРУЗКИ... 
/// </summary>
/// <remarks>
/// Создается и инициализируется в PersistentEntryPoint
/// </remarks>
public class LoadGameSubMenuUI : ToggleVisibleAnimatioMenuUI
{
    [SerializeField] private Button _loadGameButton;    //Загрузить игру
    [SerializeField] private Button _deleteSaveButton;  //Удалить сохранение
    [SerializeField] private Button _quitButton;        //Выйти из меню
    public void Init(GameInput gameInput, HashAnimationName hashAnimationName)
    {
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;

        Setup();
        SetAnimationOpenClose();
    }

    private void Setup()
    {
        _loadGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА_ЗАГРУЗИТЬ"); });
        _deleteSaveButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА_УДАЛИТЬ"); }); // Сделать дополнительное окно на согласие (ДА/НЕТ)
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.LoadGameSubMenuOpen;
        _animationClose = _hashAnimationName.LoadGameSubMenuClose;
    }
}
