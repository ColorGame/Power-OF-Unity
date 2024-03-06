using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour // Менеджер ЗВУКА
{

    private AudioSource audioSource; // Компонент источника звука (висит на SoundManager в сцене)
    private Dictionary<SoundName, AudioClip> soundAudioClipDictionary; // Словарь Звуковой Аудио-клип(состояние Звука - ключ, Аудиоклип- -значение)
    private float volume = .5f; // Громкость по умолчанию 50%

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Получим компонент источника звука

        volume = PlayerPrefs.GetFloat("soundVolume", .5f); // Загрузим громкость из сохранения

        // Загрузим все звуки при запуске, чтобы не искать их при воспроизведении
        soundAudioClipDictionary = new Dictionary<SoundName, AudioClip>(); // Создадим словарь Звуковой Аудио-клип

        foreach (SoundName sound in System.Enum.GetValues(typeof(SoundName))) // Переберем массив состояния звука  (GetValues(Type) - Возвращает массив значений констант в указанном перечислении.)
        {
            soundAudioClipDictionary[sound] = Resources.Load<AudioClip>(sound.ToString()); //Присвоим ключу значение - ресурс запрошенного типа, хранящийся по адресу path(путь) в папке Resources(эту папку я создал в папке Sounds).
        }
    }

    public void PlaySoundOneShot(SoundName sound) // Воспроизведение Звука один раз
    {
        audioSource.PlayOneShot(soundAudioClipDictionary[sound], volume);// Воспроизводит аудиоклип и масштабирует громкость аудиоисточника по шкале громкости.
    }

    public void IncreaseVolume() // Увеличение громкости
    {
        volume += .1f;
        volume = Mathf.Clamp01(volume); // Ограничем между 0 и 1
        PlayerPrefs.SetFloat("soundVolume", volume); // Сохраним установленную громкость
    }

    public void DecreaseVolume() // Уменьшение громкости
    {
        volume -= .1f;
        volume = Mathf.Clamp01(volume); // Ограничем между 0 и 1
        PlayerPrefs.SetFloat("soundVolume", volume); // Сохраним установленную громкость
    }

    public float GetVolume()
    {
        return volume;
    }

    public void SetLoop(bool loop)
    {
        audioSource.loop = loop;
    }

    public void Play(SoundName sound)
    {
        audioSource.clip = soundAudioClipDictionary[sound];
        audioSource.volume = volume;
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}
