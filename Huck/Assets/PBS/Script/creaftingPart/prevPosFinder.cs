using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prevPosFinder : MonoBehaviour
{
    private GameObject objCenter;

    private void Awake()
    {
        objCenter = gameObject.FindChildObj("floorPoint").FindChildObj("Center");
    }

    public Vector3 FindCenter()
    {
        return objCenter.transform.position;
    }
    public Vector3 FindDirToName(string name)
    {
        GameObject resultDir = gameObject.FindChildObj("floorPoint").FindChildObj(name);

        Vector3 tempA = resultDir.transform.position; tempA.y = 0;
        Vector3 tempB = objCenter.transform.position; tempB.y = 0;

        Vector3 Dir = tempA - tempB;
        return Dir.normalized;
    }
}
