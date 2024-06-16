using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    private ScenesService _scenesService;

    public void Init(ScenesService scenesService)
    {
        _scenesService = scenesService;
    }

    private void Awake()
    {        

        transform.Find("mainMenuButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _scenesService.Load(SceneName.MainMenu);
        });

        transform.Find("againButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            _scenesService.Load(SceneName.Level_1);
        });
    }   
}
