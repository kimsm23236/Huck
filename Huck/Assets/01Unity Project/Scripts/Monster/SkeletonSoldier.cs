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
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! 공격 데미지 처리 함수
    private void OnDamage()
    {

    } // OnDamage

    //! 원거리 공격 함수
    private void LongDistanceAttack()
    {
        StartCoroutine(AttackC());
    } // LongDistanceAttack

    //! 공격이 끝났을 때 실행하는 이벤트함수
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isAttackC_End", false);
        StartCoroutine(AttackDelay(mController));
    } // ExitAttack

    //! AttackC 돌진 공격 코루틴함수
    private IEnumerator AttackC()
    {
        Debug.Log("AttackC 코루틴 시작");
        // 돌진 준비 모션 끝나면 돌진 시작
        mController.monsterAni.SetBool("isAttackC_Start", false);
        mController.monsterAni.SetBool("isAttackC_Loop", true);
        bool isAttackC = false;
        while (isAttackC == false)
        {
            Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * (moveSpeed * 2f) * Time.deltaTime;
            // 돌진 중 타겟이 근접공격사거리 안이라면 돌진 마무리 시작
            if (mController.distance <= meleeAttackRange)
            {
                mController.monsterAni.SetBool("isAttackC_Loop", false);
                mController.monsterAni.SetBool("isAttackC_End", true);
                isAttackC = true;
            }
            yield return null;
        }
    } // AttackC

    private void SkillA()
    {

    }
}
