using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour, ISetupForSpawn // ������������ ������ �����
{
    private Unit _unit; // ���� � �������� ���������� ������ �����.
    private MeshRenderer _meshRenderer; // ����� �������� � ����. MeshRenderer ��� �� ������ ��� �������� ��� ���������� ������
    private UnitActionSystem _unitActionSystem;

    private void Awake() //��� ��������� ������ Awake() ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _unitActionSystem = _unit.GetUnitActionSystem();

        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // ������������� �� Event �� UnitActionSystem (���������� �����������). ���������� ��� �� ��������� ������� UnitActionSystem_OnSelectedUnitChanged()
                                                                                           // ����� ����������� ������ ��� ����� �� ������ ���������� �����.
        Unit selectedUnit = _unitActionSystem.GetSelectedUnit();
        UpdateVisual(selectedUnit); // ��� �� ��� ������ ������ ��� ������� ������ � ���������� ������
    }
    
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
