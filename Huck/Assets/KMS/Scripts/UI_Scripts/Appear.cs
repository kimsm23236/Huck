using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Appear : MonoBehaviour
{
    private Image Bg = default;
    private Text Yd = default;
    private Image Edg = default;
    private Text TT = default;
    private int cnt = 0;

    // Player Die Scene
    void Start()
    {
        Bg = UIManager.Instance.dead_1;
        Yd = UIManager.Instance.dead_2;
        Edg = UIManager.Instance.dead_3;
        TT = UIManager.Instance.dead_4;
    }
    private void Update()
    {
        textAppear();
    }
    void textAppear()
    {
        if (cnt < 2550)
        {
            Bg.color += new Color(0, 0, 0, 0.1f) * Time.deltaTime;
            Yd.color += new Color(0, 0, 0, 0.1f) * Time.deltaTime;
            Edg.color += new Color(0, 0, 0, 0.1f) * Time.deltaTime;
            TT.color += new Color(0, 0, 0, 0.1f) * Time.deltaTime;
            cnt++;
        }
    }
}
