using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            // ����ó�� instance�� null���� ��
            if (instance == null)
            {
                // ��򰡿� instance�� ������ ã�Ƽ� ����
                instance = (T)FindObjectOfType(typeof(T));
                // ã�ƺôµ��� ������ ���ο� ������Ʈ�� ������
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
        if(SceneManager.GetActiveScene().name == GData.SCENENAME_TITLE)
        {
            return;
        }
        // ����: �ش� ������Ʈ�� �θ� ������Ʈ�� �ֻ����� ���𰡰� �����Ѵٸ�
        if (transform.parent != null && transform.root != null)
        {
            // �����Ѵٸ� �� ����� ������Ŵ
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            // �������������� �ڱ��ڽ��� ������Ŵ
            DontDestroyOnLoad(this.gameObject);
        }
        Init();
        // �̷��� ����ó���� �ϴ� ������ �ֻ����� Managers���� �������Ʈ�� �����
        // �� �ȿ� Game, Sound, Ui���� Manager�� �־ �����ҷ��� �� ����
        Init();
    }

    protected virtual void Init()
    {
        // Awake() ����
    }
}
// [��ó] �̱��� | �ۼ��� �ε�����
