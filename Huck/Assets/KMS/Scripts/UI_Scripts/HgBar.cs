using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HgBar : MonoBehaviour
{
    private Image hgBar = default;

    private void Start()
    {
        hgBar = GetComponent<Image>();
    }

    // { Set rate Currnt Hungry = 100 * fillAmount 
    private void Update()
    {
        hgBar.fillAmount = PlayerStat.curHungry / 100;
    }
    // } Set rate Currnt Hungry = 100 * fillAmount 
}
