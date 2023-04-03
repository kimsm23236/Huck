using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkeletonSoldier : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private GameObject shield = default;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private bool useSkillB = default;
    [SerializeField] private float skillA_MaxCool = default;
    [SerializeField] private float skillB_MaxCool = default;
    private int defaultDamage = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.NOMAL, monsterData);
        mController.monster = this;
        defaultDamage = damage;
        CheckUseSkill();
    } // Awake

    //! 해골병사 공격 오버라이드
    public override void Attack()
    {
        //모션 2개 중 랜덤으로 한개 실행
        int number = Random.Range(0, 10);
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (number <= 6)
        {
            mController.monsterAni.SetBool("isAttackA", true);
        }
        else
        {
            mController.monsterAni.SetBool("isAttackB", true);
        }
    } // Attack

    //! 해골병사 스킬 오버라이드
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);

        if (useSkillA == true && mController.distance >= 13f)
        {
            useSkillA = false;
            CheckUseSkill();
            SkillA();
            return;
        }
        else if (useSkillA == true && mController.distance < 13f)
        {
            useSkillA = false;
            CheckUseSkill();
            // 돌진스킬이 사용가능하지만 타겟이 최소사거리 안에 있을때 돌진스킬 사용X Idle상태로 초기화
            StartCoroutine(CheckSkillADistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (useSkillB == true)
        {
            useSkillB = false;
            CheckUseSkill();
            SkillB();
            return;
        }
    } // SKILL

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

    //! { 해골병사 항목별 region 모음
    #region 공격 처리 (Collider)
    //! 무기 공격 처리 이벤트함수 (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! 쉴드 공격 처리 이벤트함수 (Collider)
    private void EnableShield()
    {
        shield.SetActive(true);
    } // EnableShield

    //! 공격종료 이벤트함수
    public override void ExitAttack()
    {
        damage = defaultDamage;
        weapon.SetActive(false);
        shield.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        mController.monsterAni.SetBool("isSkillB", false);
        // 공격종료 후 딜레이 시작
        mController.isDelay = true;
    } // ExitAttack
    #endregion // 공격 처리 (Collider, RayCast)

    #region 스킬A (돌진)
    //! 스킬A 함수 (돌진 공격)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! 스킬A 돌진 사용 거리체크하는 코루틴함수
    private IEnumerator CheckSkillADistance()
    {
        isNoRangeSkill = true;
        while (isNoRangeSkill == true)
        {
            // 타겟이 돌진 최소사거리 밖에 있으면 돌진 사용가능
            if (mController.distance >= 13f)
            {
                useSkillA = true;
                isNoRangeSkill = false;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckSkillADistance

    //! 스킬A 돌진 공격 코루틴함수
    private IEnumerator UseSkillA()
    {
        // 돌진 쿨타임 시작
        StartCoroutine(SkillACooldown());
        // 돌진 공격 전 함성 시작
        mController.monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        mController.monsterAni.SetBool("isSkillA_Start", true);
        // 돌진 준비 모션 끝나면 돌진 시작
        bool isStart = true;
        bool isSkillA = true;
        mController.mAgent.speed = moveSpeed * 2f;
        while (isSkillA == true)
        {
            if (mController.monsterAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && isStart == true)
            {
                // 돌진 시작 모션 끝나면 Loop모션으로 전환
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", true);
                isStart = false;
            }
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            // 돌진 중 타겟이 근접공격사거리 안이라면 돌진 마무리 시작
            if (mController.distance <= meleeAttackRange)
            {
                damage = Mathf.FloorToInt(defaultDamage * 1.5f);
                mController.mAgent.speed = moveSpeed;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = false;
            }
            yield return null;
        }
    } // UseSkillA

    //! 스킬A 쿨다운 코루틴함수
    private IEnumerator SkillACooldown()
    {
        skillACool = 0f;
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 사용가능
        isNoRangeSkill = true;
        while (skillACool < skillA_MaxCool)
        {
            skillACool += Time.deltaTime;
            yield return null;
        }
        skillACool = 0f;
        useSkillA = true;
        isNoRangeSkill = false;
        CheckUseSkill();
    } // SkillACooldown
    #endregion //스킬A (돌진)

    #region 스킬B (연속 베기)
    //! 스킬B 함수 (연속 베기)
    private void SkillB()
    {
        // 스킬 데미지 적용
        damage = Mathf.FloorToInt(defaultDamage * 0.7f);
        mController.monsterAni.SetBool("isSkillB", true);
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! 스킬B 쿨다운 코루틴함수
    private IEnumerator SkillBCooldown()
    {
        skillBCool = 0f;
        while (skillBCool < skillB_MaxCool)
        {
            skillBCool += Time.deltaTime;
            yield return null;
        }
        skillBCool = 0f;
        useSkillB = true;
        CheckUseSkill();
    } // SkillBCooldown
    #endregion // 스킬B (연속 베기)

    #region 사운드 모음
    private void RoarSound()
    {
        mController.monsterAudio.clip = roarClip;
        mController.monsterAudio.Play();
    } // RoarSound
    private void DeadSound()
    {
        mController.monsterAudio.clip = deadClip;
        mController.monsterAudio.Play();
    } // DeadSound
    private void MoveSound()
    {
        mController.monsterAudio.clip = moveClip;
        mController.monsterAudio.Play();
    } // MoveSound
    private void HitSound()
    {
        mController.monsterAudio.clip = hitClip;
        mController.monsterAudio.Play();
    } // HitSound
    private void WeaponSound()
    {
        mController.monsterAudio.clip = weaponClip;
        mController.monsterAudio.Play();
    } // WeaponSound
    #endregion // 사운드 모음
    //! } 해골병사 항목별 region 모음
} // SkeletonSoldier
