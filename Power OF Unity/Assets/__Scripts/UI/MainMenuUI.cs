using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour // Главное меню
{
    [SerializeField] private Button _resumeGameButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _loadGameButton;
    [SerializeField] private Button _setupGameButton;
    [SerializeField] private Button _quitGameButton;

    private void Awake()
    {
        

        _resumeGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА"); });
        _loadGameButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА"); });
        _setupGameButton.onClick.AddListener(() => { CoreEntryPoint.Instance.optionsMenuUI.ToggleVisible(); });
        _startGameButton.onClick.AddListener(() => { SceneManager.LoadSceneAsync(SceneName.UnitMenuScene.GetHashCode()); });
        _quitGameButton.onClick.AddListener(() => { Application.Quit(); });// Кнопка будет работать тоько после сборки  

    }
}
