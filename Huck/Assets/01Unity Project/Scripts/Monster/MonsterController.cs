using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    //! 몬스터의 상태 종류
    public enum MonsterState
    {
        IDLE = 0,
        MOVE,
        SEARCH,
        ATTACK,
        SkillA,
        HIT,
        DEAD
    }; //MonsterState

    private Dictionary<MonsterState, IMonsterState> dicState = new Dictionary<MonsterState, IMonsterState>(); // 몬스터의 상태를 담을 딕셔너리
    private MStateMachine mStateMachine; // 몬스터의 상태를 처리할 스테이트머신
    public MStateMachine MStateMachine { get; private set; }
    public Monster monster;
    public MonsterState enumState = MonsterState.IDLE; // 몬스터의 현재 상태를 체크하기 위한 변수
    public float currentHp; // 몬스터의 현재 HP 변수
    public Rigidbody monsterRb = default;
    public Animator monsterAni = default;
    public AudioSource monsterAudio = default;
    public TargetSearchRay targetSearch = default;
    public NavMeshAgent mAgent;
    public bool isAttack = false;
    //Test
    public Transform targetPos;
    public float distance; // 타겟과의 거리 변수
    //Test

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monster.monsterHp;
        monsterRb = gameObject.GetComponent<Rigidbody>();
        monsterAni = gameObject.GetComponent<Animator>();
        monsterAudio = gameObject.GetComponent<AudioSource>();
        targetSearch = gameObject.GetComponent<TargetSearchRay>();
        mAgent = gameObject.GetComponent<NavMeshAgent>();

        // { 각 상태를 Dictionary에 저장
        IMonsterState idle = new MonsterIdle();
        IMonsterState move = new MonsterMove();
        IMonsterState search = new MonsterSearch();
        IMonsterState attack = new MonsterAttack();
        IMonsterState hit = new MonsterHit();
        IMonsterState dead = new MonsterDead();

        dicState.Add(MonsterState.IDLE, idle);
        dicState.Add(MonsterState.MOVE, move);
        dicState.Add(MonsterState.SEARCH, search);
        dicState.Add(MonsterState.ATTACK, attack);
        dicState.Add(MonsterState.HIT, hit);
        dicState.Add(MonsterState.DEAD, dead);
        // } 각 상태를 Dictionary에 저장

        // 입력받은 상태를 처리할 MStateMachine 초기화 
        MStateMachine = new MStateMachine(idle, this);
    }

    // Update is called once per frame
    void Update()
    {
        // { Test
        MonsterSetState();
        // } Test
        MStateMachine.DoUpdate();
    } // Update

    void FixedUpdate()
    {
        MStateMachine.DoFixedUpdate();
    } // FixedUpdate

    //interface를 상속받은 클래스는 MonoBehaviour를 상속 받지 못해서 코루틴을 대신 실행시켜줄 함수
    public void CoroutineDeligate(IEnumerator func)
    {
        StartCoroutine(func);
    } // CoroutineDeligate

    //! 몬스터 상태 정하는 함수
    private void MonsterSetState()
    {
        float _distance = Vector3.Distance(this.transform.position, targetPos.position);
        // 타겟이 몬스터의 탐색범위 밖에 있으면 추적
        if (_distance > monster.searchRange)
        {
            MStateMachine.SetState(dicState[MonsterState.SEARCH]);
            return;
        }

        // 타겟이 몬스터의 탐색범위 안에 있을 때
        distance = Vector3.Distance(this.transform.position, targetSearch.hit.transform.position);
        // 공격상태가 아니면 이동상태 시작
        if (enumState != MonsterState.ATTACK)
        {
            MStateMachine.SetState(dicState[MonsterState.MOVE]);
        }
        // 타겟이 공격사거리 안에 있고 공격중이 아니라면 공격상태 시작
        if (distance <= monster.attackRange)
        {
            if (distance > monster.meleeAttackRange && isAttack == false)
            {
                MStateMachine.SetState(dicState[MonsterState.ATTACK]);
            }
            if (distance <= monster.meleeAttackRange)
            {
                MStateMachine.SetState(dicState[MonsterState.ATTACK]);
            }
        }
    } // MonsterSetState
}
