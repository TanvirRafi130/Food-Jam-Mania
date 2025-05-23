using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Drawing;
using System;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct FoodData
    {
        public foodType type;
        //  public colorType color;
        public Sprite sprite;
        public GameObject MyPrefabModel;

    }
    [System.Serializable]
    public struct FoodSerial
    {
        public foodType type;
        public colorType trayColor;

    }
    public Sprite crossImage;
    [SerializeField] public FoodData[] FoodList;
    [SerializeField] GameObject spawnParent;
    [SerializeField] GameObject stackParent;
    [SerializeField] List<GameObject> playersPrefab;
    [NonSerialized] public List<GameObject> stackPlayers = new List<GameObject>();
    [NonSerialized] public List<GameObject> stackTotal = new List<GameObject>();
    //[SerializeField] Image nextFoodImage;
    // [SerializeField] TextMeshProUGUI nextFoodNumberValues;
    //[SerializeField] foodType nextType;
    // [SerializeField] int nextTypeNumbers;
    [Space(100)]
    public List<FoodSerial> levelFoodSerial; // LevelDesigner selects food order in editor
    [SerializeField]List<GameObject> orderSerial = new List<GameObject>(); //takes data from ordersserial
    int foodNumber = 0;
    [Space(100)]
    [SerializeField] List<Transform> plateStackPositions;
    int plateNumber = 0;
    public List<int> freePlates;
    [SerializeField] Transform ovenFoodPosition;
    [SerializeField] GameObject shopFrontStanding;
    [SerializeField] GameObject shopInsidePos;

    [SerializeField] Queue<GameObject> playerToCollectFood = new Queue<GameObject>();

    // [SerializeField] Queue<GameObject> foodToCollect = new Queue<GameObject>();

    [SerializeField] Queue<int> nextPlateNum = new Queue<int>();

    [SerializeField] List<GameObject> foodsAvailableToCollect = new List<GameObject>();

    [SerializeField] GameObject exitDoor;
    [SerializeField] GameObject foodTrayPrefab;


    [NonSerialized] public List<GameObject> availablePlayers = new List<GameObject>();

    [NonSerialized] public bool canShopEscMove = false;
    [SerializeField] float escMovDuration;
    public float escMovSpeed = 5;




    private static GameManager _instance;

    public static GameManager Instance => _instance;


    private void Awake()
    {
        _instance = this;
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(foodTrayPrefab,shopInsidePos.transform.position, Quaternion.identity);
        ObjectInitializer();
        StartCoroutine(wait());
        GetInLine();
        PutInOvenForFirstThree();
        // StartCoroutine(CustomMatcherCall());

    }

    // Update is called once per frame
    void Update()
    {
        //  matcher();
        //  Debug.Log(playerToCollectFood.Count);
    }

    //get level food order searial and food prefab

    void GetInLine()
    {
        orderSerial.Clear(); // Clear the list to start fresh

        foreach (FoodSerial serial in levelFoodSerial)
        {
            foreach (FoodData food in FoodList)
            {
                if (food.type == serial.type)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        var id = Instantiate(food.MyPrefabModel, new Vector3(0f, 10000f, 0f), Quaternion.identity);
                        id.GetComponent<Food>().trayColor = serial.trayColor;
                        orderSerial.Add(id);
                    }
                    
                    break; // Break out of the inner loop to move on to the next serial
                }
            }
        }
        //NameCHeck();
    }


    void NameCHeck()
    {
        foreach (GameObject food in orderSerial)
        {
            Debug.LogError(food.GetComponent<Food>().trayColor + (" ") + food.name);
        }
    }






    [SerializeField] GameObject ovenDoor;
    [SerializeField] float ovenDoorCloseValue;
    [SerializeField] float ovenDoorOpenValue;




    /// <summary>
    /// //////////////
    /// </summary>
    void PutInOvenForFirstThree()
    {

        StartCoroutine(PutFoodsInPlateForFirstThree());



    }

    int iterationValues = 6;
    Queue<GameObject> cannotCollect = new Queue<GameObject>();
    bool isFirstRotDone = false;
    bool separate = false;
    IEnumerator PutFoodsInPlateForFirstThree()
    {
        if (orderSerial.Count > 0 && foodNumber < orderSerial.Count)
        {
            int currentFoodNum = foodNumber;
            for (foodNumber = currentFoodNum; foodNumber < currentFoodNum + iterationValues; foodNumber++)
            {
                GameObject id = Instantiate(orderSerial[foodNumber], ovenFoodPosition.position, Quaternion.identity);
                id.transform.parent = ovenFoodPosition.transform;
                id.GetComponent<Food>().foodPlateNumber = plateNumber;
                if (!separate)
                {
                    foodsAvailableToCollect.Add(id);
                    if (foodsAvailableToCollect.Count == 3) { separate = true; }
                }
                else
                {
                    cannotCollect.Enqueue(id);
                }
                yield return PutInPlateForFirst3Animations(id, plateNumber);
                plateNumber++;
                if (plateNumber == 3 && !isFirstRotDone)
                {
                    isFirstRotDone = true;
                    yield return MoveThePlatesTimer(escMovDuration);
                }
            }
            if (plateNumber == 6)
            {

                plateNumber = 0;
            }
            iterationValues = 3;
        }
        else
        {
            // Handle the case where there are no more foods to put in the plate
            Debug.Log("No more foods to put in the plate");

        }
    }
    IEnumerator PutInPlateForFirst3Animations(GameObject food, int pn)
    {
        var idTray = Instantiate(foodTrayPrefab, shopInsidePos.transform.position, Quaternion.identity);
        Sequence sequence = DOTween.Sequence();
        idTray.transform.parent = plateStackPositions[pn].transform;
        colorType cl = food.GetComponent<Food>().trayColor;
        UnityEngine.Color c = ColorSelector(cl);
        idTray.GetComponent<MeshRenderer>().material.color = c;
        sequence.Append(idTray.transform.DOLocalJump(Vector3.zero, 2f, 1, 0.2f).SetEase(Ease.OutFlash));
        sequence.Append(ovenDoor.transform.DOLocalMoveZ(ovenDoorOpenValue, 0.2f));
        sequence.AppendCallback(() => food.transform.parent = idTray.transform.Find("FoodHolder").transform); // Set parent to null after door opens
        sequence.Append(food.transform.DOLocalJump(Vector3.zero, 2f, 1, 0.2f).SetEase(Ease.OutFlash));
        sequence.Append(ovenDoor.transform.DOLocalMoveZ(ovenDoorCloseValue, 0.2f));
        sequence.AppendCallback(() => { Matcher(); });



        /*        sequence.Append(ovenDoor.transform.DOLocalMoveZ(ovenDoorOpenValue, 0.2f));
                sequence.AppendCallback(() => food.transform.parent = plateStackPositions[pn].transform); // Set parent to null after door opens
                sequence.Append(food.transform.DOLocalJump(Vector3.zero, 2f, 1, 0.2f).SetEase(Ease.OutFlash));
                sequence.Append(ovenDoor.transform.DOLocalMoveZ(ovenDoorCloseValue, 0.2f));
                sequence.AppendCallback(() => { Matcher(); });
        */
        yield return sequence.WaitForCompletion();

    }


    IEnumerator MoveThePlatesTimer(float time)
    {
        canShopEscMove = true;
        yield return new WaitForSeconds(time);
        isCollectingFoodPlaceBusy = false;
        InputManager.Instance.InputActivator();

        //canShopEscMove = false;
        // Matcher();
    }


    /// <summary>
    /// ////////////////////////
    /// </summary>
    /// <returns></returns>

    void collectStacks()
    {
        stackTotal = new List<GameObject>();
        foreach (Transform child in stackParent.transform)
        {
            if (child.gameObject != null)
            {
                stackTotal.Add(child.gameObject);
            }
        }
    }


    public void CallStackCheckIfRemoved(GameObject stack)
    {
        stack.transform.parent = null;
        //stack.GetComponent<StackHandler>().StackIsNotAvailable();
        collectStacks();
    }


    //gives the player a stack if none exit do whateve u want to end the level or maybe fail the level in else condition
    public void LocationGiver(GameObject player)
    {
        List<GameObject> g = av();
        if (g.Count > 0)
        {
            var selectedStack = g[UnityEngine.Random.Range(0, g.Count)];
            StackHandler sh = selectedStack.GetComponent<StackHandler>();
            sh.AddPlayerToStack(player);
            player.GetComponent<Player>().MoveToStackOrShop(selectedStack, shopFrontStanding);

        }
        else
        {
            //Debug.LogError("No space left GameOver");
            UiManager.Instance.openGameOverPanel();
        }
    }



    public void Matcher()
    {


        if (stackPlayers.Count > 0)
        {

            for (int i = stackTotal.Count - 1; i >= 0; i--)

            {
                var player = stackTotal[i].GetComponent<StackHandler>().playerOnMe;

                if (player != null && player.GetComponent<Player>().foodAssigned == false)
                {


                    var playerType = player.GetComponent<Player>().myType;
                    var playerCol = player.GetComponent<Player>().myColor;
                    //foreach (var food in foodsAvailableToCollect)
                    if (foodsAvailableToCollect.Count > 0)
                    {
                        for (int j = 0; j < foodsAvailableToCollect.Count; j++)
                        {
                            if (foodsAvailableToCollect[j] != null)
                            {
                                var food = foodsAvailableToCollect[j];
                                // Debug.LogError(food.name);
                                var foodType = food.GetComponent<Food>().myFoodType;
                                var trayCol = food.GetComponent<Food>().trayColor;
                                if (playerType == foodType && playerCol == trayCol)
                                {
                                    playerToCollectFood.Enqueue(player);
                                    player.GetComponent<Player>().myFood = food;
                                    player.GetComponent<Player>().foodAssigned = true;
                                    //foodToCollect.Enqueue(food);
                                    foodsAvailableToCollect.Remove(food); // Remove the food from the available list
                                    CollectFoodFromQueue();
                                    break; // Break out of the inner loop
                                }
                            }
                        }
                    }
                }


            }
        }



        CollectFoodFromQueue();
    }







    bool isCollectingFoodPlaceBusy = true;
    void CollectFoodFromQueue()
    {
        // Debug.LogError(isCollectingFoodPlaceBusy);
        if (playerToCollectFood.Count > 0 && !isCollectingFoodPlaceBusy)
        {
            isCollectingFoodPlaceBusy = true;
            var player = playerToCollectFood.Dequeue();
            player.transform.LookAt(shopFrontStanding.transform.position);
            if (player.TryGetComponent<Player>(out Player p))
            {
                // player.transform.DOKill(true);
                p.shouldPlayerGoTostack = false;

                player.GetComponent<Player>().canPlayerBeIdle = false;
                var food = player.GetComponent<Player>().myFood;
                var foodPlateNumber = food.GetComponent<Food>().foodPlateNumber;
                Sequence sequence = DOTween.Sequence();
                nextPlateNum.Enqueue(foodPlateNumber);

                //plateNumber = foodPlateNumber;

                sequence.Append(player.transform.DOMove(shopFrontStanding.transform.position, 0.8f).OnStart(() =>
                {
                    //Quaternion targetRotation = Quaternion.LookRotation(shopFrontStanding.transform.position - transform.position);
                    //player.transform.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.Linear);
                    player.GetComponent<Animator>().SetBool("run2", true);
                    player.GetComponent<Animator>().SetBool("idle2", false);
                    player.GetComponent<Player>().FreeMyStack();
                    // Matcher();

                }).SetEase(Ease.Linear).OnComplete(() =>
                 {
                     player.GetComponent<Animator>().SetBool("run2", false);
                     player.GetComponent<Animator>().SetBool("idle2", false);
                     // Debug.LogError("shop reahed carry");
                     player.GetComponent<Animator>().SetTrigger("carry");
                     GameObject foodHolder = player.transform.Find("FoodHolder").gameObject;
                     var foodParentTransform = food.transform.parent.parent;
                     foodParentTransform.transform.DOJump(foodHolder.transform.position, 2f, 1, 0.5f).OnComplete(() =>
                     {
                         //Debug.LogError("calling check food stack " + nextFoodStack);
                         StartCoroutine(CheckForNewFoodTime());

                         foodParentTransform.transform.parent = foodHolder.transform;
                         player.transform.DOMove(exitDoor.transform.position, 10f).OnStart(() =>
                         {
                             GameObject canvas = player.transform.Find("CanvasForOrderImage").gameObject;
                             Destroy(canvas);
                             availablePlayers.Remove(player);
                             checkLevelDone();
                             // Debug.LogError("exiting");
                             player.GetComponent<Animator>().SetTrigger("carryAndWalk");
                             StartCoroutine(WaitBeforeFoodPlaceIsFree());
                             Quaternion targetRotation = Quaternion.LookRotation(exitDoor.transform.position - player.transform.position);
                             player.transform.DORotateQuaternion(targetRotation, 0.2f).SetEase(Ease.Linear);
                             //if (playerToCollectFood.Count > 0) { StartCoroutine(CustomMatcherCall()); }
                             isCollectingFoodPlaceBusy = false;
                             Matcher();
                         }).OnComplete(() =>
                         {
                             Destroy(player);

                         });
                     });

                 }));
            }
            // else { isCollectingFoodPlaceBusy = false; }

        }
    }

    int nextFoodStack = 3;
    bool isCheckForNewFoodsAvailbale = true;
    IEnumerator CheckForNewFoodTime()
    {

        if (nextFoodStack > 0)
        {

            nextFoodStack--;
            //Debug.LogError("Minus done stack " + nextFoodStack); 

        }
        if (nextFoodStack <= 0 && isCheckForNewFoodsAvailbale)
        {
            isCheckForNewFoodsAvailbale = false;
            // isCollectingFoodPlaceBusy = true;
            yield return MoveThePlatesTimer(escMovDuration);
            for (global::System.Int32 i = 0; i < 3; i++)
            {

                if (cannotCollect.Count > 0)
                {
                    foodsAvailableToCollect.Add(cannotCollect.Dequeue());
                }
                else { Debug.Log("No cc"); }
            }
            Matcher();
            //Debug.LogError("new foods");
            //isCollectingFoodPlaceBusy = false;
            StartCoroutine(PutFoodsInPlateForFirstThree());
            nextFoodStack = 3;
            isCheckForNewFoodsAvailbale = true;
        }
        yield return null;
    }




    IEnumerator WaitBeforeFoodPlaceIsFree()
    {
        yield return new WaitForSeconds(0.5f);
        isCollectingFoodPlaceBusy = false;
    }

    //bool isCustommatcherRunning = false;
    /*    public IEnumerator CustomMatcherCall()
        {

            while (true)
            {
                //CollectFoodFromQueue();
                Matcher();

                yield return new WaitForSeconds(1f);
            }
        }*/




    void checkLevelDone()
    {
        if (availablePlayers.Count <= 0)
        {
            UiManager.Instance.openGameDonerPanel();
        }

    }





    //dont remember but thats one important method XD
    private List<GameObject> av()
    {
        List<GameObject> gList = new List<GameObject>();

        for (int i = 0; i < stackTotal.Count; i++)
        {
            bool t = stackTotal[i].GetComponent<StackHandler>().amIfreeStack;
            if (t) { gList.Add(stackTotal[i]); }

        }
        return gList;
    }




    void ObjectInitializer()

    {

        foreach (Transform child in spawnParent.transform)

        {

            GameObject g = child.gameObject;

            var script = g.GetComponent<SpawnHandler>();

            if (script != null)

            {

                var type = script.type;
                var colorType = script.colortype;
                // ColorSelector(colorType);
                GameObject selectedPlayer = null;
                //foodTypeSelector(type);

                Sprite sp = GetSpriteFromFoodData(type);
                foreach (GameObject player in playersPrefab)
                {
                    colorType playerColor = player.GetComponent<Player>().myColor;
                    if (playerColor == colorType)
                    {
                        selectedPlayer = player.gameObject;
                        break;
                    }

                }

                script.Ins(selectedPlayer, /*color,*/ type, sp);

            }
            else
            {
                Debug.LogError("Spawn script not found in " + g.name);
            }

        }


    }


   public UnityEngine.Color ColorSelector(colorType colorType)
    {
        switch (colorType)
        {
            case colorType.Blue:
                return new UnityEngine.Color(0.04705883f, 0.6627451f, 0.9450981f); // Sky blue color
            case colorType.Green:
                return UnityEngine.Color.green;
            case colorType.Pink:
                return new UnityEngine.Color(1f, 0f, 0.7f);
            case colorType.Purple:
                return new UnityEngine.Color(0.5f, 0f, 0.5f);
            case colorType.Red:
                return UnityEngine.Color.red;
            case colorType.Yellow:
                return UnityEngine.Color.yellow;
            default:
                return UnityEngine.Color.white; // Return a default color if none of the above cases match
        }
    }


    //used differently for selecting player color for more ligher versions than color selector
    public UnityEngine.Color PlayerOutlineColorSelector(colorType colorType)
    {
        switch (colorType)
        {
            case colorType.Blue:
                return new UnityEngine.Color(0.5f, 0.8f, 1f); // Light blue
            case colorType.Green:
                return new UnityEngine.Color(0.5f, 1f, 0.5f); // Light green
            case colorType.Pink:
                return new UnityEngine.Color(1f, 0.7f, 0.8f); // Pastel pink
            case colorType.Purple:
                return new UnityEngine.Color(0.8f, 0.5f, 0.8f); // Light purple
            case colorType.Red:
                return new UnityEngine.Color(1f, 0.5f, 0.5f); // Light red
            case colorType.Yellow:
                return new UnityEngine.Color(1f, 1f, 0.5f); // Light yellow
            default:
                return UnityEngine.Color.white; // Return a default color if none of the above cases match
        }
    }


    Sprite GetSpriteFromFoodData(foodType type)

    {

        foreach (var foodData in FoodList)

        {

            if (foodData.type == type)

            {

                return foodData.sprite;

            }

        }

        return null; // or some default sprite

    }






    //after start func wait for a frame then selects targets and check how many stacks are there
    IEnumerator wait()
    {
        yield return new WaitForEndOfFrame();
        collectStacks();
        EqualityCheckforFoodsAndPlayers();
    }


    void EqualityCheckforFoodsAndPlayers()
    {
        // Check if the number of food orders and players are equal
        int orderCount = 0;
        foreach (GameObject food in orderSerial)
        {
            orderCount++;
        }
        if (orderCount != availablePlayers.Count)
        {
            Debug.LogError($"Number of food orders ({orderCount}) does not match the number of players orders in tiles ({availablePlayers.Count})");
            ExitGamePlayMode();
        }

        // Check if the number of food types and players that will collect that type of foods are equal
        Dictionary<(foodType, colorType), int> foodTypeAndColorCounts = new Dictionary<(foodType, colorType), int>();
        foreach (GameObject food in orderSerial)
        {
            foodType foodType = food.GetComponent<Food>().myFoodType;
            colorType trayColor = food.GetComponent<Food>().trayColor;
            var key = (foodType, trayColor);
            if (foodTypeAndColorCounts.ContainsKey(key))
            {
                foodTypeAndColorCounts[key]++;
            }
            else
            {
                foodTypeAndColorCounts[key] = 1;
            }
        }

        foreach (GameObject player in availablePlayers)
        {
            foodType playerType = player.GetComponent<Player>().myType;
            colorType playerColor = player.GetComponent<Player>().myColor;
            var key = (playerType, playerColor);
            if (foodTypeAndColorCounts.ContainsKey(key))
            {
                foodTypeAndColorCounts[key]--;
            }
            else
            {
                Debug.LogError($"No food of type {playerType} with tray color {playerColor} found in the food serial");
                ExitGamePlayMode();
            }
        }

        foreach (KeyValuePair<(foodType, colorType), int> pair in foodTypeAndColorCounts)
        {
            if (pair.Value != 0)
            {
                Debug.LogError($"Food type {pair.Key.Item1} with tray color {pair.Key.Item2} has {pair.Value} extra/missing foods in the serial");
                ExitGamePlayMode();
            }
        }
    }

    void ExitGamePlayMode()
    {
        // Your code to exit the game play mode, e.g.
        //Time.timeScale = 0;
        UiManager.Instance.openGameOverPanel();
        // or any other way to exit the game play mode
    }



}


public enum foodType { Brurger, Pizza, Hot_Dog, Green_Ice_Cream, Fries }
public enum colorType { Blue, Green, Pink, Purple, Red, Yellow }