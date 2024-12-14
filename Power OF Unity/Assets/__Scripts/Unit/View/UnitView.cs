using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ �����. ����������� �����
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _attachPointShieldLeft;
    [SerializeField] private Transform _attachPointSwordRight;
    [SerializeField] private Transform _attachPointGunRight;
    [SerializeField] private Transform _attachPointGrenade;
    [SerializeField] private Animator _animator;

    [Header("���������� � ������� ����� �����������\n��������� MeshRenderer")]
    [SerializeField] protected Transform _hair;
    [SerializeField] protected Transform _headArmorMilitary;
    [SerializeField] protected Transform _headArmorJunker;
    [SerializeField] protected Transform _headArmorCyberNoFace;
    [SerializeField] protected Transform _headArmorCyberZenica;
    [SerializeField] protected Transform _headArmorCyberXO;
    [SerializeField] protected Transform _beard;
    
    /// <summary>
    /// ������ ������ ������
    /// </summary>
    protected List<MeshRenderer[]> _headViewList;

    protected MeshRenderer[] _headArmorMilitaryMeshArray;     // ���������� ������� ����
    protected MeshRenderer[] _headArmorJunkerMeshArray;       // ��������� ������� ����
    protected MeshRenderer[] _headArmorCyberNoFaceMeshArray;  // ����� ���� ����������� ��� ����
    protected MeshRenderer[] _headArmorCyberZenicaMeshArray;  // ����� ���� ������
    protected MeshRenderer[] _headArmorCyberXOMeshArray;      // ����� ���� XO

    protected MeshRenderer[] _hairMeshArray;                  // ������
    protected MeshRenderer[] _beardMeshArray;                 // ������


    protected void InitMeshRender()
    {
        _headArmorMilitaryMeshArray = _headArmorMilitary.GetComponentsInChildren<MeshRenderer>();
        _headArmorJunkerMeshArray = _headArmorJunker.GetComponentsInChildren<MeshRenderer>();
        _headArmorCyberNoFaceMeshArray = _headArmorCyberNoFace.GetComponentsInChildren<MeshRenderer>();
        _headArmorCyberZenicaMeshArray = _headArmorCyberZenica.GetComponentsInChildren<MeshRenderer>();
        _headArmorCyberXOMeshArray = _headArmorCyberXO.GetComponentsInChildren<MeshRenderer>();

        _hairMeshArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshArray = _beard.GetComponentsInChildren<MeshRenderer>();

        _headViewList = new List<MeshRenderer[]>
        {
            _headArmorMilitaryMeshArray,
            _headArmorJunkerMeshArray,
            _headArmorCyberNoFaceMeshArray,
            _headArmorCyberZenicaMeshArray,
            _headArmorCyberXOMeshArray,
            _hairMeshArray,
            _beardMeshArray,
        };
    }

    /// <summary>
    /// ��������� ����� ������
    /// </summary>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);  

    /// <summary>
    /// ��������� ����� ���� � ������.<br/>
    /// ��� ����� ����� ����, ������������ ����� ������,<br/>
    /// ������� �� ���� ���� ��������.
    /// </summary>
    public void SetBodyAndHeadArmor(BodyArmorTypeSO bodyArmorTypeSO, HeadArmorTypeSO headArmorTypeSO)
    {
        SetBodyArmor(bodyArmorTypeSO);
        SetHeadArmor(headArmorTypeSO);
    }
    /// <summary>
    /// ��������� ����� ����.
    /// </summary>
    protected abstract void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO);
    /// <summary>
    /// �������� ���� �� ���������
    /// </summary>
    public void CleanHands()
    {
        CleanAttachPointShield();
        CleanAttachPointSword();
        CleanAttachPointGun();
        CleanAttachPointGrenade();
    }

    public void CleanAttachPointShield()
    {
        ClearTransform(_attachPointShieldLeft);       
    }
    public void CleanAttachPointSword()
    {
        ClearTransform(_attachPointSwordRight);       
    }
    public void CleanAttachPointGun()
    {
        ClearTransform(_attachPointGunRight);       
    }
    public void CleanAttachPointGrenade()
    {
        ClearTransform(_attachPointGrenade);
    }

    private void ClearTransform(Transform transform)
    {
        foreach (Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
    }
       

    /// <summary>
    /// �������� ���������� HashSet ������<br/> 
    /// ��������� "_headViewList" � ������ ���, ����� ����������� HashSet ������
    /// </summary>
    protected void SetMeshArrayInHeadViewList(HashSet<MeshRenderer[]> showHashList)
    {
        SetMeshInEnumerable(showHashList, _headViewList);
    }

    /// <summary>
    /// ��������� ���������� HashSet ������<br/> 
    /// ��������� ���������� ������������ "MeshRenderer[]" � ������ ���, ����� ����������� HashSet ������
    /// </summary>
    protected void SetMeshInEnumerable(HashSet<MeshRenderer[]> showHashList, IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {
            bool contains = showHashList.Contains(viewHead);
            foreach (MeshRenderer view in viewHead)
            {
                view.enabled = contains;
            }
        }
    }
    /// <summary>
    /// ��������� ���������� HashSet ������<br/> 
    /// ��������� ���������� ������������ "SkinnedMeshRenderer" � ������ ���, ����� ����������� HashSet ������
    /// </summary>
    protected void SetSkinMeshInEnumerable(HashSet<SkinnedMeshRenderer> showHashList, IEnumerable<SkinnedMeshRenderer> enumerable)
    {
        foreach (SkinnedMeshRenderer skinMesh in enumerable)
        {
            skinMesh.enabled = showHashList.Contains(skinMesh);          
        }
    }

    /// <summary>
    /// �������� ��� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void ShowMeshInEnumerable(IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {
            foreach (MeshRenderer view in viewHead)
            {
                view.enabled = false;
            }
        }
    }
    /// <summary>
    /// ������ ��� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void HideMeshInEnumerable(IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {           
            foreach (MeshRenderer view in viewHead)
            {
                view.enabled = false;
            }
        }
    }    

    public virtual Animator GetAnimator() { return _animator; }
    public Transform GetAttachPointShield() {return _attachPointShieldLeft; }
    public Transform GetAttachPointSword() {return _attachPointSwordRight; }
    public Transform GetAttachPointGun() {return _attachPointGunRight; }
    public Transform GetAttachPointGrenade() {return _attachPointGrenade; }
}
