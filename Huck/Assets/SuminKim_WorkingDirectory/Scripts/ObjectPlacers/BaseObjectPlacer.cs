using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif  // UNITY_EDITOR

[System.Serializable]
public class PlaceableObjectConfig
{
    public bool hasHeightLimits = false;
    public float minHeightToSpawn = 0f;
    public float maxHeightToSpawn = 0f;
    public bool canGoInWater = false;
    public bool canGoAboveWater = true;
    [Range(0f, 1f)] public float weighting;
    public List<GameObject> prefabs;

    public float NormalisedWeighting { get; set;} = 0f;
}

public class BaseObjectPlacer : MonoBehaviour
{
    
    [SerializeField] protected List<PlaceableObjectConfig> objects;
    [SerializeField] protected float targetDensity = 0.1f;
    [SerializeField] protected int maxSpawnCount = 1000;
    [SerializeField] protected int maxInvalidLocationSkips = 10;
    [SerializeField] protected float maxPositionJitter = 0.1f;

    protected List<Vector3> GetAllLocationForBiome(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap, int biomeIndex)
    {
        List<Vector3> locations = new List<Vector3>(mapResolution * mapResolution / 10);

        for(int y = 0; y < mapResolution; y++)
        {
            for(int x = 0; x < mapResolution; x++)
            {
                if (biomeMap[x, y] != biomeIndex)
                    continue;

                float height = heightMap[x,y] * heightmapScale.y;

                

                locations.Add(new Vector3(y * heightmapScale.z, heightMap[x, y] * heightmapScale.y, x * heightmapScale.x));
            }
        }

        return locations;
    }
    public virtual void Execute(ProcGenConfigSO globalConfig,Transform objectRoot, int mapResolution, float[,] heightMap, Vector3 heightmapScale, float[,] slopeMap, float[,,] alphaMaps, int alphaMapResolution, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        // validate the configs
        foreach(var config in objects)
        {
            if(!config.canGoInWater && !config.canGoAboveWater)
                throw new System.InvalidOperationException($"Object placer forbids both in and out of water. Cannot run!");
        }
        // normalize the weighting
        float weightSum = 0f;
        foreach(var config in objects)
        {
            weightSum += config.weighting;
        }
        foreach(var config in objects)
        {
            config.NormalisedWeighting = config.weighting / weightSum;
        }
    }

    protected virtual void ExecuteSimpleSpawning(ProcGenConfigSO globalConfig, Transform objectRoot, List<Vector3> candidateLocations)
    {
        foreach(var spawnConfig in objects)
        {
            // pick a random prefab
            var prefab = spawnConfig.prefabs[Random.Range(0, spawnConfig.prefabs.Count)];

            // determine the spawn count
            float baseSpawnCount =  Mathf.Min(maxSpawnCount, candidateLocations.Count * targetDensity);
            int numToSpawn = Mathf.FloorToInt(spawnConfig.NormalisedWeighting * baseSpawnCount);

            int skipCount = 0;
            int numPlaced = 0;
            for(int index = 0; index < maxSpawnCount; index++)
            {
                // pick a random location to spawn at
                int randomLocationIndex = Random.Range(0, candidateLocations.Count);
                Vector3 spawnLocation = candidateLocations[randomLocationIndex];

                // height is invalid?
                bool isValid = true;
                if(spawnLocation.y < globalConfig.waterHeight && !spawnConfig.canGoInWater)
                        isValid = false;
                if(spawnLocation.y >= globalConfig.waterHeight && !spawnConfig.canGoAboveWater)
                        isValid = false;

                // skip if outside of height limits
                if(spawnConfig.hasHeightLimits && (spawnLocation.y < spawnConfig.minHeightToSpawn || spawnLocation.y >= spawnConfig.maxHeightToSpawn))
                    isValid = false;

                // location is not valid?
                if(!isValid)
                {
                    skipCount++;
                    --index;

                    if(skipCount >= maxInvalidLocationSkips)
                        break;

                    continue;
                }
                skipCount = 0;
                numPlaced++;
                // remove the location if chosen
                candidateLocations.RemoveAt(randomLocationIndex);

                SpawnObject(prefab, spawnLocation, objectRoot);
            }
            Debug.Log($"Placed : {numPlaced} objects out of {numToSpawn}");
        }
    }

    protected virtual void SpawnObject(GameObject prefab, Vector3 spawnLocation, Transform objectRoot)
    {
        Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Vector3 positionOffset = new Vector3(Random.Range(  -maxPositionJitter, maxPositionJitter),
                                                            0,
                                                            Random.Range(-maxPositionJitter, maxPositionJitter));

        // instantiate the prefab
#if UNITY_EDITOR
        if (Application.isPlaying)
            Instantiate(prefab, spawnLocation + positionOffset, spawnRotation, objectRoot);
        else
        {
            var spawnedGO = PrefabUtility.InstantiatePrefab(prefab, objectRoot) as GameObject;
            spawnedGO.transform.position = spawnLocation + positionOffset;
            spawnedGO.transform.rotation = spawnRotation;
            Undo.RegisterCreatedObjectUndo(spawnedGO, "Placed object");
        }
#else
        Instantiate(prefab, spawnLocation + positionOffset, spawnRotation, objectRoot);
#endif // UNITY_EDITOR
    }
}
