using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    private const string BUILD_LAYER = GData.BUILD_MASK;
    public bool IsOnCollider = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER))
        {
            IsOnCollider = false;
        }
        else
        {
            IsOnCollider = true;
        }
    }
}
