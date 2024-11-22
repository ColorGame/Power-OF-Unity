using UnityEngine;
/// <summary>
/// Визуал ЮНИТА. Абстрактный класс
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
    /// Настройка брони головы
    /// </summary>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);
    /// <summary>
    /// Настройка брони тела.<br/>
    /// При смене брони тела, сбрасывается шлем.<br/>
    /// Поэтому после надо вызвать метод "SetHeadArmor()"
    /// </summary>
    protected abstract void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO);
    public void SetMainWeapon(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO) 
    {
        if (placedObjectTypeWithActionSO != null)
        {
            SetAnimatorOverrideController(placedObjectTypeWithActionSO);
        }
        

        // поменять оружие
    }
    /// <summary>
    /// В зависимости от типа оружия переопределим анимации юнита
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
    /// Настройка брони тела и головы.<br/>
    /// При смене брони тела, сбрасывается броня головы,<br/>
    /// Поэтому ее тоже надо обновить.
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
    /// Показать
    /// </summary>
    protected void Show(MeshRenderer[] meshRendererArray)
    {
        foreach (MeshRenderer meshRenderer in meshRendererArray) { meshRenderer.enabled = true; }
    }
    /// <summary>
    /// Скрыть
    /// </summary>
    protected void Hide(MeshRenderer[] meshRendererArray)
    {
        foreach (MeshRenderer meshRenderer in meshRendererArray) { meshRenderer.enabled = false; }
    }
}
