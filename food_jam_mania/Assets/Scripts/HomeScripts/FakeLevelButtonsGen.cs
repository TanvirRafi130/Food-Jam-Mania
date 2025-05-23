using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeLevelButtonsGen : MonoBehaviour
{
    List<GameObject> doneCloning = new List<GameObject>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!doneCloning.Contains(collision.gameObject))
        {
            HomeManager.Instance.PopulateLevelsPanelWithFakeLevelData();
            doneCloning.Add(collision.gameObject);
        }
    }
}