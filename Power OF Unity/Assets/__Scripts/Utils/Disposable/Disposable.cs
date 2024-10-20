using System;

/// <summary>
/// Одноразовое использование
/// </summary>
public class Disposable<T> : IDisposable
{
    private static readonly Action<T> EmptyDelegate = _ => { };

    private readonly Action<T> _dispose; // делегат очистки
    private bool _isDisposed; //Удален? 

    /// <summary>
    /// Одноразовое использование
    /// </summary>
    ///<remarks>Получает объект очистки, и делегат для очистки этого объекта</remarks>
    public Disposable(T value, Action<T> dispose)
    {
        Value = value;
        _dispose = dispose;
    }

    /// <summary>
    /// Объект который в дальнейшем будет очищен
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Избавиться
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _dispose(Value); // вызовим делегат освобождения и передадим от кого надо избавиться
    }
    /// <summary>
    /// Занять
    /// </summary>
    public static Disposable<T> Borrow(T value, Action<T> dispose) => new(value, dispose);
    /// <summary>
    /// Фальшивый заем
    /// </summary>
    public static Disposable<T> FakeBorrow(T value) => new(value, EmptyDelegate);
}
