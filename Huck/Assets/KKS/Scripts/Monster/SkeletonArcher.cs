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
            CheckUseSkill();
            SkillA();
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

    //! { 해골궁수 항목별 region 모음
    #region 공격 처리 (Collider)
    //! 공격 처리 이벤트함수 (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! 화살 쏘는 함수
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

    //! 공격종료 이벤트함수
    public override void ExitAttack()
    {
        damage = defaultDamage;
        weapon.SetActive(false);
        mController.monsterAni.SetBool("isAttackA", false);
        mController.monsterAni.SetBool("isAttackB", false);
        mController.monsterAni.SetBool("isSkillA", false);
        // 공격종료 후 딜레이 시작
        mController.isDelay = true;
    } // ExitAttack
    #endregion // 공격 처리 (Collider, RayCast)

    #region 스킬A (화살비)
    //! 스킬A 함수 (화살비)
    private void SkillA()
    {
        mController.monsterAni.SetBool("isSkillA", true);
        StartCoroutine(LookAtTarget());
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! 스킬A 사용 이벤트함수
    private void UseSkillA()
    {
        StartCoroutine(OnEffectSkillA());
    } //UseSkillA

    //! 스킬A 데미지판정 함수
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

    //! 스킬A 이펙트 코루틴함수
    private IEnumerator OnEffectSkillA()
    {
        Vector3 pos = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
        // 공격범위 표시
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

    //! 스킬A 쿨다운 코루틴함수
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
    #endregion // 스킬A (모아 쏘기)

    #region 타겟 조준
    //! 타겟을 바라보는 코루틴함수
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

    //! 타겟 바라보기 중지하는 이벤트함수
    private void OffLookAtTarget()
    {
        isAttackDelay = true;
    } // OffLookAtTarget
    #endregion // 타겟 조준

    #region 사운드 모음
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
    #endregion // 사운드 모음
    //! } 해골궁수 항목별 region 모음
} // SkeletonArcher
