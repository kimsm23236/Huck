using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator_VoronoiBased : BaseBiomeMapGenerator
{
    [SerializeField] int numCells = 20;
    [SerializeField] int resampleDistance = 20;
    public override void Execute(ProcGenConfigSO globalConfig, int mapResolution, byte[,] biomeMap, float[,] biomeStrengths)
    {
        int cellSize = Mathf.CeilToInt((float)mapResolution / numCells);

        // Generate our seed points
        Vector3Int[] biomeSeeds = new Vector3Int[numCells * numCells];
        for(int cellY = 0; cellY < numCells; cellY++)
        {
            int centerY = Mathf.RoundToInt((cellY + 0.5f) * cellSize);

            for(int cellX = 0; cellX < numCells; cellX++)
            {
                int cellIndex = cellX + cellY * numCells;

                int centerX = Mathf.RoundToInt((cellX + 0.5f) * cellSize);

                biomeSeeds[cellIndex].x = centerX + Random.Range(-cellSize / 2, cellSize / 2);
                biomeSeeds[cellIndex].y = centerY + Random.Range(-cellSize / 2, cellSize / 2);
                biomeSeeds[cellIndex].z = Random.Range(0, globalConfig.NumBiomes);
            }
        }

        // Generate our base biome map
        byte[,] baseBiomeMap = new byte[mapResolution, mapResolution];
        for(int y = 0; y < mapResolution; y++)
        {
            for(int x = 0; x < mapResolution; x++)
            {
                baseBiomeMap[x,y] = FindClosestBiome(x,y,numCells,cellSize,biomeSeeds);
            }
        }

        for(int y = 0; y < mapResolution; y++)
        {
            for(int x = 0; x < mapResolution; x++)
            {
                biomeMap[x,y] = ResampleBiomeMap(x, y, baseBiomeMap, mapResolution);
            }
        }

#if UNITY_EDITOR
        // save out the biome map
        Texture2D biomeMapTexture = new Texture2D(mapResolution, mapResolution, TextureFormat.RGB24, false);
        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                float hue = ((float)baseBiomeMap[x, y] / (float)globalConfig.NumBiomes);
                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }
        biomeMapTexture.Apply();

        System.IO.File.WriteAllBytes("BiomeMap_Voronoi_Base.png", biomeMapTexture.EncodeToPNG());

        biomeMapTexture = new Texture2D(mapResolution, mapResolution, TextureFormat.RGB24, false);
        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                float hue = ((float)biomeMap[x, y] / (float)globalConfig.NumBiomes);
                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }
        biomeMapTexture.Apply();

        System.IO.File.WriteAllBytes("BiomeMap_Voronoi_Final.png", biomeMapTexture.EncodeToPNG());
#endif  // UNITY_EDITOR

    }

    Vector2Int[] NeighbourOffsets = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
    };
    
    byte FindClosestBiome(int x, int y, int numCells, int cellSize, Vector3Int[] biomeSeeds)
    {
        int cellX = x / cellSize;
        int cellY = y / cellSize;
        int cellIndex = cellX + cellY * numCells;

        float closestSeedDistanceSq =   (biomeSeeds[cellIndex].x - x) * (biomeSeeds[cellIndex].x - x) +
                                        (biomeSeeds[cellIndex].y - y) * (biomeSeeds[cellIndex].y - y);
        byte bestBiome = (byte)biomeSeeds[cellX + cellY * numCells].z;

        foreach(var neighbourOffset in NeighbourOffsets)
        {
            int workingCellX = cellX + neighbourOffset.x;
            int workingCellY = cellY + neighbourOffset.y;
            cellIndex = workingCellX + workingCellY * numCells;

            if(workingCellX < 0 || workingCellY < 0 || workingCellX >= numCells || workingCellY >= numCells)
                continue;

            float distanceSq =  (biomeSeeds[cellIndex].x - x) * (biomeSeeds[cellIndex].x - x) +
                                (biomeSeeds[cellIndex].y - y) * (biomeSeeds[cellIndex].y - y);

            if(distanceSq < closestSeedDistanceSq)
            {
                closestSeedDistanceSq = distanceSq;
                bestBiome = (byte)biomeSeeds[cellIndex].z;
            }
        }

        return bestBiome;
    }

    byte ResampleBiomeMap(int x, int y, byte[,] biomeMap, int mapResolution)
    {
        float noise = 2f * (Mathf.PerlinNoise((float)x / mapResolution, (float)y / mapResolution) - 0.5f);

        int newX = Mathf.Clamp(Mathf.RoundToInt(x + noise * resampleDistance), 0, mapResolution - 1);
        int newY = Mathf.Clamp(Mathf.RoundToInt(y + noise * resampleDistance), 0, mapResolution - 1);

        return biomeMap[newX, newY];
    }
}
