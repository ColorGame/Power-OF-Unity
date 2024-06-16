using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ������ ���������� �����
/// </summary>
public class SelectUnitEnemyButtonUI : MonoBehaviour // ������ �����
{
    public static event EventHandler<Unit> OnAnyEnemylyUnitButtonPressed; // ������ ����� ������ �������������� ����� // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
                                                                          // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 
    [SerializeField] private Button _button; // ���� ������      

    private Unit _enemyUnit;


    public void Init(Unit unit)
    {
        _enemyUnit = unit;

        // �.�. ������ ��������� ����������� �� � ������� ����������� � ������� � �� � ����������
        //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        _button.onClick.AddListener(() =>
        {
            OnAnyEnemylyUnitButtonPressed?.Invoke(this, _enemyUnit); // �������� ������� � ��������� ����������� �����    
        });
    }


}
