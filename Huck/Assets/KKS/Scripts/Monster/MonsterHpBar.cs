using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHpBar : MonoBehaviour
{
    private Transform cameraT = default;
    [SerializeField] Image hpFront = default;
    [SerializeField] MonsterController mController = default;
    // Start is called before the first frame update
    void Start()
    {
        cameraT = Camera.main.transform;
    } // Start

    // Update is called once per frame
    void Update()
    {
        // Hpbar가 메인카메라 정면을 바라보도록 설정
        transform.LookAt(transform.position + cameraT.rotation * Vector3.forward, cameraT.rotation * Vector3.up);
        hpFront.fillAmount = (float)mController.monster.monsterHp / (float)mController.monster.monsterMaxHp;
    } // Update
}
