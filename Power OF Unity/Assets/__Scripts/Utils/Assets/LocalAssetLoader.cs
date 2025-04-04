using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalAssetLoader
{
    protected GameObject _cachedObject;
    /// <summary>
    /// ����������� �������� ���������� � ������� "Addressables.InstantiateAsync", � ��������� ���������� ���� <�>
    /// </summary>
    /// <remarks>��������� ��������� ����������</remarks>
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
    /// ����������� ��������
    /// </summary>
    /// <remarks>��������� ������ � ���������� ������� ��������</remarks>
    public async UniTask<Disposable<T>> LoadDisposable<T>(string assetId, Transform parent = null)
    {
        var component = await Load<T>(assetId, parent);
        return new Disposable<T>(component, _ => Unload());
    }
    /// <summary>
    /// ���������� � ��������� ������, ��������� � ������� Addressables.InstantiateAsync
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
