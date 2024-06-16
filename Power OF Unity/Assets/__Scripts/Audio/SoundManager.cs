using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour // Менеджер ЗВУКА
{

    private AudioSource _audioSource; // Компонент источника звука (висит на SoundManager в сцене)
    private Dictionary<SoundName, AudioClip> _soundAudioClipDictionary; // Словарь Звуковой Аудио-клип(состояние Звука - ключ, Аудиоклип- -значение)
    private float _volume = .5f; // Громкость по умолчанию 50%
    /*private float _soundTimer; //Таймер состояния 
    private bool _isPlaying = false; 
    private SoundName _currentNameSound=SoundName.DefaultEmpty; //Текущее название звука*/


    private void Awake()
    {      
        _audioSource = GetComponent<AudioSource>(); // Получим компонент источника звука

        _volume = PlayerPrefs.GetFloat("soundVolume", .5f); // Загрузим громкость из сохранения

        // Загрузим все звуки при запуске, чтобы не искать их при воспроизведении
        _soundAudioClipDictionary = new Dictionary<SoundName, AudioClip>(); // Создадим словарь Звуковой Аудио-клип

        foreach (SoundName sound in System.Enum.GetValues(typeof(SoundName))) // Переберем массив состояния звука  (GetValues(Type) - Возвращает массив значений констант в указанном перечислении.)
        {
            _soundAudioClipDictionary[sound] = Resources.Load<AudioClip>(sound.ToString()); //Присвоим ключу значение - ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке Sounds).
        }
    }

   /* private void Update()
    {
        if (!_isPlaying) { return; }

        _soundTimer -= Time.deltaTime; // Запустим таймер для переключения состояний

        if (_soundTimer <= 0) // По истечению времени 
        {
            _currentNameSound = SoundName.DefaultEmpty; //Установим дефолтное состояние
        }
    }*/

    public void SetVolume( float Value) // Установить громкость
    {
        _volume = Mathf.Clamp01(Value); // Ограничем между 0 и 1
        PlayerPrefs.SetFloat("soundVolume", _volume); // Сохраним установленную громкость
    }    

    public float GetNormalizedVolume()
    {
        return _volume;
    }

    public void SetLoop(bool loop)
    {
        _audioSource.loop = loop;
    }

    /// <summary>
    /// Воспроизвести не прерывя предыдущий даже если он тот же.
    /// </summary>   
    public void PlayOneShot(SoundName sound)
    {
        _audioSource.PlayOneShot(_soundAudioClipDictionary[sound], _volume);// Воспроизводит аудиоклип и масштабирует громкость аудиоисточника по шкале громкости. не отменяет клипы, которые уже воспроизводятся
        /* if (_currentNameSound != sound)
         {
             _currentNameSound = sound;
             _audioSource.PlayOneShot(_soundAudioClipDictionary[sound], _volume);// Воспроизводит аудиоклип и масштабирует громкость аудиоисточника по шкале громкости. не отменяет клипы, которые уже воспроизводятся
             _soundTimer = _soundAudioClipDictionary[sound].length;
             _isPlaying = true;
         }    */
    }

    /// <summary>
    /// Воспроизвести. Если тот же клип, который воспроизводится, то клип будет звучать так, как будто он запущен заново.
    /// </summary>    
    public void Play(SoundName sound)
    {
        _audioSource.clip = _soundAudioClipDictionary[sound];
        _audioSource.volume = _volume;
        _audioSource.Play();
    }
    public void Stop()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
