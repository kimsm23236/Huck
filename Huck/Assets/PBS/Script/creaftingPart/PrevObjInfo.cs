using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevObjInfo : MonoBehaviour
{
    public bool isBuildAble = false;
    private List<GameObject> cols = new List<GameObject>();

    private buildType PrevType = buildType.none;
    public float[] Size = new float[3];
    private Vector3[] dotPoint = new Vector3[5];
    private Vector3 middlePos;
    private float BorderSize = 0.5f;
    private const float MAX_SIZE = 500.0f;

    private bool isAble = false;
    private const float RAYCAST_DISTANCE = 2.0f;
    private RaycastHit downHit;

    void Update()
    { 
        changeCursor();
        //setPoint();
        //CheckBuild();
    }

    private void changeCursor()
    {
        if (cols.Count > 0)
        {
            isBuildAble = false;
        }
        else
        {
            //if(PrevType != buildType.floor)
            //{
            //    if (isAble == true)
            //    {
            //        isBuildAble = true;
            //    }
            //}
            //else
            //{
                isBuildAble = true;
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("build"))
        {
            cols.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("build"))
        {
            cols.Remove(other.gameObject);
        }
    }

    public void deleteObjTime()
    {
        if (cols.Count > 0)
        {
            for (int i = 0; i < cols.Count; i++)
            {
                cols.RemoveAt(0);
            }
        }
    }

    //===================================================
    public void setMid(Vector3 mid)
    {
        middlePos = mid;
    }

    //public void SetType(buildType setType, Vector3 mid)
    //{
    //    PrevType = setType;
    //    middlePos = mid;
    //    typeToSizeSet(setType);
    //}

    //private void typeToSizeSet(buildType setType)
    //{
    //    Vector3 result = Vector3.zero;

    //    switch(setType)
    //    {
    //        case buildType.none:
    //            result = Vector3.zero;
    //            break;
    //        case buildType.floor:
    //            result = new Vector3(2.0f, 0.0f, 2.0f);
    //            break;
    //        case buildType.wall:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //        case buildType.windowswall:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //        case buildType.cut:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //        case buildType.roof:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //        case buildType.stairs:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //        case buildType.beam:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //        case buildType.door:
    //            result = new Vector3(5.0f, 0.0f, 5.0f);
    //            break;
    //    }
    //    SetSize(result);
    //}

    private void SetSize(Vector3 size_)
    {
        Size[0] = size_.x;
        Size[1] = size_.y;
        Size[2] = size_.z;
    }

    //private void setPoint()
    //{
    //    //dotPoint[0] = new Vector3(middlePos.x, middlePos.y, middlePos.z); //중간
    //    //dotPoint[1] = new Vector3(middlePos.x - Size[0] / 2 + BorderSize,
    //    //    middlePos.y + Size[1] / 2 - BorderSize, middlePos.z); //좌측상단
    //    //dotPoint[2] = new Vector3(middlePos.x + Size[0] / 2 - BorderSize,
    //    //    middlePos.y + Size[1] / 2 - BorderSize, middlePos.z); //우측상단
    //    //dotPoint[3] = new Vector3(middlePos.x - Size[0] / 2 + BorderSize,
    //    //    middlePos.y - Size[1] / 2 + BorderSize, middlePos.z); //좌측하단
    //    //dotPoint[4] = new Vector3(middlePos.x + Size[0] / 2 - BorderSize,
    //    //    middlePos.y - Size[1] / 2 + BorderSize, middlePos.z); //우측하단
    //    dotPoint[0] = new Vector3(middlePos.x, middlePos.y, middlePos.z); //중간
    //    dotPoint[1] = new Vector3(middlePos.x - Size[0] / 2 + BorderSize, 
    //        middlePos.y, middlePos.z + Size[2] / 2 - BorderSize); //좌측상단
    //    dotPoint[2] = new Vector3(middlePos.x + Size[0] / 2 - BorderSize,
    //        middlePos.y, middlePos.z + Size[2] / 2 - BorderSize); //우측상단
    //    dotPoint[3] = new Vector3(middlePos.x - Size[0] / 2 + BorderSize,
    //        middlePos.y, middlePos.z - Size[2] / 2 + BorderSize); //좌측하단
    //    dotPoint[4] = new Vector3(middlePos.x + Size[0] / 2 - BorderSize,
    //        middlePos.y, middlePos.z - Size[2] / 2 + BorderSize); //우측하단
    //    for (int i = 0; i < 5; i++)
    //    {
    //        Debug.DrawRay(dotPoint[i], Vector3.up, Color.blue);
    //    }
    //}

    private bool IsOnSlope(Vector3 Pos)
    {
        Ray ray = new Ray(Pos, Vector3.down);
        //Debug.DrawLine(Pos, ray.direction * RAYCAST_DISTANCE, Color.red);
        if (Physics.Raycast(ray, out downHit, RAYCAST_DISTANCE))
        {
            float angle = Vector3.Angle(Vector3.up, downHit.normal);

            if (angle >= -45.0f && angle <= 45.0f)
            {
                //Debug.DrawLine(Pos, downHit.normal * RAYCAST_DISTANCE, Color.green);
                return true;
            }
            else
            {
                //Debug.DrawLine(Pos, downHit.normal * RAYCAST_DISTANCE, Color.yellow);
            }
        }
        return false;
    }


    private void CheckBuild()
    {
        if (IsOnSlope(dotPoint[0]) &&
            IsOnSlope(dotPoint[1]) &&
            IsOnSlope(dotPoint[2]) &&
            IsOnSlope(dotPoint[3]) &&
            IsOnSlope(dotPoint[4]))
        {
            isAble = true;
        }
        else
        {
            isAble = false;
        }
    }
}

// Regucy
//public class PrevObjInfo : MonoBehaviour
//{
//    public bool isBuildAble = false;
//    private List<GameObject> cols = new List<GameObject>();


//    void Update()
//    {
//        changeCursor();
//    }

//    private void changeCursor()
//    {
//        if (cols.Count > 0)
//        {
//            isBuildAble = false;
//        }
//        else
//        {
//            isBuildAble = true;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.layer == LayerMask.NameToLayer("build"))
//        {
//            cols.Add(other.gameObject);
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.gameObject.layer == LayerMask.NameToLayer("build"))
//        {
//            cols.Remove(other.gameObject);
//        }
//    }

//    public void deleteObjTime()
//    {
//        if (cols.Count > 0)
//        {
//            for (int i = 0; i < cols.Count; i++)
//            {
//                cols.RemoveAt(0);
//            }
//        }
//    }
//}
