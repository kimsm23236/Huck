using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonArcher : Monster
{
    private MonsterController mController = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private MonsterData monsterData;
    private float skillACool = 0f;
    private bool isAttackDelay = false;
    private void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.RANGE, monsterData);
        mController.monster = this;
    } // Awake

    //! ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! ȭ�� ��� �Լ�
    private void ShootArrow()
    {
        Vector3 dir = (mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        ArrowPool.Instance.GetArrow(dir, weapon.transform.position);
    } // ShootArrow

    //! �ذ�ü� ���� �������̵�
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

    //! �ذ�ü� ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        SkillA();
    } // Skill

    //! �������� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        // �������� �� ������ ����
        StartCoroutine(AttackDelay(mController, 4));
    } // ExitAttack

    //! ��ųA �Լ�
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(LookAtTarget());
        StartCoroutine(SkillACooldown());
    } // SkillA

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

    //! Ÿ���� �ٶ󺸴� �ڷ�ƾ�Լ�
    private IEnumerator LookAtTarget()
    {
        isAttackDelay = false;
        bool isLookAt = true;
        while (isLookAt == true)
        {
            // ���ݵ����̰� ���۵Ǹ� ����
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

    //! Ÿ�� �ٶ󺸱� �����ϴ� �̺�Ʈ�Լ�
    private void OffLookAtTarget()
    {
        isAttackDelay = true;
    } // OffLookAtTarget
} // SkeletonArcher
