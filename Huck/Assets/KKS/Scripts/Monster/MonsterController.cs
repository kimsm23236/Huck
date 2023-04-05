using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour, IDamageable
{
    //! 몬스터의 상태 종류
    public enum MonsterState
    {
        IDLE = 0,
        MOVE,
        SEARCH,
        ATTACK,
        SKILL,
        DELAY,
        HIT,
        DEAD
    }; //MonsterState

    private Dictionary<MonsterState, IMonsterState> dicState = new Dictionary<MonsterState, IMonsterState>(); // 몬스터의 상태를 담을 딕셔너리
    private MStateMachine mStateMachine; // 몬스터의 상태를 처리할 스테이트머신
    private bool isSpawn = true;
    public MStateMachine MStateMachine { get; private set; }
    public MonsterState enumState = MonsterState.IDLE; // 몬스터의 현재 상태를 체크하기 위한 변수
    [SerializeField] private bool isBattle = false; // 몬스터의 감지범위에 따라 distance를 구하는 코드 실행 조건
    public MonsterHpBar hpBar = default; // HpBar 오브젝트
    public AttackIndicator attackIndicator = default; // 공격범위 지시자 pool에 접근할 변수
    [HideInInspector] public Monster monster; // 몬스터 정보
    [HideInInspector] public Rigidbody monsterRb = default; // 리지드바디
    [HideInInspector] public Animator monsterAni = default; // 애니메이터
    [HideInInspector] public AudioSource monsterAudio = default; // 오디오
    [HideInInspector] public TargetSearchRay targetSearch = default; // 탐색범위 레이케스트
    [HideInInspector] public NavMeshAgent mAgent = default; // 네비매쉬
    [HideInInspector] public GameObject attacker = default; // 데미지를 가한 상대의 정보를 담을 변수
    [HideInInspector] public bool isDelay = false; // Delay상태 체크 변수
    [HideInInspector] public bool isHit = false; // HIT상태 체크 변수
    [HideInInspector] public bool isDead = false; // Dead상태 체크 변수
    [HideInInspector] public float distance; // 타겟과의 거리 변수
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        monsterRb = gameObject.GetComponent<Rigidbody>();
        monsterAni = gameObject.GetComponent<Animator>();
        monsterAudio = gameObject.GetComponent<AudioSource>();
        targetSearch = gameObject.GetComponent<TargetSearchRay>();
        mAgent = gameObject.GetComponent<NavMeshAgent>();
        attackIndicator = GFunc.GetRootObj("AttackIndicator").GetComponent<AttackIndicator>();
        mAgent.acceleration = 100f;
        mAgent.angularSpeed = 180f;
        mAgent.speed = monster.moveSpeed;
        mAgent.enabled = true;

        // { 각 상태를 Dictionary에 저장
        IMonsterState idle = new MonsterIdle();
        IMonsterState move = new MonsterMove();
        IMonsterState search = new MonsterSearch();
        IMonsterState attack = new MonsterAttack();
        IMonsterState skill = new MonsterSkill();
        IMonsterState delay = new MonsterDelay();
        IMonsterState hit = new MonsterHit();
        IMonsterState dead = new MonsterDead();

        dicState.Add(MonsterState.IDLE, idle);
        dicState.Add(MonsterState.MOVE, move);
        dicState.Add(MonsterState.SEARCH, search);
        dicState.Add(MonsterState.ATTACK, attack);
        dicState.Add(MonsterState.SKILL, skill);
        dicState.Add(MonsterState.DELAY, delay);
        dicState.Add(MonsterState.HIT, hit);
        dicState.Add(MonsterState.DEAD, dead);
        // } 각 상태를 Dictionary에 저장

        // 입력받은 상태를 처리할 MStateMachine 초기화 
        MStateMachine = new MStateMachine(idle, this);
        target = GameManager.Instance.playerObj;
        StartCoroutine(Spawn());
        hpBar.gameObject.SetActive(false);
    } // Start

    // Update is called once per frame
    void Update()
    {
        if (isSpawn == false && isDead == false)
        {
            MonsterSetState();
        }
        MStateMachine.DoUpdate();
    } // Update

    void FixedUpdate()
    {
        MStateMachine.DoFixedUpdate();
    } // FixedUpdate

    #region 몬스터의 스폰과 공용 데미지처리 함수
    //! 몬스터 스폰 코루틴함수
    private IEnumerator Spawn()
    {
        monsterAni.SetTrigger("isSpawn");
        isSpawn = true;
        // 기본상태가 Idle이기 때문에 현재Clip이 Spawn으로 갱신되도록 0.1초 기다림
        yield return null;
        float waitTime = monsterAni.GetCurrentAnimatorStateInfo(0).length;
        if (monster.monsterType == Monster.MonsterType.BOSS)
        {
            // 보스몬스터일 경우 타겟이 일정범위 안까지 올때까지 대기
            monsterAni.speed = 0.5f;
            waitTime = waitTime * 2f;
            monsterAni.StartPlayback();
            bool isStart = false;
            while (isStart == false)
            {
                float distance = Vector3.SqrMagnitude(transform.position - target.transform.position);
                // SqrMagnitude로 거리를 구해서 제곱으로 비교
                if (distance <= 15f * 15f)
                {
                    isStart = true;
                }
                yield return null;
            }
            monsterAni.StopPlayback();
            GameManager.Instance.StartBossBGM();
        }
        //Debug.Log($"{monsterAni.GetCurrentAnimatorClipInfo(0)[0].clip.name}");
        //Debug.Log($"{monster.monsterName}, {monsterAni.GetCurrentAnimatorStateInfo(0).length}");
        yield return new WaitForSeconds(waitTime);
        monsterAni.speed = 1f;
        monsterAni.SetTrigger("isRoar");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(monsterAni.GetCurrentAnimatorStateInfo(0).length);
        isSpawn = false;
        if (monster.monsterType == Monster.MonsterType.BOSS)
        {
            hpBar.gameObject.SetActive(true);
        }
    } // Spawn

    //! 공격받으면 처리하는 함수 (interface 상속)
    public void TakeDamage(DamageMessage message)
    {
        // 스폰, 죽음 상태일 때 무적처리
        if (isSpawn == true || isDead == true || message.causer.tag == GData.ENEMY_MASK)
        {
            return;
        }
        monster.monsterHp -= message.damageAmount;
        // hpBar에 표시될 hp정보 전달
        hpBar.InitHpBar((float)monster.monsterHp);
        // HP가 0 이하면 사망
        if (monster.monsterHp <= 0f)
        {
            isDead = true;
            hpBar.gameObject.SetActive(false);
            monster.ExitAttack();
            MStateMachine.SetState(dicState[MonsterState.DEAD]);
            return;
        }
        // 공격중이 아닐 때만 Hit상태로 전환하기 위한 예외처리
        if (enumState != MonsterState.ATTACK && enumState != MonsterState.SKILL)
        {
            isHit = true;
        }
        attacker = message.causer;
        //Debug.Log($"{attacker.name}한테 {message.damageAmount} 피해입음! 현재체력:{monster.monsterHp}, {isHit}");
    } // TakeDamage
    #endregion // 몬스터의 스폰과 데미지처리 함수

    #region MonoBehaviour를 상속받지 않는 클래스에서 대신 기능을 수행시켜줄 함수모음
    //! 코루틴을 대신 실행시켜줄 함수
    public void CoroutineDeligate(IEnumerator func)
    {
        StartCoroutine(func);
    } // CoroutineDeligate

    //! 코루틴을 대신 종료시켜줄 함수
    public void StopCoroutineDeligate(IEnumerator func)
    {
        StopCoroutine(func);
    } // StopCoroutineDeligate

    //! 디스트로이어를 대신 실행시켜줄 함수
    public void DestroyObj(GameObject obj)
    {
        Destroy(obj);
    } // DestroyObj
    #endregion // MonoBehaviour를 상속받지 않는 클래스에서 대신 기능을 수행시켜줄 함수모음

    #region 조건에 따라 몬스터의 상태 정하는 함수
    //! 몬스터 상태 정하는 함수
    private void MonsterSetState()
    {
        // 탐색범위안의 타겟이 없으면 탐색상태로 전환
        if (targetSearch.hit == null)
        {
            MStateMachine.SetState(dicState[MonsterState.SEARCH]);
        }
        else
        {
            distance = Vector3.Distance(targetSearch.hit.transform.position, transform.position);
            // 공격상태 이후 딜레이 상태로 전환
            if (isDelay == true)
            {
                MStateMachine.SetState(dicState[MonsterState.DELAY]);
            }
            // 공격을 당하면 HIT상태로 전환
            if (isHit == true)
            {
                MStateMachine.SetState(dicState[MonsterState.HIT]);
            }

            if (monster.monsterType != Monster.MonsterType.BOSS)
            {
                if (distance < 10f)
                {
                    hpBar.gameObject.SetActive(true);
                }
                else
                {
                    hpBar.gameObject.SetActive(false);
                }
            }

            // 이동상태로 전환 체크
            if (enumState != MonsterState.ATTACK
                && enumState != MonsterState.SKILL
                && enumState != MonsterState.HIT
                && enumState != MonsterState.DELAY)
            {
                MStateMachine.SetState(dicState[MonsterState.MOVE]);
            }
            // { 타겟이 공격사거리 안에 있으면 공격 및 스킬 상태로 전환
            if (distance <= monster.attackRange && enumState != MonsterState.DELAY && enumState != MonsterState.HIT)
            {
                // 몬스터의 스킬이 사용가능할 때
                if (enumState != MonsterState.ATTACK && monster.useSkill == true)
                {
                    // 몬스터의 원거리 스킬 유무에 따라 실행
                    switch (monster.isNoRangeSkill)
                    {
                        case true:
                            if (distance <= monster.meleeAttackRange)
                            {
                                MStateMachine.SetState(dicState[MonsterState.SKILL]);
                            }
                            break;
                        case false:
                            MStateMachine.SetState(dicState[MonsterState.SKILL]);
                            break;
                    } // switch end
                } // if end
                  // 스킬상태가 아니면 공격상태 진입 여부 체크
                if (enumState != MonsterState.SKILL)
                {
                    // 몬스터의 원거리 공격 유무에 따라 실행
                    switch (monster.isNoRangeAttack)
                    {
                        case true:
                            if (distance <= monster.meleeAttackRange)
                            {
                                MStateMachine.SetState(dicState[MonsterState.ATTACK]);
                            }
                            break;
                        case false:
                            MStateMachine.SetState(dicState[MonsterState.ATTACK]);
                            break;
                    } // switch end
                } // if end
            }
        }
        // } 타겟이 공격사거리 안에 있으면 공격 및 스킬 상태로 전환
    } // MonsterSetState
    #endregion // 조건에 따라 몬스터의 상태 정하는 함수
} // MonsterController
