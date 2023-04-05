using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유니티 Create로 스크립터블 오브젝트를 만들수 있는 menu가 생김
[CreateAssetMenu(fileName = "Biome Config", menuName = "Procedural Generation/Biome Configuration", order = -1)]
public class BiomeConfigSO : ScriptableObject
{
    // 바이옴 이름
    public string Name;

    // 최소 최대 밀도 * 높을수록 바이옴 크기 넓어짐
    [Range(0f, 1.0f)]
    public float MinIntensity = 0.5f;
    [Range(0f, 1.0f)]
    public float MaxIntensity = 1f;

    // 최소 최대 감쇠율 * 높을수록 바이옴 크기 작아짐
    [Range(0f, 1.0f)]
    public float MaxDecayRate = 0.01f;
    [Range(0f, 1.0f)]
    public float MinDecayRate = 0.02f;

    // 맵 표시 컬러
    public Color mapColor = default;

    // 높이 수정자
    public GameObject HeightModifier;
    // 텍스처레이어 수정자
    public GameObject TerrainPainter;
    // 오브젝트 배치자
    public GameObject ObjectPlacer;
    // 디테일 페인트 수정자
    public GameObject DetailPainter;

    public List<TextureConfig> RetrieveTextures()
    {
        if (TerrainPainter == null)
            return null;
        
        List<TextureConfig> allTextures = new List<TextureConfig>();
        BaseTexturePainter[] allPainters = TerrainPainter.GetComponents<BaseTexturePainter>();
        foreach(var painter in allPainters)
        {
            var painterTextures = painter.RetrieveTextures();

            if(painterTextures == null || painterTextures.Count == 0)
            {
                continue;
            }
            allTextures.AddRange(painterTextures);
        }

        return allTextures;
    }
    public List<TerrainDetailConfig> RetrieveTerrainDetails()
    {
        if (DetailPainter == null)
            return null;

        // extract all terrain details from every painter
        List<TerrainDetailConfig> allTerrainDetails = new List<TerrainDetailConfig>();
        BaseDetailPainter[] allPainters = DetailPainter.GetComponents<BaseDetailPainter>();
        foreach (var painter in allPainters)
        {
            var terrainDetails = painter.RetrieveTerrainDetails();

            if (terrainDetails == null || terrainDetails.Count == 0)
                continue;

            allTerrainDetails.AddRange(terrainDetails);
        }

        return allTerrainDetails;
    }

}
