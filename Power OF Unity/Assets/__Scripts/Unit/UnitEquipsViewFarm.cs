
using UnityEngine;
using static UnitEquipment;

/// <summary>
/// ������� - �������(�������) ������ ����� � �������� ��� ����������. 
/// </summary>
public class UnitEquipsViewFarm
{
    /// <summary>
    /// ������� - �������(�������) ������ ����� � �������� ��� ����������. 
    /// </summary>
    public UnitEquipsViewFarm(Unit unit)
    {
        _unit = unit;
        Setup();
    }

    public event System.EventHandler<UnitView> OnChangeUnitView; // �������� ����� ����� 

    readonly Unit _unit;

    private UnitView _currentUnitView;
    private UnitFriendSO _unitFriendSO;
    private UnitEquipment _unitEquipment;

    private Transform _pointSpawnUnit;

    // ����� ��������� ���������
    private Transform _attachPointShield;
    private Transform _attachPointSword;
    private Transform _attachPointGun;
    private Transform _attachPointGrenade;

    private Transform _attachMainShootingWeapon;// ������������� �������� ���������� ������

    private bool _skipCurrentChangeWeaponEvent = false; // ���������� ������� ������� ����� ������ (��� ������� ����� ��� ������� ������������ �������, � ���� �� ����� ����� ������� ������ ������ ��� � ����� �����������)

    private void Setup()
    {
        _unitFriendSO = _unit.GetUnitTypeSO<UnitFriendSO>();
        _unitEquipment = _unit.GetUnitEquipment();

        _unitEquipment.OnChangeMainWeapon += UnitEquipment_OnChangeMainWeapon;
        _unitEquipment.OnChangeOtherWeapon += UnitEquipment_OnChangeOtherWeapon;
        _unitEquipment.OnChangeHeadArmor += UnitEquipment_OnChangeHeadArmor;
        _unitEquipment.OnChangeBodyArmor += UnitEquipment_OnChangeBodyArmor;
    }

    public void SetupOnDestroyAndQuit()
    {
        _unitEquipment.OnChangeMainWeapon -= UnitEquipment_OnChangeMainWeapon;
        _unitEquipment.OnChangeOtherWeapon -= UnitEquipment_OnChangeOtherWeapon;
        _unitEquipment.OnChangeHeadArmor -= UnitEquipment_OnChangeHeadArmor;
        _unitEquipment.OnChangeBodyArmor -= UnitEquipment_OnChangeBodyArmor;
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
            _currentUnitView.CleanAttachPointGun();
            _currentUnitView.CleanAttachPointSword();

            SetOtherWeapon(mainOtherWeapon.otherWeapon);
        }
    }

    /// <summary>
    /// �������� ������ ����� ��� ����� ����� ��� ����
    /// </summary>
    /// <remarks>�������� ��������� �������� ������� ��� ��� ������������� ������� �����</remarks>
    private void UnitEquipment_OnChangeBodyArmor(object sender, BodyArmorTypeSO newBodyArmorTypeSO)
    {
        UnitView newUnitViewPrefab = _unitFriendSO.GetUnitViewPrefab(newBodyArmorTypeSO);
        if (newUnitViewPrefab.GetType() == _currentUnitView.GetType())
        {
            _currentUnitView.SetBodyAndHeadArmor(newBodyArmorTypeSO, _unitEquipment.GetHeadArmor());
        }
        else
        {
            Object.Destroy(_currentUnitView.gameObject);
            InstantiateView(newUnitViewPrefab, _pointSpawnUnit);
        }
    }

    private void UnitEquipment_OnChangeHeadArmor(object sender, HeadArmorTypeSO newHeadArmorTypeSO)
    {
        _currentUnitView.SetHeadArmor(newHeadArmorTypeSO);
    }

    /// <summary>
    /// ��������� ��������� ������
    /// </summary>
    private void SetMainWeapon(MainOtherWeapon mainOtherWeapon)
    {
        _currentUnitView.CleanHands();
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
    /// ������� ������ ������ �����, � ���������� �����
    /// </summary>
    public void InstantiateOnlyUnitView(Transform parentTransform)
    {
        _pointSpawnUnit = parentTransform;
        UnitView unitViewPrefab = _unitFriendSO.GetUnitViewPrefab(_unitEquipment.GetBodyArmor());
        InstantiateView(unitViewPrefab, _pointSpawnUnit);
        _unit.SetUnitState(Unit.UnitState.UnitSetupMenu);
    }

    private void InstantiateView(UnitView newUnitViewPrefab, Transform parentTransform)
    {
        _currentUnitView = Object.Instantiate(newUnitViewPrefab, parentTransform);
        _currentUnitView.SetBodyAndHeadArmor(_unitEquipment.GetBodyArmor(), _unitEquipment.GetHeadArmor());

        _attachPointShield = _currentUnitView.GetAttachPointShield();
        _attachPointSword = _currentUnitView.GetAttachPointSword();
        _attachPointGun = _currentUnitView.GetAttachPointGun();
        _attachPointGrenade = _currentUnitView.GetAttachPointGrenade();

        SetWeaponAtSpawn();

        OnChangeUnitView?.Invoke(this, _currentUnitView);
    }

    private void SetWeaponAtSpawn()
    {
        SetMainWeapon(_unitEquipment.GetMainOtherWeapon());
    }

    public void SetSkipCurrentChangeWeaponEvent(bool skipCurrentEvent) { _skipCurrentChangeWeaponEvent = skipCurrentEvent; }

    public UnitView GetCurrentUnitView() { return _currentUnitView; }
    /// <summary>
    /// �������� - ������������� �������� ���������� ������
    /// </summary>
    public Transform GetAttachMainShootingWeapon() { return _attachMainShootingWeapon; }
}
