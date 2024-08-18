
/// <summary>
/// Точность стрельбы метания.
/// </summary>
public class UnitAccuracySystem
{
    /// <summary>
    /// Точность стрельбы метания.
    /// </summary>
    public UnitAccuracySystem(int accuracy)
    {
        _accuracy = accuracy;
    }

    private int _accuracy;

    public int GetAccuracy() { return _accuracy; }
    public void SetAccuracy(int accuracy) { _accuracy = accuracy; }
}
