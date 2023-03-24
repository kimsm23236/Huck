using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevObjDir : MonoBehaviour
{
    public buildDirType PrevDir = buildDirType.None;
    private buildType PrevType = buildType.none;

    //size info
    /**
     * size[0] = width
     * size[1] = height
     * size[2] = depth
     */
    private float[] Size = new float[3];
    private Vector3 middlePos;
    private float BorderSize = 0.3f;
    private const float MAX_SIZE = 500.0f;

    void Update()
    {
        
    }

    public void SetType(buildType set,Vector3 mid)
    {
        PrevType = set;
        middlePos = mid;
        SizeToType();
    }

    private void SetSize(float sizeX, float sizeY, float sizeZ)
    {
        Size[0] = sizeX;
        Size[1] = sizeY;
        Size[2] = sizeZ;
    }
    
    //정사각형 사이즈 주기
    private void SizeToType()
    {
        switch (PrevType)
        {
            case buildType.none:
                SetSize(MAX_SIZE, MAX_SIZE, MAX_SIZE);
                break;
            case buildType.wall:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.cut:
                SetSize(1f, 1f, 1f);
                break;
            case buildType.door:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.windowswall:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.stairs:
                SetSize(2.5f, 2.5f, 2.5f);
                break;
            case buildType.beam:
                SetSize(0.3f, 2.5f, 0.3f);
                break;
            case buildType.floor:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.roof:
                SetSize(1f, 1f, 1f);
                break;
        }
    }

    public void RayToPrevObjDir(RaycastHit hit)
    {
        if (hit.point != null && hit.transform.gameObject.Equals(this))
        {
            int numberX = 0, numberY = 0;

            //Left
            if ((middlePos.x - Size[0] / 2 + BorderSize) > hit.point.x)
            {
                numberX = 1;
            }

            //center
            else if ((middlePos.x - Size[0] / 2 + BorderSize) <= hit.point.x &&
                (middlePos.x + Size[0] / 2 - BorderSize) >= hit.point.x)
            {
                numberX = 2;
            }

            //right
            else if ((middlePos.x + Size[0] / 2 - BorderSize) < hit.point.x)
            {
                numberX = 3;
            }
            else
            {
                numberX = 0;
            }

            //up
            if ((middlePos.y + Size[1] / 2 - BorderSize) < hit.point.y)
            {
                numberY = 1;
            }
            //mid
            else if ((middlePos.y + Size[1] / 2 - BorderSize) >= hit.point.y &&
                (middlePos.y - Size[1] / 2 + BorderSize) <= hit.point.y)
            {
                numberY = 2;
            }
            //down
            else if ((middlePos.y - Size[1] / 2 + BorderSize) > hit.point.y)
            {
                numberY = 3;
            }
            else
            {
                numberY = 0;
            }

            //조건 확인
            //LeftUp
            if (numberX == 1 && numberY == 1)
            {
                PrevDir = buildDirType.LeftUp_P;
            }
            //midUp
            else if (numberX == 2 && numberY == 1)
            {
                PrevDir = buildDirType.UpBorder;
            }
            //rightUp
            else if (numberX == 3 && numberY == 1)
            {
                PrevDir = buildDirType.RightUp_P;
            }

            //Leftmid
            else if (numberX == 1 && numberY == 2)
            {
                PrevDir = buildDirType.LeftBorder;
            }
            //rightmid
            else if (numberX == 3 && numberY == 2)
            {
                PrevDir = buildDirType.RightBorder;
            }

            //LeftDown
            else if (numberX == 1 && numberY == 3)
            {
                PrevDir = buildDirType.LeftDown_P;
            }
            //midDown
            else if (numberX == 2 && numberY == 3)
            {
                PrevDir = buildDirType.DownBorder;
            }
            //rightDown
            else if (numberX == 3 && numberY == 3)
            {
                PrevDir = buildDirType.RightDown_P;
            }
            else
            {
                PrevDir = buildDirType.None;
            }
        }
    }
}

public enum buildDirType
{
    None = -1,LeftUp_P,UpBorder,RightUp_P,
    LeftBorder,RightBorder,
    LeftDown_P,DownBorder,RightDown_P
}
