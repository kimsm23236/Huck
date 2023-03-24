using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private Image hpBar = default;

    private void Start()
    {
        hpBar = GetComponent<Image>();
    }

    // {} Set rate Currnt Hp = 100 * fillAmount 
    private void Update()
    {
        hpBar.fillAmount = PlayerStat.curHp / 100f;
    }
    // } Set rate Currnt Hp = 100 * fillAmount 
}
