using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditor.Presets;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class BuildSystem : MonoBehaviour
{
    private List<GameObject> buildObjs;
    private List<Material> buildMats;

    private float hitDistance = 20.0f;
    private RaycastHit hit;
    private Ray ray;

    private Transform cameraTrans;
    private GameObject prevObj;
    private PrevObjInfo prevInfo;
    private Vector3 prevPos;
    private Vector3 prevRot;
    private float prevYAngle;

    private buildType prevType;
    private buildTypeMat prevMat;
    private int layermask;
    private bool IsBuildTime;

    private float gridSize = 0.1f;
    private bool debugMode = false;

    void Awake()
    {
        buildObjs = new List<GameObject>();
        buildMats = new List<Material>();

        GameObject[] loadObjs = Resources.LoadAll<GameObject>("PBS/BuildPreFab/prevBuild");
        Material[] loadMats = Resources.LoadAll<Material>("PBS/BuildPreFab/Materials");

        for (int i = 0; i < loadObjs.GetLength(0); i++)
        {
            buildObjs.Add(loadObjs[i]);
        }

        for (int i = 0; i < loadMats.GetLength(0); i++)
        {
            buildMats.Add(loadMats[i]);
        }

        cameraTrans = Camera.main.transform;

        //초기값
        IsBuildTime = false;
        prevType = buildType.floor;
        prevMat = buildTypeMat.green;

        prevRot = Vector3.zero;
        prevYAngle = 0.0f;
        // layermask = (-1) - (1 << LayerMask.NameToLayer("buildThings") | 1 << LayerMask.NameToLayer("Player")); //해당 레이어들만 제외
        layermask = (-1) - (1 << LayerMask.NameToLayer("buildThings")); //해당 레이어만 제외

        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {

    }

    void Update()
    {
        ControlKey();
        if (IsBuildTime) { RaycastUpdate(); }
    }

    private void ControlKey()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!debugMode) debugMode = true;
            else if (debugMode) debugMode = false;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (IsBuildTime == true)
            {
                D_prevObj();
                IsBuildTime = false;
            }
            else if (IsBuildTime == false)
            {
                IsBuildTime = true;
                C_prevObj(prevType, 1);
            }
        }

        if (IsBuildTime)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                D_prevObj();
                prevType++;
                if ((int)prevType > 7) prevType = 0;
                C_prevObj(prevType, 1);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                prevYAngle += 45.0f;
                if (prevYAngle > 360.0f) prevYAngle = 0.0f;
            }

            if (Input.GetMouseButtonDown(1))
            {
                //설치
                BuildObj();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.point != null && hit.point != default)
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("build"))
                    {
                        Destroy(hit.transform.parent.gameObject);
                        if (prevInfo != null || prevInfo != default) prevInfo.deleteObjTime();
                    }
                }
            }
        }
    }


    private void RayFloor(RaycastHit hit2)
    {
        switch(hit2.transform.name)
        {
            //case "Center":
            //    break;
            //case "LeftTop":
            //    break;
            //case "LeftTop":
            //    break;
            //case "LeftTop":
            //    break;
            //case "LeftTop":
            //    break;
            //case "LeftTop":
            //    break;
            //case "LeftTop":
            //    break;
            //case "LeftTop":
            //    break;
        }
    }

    private void RaycastUpdate()
    {
        //정가운데 화면 레이 쏘기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out hit, hitDistance, layermask))
        {
            if (hit.point != null)
            {
                //if (Physics.Raycast(ray, out hit, hitDistance, LayerMask.NameToLayer("buildFloor")))
                //{
                //    InverseTransformPoint
                //    if (hit.point.y < hit.transform )
                //    RayFloor(hit);
                //}
                //else
                //{
                    prevUpdate(hit);   //자리 세팅
                    if (debugMode) Debug.DrawLine(ray.origin, hit.point, Color.green);
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
                        SetPrevMat(prevType, 1, prevMat);
                    }
                //}
            }
        }
        else
        {
            if (prevMat != buildTypeMat.red)
            {
                prevMat = buildTypeMat.red;
                SetPrevMat(prevType, 1, prevMat);
            }
            prevObj.transform.position = ray.direction * hitDistance;
            if (debugMode) Debug.DrawLine(ray.origin, ray.direction * hitDistance, Color.red);
        }
    }

    private void prevUpdate(RaycastHit hit2)
    {
        if (prevObj != null || prevObj != default)
        {
            //Legacy
            // prevPos = hit2.point;
            // prevPos -= Vector3.one * offset;
            // prevPos /= gridSize;
            // prevPos = new Vector3(Mathf.Round(prevPos.x), Mathf.Round(prevPos.y), Mathf.Round(prevPos.z));
            // prevPos *= gridSize;
            // prevPos += Vector3.one * offset;
            // prevObj.transform.position = prevPos;

            prevPos = hit2.point;
            prevPos /= gridSize;
            prevPos = new Vector3(Mathf.Round(prevPos.x),Mathf.Round(prevPos.y),Mathf.Round(prevPos.z));
            prevPos *= gridSize;
            prevObj.transform.position = prevPos;

            prevRot = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + prevYAngle, 0);
            prevObj.transform.rotation = Quaternion.Euler(prevRot);

            if (prevInfo != null || prevInfo != default)
            {
                prevInfo.setMid(prevPos);
            }
        }
    }

    private void D_prevObj()
    {
        if (prevObj != null || prevObj != default)
            Destroy(prevObj);
    }

    private void C_prevObj(buildType buildtype, int type)
    {
        if (type == 0)
        {
            prevObj = Instantiate(buildObjs[(int)buildtype]);
            prevObj.layer = LayerMask.NameToLayer("build");
            SetPrevMat(buildtype, 0, buildTypeMat.none);
            prevObj.GetComponent<MeshCollider>().convex = true;
            prevObj.GetComponent<MeshCollider>().isTrigger = false;
        }
        else if (type == 1)
        {
            prevObj = Instantiate(buildObjs[(int)buildtype]);
            prevObj.name = "prevObj";
            SetLayer();
            prevObj.transform.GetChild(0).GetComponent<MeshCollider>().convex = true;
            prevObj.transform.GetChild(0).GetComponent<MeshCollider>().isTrigger = true;
            SetPrevMat(buildtype, 1, buildTypeMat.green);
            prevInfo = prevObj.FindChildObj("BuildCollider").GetComponent<PrevObjInfo>();
            //prevInfo.SetType(buildtype, prevPos);
            prevYAngle = 0.0f;
        }
    }

    private void SetLayer()
    {
        prevObj.layer = LayerMask.NameToLayer("buildThings");

        if (prevObj.transform.childCount > 0)
        {
            Transform[] allChildren = prevObj.GetComponentsInChildren<Transform>();

            foreach(Transform child in allChildren)
            {
                child.gameObject.layer = LayerMask.NameToLayer("buildThings");
            }
        }
    }

    private void BuildObj()
    {
        if (prevInfo != null && prevInfo != default && prevInfo.isBuildAble == true)
        {
            GameObject buildObj = Instantiate(buildObjs[(int)prevType],prevPos,Quaternion.Euler(prevRot));
            //buildObj.layer = LayerMask.NameToLayer("build");
            buildObj.layer = 0;
            buildObj.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(GData.BUILD_MASK);
            buildObj.FindChildObj("BuildPart").SetActive(false);
        }
    }

    public void SetPrevMat(buildType buildtemp,int type, buildTypeMat mat)
    {
        if (prevObj != null || prevObj != default)
        {
            if (type == 0)
            {
                switch (buildtemp)
                {
                    case buildType.door:
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.none];
                            prevObj.FindChildObj("Door").GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.none];
                        break;
                    case buildType.windowswall:
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.none];
                            prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.GlassNone];
                        break;
                    default:
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.none];
                        break;
                }
            }
            else if (type == 1)
            {
                switch (buildtemp)
                {
                    case buildType.door:
                        if (mat == buildTypeMat.green)
                        {
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.green];
                            prevObj.FindChildObj("Door").GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.green];
                        }
                        else if (mat == buildTypeMat.red)
                        {
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.red];
                            prevObj.FindChildObj("Door").GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.red];
                        }
                        break;
                    case buildType.windowswall:
                        if (mat == buildTypeMat.green)
                        {
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.green];
                            prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.GlassGreen];
                        }
                        else if (mat == buildTypeMat.red)
                        {
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.red];
                            prevObj.FindChildObj("Glass").GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.red];
                        }
                        break;
                    default:
                        if (mat == buildTypeMat.green)
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.green];
                        else if (mat == buildTypeMat.red)
                            prevObj.transform.GetChild(0).GetComponent<Renderer>().material = buildMats[(int)buildTypeMat.red];
                        break;
                }
            }
        }
    }

    public Vector3 SetPrevOffset(buildType buildtemp)
    {
        Vector3 Result = default;

        switch (buildtemp)
        {
            case buildType.beam:
                Result = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case buildType.cut:
                Result = new Vector3(12.5f, 0.0f, 0.0f);
                break;
            case buildType.door:
                Result = new Vector3(12.5f, 0.0f, 0.0f);
                break;
            case buildType.floor:
                //Result = new Vector3(14.0f, 1.0f, -14.0f);
                Result = new Vector3(12.5f, 1.0f, 12.5f);
                break;
            case buildType.roof:
                Result = new Vector3(12.5f, 0.0f, 12.5f);
                break;
            case buildType.stairs:
                Result = new Vector3(14.0f, 0.0f, 0.0f);
                break;
            case buildType.wall:
                Result = new Vector3(12.5f, 0.0f, 0.0f);
                break;
            case buildType.windowswall:
                Result = new Vector3(12.5f, 0.0f, 0.0f);
                break;
        }
        return Result;
    }
}

public enum buildType
{
    none = -1, beam, cut, door, floor, roof, stairs, wall, windowswall
}

public enum buildTypeMat
{
    none, green, red, GlassNone, GlassGreen, GlassRed
}