using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPositioning : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPos = Camera.main.transform.position - transform.position;
        lookPos.x = 0; // Ensure the rotation is always 0 on the Y axis
        Quaternion rotation = Quaternion.LookRotation(lookPos, Vector3.up);
        transform.rotation = rotation;
    }
}