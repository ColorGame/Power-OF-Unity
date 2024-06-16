
using TMPro;
using UnityEngine;

public class OptionsMenagerUI : MonoBehaviour
{
    [SerializeField] private Transform _gameEndUI;
    [SerializeField] TextMeshProUGUI _gameEndTextText; // Текст окончания игры

    private UnitManager _unitManager;
    private UnitActionSystem _unitActionSystem;

    public void Init(UnitManager unitManager, UnitActionSystem unitActionSystem)
    {
        _unitManager = unitManager;
        _unitActionSystem = unitActionSystem;
    }


    private void Start()
    {
        _unitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;
        _unitActionSystem.OnGameOver += UnitActionSystem_OnGameOver;

        _gameEndUI.gameObject.SetActive(false);
    }



    private void UnitActionSystem_OnGameOver(object sender, System.EventArgs e)
    {
        _gameEndUI.gameObject.SetActive(true);
        _gameEndTextText.SetText("НАШЫ погибли ЭТО ПЕЧАЛЬНО :(");
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, System.EventArgs e)
    {
        if (_unitManager.GetUnitEnemyList().Count == 0) // проверим список врагов если их число =0
        {
            _gameEndUI.gameObject.SetActive(true);
            _gameEndTextText.SetText("ВСЕ ВРАГИ УНИЧТОЖЕНЫ :)");
        }
    }
}
