using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Player : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.playerObj.transform.position = gameObject.transform.position;
        gameObject.SetActive(false);
    }
}