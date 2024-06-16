using System;
using UnityEngine;

public class ScreenChromaticAberrationActions : MonoBehaviour //����������� ����� ������� ��������� ������������� ��������� ������ (��������� �����) 
{
    private void Start()
    {       
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // ���������� ����� ����� ��������
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;// ���������� ����� ������� ����������
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;// ���������� ����� ����� ���� �����
        GrappleAction.OnAnyUnitStunComboAction += ComboAction_OnAnyUnitStunComboAction;

    }

    private void ComboAction_OnAnyUnitStunComboAction(object sender, EventArgs e)
    {
        ScreenChromaticAberration.Instance.SetWeight(1f);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenChromaticAberration.Instance.SetWeight(1f);
    }
    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, GrenadeProjectile.TypeGrenade typeGrenade)
    {
        if (typeGrenade != GrenadeProjectile.TypeGrenade.Smoke) // ���� �� ������� �� 
        {
            ScreenChromaticAberration.Instance.SetWeight(1f, 0.5f);
        }
    }
    private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        ScreenChromaticAberration.Instance.SetWeight(1f);
    }




}
