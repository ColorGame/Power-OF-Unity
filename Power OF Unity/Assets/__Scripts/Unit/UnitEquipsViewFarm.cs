using UnityEngine;

/// <summary>
/// ������� - ��������� ������ ����� ���������� �� ��� ���������� (������� � ������� ������������� ������� � ������ �����). 
/// </summary>
public class UnitEquipsViewFarm
{
    /// <summary>
    /// ������� - ��������� ������ ����� ���������� �� ��� ���������� (������� � ������� ������������� ������� � ������ �����). 
    /// </summary>
    public UnitEquipsViewFarm(Unit unit, HashAnimationName hashAnimationName)
    {
        _unit = unit;
        _hashAnimationName = hashAnimationName;
        Setup();
    }

    private Unit _unit;
    private UnitView _currentUnitView;
    private UnitFriendSO _unitFriendSO;
    private UnitEquipment _unitEquipment;
    private HashAnimationName _hashAnimationName;

    private Transform _parentTransform;
    private Transform _rightHandTransform = null;
    private Transform _leftHandTransform = null;

    private void Setup()
    {
        _unitFriendSO = _unit.GetUnitTypeSO<UnitFriendSO>();
        _unitEquipment = _unit.GetUnitEquipment();

        _unitEquipment.OnChangeMainWeapon += UnitEquipment_OnChangeMainWeapon;
        _unitEquipment.OnChangeHeadArmor += UnitEquipment_OnChangeHeadArmor;
        _unitEquipment.OnChangeBodyArmor += UnitEquipment_OnChangeBodyArmor;
    }

    private void UnitEquipment_OnChangeMainWeapon(object sender, PlacedObjectTypeWithActionSO newMainWeapon)
    {
        _currentUnitView.SetMainWeapon(newMainWeapon);
    }

    /// <summary>
    /// �������� ������ ����� ��� ����� ����� ��� ����
    /// </summary>
    /// <remarks>�������� ��������� �������� ������� ��� ��� ������������� ������� �����</remarks>
    private void UnitEquipment_OnChangeBodyArmor(object sender, BodyArmorTypeSO newBodyArmorTypeSO)
    {
        UnitView newUnitView = _unitFriendSO.GetUnitViewPrefab(newBodyArmorTypeSO);
        if (newUnitView.GetType() == _currentUnitView.GetType())
        {
            _currentUnitView.SetBodyAndHeadArmor(newBodyArmorTypeSO, _unitEquipment.GetHeadArmor());           
        }
        else
        {
            Object.Destroy(_currentUnitView.gameObject);
            _currentUnitView = Object.Instantiate(newUnitView, _parentTransform);             
            _currentUnitView.Init(_unitEquipment, _hashAnimationName);
        }
    }

    private void UnitEquipment_OnChangeHeadArmor(object sender, HeadArmorTypeSO newHeadArmorTypeSO)
    {
        _currentUnitView.SetHeadArmor(newHeadArmorTypeSO);
    }

    /// <summary>
    /// ������� ������ ������ �����, � ���������� �����
    /// </summary>
    public void InstantiateOnlyUnitView(Transform parentTransform)
    {
        _parentTransform = parentTransform;

        UnitView unitViewPrefab = _unitFriendSO.GetUnitViewPrefab(_unitEquipment.GetBodyArmor());        

        _currentUnitView = Object.Instantiate(unitViewPrefab, _parentTransform);
        _currentUnitView.Init(_unitEquipment, _hashAnimationName);
    }

    public void SetupForSpawn()
    {
        // ����������� �� ������� ����� ������� ����� ��� ��������� ���������� rightHandTransform � leftHandTransform �.�. ��� ������ ����� ������ �������
        if (_unit.IsEnemy())
        {
            PlacedObjectTypeWithActionSO mainplacedObjectTypeWithActionSO = _unit.GetUnitTypeSO<UnitEnemySO>().GetMainplacedObjectTypeWithActionSO(); // ������� �������� ������ ��� ���������� �����
                                                                                                                                                      // �������
                                                                                                                                                      //  PlacedObject placedObject = PlacedObject.CreateInWorld(_rightHandTransform.position, mainplacedObjectTypeWithActionSO, _unit.GetTransform(), _unit.Get);
                                                                                                                                                      //   EquipWeapon(placedObject);
        }
    }

    /// <summary>
    ///  ����������� �������, ����� � �������� ���������� ���� ������
    /// </summary>
    public void EquipWeapon(PlacedObject placedObject)
    {
        //�������� �� PlacedObject  SO � �� ���� �� ����� ���� � ������

    }
    /// <summary>
    ///  ����������� ������, ����� � �������� ���������� ���� ������
    /// </summary>
    public void EquipArmor()
    {

    }

    public void FreeHands() // ���������� ����
    {
        /* if (EquipmentSlot.MainWeaponSlot == placedObject.GetActiveGridSystemXY().GetGridSlot())//���� ������ �� ����� ��������� ������
         {
            // ������ ����������
         }*/
    }
}
