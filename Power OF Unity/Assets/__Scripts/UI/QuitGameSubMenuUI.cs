/// <summary>
/// Под меню выхода из игрового меню
/// </summary>
/// <remarks>
/// Создается и инициализируется в каждой сцене
/// </remarks>
public class QuitGameSubMenuUI : ToggleVisibleAnimatioSubscribeMenuUI
{
    private ScenesService _scenesService;

    public void Init(GameInput gameInput, ScenesService scenesService)
    {
        _gameInput = gameInput;
        _scenesService = scenesService;

        Setup();
    }

    private void Setup()
    {
       
    }

    protected override void SetAnimationOpenClose()
    {
      
    }
}
