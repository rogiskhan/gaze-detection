using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUI : MonoBehaviour
{
    public GameObject scenemanager;
    public Text myText;
    public Slider slider;
    public Scene SampleScene;
    public GameObject Video;


    private SceneController SceneMan;
    private StreamVideo StreamVid;


    private void Start()
    {
        //prendo i componenti utili alla gui
        SceneMan = scenemanager.GetComponent<SceneController>();
        StreamVid = scenemanager.GetComponent<StreamVideo>();
    }
    public void ButtonLeft()
    {
        //stoppo il timer e seleziono l'occhio sinistro
        SceneMan.timer.Stop();
        SceneMan.pushed = true;
    }

    public void ButtonRight()
    {
        //stoppo il timere e seleziono l'occhio destro
        SceneMan.timer.Stop();
        SceneMan.pushed = true;
        SceneMan.right = true;
    }

    public void Slider()
    {
        //comando lo slider per regolare la threshold per la sgmentazione
        myText.text = "Threshold: " + (int)slider.value;
        StreamVid.th = (int)slider.value;
    }

    public void ToTheGame()
    {
        //chiamo la funzione per andare al gioco
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        //porto via gli oggetti che servono per l'eye tracking e cambio scena
        SampleScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.MoveGameObjectToScene(scenemanager, SceneManager.GetSceneByName("GameScene"));
        SceneManager.MoveGameObjectToScene(Video, SceneManager.GetSceneByName("GameScene"));
        SceneManager.UnloadSceneAsync(SampleScene);
    }

}


