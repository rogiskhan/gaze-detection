using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Load : MonoBehaviour
{

    private GameObject gamemanager;
    private GameObject scenemanager;

    private GameObject Video;
    public Scene LoadScene;
    private void Start()
    {
        StartCoroutine(ReloadGameScene());
    }
    
    IEnumerator ReloadGameScene()
    {
        
        //ricarico la scena di gioco
        scenemanager = GameObject.Find("SceneManager");
        gamemanager = GameObject.Find("GameManager");
        Video = GameObject.Find("OriginalRawImage");
        LoadScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.MoveGameObjectToScene(scenemanager, SceneManager.GetSceneByName("GameScene"));
        SceneManager.MoveGameObjectToScene(gamemanager, SceneManager.GetSceneByName("GameScene"));
        SceneManager.MoveGameObjectToScene(Video, SceneManager.GetSceneByName("GameScene"));
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LoadScene"));
    }


}
