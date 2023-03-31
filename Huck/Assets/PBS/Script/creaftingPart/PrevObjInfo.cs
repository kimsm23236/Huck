using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevObjInfo : MonoBehaviour
{
    private buildType CheckBuild;
    public bool isBuildAble = false;
    private List<GameObject> cols = new List<GameObject>();

    void Update()
    {
        changeCursor();
    }

    public void SetLayerType(buildType input)
    {
        CheckBuild = input;
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
        if (other.gameObject.layer == LayerMask.NameToLayer(GData.BUILD_MASK) ||
            other.gameObject.layer == LayerMask.NameToLayer("BuildItem"))
        {
            cols.Add(other.gameObject);
        }

        // if (CheckBuild == buildType.Anvil || CheckBuild == buildType.Stove || CheckBuild == buildType.Workbench)
        // {
        //     if (other.gameObject.layer == LayerMask.NameToLayer(GData.BUILD_MASK) ||
        //     other.gameObject.layer == LayerMask.NameToLayer("BuildItem"))
        //     {
        //         cols.Add(other.gameObject);
        //     }
        // }
        // else
        // {
        //     if (other.gameObject.layer == LayerMask.NameToLayer(GData.BUILD_MASK))
        //     {
        //         cols.Add(other.gameObject);
        //     }
        // }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer(GData.BUILD_MASK) ||
            other.gameObject.layer == LayerMask.NameToLayer("BuildItem"))
        {
            cols.Remove(other.gameObject);
        }

        // if (CheckBuild == buildType.Anvil || CheckBuild == buildType.Stove || CheckBuild == buildType.Workbench)
        // {
        //     if (other.gameObject.layer == LayerMask.NameToLayer(GData.BUILD_MASK) ||
        //     other.gameObject.layer == LayerMask.NameToLayer("BuildItem"))
        //     {
        //         cols.Remove(other.gameObject);
        //     }
        // }
        // else
        // {
        //     if (other.gameObject.layer == LayerMask.NameToLayer(GData.BUILD_MASK))
        //     {
        //         cols.Remove(other.gameObject);
        //     }
        // }
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