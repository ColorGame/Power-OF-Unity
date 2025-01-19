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
    [SerializeField] protected MeshRenderer[] _headArmorCyberNoFaceModMeshArray; // Кибер шлем улучшенный закрывающий все лицо
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

    //Двумерный массивы мешей которые будем показывать (если надо показать только один MeshRenderer[] то массив оставим не инициализированным)
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

        //Запоним хэшированные массивы
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
    /// Настройка брони головы
    /// </summary>
    public virtual void SetHeadArmor(HeadArmorTypeSO headArmorTypeSO)
    {
        if (headArmorTypeSO == null)
        {
            SetMeshArrayInHeadViewList(_withoutHeadArmorMeshArrayEnumerable);
            return; // выходим и игнорируем код ниже
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
    /// Переберет "_headViewList" и скроет все, кроме кроме полученного "showMeshArrayEnumerable"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(IEnumerable<MeshRenderer[]> showMeshArrayEnumerable)
    {
        SetMeshArrayInEnumerable(showMeshArrayEnumerable, _headViewList);
    }
    /// <summary>
    /// Переберет "_headViewList" и скроет все, кроме кроме полученного "showMeshArray"
    /// </summary>
    protected void SetMeshArrayInHeadViewList(MeshRenderer[] showMeshArray)
    {
        SetMeshArrayInEnumerable(showMeshArray, _headViewList);
    }

    /// <summary>
    /// Переберет переданное перечисление "setMeshArrayEnumerable", которые надо настроить,<br/> и скроем все, кроме полученного "showMeshArrayEnumerable"
    /// </summary>
    protected void SetMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> showMeshArrayEnumerable, IEnumerable<MeshRenderer[]> setMeshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in setMeshArrayEnumerable)
        {
            bool contains = false; // предположим что объекта нет в искомом списке / Если найдем его в искомом списке то перезапишем флаг  
            foreach (MeshRenderer[] showMeshArray in showMeshArrayEnumerable)
            {
                if (setMeshArray == showMeshArray)
                {
                    contains = true;
                    break; // Если есть совпадение выходим из цикла
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
    /// Переберет переданное перечисление "setMeshArrayEnumerable", которые надо настроить,<br/> и скроем все, кроме полученного "showMeshArray"
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
    /// Переберет переданное перечисление "setSkinEnumerable", которые надо настроить,<br/> и скроем все, кроме полученного "showSkinEnumerable"
    /// </summary>
    protected void SetSkinMeshInEnumerable(IEnumerable<SkinnedMeshRenderer> showSkinEnumerable, IEnumerable<SkinnedMeshRenderer> setSkinEnumerable)
    {
        foreach (SkinnedMeshRenderer setSkin in setSkinEnumerable)
        {
            bool contains = false; // предположим что объекта нет в искомом списке / Если найдем его в искомом списке то перезапишем флаг  
            foreach (SkinnedMeshRenderer showSkin in showSkinEnumerable)
            {
                if (setSkin == showSkin)
                {
                    contains = true;
                    break;// Если есть совпадение выходим из цикла
                }
            }
            if (setSkin != null)
                setSkin.enabled = (contains);
        }
    }
    /// <summary>
    /// Переберет переданное перечисление "setSkinEnumerable", которые надо настроить,<br/> и скроем все, кроме полученного "showSkin"
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
    /// Показать все массивы MeshRenderer в переданном перечисление
    /// </summary>    
    protected void ShowMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> meshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in meshArrayEnumerable)
        {
            ShowMeshEnumerable(setMeshArray);
        }
    }
    /// <summary>
    /// Скрыть все массивы MeshRenderer в переданном перечисление
    /// </summary>    
    protected void HideMeshArrayInEnumerable(IEnumerable<MeshRenderer[]> meshArrayEnumerable)
    {
        foreach (MeshRenderer[] setMeshArray in meshArrayEnumerable)
        {
            HideMeshEnumerable(setMeshArray);
        }
    }

    /// <summary>
    /// Показать все MeshRenderer в переданном перечисление
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
    /// Скрыть все MeshRenderer в переданном перечисление
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
