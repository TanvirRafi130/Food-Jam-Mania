using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [SerializeField] float raycaseDistance = 100f;
    [SerializeField] public float inputLatency = 0.3f;
    bool canClick =true;

    private static InputManager _instance;

    public static InputManager Instance => _instance;


    private void Awake()
    {
        _instance = this;

    }



// Update is called once per frame
void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, raycaseDistance) && canClick)
            {
                canClick = false;
                // Debug.LogError(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Player")
                {
                    Player playerSc = hit.transform.gameObject.GetComponent<Player>();

                    if (playerSc.amIInteractable == true )
                    {
                        playerSc.StartPathFinder();
                       
                    }
                }
                StartCoroutine(InputLatencyActivator());
            }
        }
    }


    IEnumerator InputLatencyActivator()
    {
        yield return new WaitForSeconds(inputLatency);
        canClick = true;
    }  
    public void InputActivator()
    {
        canClick = true;
    }
}
