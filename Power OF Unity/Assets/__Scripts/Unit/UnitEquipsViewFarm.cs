
using UnityEngine;

/// <summary>
/// ������� - ��������� ������ ����� ���������� �� ��� ���������� (������� � ������� ������������� ������� � ������ �����). 
/// </summary>
public class UnitEquipsViewFarm
{
    /// <summary>
    /// ������� - ��������� ������ ����� ���������� �� ��� ���������� (������� � ������� ������������� ������� � ������ �����). 
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


    private void Setup()
    {
        _unitFriendSO = _unit.GetUnitTypeSO<UnitFriendSO>();
        _unitEquipment = _unit.GetUnitEquipment();

        _unitEquipment.OnChangeMainWeapon += UnitEquipment_OnChangeMainWeapon;
        _unitEquipment.OnChangeOtherWeapon += UnitEquipment_OnChangeOtherWeapon;
        _unitEquipment.OnChangeHeadArmor += UnitEquipment_OnChangeHeadArmor;
        _unitEquipment.OnChangeBodyArmor += UnitEquipment_OnChangeBodyArmor;
    }
    /// <summary>
    /// �������� �������� ������
    /// </summary>
    private void UnitEquipment_OnChangeMainWeapon(object sender, PlacedObjectTypeWithActionSO newMainWeapon)
    {
        _currentUnitView.CleanHands();
        SetMainWeapon(newMainWeapon);
    }

    /// <summary>
    /// �������� �������������� ������
    /// </summary>
    /// <remarks>
    /// �������������� ������ ����� ����������,<br/>
    /// ������ ������ � �����<br/>
    /// ��� ����� ��� ��������� ������ ����.
    /// </remarks>
    private void UnitEquipment_OnChangeOtherWeapon(object sender, PlacedObjectTypeWithActionSO newOtherWeapon)
    {       
        // �������� ��� ����� ����
        _currentUnitView.CleanAttachPointGun();
        _currentUnitView.CleanAttachPointSword();

        PlacedObjectTypeWithActionSO mainWeaponSO = _unitEquipment.GetPlacedObjectMainWeaponSlot();
        if (mainWeaponSO != null && mainWeaponSO is not ShieldItemTypeSO) //���� ���� �������� ������� � ��� �� ��� ��        
            return;// ������� � ���������� ��� ����   

        SetOtherWeapon(newOtherWeapon);
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
    private void SetMainWeapon(PlacedObjectTypeWithActionSO newMainWeapon)
    {
        if (newMainWeapon == null) return;

        switch (newMainWeapon)
        {
            case GrappleTypeSO:
            case ShootingWeaponTypeSO:
                Object.Instantiate(newMainWeapon.GetPrefab3D(), _attachPointGun);

                break;
            case SwordTypeSO:
                Object.Instantiate(newMainWeapon.GetPrefab3D(), _attachPointSword);
                break;

            case ShieldItemTypeSO: // ��� ���������� �����, ����� ���������� �������������� ������
                Object.Instantiate(newMainWeapon.GetPrefab3D(), _attachPointShield);
                SetOtherWeapon(_unitEquipment.GetPlacedObjectOtherWeaponSlot());
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
        _currentUnitView.CleanHands();
        PlacedObjectTypeWithActionSO mainWeaponSO = _unitEquipment.GetPlacedObjectMainWeaponSlot();
        SetMainWeapon(mainWeaponSO);

        if (mainWeaponSO == null)// ���� ��� ��������� ������ �� ������� ��������������  
            SetOtherWeapon(_unitEquipment.GetPlacedObjectOtherWeaponSlot());
    }

}
