using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour, IDamageable
{
    //! ������ ���� ����
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

    private Dictionary<MonsterState, IMonsterState> dicState = new Dictionary<MonsterState, IMonsterState>(); // ������ ���¸� ���� ��ųʸ�
    private MStateMachine mStateMachine; // ������ ���¸� ó���� ������Ʈ�ӽ�
    [SerializeField] private bool isBattle = false; // ������ ���������� ���� distance�� ���ϴ� �ڵ� ���� ����
    private bool isSpawn = true;
    public MStateMachine MStateMachine { get; private set; }
    [HideInInspector] public Monster monster;
    public MonsterState enumState = MonsterState.IDLE; // ������ ���� ���¸� üũ�ϱ� ���� ����
    public int currentHp; // ������ ���� HP ����
    [HideInInspector] public Rigidbody monsterRb = default;
    [HideInInspector] public Animator monsterAni = default;
    [HideInInspector] public AudioSource monsterAudio = default;
    [HideInInspector] public TargetSearchRay targetSearch = default;
    [HideInInspector] public NavMeshAgent mAgent = default;
    [HideInInspector] public bool isDelay = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public GameObject attacker = default; // �������� ���� ����� ������ ���� ����
    // { Test
    public GameObject target;
    public float distance; // Ÿ�ٰ��� �Ÿ� ����
    public bool useSkill;
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
        mAgent.angularSpeed = 180f;
        mAgent.speed = monster.moveSpeed;

        // { �� ���¸� Dictionary�� ����
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
        // �ν����� ��ų ��밡������ Ȯ�ο�
        useSkill = monster.useSkill;

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

    //! ���ݹ����� ó���ϴ� �Լ� (interface ���)
    public void TakeDamage(GameObject _attacker, int _damage)
    {
        // ���������� �� ����ó��
        if (isSpawn == true)
        {
            return;
        }
        // �������� �ƴ� ���� Hit���·� ��ȯ�ϱ� ���� ����ó��
        if (enumState != MonsterState.ATTACK && enumState != MonsterState.SKILL)
        {
            isHit = true;
        }
        monster.monsterHp -= _damage;
        attacker = _attacker;
        if (monster.monsterHp <= 0f)
        {
            isDead = true;
        }
        Debug.Log($"{_attacker.name}���� {_damage} ��������! ����ü��:{monster.monsterHp}, {isHit}");
    } // TakeDamage

    //! interface�� ��ӹ��� Ŭ������ MonoBehaviour�� ��� ���� ���ؼ� �ڷ�ƾ�� ��� ��������� �Լ�
    public void CoroutineDeligate(IEnumerator func)
    {
        StartCoroutine(func);
    } // CoroutineDeligate

    //! �ڷ�ƾ�� ��� ��������� �Լ�
    public void StopCoroutineDeligate(IEnumerator func)
    {
        StopCoroutine(func);
    } // StopCoroutineDeligate

    //! ���� ���� ���ϴ� �Լ�
    private void MonsterSetState()
    {
        // ���ݻ��� ���� ������ ���·� ��ȯ
        if (isDelay == true)
        {
            MStateMachine.SetState(dicState[MonsterState.DELAY]);
        }
        // ����, ��ų ������ ��� HIT ���� ����X (�´¸�� ���� ���ϱ� ���� ����ó��) 
        if (isHit == true && (enumState == MonsterState.ATTACK || enumState == MonsterState.SKILL))
        {
            isHit = false;
        }
        // ������ ���ϸ� HIT���·� ��ȯ
        if (isHit == true)
        {
            MStateMachine.SetState(dicState[MonsterState.HIT]);
        }
        // ��Ʋ���� �ƴ� ���� Distance ���ϴ°� ���߱� ���� ����ó�� (����ȭ)
        if (isBattle == false)
        {
            float _distance = Vector3.Distance(transform.position, target.transform.position);
            Debug.Log($"��Ʋ���� �ƴ� : {_distance}");
            // Ÿ���� ������ Ž������ �ۿ� ������ ����
            if (_distance > monster.searchRange)
            {
                MStateMachine.SetState(dicState[MonsterState.SEARCH]);
            }
            else
            {
                isBattle = true;
            }
            return;
        }
        // Ÿ���� ������ Ž������ �ȿ� ���� ���� Ž�� ���� (����ȭ)
        targetSearch.SearchTarget();
        distance = Vector3.Distance(targetSearch.hit.transform.position, transform.position);
        // Ÿ���� ���������� ����� ��Ʋ����
        if (distance > monster.searchRange)
        {
            isBattle = false;
            return;
        }

        // ����, ��ų ���°� �ƴϸ� �̵����·� ��ȯ
        if (enumState != MonsterState.ATTACK
            && enumState != MonsterState.SKILL
            && enumState != MonsterState.HIT
            && enumState != MonsterState.DELAY)
        {
            MStateMachine.SetState(dicState[MonsterState.MOVE]);
        }
        // { Ÿ���� ���ݻ�Ÿ� �ȿ� ������ ���� �� ��ų ���·� ��ȯ
        if (distance <= monster.attackRange && enumState != MonsterState.DELAY && enumState != MonsterState.HIT)
        {
            // ������ ��ų�� ��밡���� ��
            if (enumState != MonsterState.ATTACK && monster.useSkill == true)
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
            // ��ų���°� �ƴϸ� ���ݻ��� ���� ���� üũ
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
                        MStateMachine.SetState(dicState[MonsterState.ATTACK]);
                        break;
                } // switch end
            } // if end
        }
        // } Ÿ���� ���ݻ�Ÿ� �ȿ� ������ ���� �� ��ų ���·� ��ȯ

    } // MonsterSetState
} // MonsterController
