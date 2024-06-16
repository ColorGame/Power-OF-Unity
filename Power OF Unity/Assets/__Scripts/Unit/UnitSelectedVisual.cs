using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour // ������������ ������ �����
{
    [SerializeField] private Unit _unit; // ���� � �������� ���������� ������ �����.

    private MeshRenderer _meshRenderer; // ����� �������� � ����. MeshRenderer ��� �� ������ ��� �������� ��� ���������� ������

    private UnitActionSystem _unitActionSystem;

    private void Awake() //��� ��������� ������ Awake() ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _unitActionSystem = _unit.GetUnitActionSystem();
    }

    private void Start() // � � ������ StartScene() ������������ ��� ������������� � ��������� ������� ������
    {
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // ������������� �� Event �� UnitActionSystem (���������� �����������). ���������� ��� �� ��������� ������� UnitActionSystem_OnSelectedUnitChanged()
                                                                                                   // ����� ����������� ������ ��� ����� �� ������ ���������� �����.
        UpdateVisual(); // ��� �� ��� ������ ������ ��� ������� ������ � ���������� ������
    }

    // ����� ����� ������� ����� ��� � Event
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) //sender - ����������� // �������� ������ ����� ���� ��������� ��� � ������� ����������� OnSelectedUnitChanged
    {
        UpdateVisual();
    }

    private void UpdateVisual() // (���������� �������) ��������� � ���������� ������������ ������.
    {
        if (_unitActionSystem.GetSelectedUnit() == _unit) // ���� ��������� ���� ����� ����� �� ������� ����� ���� ������ ��
        {
            _meshRenderer.enabled = true; // ������� ����
        }
        else
        {
            _meshRenderer.enabled = false; // �������� ����
        }
    }

    private void OnDestroy() // ���������� ������� � MonoBehaviour � ���������� ��� ����������� �������� �������
    {
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged; // �������� �� ������� ����� �� ���������� ������� � ��������� ��������.(���������� MissingReferenceException: ������ ���� 'MeshRenderer' ��� ���������, �� �� ��� ��� ��������� �������� � ���� ������.)
    }
}
