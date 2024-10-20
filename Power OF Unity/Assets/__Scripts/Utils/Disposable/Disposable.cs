using System;

/// <summary>
/// ����������� �������������
/// </summary>
public class Disposable<T> : IDisposable
{
    private static readonly Action<T> EmptyDelegate = _ => { };

    private readonly Action<T> _dispose; // ������� �������
    private bool _isDisposed; //������? 

    /// <summary>
    /// ����������� �������������
    /// </summary>
    ///<remarks>�������� ������ �������, � ������� ��� ������� ����� �������</remarks>
    public Disposable(T value, Action<T> dispose)
    {
        Value = value;
        _dispose = dispose;
    }

    /// <summary>
    /// ������ ������� � ���������� ����� ������
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// ����������
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _dispose(Value); // ������� ������� ������������ � ��������� �� ���� ���� ����������
    }
    /// <summary>
    /// ������
    /// </summary>
    public static Disposable<T> Borrow(T value, Action<T> dispose) => new(value, dispose);
    /// <summary>
    /// ��������� ����
    /// </summary>
    public static Disposable<T> FakeBorrow(T value) => new(value, EmptyDelegate);
}
