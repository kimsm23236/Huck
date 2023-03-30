using System.Collections;
using UnityEngine;

public class BuildObjDir : MonoBehaviour
{
    private buildType PrevType = buildType.None;

    private float[] Size = new float[3];
    private Vector3[] dotPoint = new Vector3[5];
    private Vector3 middlePos;
    private float BorderSize = 0.3f;
    private const float MAX_SIZE = 500.0f;

    private bool isAble = false;

    private const float RAYCAST_DISTANCE = 2.0f;
    private RaycastHit downHit;
    void Update()
    {
        setPoint();
        CheckBuild();
    }

    public void setMid(Vector3 mid)
    {
        middlePos = mid;
    }

    public void SetType(buildType setType, Vector3 mid)
    {
        PrevType = setType;
        middlePos = mid;
        SizeToType();
    }

    private void SetSize(float sizeX, float sizeY, float sizeZ)
    {
        Size[0] = sizeX;
        Size[1] = sizeY;
        Size[2] = sizeZ;
    }

    //���簢�� ������ �ֱ�
    private void SizeToType()
    {
        switch (PrevType)
        {
            case buildType.None:
                SetSize(MAX_SIZE, MAX_SIZE, MAX_SIZE);
                break;
            case buildType.Wall:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.Cut:
                SetSize(1f, 1f, 1f);
                break;
            case buildType.Door:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.Windowswall:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.Stairs:
                SetSize(2.5f, 2.5f, 2.5f);
                break;
            case buildType.Beam:
                SetSize(0.3f, 2.5f, 0.3f);
                break;
            case buildType.Floor:
                SetSize(2.5f, 2.5f, 0.1f);
                break;
            case buildType.Roof:
                SetSize(1f, 1f, 1f);
                break;
        }
    }

    private void setPoint()
    {
        dotPoint[0] = new Vector3(middlePos.x, middlePos.y, middlePos.z); //�߰�
        dotPoint[1] = new Vector3(middlePos.x - Size[0] / 2 + BorderSize,
            middlePos.y + Size[1] / 2 - BorderSize, middlePos.z); //�������
        dotPoint[2] = new Vector3(middlePos.x + Size[0] / 2 - BorderSize,
            middlePos.y + Size[1] / 2 - BorderSize, middlePos.z); //�������
        dotPoint[3] = new Vector3(middlePos.x - Size[0] / 2 + BorderSize,
            middlePos.y - Size[1] / 2 + BorderSize, middlePos.z); //�����ϴ�
        dotPoint[4] = new Vector3(middlePos.x + Size[0] / 2 - BorderSize,
            middlePos.y - Size[1] / 2 + BorderSize, middlePos.z); //�����ϴ�
    }

    private bool IsOnSlope(Vector3 Pos)
    {
        Ray ray = new Ray(Pos, Vector3.down);
        Debug.DrawLine(Pos, ray.direction * RAYCAST_DISTANCE, Color.red);
        if (Physics.Raycast(ray, out downHit, RAYCAST_DISTANCE))
        {
            float angle = Vector3.Angle(Vector3.up, downHit.normal);

            if (angle >= -45.0f && angle <= 45.0f)
            {
                Debug.DrawLine(Pos, downHit.normal * RAYCAST_DISTANCE, Color.green);
                return true;
            }
            else
            {
                Debug.DrawLine(Pos, downHit.normal * RAYCAST_DISTANCE, Color.yellow);
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
