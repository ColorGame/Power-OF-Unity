using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using static UnitEquipment;
/// <summary>
/// Анимация юнита (без MonoBehaviour)
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
    private bool _skipCurrentChangeWeaponEvent = false; // Пропустить текущее событие смены оружия (При Очистке слота для другого размещаемого объекта, в этом же кадре будет помещен другой объект его и будем настраивать)

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
    /// Установить анимацию покоя
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

            case ShieldItemTypeSO: // при снарежинии щитом, будем показывать дополнительное оружие
                SetIdleAnimationForOtherWeapon(mainOtherWeapon.otherWeapon, _hashAnimationName.Shield_Idle, _hashAnimationName.Shield_Pistol_Idle, _hashAnimationName.Shield_Sword_Idle);
                break;
        }
    }
    /// <summary>
    /// Установить анимацию покоя для ДОПОЛНИТЕЛЬНОГО ОРУЖИЯ.<br/>
    /// Не делаю проверку "IsOneHand()" т.к. в слот доп оружия можно установить только оружие для одной руки
    /// </summary>
    private void SetIdleAnimationForOtherWeapon(PlacedObjectTypeWithActionSO otherWeapon, int freeHandAnimation, int pistolHandAnimation, int swordlHandAnimation)
    {
        if (otherWeapon == null)
        {
            StartAnimation(freeHandAnimation);
            return;// выходим и игнорируем код ниже   
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
        if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != nameHash) // Если сейчас проигрывается другая анимация то



            _animator.StopPlayback();
        _animator.Play(nameHash, 0);

    }

    private void UnitEquipment_OnChangeWeapon(object sender, MainOtherWeapon mainOtherWeapon)
    {
        if (_skipCurrentChangeWeaponEvent) // Если надо пропустить событие то
        {
            _skipCurrentChangeWeaponEvent = false;//пропускаем событие и переключим флаг
            return; // выходим и игнорируем код ниже
        }

        SetAnimationAndRig();// выполним его
    }

    public void SetSkipCurrentChangeWeaponEvent(bool skipCurrentEvent) { _skipCurrentChangeWeaponEvent = skipCurrentEvent; }
    /// <summary>
    /// В зависимости от типа оружия переопределим анимации юнита
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
            if (TryGetComponent<MoveAction>(out MoveAction moveAction)) // Попробуем получить компонент MoveAction и если получиться сохраним в moveAction
            {
                moveAction.OnStartMoving += MoveAction_OnStartMoving; // Подпишемся на событие
                moveAction.OnStopMoving += MoveAction_OnStopMoving; // Подпишемся на событие
                moveAction.OnChangedFloorsStarted += MoveAction_OnChangedFloorsStarted; // Подпишемся на событие
            }
            if (TryGetComponent<ShootAction>(out ShootAction shootAction)) // Попробуем получить компонент ShootAction и если получиться сохраним в shootAction
            {
                shootAction.OnShoot += ShootAction_OnShoot; // Подпишемся на событие
            }
            if (TryGetComponent<SwordAction>(out SwordAction swordAction)) // Попробуем получить компонент SwordAction и если получиться сохраним в swordAction
            {
                swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted; // Подпишемся на событие
                swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
            }
            if (TryGetComponent<HealAction>(out HealAction healAction)) // Попробуем получить компонент HealAction и если получиться сохраним в healAction
            {
                healAction.OnHealActionStarted += HealAction_OnHealActionStarted;// Подпишемся на событие
                healAction.OnHealActionCompleted += HealAction_OnHealActionCompleted;
            }
            if (TryGetComponent<GrenadeFragmentationAction>(out GrenadeFragmentationAction grenadeFragmentationAction))// Попробуем получить компонент GrenadeAction и если получиться сохраним в grenadeAction
            {
                grenadeFragmentationAction.OnGrenadeActionStarted += GrenadeAction_OnGrenadeActionStarted;// Подпишемся на событие
                grenadeFragmentationAction.OnGrenadeActionCompleted += GrenadeAction_OnGrenadeActionCompleted;
            }
            if (TryGetComponent<GrenadeSmokeAction>(out GrenadeSmokeAction grenadeSmokeAction))// Попробуем получить компонент GrenadeAction и если получиться сохраним в grenadeAction
            {
                grenadeSmokeAction.OnGrenadeActionStarted += GrenadeAction_OnGrenadeActionStarted;// Подпишемся на событие
                grenadeSmokeAction.OnGrenadeActionCompleted += GrenadeAction_OnGrenadeActionCompleted;
            }
            if (TryGetComponent<GrenadeStunAction>(out GrenadeStunAction grenadeStunAction))// Попробуем получить компонент GrenadeAction и если получиться сохраним в grenadeAction
            {
                grenadeStunAction.OnGrenadeActionStarted += GrenadeAction_OnGrenadeActionStarted;// Подпишемся на событие
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
            if (e.targetGridPosition.floor > e.unitGridPosition.floor) // Если целевая позиция находиться этажом выше то
            {
                // Прыгать
                _animator.SetTrigger("JumpUp");
            }
            else
            {
                // Падать
                _animator.SetTrigger("JumpDown");
            }
        }

        private void GrenadeAction_OnGrenadeActionStarted(object sender, EventArgs e)
        {

            _animator.SetTrigger("Grenady");// Установить тригер( нажать спусковой крючок)
        }
        private void GrenadeAction_OnGrenadeActionCompleted(object sender, EventArgs e)
        {

        }

        private void HealAction_OnHealActionStarted(object sender, Unit unit)
        {

            _animator.SetTrigger("Heal");// Установить тригер( нажать спусковой крючок)
        }
        private void HealAction_OnHealActionCompleted(object sender, Unit unit)
        {


            //Буду создавть в Анимации через AnimationEvent (HandleAnimationEvents)
            //Instantiate(_healFXPrefab, unit.GetWorldPositionCenterСornerCell(), Quaternion.LookRotation(Vector3.up)); // Создадим префаб частиц для юнита которого исцеляем (Не забудь в инспекторе включить у частиц Stop Action - Destroy)
        }

        private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
        {


            _animator.SetTrigger("SwordSlash");// Установить тригер( нажать спусковой крючок)
        }
        private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e) // Когда действие завершено поменяем меч на винтовку
        {

        }

        private void MoveAction_OnStartMoving(object sender, EventArgs empty)
        {
            _animator.SetBool("IsWalking", true); // Включить анимацию Хотьбыw
        }
        private void MoveAction_OnStopMoving(object sender, EventArgs empty)
        {
            _animator.SetBool("IsWalking", false); // Выключить анимацию Хотьбы
        }

        private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            _animator.SetTrigger("Shoot"); // Установить тригер( нажать спусковой крючок)       
        }
    */


}
