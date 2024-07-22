using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour, ISetupForSpawn // ���� ��������� ����� ����������// ����� �� �����
{
    [SerializeField] private Transform _ragdollPrefab; // ������ ��������� �����
    [SerializeField] private Transform _originalRootBone; // ������������ �������� �����(�����) // � ���������� �������� ����� ����� ��� ��������� Root 

    private HealthSystem _healthSystem;
    private UnitActionSystem _unitActionSystem;

    private Unit _keelerUnit; //�������� ����� ������� ����� ��� ����� ������.
    private Unit _unit; // ���� �� ������� ����� ���� ������

    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _healthSystem = _unit.GetHealthSystem();
        _unitActionSystem = _unit.GetUnitActionSystem();

        _healthSystem.OnDead += HealthSystem_OnDead; // ���������� �� ������� ���� (�������� ������� ����� ���� ����)
    }
       

    private void Start()
    {
        _healthSystem.OnDead += HealthSystem_OnDead; // ���������� �� ������� ���� (�������� ������� ����� ���� ����)

        /*ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // ���������� �� ������� ����� ����� �������� (�� OnShoot �� ���� ����������� �.�. �� �� static � ����� ������ �� ������� ������)
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit; // ���������� �� c������ - ����� ����� ���� �����*/
    }

   /* private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        if (e.targetUnit == _unit) // ���� ����� �������� ���� ���� , �� �������� ���� ��� �� ��� �������
        {
            _keelerUnit = e.hittingUnit;
        }
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        if (e.targetUnit == _unit) // ���� ����� �������� ���� ���� , �� �������� ���� ��� �� ��� �������
        {
            _keelerUnit = e.shootingUnit;
        }
    }*/


    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Unit keelerUnit = _unitActionSystem.GetSelectedUnit();// ����� ��� �������� ����
        Transform ragdollTransform = Instantiate(_ragdollPrefab, transform.position, transform.rotation); // �������� ����� �� ������� � ������� �����
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>(); // ������ ��������� UnitRagdoll �� �������

        unitRagdoll.Setup(_unitActionSystem, _originalRootBone, keelerUnit); // � ��������� � ����� Init ��������� ������������ �������� ����� � �������
    }
}
