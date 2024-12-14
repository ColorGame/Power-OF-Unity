using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Визуал ЮНИТА. Абстрактный класс
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _attachPointShieldLeft;
    [SerializeField] private Transform _attachPointSwordRight;
    [SerializeField] private Transform _attachPointGunRight;
    [SerializeField] private Transform _attachPointGrenade;
    [SerializeField] private Animator _animator;

    [Header("Контейнеры в которых будем переключать\nвидимость MeshRenderer")]
    [SerializeField] protected Transform _hair;
    [SerializeField] protected Transform _headArmorMilitary;
    [SerializeField] protected Transform _headArmorJunker;
    [SerializeField] protected Transform _headArmorCyberNoFace;
    [SerializeField] protected Transform _headArmorCyberZenica;
    [SerializeField] protected Transform _headArmorCyberXO;
    [SerializeField] protected Transform _beard;
    
    /// <summary>
    /// Список Вьюшек головы
    /// </summary>
    protected List<MeshRenderer[]> _headViewList;

    protected MeshRenderer[] _headArmorMilitaryMeshArray;     // Стандартый военный шлем
    protected MeshRenderer[] _headArmorJunkerMeshArray;       // Улучшеный военный шлем
    protected MeshRenderer[] _headArmorCyberNoFaceMeshArray;  // Кибер шлем закрывающий все лицо
    protected MeshRenderer[] _headArmorCyberZenicaMeshArray;  // Кибер шлем Зеница
    protected MeshRenderer[] _headArmorCyberXOMeshArray;      // Кибер шлем XO

    protected MeshRenderer[] _hairMeshArray;                  // Волосы
    protected MeshRenderer[] _beardMeshArray;                 // Борода


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
    /// Настройка брони головы
    /// </summary>
    public abstract void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO);  

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
    /// <summary>
    /// Настройка брони тела.
    /// </summary>
    protected abstract void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO);
    /// <summary>
    /// Очистить руки от предметов
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
    /// Натроить переданныи HashSet список<br/> 
    /// Переберет "_headViewList" и скроет все, кроме переданного HashSet списка
    /// </summary>
    protected void SetMeshArrayInHeadViewList(HashSet<MeshRenderer[]> showHashList)
    {
        SetMeshInEnumerable(showHashList, _headViewList);
    }

    /// <summary>
    /// Настроить переданный HashSet список<br/> 
    /// Переберет переданное перечисление "MeshRenderer[]" и скроет все, кроме переданного HashSet списка
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
    /// Настроить переданный HashSet список<br/> 
    /// Переберет переданное перечисление "SkinnedMeshRenderer" и скроет все, кроме переданного HashSet списка
    /// </summary>
    protected void SetSkinMeshInEnumerable(HashSet<SkinnedMeshRenderer> showHashList, IEnumerable<SkinnedMeshRenderer> enumerable)
    {
        foreach (SkinnedMeshRenderer skinMesh in enumerable)
        {
            skinMesh.enabled = showHashList.Contains(skinMesh);          
        }
    }

    /// <summary>
    /// Показать все MeshRenderer в переданном перечисление
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
    /// Скрыть все MeshRenderer в переданном перечисление
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
