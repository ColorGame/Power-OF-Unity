using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал юнита "Солдат". 
/// </summary>
/// <remarks>сотояние брони - 1.BodyArmor_1A_Military 2.BodyArmor_1B_MilitaryMod 3.BodyArmor_3A_Cyber 4.BodyArmor_3B_CyberMod</remarks>
public class UnitMilitaryAndCyberSoldierView : UnitWithChangeHeadArmorView
{
    // Чтобы не настраивать визуал для каждого юнита, закинем в общем префабе контейнеры в которых храняться нужные вьюхи
    [Header("Контейнер визуала брони для тела")]
    [SerializeField] private Transform _viewBodyArmorMilitary;
    [SerializeField] private Transform _viewBodyArmorMilitaryMod;
    [SerializeField] private Transform _viewBodyArmorCyber;
    [SerializeField] private Transform _viewBodyArmorCyberMod;
    [Header("Список доп. предметов для Military")]
    [SerializeField] private MeshRenderer[] _attachMeshMilitaryArray;
    [Header("Список доп. предметов для CuberMod")]
    [SerializeField] private MeshRenderer[] _attachMeshCuberModArray;

    private SkinnedMeshRenderer _viewBodyArmorMilitarySkinMesh;       // Военный бронежилет
    private SkinnedMeshRenderer _viewBodyArmorMilitaryModSkinMesh;    // Военный бронежилет улучшенный
    private SkinnedMeshRenderer _viewBodyArmorCyberSkinMesh;          // Кибер бронежилет
    private SkinnedMeshRenderer _viewBodyArmorCyberModSkinMesh;       // Кибер бронежилет улучшенный


    private MeshRenderer[][] _attachFullMeshArray;
    private SkinnedMeshRenderer[] _viewFullBodySkinMeshArray;


    private MeshRenderer[][] _showMilitaryMeshArrayEnumerable;

    private void Awake()
    {
        _viewBodyArmorMilitarySkinMesh = _viewBodyArmorMilitary.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorMilitaryModSkinMesh = _viewBodyArmorMilitaryMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorCyberSkinMesh = _viewBodyArmorCyber.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorCyberModSkinMesh = _viewBodyArmorCyberMod.GetComponentInChildren<SkinnedMeshRenderer>();

        _viewFullBodySkinMeshArray = new SkinnedMeshRenderer[]
        {
            _viewBodyArmorMilitarySkinMesh,
            _viewBodyArmorMilitaryModSkinMesh,
            _viewBodyArmorCyberSkinMesh,
            _viewBodyArmorCyberModSkinMesh,
        };

        _attachFullMeshArray = new MeshRenderer[][]
        {
            _attachMeshMilitaryArray,
            _attachMeshCuberModArray,
        };

        InitMeshRender();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // Этот тип визуала не может принять в аргументе null (null может принять только UnitSpaceSoldierView т.к. содержит дефолтное состояние брони)

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmor_1A_Military:
                SetMeshArrayInEnumerable(_attachMeshMilitaryArray, _attachFullMeshArray);
                SetSkinMeshInEnumerable(_viewBodyArmorMilitarySkinMesh, _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmor_1B_MilitaryMod:
                SetMeshArrayInEnumerable(_attachMeshMilitaryArray, _attachFullMeshArray);
                SetSkinMeshInEnumerable(_viewBodyArmorMilitaryModSkinMesh, _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmor_3A_Cyber:
                HideMeshArrayInEnumerable(_attachFullMeshArray);
                SetSkinMeshInEnumerable(_viewBodyArmorCyberSkinMesh, _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmor_3B_CyberMod:
                SetMeshArrayInEnumerable(_attachMeshCuberModArray, _attachFullMeshArray);
                SetSkinMeshInEnumerable(_viewBodyArmorCyberModSkinMesh, _viewFullBodySkinMeshArray);
                break;
        }
    }


}
