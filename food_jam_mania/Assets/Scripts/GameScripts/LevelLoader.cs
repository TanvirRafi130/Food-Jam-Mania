using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    private static LevelLoader _instance;

    public static LevelLoader Instance => _instance;


    private void Awake()
    {
        _instance = this;

    }
    // Start is called before the first frame update
    void Start()

    {

        DontDestroyOnLoad(gameObject);




    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadNextLevel()
    {

        var name = SceneManager.GetActiveScene().name;
        int nameToInt;
        nameToInt = int.Parse(name);
        if (nameToInt >= SceneManager.sceneCountInBuildSettings - 1)
        {
            nameToInt = 1;
        }
        else
        {
            nameToInt++;
        }
        SceneManager.LoadScene(nameToInt.ToString());
    }
}
