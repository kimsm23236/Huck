using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomPainterConfig
{
    public TextureConfig textureToPaint;
    [Range(0f, 1f)] public float intensityModifier = 1f;
    public float noiseScale;
    [Range(0f, 1f)] public float noiseThreshold;
}

public class TexturePainter_Random : BaseTexturePainter
{
    [SerializeField] TextureConfig baseTexture;
    [SerializeField] List<RandomPainterConfig> paintingConfigs;

    public override void Execute(ProcGenManager manager, int mapResolution, float[,] heightMap, Vector3 heightmapScale, float[,] slopeMap, float[,,] alphaMaps, int alphaMapResolution, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int baseTextureLayer = manager.GetLayerForTexture(baseTexture);
        for (int y = 0; y < alphaMapResolution; y++)
        {
            int heightMapY = Mathf.FloorToInt((float)y * (float)mapResolution / (float)alphaMapResolution);
            for (int x = 0; x < alphaMapResolution; x++)
            {
                int heightMapX = Mathf.FloorToInt((float)x * (float)mapResolution / (float)alphaMapResolution);

                if (biomeIndex >= 0 && biomeMap[heightMapX, heightMapY] != biomeIndex)
                    continue;
                
                // perform the painting
                foreach(var config in paintingConfigs)
                {
                    // check if noise test passed?
                    float noiseValue = Mathf.PerlinNoise(x * config.noiseScale, y * config.noiseScale);
                    if(Random.Range(0f, 1f) >= noiseValue)
                    {
                        int layer = manager.GetLayerForTexture(config.textureToPaint);
                        alphaMaps[x, y, layer] = Strength * config.intensityModifier;
                    }
                }
                
                alphaMaps[x, y, baseTextureLayer] = Strength;
            }
        }
    }

    [System.NonSerialized] List<TextureConfig> cachedTexture = null;

    public override List<TextureConfig> RetrieveTextures()
    {
        if(cachedTexture == null)
        {
            cachedTexture = new List<TextureConfig>();
            cachedTexture.Add(baseTexture);
            foreach(var config in paintingConfigs)
            {
                cachedTexture.Add(config.textureToPaint);
            }
        }
        return cachedTexture;
    }
}
