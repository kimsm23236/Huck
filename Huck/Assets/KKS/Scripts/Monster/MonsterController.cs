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
    private bool isSpawn = true;
    public MStateMachine MStateMachine { get; private set; }
    public MonsterState enumState = MonsterState.IDLE; // ������ ���� ���¸� üũ�ϱ� ���� ����
    [SerializeField] private bool isBattle = false; // ������ ���������� ���� distance�� ���ϴ� �ڵ� ���� ����
    public MonsterHpBar hpBar = default; // HpBar ������Ʈ
    public AttackIndicator attackIndicator = default; // ���ݹ��� ������ pool�� ������ ����
    [HideInInspector] public Monster monster; // ���� ����
    [HideInInspector] public Rigidbody monsterRb = default; // ������ٵ�
    [HideInInspector] public Animator monsterAni = default; // �ִϸ�����
    [HideInInspector] public AudioSource monsterAudio = default; // �����
    [HideInInspector] public TargetSearchRay targetSearch = default; // Ž������ �����ɽ�Ʈ
    [HideInInspector] public NavMeshAgent mAgent = default; // �׺�Ž�
    [HideInInspector] public GameObject attacker = default; // �������� ���� ����� ������ ���� ����
    [HideInInspector] public bool isDelay = false; // Delay���� üũ ����
    [HideInInspector] public bool isHit = false; // HIT���� üũ ����
    [HideInInspector] public bool isDead = false; // Dead���� üũ ����
    [HideInInspector] public float distance; // Ÿ�ٰ��� �Ÿ� ����
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

    #region ������ ������ ���� ������ó�� �Լ�
    //! ���� ���� �ڷ�ƾ�Լ�
    private IEnumerator Spawn()
    {
        monsterAni.SetTrigger("isSpawn");
        isSpawn = true;
        // �⺻���°� Idle�̱� ������ ����Clip�� Spawn���� ���ŵǵ��� 0.1�� ��ٸ�
        yield return null;
        float waitTime = monsterAni.GetCurrentAnimatorStateInfo(0).length;
        if (monster.monsterType == Monster.MonsterType.BOSS)
        {
            // ���������� ��� Ÿ���� �������� �ȱ��� �ö����� ���
            monsterAni.speed = 0.5f;
            waitTime = waitTime * 2f;
            monsterAni.StartPlayback();
            bool isStart = false;
            while (isStart == false)
            {
                float distance = Vector3.SqrMagnitude(transform.position - target.transform.position);
                // SqrMagnitude�� �Ÿ��� ���ؼ� �������� ��
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

    //! ���ݹ����� ó���ϴ� �Լ� (interface ���)
    public void TakeDamage(DamageMessage message)
    {
        // ����, ���� ������ �� ����ó��
        if (isSpawn == true || isDead == true || message.causer.tag == GData.ENEMY_MASK)
        {
            return;
        }
        monster.monsterHp -= message.damageAmount;
        // hpBar�� ǥ�õ� hp���� ����
        hpBar.InitHpBar((float)monster.monsterHp);
        // HP�� 0 ���ϸ� ���
        if (monster.monsterHp <= 0f)
        {
            isDead = true;
            hpBar.gameObject.SetActive(false);
            monster.ExitAttack();
            MStateMachine.SetState(dicState[MonsterState.DEAD]);
            return;
        }
        // �������� �ƴ� ���� Hit���·� ��ȯ�ϱ� ���� ����ó��
        if (enumState != MonsterState.ATTACK && enumState != MonsterState.SKILL)
        {
            isHit = true;
        }
        attacker = message.causer;
        //Debug.Log($"{attacker.name}���� {message.damageAmount} ��������! ����ü��:{monster.monsterHp}, {isHit}");
    } // TakeDamage
    #endregion // ������ ������ ������ó�� �Լ�

    #region MonoBehaviour�� ��ӹ��� �ʴ� Ŭ�������� ��� ����� ��������� �Լ�����
    //! �ڷ�ƾ�� ��� ��������� �Լ�
    public void CoroutineDeligate(IEnumerator func)
    {
        StartCoroutine(func);
    } // CoroutineDeligate

    //! �ڷ�ƾ�� ��� ��������� �Լ�
    public void StopCoroutineDeligate(IEnumerator func)
    {
        StopCoroutine(func);
    } // StopCoroutineDeligate

    //! ��Ʈ���̾ ��� ��������� �Լ�
    public void DestroyObj(GameObject obj)
    {
        Destroy(obj);
    } // DestroyObj
    #endregion // MonoBehaviour�� ��ӹ��� �ʴ� Ŭ�������� ��� ����� ��������� �Լ�����

    #region ���ǿ� ���� ������ ���� ���ϴ� �Լ�
    //! ���� ���� ���ϴ� �Լ�
    private void MonsterSetState()
    {
        // Ž���������� Ÿ���� ������ Ž�����·� ��ȯ
        if (targetSearch.hit == null)
        {
            MStateMachine.SetState(dicState[MonsterState.SEARCH]);
        }
        else
        {
            distance = Vector3.Distance(targetSearch.hit.transform.position, transform.position);
            // ���ݻ��� ���� ������ ���·� ��ȯ
            if (isDelay == true)
            {
                MStateMachine.SetState(dicState[MonsterState.DELAY]);
            }
            // ������ ���ϸ� HIT���·� ��ȯ
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

            // �̵����·� ��ȯ üũ
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
                            MStateMachine.SetState(dicState[MonsterState.SKILL]);
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
        }
        // } Ÿ���� ���ݻ�Ÿ� �ȿ� ������ ���� �� ��ų ���·� ��ȯ
    } // MonsterSetState
    #endregion // ���ǿ� ���� ������ ���� ���ϴ� �Լ�

    
} // MonsterController
