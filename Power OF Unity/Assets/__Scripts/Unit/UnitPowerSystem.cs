using UnityEngine;
/// <summary>
/// Сила юнита для тоскания снарежения
/// </summary>
public class UnitPowerSystem
{
    /// <summary>
    /// Сила юнита для тоскания снарежения
    /// </summary>
    public UnitPowerSystem(int power)
    {
        _power = power;
    }

    private int _power;

    public int GetPower() {  return _power; }
    public void SetPower(int power) { _power = power; } 
}
