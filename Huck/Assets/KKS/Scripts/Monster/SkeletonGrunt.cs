using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGrunt : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private GameObject shoulder = default;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private bool useSkillB = default;
    [SerializeField] private float skillA_MaxCool = default;
    [SerializeField] private float skillB_MaxCool = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    private float rushCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.MELEE, monsterData);
        mController.monster = this;
    } // Awake

    //! ���� ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! ��� ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableShoulderAttack()
    {
        shoulder.SetActive(true);
    } // EnableShoulderAttack

    //! �ذ�׷�Ʈ ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (mController.distance >= 13f)
        {
            StartCoroutine(RushAttack());
            return;
        }
        else if (mController.distance < 13f)
        {
            // ���������� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ �������� ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckRushDistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (mController.distance <= meleeAttackRange)
        {
            int number = Random.Range(0, 10);
            if (number > 7)
            {
                mController.monsterAni.SetBool("isAttackC", true);
                return;
            }
            else if (number > 4)
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

    //! �ذ�׷�Ʈ ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (useSkillA == true && mController.distance >= 13f)
        {
            useSkillA = false;
            SkillA();
            CheckUseSkill();
            return;
        }
        else if (useSkillA == true && mController.distance > meleeAttackRange)
        {
            useSkillA = false;
            // ��ųA�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ��ųA ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckSkillADistance());
            CheckUseSkill();
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (useSkillB == true)
        {
            useSkillB = false;
            SkillB();
            CheckUseSkill();
            return;
        }
    } // Skill

    //! ��밡���� ��ų�� �ִ��� üũ�ϴ� �Լ� (������Ʈ�ѷ����� �������� üũ�ϱ� ����)
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

    //! �ذ�׷�Ʈ ��ųA �Լ� (���� ����)
    private void SkillA()
    {
        // ������ �̵��Լ��� ����ϱ� ���� Parabola �ʱ�ȭ
        Parabola parabola = new Parabola();
        // ���Ͱ� Ÿ���� �ٶ󺸴� ������ �ݴ������ ����
        Vector3 dir = -(mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        // ��ǥ��ġ�� dir�������� meleeAttackRange��ŭ �̵��� ��ǥ�� ����
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * meleeAttackRange;
        StartCoroutine(parabola.ParabolaMoveToTarget(mController.transform.position, targetPos, 1.5f, gameObject));
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! ��ųA ��� �Ÿ�üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSkillADistance()
    {
        isNoRangeSkill = true;
        while (isNoRangeSkill == true)
        {
            // Ÿ���� ��ųA �ּһ�Ÿ� �ۿ� ������ ��ųA ��밡��
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

    //! SkillA ��� �� Ư�� �������� �ִϸ��̼� ���ߴ� �̺�Ʈ�Լ�
    private void StopSkillA_Ani()
    {
        StartCoroutine(PlaySkillA_Ani());
    } // StopSkillA_Ani
    //! SkillA ����� �ִϸ��̼� ����ϴ� �ڷ�ƾ�Լ�
    private IEnumerator PlaySkillA_Ani()
    {
        mController.monsterAni.StartPlayback();
        yield return new WaitForSeconds(0.5f);
        mController.monsterAni.StopPlayback();
    } // PlaySkillA

    //! ��ųA ���������� �Լ�
    private void SkillA_Damage()
    {
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        if (hits.Length > 0)
        {
            foreach (var _hit in hits)
            {
                Debug.Log($"{_hit.collider.name} ����!");
            }
        }
    } // SkillA_Damage

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.transform.position, 3f);
    } // OnDrawGizmos

    //! �ذ�׷�Ʈ ��ųB �Լ� (���� ������)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! �������� �̺�Ʈ�Լ�
    private void ExitAttack()
    {
        weapon.SetActive(false);
        shoulder.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isRushAttack", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isSkillB_End", false);
        // �������� �� ������ ����
        mController.isDelay = true;
    } // ExitAttack

    //! ���� ���� ��� �Ÿ� üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckRushDistance()
    {
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
        {
            // Ÿ���� ���� �ּһ�Ÿ� �ۿ� ������ ���� ��밡��
            if (mController.distance >= 13f)
            {
                isNoRangeAttack = false;
                yield break;
            }
            yield return null;
        }
    } // CheckRushDistance

    //! ���� ���� �ڷ�ƾ �Լ�
    private IEnumerator RushAttack()
    {
        StartCoroutine(RushCooldown());
        // ���� ���� �� �Լ� ����
        mController.monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        mController.monsterAni.SetBool("isRun", true);
        bool isRush = true;
        bool isFinishRush = false;
        float timeCheck = 0f;
        mController.mAgent.speed = moveSpeed * 2.5f;
        while (isRush == true)
        {
            // ���� ������ ���� ���� ������ Ÿ���� ���Ͽ� ����
            if (isFinishRush == false)
            {
                mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            }
            else
            {
                // ���� ������ ���� ���۵Ǹ� �����ϴ� ���� �״�� 1�ʰ� ����
                timeCheck += Time.deltaTime;
                mController.mAgent.Move(mController.transform.forward * moveSpeed * Time.deltaTime);
                if (timeCheck >= 1f)
                {
                    isRush = false;
                }
            }
            // ���� ������ ���� ����
            if (mController.distance <= meleeAttackRange && isFinishRush == false)
            {
                mController.mAgent.speed = moveSpeed;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isRun", false);
                mController.monsterAni.SetBool("isRushAttack", true);
                isFinishRush = true;
            }
            yield return null;
        }
    } // RushAttack

    //! ��ųB ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillB()
    {
        mController.monsterAni.SetBool("isSkillB_Start", true);
        bool isStart = true;
        float time = 0f;
        while (time <= 2.5f)
        {
            time += Time.deltaTime;
            mController.transform.LookAt(mController.targetSearch.hit.transform.position);
            if (time >= 0.24f && isStart == true)
            {
                mController.monsterAni.SetBool("isSkillB_Start", false);
                mController.monsterAni.SetBool("isSkillB_Loop", true);
                isStart = false;
            }
            yield return null;
        }
        mController.monsterAni.SetBool("isSkillB_Loop", false);
        mController.monsterAni.SetBool("isSkillB_End", true);
    } // UseSkillB

    //! ���� ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator RushCooldown()
    {
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ���� ���� üũ
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
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

    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        isNoRangeSkill = true;
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
        while (true)
        {
            skillACool += Time.deltaTime;
            if (skillACool >= skillA_MaxCool)
            {
                skillACool = 0f;
                useSkillA = true;
                isNoRangeSkill = false;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // SkillACooldown

    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillBCooldown()
    {
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
} // SkeletonGrunt
