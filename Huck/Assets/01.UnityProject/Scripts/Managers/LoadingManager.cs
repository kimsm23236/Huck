using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum EGenerationStage
{
    EnterPlayScene = 1,

    Beginning,
    BuildTextureMap,
    BuildDetailMap,
    BuildLowResolutionBiomeMap,
    BuildHighResolutionBiomeMap,
    HeightMapGeneration,
    TerrainPainting,
    ObjectPlacement,
    DetailPainting,

    Complete,
    NumStage = Complete
}

public class LoadingManager : Singleton<LoadingManager>
{
    public static string nextScene;

    // 로딩 중에 표시될 UI 요소들
    public GameObject loadingCanvas = default;
    public GameObject loadingScreen = default;
    public TMP_Text loadingText = default;
    [SerializeField]
    private ProcGenManager procGenManager = default;
    private Animator loadingScreenAnim = default;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(loadingCanvas);
    }
    void Start()
    {
        nextScene = GData.SCENENAME_PLAY;
        loadingScreenAnim = loadingScreen.GetComponent<Animator>();
        // 다음 씬을 비동기적으로 로드
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // 로딩 중 UI 활성화
        loadingScreen.SetActive(true);

        // 다음 씬 비동기적으로 로드
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        OnStatusReported(EGenerationStage.EnterPlayScene, "Load PlayScene");
        // 로딩 진행 상황 감시
        while (!operation.isDone)
        {
            OnStatusReported(EGenerationStage.EnterPlayScene, "Load PlayScene");
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void OnStatusReported(EGenerationStage currentStage, string status)
    {
        string newText = $"Step {(int)currentStage} of {(int)EGenerationStage.NumStage} : {status} ";
        loadingText.text = newText;

        Debug.Log($"{currentStage}");
        if(currentStage == EGenerationStage.Complete)
        {
            loadingScreenAnim.SetBool("isFadeOut", true);
            SceneManager.UnloadSceneAsync(GData.SCENENAME_LOADING);
            Debug.Log($"after Play animation");
        }
    }
}
