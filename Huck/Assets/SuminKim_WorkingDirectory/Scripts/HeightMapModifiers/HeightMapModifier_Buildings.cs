using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR


[System.Serializable]
public class BuildingConfig
{
    public Texture2D heightMap;
    public GameObject prefab;
    public int Radius;
    public int numToSpawn = 1;
    public bool hasHeightLimits = false;
    public float minHeightToSpawn = 0f;
    public float maxHeightToSpawn = 0f;

    public bool canGoInWater = false;
    public bool canGoAboveWater = true;
}
public class HeightMapModifier_Buildings : BaseHeightMapModifier
{
    [SerializeField]
    List<BuildingConfig> Buildings;
    

    protected void SpawnBuilding(ProcGenConfigSO globalConfig, BuildingConfig building, 
                                 int spawnX, int spawnY,
                                 int mapResolution, float[,] heightMap, Vector3 heightmapScale,
                                 Transform buildingRoot)
    {
        float averageHeight = 0f;
        int numHeightSamples = 0;

        // sum the height values under the building
        for (int y = -building.Radius; y <= building.Radius; ++y)
        {
            for (int x = -building.Radius; x <= building.Radius; ++x)
            {
                // sum the heightmap values
                averageHeight += heightMap[x + spawnX, y +spawnY];
                ++numHeightSamples;
            }
        }

        // calculate the average height
        averageHeight /= numHeightSamples;

        float targetHeight = averageHeight;

        if (!building.canGoInWater)
            targetHeight = Mathf.Max(targetHeight, globalConfig.waterHeight / heightmapScale.y);
        if (building.hasHeightLimits)
            targetHeight = Mathf.Clamp(targetHeight, building.minHeightToSpawn / heightmapScale.y, building.maxHeightToSpawn / heightmapScale.y);

        // apply the building heightmap
        for (int y = -building.Radius; y <= building.Radius; ++y)
        {
            int workingY = y + spawnY;
            float textureY = Mathf.Clamp01((float)(y + building.Radius) / (building.Radius * 2f));
            for (int x = -building.Radius; x <= building.Radius; ++x)
            {
                int workingX = x + spawnX;
                float textureX = Mathf.Clamp01((float)(x + building.Radius) / (building.Radius * 2f));

                // sample the height map
                var pixelColour = building.heightMap.GetPixelBilinear(textureX, textureY);
                float strength = pixelColour.r;

                // blend based on strength
                heightMap[workingX, workingY] = Mathf.Lerp(heightMap[workingX, workingY], targetHeight, strength);
            }
        }

        // Spawn the building
        Vector3 buildingLocation = new Vector3(spawnY * heightmapScale.z, 
                                               heightMap[spawnX, spawnY] * heightmapScale.y, 
                                               spawnX * heightmapScale.x);

        // instantiate the prefab
#if UNITY_EDITOR
        if (Application.isPlaying)
            Instantiate(building.prefab, buildingLocation, Quaternion.identity, buildingRoot);
        else
        {
            var spawnedGO = PrefabUtility.InstantiatePrefab(building.prefab, buildingRoot) as GameObject;
            spawnedGO.transform.position = buildingLocation;
            Undo.RegisterCreatedObjectUndo(spawnedGO, "Add building");
        }
#else
        Instantiate(building.prefab, buildingLocation, Quaternion.identity, buildingRoot);
#endif // UNITY_EDITOR   
    }

    protected List<Vector2Int> GetSpawnLocationsForBuilding(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, BuildingConfig buildingConfig)
    {
        List<Vector2Int> locations = new List<Vector2Int>(mapResolution * mapResolution / 10);

        for (int y = buildingConfig.Radius; (y < mapResolution - buildingConfig.Radius); y += buildingConfig.Radius * 2)
        {
            for (int x = buildingConfig.Radius; (x < mapResolution - buildingConfig.Radius); x += buildingConfig.Radius * 2)
            {
                float height = heightMap[x, y] * heightmapScale.y;

                // height is invalid?
                if (height < globalConfig.waterHeight && !buildingConfig.canGoInWater)
                    continue;
                if (height >= globalConfig.waterHeight && !buildingConfig.canGoAboveWater)
                    continue;

                // skip if outside of height limits
                if (buildingConfig.hasHeightLimits && (height < buildingConfig.minHeightToSpawn || height >= buildingConfig.maxHeightToSpawn))
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
        foreach (var building in Buildings)
        {
            var spawnLocations = GetSpawnLocationsForBuilding(globalConfig, mapResolution, heightMap, heightmapScale, building);
            Debug.Log($"SpawnLoc Count : {spawnLocations.Count}");
            for (int buildingIndex = 0; buildingIndex < building.numToSpawn && spawnLocations.Count > 0; buildingIndex++)
            {
                int spawnIndex = Random.Range(0, spawnLocations.Count);
                var spawnPos = spawnLocations[spawnIndex];
                spawnLocations.RemoveAt(spawnIndex);

                SpawnBuilding(globalConfig, building, spawnPos.x, spawnPos.y, mapResolution, heightMap, heightmapScale, buildingRoot);
            }

        }

    }
}
