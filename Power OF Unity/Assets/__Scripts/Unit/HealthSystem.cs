using System;

/// <summary>
///  ������� �������� 
/// </summary>
/// <remarks>
/// ����� �������������� ��� ����������� ���������
/// </remarks>
public class HealthSystem  
{
    public HealthSystem(int health, SoundManager soundManager)
    {
        _health = health;
        _healthFull = _health;
        _soundManager = soundManager;
    }
    
    private SoundManager _soundManager;

    public event EventHandler OnDead; // ���� (�������� ������� ����� ���� ����)
    public event EventHandler OnDamageAndHealing; // ������� ����������� ��� ��������� (������ ������� ��� ��������� ����� ��� ������� )

    private int _health; // ������� ��������
    private int _healthFull;

    public void SetupForSpawn()
    {
        _health = _healthFull;
    }

    public void Damage(int damageAmount) // ���� (� �������� �������� �������� �����)
    {
        _health -= damageAmount;

        if (_health < 0)
        {
            _health = 0;
        }

        OnDamageAndHealing?.Invoke(this, EventArgs.Empty); // ������� ������� ��� ��������� �����

        if (_health == 0)
        {
            Die();
            _soundManager.PlayOneShot(SoundName.DeathCry);
        }
    }

    public void Healing(int healingAmount) // ��������� (� �������� �������� �������� ����������������� ��������)
    {
        _health += healingAmount;

        if (_health > _healthFull)
        {
            _health = _healthFull;
        }

        OnDamageAndHealing?.Invoke(this, EventArgs.Empty); // ������� ������� ��� �������������� ��������               
    }

    private void Die() // �������� ������� ��� ���������� ����� ������ � �� ����� ����� �������� ��������� ����� ��������
    {
        OnDead?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ���� ���� (�� ��� ����� ������������� Unit, UnitRagdollSpawner)        
    }

    /// <summary>
    /// ������ ��������������� �������� �������� (�� 0 �� 1) ��� ��������� �������� ����� ��������
    /// </summary>
    public float GetHealthNormalized() 
    {
        return (float)_health / _healthFull; // ����������� �� int � float. ����� ���� 10/100 ������ 0
    }

    public int GetHealth() { return _health; }
    public int GetHealthFull() { return _healthFull; }
    public bool IsDead() { return _health == 0; }
}
