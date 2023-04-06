using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonKing : Monster
{
    private MonsterController mController = default;
    [SerializeField] private MonsterData monsterData = default; // ��������
    [SerializeField] private GameObject weapon = default; // ���� ������Ʈ
    [SerializeField] private GameObject summonObjPrefab = default; // ��ųA ��ȯ������Ʈ Prefab
    [SerializeField] private bool useSkillA = default; // ��ųA ��밡�� üũ
    [SerializeField] private bool useSkillB = default; // ��ųB ��밡�� üũ
    [SerializeField] private bool useSkillC = default; // ��ųC ��밡�� üũ
    [SerializeField] private bool useSkillD = default; // ��ųD ��밡�� üũ
    [SerializeField] private float skillA_MaxCool = default; // ��ųA �ִ� ��ٿ�
    [SerializeField] private float skillB_MaxCool = default; // ��ųB �ִ� ��ٿ�
    [SerializeField] private float skillC_MaxCool = default; // ��ųC �ִ� ��ٿ�
    [SerializeField] private float skillD_MaxCool = default; // ��ųD �ִ� ��ٿ�
    [SerializeField] private float slideAttack_MaxCool = default; // �����̵���� �ִ� ��ٿ�
    [SerializeField] private float crushAttack_MaxCool = default; // ũ�������� �ִ� ��ٿ�
    [SerializeField] private AudioClip spawnClip = default;
    [HideInInspector] public bool is2Phase = false; // 1 ~ 2������ üũ
    private ProjectilePool skillD_Pool = default;
    private DamageMessage damageMessage = default; // ������ ó��
    private GameObject skillB_Prefab = default; // ��ųB ����Ʈ Prefab
    private GameObject skillC_Prefab = default; // ��ųC ����Ʈ Prefab
    private int defaultDamage = default; // �⺻ ������ ���� ����
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
        skillD_Pool = gameObject.GetComponent<ProjectilePool>();
        damageMessage = new DamageMessage(gameObject, damage);
        skillB_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_King_Effect/LeapEffect") as GameObject;
        skillC_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_King_Effect/Thunder") as GameObject;
        CheckUseSkill();
        GameManager.Instance.onPlayerDead += OffBossHpBar;
    } // Awake

    //! �ذ�� ���� �������̵�
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // �����̵� ���� ����üũ
        if (slideAttackCool <= 0f && isNoRangeAttack == false && mController.distance >= 7f)
        {
            isNoRangeAttack = true;
            SlideAttack();
            return;
        }
        else if (slideAttackCool <= 0f && isNoRangeAttack == false && mController.distance < 7f)
        {
            isNoRangeAttack = true;
            StartCoroutine(CheckSlideDistance());
            // �����̵� ������ ��밡�������� Ÿ���� �ּһ�Ÿ� �ȿ� ������ ���X Idle���·� �ʱ�ȭ
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            mController.isDelay = false;
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
            }
            else if (number > 2)
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

    //! �ذ�� ��ų �������̵�
    public override void Skill()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        

        

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
            mController.isDelay = false;
            return;
        }

        if (useSkillC == true && mController.distance <= meleeAttackRange)
        {
            useSkillC = false;
            CheckUseSkill();
            SkillC();
            return;
        }

        if (useSkillD == true)
        {
            useSkillD = false;
            SkillD();
            CheckUseSkill();
            return;
        }

        // ��ųA ��ȯ��ų ���� : ü���� 50%������ ���� ��밡��
        if (useSkillA == true && monsterHp <= monsterMaxHp * 0.5f)
        {
            useSkillA = false;
            CheckUseSkill();
            SkillA();
            return;
        }
        else if (useSkillA == true && monsterHp > monsterMaxHp * 0.5f)
        {
            useSkillA = false;
            CheckUseSkill();
            StartCoroutine(CheckUseSkillA());
            // ü���� ���� �ʰ��� ���X Idle���·� �ʱ�ȭ
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            mController.isDelay = false;
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
            if(mController.isDead == true)
            {
                yield break;
            }
            time += Time.deltaTime;
            mController.mAgent.Move(mController.transform.forward * moveSpeed * 0.5f * Time.deltaTime);
            yield return null;
        }
    } // UseAttackE

    //! ��ų ���������� �Լ� (SphereCastAll)
    private void Skill_Damage(Vector3 _Pos, float _radius, float _damageMultiplier)
    {
        damageMessage.damageAmount = Mathf.FloorToInt(defaultDamage * _damageMultiplier);
        RaycastHit[] hits = Physics.SphereCastAll(_Pos, _radius, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK, GData.WALL_MASK));
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
        damageMessage.damageAmount = defaultDamage;
    } // Skill_Damage
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
        GameManager.Instance.bgmAudio.Stop();
        mController.mAgent.ResetPath();
        mController.monsterAni.speed = 1f;
        mController.monsterAni.SetBool("isDead", true);
        mController.monsterAudio.clip = deadClip;
        mController.monsterAudio.Play();
        // �״� �ִϸ��̼� �ǰ��⸦ ���� floatƮ����
        mController.monsterAni.SetFloat("RewindDead", 1f);
        yield return null;
        float deadAniTime = mController.monsterAni.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deadAniTime);
        mController.monsterAni.SetFloat("RewindDead", 0f);
        yield return new WaitForSeconds(3f);
        if (is2Phase == false)
        {
            // 1������� �׾��� ��� ��Ȱ�ϰ� 2������� ��ȯ
            mController.monsterAni.speed = 0.5f;
            // �״� �ִϸ��̼� �ǰ���
            mController.monsterAni.SetFloat("RewindDead", -1f);
            yield return null;
            yield return new WaitForSeconds(deadAniTime * 2f);
            mController.monsterAni.SetBool("isDead", false);
            mController.monsterAni.SetTrigger("isRoar");
            yield return new WaitForSeconds(0.1f);
            mController.hpBar.gameObject.SetActive(true);
            float time = 0f;
            float endTime = mController.monsterAni.GetCurrentAnimatorStateInfo(0).length - 2f;
            GameManager.Instance.bgmAudio.Play();
            while (time < endTime)
            {
                time += Time.deltaTime;
                float reviveTime = time / endTime;
                // endTime ���� Hp�� MaxHp���� ȸ��
                float _hp = Mathf.Lerp(0f, monsterMaxHp, reviveTime);
                monsterHp = (int)_hp;
                mController.hpBar.InitHpBar(monsterHp);
                // endTime ���� scale�� 1���� 1.5���� �ø�
                mController.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, reviveTime);
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
            GameManager.Instance.StartBGM();
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
            GameManager.Instance.isExistenceBoss = false;
            GameManager.Instance.StartEnding();
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
            if (mController.isDead == true)
            {
                yield break;
            }
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

    //! ��ųA ��밡�� ���� üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckUseSkillA()
    {
        while (useSkillA == false)
        {
            if (monsterHp <= monsterMaxHp * 0.5f)
            {
                useSkillA = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckUseSkillA

    //! ��ųA �ذ�׷�Ʈ ��ȯ�ϴ� �Լ�
    private void Summon()
    {
        GetRandomPosition getRandomPos = new GetRandomPosition();
        // �������� ��ֹ��� ���� ��ǥ�� ������
        //Vector3 pos = getRandomPos.GetRandomCirclePos(transform.position, 10, 4);
        Vector3 pos = transform.position + (transform.forward * 2f);
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
        mController.attackIndicator.GetCircleIndicator(mController.targetSearch.hit.transform.position, 6f, leapTime);
    } // UseSkillB

    //! ��ųB ���������� �̺�Ʈ�Լ�
    private void SkillB_Damage()
    {
        StartCoroutine(OnEffectB());
    } // SkillB_Damage

    //! ��ųB ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectB()
    {
        GameObject effectObj = Instantiate(skillB_Prefab);
        ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
        Vector3 targetPos = weapon.transform.position + (Vector3.up * 5f);
        Vector3 pos = default;
        RaycastHit hit = default;
        if (Physics.Raycast(targetPos, Vector3.down, out hit, 10f) == true)
        {
            // ����Ʈ ���� ��ġ ������
            pos = hit.point + (Vector3.up * 0.1f);
        }
        effectObj.transform.position = pos;
        effectObj.transform.forward = transform.forward;
        effect.Play();
        Skill_Damage(pos, 3f, 2f);
        yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
        Destroy(effectObj);
    } // OnEffectB

    //! ��ųB (���� ����) ���Ÿ� üũ�ϴ� �ڷ�ƾ�Լ�
    private IEnumerator CheckSkillBDistance()
    {
        while (mController.distance < 13f)
        {
            yield return null;
        }
        useSkillB = true;
        CheckUseSkill();
    } // CheckSkillBDistance

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
        Vector3 targetPos = weapon.transform.position + Vector3.up * 5f;
        Vector3 pos = targetPos;
        RaycastHit hit = default;
        if (is2Phase == false)
        {
            // 1������
            if (Physics.Raycast(pos, Vector3.down, out hit, 10f) == true)
            {
                // ���� ������ ��ġ ������
                pos = hit.point + (Vector3.up * 0.1f);
            }
            // ���ݹ��� ǥ��
            mController.attackIndicator.GetCircleIndicator(pos, 3f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            GameObject effectObj = Instantiate(skillC_Prefab);
            ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
            effectObj.transform.position = pos;
            effect.Play();
            Skill_Damage(effectObj.transform.position, 1.5f, 1.5f);
            yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
            Destroy(effectObj);
        }
        else
        {
            // 2������
            Vector3 dir = transform.forward;
            List<GameObject> effectObjList = new List<GameObject>();
            ParticleSystem lasteffect = default;
            for (int i = 0; i < 3; i++)
            {
                if (Physics.Raycast(pos, Vector3.down, out hit, 10f) == true)
                {
                    // ���� ������ ��ġ ������
                    pos = hit.point + (Vector3.up * 0.1f);
                }
                Vector3 _Pos = pos + (dir * i * 2f);
                // ���ݹ��� ǥ��
                mController.attackIndicator.GetCircleIndicator(_Pos, 3f, 0.5f);
                yield return new WaitForSeconds(0.5f);
                GameObject effectObj = Instantiate(skillC_Prefab);
                ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
                effectObj.transform.position = _Pos;
                effect.Play();
                if (i == 2)
                {
                    lasteffect = effect;
                }
                Skill_Damage(effectObj.transform.position, 1.5f, 1.5f);
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
            // 1������
            Vector3 pos = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
            GameObject swordObj = skillD_Pool.GetProjecttile();
            ParticleSystem effect = swordObj.GetComponent<ParticleSystem>();
            swordObj.transform.position = pos;
            // ���ݹ��� ǥ��
            mController.attackIndicator.GetCircleIndicator(pos, 4f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            swordObj.gameObject.SetActive(true);
            effect.Play();
            Skill_Damage(swordObj.transform.position, 2f, 1.5f);
            yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
            effect.Stop();
            skillD_Pool.EnqueueProjecttile(swordObj);
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
                mController.attackIndicator.GetCircleIndicator(randomPosList[i], 4f, 1.5f);
                if (i % 2 == 0)
                {
                    // ¦����°���� 0.3�� �ʰ� ǥ�õǰ� ó��
                    yield return new WaitForSeconds(0.3f);
                }
            }
            ParticleSystem lastEffect = default;
            List<GameObject> thunderList = new List<GameObject>();
            for (int i = 0; i < 10; i++)
            {
                GameObject swordObj = skillD_Pool.GetProjecttile();
                ParticleSystem effect = swordObj.GetComponent<ParticleSystem>();
                swordObj.transform.position = randomPosList[i];
                swordObj.gameObject.SetActive(true);
                effect.Play();
                Skill_Damage(swordObj.transform.position, 2f, 1.5f);
                if (i == 9)
                {
                    lastEffect = effect;
                }
                thunderList.Add(swordObj);
                if (i % 2 == 0)
                {
                    // ¦����°���� 0.3�� �ʰ� �������� ó��
                    yield return new WaitForSeconds(0.3f);
                }
            }
            yield return new WaitForSeconds(lastEffect.main.duration + lastEffect.main.startLifetime.constant);
            for (int i = 0; i < 10; i++)
            {
                skillD_Pool.EnqueueProjecttile(thunderList[i]);
            }
        }
    } // OnEffectSkillD

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

    #region ���� ����
    private void RoarSound()
    {
        mController.monsterAudio.clip = roarClip;
        mController.monsterAudio.Play();
    } // RoarSound
    private void spawnSound()
    {
        mController.monsterAudio.clip = spawnClip;
        mController.monsterAudio.Play();
    } // spawnSound
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
    #endregion // ���� ����
    //! } �ذ�� �׸� region ����

    public void OffBossHpBar()
    {
        if (monsterType == Monster.MonsterType.BOSS)
        {
            mController.hpBar.gameObject.SetActive(false);
        }
    } // OffBossHpBar
} // SkeletonKing
