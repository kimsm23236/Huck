using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OnNavAgent : PostProcessOnLoading
{
    public override void Execute()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");

        foreach(var animal in animals)
        {
            Debug.Log($"{animal.name} NavMeshAgent Enabled -> true");
            NavMeshAgent navMeshAgent = animal.GetComponent<NavMeshAgent>();
            if(navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }
        }
        Destroy(gameObject, 1f);
    }
}
