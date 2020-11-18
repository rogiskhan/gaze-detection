using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class GameController : MonoBehaviour
{
    public Question[] questions;
    private static List<Question> unansweredQuestions; //lista che contiene tutte le domande e che permette di rimuovere quelle a cui si è già risposto

    private Question currentQuestion;

    [SerializeField]
    private Text factText;

    [SerializeField]
    private float timeBetweenQuestions = 1f; // 1 secondo di ritardo tra le domande

    public string collisione;


    private GameObject Video;
    public Scene GameScene;

    private GameObject gamemanager;
    private GameObject scenemanager;

    void Start()
    {

        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questions.ToList<Question>();
        }
        SetRandomQuestion();

    }

    void SetRandomQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomQuestionIndex];

        factText.text = currentQuestion.fact;
        print("Question number:" + unansweredQuestions.Count);


    }
    //permette di aspettare qualche secondo prima di un nuovo evento
    IEnumerator TransitionToNextQuestion()
    {
        unansweredQuestions.Remove(currentQuestion);

        yield return new WaitForSeconds(timeBetweenQuestions);

        scenemanager = GameObject.Find("SceneManager");
        gamemanager = GameObject.Find("GameManager");
        Video = GameObject.Find("OriginalRawImage");
        GameScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.MoveGameObjectToScene(scenemanager, SceneManager.GetSceneByName("LoadScene"));
        SceneManager.MoveGameObjectToScene(gamemanager, SceneManager.GetSceneByName("LoadScene"));
        SceneManager.MoveGameObjectToScene(Video, SceneManager.GetSceneByName("LoadScene"));
        SceneManager.UnloadSceneAsync(GameScene);


    }

    //feedback delle risposte

    public void UserSelectTrue()
    {



        if (currentQuestion.isTrue)
        {
            Debug.Log("CORRECT");

        }
        else
        {
            Debug.Log("WRONG");
        }
        StartCoroutine(TransitionToNextQuestion());
    }

    public void UserSelectFalse()
    {


        if (!currentQuestion.isTrue && collisione == "false")
        {
            Debug.Log("CORRECT");
        }
        else
        {
            Debug.Log("WRONG");
        }
        StartCoroutine(TransitionToNextQuestion());
    }
}
