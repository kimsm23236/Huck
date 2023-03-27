using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapModifier_Smooth : BaseHeightMapModifier
{
    [SerializeField] int SmoothingKernelSize = 5;
    [SerializeField] bool useAdaptiveKernel = false;
    [SerializeField] [Range(0f, 1f)] float maxHeightThreshold = 0.5f;
    [SerializeField] int minKernelSize = 2;
    [SerializeField] int maxKernelSize = 7;
    public override void Execute(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        if(biomeMap != null)
        {
            // Debug.LogError("HeightModifier_Smooth is not supported as a per biome modifier [" + gameObject.name + "]");
            // return;
        }

        float[,] smoothHeight = new float[mapResolution, mapResolution];

        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                float heightSum = 0f;
                int numValues = 0;
                
                // set the kernell size
                int kernelSize = SmoothingKernelSize;
                if(useAdaptiveKernel)
                {
                    kernelSize = Mathf.RoundToInt(Mathf.Lerp(minKernelSize, maxKernelSize, heightMap[x, y] / maxHeightThreshold));
                }

                // sum the neighbouring value
                for(int yDelta = -kernelSize; yDelta <= kernelSize; yDelta++)
                {
                    int workingY = y + yDelta;
                    if (workingY < 0 || workingY >= mapResolution)
                        continue;

                    for(int xDelta = -kernelSize; xDelta <= kernelSize; xDelta++)
                    {
                        int workingX = x + xDelta;
                        if (workingX < 0 || workingX >= mapResolution)
                            continue;

                        heightSum += heightMap[workingX, workingY];
                        ++numValues;
                    }
                }
                // store the smoothed
                smoothHeight[x, y] = heightSum / numValues;
            }
        }

        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                // blend based on strength
                heightMap[x, y] = Mathf.Lerp(heightMap[x, y], smoothHeight[x,y], Strength);
            }
        }
    }
}
