using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeConfig
{
    public BiomeConfigSO Biome;

    [Range(0f, 1.0f)] public float Weighting = 1f;
}

[CreateAssetMenu(fileName = "ProcGen Config", menuName = "Procedural Generation/ProcGen Configuration", order = -1)]
public class ProcGenConfigSO : ScriptableObject
{
    public List<BiomeConfig> Biomes;

    public enum EBiomeMapBaseResolution
    {
        Size_64x64      = 64,
        Size_128x128    = 128,
        Size_256x256    = 256,
        Size_512x512    = 512,
        Size_1024x1024 = 1024,
    }

    [Range(0f, 1.0f)]
    public float BiomeSeedPointDensity = 0.1f;

    public EBiomeMapBaseResolution biomeMapResolution = EBiomeMapBaseResolution.Size_64x64;

    public GameObject InitialHeightModifier;
    public GameObject HeightPostProcessingModifier;

    public GameObject PaintingPostProcessingModifier;
    public GameObject DetailPaintingPostProcessingModifier;

    public float waterHeight = 15f;

    public int NumBiomes => Biomes.Count;

    public float TotalWeighting
    {
        get
        {
            float sum = 0f;

            foreach(var config in Biomes)
            {
                sum += config.Weighting;
            }
            return sum;
        }
    }
}
