
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionsMenuUI : MonoBehaviour // Меню настроик 
{
    public event EventHandler<bool> OnEdgeScrollingChange;

    private SoundManager _soundManager; // Менеджер ЗВУКА
    private MusicManager _musicManager; // Менеджер МУЗЫКИ
    private GameInput _gameInput;

    private ControlMenuUI _controlMenu; // Меню управления

    private TextMeshProUGUI _soundVolumeText; // Текст громкости звука
    private TextMeshProUGUI _musicVolumeText; // Текст громкости музыки

    private bool _edgeScrolling; // Прокрутка по краям


    public void Initialize(GameInput gameInput, SoundManager soundManager, MusicManager musicManager)
    {
        _soundManager = soundManager;
        _musicManager = musicManager;
        _gameInput = gameInput;
    }

    private void Awake()
    {
        _soundVolumeText = transform.Find("soundVolumeText").GetComponent<TextMeshProUGUI>();
        _musicVolumeText = transform.Find("musicVolumeText").GetComponent<TextMeshProUGUI>();
        _controlMenu = GetComponentInChildren<ControlMenuUI>();

        // Найдем кнопки и Добавим событие при нажатии на наши кнопки
        // AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        transform.Find("soundIncreaseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _soundManager.IncreaseVolume();
            UpdateText();
        });
        transform.Find("soundDecreaseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _soundManager.DecreaseVolume();
            UpdateText();
        });

        transform.Find("musicIncreaseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _musicManager.IncreaseVolume();
            UpdateText();
        });
        transform.Find("musicDecreaseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _musicManager.DecreaseVolume();
            UpdateText();
        });

        transform.Find("musicNextButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _musicManager.NextMusic();
        });

        transform.Find("mainMenuButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneName.MainMenuScene);
        });

        transform.Find("edgeScrollingToggle").GetComponent<Toggle>().onValueChanged.AddListener(((bool set) => // Подпишемся на изменение значения Тумблера прокрутка по краям (принимает булевое значение)
        {
            SetEdgeScrolling(set);
        }));

        transform.Find("resumeButton").GetComponent<Button>().onClick.AddListener(() =>
        {

            gameObject.SetActive(false); // спрячем меню
        });

        transform.Find("controlButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            ToggleVisibleControlMenu();
        });

        _gameInput.OnMenuAlternate += GameInput_OnMenuAlternate;
    }

    private void Start()
    {
        UpdateText();
        gameObject.SetActive(false); // спрячем меню

        _edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 1) == 1; // Загрузим сохраненый параметр _edgeScrolling и если это 1 то будет истина если не=1 то будет ложь (из PlayerPrefs.GetInt нельзя тегать булевые параметры поэтому используем строку)
        transform.Find("edgeScrollingToggle").GetComponent<Toggle>().SetIsOnWithoutNotify(_edgeScrolling); // Установим актуальное значения Тумблера прокрутка по краям  
    }

    private void GameInput_OnMenuAlternate(object sender, System.EventArgs e)
    {
        ToggleVisible();
    }

    private void UpdateText() // Обновим текс громкости
    {
        // Что бы легче читалось умножим на 10 и округлим до целых
        _soundVolumeText.SetText(Mathf.RoundToInt(_soundManager.GetVolume() * 10).ToString());
        _musicVolumeText.SetText(Mathf.RoundToInt(_musicManager.GetVolume() * 10).ToString());
    }

    public void ToggleVisible() // Переключатель видимости меню НАСТРОЙКИ (будем вызывать через инспектор кнопкой OptionsButton)
    {
        gameObject.SetActive(!gameObject.activeSelf); // Переключим в противоположное состояние        
    }


    public void ToggleVisibleControlMenu()
    {
        _controlMenu.SetIsOpen(!_controlMenu.GetIsOpen()); // Переключим в противоположное состояние
        _controlMenu.UpdateStateControlMenu(_controlMenu.GetIsOpen());
    }

    /// <summary>
    /// Установить булевое значение для - прокрутки по краям
    /// </summary>    
    public void SetEdgeScrolling(bool edgeScrolling)
    {
        _edgeScrolling = edgeScrolling;
        PlayerPrefs.SetInt("edgeScrolling", edgeScrolling ? 1 : 0); // Сохраним полученное значение в память (если _edgeScrolling истина то установим 1 если ложь установим 0 )
        OnEdgeScrollingChange?.Invoke(this, _edgeScrolling);
    }


}
