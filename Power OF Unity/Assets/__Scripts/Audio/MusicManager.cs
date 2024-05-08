using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour   // Менеджер Фоновой музыки 
                                            // В настроиках поставить галочку возле Play On Awake
{
   
    private MusicName _music; // Аудио трек
    private float _musicTimer; //Таймер состояния

    private AudioSource _audioSource; // Компонент источника звука (висит на MusicManager в сцене)
    private Array _musicNameArray;
    private Dictionary<MusicName, AudioClip> _musicAudioClipDictionary; // Словарь Звуковой Аудио-клип(состояние Звука - ключ, Аудиоклип- -значение)
    private float _volume = .5f; // Громкость по умолчанию 50%    
    private System.Random _random = new System.Random();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _volume = PlayerPrefs.GetFloat("musicVolume", .5f); // Загрузим громкость из сохранения

        _audioSource.volume = _volume; // Установим громкость по умолчанию

        // Загрузим все звуки при запуске, чтобы не искать их при воспроизведении
        _musicAudioClipDictionary = new Dictionary<MusicName, AudioClip>(); // Создадим словарь Звуковой Аудио-клип
        _musicNameArray = Enum.GetValues(typeof(MusicName));
        foreach (MusicName sound in _musicNameArray) // Переберем массив состояния звука  (GetValues(Type) - Возвращает массив значений констант в указанном перечислении.)
        {
            _musicAudioClipDictionary[sound] = Resources.Load<AudioClip>(sound.ToString()); //Присвоим ключу значение - ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке Sounds).
        }
    }    

    private void Start()
    {
        _music= RandomEnumValue<MusicName>();
        _musicTimer = _musicAudioClipDictionary[_music].length;
        PlayMusic(_music); // Воспроизведем данный трек
    }


    private void Update()
    {
        _musicTimer -= Time.deltaTime; // Запустим таймер для переключения состояний
        
        if (_musicTimer <= 0) // По истечению времени _musicTimer вызовим NextMusic() которая в свою очередь переключит состояние. Например - у меня было TypeGrenade.Aiming: тогда в case TypeGrenade.Aiming: переключу на TypeGrenade.Shooting;
        {
            NextMusic(); //Следующая композиция
        }
    }

    /// <summary>
    /// Рандомная музыка из нашего перечисления
    /// </summary>    
    private MusicName RandomEnumValue<MusicName>()
    {
        return (MusicName)_musicNameArray.GetValue(_random.Next(_musicNameArray.Length));
    }

    public void NextMusic() //Автомат переключения состояний
    {
        switch (_music)
        {
            case MusicName.SeasonedOak:
                _music = MusicName.KingArthur58LegendOfTheSword;
               
                break;
            case MusicName.KingArthur58LegendOfTheSword:
                _music = MusicName.CaveFight;
                break;

            case MusicName.CaveFight:
                _music = MusicName.AssassinsBreathe;
                break;

            case MusicName.AssassinsBreathe:
                _music = MusicName.TheDarklands;
                break;

            case MusicName.TheDarklands:
                _music = MusicName.KingArthur58DestinyOfTheSword;
                break;

            case MusicName.KingArthur58DestinyOfTheSword:
                _music = MusicName.GrowingUpLondinium;
                break;

            case MusicName.GrowingUpLondinium:
                _music = MusicName.SeasonedOak;
                break;
        }
        _audioSource.Stop();
        _musicTimer = _musicAudioClipDictionary[_music].length; // Задаем продолжительность след трека
        PlayMusic(_music); // Воспроизведем полученный трек
    }

    public void PlayMusic(MusicName music) // Воспроизведение Звука
    {
        _audioSource.PlayOneShot(_musicAudioClipDictionary[music], _volume);// Воспроизводит аудиоклип и масштабирует громкость аудиоисточника по шкале громкости.
    }

    public void SetVolume(float Value) // Установить громкость
    {
        _volume = Mathf.Clamp01(Value); // Ограничем между 0 и 1
        _audioSource.volume = _volume; // Установим громкость
        PlayerPrefs.SetFloat("musicVolume", _volume); // Сохраним установленную громкость
    }   

    public float GetNormalizedVolume() // Получить громкость
    {
        return _volume;
    }
}
