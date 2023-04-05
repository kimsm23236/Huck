using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHpBar : MonoBehaviour
{
    private Transform cameraT = default;
    [SerializeField] MonsterController mController = default;
    [SerializeField] Image hpFront = default;
    private float maxHp = default;
    // Start is called before the first frame update
    void Start()
    {
        cameraT = Camera.main.transform;
        maxHp = mController.monster.monsterMaxHp;
    } // Start

    // Update is called once per frame
    void Update()
    {
        // Hpbar가 메인카메라 정면을 바라보도록 설정
        if (mController.monster.monsterType != Monster.MonsterType.BOSS)
        {
            transform.LookAt(transform.position + cameraT.rotation * Vector3.forward, cameraT.rotation * Vector3.up);
        }
    } // Update

    public void InitHpBar(float hp)
    {
        hpFront.fillAmount = hp / maxHp;
    } // InitHpBar
} // MonsterHpBar
