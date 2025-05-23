using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class UiManager : MonoBehaviour
{

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameDonePanel;
    Button restartButton;
    Button nextLevelButton;
    Button button_Home_Level_Failed;
    Button button_Home_Level_Complete;



    private static UiManager _instance;

    public static UiManager Instance => _instance;


    private void Awake()
    {
        _instance = this;

    }


    // Start is called before the first frame update
    void Start()
    {
        ButtonInit();
        //gameOverPanel.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.Flash);
        gameOverPanel.transform.localScale = Vector3.zero;
        gameDonePanel.transform.localScale = Vector3.zero;
        
        GameObject.Find("LevelNumberText").GetComponent<TextMeshProUGUI>().text ="LV: "+SceneManager.GetActiveScene().name;
    }

    void ButtonInit()
    {
        restartButton = GameObject.Find("Button_Restart").GetComponent<Button>();
        nextLevelButton = GameObject.Find("Button_Next_Level").GetComponent<Button>();
        button_Home_Level_Complete = GameObject.Find("Button_Home_Level_Complete").GetComponent<Button>();
        button_Home_Level_Failed = GameObject.Find("Button_Home_Level_Failed").GetComponent<Button>();

        /////////////////////////////
        ///

        restartButton.onClick.AddListener(() => Restart());
        nextLevelButton.onClick.AddListener(() => NextLevel());
        button_Home_Level_Failed.onClick.AddListener(() => Return_Home());
        button_Home_Level_Complete.onClick.AddListener(() => Return_Home());

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void openGameOverPanel()
    {
        gameOverPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Flash).OnComplete(() => { Time.timeScale = 0; });
    } 
    public void openGameDonerPanel()
    {
        //GameObject.Find("Button_Restart").GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        gameDonePanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Flash).OnComplete(() => { Time.timeScale = 0; });
    }

    public void NextLevel() 
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        LevelLoader.Instance.LoadNextLevel();

    }
        public void Restart() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       // LevelLoader.Instance.LoadNextLevel();

    }

    void Return_Home()
    {
        Destroy(GameObject.Find("LevelLoader"));
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeMenu");

    }

}
