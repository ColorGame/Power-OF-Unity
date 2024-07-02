using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrenadeProjectile;

public class GrenadeFragmentationAction : GrenadeAction // ���������� ������� 
{
    [SerializeField, Min(0)] private int _grenadeDamage = 36;
    public override void HandleAnimationEvents_OnAnimationTossGrenadeEventStarted(object sender, EventArgs e)  // ������� ����������� ������� - "� �������� "������ �������" ���������� �������"
    {
        if (_unit.GetUnitActionSystem().GetSelectedAction() == this) // �������� ���� �������� �������� (��������) // ��� ���� ������ ��������� �� ������� � ��������, ���� �� ������� �������� �� ���� ������ ��� ������� ������������
        {
            Transform grenadeProjectileTransform = Instantiate(GameAssets.Instance.grenadeProjectilePrefab, _grenadeSpawnTransform.position, Quaternion.identity); // �������� ������ ������� 
            GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>(); // ������� � ������� ��������� GrenadeProjectile

            grenadeProjectile.Init(_targetGridPositin, TypeGrenade.Fragmentation, OnGrenadeBehaviorComplete, _grenadeDamage, _unit.GetSoundManager(),_unit.GetTurnSystem(), _unit.GetLevelGrid()); // � ������� ������� Init() ������� � ��� ������� ������� (��������� ������� ������� ����) ��� ������� � ��������� � ������� ������� OnGrenadeBehaviorComplete ( ��� ������ ������� ����� �������� ��� �������)
        }
    }
    public override EnemyAIAction GetEnemyAIAction(GridPositionXZ gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 45, //�������� �������� ��������. ����� ������� ������� ���� ������ ������� ������� �� �����, 
        };
    }
    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "����������";
    }


    public override string GetToolTip()
    {
        return "���� - " + GetActionPointCost() + "\n" +
            "��������� - " + GetMaxActionDistance() + "\n" +
             "���� - " + GetGrenadeDamage() + "\n" +
            "������ �� ������ ������, ���� �����������";
    }

    public override int GetGrenadeDamage()
    {
        return _grenadeDamage;
    }
}
