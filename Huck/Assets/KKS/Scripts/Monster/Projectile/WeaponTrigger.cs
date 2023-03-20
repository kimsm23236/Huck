using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    //! 콜라이더 트리거
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log($"{other.tag} 맞춤!");
            gameObject.SetActive(false);
        }
    } // OnTriggerEnter
}
