using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ������ ���� � ����� �������� �����
/// </summary>
public class UpperMenuBarOnUnitSetupUI : MonoBehaviour
{
    [Header("������ ��������� �������� ����")]
    [SerializeField] private Button _gameMenuButton;
    [Header("����� ���������� �����")]
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
