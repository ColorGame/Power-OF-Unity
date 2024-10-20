
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ �����. ����������� �����
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _handLeft;
    [SerializeField] private Transform _handRight;
    [SerializeField] private Animator _animator;

   
    /// <summary>
    /// ��������� ����� ������
    /// </summary>
    /// <remarks>!!! ������� ��������� BodyArmor � ����� HeadArmor</remarks>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);
    /// <summary>
    /// �������� ����� ����
    /// </summary>
    /// <remarks>!!! ������� ��������� BodyArmor � ����� HeadArmor</remarks>
    public abstract void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO);
    public abstract void SetMainWeapon(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO); 
    public virtual Transform GetHandLeft() { return _handLeft; }
    public virtual Transform GetHandRight() { return _handRight; }
    public virtual Animator GetAnimator() { return _animator; }

    /// <summary>
    /// ��������
    /// </summary>
    protected void Show(MeshRenderer[] meshRendererArray)
    {
        foreach (MeshRenderer meshRenderer in meshRendererArray) { meshRenderer.enabled = true; }
    }
    /// <summary>
    /// ������
    /// </summary>
    protected void Hide(MeshRenderer[] meshRendererArray)
    {
        foreach (MeshRenderer meshRenderer in meshRendererArray) { meshRenderer.enabled = false; }
    }
}
