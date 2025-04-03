using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
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
        _unitEquipment = unit.GetUnitEquipment();
        _unitEquipsViewFarm = unit.GetUnitEquipsViewFarm();
        Setup();
    }

    readonly Unit _unit;
    readonly HashAnimationName _hashAnimationName;
    readonly UnitEquipment _unitEquipment;
    readonly UnitEquipViewFarm _unitEquipsViewFarm;

    private Animator _animator;
    private bool _skipCurrentChangeWeaponEvent = false; // ���������� ������� ������� ����� ������ (��� ������� ����� ��� ������� ������������ �������, � ���� �� ����� ����� ������� ������ ������ ��� � ����� �����������)

    /// <summary>
    /// �������� ����� ��� ��������� ������
    /// </summary>
    private AnimationMainWeapon _idleAnimationMainWeapon;
    /// <summary>
    /// �������� ����� ��� ��������������� ������
    /// </summary>
    private AnimationOtherWeapon _idleAnimationOtherWeapon;
    /// <summary>
    /// �������� ����� ��� ��������������� ������ � �����
    /// </summary>
    private AnimationOtherWeapon _idleAnimationOtherWeaponWithShield;
    /// <summary>
    /// �������� �������� ��� ��������� ������
    /// </summary>
    private AnimationMainWeapon _holdAnimationMainWeapon;
    /// <summary>
    /// �������� �������� ��� ��������������� ������
    /// </summary>
    private AnimationOtherWeapon _holdAnimationOtherWeapon;
    /// <summary>
    /// �������� �������� ��� ��������������� ������ � �����
    /// </summary>
    private AnimationOtherWeapon _holdAnimationOtherWeaponWithShield;


    private void Setup()
    {
        _unitEquipment.OnChangeMainWeapon += UnitEquipment_OnChangeWeapon;
        _unitEquipment.OnChangeOtherWeapon += UnitEquipment_OnChangeWeapon;
        _unitEquipsViewFarm.OnChangeUnitView += UnitEquipsViewFarm_OnChangeUnitView;

        _idleAnimationMainWeapon = new AnimationMainWeapon(
            rifleHandAnimation: _hashAnimationName.Rifle_Idle,
            pistolHandAnimation: _hashAnimationName.Pistol_Idle,
            swordOneHandAnimation: _hashAnimationName.Sword_Idle_OneHanded,
            swordTwoHandAnimation: _hashAnimationName.Sword_Idle_TwoHanded);

        _idleAnimationOtherWeapon = new AnimationOtherWeapon(
            otherHandUnarmedAnimation: _hashAnimationName.Unarmed,
            pistolHandAnimation: _hashAnimationName.Pistol_Idle,
            swordlOneHandAnimation: _hashAnimationName.Sword_Idle_OneHanded);

        _idleAnimationOtherWeaponWithShield = new AnimationOtherWeapon(
            otherHandUnarmedAnimation: _hashAnimationName.Shield_Idle_OtherHandUnarmed,
            pistolHandAnimation: _hashAnimationName.Shield_Idle_OtherPistol,
            swordlOneHandAnimation: _hashAnimationName.Shield_Idle_OtherSword);

        
    }

    public void SetupOnDestroyAndQuit()
    {
        _unitEquipment.OnChangeMainWeapon -= UnitEquipment_OnChangeWeapon;
        _unitEquipment.OnChangeOtherWeapon -= UnitEquipment_OnChangeWeapon;
        _unitEquipsViewFarm.OnChangeUnitView -= UnitEquipsViewFarm_OnChangeUnitView;
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

        GameObject attachMainShootingWeapon = _unitEquipsViewFarm.GetAttachMainShootingWeapon();
        if (attachMainShootingWeapon != null && attachMainShootingWeapon.TryGetComponent(out TargetForAnimationRigging targetForAnimationRigging))
        {
            TwoBoneIKConstraint twoBoneIKConstraint = rigBuilder.GetComponentInChildren<TwoBoneIKConstraint>();
            twoBoneIKConstraint.data.target = targetForAnimationRigging.GetTargetForHandIdleAnimation();
            rigBuilder.enabled = true;
        }


        switch (_unit.GetUnitState())
        {
            case Unit.UnitState.Idle:
                SetIdleAnimation();
                break;
            case Unit.UnitState.Hold:
                SetHoldAnimation();
                break;
        }
    }

    /// <summary>
    /// ���������� �������� �����
    /// </summary>
    private void SetIdleAnimation()
    {
        MainOtherWeapon mainOtherWeapon = _unit.GetUnitEquipment().GetMainOtherWeapon();
        SetAnimationForMainWeapon(mainOtherWeapon, _idleAnimationMainWeapon, _idleAnimationOtherWeapon, _idleAnimationOtherWeaponWithShield);       
    }
    /// <summary>
    /// ���������� �������� ���������
    /// </summary>
    private void SetHoldAnimation()
    {
        MainOtherWeapon mainOtherWeapon = _unit.GetUnitEquipment().GetMainOtherWeapon();
        SetAnimationForMainWeapon(mainOtherWeapon, _holdAnimationMainWeapon, _holdAnimationOtherWeapon,_holdAnimationOtherWeaponWithShield);
    }

    private void SetAnimationForMainWeapon(MainOtherWeapon mainOtherWeapon, AnimationMainWeapon animationMainWeapon , AnimationOtherWeapon animationOtherWeapon, AnimationOtherWeapon animationOtherWeaponWithShield)
    {
        if (mainOtherWeapon.mainWeapon == null)
        {
            SetAnimationForOtherWeapon(mainOtherWeapon.otherWeapon, animationOtherWeapon);
            return;
        }

        switch (mainOtherWeapon.mainWeapon)
        {
            case GrappleTypeSO grappleTypeSO:
                if (grappleTypeSO.GetIsOneHand())
                    StartAnimation(animationMainWeapon.pistolHandAnimation);
                else
                    StartAnimation(animationMainWeapon.rifleHandAnimation);
                break;

            case ShootingWeaponTypeSO shootingWeaponTypeSO:
                if (shootingWeaponTypeSO.GetIsOneHand())
                    StartAnimation(animationMainWeapon.pistolHandAnimation);
                else
                    StartAnimation(animationMainWeapon.rifleHandAnimation);
                break;

            case SwordTypeSO swordTypeSO:
                if (swordTypeSO.GetIsOneHand())
                    StartAnimation(animationMainWeapon.swordOneHandAnimation);
                else
                    StartAnimation(animationMainWeapon.swordTwoHandAnimation);
                break;

            case ShieldItemTypeSO: // ��� ���������� �����, ����� ���������� �������������� ������
                SetAnimationForOtherWeapon(mainOtherWeapon.otherWeapon, animationOtherWeaponWithShield);
                break;
        }
    }

    /// <summary>
    /// ���������� �������� ��� ��������������� ������.<br/>
    /// �� ����� �������� "IsOneHand()" �.�. � ���� ��� ������ ����� ���������� ������ ������ ��� ����� ����
    /// </summary>
    private void SetAnimationForOtherWeapon(PlacedObjectTypeWithActionSO otherWeapon, AnimationOtherWeapon animationOtherWeapon)
    {
        if (otherWeapon == null)
        {
            StartAnimation(animationOtherWeapon.otherHandUnarmedAnimation);
            return;// ������� � ���������� ��� ����   
        }

        switch (otherWeapon)
        {
            case GrappleTypeSO:
            case ShootingWeaponTypeSO shootingWeaponTypeSO:
                StartAnimation(animationOtherWeapon.pistolHandAnimation);
                break;

            case SwordTypeSO:
                StartAnimation(animationOtherWeapon.swordlOneHandAnimation);
                break;

            case SpotterFireItemTypeSO://��� ������� � ������� ��� �������
            case HealItemTypeSO:
                StartAnimation(animationOtherWeapon.otherHandUnarmedAnimation);
                break;
        }
    }

    


    private void StartAnimation(int nameHash, int layer = 0)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != nameHash) // ���� ������ ������������� ������ �������� ��
            _animator.StopPlayback();

        _animator.CrossFade(nameHash, layer);
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

    private struct AnimationMainWeapon
    {
        public int rifleHandAnimation;
        public int pistolHandAnimation;
        public int swordOneHandAnimation;
        public int swordTwoHandAnimation;

        public AnimationMainWeapon(int rifleHandAnimation, int pistolHandAnimation, int swordOneHandAnimation, int swordTwoHandAnimation)
        {
            this.rifleHandAnimation = rifleHandAnimation;
            this.pistolHandAnimation = pistolHandAnimation;
            this.swordOneHandAnimation = swordOneHandAnimation;
            this.swordTwoHandAnimation = swordTwoHandAnimation;
        }
    }

    private struct AnimationOtherWeapon
    {
        public int otherHandUnarmedAnimation;
        public int pistolHandAnimation;
        public int swordlOneHandAnimation;

        public AnimationOtherWeapon(int otherHandUnarmedAnimation, int pistolHandAnimation, int swordlOneHandAnimation)
        {
            this.otherHandUnarmedAnimation = otherHandUnarmedAnimation;
            this.pistolHandAnimation = pistolHandAnimation;
            this.swordlOneHandAnimation = swordlOneHandAnimation;
        }
    }
}
