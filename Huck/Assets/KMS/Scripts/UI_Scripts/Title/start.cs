using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    public Transform startBtn = default;
    private string nextScene = default;

    public void OnStartBtn()
    {
        nextScene = GData.SCENENAME_LOADING;
        SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        //GFunc.LoadScene(nextScene);
        //GFunc.LoadScene("SampleTestScene");
    }

    public void OnEnterMouseThis_S()
    {
        startBtn.position += new Vector3(0, 0, 0.2f);
    }

    public void OnExitMouseThis_S()
    {
        startBtn.position += new Vector3(0, 0, -0.2f);
    }
}
