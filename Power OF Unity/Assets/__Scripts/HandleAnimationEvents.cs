using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAnimationEvents : MonoBehaviour // ���������� ������������ �������. 
{
    public event EventHandler OnAnimationTossGrenadeEventStarted;     // �������� � �������� "������ �������" ���������� �������  (� ���� ������ ����� ���������� �������) // ��� ������������� ������� ����� AnimationEvent � GrenadyAction � ��� ����� ��������� �������

    [SerializeField] private Unit _unit;

    private Unit _targetUnit;
    private HealAction _healAction;


    private void Start()
    {
        //Unit unit = GetComponentInParent<Unit>(); // ������� ��������� Unit �� �������� 
        if (_unit != null) // ���� ���� ����������
        {
            _unit.TryGetComponent<HealAction>(out HealAction healAction);// ��������� �� ����� �������� ��������� HealAction � ���� ���������� �������� � healAction
            _healAction = healAction;

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

    private void StartIntermediateEvent() // ����� �������������� �������
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
