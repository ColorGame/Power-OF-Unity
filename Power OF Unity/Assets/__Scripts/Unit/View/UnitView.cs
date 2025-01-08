using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// Визуал ЮНИТА. Абстрактный класс
/// </summary>
public abstract class UnitView : MonoBehaviour
{
    [SerializeField] private Transform _attachPointShieldLeft;
    [SerializeField] private Transform _attachPointSwordRight;
    [SerializeField] private Transform _attachPointGunRight;
    [SerializeField] private Transform _attachPointGrenade;
    [Header("Настроика анимации")]
    [SerializeField] private Animator _animator;
    [SerializeField] private RigBuilder _rigBuilder;

    [Header("Броня для головы")]
    [SerializeField] protected MeshRenderer[] _headArmorMilitaryMeshArray;       // Стандартый военный шлем
    [SerializeField] protected MeshRenderer[] _headArmorJunkerMeshArray;         // Улучшеный военный шлем
    [SerializeField] protected MeshRenderer[] _headArmorSpaceNoFaceMeshArray;    // Космический шлем закрывающий все лицо
    [SerializeField] protected MeshRenderer[] _headArmorCyberXOMeshArray;        // Кибер шлем XO
    [SerializeField] protected MeshRenderer[] _headArmorCyberZenicaMeshArray;    // Кибер шлем Зеница
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceMeshArray;    // Кибер шлем закрывающий все лицо
    [SerializeField] protected MeshRenderer[] _headArmorCyberModNoFaceMeshArray; // Кибер шлем улучшенный закрывающий все лицо
    // Чтобы не настраивать волосы и бороду для каждого юнита, закинем в общем префабе контейнеры в которых храняться нужные вьюхи
    [Header("Контейнеры для волос и бороды")]
    [SerializeField] protected Transform _hair;
    [SerializeField] protected Transform _beard;

    /// <summary>
    /// Список Вьюшек головы
    /// </summary>
    protected List<MeshRenderer[]> _headViewList;

    protected MeshRenderer[] _hairMeshArray;                  // Волосы
    protected MeshRenderer[] _beardMeshArray;                 // Борода


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
    /// Настройка брони головы
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
        SetMeshArrayInEnumerable(showHashList, _headViewList);
    }

    /// <summary>
    /// Настроить переданный HashSet список<br/> 
    /// Переберет переданное перечисление "MeshRenderer[]" и скроет все, кроме переданного HashSet списка
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
    /// Показать все массивы MeshRenderer в переданном перечисление
    /// </summary>    
    protected void ShowMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {
            ShowMeshEnumerable(viewHead);
        }
    }
    /// <summary>
    /// Скрыть все массивы MeshRenderer в переданном перечисление
    /// </summary>    
    protected void HideMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> enumerable)
    {
        foreach (MeshRenderer[] viewHead in enumerable)
        {
            HideMeshEnumerable(viewHead);
        }
    }

    /// <summary>
    /// Показать все MeshRenderer в переданном перечисление
    /// </summary>    
    protected void ShowMeshEnumerable(IEnumerable<MeshRenderer> enumerable)
    {
        foreach (MeshRenderer view in enumerable)
        {
            view.enabled = true;
        }
    }
    /// <summary>
    /// Показать все MeshRenderer в переданном перечисление
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
