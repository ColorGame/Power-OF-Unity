using UnityEngine;
/// <summary>
/// Сила юнита для тоскания снарежения
/// </summary>
public class UnitPower
{
    /// <summary>
    /// Сила юнита для тоскания снарежения
    /// </summary>
    public UnitPower(int power)
    {
        _power = power;
    }

    private int _power;

    public int GetPower() {  return _power; }
    public void SetPower(int power) { _power = power; } 
}
