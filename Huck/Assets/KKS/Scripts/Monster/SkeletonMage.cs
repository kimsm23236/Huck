using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Monster;

public class SkeletonMage : Monster
{
    private MonsterController mController = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    public MonsterData monsterData;
    public GameObject attackA_Effect;
    private void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.RANGE, monsterData);
        mController.monster = this;
    } // Awake

    //! 해골마법사 공격 오버라이드
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (mController.distance > meleeAttackRange)
        {
            mController.monsterAni.SetBool("isAttackB", true);
            StartCoroutine(LookTarget());
        }
        else
        {
            mController.monsterAni.SetBool("isAttackA", true);
        }
    } // Attack

    //! 해골마법사 스킬 오버라이드
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (useSkillA == true)
        {
            SkillA();
            return;
        }

        if (useSkillB == true)
        {
            SkillB();
            return;
        }
    } // Skill

    //! 공격종료 이벤트함수
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isSkillB", false);
        // 공격종료 후 딜레이 시작
        StartCoroutine(AttackDelay(mController, 4));
    } // ExitAttack

    //! 스킬A 함수
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(LookTarget());
        StartCoroutine(SkillACooldown());
    } // SkillA

    private void SkillB()
    {
        mController.monsterAni.SetBool("isSkillB", true);
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! 스킬A 쿨다운 코루틴함수
    private IEnumerator SkillACooldown()
    {
        useSkillA = false;
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 쿨 적용
        while (true)
        {
            skillACool += Time.deltaTime;
            if (skillACool >= skillA_MaxCool)
            {
                skillACool = 0f;
                useSkillA = true;
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! 스킬B 쿨다운 코루틴함수
    private IEnumerator SkillBCooldown()
    {
        useSkillB = false;
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 쿨 적용
        while (true)
        {
            skillBCool += Time.deltaTime;
            if (skillBCool >= skillB_MaxCool)
            {
                skillBCool = 0f;
                useSkillB = true;
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! 타겟을 바라보는 코루틴함수
    private IEnumerator LookTarget()
    {
        bool isLookAt = true;
        while (isLookAt == true)
        {
            if (mController.enumState != MonsterController.MonsterState.SKILL
                && mController.enumState != MonsterController.MonsterState.ATTACK)
            {
                isLookAt = false;
                yield break;
            }
            Vector3 dir = (mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            yield return null;
        }
    } // LookTarget
} // SkeletonMage
