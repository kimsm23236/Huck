using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LandmarkConfig
{
    public Texture2D heightMap;
    public GameObject prefab;
    public EBiome placedBiome;
    public int Radius;
    public int numToSpawn = 1;
    public bool hasHeightLimits = false;
    public float minHeightToSpawn = 0f;
    public float maxHeightToSpawn = 0f;

    public bool canGoInWater = false;
    public bool canGoAboveWater = true;
}

public class HeightMapModifier_Landmark : BaseHeightMapModifier
{
    [SerializeField]
    List<LandmarkConfig> landmarks;


    protected void SpawnBuilding(ProcGenConfigSO globalConfig, LandmarkConfig landmark,
                                 int spawnX, int spawnY,
                                 int mapResolution, float[,] heightMap, Vector3 heightmapScale,
                                 Transform buildingRoot)
    {
        float averageHeight = 0f;
        int numHeightSamples = 0;

        // sum the height values under the building
        for (int y = -landmark.Radius; y <= landmark.Radius; ++y)
        {
            for (int x = -landmark.Radius; x <= landmark.Radius; ++x)
            {
                // 원 범위 체크
                double distance = Mathf.Sqrt(Mathf.Pow(y - spawnY, 2) + Mathf.Pow(x - spawnX, 2));
                if (distance > landmark.Radius)
                    continue;

                // sum the heightmap values
                averageHeight += heightMap[x + spawnX, y + spawnY];
                ++numHeightSamples;
            }
        }

        // calculate the average height
        averageHeight /= numHeightSamples;

        float targetHeight = averageHeight;

        if (!landmark.canGoInWater)
            targetHeight = Mathf.Max(targetHeight, globalConfig.waterHeight / heightmapScale.y);
        if (landmark.hasHeightLimits)
            targetHeight = Mathf.Clamp(targetHeight, landmark.minHeightToSpawn / heightmapScale.y, landmark.maxHeightToSpawn / heightmapScale.y);

        // apply the building heightmap
        for (int y = -landmark.Radius; y <= landmark.Radius; ++y)
        {
            int workingY = y + spawnY;
            float textureY = Mathf.Clamp01((float)(y + landmark.Radius) / (landmark.Radius * 2f));
            for (int x = -landmark.Radius; x <= landmark.Radius; ++x)
            {
                int workingX = x + spawnX;
                float textureX = Mathf.Clamp01((float)(x + landmark.Radius) / (landmark.Radius * 2f));

                // 원 범위 체크
                double distance = Mathf.Sqrt(Mathf.Pow(workingY - spawnY, 2) + Mathf.Pow(workingX - spawnX, 2));
                if (distance > landmark.Radius)
                    continue;

                // sample the height map
                var pixelColour = landmark.heightMap.GetPixelBilinear(textureX, textureY);
                float strength = pixelColour.r;

                // blend based on strength
                // heightMap[workingX, workingY] = Mathf.Lerp(heightMap[workingX, workingY], targetHeight, strength);
                heightMap[workingX, workingY] = targetHeight;

                //float offsetValue = -5f;
                // calculate the new height
                //float newHeight = heightMap[workingX, workingY] + (offsetValue / heightmapScale.y);

                // blend based on strength
                //heightMap[workingX, workingY] = /*Mathf.Lerp(heightMap[workingX, workingY], */newHeight;//, strength);



            }
        }

        // Spawn the building
        Vector3 buildingLocation = new Vector3(spawnY * heightmapScale.z,
                                               heightMap[spawnX, spawnY] * heightmapScale.y,
                                               spawnX * heightmapScale.x);

        // instantiate the prefab
#if UNITY_EDITOR
        if (Application.isPlaying)
            Instantiate(landmark.prefab, buildingLocation, Quaternion.identity, buildingRoot);
        else
        {
            var spawnedGO = PrefabUtility.InstantiatePrefab(landmark.prefab, buildingRoot) as GameObject;
            spawnedGO.transform.position = buildingLocation;
            Undo.RegisterCreatedObjectUndo(spawnedGO, "Add building");
        }
#else
        Instantiate(landmark.prefab, buildingLocation, Quaternion.identity, buildingRoot);
#endif // UNITY_EDITOR
    }

    protected List<Vector2Int> GetSpawnLocationsForBuilding(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, LandmarkConfig landmarkConfig, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        List<Vector2Int> locations = new List<Vector2Int>(mapResolution * mapResolution / 10);

        for (int y = landmarkConfig.Radius; (y < mapResolution - landmarkConfig.Radius); y += landmarkConfig.Radius * 2)
        {
            for (int x = landmarkConfig.Radius; (x < mapResolution - landmarkConfig.Radius); x += landmarkConfig.Radius * 2)
            {
                float height = heightMap[x, y] * heightmapScale.y;

                // 바이옴 체크
                if ((EBiome)biomeMap[x, y] != landmarkConfig.placedBiome)
                {
                    continue;
                }

                // height is invalid?
                if (height < globalConfig.waterHeight && !landmarkConfig.canGoInWater)
                    continue;
                if (height >= globalConfig.waterHeight && !landmarkConfig.canGoAboveWater)
                    continue;

                // skip if outside of height limits
                if (landmarkConfig.hasHeightLimits && (height < landmarkConfig.minHeightToSpawn || height >= landmarkConfig.maxHeightToSpawn))
                    continue;


                locations.Add(new Vector2Int(x, y));
            }
        }

        return locations;
    }

    public override void Execute(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        var buildingRoot = FindObjectOfType<ProcGenManager>().transform;
        // traverse the features
        foreach (var landmark in landmarks)
        {
            var spawnLocations = GetSpawnLocationsForBuilding(globalConfig, mapResolution, heightMap, heightmapScale, landmark, biomeMap, biomeIndex, biome);
            
            foreach(var spawnLocation in spawnLocations)

            for (int buildingIndex = 0; buildingIndex < landmark.numToSpawn && spawnLocations.Count > 0; buildingIndex++)
            {
                int spawnIndex = Random.Range(0, spawnLocations.Count);
                var spawnPos = FindCenterLocation(spawnLocations, mapResolution);
                // spawnLocations.RemoveAt(spawnIndex);

                SpawnBuilding(globalConfig, landmark, spawnPos.x, spawnPos.y, mapResolution, heightMap, heightmapScale, buildingRoot);
            }

        }
    }
    private Vector2Int FindCenterLocation(List<Vector2Int> locations, int mapResolution)
    {
        if(locations.Count <= 0)
            return Vector2Int.zero;

        int closestIndex = 0;
        int min = int.MaxValue;
        int centerX = mapResolution / 2;
        int centerY = mapResolution / 2;
        for(int i = 0; i < locations.Count; i++)
        {
            int distX = Mathf.Abs(centerX - locations[i].x);
            int distY = Mathf.Abs(centerY - locations[i].y);
            if(min > (distX + distY))
            {
                min = distX + distY;
                closestIndex = i;
            }
        }
        return locations[closestIndex];
    }
}
