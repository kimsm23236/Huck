using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif  // UNITY_EDITOR

public class ObjectPlacer_Random : BaseObjectPlacer
{
    [SerializeField]
    float TargetDensity = 0.1f;
    [SerializeField]
    int MaxSpawnCount = 1000;
    public override void Execute(   ProcGenConfigSO globalConfig,Transform objectRoot, int mapResolution, float[,] heightMap,
                                    Vector3 heightmapScale, float[,] slopeMap, float[,,] alphaMaps, int alphaMapResolution,
                                    byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        base.Execute(globalConfig, objectRoot, mapResolution, heightMap, heightmapScale, slopeMap, alphaMaps, alphaMapResolution, 
                        biomeMap, biomeIndex, biome);
        // get potential spawn location
        List<Vector3> candidateLocations = GetAllLocationForBiome(globalConfig, mapResolution, heightMap, heightmapScale, biomeMap, biomeIndex);

        ExecuteSimpleSpawning(globalConfig, objectRoot, candidateLocations);
    }
}

/*
    // height is invalid?
    if(height < globalConfig.waterHeight && !canGoInWater)
            continue;
    if(height >= globalConfig.waterHeight && !canGoAboveWater)
            continue;

    // skip if outside of height limits
    if(hasHeightLimits && (height < minHeightToSpawn || height >= maxHeightToSpawn))
        continue;
*/