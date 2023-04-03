using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResObjectDestroying : PostProcessOnLoading
{
    [SerializeField]
    bool isActive = false;
    public override void Execute()
    {
        enabled = true;
        StartCoroutine(AsyncLoadingPostProcess());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GData.GATHER_MASK)
        {
            Debug.Log($"destroy : {other.gameObject.name}");
            Destroy(other.gameObject);
        }
    }

    private void OnEnable()
    {
        
    }

    public override IEnumerator AsyncLoadingPostProcess()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject, 1f);
    }
}
