
/// <summary>
/// �������� �������� �������.
/// </summary>
public class UnitAccuracy
{
    /// <summary>
    /// �������� �������� �������.
    /// </summary>
    public UnitAccuracy(int accuracy)
    {
        _accuracy = accuracy;
    }

    private int _accuracy;

    public int GetAccuracy() { return _accuracy; }
    public void SetAccuracy(int accuracy) { _accuracy = accuracy; }
}
