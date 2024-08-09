using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Верхняя панель меню в сцене нстройки юнита
/// </summary>
public class UpperMenuBarOnUnitSetupUI : MonoBehaviour
{
    [Header("Кнопки включения игрового меню")]
    [SerializeField] private Button _gameMenuButton;
    [Header("Текст количества монет")]
    [SerializeField] private TextMeshProUGUI _coinCountText;

    private GameInput _gameInput;
    private GameMenuUI _gameMenuUI;

    public void Init(GameInput gameInput, GameMenuUI gameMenuUI)
    {
        _gameInput = gameInput;
        _gameMenuUI = gameMenuUI;

        Setup();
    }

    private void Setup()
    {
        _gameMenuButton.onClick.AddListener(() => { _gameMenuUI.ToggleVisible(); });
        _gameInput.OnMenuAlternate += (object sender, EventArgs e) => { _gameMenuUI.ToggleVisible(); };
    }
}
