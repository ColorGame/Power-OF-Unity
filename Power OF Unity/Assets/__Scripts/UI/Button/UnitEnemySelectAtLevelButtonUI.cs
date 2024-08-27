using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ - ������ ���������� ����� �� ������� ������.
/// </summary>
public class UnitEnemySelectAtLevelButtonUI : MonoBehaviour // ������ �����
{
    private Button _button; // ���� ������     
    private Unit _enemyUnit;


    public void Init(Unit unit, CameraFollow cameraFollow)
    {
        _enemyUnit = unit;

        _button = GetComponent<Button>();
        // �.�. ������ ��������� ����������� �� � ������� ����������� � ������� � �� � ����������
        //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        _button.onClick.AddListener(() =>
        {
            cameraFollow.SetTargetUnit(_enemyUnit);
        });
    }


}
