using System;

/// <summary>
///  ������� �������� 
/// </summary>
/// <remarks>
/// ����� �������������� ��� ����������� ���������
/// </remarks>
public class Health  
{
    public Health(int health, SoundManager soundManager)
    {
        _health = health;
        _healthMax = _health;
        _soundManager = soundManager;
    }
    
    private SoundManager _soundManager;

    public event EventHandler OnDead; // ���� (�������� ������� ����� ���� ����)
    public event EventHandler OnDamageAndHealing; // ������� ����������� ��� ��������� (������ ������� ��� ��������� ����� ��� ������� )

    private int _health; // ������� ��������
    private int _healthMax;

    public void SetupForSpawn()
    {
        _health = _healthMax;
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

        if (_health > _healthMax)
        {
            _health = _healthMax;
        }

        OnDamageAndHealing?.Invoke(this, EventArgs.Empty); // ������� ������� ��� �������������� ��������               
    }

    private void Die() // �������� ������� ��� ���������� ����� ������ � �� ����� ����� �������� ��������� ����� ��������
    {
        OnDead?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ���� ���� (�� ��� ����� ������������� Unit, UnitRagdollSpawner)        
    }

    public float GetHealthNormalized() // ������ ��������������� �������� �������� (�� 0 �� 1) ��� ��������� �������� ����� ��������
    {
        return (float)_health / _healthMax; // ����������� �� int � float. ����� ���� 10/100 ������ 0
    }

    public int GetHealth() { return _health; }
    public int GetHealthMax() { return _healthMax; }
    public bool IsDead() { return _health == 0; }
}
