
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    

    private void Awake()
    {        

        transform.Find("mainMenuButton").GetComponent<Button>().onClick.AddListener(() =>
        {            
            SceneLoader.Load(SceneName.UnitMenuScene);
        });

        transform.Find("againButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneName.GameScene_MultiFloors);
        });
    }   
}
