using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    //! �ݶ��̴� Ʈ����
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log($"{other.tag} ����!");
            gameObject.SetActive(false);
        }
    } // OnTriggerEnter
}
