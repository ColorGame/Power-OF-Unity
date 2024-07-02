using System;
using UnityEngine;
/// <summary>
/// ���������� ������������ �������. �� Animation Event ���������� ����������� ����� ���, ������� ������� ����������� ������� � ������������ �� ���� 
/// </summary>
/// <remarks>
/// ���������� ����� � Animator 
/// </remarks>
public class HandleAnimationEvents : MonoBehaviour 
{
    public event EventHandler OnAnimationTossGrenadeEventStarted;  // � �������� "������ �������" ���������� �������  (� ���� ������ ����� ���������� �������) // ��� ������������� ������� ����� AnimationEvent � GrenadyAction 
    public event EventHandler OnPrayingStendUpEventStarted; // � �������� "��������� ������" ���������� ������� (� ���� ������ ����� ���������� �������� ���������) // ��� ������������� ������� ����� AnimationEvent � HealAction 
    
    
    private void InstantiateHealFXPrefab() // ������� � AnimationEvent �� �������� ������� StendUp
    {
        OnPrayingStendUpEventStarted?.Invoke(this, EventArgs.Empty);
       
    }

    private void StartIntermediateEvent() // ������� � AnimationEvent �� �������� �������� �������
    {
        OnAnimationTossGrenadeEventStarted?.Invoke(this, EventArgs.Empty); // �������� ������� � �������� "������ �������" ���������� ������� (��������� GrenadyAction)           
    }   


}
