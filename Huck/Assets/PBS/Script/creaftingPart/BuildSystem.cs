using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildSystem : MonoBehaviour
{
    //using LayerMaskName
    private const string BUILD_TEMP_LAYER = "BuildThings";
    private const string BUILD_FLOOR_LAYER = "BuilFloor";
    private const string BUILD_WALL_LAYER = "BuildWall";
    private const string BUILD_OBJ_LAYER = "BuildObj";
    private const string BUILD_ITEM_LAYER = "BuildItem";
    private const string BUILD_LAYER = GData.BUILD_MASK;

    private List<GameObject> BuildLoadObjs;
    private List<Material> BuildLoadMats;

    private const float HIT_DISTANCE = 10.0f;
    private RaycastHit hit;
    private Ray ray;

    private GameObject prevObj;
    public bool IsBuildAct;
    private PrevObjInfo prevInfo;
    private PrevObjInfo prevDefaultInfo;
    private Vector3 prevPos;
    private Vector3 prevRot;
    private float prevYAngle;

    private buildType prevType;
    private buildTypeMat prevMat;
    private string prevName;
    private int buildObjNums;
    private int layerMask;
    public bool IsBuildTime;
    private bool IsResetCall;

    private float gridSize = 0.1f;
    public bool IsDefaultLayer = false;

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
        IsBuildAct = false;
        prevType = buildType.Foundation;
        prevMat = buildTypeMat.Green;

        prevRot = Vector3.zero;
        prevYAngle = 0.0f;
        buildObjNums = 0;

        //raycast rayerSet
        layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER));
        // DefaultLayerMask = (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) | 1 << LayerMask.NameToLayer("Default"));
    }
    private void Start()
    {
        GameManager.Instance.playerObj.GetComponent<InHand>().buildSystem = this;
    }
    void Update()
    {
        ControlKey();

        float X_Angle = Quaternion.Angle(Camera.main.transform.rotation, Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0));

        if (IsBuildTime && X_Angle < 45.0f)
        {
            C_prevObj(prevType);
            if (!IsResetCall) RaycastUpdate();
        }
        else if (IsBuildTime && X_Angle >= 45.0f)
        {
            D_prevObj();
        }
    }

    public void CallingPrev()
    {
        D_prevObj();
    }

    public void CallingPrev(string btype)
    {
        IsResetCall = true;
        prevName = btype;
        switch (btype)
        {
            case "WoodBeam":
                prevType = buildType.Beam;
                break;
            case "WoodCut":
                prevType = buildType.Cut;
                break;
            case "WoodDoor":
                prevType = buildType.Door;
                break;
            case "WoodWindowWall":
                prevType = buildType.Windowswall;
                break;
            case "WoodWall":
                prevType = buildType.Wall;
                break;
            case "WoodFloor":
                prevType = buildType.Floor;
                break;
            case "WoodFoundation":
                prevType = buildType.Foundation;
                break;
            case "WoodRoof":
                prevType = buildType.Roof;
                break;
            case "WoodStairs":
                prevType = buildType.Stairs;
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
    }

    private void ControlKey()
    {
        if (!IsBuildTime)
        {
            //문열기
            if (Input.GetKeyDown(KeyCode.F))
            {
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

        if (prevType == buildType.Floor || prevType == buildType.Foundation)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_WALL_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_OBJ_LAYER));
        }
        else if (prevType == buildType.Wall || prevType == buildType.Windowswall || prevType == buildType.Door || prevType == buildType.Cut || prevType == buildType.Beam)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_FLOOR_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_OBJ_LAYER));
        }
        else if (prevType == buildType.Stairs || prevType == buildType.Roof)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_WALL_LAYER) |
                                1 << LayerMask.NameToLayer(BUILD_FLOOR_LAYER));
        }
        else if (prevType == buildType.Anvil || prevType == buildType.Stove || prevType == buildType.Workbench)
        {
            layerMask = (-1) - (1 << LayerMask.NameToLayer(BUILD_TEMP_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_WALL_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_FLOOR_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_OBJ_LAYER) |
                    1 << LayerMask.NameToLayer(BUILD_ITEM_LAYER));
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

                //기본 부분
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
                    if (prevType == buildType.Foundation && hit.transform.gameObject.layer != LayerMask.NameToLayer("Terrain"))
                    {
                        IsBuildAct = false;
                        prevMat = buildTypeMat.Red;
                        SetPrevMat(prevType, prevMat);
                        prevObj.transform.position = hit.point;
                    }
                    else
                    {
                        if (prevDefaultInfo != null || prevDefaultInfo != default)
                        {
                            if (prevDefaultInfo.isBuildAble == false)
                            {
                                prevMat = buildTypeMat.Red;
                                IsBuildAct = false;
                                prevObj.transform.position = hit.point;
                            }
                            else
                            {
                                prevMat = buildTypeMat.Green;
                                IsBuildAct = true;
                            }
                            SetPrevMat(prevType, prevMat);
                        }
                    }
                }
                else if (!IsDefaultLayer)   //벽에 붙는 부분
                {
                    if (prevInfo != null || prevInfo != default)
                    {
                        if (prevInfo.isBuildAble == false)
                        {
                            prevMat = buildTypeMat.Red;
                            IsBuildAct = false;
                        }
                        else
                        {
                            prevMat = buildTypeMat.Green;
                            IsBuildAct = true;
                        }
                        SetPrevMat(prevType, prevMat);
                    }
                }
            }
        }
        else
        {
            if (prevMat != buildTypeMat.Red)
            {
                IsBuildAct = false;
                prevMat = buildTypeMat.Red;
                SetPrevMat(prevType, prevMat);
            }
            prevObj.transform.position = ray.direction * HIT_DISTANCE;
        }
    }

    private void prevUpdate(RaycastHit hit2)
    {
        if (prevObj != null || prevObj != default)
        {
            if (hit2.transform.gameObject.GetComponent<CheckTrigger>() != null)
            {
                if (hit2.transform.gameObject.GetComponent<CheckTrigger>().IsOnCollider == true)
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
                        }
                        else if (prevType == buildType.Wall || prevType == buildType.Windowswall || prevType == buildType.Door)
                        {
                            string temp = hit2.transform.name;
                            if (temp.Equals("LeftBot") || temp.Equals("LeftTop") || temp.Equals("RightTop") || temp.Equals("RightBot")) { /* Do nothing */ }
                            else
                            {
                                prevPos = hit2.transform.position;
                                prevObj.transform.position = prevPos;
                                prevRot = new Vector3(0, hit2.transform.rotation.eulerAngles.y + prevYAngle, 0);
                                prevObj.transform.rotation = Quaternion.Euler(prevRot);
                            }
                        }
                        else
                        {
                            prevPos = hit2.transform.position;
                            prevObj.transform.position = prevPos;
                            prevRot = new Vector3(0, hit2.transform.rotation.eulerAngles.y + prevYAngle, 0);
                            prevObj.transform.rotation = Quaternion.Euler(prevRot);
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
                    }
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
            SetPrevMat(buildtype, buildTypeMat.Green);
            prevInfo = prevObj.FindChildObj("BuildCollider").GetComponent<PrevObjInfo>();
            prevDefaultInfo = prevObj.FindChildObj("BuildDefaultCollider").GetComponent<PrevObjInfo>();
            prevInfo.SetLayerType(buildtype);
            prevDefaultInfo.SetLayerType(buildtype);
            prevYAngle = 0.0f;

            IsResetCall = false;
            IsBuildAct = false;
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
            case buildType.Anvil:
            case buildType.Stove:
            case buildType.Workbench:
                prevObj.transform.GetComponent<MeshCollider>().convex = true;
                prevObj.transform.GetComponent<MeshCollider>().isTrigger = true;
                break;
            case buildType.Door:
                prevObj.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = true;
                prevObj.transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>().isTrigger = true;
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
            if (prevInfo != null && prevInfo != default && IsBuildAct == true)
            {
                buildObj = Instantiate(BuildLoadObjs[(int)prevType], prevPos, Quaternion.Euler(prevRot));
                buildObj.transform.parent = this.transform;

                switch (prevType)
                {
                    case buildType.Anvil:
                        buildObj.tag = "Anvil";
                        break;
                    case buildType.Stove:
                        buildObj.tag = "Stove";
                        break;
                    case buildType.Workbench:
                        buildObj.tag = "Workbench";
                    break;
                    default:
                        buildObj.transform.GetChild(0).tag = "Gather";
                        break;
                }

                switch (prevType)
                {
                    case buildType.Anvil:
                    case buildType.Stove:
                    case buildType.Workbench:
                        buildObj.AddComponent<NavMeshObstacle>();

                        buildObj.layer = LayerMask.NameToLayer(BUILD_ITEM_LAYER);
                        if (buildObj.transform.childCount > 0)
                        {
                            Transform[] allChildren = buildObj.GetComponentsInChildren<Transform>();
                            foreach (Transform child in allChildren)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer(BUILD_ITEM_LAYER);
                            }
                        }

                        break;
                    default:
                        buildObj.transform.GetChild(0).gameObject.AddComponent<NavMeshObstacle>();

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
                    case buildType.Door: /* Do nothing */
                        break;
                    case buildType.Anvil:
                    case buildType.Stove:
                    case buildType.Workbench:
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
            if (prevDefaultInfo != null && prevDefaultInfo != default && IsBuildAct == true)
            {
                buildObj = Instantiate(BuildLoadObjs[(int)prevType], prevPos, Quaternion.Euler(prevRot));
                buildObj.transform.parent = this.transform;

                switch (prevType)
                {
                    case buildType.Anvil:
                        buildObj.tag = "Anvil";
                        break;
                    case buildType.Stove:
                        buildObj.tag = "Stove";
                        break;
                    case buildType.Workbench:
                        buildObj.tag = "Workbench";
                    break;
                    default:
                        buildObj.transform.GetChild(0).tag = "Gather";
                        break;
                }

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
                        buildObj.layer = LayerMask.NameToLayer(BUILD_ITEM_LAYER);

                        if (buildObj.transform.childCount > 0)
                        {
                            Transform[] allChildren = buildObj.GetComponentsInChildren<Transform>();

                            foreach (Transform child in allChildren)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer(BUILD_ITEM_LAYER);
                            }
                        }

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
                    case buildType.Door: /* Do nothing */
                        break;
                    case buildType.Anvil:
                    case buildType.Stove:
                    case buildType.Workbench:
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
            buildObj.name = prevName + buildObjNums;
            buildObjNums++;
            BuildingList.Add(buildObj);
        }
    }

    public void SetPrevMat(buildType buildtemp, buildTypeMat mat)
    {
        if (prevObj != null || prevObj != default)
        {
            switch (buildtemp)
            {
                case buildType.Door:
                    if (mat == buildTypeMat.Green)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Green];
                        prevObj.FindChildObj("Door").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Green];
                    }
                    else if (mat == buildTypeMat.Red)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                        prevObj.FindChildObj("Door").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                    }
                    break;
                case buildType.Windowswall:
                    if (mat == buildTypeMat.Green)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Green];
                        prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.GlassGreen];
                    }
                    else if (mat == buildTypeMat.Red)
                    {
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                        prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                    }
                    break;
                case buildType.Foundation:
                    if (mat == buildTypeMat.Green)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            prevObj.transform.GetChild(i).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Green];
                        }
                    }
                    else if (mat == buildTypeMat.Red)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            prevObj.transform.GetChild(i).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                        }
                    }
                    break;
                case buildType.Anvil:
                    if (mat == buildTypeMat.Green)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.VikingGreen];
                    else if (mat == buildTypeMat.Red)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.VikingRed];
                    break;
                case buildType.Workbench:
                case buildType.Stove:
                    if (mat == buildTypeMat.Green)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Green];
                    else if (mat == buildTypeMat.Red)
                        prevObj.GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                    break;
                default:
                    if (mat == buildTypeMat.Green)
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Green];
                    else if (mat == buildTypeMat.Red)
                        prevObj.transform.GetChild(0).GetComponent<Renderer>().material = BuildLoadMats[(int)buildTypeMat.Red];
                    break;
            }
        }
    }

    public void FindBuildObj(GameObject Obj)
    {
        if(Obj.tag == "Anvil" || Obj.tag == "Stove" || Obj.tag == "Workbench")
        {
            FindAndDestory(Obj.name);
        }
        else
        {
            FindAndDestory(Obj.transform.parent.name);
        }
    }

    public void FindAndDestory(string ObjName)
    {
        for (int i = 0; i < BuildingList.Count; i++)
        {
            if (BuildingList[i].name.Equals(ObjName))
            {
                Destroy(BuildingList[i]);
                BuildingList.RemoveAt(i);
                break;
            }
        }
    }
}

public enum buildType
{
    None = -1, Beam, Cut, Door, Floor, Roof, Stairs, Wall, Windowswall, Foundation, Anvil, Stove, Workbench
}

public enum buildTypeMat
{
    None, Green, Red, GlassNone, GlassGreen, GlassRed, VikingNone, VikingGreen, VikingRed
}