using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал юнита "Солдат". 
/// </summary>
/// <remarks>сотояние брони - 1.BodyArmorMilitary 2.BodyArmorMilitaryMod 3.BodyArmorCyber</remarks>
public class UnitMilitaryAndCyberSoldier : UnitView
{
    [Header("Контейнеры в которых будем переключать\nвидимость SkinnedMeshRenderer")]
    [SerializeField] private Transform _viewBodyArmorMilitary;
    [SerializeField] private Transform _viewBodyArmorMilitaryMod;
    [SerializeField] private Transform _viewBodyArmorCyber;
    [Header("Контейнеры для крепления дополнительных предметов")]
    [SerializeField] private Transform _attachSpine01;     //Крепление дополнительных карманов для брони
    [SerializeField] private Transform _attachSpine02;     //Крепление богажа
    [SerializeField] private Transform _attachSpine03;     //Крепление рации
    [SerializeField] private Transform _attachNeck;        //Крепление шарфа


    private SkinnedMeshRenderer _viewBodyArmorMilitarySkinMesh;       // Военный бронежилет
    private SkinnedMeshRenderer _viewBodyArmorMilitaryModSkinMesh;    // Улучшенный военный бронежилет 
    private SkinnedMeshRenderer _viewBodyArmorCyberSkinMesh;          // Кибер бронежилет

    private SkinnedMeshRenderer[] _viewBodySkinMeshArray;

    private MeshRenderer[] _attachSpine01MeshArray;
    private MeshRenderer[] _attachSpine02MeshArray;
    private MeshRenderer[] _attachSpine03MeshArray;
    private MeshRenderer[] _attachNeckMeshArray;

    private MeshRenderer[][] _attachViewMeshArray;


    private void Awake()
    {
        _viewBodyArmorMilitarySkinMesh = _viewBodyArmorMilitary.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorMilitaryModSkinMesh = _viewBodyArmorMilitaryMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorCyberSkinMesh = _viewBodyArmorCyber.GetComponentInChildren<SkinnedMeshRenderer>();

        _viewBodySkinMeshArray = new SkinnedMeshRenderer[] 
        {
            _viewBodyArmorMilitarySkinMesh,
            _viewBodyArmorMilitaryModSkinMesh,
            _viewBodyArmorCyberSkinMesh,
        };

        _attachSpine01MeshArray = _attachSpine01.GetComponentsInChildren<MeshRenderer>();
        _attachSpine02MeshArray = _attachSpine02.GetComponentsInChildren<MeshRenderer>();
        _attachSpine03MeshArray = _attachSpine03.GetComponentsInChildren<MeshRenderer>();
        _attachNeckMeshArray = _attachNeck.GetComponentsInChildren<MeshRenderer>();

        _attachViewMeshArray = new MeshRenderer[][]
        {
            _attachSpine01MeshArray,
            _attachSpine02MeshArray,
            _attachSpine03MeshArray,
            _attachNeckMeshArray
        };

        InitMeshRender();
    }

    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        // Этот тип визуала не может принять в аргументе null (null может принять только UnitSpaceSolderView т.к. содержит дефолтное состояние брони)

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmorMilitary:              
                ShowMeshInEnumerable(_attachViewMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorMilitarySkinMesh }, _viewBodySkinMeshArray);                
                break;

            case BodyArmorType.BodyArmorMilitaryMod:                
                ShowMeshInEnumerable(_attachViewMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorMilitaryModSkinMesh }, _viewBodySkinMeshArray);                
                break;

            case BodyArmorType.BodyArmorCyber:               
                HideMeshInEnumerable( _attachViewMeshArray);
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorCyberSkinMesh }, _viewBodySkinMeshArray);                
                break;
        }
    }

    public override void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
            {
                _hairMeshArray,
                _beardMeshArray,
            });
            return; // выходим и игнорируем код ниже
        }

        switch (headArmorTypeSO.GetHeadArmorType())
        {
            case HeadArmorType.HeadArmorMilitary:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorMilitaryMeshArray,
                    _beardMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorJunker:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorJunkerMeshArray,
                    _beardMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberNoFaceMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberZenica:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberZenicaMeshArray,
                    _beardMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberXO:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberXOMeshArray,
                    _beardMeshArray,
                });
                break;
        }
    }

}
