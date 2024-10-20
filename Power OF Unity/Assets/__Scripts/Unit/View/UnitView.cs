
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал ЮНИТА. Абстрактный класс
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _handLeft;
    [SerializeField] private Transform _handRight;
    [SerializeField] private Animator _animator;

   
    /// <summary>
    /// Настройка брони головы
    /// </summary>
    /// <remarks>!!! Сначала настроить BodyArmor а потом HeadArmor</remarks>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);
    /// <summary>
    /// Нвтройка брони тела
    /// </summary>
    /// <remarks>!!! Сначала настроить BodyArmor а потом HeadArmor</remarks>
    public abstract void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO);
    public abstract void SetMainWeapon(PlacedObjectTypeWithActionSO placedObjectTypeWithActionSO); 
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
