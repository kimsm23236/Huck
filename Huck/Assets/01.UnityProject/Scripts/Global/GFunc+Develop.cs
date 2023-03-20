using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GFunc
{
    #region Print log func
    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Log(object message)
    {
#if DEBUG_MODE
        Debug.Log(message);
#endif      // DEBUG_MODE
    }       // Log()

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Log(object message, UnityEngine.Object context)
    {
#if DEBUG_MODE
        Debug.Log(message, context);
#endif      // DEBUG_MODE
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogWarning(object message)
    {
#if DEBUG_MODE
        Debug.LogWarning(message);
#endif      // DEBUG_MODE
    }       // Log()
    #endregion      // Print log func

    #region Assert for debug
    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Assert(bool condition)
    {
#if DEBUG_MODE
        Debug.Assert(condition);
#endif      // DEBUG_MODE
    }       // Assert()

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Assert(bool condition, object message)
    {
#if DEBUG_MODE
        Debug.Assert(condition, message);
#endif      // DEBUG_MODE
    }       // Assert()
    #endregion      // Assert for debug

    #region Vaild Func
    //! ������Ʈ�� ��ȿ���� �˻��Ѵ�.
    public static bool IsValid<T>(this T component_) where T : Component
    {
        Component convert_ = (Component)(component_ as Component);
        bool isInvalid = convert_ == null || convert_ == default;
        return !isInvalid;
    }   // IsValid()

    //! ������Ʈ�� ��ȿ���� �˻��Ѵ�.
    public static bool IsValid(this GameObject obj_)
    {
        bool isInvalid = (obj_ == null || obj_ == default);
        return !isInvalid;
    }   // IsValid()

    //! ����Ʈ�� ��ȿ���� �˻��Ѵ�.
    public static bool IsValid<T>(this List<T> list_)
    {
        bool isInvalid = (list_ == null || list_ == default || list_.Count < 1);
        return !isInvalid;
    }   // IsValid()
    public static bool IsValid<T>(this List<T> list_, int index_)
    {
        bool isInvalid = (list_.IsValid() == false) ||
            (index_ < 0 || list_.Count <= index_);
        return !isInvalid;
    }   // IsValid()
    #endregion      // Vaild Func

    //! ����Ʈ�� �����ؼ� �����ϴ� �Լ�
    /**
     *  @param int listLength : ������ ����Ʈ�� ����
     *  @param int startIndex : ����Ʈ�� �������� �Ҵ��� �ε����� ���� ����
     *  @return List<T> list_ : ���ӵ� ���ڷ� ������ ����Ʈ
     */
    public static List<int> CreateList(int listLength, int startIndex = 0)
    {
        List<int> list_ = new List<int>();
        for(int i = 0; i < listLength; i++)
        {
            list_.Add(startIndex + i);
        }
        return list_;
    }   // CreateList()

    //! �� ������ ���� Swap �ϴ� �Լ�
    public static void Swap<T>(ref T sourValue, ref T destValue)
    {
        T tempValue = sourValue;
        sourValue = destValue;
        destValue = tempValue;
    }   // Swap()

    //! �� ������ ���� Swap �ϴ� �Լ�
    public static void Swap<T>( (T sourValue, T destValue) swapValue)
    {
        (T sourValue, T destValue) = (swapValue.destValue, swapValue.sourValue);
    }   // Swap()
}
