using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public IInteractable currentInteractInterface = default;
    public LayerMask exceptionLayer = default;
    public float raycastDistance = 3f;
    public float rayYOffset = 5f;
    private Ray ray = default;

    // Update is called once per frame
    void Update()
    {
        Raycast();
        OnInteract();
    }
    void Raycast()
    {
        Vector3 offset = new Vector3(0f, rayYOffset, 0f);
        ray =  new Ray(transform.position + offset, transform.forward + offset);
        RaycastHit hit;
        bool isHitRay = Physics.Raycast(ray, out hit, raycastDistance, ~exceptionLayer);
#if UNITY_EDITOR
        Debug.DrawRay(transform.position + offset, transform.forward * raycastDistance, isHitRay ? Color.green : Color.red);
#endif
        if(isHitRay)
        {
            Debug.Log($"ray hit : {hit.collider.gameObject.name}");
            IInteractable hitInterface = hit.collider.gameObject.GetComponent<IInteractable>();
            if(hitInterface != null)
            {
                Debug.Log("Interface Hit");
                currentInteractInterface = hitInterface;
            }
            else
            {
                currentInteractInterface = default;
            }
        }
        else
        {
            currentInteractInterface = default;
        }
    }

    void OnInteract()
    {
        if(currentInteractInterface == null)
            return;
        if(Input.GetKeyDown(KeyCode.E))
        {
            currentInteractInterface.Execute();
        }
    }
}
