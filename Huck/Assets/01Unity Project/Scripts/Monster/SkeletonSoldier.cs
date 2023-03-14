using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoldier : Monster
{
    private MonsterController mController = default;
    public MonsterData monsterData;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.NOMAL, monsterData);
        mController.monster = this;
    } // Awake

    private void OnDamage()
    {

    }

    public void LongDistanceAttack()
    {
        StartCoroutine(AttackC());
    }

    private IEnumerator AttackC()
    {
        Debug.Log("AttackC 코루틴 시작");
        mController.monsterAni.SetBool("isAttackC_Start", false);
        mController.monsterAni.SetBool("isAttackC_Loop", true);
        bool isAttackC = false;
        while (isAttackC == false)
        {
            Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            mController.transform.position += dir * (moveSpeed * 1.5f) * Time.deltaTime;
            if (mController.distance <= attackRange)
            {
                mController.monsterAni.SetBool("isAttackC_Loop", false);
                mController.monsterAni.SetBool("isAttackC_End", true);
                isAttackC = true;
            }
            yield return null;
        }
    }
}
