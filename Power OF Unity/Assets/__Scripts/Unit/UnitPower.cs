using UnityEngine;
/// <summary>
/// ���� ����� ��� �������� ����������
/// </summary>
public class UnitPower
{
    /// <summary>
    /// ���� ����� ��� �������� ����������
    /// </summary>
    public UnitPower(int power)
    {
        _power = power;
    }

    private int _power;

    public int GetPower() {  return _power; }
    public void SetPower(int power) { _power = power; } 
}
