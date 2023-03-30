using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            // 예외처리 instance가 null값일 때
            if (instance == null)
            {
                // 어딘가에 instance가 있으면 찾아서 저장
                instance = (T)FindObjectOfType(typeof(T));
                // 찾아봤는데도 없으면 새로운 오브젝트를 저장함
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
    // 조건: 해당 오브젝트의 부모 오브젝트나 최상위에 무언가가 존재한다면
        if (transform.parent != null && transform.root != null)
        {
            // 존재한다면 그 대상을 유지시킴
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            // 존재하지않으면 자기자신을 유지시킴
            DontDestroyOnLoad(this.gameObject);
        }
        // 이러한 예외처리를 하는 이유는 최상위에 Managers같은 빈오브젝트를 만들고
        // 그 안에 Game, Sound, Ui등의 Manager를 넣어서 관리할려고 한 것임
    }

    protected virtual void Init()
    {
        // Awake() 구현
    }
}
// [출처] 싱글톤 | 작성자 부두좀비
