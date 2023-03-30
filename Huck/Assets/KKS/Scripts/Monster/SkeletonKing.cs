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

    //! 해골왕 공격 오버라이드
    public override void Attack()
    {
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        // 슬라이드 공격 조건체크
        if (slideAttackCool <= 0f && isNoRangeAttack == false && mController.distance >= 7f)
        {
            SlideAttack();
        }
        else if (slideAttackCool <= 0f && isNoRangeAttack == false && mController.distance < 7f)
        {
            StartCoroutine(CheckSlideDistance());
            // 슬라이드 공격이 사용가능하지만 타겟이 최소사거리 안에 있을때 사용X Idle상태로 초기화
            IMonsterState nextState = new MonsterIdle();
            mController.MStateMachine.onChangeState?.Invoke(nextState);
            return;
        }

        // 근접 공격 조건체크
        if (mController.distance <= meleeAttackRange)
        {
            if (crushAttackCool <= 0f)
            {
                CrushAttack();
                return;
            }
            // 모션 5개 중 랜덤으로 한개 실행
            int number = default;
            if (is2Phase == true)
            {
                // 2페이즈에선 공격 모션 E 타입 추가
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

    //! 해골왕 스킬 오버라이드
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
            // 도약 공격 스킬이 사용가능하지만 타겟이 최소사거리 안에 있을때 스킬 사용X Idle상태로 초기화
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

    //! 사용가능한 스킬이 있는지 체크하는 함수 (몬스터컨트롤러에서 상태진입 체크하기 위함)
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

    //! { 해골왕 항목별 region 모음
    #region 공격 처리 (Collider, Raycast)
    //! 공격 처리 이벤트함수 (Collider)
    private void EnableWeapon()
    {
        weapon.SetActive(true);
    } // EnableWeapon

    //! 공격종료 이벤트함수
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
        // 공격종료 후 딜레이 상태로 전환
        mController.isDelay = true;
    } // ExitAttack

    //! 공격 E타입 코루틴함수
    private IEnumerator UseAttackE()
    {
        mController.monsterAni.SetBool("isAttackE", true);
        yield return new WaitForSeconds(1f);
        float time = 0f;
        // 2페이즈 : 애니메이션 속도가 1.2배 증가한만큼 시간변경
        float maxTime = 1.5f / mController.monsterAni.speed;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            mController.mAgent.Move(mController.transform.forward * moveSpeed * 0.5f * Time.deltaTime);
            yield return null;
        }
    } // UseAttackE
    #endregion // 공격 처리 (Collider, Raycast)

    #region 보스몬스터 죽음 처리
    //! 보스몬스터 죽음 처리 함수
    public override void BossDead()
    {
        StartCoroutine(Dead());
    } // BossDead

    //! 보스몬스터 페이즈별 죽음 처리 함수
    private IEnumerator Dead()
    {
        mController.monsterAni.speed = 1f;
        mController.monsterAni.SetBool("isDead", true);
        // 죽는 모션 되감기를 위한 float트리거
        mController.monsterAni.SetFloat("RewindDead", 1f);
        yield return null;
        float deadAniTime = mController.monsterAni.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deadAniTime);
        mController.monsterAni.SetFloat("RewindDead", 0f);
        yield return new WaitForSeconds(2f);
        if (is2Phase == false)
        {
            // 1페이즈에서 죽었을 경우 부활하고 2페이즈로 전환
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
                // endTime 까지 scale을 1에서 1.5까지 늘림
                time += Time.deltaTime;
                mController.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, time / endTime);
                yield return null;
            }
            yield return new WaitForSeconds(2f);
            mController.isDead = false;
            is2Phase = true;
            // 2페이즈 전환시 애니메이션 속도 1.2배로 설정
            mController.monsterAni.speed = 1.2f;
            meleeAttackRange = 4f;
        }
        else
        {
            // 2페이즈에서 완전히 죽음
            // 밑으로 시체가 내려가게 하기위해 네비매쉬 비활성화
            mController.mAgent.enabled = false;
            // 4초에 걸쳐 총 2f만큼 밑으로 내려간 뒤에 디스트로이
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
    #endregion // 보스몬스터 죽음 처리

    #region CrushAttack 타겟유도 연속 베기
    //! 해골왕 CrushAttack 함수 (타겟유도 연속 베기)
    private void CrushAttack()
    {
        StartCoroutine(UseCrushAttack());
    } // CrushAttack

    //! CrushAttack 사용 코루틴함수
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

    //! CrushAttack 쿨다운 코루틴함수
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
    #endregion // CrushAttack 타겟유도 연속 베기

    #region 슬라이드 공격(원거리)
    //! 슬라이드 공격 함수
    private void SlideAttack()
    {
        StartCoroutine(UseSlideAttack());
    } // SlideAttack

    //! 슬라이드 공격 코루틴함수
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

    //! 슬라이드 공격 사용 거리 체크하는 코루틴함수
    private IEnumerator CheckSlideDistance()
    {
        isNoRangeAttack = true;
        while (isNoRangeAttack == true)
        {
            // 타겟이 슬라이드 최소사거리 밖에 있으면 슬라이드 공격 사용가능
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
    #endregion // 슬라이드 공격(원거리)

    #region 스킬A 해골그런트 소환
    //! 해골왕 스킬A 함수 (소환 스킬)
    private void SkillA()
    {
        StartCoroutine(UseSkillA());
    } // SkillA

    //! 스킬A 공격 코루틴함수
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
                // 소환 준비 모션 끝나면 소환 시작
                mController.monsterAni.SetBool("isSkillA_Start", false);
                mController.monsterAni.SetBool("isSkillA_Loop", true);
                isStart = false;
                Summon();
            }
            // 5초가 지나면 소환 마무리 시작
            if (isStart == false && timeCheck >= 4f)
            {
                mController.monsterAni.SetBool("isSkillA_Loop", false);
                mController.monsterAni.SetBool("isSkillA_End", true);
                isSkillA = true;
            }
            yield return null;
        }
    } // UseSkillA

    //! 스킬A 해골그런트 소환하는 함수
    private void Summon()
    {
        GetRandomPosition getRandomPos = new GetRandomPosition();
        // 원범위의 장애물이 없는 좌표를 가져옴
        Vector3 pos = getRandomPos.GetRandomCirclePos(transform.position, 10, 4);
        Vector3 dirToTarget = (mController.targetSearch.hit.transform.position - pos).normalized;
        // 해골그런트가 소환될 때 타겟을 바라보면서 소환되게 회전축 설정
        Instantiate(summonObjPrefab, pos, Quaternion.LookRotation(dirToTarget));
    } // Summon

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
    #endregion // 스킬A 해골그런트 소환

    #region 스킬B 도약 공격
    //! 해골왕 스킬B 함수 (도약 공격)
    private void SkillB()
    {
        StartCoroutine(UseSkillB());
    } // SkillB

    //! 스킬B (도약 공격) 코루틴함수
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
            // 2페이즈 : 애니메이션 속도가 1.2배 증가한만큼 시간변경
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
            // 도약준비 시간동안 타겟방향으로 천천히 이동
            time += Time.deltaTime;
            mController.mAgent.SetDestination(mController.targetSearch.hit.transform.position);
            yield return null;
        }
        mController.mAgent.speed = moveSpeed;
        mController.mAgent.ResetPath();
        // 포물선 이동함수를 사용하기 위한 Parabola 초기화
        Parabola parabola = new Parabola();
        // 몬스터가 타겟을 바라보는 방향의 반대방향을 구함
        Vector3 dir = -(mController.targetSearch.hit.transform.position - mController.transform.position).normalized;
        // 목표위치를 dir방향으로 meleeAttackRange만큼 이동된 좌표로 설정
        Vector3 targetPos = mController.targetSearch.hit.transform.position + dir * meleeAttackRange;
        mController.transform.LookAt(mController.targetSearch.hit.transform.position);
        StartCoroutine(parabola.ParabolaMoveToTarget(mController.transform.position, targetPos, leapTime, gameObject));
        // 공격범위 표시
        GameObject indicator = Instantiate(indicator_Prefab, mController.targetSearch.hit.transform.position, indicator_Prefab.transform.rotation);
        indicator.GetComponent<AttackIndicator>().InitAttackIndicator(6f, leapTime);
    } // UseSkillB

    //! 스킬B 데미지판정 이벤트함수
    private void SkillB_Damage()
    {
        StartCoroutine(OnEffectB());
        damageMessage.damageAmount = defaultDamage * 2;
        RaycastHit[] hits = Physics.SphereCastAll(weapon.transform.position, 3f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        if (hits.Length > 0)
        {
            foreach (var _hit in hits)
            {
                // if : 플레이어 또는 건축물일 때
                if (_hit.collider.tag == GData.PLAYER_MASK || _hit.collider.tag == GData.BUILD_MASK)
                {
                    _hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
                }
            }
        }
        damageMessage.damageAmount = defaultDamage;
    } // SkillB_Damage

    //! 스킬B 이펙트 코루틴함수
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

    //! 스킬B (도약 공격) 사용거리 체크하는 코루틴함수
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

    //! 스킬B 쿨다운 코루틴함수
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
    #endregion // 스킬B 도약 공격

    #region 스킬C 연속 베기
    //! 해골왕 스킬C 함수 (연속 베기)
    private void SkillC()
    {
        StartCoroutine(UseSkillC());
    } // SkillC

    //! 스킬C (연속 베기) 코루틴함수
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

    //! 스킬C 이펙트 코루틴함수
    private IEnumerator OnEffectC()
    {
        Vector3 pos = weapon.transform.position + Vector3.up * 0.5f;
        if (is2Phase == false)
        {
            // 1페이즈
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
            // 2페이즈
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

    //! 스킬C 쿨다운 코루틴함수
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
    #endregion // 스킬C 연속 베기

    #region 스킬D 칼날비
    //! 해골왕 스킬D 함수 (칼날비)
    private void SkillD()
    {
        StartCoroutine(UseSkillD());
    } // SkillD

    //! 스킬D 사용 코루틴함수
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

    //! 스킬D 이펙트 코루틴함수
    private IEnumerator OnEffectSkillD()
    {
        if (is2Phase == false)
        {
            // 1페이즈 낙뢰 이펙트
            GameObject thunder = ProjectilePool.Instance.GetProjecttile();
            ParticleSystem effect = thunder.GetComponent<ParticleSystem>();
            thunder.transform.position = mController.targetSearch.hit.transform.position + new Vector3(0f, 0.1f, 0f);
            // 공격범위 표시
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
            // 2페이즈 낙뢰 이펙트
            GetRandomPosition getRandomPos = new GetRandomPosition();
            List<Vector3> randomPosList = new List<Vector3>();
            while (randomPosList.Count < 10)
            {
                // 타겟위치 원범위에서 중복되지않는 랜덤좌표 10개 뽑아옴 
                Vector3 pos = getRandomPos.GetRandomCirclePos(mController.targetSearch.hit.transform.position, 10, 0);
                if (!randomPosList.Contains(pos))
                {
                    randomPosList.Add(pos);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                // 공격범위 표시
                GameObject indicator = Instantiate(indicator_Prefab, randomPosList[i], indicator_Prefab.transform.rotation);
                indicator.GetComponent<AttackIndicator>().InitAttackIndicator(4f, 1.5f);
                if (i % 2 == 0)
                {
                    // 짝수번째마다 0.3초 늦게 떨어지게 처리
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
                    // 짝수번째마다 0.3초 늦게 떨어지게 처리
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

    //! 스킬D 데미지판정 함수
    private void SkillD_Damage(Vector3 _effectPos)
    {
        damageMessage.damageAmount = Mathf.FloorToInt(defaultDamage * 1.5f);
        RaycastHit[] hits = Physics.SphereCastAll(_effectPos, 2f, Vector3.up, 0f, LayerMask.GetMask(GData.PLAYER_MASK, GData.BUILD_MASK));
        if (hits.Length > 0)
        {
            foreach (var _hit in hits)
            {
                // if : 플레이어 또는 건축물일 때
                if (_hit.collider.tag == GData.PLAYER_MASK || _hit.collider.tag == GData.BUILD_MASK)
                {
                    _hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damageMessage);
                }
            }
        }
        damageMessage.damageAmount = defaultDamage;
    } // SkillD_Damage

    //! 스킬D 쿨다운 코루틴함수
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
    #endregion // 스킬D
    //! } 해골왕 항목별 region 모음
} // SkeletonKing
