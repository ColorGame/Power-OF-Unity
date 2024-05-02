using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitType")] // ��� �������� ����������� ScriptableObject. � ����������� ���� �� ������� create ��������� ������� ScriptableObjects ������ ����� UnitType(��� ������)
public class UnitTypeSO : ScriptableObject //��� ����� - ��� ��������� ������
{
    [SerializeField] private Transform _unitPortfolioVisualPrefab;
    [SerializeField] private Transform _unitEasyVisualPrefab;
    [SerializeField] private Transform _unitMediumVisualPrefab;
    [SerializeField] private Transform _unitHardPrefab;

    public string nameString;   // ��� �����
    public int health; // ��������
    public int moveDistance; // ��������� �������� (���������� � ����� �����, �������� ���� � ������ ������)
    public int shootDistance; // ��������� ��������

    public Transform GetUnitPortfolioVisualPrefab()
    {
        return _unitPortfolioVisualPrefab;
    }
}