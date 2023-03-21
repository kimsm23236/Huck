using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    //! ������ ���� ����
    public enum MonsterState
    {
        IDLE = 0,
        MOVE,
        SEARCH,
        ATTACK,
        SKILL,
        HIT,
        DEAD
    }; //MonsterState

    private Dictionary<MonsterState, IMonsterState> dicState = new Dictionary<MonsterState, IMonsterState>(); // ������ ���¸� ���� ��ųʸ�
    private MStateMachine mStateMachine; // ������ ���¸� ó���� ������Ʈ�ӽ�
    private bool isSpawn = true;
    public MStateMachine MStateMachine { get; private set; }
    [HideInInspector] public Monster monster;
    public MonsterState enumState = MonsterState.IDLE; // ������ ���� ���¸� üũ�ϱ� ���� ����
    public float currentHp; // ������ ���� HP ����
    [HideInInspector] public Rigidbody monsterRb = default;
    [HideInInspector] public Animator monsterAni = default;
    [HideInInspector] public AudioSource monsterAudio = default;
    [HideInInspector] public TargetSearchRay targetSearch = default;
    [HideInInspector] public NavMeshAgent mAgent = default;
    // { Test
    public GameObject target;
    public float distance; // Ÿ�ٰ��� �Ÿ� ����
    // } Test

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monster.monsterHp;
        monsterRb = gameObject.GetComponent<Rigidbody>();
        monsterAni = gameObject.GetComponent<Animator>();
        monsterAudio = gameObject.GetComponent<AudioSource>();
        targetSearch = gameObject.GetComponent<TargetSearchRay>();
        mAgent = gameObject.GetComponent<NavMeshAgent>();
        mAgent.acceleration = 100f;
        mAgent.speed = monster.moveSpeed;

        // { �� ���¸� Dictionary�� ����
        IMonsterState idle = new MonsterIdle();
        IMonsterState move = new MonsterMove();
        IMonsterState search = new MonsterSearch();
        IMonsterState attack = new MonsterAttack();
        IMonsterState skill = new MonsterSkill();
        IMonsterState hit = new MonsterHit();
        IMonsterState dead = new MonsterDead();

        dicState.Add(MonsterState.IDLE, idle);
        dicState.Add(MonsterState.MOVE, move);
        dicState.Add(MonsterState.SEARCH, search);
        dicState.Add(MonsterState.ATTACK, attack);
        dicState.Add(MonsterState.SKILL, skill);
        dicState.Add(MonsterState.HIT, hit);
        dicState.Add(MonsterState.DEAD, dead);
        // } �� ���¸� Dictionary�� ����

        // �Է¹��� ���¸� ó���� MStateMachine �ʱ�ȭ 
        MStateMachine = new MStateMachine(idle, this);
        // �������Ӹ��� ���� ������ �ʿ䰡 ��� 0.5�ʸ��� Ÿ�� ���� ����
        InvokeRepeating("GetTarget", 0f, 0.5f);
        StartCoroutine(Spawn());
    } // Start

    // Update is called once per frame
    void Update()
    {
        if (isSpawn == false)
        {
            MonsterSetState();
        }
        MStateMachine.DoUpdate();
    } // Update

    void FixedUpdate()
    {
        MStateMachine.DoFixedUpdate();
    } // FixedUpdate

    //! Ÿ���� ������ �������� �Լ�
    private void GetTarget()
    {
        target = GameManager.Instance.playerObj;
    } // GetTarget

    //! ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator Spawn()
    {
        monsterAni.SetTrigger("isSpawn");
        isSpawn = true;
        // �⺻���°� Idle�̱� ������ ����Clip�� Spawn���� ���ŵǵ��� 0.1�� ��ٸ�
        yield return null;
        //Debug.Log($"{monsterAni.GetCurrentAnimatorClipInfo(0)[0].clip.name}");
        //Debug.Log($"{monster.monsterName}, {monsterAni.GetCurrentAnimatorStateInfo(0).length}");
        yield return new WaitForSeconds(monsterAni.GetCurrentAnimatorStateInfo(0).length);
        isSpawn = false;
    } // Spawn

    //! interface�� ��ӹ��� Ŭ������ MonoBehaviour�� ��� ���� ���ؼ� �ڷ�ƾ�� ��� ��������� �Լ�
    public void CoroutineDeligate(IEnumerator func)
    {
        StartCoroutine(func);
    } // CoroutineDeligate

    //! ���� ���� ���ϴ� �Լ�
    private void MonsterSetState()
    {
        if (monster.monsterHp < currentHp)
        {
            MStateMachine.SetState(dicState[MonsterState.HIT]);
        }

        float _distance = Vector3.Distance(this.transform.position, target.transform.position);
        // Ÿ���� ������ Ž������ �ۿ� ������ ����
        if (_distance > monster.searchRange)
        {
            MStateMachine.SetState(dicState[MonsterState.SEARCH]);
            return;
        }

        // Ÿ���� ������ Ž������ �ȿ� ���� �� Ž�� ����
        targetSearch.SearchTarget();
        distance = Vector3.Distance(this.transform.position, targetSearch.hit.transform.position);
        // ����, ��ų ���°� �ƴϸ� �̵����·� ��ȯ
        if (enumState != MonsterState.ATTACK && enumState != MonsterState.SKILL)
        {
            MStateMachine.SetState(dicState[MonsterState.MOVE]);
        }
        // { Ÿ���� ���ݻ�Ÿ� �ȿ� ������ ���� �� ��ų ���·� ��ȯ
        if (distance <= monster.attackRange)
        {
            // ������ ��ų�� ��밡���� ��
            if (enumState != MonsterState.ATTACK && (monster.useSkillA == true || monster.useSkillB == true))
            {
                // ������ ���Ÿ� ��ų ������ ���� ����
                switch (monster.isNoRangeSkill)
                {
                    case true:
                        if (distance <= monster.meleeAttackRange)
                        {
                            MStateMachine.SetState(dicState[MonsterState.SKILL]);
                        }
                        break;
                    case false:
                        if (distance > monster.meleeAttackRange)
                        {
                            MStateMachine.SetState(dicState[MonsterState.SKILL]);
                        }
                        break;
                } // switch end
            } // if end
            // if : ��ų�� ��� ���Ұ����ϸ� ����
            if (enumState != MonsterState.SKILL)
            {
                // ������ ���Ÿ� ���� ������ ���� ����
                switch (monster.isNoRangeAttack)
                {
                    case true:
                        if (distance <= monster.meleeAttackRange)
                        {
                            MStateMachine.SetState(dicState[MonsterState.ATTACK]);
                        }
                        break;
                    case false:
                        if (distance > monster.meleeAttackRange)
                        {
                            MStateMachine.SetState(dicState[MonsterState.ATTACK]);
                        }
                        break;
                } // switch end
            } // if end
        }
        // } Ÿ���� ���ݻ�Ÿ� �ȿ� ������ ���� �� ��ų ���·� ��ȯ

    } // MonsterSetState
} // MonsterController
