/// <summary>
/// ��� ���� ������ �� �������� ����
/// </summary>
/// <remarks>
/// ��������� � ���������������� � ������ �����
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
