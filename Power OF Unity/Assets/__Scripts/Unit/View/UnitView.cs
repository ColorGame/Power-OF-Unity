using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// ������ �����
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] protected Transform _attachPointShieldLeft;
    [SerializeField] protected Transform _attachPointSwordRight;
    [SerializeField] protected Transform _attachPointGunRight;
    [SerializeField] protected Transform _attachPointGrenade;
    [Header("��������� ��������")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected RigBuilder _rigBuilder;


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
    /// ��������� ����� ������
    /// </summary>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);


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


    public Animator GetAnimator() { return _animator; }
    public Transform GetAttachPointShield() { return _attachPointShieldLeft; }
    public Transform GetAttachPointSword() { return _attachPointSwordRight; }
    public Transform GetAttachPointGun() { return _attachPointGunRight; }
    public Transform GetAttachPointGrenade() { return _attachPointGrenade; }
    public RigBuilder GetRigBuilder() { return _rigBuilder; }
}
