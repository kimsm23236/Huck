using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResObjectDestroying : PostProcessOnLoading
{
    [SerializeField]
    bool isActive = false;
    public override void Execute(System.Action<EGenerationStage, string> reportStatusFn = null)
    {
        enabled = true;
        StartCoroutine(AsyncLoadingPostProcess(reportStatusFn));
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

    public override IEnumerator AsyncLoadingPostProcess(System.Action<EGenerationStage, string> reportStatusFn = null)
    {
        if (reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.PostProcessOnLoading, "Overlapped ResourceObject Destroying");
        
        yield return new WaitForSeconds(5f);
        Destroy(gameObject, 1f);
    }
}
