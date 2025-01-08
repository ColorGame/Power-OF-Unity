using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using static UnitEquipment;
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
        _unitEquipment = unit.GetUnitEquipment();
        Setup();
    }

    readonly Unit _unit;
    readonly HashAnimationName _hashAnimationName;
    readonly UnitEquipsViewFarm _unitEquipsViewFarm;
    readonly UnitEquipment _unitEquipment;

    private Animator _animator;
    private bool _skipCurrentChangeWeaponEvent = false; // ���������� ������� ������� ����� ������ (��� ������� ����� ��� ������� ������������ �������, � ���� �� ����� ����� ������� ������ ������ ��� � ����� �����������)

    private void Setup()
    {
        _unitEquipsViewFarm.OnChangeUnitView += UnitEquipsViewFarm_OnChangeUnitView;
        _unitEquipment.OnChangeMainWeapon += UnitEquipment_OnChangeWeapon;
        _unitEquipment.OnChangeOtherWeapon += UnitEquipment_OnChangeWeapon;
    }

    public void SetupOnDestroyAndQuit()
    {
        _unitEquipsViewFarm.OnChangeUnitView -= UnitEquipsViewFarm_OnChangeUnitView;
        _unitEquipment.OnChangeMainWeapon -= UnitEquipment_OnChangeWeapon;
        _unitEquipment.OnChangeOtherWeapon -= UnitEquipment_OnChangeWeapon;
    }

    private void UnitEquipsViewFarm_OnChangeUnitView(object sender, UnitView newUnitView)
    {
        _animator = newUnitView.GetAnimator();

        SetAnimationAndRig();

    }

    private void SetAnimationAndRig()
    {
        RigBuilder rigBuilder = _unitEquipsViewFarm.GetCurrentUnitView().GetRigBuilder();
        if (rigBuilder != null)
            rigBuilder.enabled = false;

        Transform attachMainShootingWeapon = _unitEquipsViewFarm.GetAttachMainShootingWeapon();
        if (attachMainShootingWeapon != null && attachMainShootingWeapon.TryGetComponent(out TargetForAnimationRigging targetForAnimationRigging))
        {
            TwoBoneIKConstraint twoBoneIKConstraint = _unitEquipsViewFarm.GetCurrentUnitView().GetRigBuilder().GetComponentInChildren<TwoBoneIKConstraint>();
            twoBoneIKConstraint.data.target = targetForAnimationRigging.GetTargetForHandIdleAnimation();
            rigBuilder.enabled = true;
        }


        switch (_unit.GetUnitState())
        {
            case Unit.UnitState.UnitSetupMenu:
                SetIdleAnimation();
                break;
            case Unit.UnitState.UnitStartLevel:
                break;
        }
    }

    /// <summary>
    /// ���������� �������� �����
    /// </summary>
    private void SetIdleAnimation()
    {
        MainOtherWeapon mainOtherWeapon = _unit.GetUnitEquipment().GetMainOtherWeapon();

        if (mainOtherWeapon.mainWeapon == null)
        {
            SetIdleAnimationForOtherWeapon(mainOtherWeapon.otherWeapon, _hashAnimationName.Idle_Unarmed, _hashAnimationName.Pistol_Idle_GunDown_TwoHanded, _hashAnimationName.Sword_Idle_OneHanded);
            return;
        }

        switch (mainOtherWeapon.mainWeapon)
        {
            case GrappleTypeSO grappleTypeSO:
                if (grappleTypeSO.GetIsOneHand())
                    StartAnimation(_hashAnimationName.Pistol_Idle_GunDown_TwoHanded);
                else
                    StartAnimation(_hashAnimationName.Rifle_Idle_GunDown_TwoHanded);
                break;

            case ShootingWeaponTypeSO shootingWeaponTypeSO:
                if (shootingWeaponTypeSO.GetIsOneHand())
                    StartAnimation(_hashAnimationName.Pistol_Idle_GunDown_TwoHanded);
                else
                    StartAnimation(_hashAnimationName.Rifle_Idle_GunDown_TwoHanded);
                break;

            case SwordTypeSO swordTypeSO:
                if (swordTypeSO.GetIsOneHand())
                    StartAnimation(_hashAnimationName.Sword_Idle_OneHanded);
                else
                    StartAnimation(_hashAnimationName.Sword_Idle_TwoHanded);
                break;

            case ShieldItemTypeSO: // ��� ���������� �����, ����� ���������� �������������� ������
                SetIdleAnimationForOtherWeapon(mainOtherWeapon.otherWeapon, _hashAnimationName.Shield_Idle, _hashAnimationName.Shield_Pistol_Idle, _hashAnimationName.Shield_Sword_Idle);
                break;
        }
    }
    /// <summary>
    /// ���������� �������� ����� ��� ��������������� ������.<br/>
    /// �� ����� �������� "IsOneHand()" �.�. � ���� ��� ������ ����� ���������� ������ ������ ��� ����� ����
    /// </summary>
    private void SetIdleAnimationForOtherWeapon(PlacedObjectTypeWithActionSO otherWeapon, int freeHandAnimation, int pistolHandAnimation, int swordlHandAnimation)
    {
        if (otherWeapon == null)
        {
            StartAnimation(freeHandAnimation);
            return;// ������� � ���������� ��� ����   
        }

        switch (otherWeapon)
        {
            case GrappleTypeSO grappleTypeSO:
            case ShootingWeaponTypeSO shootingWeaponTypeSO:
                StartAnimation(pistolHandAnimation);
                break;

            case SwordTypeSO swordTypeSO:
                StartAnimation(swordlHandAnimation);
                break;
        }
    }


    private void StartAnimation(int nameHash)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != nameHash) // ���� ������ ������������� ������ �������� ��



            _animator.StopPlayback();
        _animator.Play(nameHash, 0);

    }

    private void UnitEquipment_OnChangeWeapon(object sender, MainOtherWeapon mainOtherWeapon)
    {
        if (_skipCurrentChangeWeaponEvent) // ���� ���� ���������� ������� ��
        {
            _skipCurrentChangeWeaponEvent = false;//���������� ������� � ���������� ����
            return; // ������� � ���������� ��� ����
        }

        SetAnimationAndRig();// �������� ���
    }

    public void SetSkipCurrentChangeWeaponEvent(bool skipCurrentEvent) { _skipCurrentChangeWeaponEvent = skipCurrentEvent; }
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
