using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF : MonoBehaviour
{
    float timer;
    float setTime;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        setTime = 30;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        lF();
    }

    void lF()
    {
        if (Input.anyKey && Time.timeScale == 0 && timer < setTime)
        {
            Time.timeScale = 1;
            LF lF = GetComponent<LF>();
            lF.enabled = false;
        }
    }
}
