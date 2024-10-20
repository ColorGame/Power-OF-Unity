using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingScreenProvider: LocalAssetLoader
{
    private LoadingScreen _loadingScreen;

    public async UniTask Load()
    {
        _loadingScreen = await Load<LoadingScreen>(AssetsConstants.LoadingScreen);
    }

    public void DontDestroyOnLoad()
    {
        Object.DontDestroyOnLoad(_loadingScreen.gameObject);
    }
}
