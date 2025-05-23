using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class Player : MonoBehaviour
{
    public GameObject myCurrentGround;
    public foodType myType;
    public colorType myColor;

    private GameObject myStack;
    private GameObject shopFrontTarget;
    [SerializeField] private GameManager myGameManager;
    [SerializeField] Image myOrderImage;
    public Sprite foodSprite;
    public bool amIInteractable = true;
    public bool foodAssigned = false;
    public GameObject myFood;
    public bool canPlayerBeIdle = true;
    public List<GameObject> dependencies = new List<GameObject>();
    public GameObject playerCanvas;
    public bool shouldPlayerGoTostack =true;

    // Start is called before the first frame update
    void Start()
    {
        myGameManager = GameObject.FindAnyObjectByType<GameManager>();
        myGameManager.availablePlayers.Add(gameObject);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.renderQueue = 2001;


    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartPathFinder()
    {
        PathFinder.Instance.search(gameObject, myCurrentGround);
    }

    public IEnumerator SetPlayerOrderImage()
    {
        playerCanvas = GetComponentInChildren<Canvas>().gameObject;
        myOrderImage = playerCanvas.GetComponentInChildren<Image>();
        yield return new WaitForEndOfFrame();
        myOrderImage.sprite = foodSprite;
    }

    public void StartNoPathImage()
    {
        StartCoroutine(NoPathReact());
    }
     IEnumerator NoPathReact()
    {
        Sprite tmp =myOrderImage.sprite;
        myOrderImage.sprite = GameManager.Instance.crossImage;
        yield return new WaitForSeconds(InputManager.Instance.inputLatency / 1.1f);
        myOrderImage.sprite = tmp;
    }



    public void MoveToStackOrShop(GameObject selectedStack, GameObject shopFront)
    {
        NotifyDependeciesThatTheyHaveAPathToIncreaseOutline();
        myStack = selectedStack;
        shopFrontTarget = shopFront;
        Vector3 target = myStack.transform.position;
        target.y = transform.position.y;
        myGameManager.Matcher();
        if (shouldPlayerGoTostack)
        {
            MovetoStack(target);
        }
        //transform.LookAt(myStack.transform.position);


    }

    void MovetoStack(Vector3 target)
    {
        transform.DOMove(target, 0.8f).SetEase(Ease.Linear).OnStart(() =>
        {
            gameObject.GetComponent<Animator>().SetBool("run2", true);
            gameObject.GetComponent<Animator>().SetBool("idle2", false);
            Quaternion targetRotation = Quaternion.LookRotation(myStack.transform.position - transform.position);
            transform.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.Linear);
        }).OnComplete(OnReachingStack);

    }

    public void NotifyDependeciesThatTheyHaveAPathToIncreaseOutline()
    {
        if (dependencies.Count > 0)
        {
            foreach (var spawnTile in dependencies)
            {
                var player = spawnTile.GetComponent<SpawnHandler>().playerOnMe;
                if (player != null)
                {
                    player.GetComponent<Player>().IncreasePlayerOutLine();

                }
            }
        }


    }

    private void OnReachingStack()
    {
        //Debug.LogError("reached stack idle" + gameObject.name);

        //gameObject.GetComponent<Animator>().SetTrigger("idle");

        Quaternion targetRotation = Quaternion.LookRotation(shopFrontTarget.transform.position - transform.position);
        transform.DORotateQuaternion(targetRotation, 0.2f).SetEase(Ease.Linear).OnStart(() =>
        {

        });
        myGameManager.Matcher();
        if (canPlayerBeIdle)
        {
            gameObject.GetComponent<Animator>().SetBool("run2", false);
            gameObject.GetComponent<Animator>().SetBool("idle2", true);
        }

    }

    public void FreeMyStack()
    {
        myStack.GetComponent<StackHandler>().amIfreeStack = true;
        myStack.GetComponent<StackHandler>().RemovePlayerFromStack(gameObject);
    }



public void IncreasePlayerOutLine()
{
    var outline = gameObject.GetComponent<Outline>();
        outline.OutlineColor = Color.black;
    DOTween.To(() => outline.OutlineWidth, x => outline.OutlineWidth = x, 3f, 0.3f);
}

    public void PlayerOutlineColorChanger()
    {
        Color c = GameManager.Instance.PlayerOutlineColorSelector(myColor);
        gameObject.TryGetComponent<Outline>(out Outline ol);
        if (ol != null)
        {
            var outline = gameObject.GetComponent<Outline>();
            outline.OutlineColor = c;
        }
    }

}
