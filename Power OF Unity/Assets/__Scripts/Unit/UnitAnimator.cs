using System;
using UnityEngine;
/// <summary>
/// �������� ����� (��� MonoBehaviour)
/// </summary>
public class UnitAnimator 
{ 
    public UnitAnimator(Unit unit, HashAnimationName hashAnimationName)
    {
        _unit = unit;
        _hashAnimationName = hashAnimationName;
        _unitEquipsViewFarm = unit.GetUnitEquipsViewFarm();
        Setup();
    }

    readonly Unit _unit;
    readonly HashAnimationName _hashAnimationName;
    readonly UnitEquipsViewFarm _unitEquipsViewFarm;

    private Animator _animator;

    private void Setup()
    {
        _unitEquipsViewFarm.OnChangeUnitView += UnitEquipsViewFarm_OnChangeUnitView;
    }

    private void UnitEquipsViewFarm_OnChangeUnitView(object sender, UnitView newUnitView)
    {
        _animator = newUnitView.GetAnimator();
    }

    /// <summary>
    /// � ����������� �� ���� ������ ������������� �������� �����
    /// </summary>
    private void SetAnimatorOverrideController(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO)
    {
        AnimatorOverrideController animatorOverrideController = placedObjectTypeWithActionSO.GetAnimatorOverrideController();
        if (animatorOverrideController != null)
        {
            _animator.runtimeAnimatorController = animatorOverrideController;
        }
    }

    /*
        private void Awake()
        {
            if (TryGetComponent<MoveAction>(out MoveAction moveAction)) // ��������� �������� ��������� MoveAction � ���� ���������� �������� � moveAction
            {
                moveAction.OnStartMoving += MoveAction_OnStartMoving; // ���������� �� �������
                moveAction.OnStopMoving += MoveAction_OnStopMoving; // ���������� �� �������
                moveAction.OnChangedFloorsStarted += MoveAction_OnChangedFloorsStarted; // ���������� �� �������
            }
            if (TryGetComponent<ShootAction>(out ShootAction shootAction)) // ��������� �������� ��������� ShootAction � ���� ���������� �������� � shootAction
            {
                shootAction.OnShoot += ShootAction_OnShoot; // ���������� �� �������
            }
            if (TryGetComponent<SwordAction>(out SwordAction swordAction)) // ��������� �������� ��������� SwordAction � ���� ���������� �������� � swordAction
            {
                swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted; // ���������� �� �������
                swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
            }
            if (TryGetComponent<HealAction>(out HealAction healAction)) // ��������� �������� ��������� HealAction � ���� ���������� �������� � healAction
            {
                healAction.OnHealActionStarted += HealAction_OnHealActionStarted;// ���������� �� �������
                healAction.OnHealActionCompleted += HealAction_OnHealActionCompleted;
            }
            if (TryGetComponent<GrenadeFragmentationAction>(out GrenadeFragmentationAction grenadeFragmentationAction))// ��������� �������� ��������� GrenadeAction � ���� ���������� �������� � grenadeAction
            {
                grenadeFragmentationAction.OnGrenadeActionStarted += GrenadeAction_OnGrenadeActionStarted;// ���������� �� �������
                grenadeFragmentationAction.OnGrenadeActionCompleted += GrenadeAction_OnGrenadeActionCompleted;
            }
            if (TryGetComponent<GrenadeSmokeAction>(out GrenadeSmokeAction grenadeSmokeAction))// ��������� �������� ��������� GrenadeAction � ���� ���������� �������� � grenadeAction
            {
                grenadeSmokeAction.OnGrenadeActionStarted += GrenadeAction_OnGrenadeActionStarted;// ���������� �� �������
                grenadeSmokeAction.OnGrenadeActionCompleted += GrenadeAction_OnGrenadeActionCompleted;
            }
            if (TryGetComponent<GrenadeStunAction>(out GrenadeStunAction grenadeStunAction))// ��������� �������� ��������� GrenadeAction � ���� ���������� �������� � grenadeAction
            {
                grenadeStunAction.OnGrenadeActionStarted += GrenadeAction_OnGrenadeActionStarted;// ���������� �� �������
                grenadeStunAction.OnGrenadeActionCompleted += GrenadeAction_OnGrenadeActionCompleted;
            }
            if (TryGetComponent<SpotterFireAction>(out SpotterFireAction spotterFireAction))
            {
                spotterFireAction.OnSpotterFireActionStarted += SpotterFireAction_OnSpotterFireActionStarted;
                spotterFireAction.OnSpotterFireActionCompleted += SpotterFireAction_OnSpotterFireActionCompleted;
            }

        }


        private void SpotterFireAction_OnSpotterFireActionStarted(object sender, EventArgs e)
        {

            _animator.SetBool("IsLooking", true);
        }
        private void SpotterFireAction_OnSpotterFireActionCompleted(object sender, EventArgs e)
        {

            _animator.SetBool("IsLooking", false);
        }

        private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
        {
            if (e.targetGridPosition.floor > e.unitGridPosition.floor) // ���� ������� ������� ���������� ������ ���� ��
            {
                // �������
                _animator.SetTrigger("JumpUp");
            }
            else
            {
                // ������
                _animator.SetTrigger("JumpDown");
            }
        }

        private void GrenadeAction_OnGrenadeActionStarted(object sender, EventArgs e)
        {

            _animator.SetTrigger("Grenady");// ���������� ������( ������ ��������� ������)
        }
        private void GrenadeAction_OnGrenadeActionCompleted(object sender, EventArgs e)
        {

        }

        private void HealAction_OnHealActionStarted(object sender, Unit unit)
        {

            _animator.SetTrigger("Heal");// ���������� ������( ������ ��������� ������)
        }
        private void HealAction_OnHealActionCompleted(object sender, Unit unit)
        {


            //���� �������� � �������� ����� AnimationEvent (HandleAnimationEvents)
            //Instantiate(_healFXPrefab, unit.GetWorldPositionCenter�ornerCell(), Quaternion.LookRotation(Vector3.up)); // �������� ������ ������ ��� ����� �������� �������� (�� ������ � ���������� �������� � ������ Stop Action - Destroy)
        }

        private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
        {


            _animator.SetTrigger("SwordSlash");// ���������� ������( ������ ��������� ������)
        }
        private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e) // ����� �������� ��������� �������� ��� �� ��������
        {

        }

        private void MoveAction_OnStartMoving(object sender, EventArgs empty)
        {
            _animator.SetBool("IsWalking", true); // �������� �������� ������w
        }
        private void MoveAction_OnStopMoving(object sender, EventArgs empty)
        {
            _animator.SetBool("IsWalking", false); // ��������� �������� ������
        }

        private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            _animator.SetTrigger("Shoot"); // ���������� ������( ������ ��������� ������)       
        }
    */


}
