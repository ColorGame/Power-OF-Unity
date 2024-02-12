
using TMPro;
using UnityEngine;

public class OptionsMenagerUI : MonoBehaviour
{
   [SerializeField] private OptionsUI _optionsUI;
   [SerializeField] private Transform _gameEndUI;
   [SerializeField] TextMeshProUGUI _gameEndTextText; // ����� ��������� ����




    private void Start()
    {
        UnitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;
        UnitActionSystem.Instance.OnGameOver += UnitActionSystem_OnGameOver;
        GameInput.Instance.OnMenuAlternate += GameInput_OnMenuAlternate;
        _gameEndUI.gameObject.SetActive(false);
    }

    private void GameInput_OnMenuAlternate(object sender, System.EventArgs e)
    {
        _optionsUI.ToggleVisible();
    }

    private void UnitActionSystem_OnGameOver(object sender, System.EventArgs e)
    {
        _gameEndUI.gameObject.SetActive(true);
        _gameEndTextText.SetText("���� ������� ��� �������� :(");
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, System.EventArgs e)
    {
       if(UnitManager.Instance.GetEnemyUnitList().Count ==0) // �������� ������ ������ ���� �� ����� =0
        {
            _gameEndUI.gameObject.SetActive(true);
            _gameEndTextText.SetText("��� ����� ���������� :)");
        }
    }    
}
