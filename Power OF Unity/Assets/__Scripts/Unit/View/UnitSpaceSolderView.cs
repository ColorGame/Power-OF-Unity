using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ����� "����������� ������". 
/// </summary>
/// <remarks>�������� ����� - 1.���������(��� �����) 2.BodyArmorSpace 3.BodyArmorSpaceMod   </remarks>
public class UnitSpaceSolderView : UnitView
{
    [Header("���������� � ������� ����� �����������\n��������� SkinnedMeshRenderer")]
    [SerializeField] private Transform _viewDefault;
    [SerializeField] private Transform _viewBodyArmorSpace;
    [SerializeField] private Transform _viewBodyArmorSpaceMod;
    [SerializeField] private Transform _viewHead;
    [SerializeField] private MeshRenderer _neckMask; // ������ ������� ����������� ���


    private SkinnedMeshRenderer _viewDefaultSkinMesh;               // ��� �����
    private SkinnedMeshRenderer _viewBodyArmorSpaceSkinMesh;        // ����������� ����������
    private SkinnedMeshRenderer _viewBodyArmorSpaceModSkinMesh;     // ����������� ��� ������������ ���������� (����������)
    private SkinnedMeshRenderer _viewHeadSkinMesh;                  // ���� ������, (����������� ����� ���� ��� ������, ������� ����)

    private SkinnedMeshRenderer[] _viewFullBodySkinMeshArray;


    private void Awake()
    {
        _viewDefaultSkinMesh = _viewDefault.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceSkinMesh = _viewBodyArmorSpace.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewBodyArmorSpaceModSkinMesh = _viewBodyArmorSpaceMod.GetComponentInChildren<SkinnedMeshRenderer>();
        _viewHeadSkinMesh = _viewHead.GetComponentInChildren<SkinnedMeshRenderer>();

        _viewFullBodySkinMeshArray = new SkinnedMeshRenderer[]
        {
            _viewDefaultSkinMesh,
            _viewBodyArmorSpaceSkinMesh,
            _viewBodyArmorSpaceModSkinMesh,
            _viewHeadSkinMesh,
        };

        InitMeshRender();
    }


    protected override void SetBodyArmor(BodyArmorTypeSO bodyArmorTypeSO)
    {
        if (bodyArmorTypeSO == null)
        {
            SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewDefaultSkinMesh },
                _viewFullBodySkinMeshArray);
            _neckMask.enabled = false;
            return;
        }

        switch (bodyArmorTypeSO.GetBodyArmorType())
        {
            case BodyArmorType.BodyArmorSpace:
                SetSkinMeshInEnumerable(new HashSet<SkinnedMeshRenderer> { _viewBodyArmorSpaceSkinMesh, _viewHeadSkinMesh },
                    _viewFullBodySkinMeshArray);
                break;

            case BodyArmorType.BodyArmorSpaceMod:
                SetSkinMeshInEnumerable(
                    new HashSet<SkinnedMeshRenderer> { _viewBodyArmorSpaceSkinMesh, _viewBodyArmorSpaceModSkinMesh, _viewHeadSkinMesh },
                    _viewFullBodySkinMeshArray);
                break;
        }
        _neckMask.enabled = true;
    }

}
