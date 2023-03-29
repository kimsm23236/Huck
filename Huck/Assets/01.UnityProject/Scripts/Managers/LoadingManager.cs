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
    NavMeshBaking,

    Complete,
    NumStage = Complete
}

public class LoadingManager : Singleton<LoadingManager>
{
    public static string nextScene;

    // �ε� �߿� ǥ�õ� UI ��ҵ�
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
        // ���� ���� �񵿱������� �ε�
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // �ε� �� UI Ȱ��ȭ
        loadingScreen.SetActive(true);

        // ���� �� �񵿱������� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        OnStatusReported(EGenerationStage.EnterPlayScene, "Load PlayScene");
        // �ε� ���� ��Ȳ ����
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
