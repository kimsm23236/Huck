using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.UIElements;
using UnityEngine.UI;

public enum EGenerationStage
{
    EnterPlayScene = 1,

    Beginning,
    BuildTextureMap,
    BuildDetailMap,
    BuildBiomeMap,
    HeightMapGeneration,
    SetupWorldMap,
    TerrainPainting,
    NavMeshBaking,
    ObjectPlacement,
    DetailPainting,
    PostProcessOnLoading,

    Complete,
    NumStage = Complete
}

public class LoadingManager : Singleton<LoadingManager>
{
    public static string nextScene;
    
    // [KMS] Add bool for PlayerControl
    public bool isLoadingEnd = false;
    // [KMS] Add bool for PlayerControl

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
        SceneManager.UnloadSceneAsync(GData.SCENENAME_TITLE);
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

            yield return null;
        }
    }

    public void OnStatusReported(EGenerationStage currentStage, string status)
    {
        string newText = $"Step {(int)currentStage} of {(int)EGenerationStage.NumStage} : {status} ";
        loadingText.text = newText;

        if(currentStage == EGenerationStage.Complete)
        {
            LoadingScreenFadeOutPlay();
            SceneManager.UnloadSceneAsync(GData.SCENENAME_LOADING);
            StartCoroutine(SetActiveFalse5Sec());
            GameManager.Instance.StartBGM();
        }
    }
    IEnumerator SetActiveFalse5Sec()
    {
        yield return new WaitForSeconds(5f);
        loadingCanvas.SetActive(false);

        
    }

    public void LoadingScreenFadeOutPlay()
    {
        loadingScreenAnim.SetBool("isFadeOut", true);
        isLoadingEnd = true;
    }
}
