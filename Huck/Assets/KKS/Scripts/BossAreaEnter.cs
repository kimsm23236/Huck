using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAreaEnter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GData.PLAYER_MASK)
        {
            GameManager.Instance.BossSpwan();
        }
    } // OnTriggerEnter
}
