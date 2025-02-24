using UnityEngine;
using static UnitEquipment;

/// <summary>
/// ������� - �������(�������) ������ ����� � �������� ��� ����������. 
/// � ������� ����� ���� �������.
/// </summary>
public class UnitEquipViewFarm
{
    /// <summary>
    /// ������� - �������(�������) ������ ����� � �������� ��� ����������. 
    /// </summary>
    public UnitEquipViewFarm(Unit unit)
    {
        _unit = unit;      
        _unitEquipment = _unit.GetUnitEquipment();
        Setup();
    }

    public event System.EventHandler<UnitView> OnChangeUnitView; // �������� ����� ����� 

    readonly Unit _unit;
    
    readonly UnitEquipment _unitEquipment;

    private UnitView _unitView;
    private Transform _coreTransform;

    // ����� ��������� ���������
    private Transform _attachPointShield;
    private Transform _attachPointSword;
    private Transform _attachPointGun;
    private Transform _attachPointGrenade;

    private GameObject _attachMainShootingWeapon;// ������������� �������� ���������� ������

    private bool _skipCurrentChangeWeaponEvent = false; // ���������� ������� ������� ����� ������ (��� ������� ����� ��� ������� ������������ �������, � ���� �� ����� ����� ������� ������ ������ ��� � ����� �����������)

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
    /// �������� �������� ������
    /// </summary>
    private void UnitEquipment_OnChangeMainWeapon(object sender, MainOtherWeapon mainOtherWeapon)
    {
        if (_skipCurrentChangeWeaponEvent) // ���� ���� ���������� ������� ��
        {
            _skipCurrentChangeWeaponEvent = false;//���������� ������� � ���������� ����
            return; // ������� � ���������� ��� ����
        }
        SetMainWeapon(mainOtherWeapon);
    }

    /// <summary>
    /// �������� �������������� ������
    /// </summary>
    /// <remarks>
    /// �������������� ������ ����� ����������,<br/>
    /// ������ ������ � �����<br/>
    /// ��� ����� ��� ��������� ������ ����.
    /// </remarks>
    private void UnitEquipment_OnChangeOtherWeapon(object sender, MainOtherWeapon mainOtherWeapon)
    {
        if (_skipCurrentChangeWeaponEvent) // ���� ���� ���������� ������� ��
        {
            _skipCurrentChangeWeaponEvent = false;//���������� ������� � ���������� ����
            return; // ������� � ���������� ��� ����
        }

        if (mainOtherWeapon.mainWeapon == null || mainOtherWeapon.mainWeapon is ShieldItemTypeSO) //���� ��� ��������� ������ ��� ��� ��� ��        
        {
            // �������� ��� ����� ����           
            _unitView.CleanAttachPointGun();
            _unitView.CleanAttachPointSword();

            SetOtherWeapon(mainOtherWeapon.otherWeapon);
        }
    }

    /// <summary>
    /// �������� ������ ����� ��� ����� ����� ��� ����
    /// </summary>
    /// <remarks>�������� ��������� �������� ������� ��� ��� ������������� ������� �����</remarks>
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
    /// ��������� ��������� ������
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

            case ShieldItemTypeSO: // ��� ���������� �����, ����� ���������� �������������� ������
                Object.Instantiate(mainOtherWeapon.mainWeapon.GetPrefab3D(), _attachPointShield);
                SetOtherWeapon(mainOtherWeapon.otherWeapon);
                break;
        }
    }

    /// <summary>
    /// ��������� �������������� ������<br/>
    /// ��� �������� ��������� ������!!
    /// </summary>
    private void SetOtherWeapon(PlacedObjectTypeWithActionSO newOtherWeapon)
    {
        if (newOtherWeapon == null)
            return;// ������� � ���������� ��� ����   

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
    /// ������� ���� �  ������ �����, � ���������� �����
    /// </summary>
    public Transform CreateCoreAndView(Transform parentTransform)
    {        
        Transform unitCore = Object.Instantiate(_unit.GetUnitTypeSO().GetUnitCorePrefab(), parentTransform); // �������� ���� ����� � ����� ������

        CreateOnlyView(unitCore);

        return unitCore;
    }

    /// <summary>
    /// ������� ������ ������ �����, � ���������� �����
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
    /// ������� ��������� �������, ��������� ������ � �����
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
    /// �������� - ������������� �������� ���������� ������
    /// </summary>
    public GameObject GetAttachMainShootingWeapon() { return _attachMainShootingWeapon; }
}
