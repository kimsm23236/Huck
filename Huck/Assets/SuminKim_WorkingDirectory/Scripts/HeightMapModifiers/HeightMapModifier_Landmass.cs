using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapModifier_Landmass : BaseHeightMapModifier
{
    [SerializeField] float landmassRadius = 500f;
    [SerializeField] float TargetHeight;

    public override void Execute(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        if (biomeMap != null)
        {
            // Debug.LogError("HeightModifier_Smooth is not supported as a per biome modifier [" + gameObject.name + "]");
            // return;
        }

        float[,] newHeight = new float[mapResolution, mapResolution];
        int center = mapResolution / 2;

        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                float heightSum = 0f;
                int numValues = 0;

                // 원 범위 체크
                double distance = Mathf.Sqrt(Mathf.Pow(y - center, 2) + Mathf.Pow(x - center, 2));
                if (distance < landmassRadius)
                    continue;

                /*
                // set the kernell size
                int kernelSize = SmoothingKernelSize;
                if (useAdaptiveKernel)
                {
                    kernelSize = Mathf.RoundToInt(Mathf.Lerp(minKernelSize, maxKernelSize, heightMap[x, y] / maxHeightThreshold));
                }

                // sum the neighbouring value
                for (int yDelta = -kernelSize; yDelta <= kernelSize; yDelta++)
                {
                    int workingY = y + yDelta;
                    if (workingY < 0 || workingY >= mapResolution)
                        continue;

                    for (int xDelta = -kernelSize; xDelta <= kernelSize; xDelta++)
                    {
                        int workingX = x + xDelta;
                        if (workingX < 0 || workingX >= mapResolution)
                            continue;

                        heightSum += heightMap[workingX, workingY] + offsetValue;
                        ++numValues;
                    }
                }
                */
                // store the smoothed
                newHeight[x, y] = TargetHeight / heightmapScale.y;
            }
        }

        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                // 원 범위 체크
                double distance = Mathf.Sqrt(Mathf.Pow(y - center, 2) + Mathf.Pow(x - center, 2));
                if (distance < landmassRadius)
                    continue;

                // blend based on strength
                heightMap[x, y] = Mathf.Lerp(heightMap[x, y], newHeight[x, y], Strength);
            }
        }
    }
}
