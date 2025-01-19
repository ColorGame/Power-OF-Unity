using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Установка спрайта на 2DVisual.<br/>
/// ВРЕМЕННЫЙ КЛАСС (Удалить и открепить в билде) 
/// </summary>
public class AutoCompletionSprite2D : MonoBehaviour
{
    [SerializeField] private Image _image;
    [ContextMenu("Автозаполнение")]

    private void AutoCompletion()
    {
        int visualDeleteLastCharName = 13;// Количество символов для удаления в имени визуала (чтобы убрать _2DVisual_1X1 )
        string visualName = name.Remove(name.Length - visualDeleteLastCharName); // Получим имя без последних 13 символов

        foreach (Sprite sprite in PlacedObjectGeneralListForAutoCompletionSO.Instance.Sprite2D)
        {
            if (visualName.Equals(sprite.name, StringComparison.OrdinalIgnoreCase))
            {
                _image.sprite = sprite;
            }
        }
    }
}
