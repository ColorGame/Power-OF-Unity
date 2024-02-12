

/// <summary>
/// ��������� ����� �����.
/// ���� �������� ������, ������� ������� �� ������������, � ������������ � �������� ��������, ��� ������ ���� ��� �� ��������� MonoBehaviour
/// </summary>
public class BootstrapEntryPoint : PersistentSingleton<BootstrapEntryPoint>
{
    private GameInput _gameInput;

    protected override void Awake()
    {
        base.Awake();

        _gameInput = GameInput.Instance; // ������� ���������
        _gameInput.Initialize(); // �������������� ���� � ������� ��������
    }

   
}
   
