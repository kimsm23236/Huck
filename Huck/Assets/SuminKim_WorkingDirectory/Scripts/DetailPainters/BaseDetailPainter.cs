using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainDetailConfig : IEquatable<TerrainDetailConfig>
{
    [Header("Grass Billboard Configuration")]
    public Texture2D billboardTexture;
    public Color healthyColour = new Color(67f / 255f, 83f / 85f, 14f / 85f, 1f);
    public Color dryColour = new Color(41f / 51f, 188f / 255f, 26f / 255f, 1f);

    [Header("Detail Mesh Configuration")]
    public GameObject detailPrefab;

    [Header("Common Configuration")]
    public float minWidth = 1f;
    public float maxWidth = 2f;
    public float minHeight = 1f;
    public float maxHeight = 2f;

    public int noiseSeed = 0;
    public float noiseSpread = 0.1f;
    [Range(0f, 1f)] public float holeEdgePadding = 0f;

    public override bool Equals(object obj)
    {
        return Equals(obj as TerrainDetailConfig);
    }

    public bool Equals(TerrainDetailConfig other)
    {
        return other is not null &&
               EqualityComparer<Texture2D>.Default.Equals(billboardTexture, other.billboardTexture) &&
               healthyColour.Equals(other.healthyColour) &&
               dryColour.Equals(other.dryColour) &&
               EqualityComparer<GameObject>.Default.Equals(detailPrefab, other.detailPrefab) &&
               minWidth == other.minWidth &&
               maxWidth == other.maxWidth &&
               minHeight == other.minHeight &&
               maxHeight == other.maxHeight &&
               noiseSeed == other.noiseSeed &&
               noiseSpread == other.noiseSpread &&
               holeEdgePadding == other.holeEdgePadding;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(billboardTexture);
        hash.Add(healthyColour);
        hash.Add(dryColour);
        hash.Add(detailPrefab);
        hash.Add(minWidth);
        hash.Add(maxWidth);
        hash.Add(minHeight);
        hash.Add(maxHeight);
        hash.Add(noiseSeed);
        hash.Add(noiseSpread);
        hash.Add(holeEdgePadding);
        return hash.ToHashCode();
    }
}

public class BaseDetailPainter : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] protected float strength = 1f;

    public virtual void Execute(ProcGenManager manager, int mapResolution, float[,] heightMap, Vector3 heightmapScale, float[,] slopeMap, float[,,] alphaMaps, int alphaMapResolution, List<int[,]> detailLayerMaps, int detailMapResolution, int maxDetailsPerPatch, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        Debug.LogError("No implementation of Execute function for " + gameObject.name);
    }

    public virtual List<TerrainDetailConfig> RetrieveTerrainDetails()
    {
        return null;
    }
}
