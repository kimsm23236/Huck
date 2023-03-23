using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnBar : MonoBehaviour
{
    private Image egBar = default;

    private void Start()
    {
        egBar = GetComponent<Image>();
    }

    // { Set rate Currnt Energy = 100 * fillAmount 
    private void Update()
    {
        egBar.fillAmount = PlayerStat.curEnergy / 100;
    }
    // } Set rate Currnt Energy = 100 * fillAmount 
}
