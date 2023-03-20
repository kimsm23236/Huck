using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProcGenDebugUI : MonoBehaviour
{
    [SerializeField]
    Button regenerateButton;
    [SerializeField]
    TextMeshProUGUI statusDisplay;
    [SerializeField]
    ProcGenManager targetManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnRegenerate()
    {
        regenerateButton.interactable = false;
        StartCoroutine(PerformRegeneration());
    }
    IEnumerator PerformRegeneration()
    {
        yield return targetManager.AsyncRegenerateWorld(OnStatusReported);

        regenerateButton.interactable = true;
    }

    void OnStatusReported(EGenerationStage currentStage, string status)
    {
        statusDisplay.text = $"Step {(int)currentStage} of {(int)EGenerationStage.NumStage} : {status}";
    }
}
