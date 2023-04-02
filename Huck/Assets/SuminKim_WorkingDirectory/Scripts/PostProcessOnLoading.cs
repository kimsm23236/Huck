using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessOnLoading : MonoBehaviour
{
    public virtual void Execute()
    {
        /* virtual method */
    }

    public virtual IEnumerator AsyncLoadingPostProcess()
    {
        yield return null;
    }
}
