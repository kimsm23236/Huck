using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    //! ������ ���� ����
    public enum MonsterState
    {
        IDLE = 0,
        MOVE,
        SEARCH,
        ATTACK,
        HIT,
        DEAD
    }; //MonsterState

    private Dictionary<MonsterState, IMonsterState> dicState = new Dictionary<MonsterState, IMonsterState>(); // ������ ���¸� ���� ��ųʸ�
    private MStateMachine mStateMachine; // ������ ���¸� ó���� ������Ʈ�ӽ�
    public MStateMachine MStateMachine { get; private set; }
    public Monster monster;
    public MonsterState enumState = MonsterState.IDLE; // ������ ���� ���¸� üũ�ϱ� ���� ����
    public float currentHp; // ������ ���� HP ����
    public Rigidbody monsterRb = default;
    public Animator monsterAni = default;
    public AudioSource monsterAudio = default;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monster.monsterHp;
        monsterRb = gameObject.GetComponent<Rigidbody>();
        monsterAni = gameObject.GetComponent<Animator>();
        monsterAudio = gameObject.GetComponent<AudioSource>();

        // { �� ���¸� Dictionary�� ����
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
        // } �� ���¸� Dictionary�� ����

        // �Է¹��� ���¸� ó���� MStateMachine �ʱ�ȭ 
        MStateMachine = new MStateMachine(idle, this);
    }

    // Update is called once per frame
    void Update()
    {
        MStateMachine.DoUpdate();
    } // Update

    void FixedUpdate()
    {
        MStateMachine.DoFixedUpdate();
    } // FixedUpdate

    //interface�� ��ӹ��� Ŭ������ MonoBehaviour�� ��� ���� ���ؼ� �ڷ�ƾ�� ��� ��������� �Լ�
    public void CoroutineDeligate(IEnumerator func)
    {
        StartCoroutine(func);
    } // CoroutineDeligate
}
