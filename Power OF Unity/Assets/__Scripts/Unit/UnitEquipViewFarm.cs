using UnityEngine;
using static UnitEquipment;

/// <summary>
/// ФАБРИКА - Создает(удаляет) визуал юнита и предметы его экипировки. 
/// У каждого юнита своя фабрика.
/// </summary>
public class UnitEquipViewFarm
{
    /// <summary>
    /// ФАБРИКА - Создает(удаляет) визуал юнита и предметы его экипировки. 
    /// </summary>
    public UnitEquipViewFarm(Unit unit)
    {
        _unit = unit;      
        _unitEquipment = _unit.GetUnitEquipment();
        Setup();
    }

    public event System.EventHandler<UnitView> OnChangeUnitView; // Изменена вьюха юнита 

    readonly Unit _unit;
    
    readonly UnitEquipment _unitEquipment;

    private UnitView _unitView;
    private Transform _coreTransform;

    // Точки крепления предметов
    private Transform _attachPointShield;
    private Transform _attachPointSword;
    private Transform _attachPointGun;
    private Transform _attachPointGrenade;

    private GameObject _attachMainShootingWeapon;// Прикрипленное основное стрелковое оружие

    private bool _skipCurrentChangeWeaponEvent = false; // Пропустить текущее событие смены оружия (При Очистке слота для другого размещаемого объекта, в этом же кадре будет помещен другой объект его и будем настраивать)

    protected virtual void Setup()
    {
        _unitEquipment.OnChangeMainWeapon += UnitEquipment_OnChangeMainWeapon;
        _unitEquipment.OnChangeOtherWeapon += UnitEquipment_OnChangeOtherWeapon;

        if (!_unit.GetIsEnemy())
        {
            _unitEquipment.OnChangeHeadArmor += UnitEquipment_OnChangeHeadArmor;
            _unitEquipment.OnChangeBodyArmor += UnitEquipment_OnChangeBodyArmor;
        }
    }

    public void SetupOnDestroyAndQuit()
    {
        _unitEquipment.OnChangeMainWeapon -= UnitEquipment_OnChangeMainWeapon;
        _unitEquipment.OnChangeOtherWeapon -= UnitEquipment_OnChangeOtherWeapon;

        if (!_unit.GetIsEnemy())
        {
            _unitEquipment.OnChangeHeadArmor -= UnitEquipment_OnChangeHeadArmor;
            _unitEquipment.OnChangeBodyArmor -= UnitEquipment_OnChangeBodyArmor;
        }
    }
    /// <summary>
    /// Изменено основное оружие
    /// </summary>
    private void UnitEquipment_OnChangeMainWeapon(object sender, MainOtherWeapon mainOtherWeapon)
    {
        if (_skipCurrentChangeWeaponEvent) // Если надо пропустить событие то
        {
            _skipCurrentChangeWeaponEvent = false;//пропускаем событие и переключим флаг
            return; // выходим и игнорируем код ниже
        }
        SetMainWeapon(mainOtherWeapon);
    }

    /// <summary>
    /// Изменено дополнительное оружие
    /// </summary>
    /// <remarks>
    /// Дополнительное оружие будем показывать,<br/>
    /// только вместе с щитом<br/>
    /// или когда лот основного оружия пуст.
    /// </remarks>
    private void UnitEquipment_OnChangeOtherWeapon(object sender, MainOtherWeapon mainOtherWeapon)
    {
        if (_skipCurrentChangeWeaponEvent) // Если надо пропустить событие то
        {
            _skipCurrentChangeWeaponEvent = false;//пропускаем событие и переключим флаг
            return; // выходим и игнорируем код ниже
        }

        if (mainOtherWeapon.mainWeapon == null || mainOtherWeapon.mainWeapon is ShieldItemTypeSO) //Если нет основного оружия или это ЩИТ то        
        {
            // Почистим все кроме щита           
            _unitView.CleanAttachPointGun();
            _unitView.CleanAttachPointSword();

            SetOtherWeapon(mainOtherWeapon.otherWeapon);
        }
    }

    /// <summary>
    /// Изменяет визуал юнита при смене брони для тела
    /// </summary>
    /// <remarks>Изменяет настройки текущего визуала или при необходимости создает новый</remarks>
    private void UnitEquipment_OnChangeBodyArmor(object sender, BodyArmorTypeSO newBodyArmorTypeSO)
    {       
        UnitView newUnitViewPrefab = _unit.GetUnitTypeSO<UnitFriendSO>().GetUnitViewPrefab(newBodyArmorTypeSO);

        if (newUnitViewPrefab.GetType() == _unitView.GetType())
        {           
            _unitView.SetBodyAndHeadArmor(newBodyArmorTypeSO, _unitEquipment.GetHeadArmor());
        }
        else
        {
            Object.Destroy(_unitView.gameObject);
            InstantiateViewSetWeaponArmor(newUnitViewPrefab);
        }
    }

