
using TMPro;
using UnityEngine;

public class OptionsMenagerUI : MonoBehaviour
{
    [SerializeField] private Transform _gameEndUI;
    [SerializeField] TextMeshProUGUI _gameEndTextText; // ����� ��������� ����

    private UnitManager _unitManager;

    public void Initialize(UnitManager unitManager)
    {
        _unitManager = unitManager;
    }


    private void Start()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;
        UnitActionSystem.Instance.OnGameOver += UnitActionSystem_OnGameOver;

        _gameEndUI.gameObject.SetActive(false);
    }



    private void UnitActionSystem_OnGameOver(object sender, System.EventArgs e)
    {
        _gameEndUI.gameObject.SetActive(true);
        _gameEndTextText.SetText("���� ������� ��� �������� :(");
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, System.EventArgs e)
    {
        if (_unitManager.GetEnemyUnitList().Count == 0) // �������� ������ ������ ���� �� ����� =0
        {
            _gameEndUI.gameObject.SetActive(true);
            _gameEndTextText.SetText("��� ����� ���������� :)");
        }
    }
}
