using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalController : MonoBehaviour, IDamageable, IDropable
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

    [SerializeField] private List<DropItemConfig> dropItems;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = 100f;
        agent.angularSpeed = 180f;
        hp = maxHp;
    } // Start

    // Update is called once per frame
    void Update()
    {
        SelectState();
    } // Update

    //! 동물 상태 정하는 함수
    private void SelectState()
    {
        if (!agent.enabled)
        {
            return;
        }
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
                    targetPos = getTargetPos.GetRandomCirclePos(transform.position, 30, 10);
                    if (targetPos == transform.position)
                    {
                        Debug.Log("멍청이 물소 파괴");
                        Destroy(gameObject);
                    }
                    isMove = true;
                    StartCoroutine(Move());
                    time = 0f;
                }
            }
        }
    } // SelectState

    //! 동물 이동하는 코루틴함수
    private IEnumerator Move()
    {
        animator.SetBool("isWalk", true);
        float timeCheck = 0f;
        while (isMove == true)
        {
            timeCheck += Time.deltaTime;
            if (timeCheck >= 2f && agent.velocity == Vector3.zero)
            {
                Debug.Log("왕따 물소 파괴");
                Destroy(gameObject);
                yield break;
            }
            float distance = Vector3.Distance(transform.position, targetPos);
            agent.SetDestination(targetPos);
            if (distance <= 1f || isHit == true)
            {
                isMove = false;
            }
            yield return null;
        }
        agent.ResetPath();
        animator.SetBool("isWalk", false);
    } // Move

    //! Hit상태일때 도망가는 코루틴함수
    private IEnumerator Escape()
    {
        Vector3 dir = -(attackerPos - transform.position).normalized;
        animator.SetBool("isRun", true);
        escapeTime = 0f;
        while (escapeTime < 2f)
        {
            if (isDead == true)
            {
                yield break;
            }
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

    //! 동물 사망하는 코루틴함수
    private IEnumerator Dead()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        animator.SetBool("isDead", true);
        yield return null;
        DropItem(dropItems, transform);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1.5f);
        // 밑으로 시체가 내려가게 하기위해 네비매쉬 비활성화
        agent.enabled = false;
        // 4초에 걸쳐 총 2f만큼 밑으로 내려간 뒤에 디스트로이
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

    //! 데미지처리 함수
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
    public void DropItem(List<DropItemConfig> dropItems, Transform targetTransform)
    {
        float yOffset = 1f;
        float maxPositionJitter = 0.5f;

        Vector3 spawnLocation = targetTransform.position;
        foreach (var item in dropItems)
        {
            if (item == null || item == default)
                continue;

            // 드랍 확률 검사
            int dropPercentage = Random.Range(0, 100);
            if (dropPercentage > item.dropPercentage)
                continue;


            // 스폰 위치, 각도 값 셋팅
            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 positionOffset = new Vector3(Random.Range(-maxPositionJitter, maxPositionJitter),
                                                            yOffset,
                                                            Random.Range(-maxPositionJitter, maxPositionJitter));

            // 스폰 및 이후 처리
            var spawnedGO = Instantiate(item.prefab, spawnLocation + positionOffset, spawnRotation);
            Item dropedItem = spawnedGO.GetComponent<Item>();
            if (dropedItem != null)
            {
                // 아이템 갯수 처리
                dropedItem.itemCount = Random.Range(item.minDropCount, item.maxDropCount);
            }
        }
    }
} // AnimalController
