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

    //     private const string BUILD_LAYER = GData.BUILD_MASK;
    // public bool IsOnCollider = false;
    // private List<GameObject> cols = new List<GameObject>();

    // void Update()
    // {
    //     changeCursor();
    // }

    // private void changeCursor()
    // {
    //     if (cols.Count > 0)
    //     {
    //         IsOnCollider = false;
    //     }
    //     else
    //     {
    //         IsOnCollider = true;
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER))
    //     {
    //         cols.Add(other.gameObject);
    //     }
    // }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (cols.Count > 0)
    //     {
    //         if (other == null)
    //         {
    //             Debug.Log("체크");
    //             cols.Remove(other.gameObject);
    //         }
    //     }
    // }


    // private const string BUILD_LAYER = GData.BUILD_MASK;
    // public bool IsOnCollider;

    // private void Awake()
    // {
    //     IsOnCollider = true;
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER))
    //     {
    //         IsOnCollider = false;
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER))
    //     {
    //         IsOnCollider = false;
    //     }
    // }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (!IsOnCollider)
    //     {
    //         if (other == null)
    //         {
    //             IsOnCollider = true;
    //         }
    //     }
    // }
}
