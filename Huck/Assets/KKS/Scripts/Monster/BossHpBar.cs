using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHpBar : MonoBehaviour
{
    [SerializeField] Image hpFront = default;
    [SerializeField] MonsterController mController = default;

    // Update is called once per frame
    void Update()
    {
        hpFront.fillAmount = (float)mController.monster.monsterHp / (float)mController.monster.monsterMaxHp;
    }
} // BossHpBar
