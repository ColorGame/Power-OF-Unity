
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour // Меню настроик 
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
    [Header("Кнопка в левом аерхнем углу")]
    [SerializeField] private Button _optionsButton;


    [Header("Фон который блокирует \n взаимодействие с задним фоном")]
    [SerializeField] private Image _backgroundBlockRaycast;

    private SoundManager _soundManager; // Менеджер ЗВУКА
    private MusicManager _musicManager; // Менеджер МУЗЫКИ
    private GameInput _gameInput;

    private bool _edgeScrolling; // Прокрутка по краям
    private bool _toggleBool;
    private List<GameObject> _panelList; // Список панелий которые будем скрывать и показывать


    private Animator _animator; //Аниматор для меню
    private HashAnimationName _animBase = new HashAnimationName();
    private float _animationTimer;   
    private int _stateHashNameAnimation;
    private bool _animationStart;

    public void Initialize(GameInput gameInput, SoundManager soundManager, MusicManager musicManager)
    {
        _soundManager = soundManager;
        _musicManager = musicManager;
        _gameInput = gameInput;
    }

    private void Awake()
    {
        _panelList = new List<GameObject>() { _videoPanel, _soundPanel, _controlPanel };
        _animator = GetComponent<Animator>();

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

        _optionsButton.onClick.AddListener(() => { ToggleVisible(); });
        _quitButton.onClick.AddListener(() => { ToggleVisible(); });
        _resetButton.onClick.AddListener(() => { Debug.Log("ЗАГЛУШКА"); });

        _gameInput.OnMenuAlternate += GameInput_OnMenuAlternate;
    }

    private void Start()
    {
        HideOptionsMenu();
        _toggleBool= false;

        _edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // Загрузим сохраненый параметр _edgeScrolling и если это 1 то будет истина если не=1 то будет ложь (из PlayerPrefs.GetInt нельзя тегать булевые параметры поэтому используем строку)
        _edgeScrollingToggle.SetIsOnWithoutNotify(_edgeScrolling); // Установим актуальное значения Тумблера прокрутка по краям  

        _soundSlider.value = _soundManager.GetNormalizedVolume();
        _musicSlider.value = _musicManager.GetNormalizedVolume();

        UpdateText();
    }

    private void Update()
    {
        if (!_animationStart)  
            return; 

        _animationTimer -= Time.deltaTime; // Запустим таймер

        if(_animationTimer <= 0)
        {
            _animator.enabled = false; // Отключим анимацию что бы вернуть управление в КОД
            _animationStart = false;
            if (_stateHashNameAnimation ==_animBase.OptionsMenuClose) // Если сейчас анимация закрывания то
                HideOptionsMenu();
        }
    }

    private void GameInput_OnMenuAlternate(object sender, System.EventArgs e)
    {
        ToggleVisible();
    }

    private void UpdateText() // Обновим текс громкости
    {
        // Что бы легче читалось умножим на 10 и округлим до целых
        _soundVolumeText.SetText(Mathf.RoundToInt(_soundManager.GetNormalizedVolume() * 10).ToString());
        _musicVolumeText.SetText(Mathf.RoundToInt(_musicManager.GetNormalizedVolume() * 10).ToString());
    }

    public void ToggleVisible() // Переключатель видимости меню НАСТРОЙКИ (будем вызывать через инспектор кнопкой OptionsButton)
    {
        _toggleBool = !_toggleBool;

        if (!_toggleBool) // Если не активны(выключена) то активируем и включим анимацию
        {
            ShowOptionsMenu();
            _stateHashNameAnimation = _animBase.OptionsMenuOpen;
            StartAnimation();
        }
        else
        {
            _stateHashNameAnimation = _animBase.OptionsMenuClose;
            StartAnimation();           
        }
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

    private void ShowOptionsMenu()
    {
        gameObject.SetActive(true);
        _backgroundBlockRaycast.enabled = true;
    }
    private void HideOptionsMenu()
    {
        gameObject.SetActive(false);
        _backgroundBlockRaycast.enabled = false;       
    }   

    private void StartAnimation()
    {
        _animator.enabled = true;
        _animator.CrossFade(_stateHashNameAnimation, 0.5f);
        _animationTimer =_animator.GetCurrentAnimatorStateInfo(0).length;
        _animationStart = true;
    }
}
