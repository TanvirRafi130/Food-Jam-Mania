using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Burst.CompilerServices;

public class PathFinder : MonoBehaviour
{
    private static PathFinder _instance;

    public static PathFinder Instance => _instance;




    // Start is called before the first frame update

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void search(GameObject player, GameObject spawnGround)
    {
        List<GameObject> path = new List<GameObject>();
        // path.Clear();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        bool foundPath = DFS(spawnGround, visited, path);

        if (foundPath)
        {
            //Debug.Log("Path found to top tile");
            MovePlayer(player, path);
            player.GetComponent<Player>().amIInteractable = false;
        }
        else
        {
            // Debug.Log("No path found");
            player.GetComponent<Player>().amIInteractable = true;
            player.GetComponent<Player>().StartNoPathImage();
            //Debug.LogError("Calling Camera shake");
            //CameraShake.Instance.ShakeCamera();
        }
    }

    bool DFS(GameObject current, HashSet<GameObject> visited, List<GameObject> path)
    {
        visited.Add(current);
        SpawnHandler currentScript = current.GetComponent<SpawnHandler>();

        if (currentScript.amITopTile /*&& currentScript.amIfree*/)
        {
            path.Add(current);
            return true;
        }

        foreach (GameObject neighbor in currentScript.connectedTilesList)
        {
            SpawnHandler neighborScript = neighbor.GetComponent<SpawnHandler>();
            if (!visited.Contains(neighbor) && neighborScript.amIfree)
            {
                bool foundPath = DFS(neighbor, visited, path);
                if (foundPath)
                {
                    path.Add(current);
                    return true;
                }
            }
        }

        return false;
    }

    void MovePlayer(GameObject player, List<GameObject> path)
    {
        path.Reverse();
        player.GetComponent<Player>().myCurrentGround.GetComponent<SpawnHandler>().amIfree = true;
        StartCoroutine(MovePlayerCoroutine(player, path));

    }



    IEnumerator MovePlayerCoroutine(GameObject player, List<GameObject> path)
    {
        bool isFirstDone = false;
        bool isRunAnimationCalled = false;
        
        
        foreach (GameObject tile in path)
        {
            if (!isFirstDone)
            {
                isFirstDone = true;

            }
            else
            {
                Sequence sequence = DOTween.Sequence();
                if (!isRunAnimationCalled)
                {
                    CallPlayerRun(player);
                    isRunAnimationCalled = true;
                }
                Vector3 targetPosition = new Vector3(tile.transform.position.x, player.transform.position.y, tile.transform.position.z);

                // player.transform.LookAt(targetPosition);
                Quaternion targetRotation = Quaternion.LookRotation(tile.transform.position - player.transform.position);
                player.transform.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.Linear);


                sequence.Append(player.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.Linear));

                yield return new WaitForSeconds(0.1f);
                // yield return sequence.WaitForCompletion();  // Adjust the delay as needed
            }
        }
        GameManager.Instance.LocationGiver(player);

    }

    void CallPlayerRun(GameObject player)
    {
        //Debug.LogError("pop" + player.name);
        //player.GetComponent<Animator>().SetTrigger("run");
        player.GetComponent<Animator>().SetBool("run2",true);
        player.GetComponent<Animator>().SetBool("idle2", false);
        
    }








}
