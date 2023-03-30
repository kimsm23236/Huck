using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SkeletonKing : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private GameObject summonObjPrefab = default;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private bool useSkillB = default;
    [SerializeField] private bool useSkillC = default;
    [SerializeField] private bool useSkillD = default;
    [SerializeField] private float skillA_MaxCool = default;
    [SerializeField] private float skillB_MaxCool = default;
    [SerializeField] private float skillC_MaxCool = default;
    [SerializeField] private float skillD_MaxCool = default;
    [SerializeField] private float slideAttack_MaxCool = default;
    [SerializeField] private float crushAttack_MaxCool = default;
    [HideInInspector] public bool is2Phase = false;
    private DamageMessage damageMessage = default;
    private GameObject indicator_Prefab = default;
    private GameObject skillB_Prefab = default;
    private GameObject skillC_Prefab = default;
    private int defaultDamage = default;
    private int summonCount = default;
    private float skillACool = 0f;
    private float skillBCool = 0f;
    private float skillCCool = 0f;
    private float skillDCool = 0f;
    private float slideAttackCool = 0f;
    private float crushAttackCool = 0f;
    void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.BOSS, monsterData);
        mController.monster = this;
        defaultDamage = damage;
        damageMessage = new DamageMessage(gameObject, damage);
        indicator_Prefab = Resources.Load("Prefabs/Monster/Projectile/Circle_Indicator") as GameObject;
        skillB_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_King_Effect/LeapEffect") as GameObject;
        skillC_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_King_Effect/Thunder") as GameObject;
        CheckUseSkill();
    } // Awake

    //! �ذ�� ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // �����̵� ���� ����üũ
        if (slideAttackCool <= 0f && isNoRangeAttack == false && mController.distance >= 7f)
        {
            SlideAttack();
        }
        else if (slideAttackCool <= 0f && isNoRangeAttack == false && mController.distance < 7f)
        {
            StartCoroutine(CheckSlideDistance());
            // �����̵� ������ ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ���X Idle���·� �ʱ�ȭ
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        // ���� ���� ����üũ
        if (mController.distance <= meleeAttackRange)
        {
            if (crushAttackCool <= 0f)
            {
                CrushAttack();
                return;
            }
            // ��� 5�� �� �������� �Ѱ� ����
            int number = default;
            if (is2Phase == true)
            {
                // 2������� ���� ��� E Ÿ�� �߰�
                number = Random.Range(0, 11);
            }
            else
            {
                number = Random.Range(0, 9);
            }

            if (number > 8)
            {
                StartCoroutine(UseAttackE());
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

    //! �ذ�� ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        if (useSkillA == true)
        {
            useSkillA = false;
            CheckUseSkill();
            SkillA();
            return;
        }

        if (useSkillD == true)
        {
            useSkillD = false;
            SkillD();
            CheckUseSkill();
            return;
        }

        if (useSkillB == true && mController.distance >= 13f)
        {
            useSkillB = false;
            CheckUseSkill();
            SkillB();
            return;
        }
        else if (useSkillB == true && mController.distance < 13f)
        {
            useSkillB = false;
            CheckUseSkill();
            // ���� ���� ��ų�� ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ��ų ���X Idle���·� �ʱ�ȭ
            StartCoroutine(CheckSkillBDistance());
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        if (useSkillC == true && mController.distance <= meleeAttackRange)
        {
            useSkillC = false;
            CheckUseSkill();
            SkillC();
            return;
        }
    } // Skill

    //! ��밡���� ��ų�� �ִ��� üũ�ϴ� �Լ� (������Ʈ�ѷ����� �������� üũ�ϱ� ����)
    private void CheckUseSkill()
    {
        if (useSkillA == false && useSkillB == false && useSkillD == false)
        {
            isNoRangeSkill = true;
        }
        else
        {
            isNoRangeSkill = false;
        }

        if (useSkillA == false && useSkillB == false && useSkillC == false && useSkillD == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

    //! { �ذ�� �׸� region ����
    #region ���� ó�� (Collider, Raycast)
    //! ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! �������� �̺�Ʈ�Լ�
    public override void ExitAttack()
    {
        weapon.SetActive(false);
        damage = defaultDamage;
        mController.monsterAni.SetBool("isSlideAttack", false);
        mController.monsterAni.SetBool("isCrushAttack", false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isAttackC", false);
        mController.monsterAni.SetBool("isAttackD", false);
        mController.monsterAni.SetBool("isAttackE", false);
        mController.monsterAni.SetBool("isSkillA_End", false);
        mController.monsterAni.SetBool("isSkillB", false);
        mController.monsterAni.SetBool("isSkillC", false);
        mController.monsterAni.SetBool("isSkillD_End", false);
        // �������� �� ������ ���·� ��ȯ
        mController.isDelay = true;
    } // ExitAttack

    //! ���� EŸ�� �ڷ�ƾ�Լ�
    private IEnumerator UseAttackE()
    {
        mController.monsterAni.SetBool("isAttackE", true);
        yield return new WaitForSeconds(1f);
        float time = 0f;
        // 2������ : �ִϸ��̼� �ӵ��� 1.2�� �����Ѹ�ŭ �ð�����
        float maxTime = 1.5f / mController.monsterAni.speed;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            mController.mAgent.Move(mController.transform.forward * moveSpeed * 0.5f * Time.deltaTime);
            yield return null;
        }
    } // UseAttackE
    #endregion // ���� ó�� (Collider, Raycast)

    #region �������� ���� ó��
    //! �������� ���� ó�� �Լ�
    public override void BossDead()
    {
        StartCoroutine(Dead());
    } // BossDead

    //! �������� ����� ���� ó�� �Լ�
    private IEnumerator Dead()
    {
        mController.monsterAni.speed = 1f;
        mController.monsterAni.SetBool("isDead", true);
        // �״� ��� �ǰ��⸦ ���� floatƮ����
        mController.monsterAni.SetFloat("RewindDead", 1f);
        yield return null;
        float deadAniTime = mController.monsterAni.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deadAniTime);
        mController.monsterAni.SetFloat("RewindDead", 0f);
        yield return new WaitForSeconds(2f);
        if (is2Phase == false)
        {
            // 1������� �׾��� ��� ��Ȱ�ϰ� 2������� ��ȯ
            mController.monsterAni.speed = 0.5f;
            mController.monsterAni.SetFloat("RewindDead", -1f);
            yield return null;
            yield return new WaitForSeconds(deadAniTime * 2f);
            mController.monsterAni.SetBool("isDead", false);
            mController.monsterAni.SetTrigger("isRoar");
            yield return new WaitForSeconds(0.1f);
            float time = 0f;
            float endTime = mController.monsterAni.GetCurrentAnimatorStateInfo(0).length - 2f;
            while (time < endTime)
            {
                // endTime ���� scale�� 1���� 1.5���� �ø�
                time += Time.deltaTime;
                mController.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, time / endTime);
                yield return null;
            }
            yield return new WaitForSeconds(2f);
            mController.isDead = false;
            is2Phase = true;
            // 2������ ��ȯ�� �ִϸ��̼� �ӵ� 1.2��� ����
            mController.monsterAni.speed = 1.2f;
            meleeAttackRange = 4f;
        }
        else
        {
            // 2������� ������ ����
            // ������ ��ü�� �������� �ϱ����� �׺�Ž� ��Ȱ��ȭ
            mController.mAgent.enabled = false;
            // 4�ʿ� ���� �� 2f��ŭ ������ ������ �ڿ� ��Ʈ����
            float deadTime = 0f;
            while (deadTime < 4f)
            {
                deadTime += Time.deltaTime;
                float deadSpeed = Time.deltaTime * 0.5f;
                mController.transform.position += Vector3.down * deadSpeed;
                yield return null;
            }
            Destroy(mController.gameObject);
        }
    } // Dead
    #endregion // �������� ���� ó��

    #region CrushAttack Ÿ������ ���� ����
    //! �ذ�� CrushAttack �Լ� (Ÿ������ ���� ����)
    private void CrushAttack()
    {
        StartCoroutine(UseCrushAttack());
    } // CrushAttack

    //! CrushAttack ��� �ڷ�ƾ�Լ�
    private IEnumerator UseCrushAttack()
    {
        StartCoroutine(CrushAttackCooldown());
        mController.monsterAni.SetBool("isCrushAttack", true);
        yield return null;
        float time = 0f;
        if (is2Phase == false)
        {
            mController.mAgent.speed = 1f;
        }
        else
        {
            mController.mAgent.speed = 2f;
        }
        while (time < mController.monsterAni.GetCurrentAnimatorStateInfo(0).length)
        {
            time += Time.deltaTime;
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            yield return null;
        }
        mController.mAgent.speed = moveSpeed;
        mController.mAgent.ResetPath();
    } // UseCrushAttack

    //! CrushAttack ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator CrushAttackCooldown()
    {
        crushAttackCool = 0f;
        while (crushAttackCool < crushAttack_MaxCool)
        {
            crushAttackCool += Time.deltaTime;
            yield return null;
        }
        crushAttackCool = 0f;
    } // CrushAttackCooldown
    #endregion // CrushAttack Ÿ������ ���� ����

    #region �����̵� ����(���Ÿ�)
    //! �����̵� ���� �Լ�
    private void SlideAttack()
    {
        StartCoroutine(UseSlideAttack());
    } // SlideAttack

    //! �����̵� ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSlideAttack()
    {
        StartCoroutine(SlideAttackCooldown());
        mController.monsterAni.SetBool("isSlideAttack", true);
        yield return null;
        float time = 0f;
        float waitTime = 0f;
        float speed = default;
        if (is2Phase == false)
        {
            speed = moveSpeed;
            waitTime = 0.5f / mController.monsterAni.speed;
        }
        else
        {
            speed = moveSpeed * 1.2f;
            waitTime = 0.5f;
        }
        while (time < mController.monsterAni.GetCurrentAnimatorStateInfo(0).length - waitTime)
        {
            time += Time.deltaTime;
            mController.mAgent.Move(mController.transform.forward * speed * Time.deltaTime);
            yield return null;
        }
    } // UseSlideAttack

    //! �����̵� ���� ��� �Ÿ� üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSlideDistance()
    {
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
        {
            // Ÿ���� �����̵� �ּһ�Ÿ� �ۿ� ������ �����̵� ���� ��밡��
            if (mController.distance >= 7f)
            {
                isNoRangeAttack = false;
                yield break;
            }
            yield return null;
        }
    } // CheckSlideDistance
    private IEnumerator SlideAttackCooldown()
    {
        slideAttackCool = 0f;
        isNoRangeAttack = true;
        while (slideAttackCool < slideAttack_MaxCool)
        {
            slideAttackCool += Time.deltaTime;
            yield return null;
        }
        slideAttackCool = 0f;
        isNoRangeAttack = false;
    } // SlideAttackCooldown
    #endregion // �����̵� ����(���Ÿ�)

    #region ��ųA �ذ�׷�Ʈ ��ȯ
    //! �ذ�� ��ųA �Լ� (��ȯ ��ų)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! ��ųA ���� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillA()
    {
        StartCoroutine(SkillACooldown());
        mController.monsterAni.SetBool("isSkillA_Start", true);
        yield return null;
        bool isStart = true;
        bool isSkillA = false;
        float timeCheck = 0f;
        while (isSkillA == false)
        {
            timeCheck += Time.deltaTime;
            if (mController.monsterAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && isStart == true)
            {
                // ��ȯ �غ� ��� ������ ��ȯ ����
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", true);
                isStart = false;
                Summon();
            }
            // 5�ʰ� ������ ��ȯ ������ ����
            if (isStart == false && timeCheck >= 4f)
            {
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = true;
            }
            yield return null;
        }
    } // UseSkillA

    //! ��ųA �ذ�׷�Ʈ ��ȯ�ϴ� �Լ�
    private void Summon()
    {
        GetRandomPosition getRandomPos = new GetRandomPosition();
        // �������� ��ֹ��� ���� ��ǥ�� ������
        Vector3 pos = getRandomPos.GetRandomCirclePos(transform.position, 10, 4);
        Vector3 dirToTarget = (mController.targetSearch.hit.transform.position - pos).normalized;
        // �ذ�׷�Ʈ�� ��ȯ�� �� Ÿ���� �ٶ󺸸鼭 ��ȯ�ǰ� ȸ���� ����
        Instantiate(summonObjPrefab, pos, Quaternion.LookRotation(dirToTarget));
    } // Summon

    //! ��ųA ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillACooldown()
    {
        skillACool = 0f;
        while (skillACool < skillA_MaxCool)
        {
            skillACool += Time.deltaTime;
            yield return null;
        }
        skillACool = 0f;
        useSkillA = true;
        CheckUseSkill();
    } // SkillACooldown
    #endregion // ��ųA �ذ�׷�Ʈ ��ȯ

    #region ��ųB ���� ����
    //! �ذ�� ��ųB �Լ� (���� ����)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
    } // SkillB

    //! ��ųB (���� ����) �ڷ�ƾ�Լ�
    private IEnumerator UseSkillB()
    {
        mController.monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        StartCoroutine(SkillBCooldown());
        mController.monsterAni.SetBool("isSkillB", true);
        float waitTime = default;
        float leapTime = default;
        if (is2Phase == true)
        {
            // 2������ : �ִϸ��̼� �ӵ��� 1.2�� �����Ѹ�ŭ �ð�����
            waitTime = 0.8f / mController.monsterAni.speed;
            leapTime = 1f / mController.monsterAni.speed;
        }
        else
        {
            waitTime = 0.8f;
            leapTime = 1f;
        }
        float time = 0f;
        mController.mAgent.speed = 2f;
        while (time < waitTime)
        {
            // �����غ� �ð����� Ÿ�ٹ������� õõ�� �̵�
            time += Time.deltaTime;
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            yield return null;
        }
        mController.mAgent.speed = moveSpeed;
        mController.mAgent.ResetPath();
        // ������ �̵��Լ��� ����ϱ� ���� Parabola �ʱ�ȭ
        Parabola parabola = new Parabola();
        // ���Ͱ� Ÿ���� �ٶ󺸴� ������ �ݴ������ ����
        Vector3 dir = -(mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        // ��ǥ��ġ�� dir�������� meleeAttackRange��ŭ �̵��� ��ǥ�� ����
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * meleeAttackRange;
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        StartCoroutine(parabola.ParabolaMoveToTarget(mController.transform.position, targetPos, leapTime, gameObject));
        // ���ݹ��� ǥ��
        GameObject indicator = Instantiate(indicator_Prefab, mController.targetSearch.hit.transform.position, indicator_Prefab.transform.rotation);
        indicator.GetComponent<AttackIndicator>().InitAttackIndicator(6f, leapTime);
    } // UseSkillB

    //! ��ųB ���������� �̺�Ʈ�Լ�
    private void SkillB_Damage()
    {
        StartCoroutine(OnEffectB());
        damageMessage.damageAmount = defaultDamage * 2;
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        if (hits.Length > 0)
        {
            foreach (var _hit in hits)
            {
                // if : �÷��̾� �Ǵ� ���๰�� ��
                if (_hit.collider.tag == GData.PLAYER_MASK || _hit.collider.tag == GData.BUILD_MASK)
                {
                    _hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
                }
            }
        }
        damageMessage.damageAmount = defaultDamage;
    } // SkillB_Damage

    //! ��ųB ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectB()
    {
        GameObject effectObj = Instantiate(skillB_Prefab);
        ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
        effectObj.transform.position = weapon.transform.position + Vector3.up * 0.5f;
        effectObj.transform.forward = transform.forward;
        effect.Play();
        yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
        Destroy(effectObj);
    } // OnEffectB

    //! ��ųB (���� ����) ���Ÿ� üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSkillBDistance()
    {
        isNoRangeSkill = true;
        while (mController.distance < 13f)
        {
            yield return null;
        }
        useSkillB = true;
        isNoRangeSkill = false;
        CheckUseSkill();
    } // CheckSkillBDistance

    //! ��ųB ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillBCooldown()
    {
        isNoRangeSkill = true;
        skillBCool = 0f;
        while (skillBCool < skillB_MaxCool)
        {
            skillBCool += Time.deltaTime;
            yield return null;
        }
        skillBCool = 0f;
        isNoRangeSkill = false;
        useSkillB = true;
        CheckUseSkill();
    } // SkillBCooldown
    #endregion // ��ųB ���� ����

    #region ��ųC ���� ����
    //! �ذ�� ��ųC �Լ� (���� ����)
    private void SkillC()
    {
        StartCoroutine(UseSkillC());
    } // SkillC

    //! ��ųC (���� ����) �ڷ�ƾ�Լ�
    private IEnumerator UseSkillC()
    {
        StartCoroutine(SkillCCooldown());
        mController.monsterAni.SetBool("isSkillC", true);
        yield return null;
        float time = 0f;
        while (time < mController.monsterAni.GetCurrentAnimatorStateInfo(0).length)
        {
            time += Time.deltaTime;
            mController.transform.LookAt(mController.targetSearch.hit.transform.position);
            yield return null;
        }
    } // UseSkillC

    //! ��ųC ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectC()
    {
        Vector3 pos = weapon.transform.position + Vector3.up * 0.5f;
        if (is2Phase == false)
        {
            // 1������
            GameObject effectObj = Instantiate(skillC_Prefab);
            ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
            effectObj.transform.position = pos;
            effectObj.transform.forward = transform.forward;
            yield return new WaitForSeconds(0.5f);
            effect.Play();
            SkillD_Damage(pos);
            yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
            Destroy(effectObj);
        }
        else
        {
            // 2������
            Vector3 dir = (mController.targetSearch.hit.transform.position - pos).normalized;
            dir.y = 0f;
            List<GameObject> effectObjList = new List<GameObject>();
            ParticleSystem lasteffect = default;
            for (int i = 0; i < 3; i++)
            {
                GameObject effectObj = Instantiate(skillC_Prefab);
                ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
                switch (i)
                {
                    case 0:
                        effectObj.transform.position = pos;
                        yield return new WaitForSeconds(0.5f);
                        effect.Play();
                        break;
                    case 1:
                        effectObj.transform.position = pos + (dir + Vector3.right).normalized * 2f;
                        yield return new WaitForSeconds(0.3f);
                        effect.Play();
                        break;
                    case 2:
                        effectObj.transform.position = pos + (dir + Vector3.left).normalized * 2f;
                        effect.Play();
                        lasteffect = effect;
                        break;
                }
                SkillD_Damage(effectObj.transform.position);
                effectObjList.Add(effectObj);
            }
            yield return new WaitForSeconds(lasteffect.main.duration + lasteffect.main.startLifetime.constant);
            foreach (var obj in effectObjList)
            {
                Destroy(obj);
            }
        }
    } // OnEffectC

    //! ��ųC ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillCCooldown()
    {
        skillCCool = 0f;
        while (skillCCool < skillC_MaxCool)
        {
            skillCCool += Time.deltaTime;
            yield return null;
        }
        skillCCool = 0f;
        useSkillC = true;
        CheckUseSkill();
    } // SkillCCooldown
    #endregion // ��ųC ���� ����

    #region ��ųD Į����
    //! �ذ�� ��ųD �Լ� (Į����)
    private void SkillD()
    {
        StartCoroutine(UseSkillD());
    } // SkillD

    //! ��ųD ��� �ڷ�ƾ�Լ�
    private IEnumerator UseSkillD()
    {
        StartCoroutine(SkillDCooldown());
        mController.monsterAni.SetBool("isSkillD_Start", true);
        yield return null;
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        mController.monsterAni.SetBool("isSkillD_Start", false);
        mController.monsterAni.SetBool("isSkillD_End", true);
        StartCoroutine(OnEffectSkillD());
    } // UseSkillD

    //! ��ųD ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectSkillD()
    {
        if (is2Phase == false)
        {
            // 1������ ���� ����Ʈ
            GameObject thunder = ProjectilePool.Instance.GetProjecttile();
            ParticleSystem effect = thunder.GetComponent<ParticleSystem>();
            thunder.transform.position = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
            // ���ݹ��� ǥ��
            GameObject indicator = Instantiate(indicator_Prefab, thunder.transform.position, indicator_Prefab.transform.rotation);
            indicator.GetComponent<AttackIndicator>().InitAttackIndicator(4f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            thunder.gameObject.SetActive(true);
            effect.Play();
            SkillD_Damage(thunder.transform.position);
            yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
            effect.Stop();
            ProjectilePool.Instance.EnqueueProjecttile(thunder);
        }
        else
        {
            // 2������ ���� ����Ʈ
            GetRandomPosition getRandomPos = new GetRandomPosition();
            List<Vector3> randomPosList = new List<Vector3>();
            while (randomPosList.Count < 10)
            {
                // Ÿ����ġ ���������� �ߺ������ʴ� ������ǥ 10�� �̾ƿ� 
                Vector3 pos = getRandomPos.GetRandomCirclePos(mController.targetSearch.hit.transform.position, 10, 0);
                if (!randomPosList.Contains(pos))
                {
                    randomPosList.Add(pos);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                // ���ݹ��� ǥ��
                GameObject indicator = Instantiate(indicator_Prefab, randomPosList[i], indicator_Prefab.transform.rotation);
                indicator.GetComponent<AttackIndicator>().InitAttackIndicator(4f, 1.5f);
                if (i % 2 == 0)
                {
                    // ¦����°���� 0.3�� �ʰ� �������� ó��
                    yield return new WaitForSeconds(0.3f);
                }
            }
            ParticleSystem lastEffect = default;
            List<GameObject> thunderList = new List<GameObject>();
            for (int i = 0; i < 10; i++)
            {
                GameObject thunder = ProjectilePool.Instance.GetProjecttile();
                ParticleSystem effect = thunder.GetComponent<ParticleSystem>();
                thunder.transform.position = randomPosList[i];
                thunder.gameObject.SetActive(true);
                effect.Play();
                SkillD_Damage(thunder.transform.position);
                if (i == 9)
                {
                    lastEffect = effect;
                }
                thunderList.Add(thunder);
                if (i % 2 == 0)
                {
                    // ¦����°���� 0.3�� �ʰ� �������� ó��
                    yield return new WaitForSeconds(0.3f);
                }
            }
            yield return new WaitForSeconds(lastEffect.main.duration + lastEffect.main.startLifetime.constant);
            for (int i = 0; i < 10; i++)
            {
                ProjectilePool.Instance.EnqueueProjecttile(thunderList[i]);
            }
        }
    } // OnEffectSkillD

    //! ��ųD ���������� �Լ�
    private void SkillD_Damage(Vector3 _effectPos)
    {
        damageMessage.damageAmount = Mathf.FloorToInt(defaultDamage * 1.5f);
        RaycastHit[] hits = Physics.SphereCastAll(_effectPos, 2f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        if (hits.Length > 0)
        {
            foreach (var _hit in hits)
            {
                // if : �÷��̾� �Ǵ� ���๰�� ��
                if (_hit.collider.tag == GData.PLAYER_MASK || _hit.collider.tag == GData.BUILD_MASK)
                {
                    _hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
                }
            }
        }
        damageMessage.damageAmount = defaultDamage;
    } // SkillD_Damage

    //! ��ųD ��ٿ� �ڷ�ƾ�Լ�
    private IEnumerator SkillDCooldown()
    {
        skillDCool = 0f;
        while (skillDCool < skillD_MaxCool)
        {
            skillDCool += Time.deltaTime;
            yield return null;
        }
        skillDCool = 0f;
        useSkillD = true;
        CheckUseSkill();
    } // SkillDCooldown
    #endregion // ��ųD
    //! } �ذ�� �׸� region ����
} // SkeletonKing
