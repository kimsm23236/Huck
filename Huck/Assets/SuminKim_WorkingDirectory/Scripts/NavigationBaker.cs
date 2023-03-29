using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface[] surfaces;

    // Use this for initialization
    void Start()
    {
        
    }
    public void BakeNavMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

}
