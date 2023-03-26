using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EResourceType
{
    NONE = -1,
    WOOD,
    ORE,
}
public enum EResourceLevel
{
    NONE = -1,
    LOW,
    MEDIUM,
    HIGH
}

[CreateAssetMenu(fileName = "Resource Object Data", menuName = "Huck SO/Resource Object Data", order = -1)]
public class ResourceObjectSO : ScriptableObject
{
    [SerializeField]
    private string resourceName;
    public string ResourceName
    {
        get { return resourceName; }
    }

    [SerializeField]
    private float hp;
    public float HP
    {
        get { return hp; }
    }
    [SerializeField]
    private EResourceType resourceType;
    public EResourceType ResourceType
    {
        get { return resourceType; }
    }
    [SerializeField]
    private EResourceLevel resourceLevel;
    public EResourceLevel ResourceLevel
    {
        get { return resourceLevel; }
    }
}
