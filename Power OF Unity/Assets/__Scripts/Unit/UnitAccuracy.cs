
/// <summary>
/// Точность стрельбы метания.
/// </summary>
public class UnitAccuracy
{
    /// <summary>
    /// Точность стрельбы метания.
    /// </summary>
    public UnitAccuracy(int accuracy)
    {
        _accuracy = accuracy;
    }

    private int _accuracy;

    public int GetAccuracy() { return _accuracy; }
    public void SetAccuracy(int accuracy) { _accuracy = accuracy; }
}
