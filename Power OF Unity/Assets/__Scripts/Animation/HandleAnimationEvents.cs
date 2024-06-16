using System;
using UnityEngine;
/// <summary>
/// ���������� ������������ ������� � Animation Event, �.�. �������� ����� ��� ��� ������� ����������
/// </summary>
/// <remarks>
/// ���������� ����� � Animator
/// </remarks>
public class HandleAnimationEvents : MonoBehaviour 
{
    public event EventHandler OnAnimationTossGrenadeEventStarted;     // �������� � �������� "������ �������" ���������� �������  (� ���� ������ ����� ���������� �������) // ��� ������������� ������� ����� AnimationEvent � GrenadyAction � ��� ����� ��������� �������

    private Unit _unit;
    private Unit _targetUnit;
    private HealAction _healAction;

    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
    }

    private void Start()
    {       
        if (_unit != null) // ���� ���� ����������
        {
            _healAction = _unit.GetAction<HealAction>();// ��������� �� ����� �������� ��������� HealAction � ���� ���������� �������� � healAction

            _healAction.OnHealActionStarted += HealAction_OnHealActionStarted; // ���������� �� �������           
        }
    }

    private void HealAction_OnHealActionStarted(object sender, Unit unit)
    {
        _targetUnit = unit;
    }


    
    private void InstantiateHealFXPrefab() // ������� � AnimationEvent �� �������� ������� StendUp
    {       
        Instantiate(GameAssets.Instance.healFXPrefab, _targetUnit.GetWorldPosition(), Quaternion.LookRotation(Vector3.up)); // �������� ������ ������ ��� ����� �������� �������� (�� ������ � ���������� �������� � ������ Stop Action - Destroy)
    }

    private void StartIntermediateEvent() // ������� � AnimationEvent �� �������� �������� �������
    {
        OnAnimationTossGrenadeEventStarted?.Invoke(this, EventArgs.Empty); // �������� ������� � �������� "������ �������" ���������� ������� (��������� GrenadyAction)           
    }

    private void OnDestroy()
    {
        if (_healAction != null)
        {
            _healAction.OnHealActionStarted -= HealAction_OnHealActionStarted;// �������� �� ������� ����� �� ���������� ������� � ��������� ��������.
        }
    }


}
