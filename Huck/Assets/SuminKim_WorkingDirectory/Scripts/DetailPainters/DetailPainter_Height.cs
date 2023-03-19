using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailPainter_Height : BaseDetailPainter
{
    [SerializeField] TerrainDetailConfig terrainDetail;
    [SerializeField] float startHeight;
    [SerializeField] float endHeight;
    [SerializeField] AnimationCurve intensity;
    [SerializeField] bool suppressOtherDetails = false;
    [SerializeField] AnimationCurve suppressionIntensity;

    public override void Execute(ProcGenManager manager, int mapResolution, float[,] heightMap, Vector3 heightmapScale, float[,] slopeMap, float[,,] alphaMaps, int alphaMapResolution, List<int[,]> detailLayerMaps, int detailMapResolution, int maxDetailsPerPatch, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int detailLayer = manager.GetDetailLayerForTerrainDetail(terrainDetail);

        float heightMapStart = startHeight / heightmapScale.y;
        float heightMapEnd = endHeight / heightmapScale.y;
        float heightMapRangeInv = 1f / (heightMapEnd - heightMapStart);

        int numDetailLayers = detailLayerMaps.Count;

        for (int y = 0; y < detailMapResolution; ++y)
        {
            int heightMapY = Mathf.FloorToInt((float)y * (float)mapResolution / (float)detailMapResolution);

            for (int x = 0; x < detailMapResolution; ++x)
            {
                int heightMapX = Mathf.FloorToInt((float)x * (float)mapResolution / (float)detailMapResolution);

                // skip if we have a biome and this is not our biome
                if (biomeIndex >= 0 && biomeMap[heightMapX, heightMapY] != biomeIndex)
                    continue;

                float height = heightMap[heightMapX, heightMapY];

                // outside of height range
                if (height < heightMapStart || height > heightMapEnd)
                    continue;

                float heightPercentage = (height - heightMapStart) * heightMapRangeInv;
                detailLayerMaps[detailLayer][x, y] = Mathf.FloorToInt(strength * intensity.Evaluate(heightPercentage) * maxDetailsPerPatch);

                // if suppression of other details is on then update the other layers
                if (suppressOtherDetails)
                {
                    float suppression = suppressionIntensity.Evaluate(heightPercentage);

                    // apply suppression to other layers
                    for (int layerIndex = 0; layerIndex < numDetailLayers; ++layerIndex)
                    {
                        if (layerIndex == detailLayer)
                            continue;

                        detailLayerMaps[detailLayer][x, y] = Mathf.FloorToInt(detailLayerMaps[detailLayer][x, y] * suppression);
                    }
                }
            }
        }
    }

    public override List<TerrainDetailConfig> RetrieveTerrainDetails()
    {
        List<TerrainDetailConfig> allTerrainDetails = new List<TerrainDetailConfig>(1);
        allTerrainDetails.Add(terrainDetail);

        return allTerrainDetails;
    }
}
