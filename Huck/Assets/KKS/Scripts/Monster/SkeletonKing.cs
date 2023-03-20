using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKing : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    private float skillACool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! 해골왕 공격 오버라이드
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // 모션 2개 중 랜덤으로 한개 실행
        int number = Random.Range(0, 10);
        if (number <= 6)
        {
            mController.monsterAni.SetBool("isAttackA", true);
        }
        else
        {
            mController.monsterAni.SetBool("isAttackB", true);
        }
    } // Attack

    //! 해골왕 스킬 오버라이드
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        SkillA();
    } // Skill

    //! 스킬A 함수
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA_Start", true);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! 스킬A 연계 공격 이벤트함수
    private void SkillA_Combo()
    {
        StartCoroutine(UseSkillA());
    } // SkillA_Combo

    //! 공격종료 이벤트함수
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        // 공격종료 후 딜레이 시작
        StartCoroutine(AttackDelay(mController, 3));
    } // ExitAttack

    //! 스킬A 공격 코루틴함수
    private IEnumerator UseSkillA()
    {
        // 소환 준비 모션 끝나면 소환 시작
        mController.monsterAni.SetBool("isSkillA_Start", false);
        mController.monsterAni.SetBool("isSkillA_Loop", true);
        bool isSkillA = false;
        float timeCheck = 0f;
        while (isSkillA == false)
        {
            timeCheck += Time.deltaTime;
            //Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            //mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            // 5초가 지나면 소환 마무리 시작
            if (timeCheck >= 5f)
            {
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = true;
            }
            yield return null;
        }
    } // UseSkillA

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
} // SkeletonKing
