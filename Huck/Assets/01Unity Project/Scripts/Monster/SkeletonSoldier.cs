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

    //! ���� ������ ó�� �Լ�
    private void OnDamage()
    {

    } // OnDamage

    //! ���Ÿ� ���� �Լ�
    private void LongDistanceAttack()
    {
        StartCoroutine(AttackC());
    } // LongDistanceAttack

    //! ������ ������ �� �����ϴ� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isAttackC_End", false);
        StartCoroutine(AttackDelay(mController));
    } // ExitAttack

    //! AttackC ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator AttackC()
    {
        Debug.Log("AttackC �ڷ�ƾ ����");
        // ���� �غ� ��� ������ ���� ����
        mController.monsterAni.SetBool("isAttackC_Start", false);
        mController.monsterAni.SetBool("isAttackC_Loop", true);
        bool isAttackC = false;
        while (isAttackC == false)
        {
            Vector3 dir = (mController.targetPos.position - mController.transform.position).normalized;
            mController.transform.rotation = Quaternion.Lerp(mController.transform.rotation, Quaternion.LookRotation(dir), 2f * Time.deltaTime);
            mController.transform.position += dir * (moveSpeed * 2f) * Time.deltaTime;
            // ���� �� Ÿ���� �������ݻ�Ÿ� ���̶�� ���� ������ ����
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
