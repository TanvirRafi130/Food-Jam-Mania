using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnHandler : MonoBehaviour
{
    public foodType type;
    public colorType colortype;
    [SerializeField] Vector3 playerPositionOffsetAdd;

    [Header("Path Finding Config")]
    [SerializeField] public bool amIfree = false;
    [SerializeField] public bool amITopTile;
    [SerializeField] public List<GameObject> connectedTilesList;
    public Image myFoodImage;
    [SerializeField] GameObject canvasPrefab;
    [SerializeField] Vector3 canvasPosOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] Vector3 foodHolderPosOffset = new Vector3(0f, 0f, 0f);
    public GameObject playerOnMe;
   // [SerializeField] List<GameObject> playersPrefab;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait());

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.matrix = transform.localToWorldMatrix; //that's for the box rotation
        Gizmos.DrawWireCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
    }






    //will get called from gamemanager
    public void Ins(GameObject player,/* Color color,*/ foodType type, Sprite sp)
    {
        var idPlayer = Instantiate(player, transform.position + playerPositionOffsetAdd, Quaternion.identity);
        playerOnMe = idPlayer;
        var idCanvas = Instantiate(canvasPrefab, idPlayer.transform);
        idCanvas.name = "CanvasForOrderImage";

        if (amITopTile)
        {
            idPlayer.GetComponent<Player>().IncreasePlayerOutLine();
        }
        else
        {
            idPlayer.GetComponent<Player>().PlayerOutlineColorChanger();
        }

        /////////////////////////////
        // Create a new empty game object

        GameObject childObject = new GameObject("FoodHolder");


        // Set the parent of the child object

        childObject.transform.SetParent(idPlayer.transform);


        // Optionally, you can also set the local position and rotation of the child object

        childObject.transform.localPosition = foodHolderPosOffset;

        childObject.transform.localRotation = Quaternion.identity;

        ///////////////////
        ///


        idCanvas.transform.localPosition = canvasPosOffset; // adjust the position to be on top of the player's head

        //idCanvas.transform.localScale = Vector3.one;

        idCanvas.transform.localRotation = Quaternion.identity;

        idCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        var playerScript = idPlayer.GetComponent<Player>();
        //playerScript.playerCanvas = idCanvas;
        playerScript.myType = type;
        playerScript.foodSprite = sp;
        playerScript.myCurrentGround = gameObject;
        playerScript.StartCoroutine(playerScript.SetPlayerOrderImage());

    }

    IEnumerator wait()
    {
        yield return new WaitForEndOfFrame();
        playerOnMe.GetComponent<Player>().dependencies = connectedTilesList;
        if (connectedTilesList.Count == 0) { Debug.LogError(gameObject.name + " has no dependencies"); }
    }



    /*  private void OnTriggerEnter(Collider other)
      {
          if(other.gameObject.tag == "Player")
          {
              amIfree = false;
          }
      }

      private void OnTriggerExit(Collider other)
      {
          if (other.gameObject.tag == "Player")
          {
              amIfree = true;
          }
      }*/
}

