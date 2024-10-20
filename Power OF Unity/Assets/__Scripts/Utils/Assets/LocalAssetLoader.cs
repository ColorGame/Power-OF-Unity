using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalAssetLoader
{
    protected GameObject _cachedObject;
    /// <summary>
    /// Асинхронное создание экземпляра с помощью "Addressables.InstantiateAsync", и получение компонента типа <Т>
    /// </summary>
    /// <remarks>Созданный экземпляр кэшируется</remarks>
    public async UniTask<T> Load<T>(string assetId, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(assetId, parent);
        _cachedObject = await handle.Task;
        if (_cachedObject.TryGetComponent(out T component) == false)
            throw new NullReferenceException($"Object of type {typeof(T)} is null on " +
                                             "attempt to load it from addressables");
        return component;
    }
    /// <summary>
    /// Одноразовая загрузка
    /// </summary>
    /// <remarks>Загружает ресурс и возвращает делегат выгрузки</remarks>
    public async UniTask<Disposable<T>> LoadDisposable<T>(string assetId, Transform parent = null)
    {
        var component = await Load<T>(assetId, parent);
        return new Disposable<T>(component, _ => Unload());
    }
    /// <summary>
    /// Освобождим и уничтожим объект, созданный с помощью Addressables.InstantiateAsync
    /// </summary>
    public virtual void Unload()
    {
        if (_cachedObject == null)
            return;
        _cachedObject.SetActive(false);
        Addressables.ReleaseInstance(_cachedObject);
        _cachedObject = null;
    }
}
