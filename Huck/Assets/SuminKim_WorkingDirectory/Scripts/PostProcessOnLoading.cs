using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessOnLoading : MonoBehaviour
{
    public virtual void Execute(System.Action<EGenerationStage, string> reportStatusFn = null)
    {
        /* virtual method */
    }

    public virtual IEnumerator AsyncLoadingPostProcess(System.Action<EGenerationStage, string> reportStatusFn = null)
    {
        yield return null;
    }
}
