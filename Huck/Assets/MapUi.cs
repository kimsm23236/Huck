using UnityEngine;
using UnityEngine.UI;

public class MapUi : MonoBehaviour
{
    private Image mapImage;
    private Image playerPosImage;
    private Image bossCastlePosImage;

    [SerializeField]
    private int terrainSizeX = default;
    [SerializeField]
    private int terrainSizeY = default;
    [SerializeField]
    private int mapSizeX = default;
    [SerializeField]
    private int mapSizeY = default;

    Material material = default;

    // Start is called before the first frame update
    public delegate void EventHandler();
    public EventHandler onShowMap;
    private void Awake()
    {
        Debug.Log($"MapUI Awake()");
        mapImage = GetComponent<Image>();
        playerPosImage = gameObject.FindChildObj("PlayerPos").GetComponent<Image>();
        bossCastlePosImage = gameObject.FindChildObj("BossPos").GetComponent<Image>();

        LoadingManager.Instance.onFinishLoading += SetMapImage;
        LoadingManager.Instance.onFinishLoading += SetBossImagePos;

        mapSizeX = Mathf.FloorToInt(mapImage.rectTransform.sizeDelta.x);
        mapSizeY = Mathf.FloorToInt(mapImage.rectTransform.sizeDelta.y);
        Debug.Log($"mapSize, X : {mapSizeX}, Y : {mapSizeY}");

        material = mapImage.material;

        transform.parent.gameObject.SetActive(false);;
    }
    void Start()
    {
        
    }
    void SetMapImage()
    {
        Debug.Log($"SetMapImage()");
        Texture2D texture2D = UIManager.Instance.worldMapTexture;
        Texture2D fogTexture = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGB24, false);
        // material.SetTexture("", fogTexture);
        material.SetTexture("_MainTex2", texture2D);
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        mapImage.sprite = sprite;
    }
    void SetBossImagePos()
    {
        terrainSizeX = Mathf.FloorToInt(GameManager.Instance.terrain.terrainData.size.x);
        terrainSizeY = Mathf.FloorToInt(GameManager.Instance.terrain.terrainData.size.z);


        Vector3 bossPos = GameManager.Instance.bossPos.position;
        Debug.Log($"bossPos : {bossPos}");
        float xRatio = bossPos.x / terrainSizeX;
        float yRatio = bossPos.z / terrainSizeY;
        Debug.Log($"xRatio : {xRatio}, yRatio : {yRatio}");
        float newBossImagePosX = mapSizeX * xRatio;
        float newBossImagePosY = mapSizeY * yRatio;
        Debug.Log($"mapSizeX : {mapSizeX}, mapSizeY : {mapSizeY}");
        Debug.Log($"newBossImagePosX : {newBossImagePosX}, newBossImagePosY : {newBossImagePosY}");
        bossCastlePosImage.rectTransform.anchoredPosition = new Vector3(newBossImagePosX, newBossImagePosY, 0f);
    }
    void SetPlayerImagePos()
    {
        Vector3 playerPos = GameManager.Instance.playerObj.transform.position;
    }
}
