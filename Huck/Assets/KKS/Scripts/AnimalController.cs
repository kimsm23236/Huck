using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalController : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int hp = default;
    [SerializeField] private int maxHp = 50;
    private NavMeshAgent agent = default;
    private Animator animator = default;
    private Vector3 targetPos = default;
    private Vector3 attackerPos = default;
    private bool isMove = false;
    private bool isHit = false;
    private bool isDead = false;
    private float time = 0f;
    private float escapeTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = 100f;
        agent.angularSpeed = 180f;
        agent.enabled = true;
        hp = maxHp;
    } // Start

    // Update is called once per frame
    void Update()
    {
        SelectState();
    } // Update

    //! ���� ���� ���ϴ� �Լ�
    private void SelectState()
    {
        if (isDead == false)
        {
            if (isHit == true && escapeTime <= 0f)
            {
                StartCoroutine(Escape());
            }
            if (isMove == false && escapeTime <= 0f)
            {
                time += Time.deltaTime;
                if (time >= 3f)
                {
                    GetRandomPosition getTargetPos = new GetRandomPosition();
                    targetPos = getTargetPos.GetRandomCirclePos(transform.position, 30, 20);
                    isMove = true;
                    StartCoroutine(Move());
                    time = 0f;
                }
            }
        }
    } // SelectState

    //! ���� �̵��ϴ� �ڷ�ƾ�Լ�
    private IEnumerator Move()
    {
        animator.SetBool("isWalk", true);
        while (isMove == true)
        {
            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= 0f || isHit == true)
            {
                isMove = false;
            }
            agent.SetDestination(targetPos);
            yield return null;
        }
        agent.ResetPath();
        animator.SetBool("isWalk", false);
    } // Move

    //! Hit�����϶� �������� �ڷ�ƾ�Լ�
    private IEnumerator Escape()
    {
        Vector3 dir = -(attackerPos - transform.position).normalized;
        animator.SetBool("isRun", true);
        escapeTime = 0f;
        while (escapeTime < 2f)
        {
            escapeTime += Time.deltaTime;
            agent.Move(dir * speed * 2f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 3f * Time.deltaTime);
            yield return null;
        }
        animator.SetBool("isRun", false);
        escapeTime = 0f;
        attackerPos = default;
        isHit = false;
    } // Escape

    //! ���� ����ϴ� �ڷ�ƾ�Լ�
    private IEnumerator Dead()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        animator.SetBool("isDead", true);
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1.5f);
        // ������ ��ü�� �������� �ϱ����� �׺�Ž� ��Ȱ��ȭ
        agent.enabled = false;
        // 4�ʿ� ���� �� 2f��ŭ ������ ������ �ڿ� ��Ʈ����
        float deadTime = 0f;
        while (deadTime < 4f)
        {
            deadTime += Time.deltaTime;
            float deadSpeed = Time.deltaTime * 0.5f;
            transform.position += Vector3.down * deadSpeed;
            yield return null;
        }
        Destroy(gameObject);
    } // Dead

    //! ������ó�� �Լ�
    public void TakeDamage(DamageMessage message)
    {
        isHit = true;
        attackerPos = message.causer.transform.position;
        hp -= message.damageAmount;
        if (hp <= 0f && isDead == false)
        {
            isDead = true;
            StartCoroutine(Dead());
        }
    } // TakeDamage
} // AnimalController
