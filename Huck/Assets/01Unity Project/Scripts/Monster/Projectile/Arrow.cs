using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private Rigidbody arrowRb;
    private CapsuleCollider arrowCollider;
    public GameObject target;
    public AnimationCurve x;
    public AnimationCurve y;
    public AnimationCurve z;

    // Start is called before the first frame update
    void Start()
    {
        arrowRb = gameObject.GetComponent<Rigidbody>();
        arrowCollider = gameObject.GetComponent<CapsuleCollider>();
        Vector3 velocity = GetVelocity(transform.position, target.transform.position, 30f, 5f);
        SetForce(velocity);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    transform.forward = arrowRb.velocity;
    //}

    //private void FixedUpdate()
    //{
    //    //arrowRb.velocity = transform.forward * speed * Time.fixedDeltaTime;
    //    arrowRb.AddForce(transform.forward * speed, ForceMode.Impulse);
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"플레이어 맞춤! {other.tag}");
            arrowRb.velocity = Vector3.zero;
            arrowCollider.isTrigger = false;
        }
    }

    void SetForce(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    Vector3 GetVelocity(Vector3 currentPos, Vector3 targetPos, float initialAngle)
    {
        Vector3 finalVelocity = GetVelocity(currentPos, targetPos, initialAngle, 1.0f);
        return finalVelocity;

        //float gravity = Physics.gravity.magnitude;
        //float angle = initialAngle * Mathf.Deg2Rad;

        //Vector3 planarTarget = new Vector3(targetPos.x, 0, targetPos.z);
        //Vector3 planarPosition = new Vector3(currentPos.x, 0, currentPos.z);

        //float distance = Vector3.Distance(planarTarget, planarPosition);
        //float yOffset = currentPos.y - targetPos.y;

        //float initialVelocity = (1 / Mathf.Cos(angle)) * 
        //    Mathf.Sqrt(
        //        (0.5f * gravity * Mathf.Pow(distance, 2)) / 
        //        (distance * Mathf.Tan(angle) + yOffset));

        //Vector3 velocity = new Vector3(0f, 
        //    initialVelocity * Mathf.Sin(angle), 
        //    initialVelocity * Mathf.Cos(angle));

        //float angleBetweenObjects = Vector3.Angle(Vector3.forward, 
        //    planarTarget - planarPosition) * (targetPos.x > currentPos.x ? 1 : -1);
        //Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        //return finalVelocity;
    }

    Vector3 GetVelocity(Vector3 currentPos, Vector3 targetPos, float initialAngle, float power)
    {
        float gravity = Physics.gravity.magnitude;
        float powerCorrection = gravity * power * 0.01f;
        Debug.Log($"{gravity}, p: {powerCorrection}");
        float angle = (initialAngle * powerCorrection) * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 planarPosition = new Vector3(currentPos.x, 0, currentPos.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = currentPos.y - targetPos.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) *
            Mathf.Sqrt(
                (0.5f * gravity * Mathf.Pow(distance, 2) * power) /
                (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0f,
            initialVelocity * Mathf.Sin(angle),
            initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects = Vector3.Angle(Vector3.forward,
            planarTarget - planarPosition) * (targetPos.x > currentPos.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }

}
