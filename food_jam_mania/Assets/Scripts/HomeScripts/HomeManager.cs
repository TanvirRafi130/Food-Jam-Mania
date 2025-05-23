using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    [SerializeField] GameObject buttonsParent;
    [SerializeField] float buttonsYNegative;
    [SerializeField] GameObject title;
    [SerializeField] GameObject levelsButtonsParent;
    [SerializeField] GameObject levelShowPannel;
    int maxLevel;
    [SerializeField] GameObject buttonPrefab; // Button Prefab with TextMesh Pro GUI



    private static HomeManager _instance;

    public static HomeManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        ButtonInit();
        Effects();
        maxLevel = SceneManager.sceneCountInBuildSettings - 1;
        maxLevelForFakeLevel = maxLevel;
        fakeLevelNameValue = maxLevel;
        PopulateLevelsPanel();

    }



    Button playButton;
    Button levelPanelCloseButton;
    Button quitButton;
    void ButtonInit()
    {
        playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        levelPanelCloseButton = GameObject.Find("LevelPanelCloseButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        ////////////////////////////
        playButton.onClick.AddListener(() => LevelShowPanel());
        levelPanelCloseButton.onClick.AddListener(() => LevelClosePanel());
        quitButton.onClick.AddListener(() => ExitTheGame());


    }
    void ExitTheGame()
    {
        Application.Quit();
    }

    bool isFirstLevelPanelOpenDone = false;
    float lastHeight;
/*    void LevelShowPanel()
    {
        if (!isFirstLevelPanelOpenDone)
        {
            var height = levelsButtonsParent.GetComponent<RectTransform>().rect.height;
            levelsButtonsParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(levelsButtonsParent.GetComponent<RectTransform>().anchoredPosition.x, -height);
            isFirstLevelPanelOpenDone = true;
        }
*//*        else
        {
            levelsButtonsParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(levelsButtonsParent.GetComponent<RectTransform>().anchoredPosition.x, -lastHeight);
        }*//*
        levelShowPannel.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
        levelShowPannel.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }*/

    void LevelShowPanel()
    {
        levelShowPannel.GetComponent<ScrollRect>().enabled = true;
        if (!isFirstLevelPanelOpenDone)
        {
            var height = levelsButtonsParent.GetComponent<RectTransform>().rect.height;
            levelsButtonsParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -height);
            isFirstLevelPanelOpenDone = true;
        }

        levelShowPannel.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
        levelShowPannel.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }

    void LevelClosePanel()
    {
        levelShowPannel.GetComponent<ScrollRect>().enabled = false;
        // lastHeight = levelsButtonsParent.GetComponent<RectTransform>().rect.height;
        levelShowPannel.transform.DOScale(0, 0.5f).SetEase(Ease.Flash);
        levelShowPannel.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);

    }

    void Effects()
    {
        title.transform.localScale = Vector3.zero;
        title.transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);

        Sequence sq = DOTween.Sequence();
        foreach (Transform child in buttonsParent.transform)
        {
            Vector3 buttonsAnimationOffset = new Vector3(child.position.x, child.position.y - buttonsYNegative, child.position.z);
            child.transform.position = buttonsAnimationOffset;
        }
        foreach (Transform child in buttonsParent.transform)
        {
            sq.Append(child.transform.DOMoveY(child.transform.position.y + buttonsYNegative, 0.3f).SetEase(Ease.Flash));
        }
    }

    void Update()
    {
        //levelsButtonsParent.GetComponent<RectTransform>().position = new Vector2(levelsButtonsParent.GetComponent<RectTransform>().position.x,0f);
    }

    private float yOffset = 0; // initial y offset
    [SerializeField] private float verticalDiff; // initial y offset

    private void PopulateLevelsPanel()
    {
        // Create a button for each level up to maxLevel
        yOffset = 0;
        for (int i = 1; i <= maxLevel; i++)
        {
            // Create a new button
            GameObject button = CreateButton(i.ToString(), yOffset);

            // Increment the y offset for the next button
            yOffset -= verticalDiff; // adjust this value to change the spacing between buttons
        }
        levelShowPannel.transform.localScale = Vector3.zero;
    }



    private GameObject CreateButton(string sceneName, float yOffset)
    {
        // Instantiate the button prefab
        GameObject button = Instantiate(buttonPrefab);
        button.transform.SetParent(levelsButtonsParent.transform);

        // Set the button's position
        button.transform.localPosition = new Vector3(0, yOffset, 0);
        //  button.transform.localScale = Vector3.one;
        // Get the button component
        Button buttonComponent = button.GetComponent<Button>();

        // Get the text component
        TMPro.TextMeshProUGUI textComponent = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        textComponent.text = sceneName;

        // Add a listener to the button's onClick event
        buttonComponent.onClick.AddListener(() => LoadScene(sceneName));
        refScale = button.transform.localScale;
        return button;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }



    //for fake level data


    int maxLevelForFakeLevel;
    int fakeLevelData = 1;
    int fakeLevelNameValue;
    Vector3 refScale;
    public void PopulateLevelsPanelWithFakeLevelData()
    {
        fakeLevelNameValue++;
        // Create a new button
        GameObject button = CreateButtonFake(fakeLevelData.ToString(), yOffset);
        fakeLevelData++;
        if (fakeLevelData > maxLevelForFakeLevel)
        {
            fakeLevelData = 1;
        }
    
       


    }


    private GameObject CreateButtonFake(string sceneName, float yOffset)
    {
        // Instantiate the button prefab
        GameObject button = Instantiate(buttonPrefab);
        button.transform.SetParent(levelsButtonsParent.transform);

        // Set the button's position
        button.transform.localPosition = new Vector3(0, yOffset, 0);
        button.transform.localScale = refScale;
        //  button.transform.localScale = Vector3.one;
        // Get the button component
        Button buttonComponent = button.GetComponent<Button>();

        // Get the text component
        TMPro.TextMeshProUGUI textComponent = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        textComponent.text = fakeLevelNameValue.ToString();

        // Add a listener to the button's onClick event
        buttonComponent.onClick.AddListener(() => LoadScene(sceneName));

        return button;
    }



}