    private void UnitEquipment_OnChangeHeadArmor(object sender, HeadArmorTypeSO newHeadArmorTypeSO)
    {
        _unitView.SetHeadArmor(newHeadArmorTypeSO);
    }

    /// <summary>
    /// Настройка основного оружия
    /// </summary>
    private void SetMainWeapon(MainOtherWeapon mainOtherWeapon)
    {
        _unitView.CleanHands();
        _attachMainShootingWeapon = null;

        if (mainOtherWeapon.mainWeapon == null)
        {
            SetOtherWeapon(mainOtherWeapon.otherWeapon);
            return;
        }

        switch (mainOtherWeapon.mainWeapon)
        {
            case GrappleTypeSO:
            case ShootingWeaponTypeSO:
                _attachMainShootingWeapon = Object.Instantiate(mainOtherWeapon.mainWeapon.GetPrefab3D(), _attachPointGun);
                break;
            case SwordTypeSO:
                Object.Instantiate(mainOtherWeapon.mainWeapon.GetPrefab3D(), _attachPointSword);
                break;

            case ShieldItemTypeSO: // при снарежинии щитом, будем показывать дополнительное оружие
                Object.Instantiate(mainOtherWeapon.mainWeapon.GetPrefab3D(), _attachPointShield);
                SetOtherWeapon(mainOtherWeapon.otherWeapon);
                break;
        }
    }

    /// <summary>
    /// Настроить дополнительное оружие<br/>
    /// БЕЗ ПРОВЕРКИ ОСНОВНОГО ОРУЖИЯ!!
    /// </summary>
    private void SetOtherWeapon(PlacedObjectTypeWithActionSO newOtherWeapon)
    {
        if (newOtherWeapon == null)
            return;// выходим и игнорируем код ниже   

        switch (newOtherWeapon)
        {
            case GrappleTypeSO grappleTypeSO:
                if (grappleTypeSO.GetIsOneHand())
                    Object.Instantiate(grappleTypeSO.GetPrefab3D(), _attachPointGun);
                break;

            case ShootingWeaponTypeSO shootingWeaponTypeSO:
                if (shootingWeaponTypeSO.GetIsOneHand())
                    Object.Instantiate(shootingWeaponTypeSO.GetPrefab3D(), _attachPointGun);
                break;

            case SwordTypeSO swordTypeSO:
                if (swordTypeSO.GetIsOneHand())
                    Object.Instantiate(swordTypeSO.GetPrefab3D(), _attachPointSword);
                break;
        }
    }

    /// <summary>
    /// Создать ЯДРО и  ВИЗУАЛ юнита, в переданной точке
    /// </summary>
    public Transform CreateCoreAndView(Transform parentTransform)
    {        
        Transform unitCore = Object.Instantiate(_unit.GetUnitTypeSO().GetUnitCorePrefab(), parentTransform); // создадим ядро юнита в точке спавна

        CreateOnlyView(unitCore);

        return unitCore;
    }

    /// <summary>
    /// Создать только ВИЗУАЛ юнита, в переданной точке
    /// </summary>
    public void CreateOnlyView(Transform coreTransform)
    {
        _coreTransform = coreTransform;
        UnitView unitViewPrefab = null;
        switch (_unit.GetUnitTypeSO())
        {
            case UnitFriendSO unitFriendSO:
                unitViewPrefab = unitFriendSO.GetUnitViewPrefab(_unitEquipment.GetBodyArmor());
                break;
            case UnitEnemySO unitEnemySO:
                unitViewPrefab = unitEnemySO.GetUnitEnemyVisualPrefab();
                break;
        }

        InstantiateViewSetWeaponArmor(unitViewPrefab);       
    }
    
    /// <summary>
    /// Создать экземпляр визуала, настроить оружие и броню
    /// </summary>
    private void InstantiateViewSetWeaponArmor(UnitView newUnitViewPrefab)
    {
        _unitView = Object.Instantiate(newUnitViewPrefab, _coreTransform);
       
        _unitView.SetBodyAndHeadArmor(_unitEquipment.GetBodyArmor(), _unitEquipment.GetHeadArmor());

        _attachPointShield = _unitView.GetAttachPointShield();
        _attachPointSword = _unitView.GetAttachPointSword();
        _attachPointGun = _unitView.GetAttachPointGun();
        _attachPointGrenade = _unitView.GetAttachPointGrenade();

        SetMainWeapon(_unitEquipment.GetMainOtherWeapon());

        OnChangeUnitView?.Invoke(this, _unitView);
    }

    public void SetSkipCurrentChangeWeaponEvent(bool skipCurrentEvent) { _skipCurrentChangeWeaponEvent = skipCurrentEvent; }

    public UnitView GetCurrentUnitView() { return _unitView; }
    /// <summary>
    /// Получить - Прикрипленное основное стрелковое оружие
    /// </summary>
    public GameObject GetAttachMainShootingWeapon() { return _attachMainShootingWeapon; }
}
