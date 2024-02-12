

/// <summary>
/// Системная точка входа.
/// Один основной объект, который никогда не уничтожается, с подсистемами в качестве дочерних, или внутри если они не наследуют MonoBehaviour
/// </summary>
public class BootstrapEntryPoint : PersistentSingleton<BootstrapEntryPoint>
{
    private GameInput _gameInput;

    protected override void Awake()
    {
        base.Awake();

        _gameInput = GameInput.Instance; // Получим экземпляр
        _gameInput.Initialize(); // Инициализируем поля и сделаем подписки
    }

   
}
   
