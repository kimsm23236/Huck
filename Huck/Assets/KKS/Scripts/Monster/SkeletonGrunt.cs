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

    //! 해골그런트 공격 오버라이드
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
            // 돌진공격이 사용가능하지만 타겟이 최소사거리 안에 있을때 돌진공격 사용X Idle상태로 초기화
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

    //! 해골그런트 스킬 오버라이드
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
            // 스킬A가 사용가능하지만 타겟이 최소사거리 안에 있을때 스킬A 사용X Idle상태로 초기화
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

    //! 사용가능한 스킬이 있는지 체크하는 함수 (몬스터컨트롤러에서 상태진입 체크하기 위함)
    private void CheckUseSkill()
    {
        // 원거리스킬 사용 유무 체크
        if (useSkillA == false)
        {
            isNoRangeSkill = true;
        }
        else
        {
            isNoRangeSkill = false;
        }
        // 스킬 사용가능 유무 체크
        if (useSkillA == false && useSkillB == false)
        {
            useSkill = false;
        }
        else
        {
            useSkill = true;
        }
    } // CheckUseSkill

    //! { 해골그런트 항목별 region 모음
    #region 공격 처리 (Collider, RayCast)
    //! 무기 공격 처리 이벤트함수 (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! 어깨 공격 처리 이벤트함수 (Collider)
    private void EnableShoulderAttack()
    {
        shoulder.SetActive(true);
    } // EnableShoulderAttack

    //! 공격종료 이벤트함수
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
        // 공격종료 후 딜레이 시작
        mController.isDelay = true;
    } // ExitAttack
    #endregion // 공격 처리 (Collider, RayCast)

    #region 돌진 공격
    //! 돌진 공격 사용 거리 체크하는 코루틴함수
    private IEnumerator CheckRushDistance()
    {
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
        {
            // 타겟이 돌진 최소사거리 밖에 있으면 돌진 사용가능
            if (mController.distance >= 13f)
            {
                isNoRangeAttack = false;
                yield break;
            }
            yield return null;
        }
    } // CheckRushDistance

    //! 돌진 공격 코루틴 함수
    private IEnumerator RushAttack()
    {
        StartCoroutine(RushCooldown());
        // 돌진 공격 전 함성 시작
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
            // 돌진 마무리 공격 시작 전까지 타겟을 향하여 돌진
            if (isFinishRush == false)
            {
                mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            }
            else
            {
                // 돌진 마무리 공격 시작되면 돌진하던 방향 그대로 1초간 돌진
                timeCheck += Time.deltaTime;
                mController.mAgent.Move(mController.transform.forward * moveSpeed * Time.deltaTime);
                if (timeCheck >= 1f)
                {
                    isRush = false;
                }
            }
            // 돌진 마무리 공격 시작
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

    //! 돌진 쿨다운 코루틴함수
    private IEnumerator RushCooldown()
    {
        rushCool = 0f;
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 공격 유무 체크
        isNoRangeAttack = true;
        while (rushCool < 20f)
        {
            rushCool += Time.deltaTime;
            yield return null;
        }
        rushCool = 0f;
        isNoRangeAttack = false;
    } // RushCooldown
    #endregion // 돌진 공격

    #region 스킬A (도약 공격)
    //! 해골그런트 스킬A 함수 (도약 공격)
    private void SkillA()
    {
        // 포물선 이동함수를 사용하기 위한 Parabola 초기화
        Parabola parabola = new Parabola();
        // 몬스터가 타겟을 바라보는 방향의 반대방향을 구함
        Vector3 dir = -(mController.targetSearch.hit.transform.position - transform.position).normalized;
        // 목표위치를 dir방향으로 meleeAttackRange만큼 이동된 좌표로 설정
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * (meleeAttackRange + 1);
        StartCoroutine(parabola.ParabolaMoveToTarget(transform.position, targetPos, 1f, gameObject));
        mController.monsterAni.SetBool("isSkillA", true);
        // 공격범위 표시
        dir.y = 0f;
        Vector3 pos = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
        mController.attackIndicator.GetCircleIndicator(pos, 6f, 1f);
        StartCoroutine(SkillACooldown());
    } // SkillA

    //! 스킬A 사용 거리체크하는 코루틴함수
    private IEnumerator CheckSkillADistance()
    {
        while (useSkillA == false)
        {
            // 타겟이 스킬A 최소사거리 밖에 있으면 스킬A 사용가능
            if (mController.distance >= 13f)
            {
                useSkillA = true;
                CheckUseSkill();
                yield break;
            }
            yield return null;
        }
    } // CheckSkillADistance

    //! 스킬A 데미지판정 이벤트함수
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

    //! 스킬A 이펙트 코루틴함수
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

    //! 스킬A 데미지판정 범위 기즈모
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weapon.transform.position, 3f);
    } // OnDrawGizmos

    //! 스킬A 쿨다운 코루틴함수
    private IEnumerator SkillACooldown()
    {
        skillACool = 0f;
        // 몬스터컨트롤러에서 상태진입 시 체크할 조건 : 원거리 스킬 쿨 적용
        while (skillACool < skillA_MaxCool)
        {
            skillACool += Time.deltaTime;
            yield return null;
        }
        skillACool = 0f;
        useSkillA = true;
        CheckUseSkill();
    } // SkillACooldown
    #endregion // 스킬A (도약 공격)

    #region 스킬B (대지 가르기)
    //! 해골그런트 스킬B 함수 (대지 가르기)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
        StartCoroutine(SkillBCooldown());
    } // SkillB

    //! 스킬B 공격 코루틴함수
    private IEnumerator UseSkillB()
    {
        mController.monsterAni.SetBool("isSkillB_Start", true);
        // 공격범위 표시
        GameObject indicator = mController.attackIndicator.GetRectangIndicator(mController.isDead, transform.position, 3f, 22f, 3.5f);
        Quaternion startRotation = indicator.transform.rotation;
        bool isStart = true;
        float time = 0f;
        while (time <= 2.5f)
        {
            time += Time.deltaTime;
            // 공격범위 지시자 오브젝트의 회전축을 x는 본인걸로 유지하면서 y축만 같이 변경 (LookAt함수로 인해 공격방향이 바뀌기 때문) 
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

    //! 스킬B 이펙트 코루틴함수
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
    //! 스킬B 쿨다운 코루틴함수
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
    #endregion // 스킬B (대지 가르기)
    //! } 해골그런트 항목별 region 모음
} // SkeletonGrunt
