using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// ������ �����. ����������� �����
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _attachPointShieldLeft;
    [SerializeField] private Transform _attachPointSwordRight;
    [SerializeField] private Transform _attachPointGunRight;
    [SerializeField] private Transform _attachPointGrenade;
    [Header("��������� ��������")]
    [SerializeField] private Animator _animator;
    [SerializeField] private RigBuilder _rigBuilder;

    [Header("����� ��� ������")]
    [SerializeField] protected MeshRenderer[] _headArmorMilitaryMeshArray;       // ���������� ������� ����
    [SerializeField] protected MeshRenderer[] _headArmorJunkerMeshArray;         // ��������� ������� ����
    [SerializeField] protected MeshRenderer[] _headArmorSpaceNoFaceMeshArray;    // ����������� ���� ����������� ��� ����
    [SerializeField] protected MeshRenderer[] _headArmorCyberXOMeshArray;        // ����� ���� XO
    [SerializeField] protected MeshRenderer[] _headArmorCyberZenicaMeshArray;    // ����� ���� ������
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceMeshArray;    // ����� ���� ����������� ��� ����
    [SerializeField] protected MeshRenderer[] _headArmorCyberModNoFaceMeshArray; // ����� ���� ���������� ����������� ��� ����
    // ����� �� ����������� ������ � ������ ��� ������� �����, ������� � ����� ������� ���������� � ������� ��������� ������ �����
    [Header("���������� ��� ����� � ������")]
    [SerializeField] protected Transform _hair;
    [SerializeField] protected Transform _beard;

    /// <summary>
    /// ������ ������ ������
    /// </summary>
    protected List<MeshRenderer[]> _headViewList;

    protected MeshRenderer[] _hairMeshArray;                  // ������
    protected MeshRenderer[] _beardMeshArray;                 // ������


    protected void InitMeshRender()
    {
        _hairMeshArray = _hair.GetComponentsInChildren<MeshRenderer>();
        _beardMeshArray = _beard.GetComponentsInChildren<MeshRenderer>();

        _headViewList = new List<MeshRenderer[]>
        {
            _headArmorMilitaryMeshArray,
            _headArmorJunkerMeshArray,
            _headArmorSpaceNoFaceMeshArray,
            _headArmorCyberXOMeshArray,
            _headArmorCyberZenicaMeshArray,
            _headArmorCyberNoFaceMeshArray,
            _headArmorCyberModNoFaceMeshArray,
            _hairMeshArray,
            _beardMeshArray,
        };
    }

    /// <summary>
    /// ��������� ����� ������
    /// </summary>
    public virtual void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
            {
                _hairMeshArray,
                _beardMeshArray,
            });
            return; // ������� � ���������� ��� ����
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

            case HeadArmorType.HeadArmorSpaceNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorSpaceNoFaceMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberXO:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberXOMeshArray,
                    _beardMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberZenica:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberZenicaMeshArray,
                    _beardMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberNoFaceMeshArray,
                });
                break;

            case HeadArmorType.HeadArmorCyberModNoFace:
                SetMeshArrayInHeadViewList(new HashSet<MeshRenderer[]>
                {
                    _headArmorCyberModNoFaceMeshArray,
                });
                break;
        }
    }

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
        SetMeshArrayInEnumerable(showHashList, _headViewList);
    }

    /// <summary>
    /// ��������� ���������� HashSet ������<br/> 
    /// ��������� ���������� ������������ "MeshRenderer[]" � ������ ���, ����� ����������� HashSet ������
    /// </summary>
    protected void SetMeshArrayInEnumerable(HashSet<MeshRenderer[]> showHashList, IEnumerable<MeshRenderer[]> enumerable)
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
    /// �������� ��� ������� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void ShowMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {
            ShowMeshEnumerable(viewHead);
        }
    }
    /// <summary>
    /// ������ ��� ������� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void HideMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {
            HideMeshEnumerable(viewHead);
        }
    }

    /// <summary>
    /// �������� ��� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void ShowMeshEnumerable(IEnumerable<MeshRenderer> enumerable)
    {
        foreach (MeshRenderer view in enumerable)
        {
            view.enabled = true;
        }
    }
    /// <summary>
    /// �������� ��� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void HideMeshEnumerable(IEnumerable<MeshRenderer> enumerable)
    {
        foreach (MeshRenderer view in enumerable)
        {
            view.enabled = false;
        }
    }

    public virtual Animator GetAnimator() { return _animator; }
    public Transform GetAttachPointShield() { return _attachPointShieldLeft; }
    public Transform GetAttachPointSword() { return _attachPointSwordRight; }
    public Transform GetAttachPointGun() { return _attachPointGunRight; }
    public Transform GetAttachPointGrenade() { return _attachPointGrenade; }

    public RigBuilder GetRigBuilder() { return _rigBuilder; }
}
