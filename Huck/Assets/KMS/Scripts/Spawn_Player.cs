using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Player : MonoBehaviour
{
    private void Start()
    {
        // Move to Spawn Point
        Debug.Log("Player Position Setup");
        Debug.Log($"Time Scale : {Time.timeScale}");
        GameManager.Instance.playerObj.transform.position = gameObject.transform.position;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}