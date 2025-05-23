using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyDetect : MonoBehaviour
{
    [SerializeField] private Color gizmosColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame 
    void Update()
    {
        
    }


    /*    private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            Gizmos.matrix = transform.localToWorldMatrix; //that's for the box rotation
            Gizmos.DrawWireCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
        }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        foreach (BoxCollider boxCollider in GetComponentsInChildren<BoxCollider>())
        {
            Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "spawn")
        {
            GetComponentInParent<SpawnHandler>().connectedTilesList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "spawn")
        {
            GetComponentInParent<SpawnHandler>().connectedTilesList.Remove(other.gameObject);
        }
    }

}
