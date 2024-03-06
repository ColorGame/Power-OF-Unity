
using TMPro;
using UnityEngine;

public class OptionsMenagerUI : MonoBehaviour
{  
   [SerializeField] private Transform _gameEndUI;
   [SerializeField] TextMeshProUGUI _gameEndTextText; // Текст окончания игры




    private void Start()
    {
        UnitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList;
        UnitActionSystem.Instance.OnGameOver += UnitActionSystem_OnGameOver;
     
        _gameEndUI.gameObject.SetActive(false);
    }

    

    private void UnitActionSystem_OnGameOver(object sender, System.EventArgs e)
    {
        _gameEndUI.gameObject.SetActive(true);
        _gameEndTextText.SetText("НАШЫ погибли ЭТО ПЕЧАЛЬНО :(");
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, System.EventArgs e)
    {
       if(UnitManager.Instance.GetEnemyUnitList().Count ==0) // проверим список врагов если их число =0
        {
            _gameEndUI.gameObject.SetActive(true);
            _gameEndTextText.SetText("ВСЕ ВРАГИ УНИЧТОЖЕНЫ :)");
        }
    }    
}
