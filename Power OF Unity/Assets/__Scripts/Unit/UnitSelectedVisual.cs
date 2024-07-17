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
        Unit selectedUnit = _unitActionSystem.GetSelectedUnit();
        UpdateVisual(selectedUnit); // ��� �� ��� ������ ������ ��� ������� ������ � ���������� ������
    }

    // ����� ����� ������� ����� ��� � Event
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, Unit selectedUnit) //sender - ����������� // �������� ������ ����� ���� ��������� ��� � ������� ����������� OnSelectedUnitChanged
    {
        UpdateVisual(selectedUnit);
    }

    private void UpdateVisual(Unit selectedUnit) // (���������� �������) ��������� � ���������� ������������ ������.
    {
        _meshRenderer.enabled = (selectedUnit == _unit);// ���� ��������� ���� ����� ����� �� ������� ����� ���� ������ �� ������� ����       
    }

    private void OnDestroy() // ���������� ������� � MonoBehaviour � ���������� ��� ����������� �������� �������
    {
        _unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged; // �������� �� ������� ����� �� ���������� ������� � ��������� ��������.(���������� MissingReferenceException: ������ ���� 'MeshRenderer' ��� ���������, �� �� ��� ��� ��������� �������� � ���� ������.)
    }
}
