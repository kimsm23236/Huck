using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Monster;

public class SkeletonGrunt : Monster
{
    private MonsterController mController = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    private float rushCool = 0f;
    public MonsterData monsterData;
    public GameObject weapon;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! 해골그런트 공격 오버라이드
    public override void Attack()
    {
        if (mController.distance > meleeAttackRange)
        {
            StartCoroutine(RushAttack());
        }
        else
        {
            int number = Random.Range(0, 10);
            if (number <= 4)
            {
                mController.monsterAni.SetBool("isAttackA", true);
            }
            else if (number > 4 && number <= 7)
            {
                mController.monsterAni.SetBool("isAttackB", true);
            }
            else
            {
                mController.monsterAni.SetBool("isAttackC", true);
            }
        }
    } // Attack

    //! 해골그런트 스킬 오버라이드
    public override void Skill()
    {
        if (useSkillA == true)
        {
            SkillA();
            Debug.Log($"스킬A 발동 {isNoRangeSkill}");
            return;
        }

        if (useSkillB == true)
        {
            SkillB();
            Debug.Log($"스킬B 발동 {isNoRangeSkill}");
            return;
        }
    } // Skill

    //! 해골그런트 스킬A 함수
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! 스킬A 데미지판정 함수
    private void SkillA_Damage()
    {
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            if (hits[0].collider.tag == "Player")
            {
                Debug.Log("플레이어 맞춤");
            }
        }
    } // SkillA_Damage

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.transform.position, 3f);
    }

    //! 해골그런트 스킬B 함수
    private void SkillB()
    {
        mController.monsterAni.SetBool("isSkillB_Start", true);
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! 공격종료 이벤트함수
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isRushAttack", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isSkillB_End", false);
        // 공격종료 후 딜레이 시작
        StartCoroutine(AttackDelay(mController, 4));
    } // ExitAttack

    //! 돌진 공격 코루틴 함수
    private IEnumerator RushAttack()
    {
        StartCoroutine(RushCooldown());
        mController.monsterAni.SetBool("isRun", true);
        bool isRush = true;
        bool isFinishRush = false;
        float timeCheck = 0f;
        while (isRush == true)
        {
            // if : 마무리 공격 시작 전까지
            if (isFinishRush == false)
            {
                Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
                mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
                mController.transform.position += dir * (moveSpeed * 2f) * Time.deltaTime;
            }
            else
            {
                // 마무리 공격 시작되면 돌진하던 방향 그대로 1초간 돌진
                timeCheck += Time.deltaTime;
                mController.transform.position += transform.forward * (moveSpeed * 1.5f) * Time.deltaTime;
                if (timeCheck >= 1f)
                {
                    isRush = false;
                }
            }

            if (mController.distance <= meleeAttackRange && isFinishRush == false)
            {
                mController.monsterAni.SetBool("isRun", false);
                mController.monsterAni.SetBool("isRushAttack", true);
                isFinishRush = true;
            }
            yield return null;
        }
    } // RushAttack

    //! 스킬A 연계 공격 코루틴함수
    private IEnumerator UseSkillA()
    {
        mController.monsterAni.SetBool("isSkillB_Start", false);
        mController.monsterAni.SetBool("isSkillB_Loop", true);
        yield return new WaitForSeconds(2f);
        mController.monsterAni.SetBool("isSkillB_Loop", false);
        mController.monsterAni.SetBool("isSkillB_End", true);
    } // UseSkillA

    //! 돌진 쿨다운 코루틴함수
    private IEnumerator RushCooldown()
    {
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 쿨 적용
        isNoRangeAttack = true;
        while (true)
        {
            rushCool += Time.deltaTime;
            if (rushCool >= 20f)
            {
                rushCool = 0f;
                isNoRangeAttack = false;
                yield break;
            }
            yield return null;
        }
    } // RushCooldown

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
    } // SkillBCooldown
} // SkeletonGrunt
