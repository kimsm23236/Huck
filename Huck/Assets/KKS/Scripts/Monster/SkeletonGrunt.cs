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
    [SerializeField] private AudioClip attackBClip = default;
    private DamageMessage damageMessage = default;
    private GameObject skillA_Prefab = default;
    private GameObject skillB_Prefab = default;
    private int defaultDamage = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    private float rushCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.NAMEED, monsterData);
        mController.monster = this;
        defaultDamage = damage;
        damageMessage = new DamageMessage(gameObject, damage);
        skillA_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_Grunt_Effect/LeapEffect") as GameObject;
        skillB_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_Grunt_Effect/Splash_Thorn") as GameObject;
        CheckUseSkill();
    } // Awake

    //! �ذ�׷�Ʈ ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (rushCool <= 0f && isNoRangeAttack == false && mController.distance >= 13f)
        {
            StartCoroutine(RushAttack());
            return;
        }
        else if (rushCool <= 0f && isNoRangeAttack == false && mController.distance < 13f)
        {
            // ���������� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ �������� ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckRushDistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            mController.isDelay = false;
            return;
        }

        if (mController.distance <= meleeAttackRange)
        {
            int number = Random.Range(0, 10);
            if (number > 7)
            {
                mController.monsterAni.SetBool("isAttackC", true);
            }
            else if (number > 4)
            {
                mController.monsterAni.SetBool("isAttackB", true);
            }
            else
            {
                mController.monsterAni.SetBool("isAttackA", true);
            }
            return;
        }
    } // Attack

    //! �ذ�׷�Ʈ ��ų �������̵�
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
            // ��ųA�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ��ųA ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckSkillADistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            mController.isDelay = false;
            return;
        }

        if (useSkillB == true)
        {
            useSkillB = false;
            CheckUseSkill();
            SkillB();
            return;
        }
    } // Skill

    //! ��밡���� ��ų�� �ִ��� üũ�ϴ� �Լ� (������Ʈ�ѷ����� �������� üũ�ϱ� ����)
    private void CheckUseSkill()
    {
        // ���Ÿ���ų ��� ���� üũ
        if (useSkillA == false)
        {
            isNoRangeSkill = true;
        }
        else
        {
            isNoRangeSkill = false;
        }
        // ��ų ��밡�� ���� üũ
        if (useSkillA == false && useSkillB == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

    //! { �ذ�׷�Ʈ �׸� region ����
    #region ���� ó�� (Collider, RayCast)
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

    //! �������� �̺�Ʈ�Լ�
    public override void ExitAttack()
    {
        weapon.SetActive(false);
        shoulder.SetActive(false);
        damage = defaultDamage;
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isRushAttack", false);
        mController.monsterAni.SetBool("isSkillA", false);
        mController.monsterAni.SetBool("isSkillB_End", false);
        // �������� �� ������ ����
        mController.isDelay = true;
    } // ExitAttack
    #endregion // ���� ó�� (Collider, RayCast)

    #region ���� ����
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
                damage = Mathf.FloorToInt(defaultDamage * 1.5f);
                mController.mAgent.speed = moveSpeed;
                mController.mAgent.ResetPath();
                mController.monsterAni.SetBool("isRun", false);
                mController.monsterAni.SetBool("isRushAttack", true);
                isFinishRush = true;
            }
            yield return null;
        }
    } // RushAttack

    //! ���� ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator RushCooldown()
    {
        rushCool = 0f;
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ���� ���� üũ
        isNoRangeAttack = true;
        while (rushCool < 20f)
        {
            rushCool += Time.deltaTime;
            yield return null;
        }
        rushCool = 0f;
        isNoRangeAttack = false;
    } // RushCooldown
    #endregion // ���� ����

    #region ��ųA (���� ����)
    //! �ذ�׷�Ʈ ��ųA �Լ� (���� ����)
    private void SkillA()
    {
        // ������ �̵��Լ��� ����ϱ� ���� Parabola �ʱ�ȭ
        Parabola parabola = new Parabola();
        // ���Ͱ� Ÿ���� �ٶ󺸴� ������ �ݴ������ ����
        Vector3 dir = -(mController.targetSearch.hit.transform.position - transform.position).normalized;
        // ��ǥ��ġ�� dir�������� meleeAttackRange��ŭ �̵��� ��ǥ�� ����
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * (meleeAttackRange + 1);
        StartCoroutine(parabola.ParabolaMoveToTarget(transform.position, targetPos, 1f, gameObject));
        mController.monsterAni.SetBool("isSkillA", true);
        // ���ݹ��� ǥ��
        dir.y = 0f;
        Vector3 pos = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
        mController.attackIndicator.GetCircleIndicator(pos, 6f, 1f);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! ��ųA ��� �Ÿ�üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSkillADistance()
    {
        while (useSkillA == false)
        {
            // Ÿ���� ��ųA �ּһ�Ÿ� �ۿ� ������ ��ųA ��밡��
            if (mController.distance >= 13f)
            {
                useSkillA = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckSkillADistance

    //! ��ųA ���������� �̺�Ʈ�Լ�
    private void SkillA_Damage()
    {
        StartCoroutine(OnEffectA());
        damageMessage.damageAmount = defaultDamage * 2;
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        if (hits.Length > 0)
        {
            foreach (var _hit in hits)
            {
                IDamageable damageable = _hit.collider.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damageMessage);
                }
            }
        }
    } // SkillA_Damage

    //! ��ųA ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectA()
    {
        GameObject effectObj = Instantiate(skillA_Prefab);
        ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
        effectObj.transform.position = weapon.transform.position;
        effectObj.transform.forward = transform.forward;
        effect.Play();
        yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
        Destroy(effectObj);
    } // OnEffectA

    //! ��ųA ���������� ���� �����
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.transform.position, 3f);
    } // OnDrawGizmos

    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        skillACool = 0f;
        // ������Ʈ�ѷ����� �������� �� üũ�� ���� : ���Ÿ� ��ų �� ����
        while (skillACool < skillA_MaxCool)
        {
            skillACool += Time.deltaTime;
            yield return null;
        }
        skillACool = 0f;
        useSkillA = true;
        CheckUseSkill();
    } // SkillACooldown
    #endregion // ��ųA (���� ����)

    #region ��ųB (���� ������)
    //! �ذ�׷�Ʈ ��ųB �Լ� (���� ������)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! ��ųB ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillB()
    {
        mController.monsterAni.SetBool("isSkillB_Start", true);
        // ���ݹ��� ǥ��
        GameObject indicator = mController.attackIndicator.GetRectangIndicator(mController.isDead, transform.position, 3f, 22f, 3.5f);
        Quaternion startRotation = indicator.transform.rotation;
        bool isStart = true;
        float time = 0f;
        while (time <= 2.5f)
        {
            time += Time.deltaTime;
            // ���ݹ��� ������ ������Ʈ�� ȸ������ x�� ���ΰɷ� �����ϸ鼭 y�ุ ���� ���� (LookAt�Լ��� ���� ���ݹ����� �ٲ�� ����) 
            indicator.transform.rotation = Quaternion.Euler(startRotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
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

    //! ��ųB ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectB()
    {
        GameObject effectObj = Instantiate(skillB_Prefab);
        ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
        effectObj.FindChildObj("thorn").GetComponent<ParticleTrigger>().InitDamageMessage(gameObject, Mathf.FloorToInt(defaultDamage * 2f));
        effectObj.transform.position = weapon.transform.position;
        effectObj.transform.forward = transform.forward;
        effect.Play();
        yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
        Destroy(effectObj);
    } // OnEffectB
    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
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
    #endregion // ��ųB (���� ������)

    #region ���� ����
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
    private void AttackBSound()
    {
        mController.monsterAudio.clip = attackBClip;
        mController.monsterAudio.Play();
    } // AttackBSound
    #endregion // ���� ����
    //! } �ذ�׷�Ʈ �׸� region ����
} // SkeletonGrunt
