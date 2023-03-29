using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevObjInfo : MonoBehaviour
{
    private const string BUILD_LAYER = GData.BUILD_MASK;
    public bool isBuildAble = false;
    private List<GameObject> cols = new List<GameObject>();

    void Update()
    {
        changeCursor();
    }

    private void changeCursor()
    {
        if (cols.Count > 0)
        {
            isBuildAble = false;
        }
        else
        {
            isBuildAble = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER))
        {
            cols.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER))
        {
            cols.Remove(other.gameObject);
        }
    }

    public void deleteObjTime()
    {
        if (cols.Count > 0)
        {
            for (int i = 0; i < cols.Count; i++)
            {
                cols.RemoveAt(0);
            }
        }
    }
}