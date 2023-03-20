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

    //! �ذ�� ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // ��� 2�� �� �������� �Ѱ� ����
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

    //! �ذ�� ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        SkillA();
    } // Skill

    //! ��ųA �Լ�
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA_Start", true);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! ��ųA ���� ���� �̺�Ʈ�Լ�
    private void SkillA_Combo()
    {
        StartCoroutine(UseSkillA());
    } // SkillA_Combo

    //! �������� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        // �������� �� ������ ����
        StartCoroutine(AttackDelay(mController, 3));
    } // ExitAttack

    //! ��ųA ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillA()
    {
        // ��ȯ �غ� ��� ������ ��ȯ ����
        mController.monsterAni.SetBool("isSkillA_Start", false);
        mController.monsterAni.SetBool("isSkillA_Loop", true);
        bool isSkillA = false;
        float timeCheck = 0f;
        while (isSkillA == false)
        {
            timeCheck += Time.deltaTime;
            //Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            //mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            // 5�ʰ� ������ ��ȯ ������ ����
            if (timeCheck >= 5f)
            {
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = true;
            }
            yield return null;
        }
    } // UseSkillA

    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        useSkillA = false;
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
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
