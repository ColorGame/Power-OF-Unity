// Copyright (c) 2024 Synty Studios Limited. All rights reserved.
//
// Использование данного программного обеспечения регулируется положениями и условиями Лицензионного соглашения с конечным пользователем Synty Studios (EULA)
// доступно по адресу: https://syntystore.com/pages/end-user-licence-agreement
//
// Sample scripts are included only as examples and are not intended as production-ready.

using UnityEngine;
using UnityEngine.UI;


/// <summary>
///    Анимирует ультрафиолетовые лучи необработанного изображения для создания эффекта прокрутки фона.
/// </summary>
public class SampleScrollUV : MonoBehaviour
{
    [Header("References")]
    public RawImage rawImage;

    [Header("Parameters")]
    public Vector2 speed = new Vector2(1, 0);
    public Vector2 size = new Vector2(256, 256);

    private void Awake()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
    }

    private void Reset()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        Vector2 calculatedSizeBasedOnScreen = new Vector2(
            rawImage.rectTransform.rect.width / size.x,
            rawImage.rectTransform.rect.height / size.y
        );
        rawImage.uvRect = new Rect(
            rawImage.uvRect.position + (speed * Time.deltaTime),
            calculatedSizeBasedOnScreen
        );
    }
}

