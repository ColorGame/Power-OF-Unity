using UnityEngine;
/// <summary>
/// ������ �����. ����������� �����
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _handLeft;
    [SerializeField] private Transform _handRight;
    [SerializeField] private Animator _animator;

    private HashAnimationName _hashAnimationName;
    public void Init(UnitEquipment unitEquipment, HashAnimationName hashAnimationName)
    {
        _hashAnimationName = hashAnimationName;
        SetBodyAndHeadArmor(unitEquipment.GetBodyArmor(), unitEquipment.GetHeadArmor());
        SetMainWeapon(unitEquipment.GetPlacedObjectMainWeaponSlot());
      //  _animator.CrossFade(_hashAnimationName.IdleHold,0f);
    }
    /// <summary>
    /// ��������� ����� ������
    /// </summary>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);
    /// <summary>
    /// ��������� ����� ����.<br/>
    /// ��� ����� ����� ����, ������������ ����.<br/>
    /// ������� ����� ���� ������� ����� "SetHeadArmor()"
    /// </summary>
    protected abstract void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO);
    public void SetMainWeapon(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO) 
    {
        if (placedObjectTypeWithActionSO != null)
        {
            SetAnimatorOverrideController(placedObjectTypeWithActionSO);
        }
        

        // �������� ������
    }
    /// <summary>
    /// � ����������� �� ���� ������ ������������� �������� �����
    /// </summary>
    private void SetAnimatorOverrideController(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO)
    {        
        AnimatorOverrideController animatorOverrideController = placedObjectTypeWithActionSO.GetAnimatorOverrideController();
        if (animatorOverrideController != null)
        {
            _animator.runtimeAnimatorController = animatorOverrideController;
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
