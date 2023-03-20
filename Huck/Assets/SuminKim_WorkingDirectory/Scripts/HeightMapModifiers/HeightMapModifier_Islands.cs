using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapModifier_Islands : BaseHeightMapModifier
{
    [SerializeField] [Range(1, 100)] int numIslands = 100;
    [SerializeField] float minIslandSize = 20f;
    [SerializeField] float maxIslandSize = 80f;
    [SerializeField] float minIslandHeight = 10f;
    [SerializeField] float maxIslandHeight = 40f;
    [SerializeField] AnimationCurve islandShapeCurve;

    public override void Execute(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        for(int island = 0; island < numIslands; island++)
        {
            PlaceIsland(globalConfig, mapResolution, heightMap, heightmapScale, biomeMap, biomeIndex, biome);
        }
    }

    void PlaceIsland(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int workingIslandSize = Mathf.RoundToInt(Random.Range(minIslandSize, maxIslandSize) / heightmapScale.x);
        float workingIslandHeight = (Random.Range(minIslandHeight, maxIslandHeight) + globalConfig.waterHeight) / heightmapScale.y;

        int centerX = Random.Range(workingIslandSize, mapResolution - workingIslandSize);
        int centerY = Random.Range(workingIslandSize, mapResolution - workingIslandSize);
        for(int islandY = -workingIslandSize; islandY <= workingIslandSize; islandY++)
        {
            int y = centerY + islandY;

            if(y < 0 || y >= mapResolution)
                continue;

            for(int islandX = -workingIslandSize; islandX <= workingIslandSize; islandX++)
            {
                int x = centerX + islandX;

                if(x < 0 || x >= mapResolution)
                    continue;

                float normalizedDistance = Mathf.Sqrt(islandX * islandX + islandY * islandY) / workingIslandSize;
                if(normalizedDistance > 1)
                    continue;

                float height = workingIslandHeight * islandShapeCurve.Evaluate(normalizedDistance);

                heightMap[x, y] = Mathf.Max(heightMap[x, y], height);
            }
        }
    }
}
