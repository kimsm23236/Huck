using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKing : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private bool useSkillB = default;
    [SerializeField] private float skillA_MaxCool = default;
    [SerializeField] private float skillB_MaxCool = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! 공격 처리 이벤트함수 (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! 공격종료 이벤트함수
    private void ExitAttack()
    {
        weapon.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isAttackD", false);
        mController.monsterAni.SetBool("isAttackE", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        mController.monsterAni.SetBool("isSkillB", false);
        // 공격종료 후 딜레이 상태로 전환
        mController.isDelay = true;
    } // ExitAttack

    //! 해골왕 공격 오버라이드
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // 모션 5개 중 랜덤으로 한개 실행
        if (mController.distance <= meleeAttackRange)
        {
            int number = Random.Range(0, 11);
            if (number > 8)
            {
                mController.monsterAni.SetBool("isAttackE", true);
            }
            else if (number > 6)
            {
                mController.monsterAni.SetBool("isAttackD", true);
            }
            else if (number > 4)
            {
                mController.monsterAni.SetBool("isAttackC", true);
                return;
            }
            else if (number > 2)
            {
                mController.monsterAni.SetBool("isAttackB", true);
                return;
            }
            else
            {
                mController.monsterAni.SetBool("isAttackA", true);
            }
        }
    } // Attack

    //! 해골왕 스킬 오버라이드
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (useSkillA == true)
        {
            useSkillA = false;
            SkillA();
            CheckUseSkill();
            return;
        }
        if (useSkillB == true && mController.distance >= 13f)
        {
            useSkillB = false;
            SkillB();
            CheckUseSkill();
            return;
        }
        else if (useSkillB == true && mController.distance < 13f)
        {
            useSkillB = false;
            // 돌진스킬이 사용가능하지만 타겟이 최소사거리 안에 있을때 돌진스킬 사용X Idle상태로 초기화
            StartCoroutine(CheckSkillBDistance());
            CheckUseSkill();
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }
    } // Skill

    //! 사용가능한 스킬이 있는지 체크하는 함수 (몬스터컨트롤러에서 상태진입 체크하기 위함)
    private void CheckUseSkill()
    {
        if (useSkillA == false && useSkillB == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

    //! 스킬A 돌진 사용 거리체크하는 코루틴함수
    private IEnumerator CheckSkillBDistance()
    {
        isNoRangeSkill = true;
        while (isNoRangeSkill == true)
        {
            // 타겟이 돌진 최소사거리 밖에 있으면 돌진 사용가능
            if (mController.distance >= 13f)
            {
                useSkillB = true;
                isNoRangeSkill = false;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckSkillBDistance

    //! 해골왕 스킬A 함수 (소환 스킬)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! 스킬A 공격 코루틴함수
    private IEnumerator UseSkillA()
    {
        StartCoroutine(SkillACooldown());
        mController.monsterAni.SetBool("isSkillA_Start", true);
        bool isStart = true;
        bool isSkillA = false;
        float timeCheck = 0f;
        while (isSkillA == false)
        {
            timeCheck += Time.deltaTime;
            if (mController.monsterAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && isStart == true)
            {
                // 소환 준비 모션 끝나면 소환 시작
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", true);
                isStart = false;
            }
            // 5초가 지나면 소환 마무리 시작
            if (isStart == false && timeCheck >= 5f)
            {
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = true;
            }
            yield return null;
        }
    } // UseSkillA

    //! 해골왕 스킬B 함수 (도약 공격)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! 스킬B (도약 공격) 코루틴함수
    private IEnumerator UseSkillB()
    {
        // 포물선 이동함수를 사용하기 위한 Parabola 초기화
        Parabola parabola = new Parabola();
        // 몬스터가 타겟을 바라보는 방향의 반대방향을 구함
        Vector3 dir = -(mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        // 목표위치를 dir방향으로 meleeAttackRange만큼 이동된 좌표로 설정
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * meleeAttackRange;
        mController.monsterAni.SetBool("isSkillB", true);
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(parabola.ParabolaMoveToTarget(mController.transform.position, targetPos, 1f, gameObject));
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length - 0.8f);
        mController.monsterAni.SetBool("isSkillB", false);
        mController.monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        mController.isDelay = true;
    } // UseSkillB



    //! 스킬A 쿨다운 코루틴함수
    private IEnumerator SkillACooldown()
    {
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 쿨 적용
        while (true)
        {
            skillACool += Time.deltaTime;
            if (skillACool >= skillA_MaxCool)
            {
                skillACool = 0f;
                useSkillA = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! 스킬B 쿨다운 코루틴함수
    private IEnumerator SkillBCooldown()
    {
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 쿨 적용
        while (true)
        {
            skillBCool += Time.deltaTime;
            if (skillBCool >= skillB_MaxCool)
            {
                skillBCool = 0f;
                useSkillB = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // SkillBCooldown
} // SkeletonKing
