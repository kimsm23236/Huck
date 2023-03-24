using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonArcher : Monster
{
    private MonsterController mController = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private float skillA_MaxCool = default;
    private float skillACool = 0f;
    private bool isAttackDelay = false;
    private void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.RANGE, monsterData);
        mController.monster = this;
    } // Awake

    //! 공격 처리 이벤트함수 (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! 화살 쏘는 함수
    private void ShootArrow()
    {
        Vector3 dir = (mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        ArrowPool.Instance.GetArrow(dir, weapon.transform.position);
    } // ShootArrow

    //! 해골궁수 공격 오버라이드
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (mController.distance > meleeAttackRange)
        {
            mController.monsterAni.SetBool("isAttackB", true);
            StartCoroutine(LookAtTarget());
        }
        else
        {
            mController.monsterAni.SetBool("isAttackA", true);
        }
    } // Attack

    //! 해골궁수 스킬 오버라이드
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
    } // Skill

    //! 사용가능한 스킬이 있는지 체크하는 함수 (몬스터컨트롤러에서 상태진입 체크하기 위함)
    private void CheckUseSkill()
    {
        if (useSkillA == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

    //! 공격종료 이벤트함수
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        // 공격종료 후 딜레이 시작
        mController.isDelay = true;
    } // ExitAttack

    //! 스킬A 함수
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(LookAtTarget());
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! 스킬A 쿨다운 코루틴함수
    private IEnumerator SkillACooldown()
    {
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

    //! 타겟을 바라보는 코루틴함수
    private IEnumerator LookAtTarget()
    {
        isAttackDelay = false;
        bool isLookAt = true;
        while (isLookAt == true)
        {
            // 공격딜레이가 시작되면 종료
            if (isAttackDelay == true)
            {
                isLookAt = false;
                yield break;
            }
            Vector3 dir = (mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
            yield return null;
        }
    } // LookTarget

    //! 타겟 바라보기 중지하는 이벤트함수
    private void OffLookAtTarget()
    {
        isAttackDelay = true;
    } // OffLookAtTarget
} // SkeletonArcher
