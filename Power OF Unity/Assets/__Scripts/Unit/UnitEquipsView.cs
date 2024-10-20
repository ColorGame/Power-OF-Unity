using System;
using UnityEngine;

/// <summary>
/// Экипирует визуал юнита предметами из его экипировки (создает и удаляет прикрипленные объекты и меняет броню). 
/// </summary>
public class UnitEquipsView
{
    /// <summary>
    /// Экипирует визуал юнита предметами из его экипировки (создает и удаляет прикрипленные объекты и меняет броню). 
    /// </summary>
    public UnitEquipsView(Unit unit)
    {
        _unit = unit;
        Setup();
    }

    private Unit _unit;
    private UnitView _currentUnitView;
    private UnitFriendSO _unitFriendSO;
    private UnitEquipment _unitEquipment;

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
    /// Изменяет визуал юнита при смене брони для тела
    /// </summary>
    /// <remarks>Изменяет настройки текущего визуала или при необходимости создает новый</remarks>
    private void UnitEquipment_OnChangeBodyArmor(object sender, BodyArmorTypeSO newBodyArmorTypeSO)
    {
        UnitView newUnitView = _unitFriendSO.GetUnitViewPrefab(newBodyArmorTypeSO);
        if (newUnitView.GetType() == _currentUnitView.GetType())
        {
            _currentUnitView.SetBodyArmor(newBodyArmorTypeSO);
            _currentUnitView.SetHeadArmor(_unitEquipment.GetHeadArmor());
        }
        else
        {
            UnityEngine.Object.Destroy(_currentUnitView.gameObject);
            _currentUnitView = UnityEngine.Object.Instantiate(newUnitView, _parentTransform);           
            _currentUnitView.SetBodyArmor(newBodyArmorTypeSO);
            _currentUnitView.SetHeadArmor(_unitEquipment.GetHeadArmor());
        }
    }

    private void UnitEquipment_OnChangeHeadArmor(object sender, HeadArmorTypeSO newHeadArmorTypeSO)
    {
        _currentUnitView.SetHeadArmor(newHeadArmorTypeSO);
    }

    /// <summary>
    /// Создать только визуал юнита, в переданной точке
    /// </summary>
    public void InstantiateOnlyUnitView(Transform parentTransform)
    {
        _parentTransform = parentTransform;

        UnitView unitViewPrefab = _unitFriendSO.GetUnitViewPrefab(_unitEquipment.GetBodyArmor());        

        _currentUnitView = UnityEngine.Object.Instantiate(unitViewPrefab, _parentTransform);
        _currentUnitView.SetBodyArmor(_unitEquipment.GetBodyArmor());
        _currentUnitView.SetHeadArmor(_unitEquipment.GetHeadArmor());
    }






    public void SetupForSpawn()
    {
        // подписаться на событие смена визуала юнита для получения АКТУАЛЬНЫХ rightHandTransform и leftHandTransform т.к. для разной брони разные скелеты
        if (_unit.IsEnemy())
        {
            PlacedObjectTypeWithActionSO mainplacedObjectTypeWithActionSO = _unit.GetUnitTypeSO<UnitEnemySO>().GetMainplacedObjectTypeWithActionSO(); // Получим основное оружие для экиперовки врага
                                                                                                                                                      // Сделать
                                                                                                                                                      //  PlacedObject placedObject = PlacedObject.CreateInWorld(_rightHandTransform.position, mainplacedObjectTypeWithActionSO, _unit.GetTransform(), _unit.Get);
                                                                                                                                                      //   EquipWeapon(placedObject);
        }
    }

    /// <summary>
    ///  Экиперовать ОРУЖИЕМ, юнита к которому прикриплен этот скрипт
    /// </summary>
    public void EquipWeapon(PlacedObject placedObject)
    {
        //Получить из PlacedObject  SO а из него на какую руку и префаб

    }
    /// <summary>
    ///  Экиперовать БРОНЕЙ, юнита к которому прикриплен этот скрипт
    /// </summary>
    public void EquipArmor()
    {

    }

    public void FreeHands() // Освободить руки
    {
        /* if (EquipmentSlot.MainWeaponSlot == placedObject.GetActiveGridSystemXY().GetGridSlot())//Если удален из Сетки Основного Оружия
         {
            // Убрать экипировку
         }*/
    }
}
