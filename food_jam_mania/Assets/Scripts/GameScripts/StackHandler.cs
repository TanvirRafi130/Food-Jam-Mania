using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StackHandler : MonoBehaviour
{
    public foodType type;
    [SerializeField] Vector3 PositionOffsetAdd;

    [Header("Path Finding Config")]
    [SerializeField] public bool amIfreeStack = true;
    [SerializeField] public GameObject playerOnMe;
    [SerializeField] private int levelToUnlockme;
    private int currentScenNumber;
    [SerializeField] private bool lockNedded=false;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(wait());
        string sceneName = SceneManager.GetActiveScene().name;
        string numericSceneName = System.Text.RegularExpressions.Regex.Replace(sceneName, @"[^\d]", "");
        currentScenNumber = int.Parse(numericSceneName);
        UnlockCheck();
    }

    /*  IEnumerator wait()
      {
          yield return new WaitForEndOfFrame();
          UnlockCheck() ;
      }
  */

    void UnlockCheck()
    {
        var unlockIco = transform.Find("UnavailableImage");
        if (lockNedded)
        {
            if (levelToUnlockme > currentScenNumber)
            {
                GameManager.Instance.CallStackCheckIfRemoved(gameObject);
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 105f / 255f, 180f / 255f);
            }
            else {
            unlockIco.gameObject.SetActive(false);
            }
        }
        else
        {
            unlockIco.gameObject.SetActive(false);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemovePlayerFromStack(GameObject player)
    {
        GameObject.FindAnyObjectByType<GameManager>().stackPlayers.Remove(player);
        playerOnMe = null;

    }  
    public void AddPlayerToStack(GameObject player)
    {
        GameObject.FindAnyObjectByType<GameManager>().stackPlayers.Add(player);
        amIfreeStack = false;
        playerOnMe = player;

    }

 /*   public void StackIsNotAvailable()
    {
        transform.DOMoveY(50f, 3f).SetEase(Ease.Linear);
    }*/

 



 /*   private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
    
            amIfreeStack = false;
            playerOnMe = other.gameObject;
            if (GameObject.FindAnyObjectByType<GameManager>().stackPlayers.Contains(other.gameObject) == false)
            {
                GameObject.FindAnyObjectByType<GameManager>().stackPlayers.Add(other.gameObject);
            }
           
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("EXIT");
           // amIfreeStack = true;
            playerOnMe = null;
            GameObject.FindAnyObjectByType<GameManager>().stackPlayers.Remove(other.gameObject);
        }
    }*/

}

