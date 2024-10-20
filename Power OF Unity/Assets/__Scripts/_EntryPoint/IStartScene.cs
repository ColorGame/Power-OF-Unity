/// <summary>
/// Запуск сцены (для включения анимации или действия, при старте сцены)
/// </summary>
public interface IStartScene
{
    /// <summary>
    /// Запуск сцены (для включения анимации при старте сцены)
    /// </summary>
    /// <remarks>!!!Должна вызываться только после Inject</remarks>
    void StartScene();
}
