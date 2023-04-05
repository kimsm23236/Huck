using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher : Monster
{
    private MonsterController mController = default;
    [SerializeField] private GameObject weapon = default;
    [SerializeField] private Transform arrowPos = default;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private AudioClip attackAClip = default;
    [SerializeField] private bool useSkillA = default;
    [SerializeField] private float skillA_MaxCool = default;
    private ProjectilePool arrowPool = default;
    private GameObject skillA_Prefab = default;
    private DamageMessage damageMessage = default;
    private int defaultDamage = default;
    private float skillACool = 0f;
    private bool isAttackDelay = false;
    private void Awake()
    {
        mController = gameObject.GetComponent<MonsterController>();
        InitMonsterData(MonsterType.NOMAL, monsterData);
        mController.monster = this;
        defaultDamage = damage;
        arrowPool = gameObject.GetComponent<ProjectilePool>();
        damageMessage = new DamageMessage(gameObject, damage);
        skillA_Prefab = Resources.Load("Prefabs/Monster/MonsterEffect/Skeleton_Archer_Effect/ArrowRain") as GameObject;
        CheckUseSkill();
    } // Awake

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
        if (useSkillA == true)
        {
            useSkillA = false;
            CheckUseSkill();
            SkillA();
            return;
        }
    } // Skill

    //! ��밡���� ��ų�� �ִ��� üũ�ϴ� �Լ� (������Ʈ�ѷ����� �������� üũ�ϱ� ����)
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

    //! { �ذ�ü� �׸� region ����
    #region ���� ó�� (Collider)
    //! ���� ó�� �̺�Ʈ�Լ� (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! ȭ�� ��� �Լ�
    private void ShootArrow()
    {
        GameObject arrow = arrowPool.GetProjecttile();
        arrow.GetComponent<Arrow>().InitDamageMessage(arrowPool, gameObject, defaultDamage);
        if (arrow == null || arrow == default)
        {
            arrow = arrowPool.GetProjecttile();
        }
        arrow.transform.position = arrowPos.position;
        Vector3 dir = ((mController.targetSearch.hit.transform.position + Vector3.up) - arrow.transform.position).normalized;
        arrow.transform.forward = dir;
        arrow.SetActive(true);

    } // ShootArrow

    //! �������� �̺�Ʈ�Լ�
    public override void ExitAttack()
    {
        damage = defaultDamage;
        weapon.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        // �������� �� ������ ����
        mController.isDelay = true;
    } // ExitAttack
    #endregion // ���� ó�� (Collider, RayCast)

    #region ��ųA (ȭ���)
    //! ��ųA �Լ� (ȭ���)
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(LookAtTarget());
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! ��ųA ��� �̺�Ʈ�Լ�
    private void UseSkillA()
    {
        StartCoroutine(OnEffectSkillA());
    } //UseSkillA

    //! ��ųA ���������� �Լ�
    private void SkillA_Damage(Vector3 _effectPos)
    {
        damageMessage.damageAmount = defaultDamage * 2;
        RaycastHit[] hits = Physics.SphereCastAll(_effectPos, 2.5f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
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
    } // SkillA_Damage

    //! ��ųA ����Ʈ �ڷ�ƾ�Լ�
    private IEnumerator OnEffectSkillA()
    {
        Vector3 pos = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
        // ���ݹ��� ǥ��
        mController.attackIndicator.GetCircleIndicator(pos, 5f, 1.5f);
        yield return new WaitForSeconds(1.5f);
        GameObject effectObj = Instantiate(skillA_Prefab);
        ParticleSystem effect = effectObj.GetComponent<ParticleSystem>();
        effectObj.transform.position = pos;
        effect.Play();
        SkillA_Damage(effectObj.transform.position);
        yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constant);
        Destroy(effectObj);
    } // OnEffectSkillA

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
    #endregion // ��ųA (��� ���)

    #region Ÿ�� ����
    //! Ÿ���� �ٶ󺸴� �ڷ�ƾ�Լ�
    private IEnumerator LookAtTarget()
    {
        isAttackDelay = false;
        while (isAttackDelay == false)
        {
            if (mController.enumState != MonsterController.MonsterState.SKILL
                && mController.enumState != MonsterController.MonsterState.ATTACK)
            {
                isAttackDelay = true;
                yield break;
            }
            mController.transform.LookAt(mController.targetSearch.hit.transform.position);
            yield return null;
        }
    } // LookTarget

    //! Ÿ�� �ٶ󺸱� �����ϴ� �̺�Ʈ�Լ�
    private void OffLookAtTarget()
    {
        isAttackDelay = true;
    } // OffLookAtTarget
    #endregion // Ÿ�� ����

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

    private void AttackASound()
    {
        mController.monsterAudio.clip = attackAClip;
        mController.monsterAudio.Play();
    } // AttackASound
    #endregion // ���� ����
    //! } �ذ�ü� �׸� region ����
} // SkeletonArcher
