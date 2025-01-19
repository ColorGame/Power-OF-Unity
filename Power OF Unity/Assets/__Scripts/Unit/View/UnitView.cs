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
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceModMeshArray; // ����� ���� ���������� ����������� ��� ����
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

    //��������� ������� ����� ������� ����� ���������� (���� ���� �������� ������ ���� MeshRenderer[] �� ������ ������� �� ������������������)
    protected MeshRenderer[][] _withoutHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _militaryHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _junkerHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberXOHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _spaceNoFaceHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberZenicaHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberNoFaceHeadArmorMeshArrayEnumerable;
    protected MeshRenderer[][] _cyberNoFaceModHeadArmorMeshArrayEnumerable;


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
            _headArmorCyberNoFaceModMeshArray,
            _hairMeshArray,
            _beardMeshArray,
        };

        //������� ������������ �������
        _withoutHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _hairMeshArray,
            _beardMeshArray
        };
        _militaryHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorMilitaryMeshArray,
            _beardMeshArray
        };
        _junkerHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
             _headArmorJunkerMeshArray,
             _beardMeshArray
        };
        _cyberXOHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorCyberXOMeshArray,
            _beardMeshArray
        };
        _cyberZenicaHeadArmorMeshArrayEnumerable = new MeshRenderer[][]
        {
            _headArmorCyberZenicaMeshArray,
            _beardMeshArray
        };

    }

    /// <summary>
    /// ��������� ����� ������
    /// </summary>
    public virtual void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(_withoutHeadArmorMeshArrayEnumerable);
            return; // ������� � ���������� ��� ����
        }

        switch (headArmorTypeSO.GetHeadArmorType())
        {
            case HeadArmorType.HeadArmor_1A_Military:
                SetMeshArrayInHeadViewList(_militaryHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_1B_Junker:
                SetMeshArrayInHeadViewList(_junkerHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_2A_CyberXO:
                SetMeshArrayInHeadViewList(_cyberXOHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_2B_SpaceNoFace:
                SetMeshArrayInHeadViewList(_headArmorSpaceNoFaceMeshArray);
                break;
            case HeadArmorType.HeadArmor_3A_1_CyberZenica:
                SetMeshArrayInHeadViewList(_cyberZenicaHeadArmorMeshArrayEnumerable);
                break;
            case HeadArmorType.HeadArmor_3A_2_CyberNoFace:
                SetMeshArrayInHeadViewList(_headArmorCyberNoFaceMeshArray);
                break;
            case HeadArmorType.HeadArmor_3B_CyberNoFaceMod:
                SetMeshArrayInHeadViewList(_headArmorCyberNoFaceModMeshArray);
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
    /// ��������� "_headViewList" � ������ ���, ����� ����� ����������� "showMeshArrayEnumerable"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(IEnumerable<MeshRenderer[]> showMeshArrayEnumerable)
    {
        SetMeshArrayInEnumerable(showMeshArrayEnumerable, _headViewList);
    }
    /// <summary>
    /// ��������� "_headViewList" � ������ ���, ����� ����� ����������� "showMeshArray"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(MeshRenderer[] showMeshArray)
    {
        SetMeshArrayInEnumerable(showMeshArray, _headViewList);
    }

    /// <summary>
    /// ��������� ���������� ������������ "setMeshArrayEnumerable", ������� ���� ���������,<br/> � ������ ���, ����� ����������� "showMeshArrayEnumerable"
    /// </summary>
    protected void SetMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> showMeshArrayEnumerable, IEnumerable<MeshRenderer[]> setMeshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in setMeshArrayEnumerable)
        {
            bool contains = false; // ����������� ��� ������� ��� � ������� ������ / ���� ������ ��� � ������� ������ �� ����������� ����  
            foreach (MeshRenderer[] showMeshArray in showMeshArrayEnumerable)
            {
                if (setMeshArray == showMeshArray)
                {
                    contains = true;
                    break; // ���� ���� ���������� ������� �� �����
                }
            }

            foreach (MeshRenderer mesh in setMeshArray)
            {
                if (mesh != null)
                    mesh.enabled = contains;
            }
        }
    }
    /// <summary>
    /// ��������� ���������� ������������ "setMeshArrayEnumerable", ������� ���� ���������,<br/> � ������ ���, ����� ����������� "showMeshArray"
    /// </summary>
    protected void SetMeshArrayInEnumerable(MeshRenderer[] showMeshArray, IEnumerable<MeshRenderer[]> setMeshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in setMeshArrayEnumerable)
        {
            bool contains = (setMeshArray == showMeshArray);
            foreach (MeshRenderer setMesh in setMeshArray)
            {
                if (setMesh != null)
                    setMesh.enabled = contains;
            }
        }
    }

    /// <summary>
    /// ��������� ���������� ������������ "setSkinEnumerable", ������� ���� ���������,<br/> � ������ ���, ����� ����������� "showSkinEnumerable"
    /// </summary>
    protected void SetSkinMeshInEnumerable(IEnumerable<SkinnedMeshRenderer> showSkinEnumerable, IEnumerable<SkinnedMeshRenderer> setSkinEnumerable)
    {
        foreach (SkinnedMeshRenderer setSkin in setSkinEnumerable)
        {
            bool contains = false; // ����������� ��� ������� ��� � ������� ������ / ���� ������ ��� � ������� ������ �� ����������� ����  
            foreach (SkinnedMeshRenderer showSkin in showSkinEnumerable)
            {
                if (setSkin == showSkin)
                {
                    contains = true;
                    break;// ���� ���� ���������� ������� �� �����
                }
            }
            if (setSkin != null)
                setSkin.enabled = (contains);
        }
    }
    /// <summary>
    /// ��������� ���������� ������������ "setSkinEnumerable", ������� ���� ���������,<br/> � ������ ���, ����� ����������� "showSkin"
    /// </summary>
    protected void SetSkinMeshInEnumerable(SkinnedMeshRenderer showSkin, IEnumerable<SkinnedMeshRenderer> setSkinList)
    {
        foreach (SkinnedMeshRenderer setSkin in setSkinList)
        {
            if (setSkin != null)
                setSkin.enabled = (setSkin == showSkin);
        }
    }

    /// <summary>
    /// �������� ��� ������� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void ShowMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> meshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in meshArrayEnumerable)
        {
            ShowMeshEnumerable(setMeshArray);
        }
    }
    /// <summary>
    /// ������ ��� ������� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void HideMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> meshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in meshArrayEnumerable)
        {
            HideMeshEnumerable(setMeshArray);
        }
    }

    /// <summary>
    /// �������� ��� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void ShowMeshEnumerable(IEnumerable<MeshRenderer> setMeshEnumerable)
    {
        foreach (MeshRenderer setMesh in setMeshEnumerable)
        {
            if (setMesh != null)
                setMesh.enabled = true;
        }
    }
    /// <summary>
    /// ������ ��� MeshRenderer � ���������� ������������
    /// </summary>    
    protected void HideMeshEnumerable(IEnumerable<MeshRenderer> setMeshEnumerable)
    {
        foreach (MeshRenderer setMesh in setMeshEnumerable)
        {
            if (setMesh != null)
                setMesh.enabled = false;
        }
    }

    public virtual Animator GetAnimator() { return _animator; }
    public Transform GetAttachPointShield() { return _attachPointShieldLeft; }
    public Transform GetAttachPointSword() { return _attachPointSwordRight; }
    public Transform GetAttachPointGun() { return _attachPointGunRight; }
    public Transform GetAttachPointGrenade() { return _attachPointGrenade; }

    public RigBuilder GetRigBuilder() { return _rigBuilder; }
}
