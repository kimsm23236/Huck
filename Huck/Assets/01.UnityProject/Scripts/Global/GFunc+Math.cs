using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GFunc
{
    //! Vector2Int 의 값을 절댓값으로 리턴하는 함수
    public static Vector2Int Abs(Vector2Int target)
    {
        target.x = Mathf.Abs(target.x);
        target.y = Mathf.Abs(target.y);
        return target;
    }   // Abs()
}
