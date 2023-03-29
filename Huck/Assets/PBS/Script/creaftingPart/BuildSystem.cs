using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BuildSystem : MonoBehaviour
{
    //using LayerMaskName
    private const string BUILD_TEMP_LAYER = "BuildThings";
    private const string BUILD_FLOOR_LAYER = "BuilFloor";
    private const string BUILD_WALL_LAYER = "BuildWall";
    private const string BUILD_OBJ_LAYER = "BuildObj";
    private const string BUILD_LAYER = GData.BUILD_MASK;

    private List<GameObject> BuildLoadObjs;
    private List<Material> BuildLoadMats;

    private const float HIT_DISTANCE = 10.0f;
    private RaycastHit hit;
    private Ray ray;

    private GameObject prevObj;
    private PrevObjInfo prevInfo;
    private PrevObjInfo prevDefaultInfo;
    private Vector3 prevPos;
    private Vector3 prevRot;
    private float prevYAngle;
    private GameObject lastBuildObj;

    private buildType prevType;
    private buildTypeMat prevMat;
    private int layerMask;
    private int DefaultLayerMask;
    public bool IsBuildTime;
    private bool IsResetCall;

    private float gridSize = 0.1f;
    private bool debugMode = false;
    private bool IsDefaultLayer = false;

    private List<GameObject> BuildingList;

    void Awake()
    {
        BuildLoadObjs = new List<GameObject>();
        BuildLoadMats = new List<Material>();
        BuildingList = new List<GameObject>();

        GameObject[] loadObjs = Resources.LoadAll<GameObject>("PBS/BuildPreFab/prevBuild");
        Material[] loadMats = Resources.LoadAll<Material>("PBS/BuildPreFab/Materials");

        for (int i = 0; i < loadObjs.GetLength(0); i++)
        {
            BuildLoadObjs.Add(loadObjs[i]);
        }

        for (int i = 0; i < loadMats.GetLength(0); i++)
        {
            BuildLoadMats.Add(loadMats[i]);
        }

        //reset
        IsBuildTime = false;
        IsResetCall = false;
        prevType = buildType.Foundation;
        prevMat = buildTypeMat.green;

        prevRot = Vector3.zero;
        prevYAngle = 0.0f;

        //raycast rayerSet
        layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER));
        DefaultLayerMask = (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) | 1 << LayerMask.NameToLayer("Default"));
    }

    void Update()
    {
        ControlKey();
        if (IsBuildTime & Camera.main.transform.rotation.eulerAngles.x <= 45.0f) 
        {
            C_prevObj(prevType);
            if(!IsResetCall) RaycastUpdate();
        }
        else if(IsBuildTime & Camera.main.transform.rotation.eulerAngles.x > 45.0f)
        {
            D_prevObj();
        }
    }

    public void CallingPrev()
    {
        D_prevObj();
        CollidersOnOff();
    }

    public void CallingPrev(string btype)
    {
        IsResetCall = true;

        switch (btype)
        {
            case "WoodBeam":
                prevType = buildType.beam;
                break;
            case "WoodCut":
                prevType = buildType.cut;
                break;
            case "WoodDoor":
                prevType = buildType.door;
                break;
            case "WoodWindowWall":
                prevType = buildType.windowswall;
                break;
            case "WoodWall":
                prevType = buildType.wall;
                break;
            case "WoodFloor":
                prevType = buildType.floor;
                break;
            case "WoodFoundation":
                prevType = buildType.Foundation;
                break;
            case "WoodRoof":
                prevType = buildType.roof;
                break;
            case "WoodStairs":
                prevType = buildType.stairs;
                break;
            case "Anvil":
                prevType = buildType.Anvil;
                break;
            case "Stove":
                prevType = buildType.Stove;
                break;
            case "Workbench":
                prevType = buildType.Workbench;
                break;
        }

        D_prevObj();
        CollidersOnOff();
    }

    private void ControlKey()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!debugMode) debugMode = true;
            else if (debugMode) debugMode = false;
        }

        if (!IsBuildTime)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                //정가운데 화면 레이 쏘기
                Ray DeleteRAY = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hitD;

                if (Physics.Raycast(DeleteRAY, out hitD, HIT_DISTANCE, LayerMask.NameToLayer(BUILD_LAYER)))
                {
                    if (hitD.point != null) { Destroy(hitD.transform.parent.gameObject); }
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                //정가운데 화면 레이 쏘기
                Ray DoorRAY = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hitD;

                if (Physics.Raycast(DoorRAY, out hitD, HIT_DISTANCE))
                {
                    if (hitD.transform.name == "DoorCollider")
                    {
                        DoorInfo temp = hitD.transform.gameObject.GetComponent<DoorInfo>();

                        temp.IsTrigger();
                    }
                }
            }
        }
        else if (IsBuildTime)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                prevYAngle += 45.0f;
                if (prevYAngle > 360.0f) prevYAngle = 0.0f;
            }
            if (Input.GetMouseButtonDown(1))
            {
                BuildObj();
            }
        }
    }

    private void RaycastUpdate()
    {
        //정가운데 화면 레이 쏘기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (prevType == buildType.floor || prevType == buildType.Foundation)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_WALL_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_OBJ_LAYER));
        }
        else if (prevType == buildType.wall || prevType == buildType.windowswall || prevType == buildType.door || prevType == buildType.cut || prevType == buildType.beam)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_FLOOR_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_OBJ_LAYER));
        }
        else if (prevType == buildType.stairs || prevType == buildType.roof)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_WALL_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_FLOOR_LAYER));
        }
        else
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_WALL_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_FLOOR_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_OBJ_LAYER));
        }

        if (Physics.Raycast(ray, out hit, HIT_DISTANCE, layerMask))
        {
            if (hit.point != null)
            {
                prevUpdate(hit);   //자리 세팅

                if (debugMode) Debug.DrawLine(ray.origin, hit.point, Color.green);

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Default") ||
                    hit.transform.gameObject.layer == LayerMask.NameToLayer(BUILD_LAYER) ||
                    hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                    IsDefaultLayer = true;
                }
                else
                {
                    IsDefaultLayer = false;
                }

                if (IsDefaultLayer)
                {
                    if (prevDefaultInfo != null || prevDefaultInfo != default)
                    {
                        if (prevDefaultInfo.isBuildAble == false)
                        {
                            prevMat = buildTypeMat.red;
                        }
                        else
                        {
                            prevMat = buildTypeMat.green;
                        }
                        SetPrevMat(prevType, prevMat);
                    }
                }
                else if (!IsDefaultLayer)
                {
                    //if (prevType == buildType.Foundation)
                    //{
                    //    prevInfo.isBuildAble = false;
                    //    prevMat = buildTypeMat.red;
                    //    SetPrevMat(prevType, prevMat);
                    //}
                    //else
                    //{
                        if (prevInfo != null || prevInfo != default)
                        {
                            if (prevInfo.isBuildAble == false)
                            {
                                prevMat = buildTypeMat.red;
                            }
                            else
                            {
                                prevMat = buildTypeMat.green;
                            }
                            SetPrevMat(prevType, prevMat);
                        }
                    //}
                }
            }
        }
        else
        {
            if (prevMat != buildTypeMat.red)
            {
                prevMat = buildTypeMat.red;
                SetPrevMat(prevType, prevMat);
            }
            prevObj.transform.position = ray.direction * HIT_DISTANCE;
            if (debugMode) Debug.DrawLine(ray.origin, ray.direction * HIT_DISTANCE, Color.red);
        }
    }

    private void prevUpdate(RaycastHit hit2)
    {
        if (prevObj != null || prevObj != default)
        {
            if (hit2.transform.gameObject.layer == LayerMask.NameToLayer(BUILD_FLOOR_LAYER) ||
                hit2.transform.gameObject.layer == LayerMask.NameToLayer(BUILD_WALL_LAYER) ||
                hit2.transform.gameObject.layer == LayerMask.NameToLayer(BUILD_OBJ_LAYER))
            {
                if (prevType == buildType.Foundation)
                {
                    prevPos = hit2.transform.position + new Vector3(0, -0.5f, 0);
                    prevObj.transform.position = prevPos;
                    prevRot = new Vector3(0, hit2.transform.rotation.eulerAngles.y + prevYAngle, 0);
                    prevObj.transform.rotation = Quaternion.Euler(prevRot);
                    lastBuildObj = hit2.transform.gameObject;
                }
                else
                {
                    prevPos = hit2.transform.position;
                    prevObj.transform.position = prevPos;
                    prevRot = new Vector3(0, hit2.transform.rotation.eulerAngles.y + prevYAngle, 0);
                    prevObj.transform.rotation = Quaternion.Euler(prevRot);
                    lastBuildObj = hit2.transform.gameObject;
                }
            }
            else
            {
                prevPos = hit2.point;
                prevPos /= gridSize;
                prevPos = new Vector3(Mathf.Round(prevPos.x), Mathf.Round(prevPos.y), Mathf.Round(prevPos.z));
                prevPos *= gridSize;
                prevObj.transform.position = prevPos;

                prevRot = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + prevYAngle, 0);
                prevObj.transform.rotation = Quaternion.Euler(prevRot);
                lastBuildObj = null;
            }
        }
    }

    private void D_prevObj()
    {
        if (prevObj != null || prevObj != default)
            Destroy(prevObj);
    }

    private void C_prevObj(buildType buildtype)
    {
        if (prevObj == null || prevObj == default)
        {
            prevObj = Instantiate(BuildLoadObjs[(int)buildtype]);
            prevObj.transform.parent = this.transform;
            prevObj.name = "prevObj";
            SetLayer();
            SetTrigger(buildtype);
            SetPrevMat(buildtype, buildTypeMat.green);
            prevInfo = prevObj.FindChildObj("BuildCollider").GetComponent<PrevObjInfo>();
            prevDefaultInfo = prevObj.FindChildObj("BuildDefaultCollider").GetComponent<PrevObjInfo>();
            prevYAngle = 0.0f;
            
            IsResetCall = false;
        }
    }

    private void SetTrigger(buildType buildtemp)
    {
        switch (buildtemp)
        {
            case buildType.Foundation:
                for (int i = 0; i < 5; i++)
                {
                    prevObj.transform.GetChild(i).GetComponent<MeshCollider>().convex = true;
                    prevObj.transform.GetChild(i).GetComponent<MeshCollider>().isTrigger = true;
                }
                break;
            case buildType.Anvil: case buildType.Stove: case buildType.Workbench:
                prevObj.transform.GetComponent<MeshCollider>().convex = true;
                prevObj.transform.GetComponent<MeshCollider>().isTrigger = true;
                break;
            default:
                prevObj.transform.GetChild(0).GetComponent<MeshCollider>().convex = true;
                prevObj.transform.GetChild(0).GetComponent<MeshCollider>().isTrigger = true;
                break;
        }
    }

    private void SetLayer()
    {
        prevObj.layer = LayerMask.NameToLayer(BUILD_TEMP_LAYER);

        if (prevObj.transform.childCount > 0)
        {
            Transform[] allChildren = prevObj.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                child.gameObject.layer = LayerMask.NameToLayer(BUILD_TEMP_LAYER);
            }
        }
    }

    private void BuildObj()
    {
        GameObject buildObj = default;

        if (!IsDefaultLayer)
        {
            if (prevInfo != null && prevInfo != default && prevInfo.isBuildAble == true)
            {
                buildObj = Instantiate(BuildLoadObjs[(int)prevType], prevPos, Quaternion.Euler(prevRot));
                buildObj.transform.parent = this.transform;

                switch (prevType)
                {
                    case buildType.Anvil:
                    case buildType.Stove:
                    case buildType.Workbench:
                        buildObj.AddComponent<NavMeshObstacle>();
                        break;
                    default:
                        buildObj.transform.GetChild(0).gameObject.AddComponent<NavMeshObstacle>();
                        break;
                }

                switch (prevType)
                {
                    case buildType.Anvil: case buildType.Stove: case buildType.Workbench:
                        buildObj.layer = LayerMask.NameToLayer(GData.BUILD_MASK);
                        break;
                    default:
                        buildObj.layer = 0;
                        buildObj.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(GData.BUILD_MASK);
                        break;
                }

                switch (prevType)
                {
                    case buildType.Foundation:
                        for (int i = 0; i < 5; i++)
                        {
                            buildObj.transform.GetChild(i).GetComponent<MeshCollider>().isTrigger = false;
                        }
                        break;
                    case buildType.door: /* Do nothing */
                        break;
                    case buildType.Anvil: case buildType.Stove: case buildType.Workbench:
                        buildObj.transform.GetComponent<MeshCollider>().isTrigger = false;
                        break;
                    default:
                        buildObj.transform.GetChild(0).GetComponent<MeshCollider>().isTrigger = false;
                        break;
                }
            }
        }
        else if (IsDefaultLayer)
        {
            if (prevDefaultInfo != null && prevDefaultInfo != default && prevDefaultInfo.isBuildAble == true)
            {
                buildObj = Instantiate(BuildLoadObjs[(int)prevType], prevPos, Quaternion.Euler(prevRot));
                buildObj.transform.parent = this.transform;

                switch (prevType)
                {
                    case buildType.Anvil:
                    case buildType.Stove:
                    case buildType.Workbench:
                        buildObj.AddComponent<NavMeshObstacle>();
                        break;
                    default:
                        buildObj.transform.GetChild(0).gameObject.AddComponent<NavMeshObstacle>();
                        break;
                }

                switch (prevType)
                {
                    case buildType.Anvil:
                    case buildType.Stove:
                    case buildType.Workbench:
                        buildObj.layer = LayerMask.NameToLayer(GData.BUILD_MASK);
                        break;
                    default:
                        buildObj.layer = 0;
                        buildObj.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(GData.BUILD_MASK);
                        break;
                }

                switch (prevType)
                {
                    case buildType.Foundation:
                        for (int i = 0; i < 5; i++)
                        {
                            buildObj.transform.GetChild(i).GetComponent<MeshCollider>().isTrigger = false;
                        }
                        break;
                    case buildType.door: /* Do nothing */
                        break;
                    case buildType.Anvil: case buildType.Stove: case buildType.Workbench:
                        buildObj.transform.GetComponent<MeshCollider>().isTrigger = false;
                        break;
                    default:
                        buildObj.transform.GetChild(0).GetComponent<MeshCollider>().isTrigger = false;
                        break;
                }
            }
        }

        if (buildObj != null || buildObj != default)
        {
            BuildingList.Add(buildObj);
        }
    }

    private void CollidersOnOff()
    {
        if (IsBuildTime)
        {
            if (BuildingList.Count > 0)
            {
                for (int i = 0; i < BuildingList.Count; i++)
                {
                    BuildingList[i].FindChildObj("BuildPart").SetActive(true);
                }
            }
        }
        else if (!IsBuildTime)
        {
            if (BuildingList.Count > 0)
            {
                for (int i = 0; i < BuildingList.Count; i++)
                {
                    BuildingList[i].FindChildObj("BuildPart").SetActive(false);
                }
            }
        }
    }

    public void SetPrevMat(buildType buildtemp, buildTypeMat mat)
    {
        if (prevObj != null || prevObj != default)
        {
            switch (buildtemp)
            {
                case buildType.door:
                    if (mat == buildTypeMat.green)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.green];
                        prevObj.FindChildObj("Door").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.green];
                    }
                    else if (mat == buildTypeMat.red)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                        prevObj.FindChildObj("Door").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                    }
                    break;
                case buildType.windowswall:
                    if (mat == buildTypeMat.green)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.green];
                        prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.GlassGreen];
                    }
                    else if (mat == buildTypeMat.red)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                        prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                    }
                    break;
                case buildType.Foundation:
                    if (mat == buildTypeMat.green)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            prevObj.transform.GetChild(i).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.green];
                        }
                    }
                    else if (mat == buildTypeMat.red)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            prevObj.transform.GetChild(i).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                        }
                    }
                    break;
                case buildType.Anvil:
                    if (mat == buildTypeMat.green)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.VikingGreen];
                    else if (mat == buildTypeMat.red)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.VikingRed];
                    break;
                case buildType.Workbench: case buildType.Stove:
                    if (mat == buildTypeMat.green)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.green];
                    else if (mat == buildTypeMat.red)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                    break;
                default:
                    if (mat == buildTypeMat.green)
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.green];
                    else if (mat == buildTypeMat.red)
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.red];
                    break;
            }
        }
    }
}

    public enum buildType
{
    none = -1, beam, cut, door, floor, roof, stairs, wall, windowswall, Foundation, Anvil, Stove, Workbench
}

public enum buildTypeMat
{
    none, green, red, GlassNone, GlassGreen, GlassRed, VikingNone,VikingGreen,VikingRed
}