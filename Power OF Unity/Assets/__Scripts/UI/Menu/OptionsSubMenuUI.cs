using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Пол меню настроек звука видео... 
/// </summary>
/// <remarks>
/// Создается и инициализируется в PersistentEntryPoint
/// </remarks>
public class OptionsSubMenuUI : ToggleVisibleAnimatioMenuUI 
{
    public event EventHandler<bool> OnEdgeScrollingChange;

    [Header("Текст который будем менять")]
    [SerializeField] private TextMeshProUGUI _soundVolumeText; // Текст громкости звука
    [SerializeField] private TextMeshProUGUI _musicVolumeText; // Текст громкости музыки
    [Header("Кнопки включения панелей")]
    [SerializeField] private Button _videoPanelButton;
    [SerializeField] private Button _soundPanelButton;
    [SerializeField] private Button _controlPanelButton;
    [Header("Панели которые будем переключать")]
    [SerializeField] private GameObject _videoPanel;
    [SerializeField] private GameObject _soundPanel;
    [SerializeField] private GameObject _controlPanel;
    [Header("Переключатели внутри панелей")]
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Button _musicNextButton;
    [SerializeField] private Toggle _edgeScrollingToggle;
    [Header("Кнопки нижней панелей")]
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _resetButton;    

    private SoundManager _soundManager; // Менеджер ЗВУКА
    private MusicManager _musicManager; // Менеджер МУЗЫКИ    

    private bool _edgeScrolling; // Прокрутка по краям   
    private List<GameObject> _panelList; // Список панелий которые будем скрывать и показывать
      

    public void Init(GameInput gameInput, SoundManager soundManager, MusicManager musicManager, HashAnimationName hashAnimationName)
    {
        _soundManager = soundManager;
        _musicManager = musicManager;
        _gameInput = gameInput;
        _hashAnimationName = hashAnimationName;

        Setup();
        SetAnimationOpenClose();
    }     

    private void Setup()
    {
        _panelList = new List<GameObject>() { _videoPanel, _soundPanel, _controlPanel };        

        _edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // Загрузим сохраненый параметр _edgeScrolling и если это 1 то будет истина если не=1 то будет ложь (из PlayerPrefs.GetInt нельзя тегать булевые параметры поэтому используем строку)
        _edgeScrollingToggle.SetIsOnWithoutNotify(_edgeScrolling); // Установим актуальное значения Тумблера прокрутка по краям  

        _soundSlider.value = _soundManager.GetNormalizedVolume();
        _musicSlider.value = _musicManager.GetNormalizedVolume();

        InitButtons();   
    }

    protected override void SetAnimationOpenClose()
    {
        _animationOpen = _hashAnimationName.OptionsSubMenuOpen;
        _animationClose = _hashAnimationName.OptionsSubMenuClose;
    }

    private void InitButtons()
    {
        _videoPanelButton.onClick.AddListener(() => { ShowPanel(_videoPanel); });
        _soundPanelButton.onClick.AddListener(() => { ShowPanel(_soundPanel); });
        _controlPanelButton.onClick.AddListener(() => { ShowPanel(_controlPanel); });

        _soundSlider.onValueChanged.AddListener((v) =>
        {
            _soundManager.SetVolume(_soundSlider.value);
            UpdateText();
        });

        _musicSlider.onValueChanged.AddListener((v) =>
        {
            _musicManager.SetVolume(_musicSlider.value);
            UpdateText();
        });

        _musicNextButton.onClick.AddListener(() => { _musicManager.NextMusic(); });
        _edgeScrollingToggle.onValueChanged.AddListener((bool set) => { SetEdgeScrolling(set); });      
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
        _resetButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА"); });
    }

    private void UpdateText() // Обновим текс громкости
    {
        // Что бы легче читалось умножим на 10 и округлим до целых
        _soundVolumeText.SetText(Mathf.RoundToInt(_soundManager.GetNormalizedVolume() * 10).ToString());
        _musicVolumeText.SetText(Mathf.RoundToInt(_musicManager.GetNormalizedVolume() * 10).ToString());
    }

    /// <summary>
    /// Установить булевое значение для - прокрутки по краям
    /// </summary>    
    private void SetEdgeScrolling(bool edgeScrolling)
    {
        _edgeScrolling = edgeScrolling;
        PlayerPrefs.SetInt("edgeScrolling", edgeScrolling ? 1 : 0); // Сохраним полученное значение в память (если _edgeScrolling истина то установим 1 если ложь установим 0 )
        OnEdgeScrollingChange?.Invoke(this, _edgeScrolling);
    }

    /// <summary>
    /// Покажем переданную панель.
    /// </summary>   
    /// <remarks>Остальные панели скроем</remarks>
    private void ShowPanel(GameObject showPanel)
    {
        foreach (GameObject panel in _panelList)
        {
            if (panel == showPanel)
                panel.SetActive(true);
            else
                panel.SetActive(false);
        }
    }

}
