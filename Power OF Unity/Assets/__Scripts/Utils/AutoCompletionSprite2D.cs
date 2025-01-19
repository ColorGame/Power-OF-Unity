using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ��������� ������� �� 2DVisual.<br/>
/// ��������� ����� (������� � ��������� � �����) 
/// </summary>
public class AutoCompletionSprite2D : MonoBehaviour
{
    [SerializeField] private Image _image;
    [ContextMenu("��������������")]

    private void AutoCompletion()
    {
        int visualDeleteLastCharName = 13;// ���������� �������� ��� �������� � ����� ������� (����� ������ _2DVisual_1X1 )
        string visualName = name.Remove(name.Length - visualDeleteLastCharName); // ������� ��� ��� ��������� 13 ��������

        foreach (Sprite sprite in PlacedObjectGeneralListForAutoCompletionSO.Instance.Sprite2D)
        {
            if (visualName.Equals(sprite.name, StringComparison.OrdinalIgnoreCase))
            {
                _image.sprite = sprite;
            }
        }
    }
}
