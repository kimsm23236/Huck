using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.XR;
using Unity.VisualScripting.FullSerializer;


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif  // UNITY_EDITOR

public class ProcGenManager : MonoBehaviour
{
    [SerializeField] ProcGenConfigSO config;
    [SerializeField] Terrain targetTerrain;
    [Header("Debugging")]
    [SerializeField] bool DEBUG_TurnOffObjectPlacers = false;
    [SerializeField] bool DEBUG_TurnOffBakeNavMeshes = false;
    [SerializeField] NavMeshSurface[] navMeshSurfaces = default;
    Dictionary<TextureConfig, int> BiomeTextureToTerrainLayerIndex = new Dictionary<TextureConfig, int>();
    Dictionary<TerrainDetailConfig, int> BiomeTerrainDetailToDetailLayerIndex = new Dictionary<TerrainDetailConfig, int>();

    byte[,] BiomeMap;
    float[,] BiomeStrengths;

    float[,] SlopeMap;

    void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // navMeshSurface = GFunc.GetRootObj("NavMesh").GetComponentMust<NavMeshSurface>();
#if UNITY_EDITOR
        if(Application.isPlaying)
            StartCoroutine(AsyncRegenerateWorld(LoadingManager.Instance.OnStatusReported));
        else
        {
            /* To do */
        }
            
#else
        StartCoroutine(AsyncRegenerateWorld(LoadingManager.Instance.OnStatusReported));
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator AsyncRegenerateWorld(System.Action<EGenerationStage, string> reportStatusFn = null)
    {
        // cache the map resolution
        int mapResolution = targetTerrain.terrainData.heightmapResolution;
        int alphaMapResolution = targetTerrain.terrainData.alphamapResolution;
        int detailMapResolution = targetTerrain.terrainData.detailResolution;
        int maxDetailsPerPatch = targetTerrain.terrainData.detailResolutionPerPatch;

        if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.Beginning, "Beginning Terrain Generation");
        yield return new WaitForSeconds(1f);

        // clear out any previously spawn objects
        for (int childIndex = transform.childCount - 1; childIndex >= 0; --childIndex)
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
                Destroy(transform.GetChild(childIndex).gameObject);
            else
                Undo.DestroyObjectImmediate(transform.GetChild(childIndex).gameObject);
#else
            Destroy(transform.GetChild(childIndex).gameObject);
#endif  // UNITY_EDITOR
        }

        if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.BuildTextureMap, "Building texture map");
        yield return new WaitForSeconds(1f);

        // Generate the texture mapping
        Perform_GenerateTextureMapping();

        if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.BuildDetailMap, "Building detail map");
        yield return new WaitForSeconds(1f);

        // Generate the terrain detail mapping
        Perform_GenerateTerrainDetailMapping();

        while(true)
        {
            if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.BuildBiomeMap, "Build biome map");
            yield return new WaitForSeconds(1f);

            // 바이옴 맵 생성
            Perform_BiomeGeneration(mapResolution);

            if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.HeightMapGeneration, "Modifying heights");
            yield return new WaitForSeconds(1f);

            // 지형 높이맵 설정
            Perform_HeightMapModification(mapResolution, alphaMapResolution);

            // Height Modifier로 높이맵과 보스 성을 만드는데 여기서 보스성을 찾을 수 있다면 루프 탈출
            if(IsValidHeightModifying())
                break;
            else
            {
                if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.HeightMapGeneration, "Validation Failed Return Build biome map");
                yield return new WaitForSeconds(1f);
            }
        }

        if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.SetupWorldMap, "Setup world map"); 
                yield return new WaitForSeconds(1f);
        Perform_SetupWorldMap(config, mapResolution);

        if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.TerrainPainting, "Painting the terrain");
        yield return new WaitForSeconds(1f);

        // 지형 칠하기 단계
        Perform_TerrainPainting(mapResolution, alphaMapResolution);

        if (reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.NavMeshBaking, "NavMesh Baking");
        yield return new WaitForSeconds(1f);

        // 네비메쉬 굽기 단계
        Perform_NavMeshBaking();

        if (reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.ObjectPlacement, "Placing objects");
        yield return new WaitForSeconds(1f);

        // 오브젝트 배치 단계
        Perform_ObjectPlacement(mapResolution, alphaMapResolution);

        if(reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.DetailPainting, "Detail Painting");
        yield return new WaitForSeconds(1f);

        // 디테일(잔디, 풀 등) 칠하기
        Perform_DetailPainting(mapResolution, alphaMapResolution, detailMapResolution, maxDetailsPerPatch);

        if (reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.PostProcessOnLoading, "PostProcessOnLoading");
        yield return new WaitForSeconds(1f);

        // 로딩 후처리 작업 단계
        Perform_PostProcessOnLoading();

        if (reportStatusFn != null) reportStatusFn.Invoke(EGenerationStage.Complete, "Terrain Generation complete");
        yield return new WaitForSeconds(1f);

        //
        // targetTerrain.terrainData.deta
        //
    }

    void Perform_GenerateTextureMapping()
    {
        BiomeTextureToTerrainLayerIndex.Clear();

        // build up list of all textures
        List<TextureConfig> allTextures = new List<TextureConfig>();
        foreach (var biomeMetadata in config.Biomes)
        {
            List<TextureConfig> biomeTextures = biomeMetadata.Biome.RetrieveTextures();

            if (biomeTextures == null || biomeTextures.Count == 0)
                continue;

            allTextures.AddRange(biomeTextures);
        }

        if (config.PaintingPostProcessingModifier != null)
        {
            // extract all textures from every painter
            BaseTexturePainter[] allPainters = config.PaintingPostProcessingModifier.GetComponents<BaseTexturePainter>();
            foreach (var painter in allPainters)
            {
                var painterTextures = painter.RetrieveTextures();

                if (painterTextures == null || painterTextures.Count == 0)
                    continue;

                allTextures.AddRange(painterTextures);
            }
        }

        // filter out any duplicate entries
        allTextures = allTextures.Distinct().ToList();

        // iterate over the texture configs
        int layerIndex = 0;
        foreach(var textureConfig in allTextures)
        {
            BiomeTextureToTerrainLayerIndex[textureConfig] = layerIndex;
            layerIndex++;
        }
    }

    void Perform_GenerateTerrainDetailMapping()
    {
        BiomeTerrainDetailToDetailLayerIndex.Clear();

        // build up list of all terrain details
        List<TerrainDetailConfig> allTerrainDetails = new List<TerrainDetailConfig>();
        foreach (var biomeMetadata in config.Biomes)
        {
            List<TerrainDetailConfig> biomeTerrainDetails = biomeMetadata.Biome.RetrieveTerrainDetails();

            if (biomeTerrainDetails == null || biomeTerrainDetails.Count == 0)
                continue;

            allTerrainDetails.AddRange(biomeTerrainDetails);
        }

        if (config.DetailPaintingPostProcessingModifier != null)
        {
            // extract all terrain details from every painter
            BaseDetailPainter[] allPainters = config.DetailPaintingPostProcessingModifier.GetComponents<BaseDetailPainter>();
            foreach (var painter in allPainters)
            {
                var terrainDetails = painter.RetrieveTerrainDetails();

                if (terrainDetails == null || terrainDetails.Count == 0)
                    continue;

                allTerrainDetails.AddRange(terrainDetails);
            }
        }

        // filter out any duplicate entries
        allTerrainDetails = allTerrainDetails.Distinct().ToList();

        // iterate over the terrain detail configs
        int layerIndex = 0;
        foreach (var terrainDetail in allTerrainDetails)
        {
            BiomeTerrainDetailToDetailLayerIndex[terrainDetail] = layerIndex;
            ++layerIndex;
        }
    }
#if UNITY_EDITOR
    public void RegenerateTexture()
    {
        Perform_LayerSetup();
    }

    public void RegenerateDetailPrototypes()
    {
        Perform_DetailPrototypeSetup();
    }

    void Perform_LayerSetup()
    {
        // delete any exisiting layers
        if (targetTerrain.terrainData.terrainLayers != null || targetTerrain.terrainData.terrainLayers.Length > 0)
        {
            Undo.RecordObject(targetTerrain, "Clearing previous layers");

            // build up  list of asset path for each layer
            List<string> layersToDelete = new List<string>();
            foreach(var layer in targetTerrain.terrainData.terrainLayers)
            {
                if (layer == null)
                    continue;

                layersToDelete.Add(AssetDatabase.GetAssetPath(layer.GetInstanceID()));
            }

            // remove all links to layers
            targetTerrain.terrainData.terrainLayers = null;
            
            // delete each layer
            foreach(var layerFile in layersToDelete)
            {
                if (string.IsNullOrEmpty(layerFile))
                    continue;

                AssetDatabase.DeleteAsset(layerFile);
            }

            Undo.FlushUndoRecordObjects();
        }

        string scenePath = System.IO.Path.GetDirectoryName(SceneManager.GetActiveScene().path);

        Perform_GenerateTextureMapping();



        // generate all of the layers
        int numLayers = BiomeTextureToTerrainLayerIndex.Count;
        List<TerrainLayer> newLayers = new List<TerrainLayer>(numLayers);

        // preallocate the layers
        for(int layerIndex = 0; layerIndex < numLayers; layerIndex++)
        {
            newLayers.Add(new TerrainLayer());
        }

        // iterate over the texture map
        foreach(var textureMappingEntry in BiomeTextureToTerrainLayerIndex)
        {
            var textureConfig = textureMappingEntry.Key;
            var textureLayerIndex = textureMappingEntry.Value;
            var textureLayer = newLayers[textureLayerIndex];

            // configure the terrain layer textures
            textureLayer.diffuseTexture = textureConfig.Diffuse;
            textureLayer.normalMapTexture = textureConfig.NormalMap;

            // save as asset
            string layerPath = System.IO.Path.Combine(scenePath, "Layer_" + textureLayerIndex);
            AssetDatabase.CreateAsset(textureLayer, $"{layerPath}.asset");
        }

        Undo.RecordObject(targetTerrain.terrainData, "Updating terrain layers");
        targetTerrain.terrainData.terrainLayers = newLayers.ToArray();
    }

    void Perform_DetailPrototypeSetup()
    {
        Perform_GenerateTerrainDetailMapping();

        // build the list of detail prototypes
        var detailPrototypes = new DetailPrototype[BiomeTerrainDetailToDetailLayerIndex.Count];
        foreach(var kvp in BiomeTerrainDetailToDetailLayerIndex)
        {
            TerrainDetailConfig detailData = kvp.Key;
            int layerIndex = kvp.Value;

            DetailPrototype newDetail = new DetailPrototype();

            // is this a mesh?
            if (detailData.detailPrefab)
            {
                newDetail.prototype = detailData.detailPrefab;
                newDetail.renderMode = DetailRenderMode.VertexLit;
                newDetail.usePrototypeMesh = true;
                newDetail.useInstancing = true;
            }
            else
            {
                newDetail.prototypeTexture = detailData.billboardTexture;
                newDetail.renderMode = DetailRenderMode.GrassBillboard;
                newDetail.usePrototypeMesh = false;
                newDetail.useInstancing = false;
                newDetail.healthyColor = detailData.healthyColour;
                newDetail.dryColor = detailData.dryColour;
            }

            // transfer the common data
            newDetail.minWidth = detailData.minWidth;
            newDetail.maxWidth = detailData.maxWidth;
            newDetail.minHeight = detailData.minHeight;
            newDetail.maxHeight = detailData.maxHeight;
            newDetail.noiseSeed = detailData.noiseSeed;
            newDetail.noiseSpread = detailData.noiseSpread;
            newDetail.holeEdgePadding = detailData.holeEdgePadding;

            // check the prototype
            string errorMessage;
            if (!newDetail.Validate(out errorMessage))
            {
                throw new System.InvalidOperationException(errorMessage);
            }

            detailPrototypes[layerIndex] = newDetail;
        }

        // update the detail prototypes
        Undo.RecordObject(targetTerrain.terrainData, "Updating Detail Prototypes");
        targetTerrain.terrainData.detailPrototypes = detailPrototypes;
        targetTerrain.terrainData.RefreshPrototypes();
    }

#endif  // UNITY_EDITOR

    void Perform_BiomeGeneration(int mapResolution)
    {
        // allocate the biome map and strength map
        BiomeMap = new byte[mapResolution, mapResolution];
        BiomeStrengths = new float[mapResolution, mapResolution];

        // execute any initial height modifiers
        if(config.BiomeGenerators != null)
        {
            BaseBiomeMapGenerator[] generators = config.BiomeGenerators.GetComponents<BaseBiomeMapGenerator>();

            foreach(var generator in generators)
            {
                generator.Execute(config, mapResolution, BiomeMap, BiomeStrengths);
            }
        }
    }
    void Perform_HeightMapModification(int mapResolution, int alphaMapResolution)
    {
        float[,] heightMap = targetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);

        // execute any initial height modifiers
        if(config.InitialHeightModifier != null)
        {
            BaseHeightMapModifier[] modifiers = config.InitialHeightModifier.GetComponents<BaseHeightMapModifier>();

            foreach(var modifier in modifiers)
            {
                modifier.Execute(config, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale);
            }
        }

        // run heightmap generation for each biome
        for (int biomeIndex = 0; biomeIndex < config.NumBiomes; biomeIndex++)
        {
            var biome = config.Biomes[biomeIndex].Biome;

            if (biome.HeightModifier == null || biome.HeightModifier == default)
                continue;


            BaseHeightMapModifier[] modifiers = biome.HeightModifier.GetComponents<BaseHeightMapModifier>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(config, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, BiomeMap, biomeIndex, biome);
            }
        }

        // execute any post processing height modifiers
        if (config.HeightPostProcessingModifier != null)
        {
            BaseHeightMapModifier[] modifiers = config.HeightPostProcessingModifier.GetComponents<BaseHeightMapModifier>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(config, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, BiomeMap);
            }
        }

        targetTerrain.terrainData.SetHeights(0, 0, heightMap);

        // generate the slope map
        SlopeMap = new float[alphaMapResolution, alphaMapResolution];
        for (int y = 0; y < alphaMapResolution; y++)
        {
            for (int x = 0; x < alphaMapResolution; x++)
            {
                SlopeMap[x, y] = targetTerrain.terrainData.GetInterpolatedNormal((float)x / alphaMapResolution, (float)y / alphaMapResolution).y;
            }
        }
    }

    public int GetLayerForTexture(TextureConfig textureConfig)
    {
        return BiomeTextureToTerrainLayerIndex[textureConfig];
    }
    public int GetDetailLayerForTerrainDetail(TerrainDetailConfig detailConfig)
    {
        return BiomeTerrainDetailToDetailLayerIndex[detailConfig];
    }

    void Perform_TerrainPainting(int mapResolution, int alphaMapResolution)
    {
        float[,] heightMap = targetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);
        float[,,] alphaMaps =  targetTerrain.terrainData.GetAlphamaps(0, 0, alphaMapResolution, alphaMapResolution);


        // zero out all layers
        for (int y = 0; y < alphaMapResolution; y++)
        {
            for (int x = 0; x < alphaMapResolution; x++)
            {
                for (int layerIndex = 0; layerIndex < targetTerrain.terrainData.alphamapLayers; layerIndex++)
                {
                    alphaMaps[x, y, layerIndex] = 0;
                }
            }
        }

        // run terrain painting for each biome
        for (int biomeIndex = 0; biomeIndex < config.NumBiomes; biomeIndex++)
        {
            var biome = config.Biomes[biomeIndex].Biome;

            if (biome.TerrainPainter == null || biome.TerrainPainter == default)
                continue;


            BaseTexturePainter[] modifiers = biome.TerrainPainter.GetComponents<BaseTexturePainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(this, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, SlopeMap, alphaMaps, alphaMapResolution,  BiomeMap, biomeIndex, biome);
            }
        }

        // run texture post processing 
        if(config.PaintingPostProcessingModifier != null)
        {
            BaseTexturePainter[] modifiers = config.PaintingPostProcessingModifier.GetComponents<BaseTexturePainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(this, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, SlopeMap, alphaMaps, alphaMapResolution);
            }
        }
        



        targetTerrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
    } // Perform_TerrainPainting

    void Perform_ObjectPlacement(int mapResolution, int alphaMapResolution)
    {
        if(DEBUG_TurnOffObjectPlacers)
            return;

        float[,] heightMap = targetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);
        float[,,] alphaMaps = targetTerrain.terrainData.GetAlphamaps(0, 0, alphaMapResolution, alphaMapResolution);


        // run object placement for each biome
        for (int biomeIndex = 0; biomeIndex < config.NumBiomes; biomeIndex++)
        {
            var biome = config.Biomes[biomeIndex].Biome;

            if (biome.ObjectPlacer == null || biome.ObjectPlacer == default)
                continue;

            BaseObjectPlacer[] modifiers = biome.ObjectPlacer.GetComponents<BaseObjectPlacer>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(config, transform, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, SlopeMap, alphaMaps, alphaMapResolution, BiomeMap, biomeIndex, biome);
            }
        }
    }

    void Perform_NavMeshBaking()
    {
        if(DEBUG_TurnOffBakeNavMeshes)
            return;
            
        Debug.Log("NavMesh Bake");
        
        for(int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }

    void Perform_DetailPainting(int mapResolution, int alphaMapResolution, int detailMapResolution, int maxDetailsPerPatch)
    {
        float[,] heightMap = targetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);
        float[,,] alphaMaps = targetTerrain.terrainData.GetAlphamaps(0, 0, alphaMapResolution, alphaMapResolution);

        // create a new empty set of layers
        int numDetailLayers = targetTerrain.terrainData.detailPrototypes.Length;
        List<int[,]> detailLayerMaps = new List<int[,]>(numDetailLayers);
        for (int layerIndex = 0; layerIndex < numDetailLayers; ++layerIndex)
        {
            detailLayerMaps.Add(new int[detailMapResolution, detailMapResolution]);
        }

        // run terrain detail painting for each biome
        for (int biomeIndex = 0; biomeIndex < config.NumBiomes; ++biomeIndex)
        {
            var biome = config.Biomes[biomeIndex].Biome;
            if (biome.DetailPainter == null)
                continue;

            BaseDetailPainter[] modifiers = biome.DetailPainter.GetComponents<BaseDetailPainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(this, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, SlopeMap, alphaMaps, alphaMapResolution, detailLayerMaps, detailMapResolution, maxDetailsPerPatch, BiomeMap, biomeIndex, biome);
            }
        }

        // run detail painting post processing
        if (config.DetailPaintingPostProcessingModifier != null)
        {
            BaseDetailPainter[] modifiers = config.DetailPaintingPostProcessingModifier.GetComponents<BaseDetailPainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(this, mapResolution, heightMap, targetTerrain.terrainData.heightmapScale, SlopeMap, alphaMaps, alphaMapResolution, detailLayerMaps, detailMapResolution, maxDetailsPerPatch);
            }
        }

        // apply the detail layers
        for (int layerIndex = 0; layerIndex < numDetailLayers; ++layerIndex)
        {
            targetTerrain.terrainData.SetDetailLayer(0, 0, layerIndex, detailLayerMaps[layerIndex]);
        }
    }

    void Perform_PostProcessOnLoading()
    {
        GameObject[] PostPrcObjects = GameObject.FindGameObjectsWithTag(GData.POSTPROCESS_ON_LOADING);
        Debug.Log($"PostPrcObjects Count : {PostPrcObjects.Length}");
        List<PostProcessOnLoading> postProcessOnLoadings = new List<PostProcessOnLoading>();
        foreach (var postPrcObj in PostPrcObjects)
        {
            PostProcessOnLoading ppol = postPrcObj.GetComponent<PostProcessOnLoading>();
            if (ppol == null)
                continue;

            postProcessOnLoadings.Add(ppol);
        }

        foreach(var postProcessor in postProcessOnLoadings)
        {
            if(postProcessor != null)
            {
                postProcessor.Execute();
            }
        }
        
    }

    void Perform_SetupWorldMap(ProcGenConfigSO config, int mapResolution)
    {
        float[,] heightMap = targetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);
        Vector3 heightMapScale = targetTerrain.terrainData.heightmapScale;
        Color heightColor_HighMountain = Color.white;
        Color heightColor_Water = Color.cyan;

        // save out the biome map
        Texture2D biomeMapTexture = new Texture2D(mapResolution, mapResolution, TextureFormat.RGB24, false);
        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                Color workingColor = config.Biomes[(int)BiomeMap[x, y]].Biome.mapColor;
                if(heightMap[x,y] * heightMapScale.y >= 80f)
                    workingColor = heightColor_HighMountain;
                else if(heightMap[x,y] * heightMapScale.y <= 15f)
                    workingColor = heightColor_Water;

                biomeMapTexture.SetPixel(x, y, workingColor);
            }
        }
        biomeMapTexture.Apply();

        UIManager.Instance.worldMapTexture = biomeMapTexture;
#if UNITY_EDITOR
        System.IO.File.WriteAllBytes("BiomeMap_WorldMap.png", biomeMapTexture.EncodeToPNG());
#endif  // UNITY_EDITOR
    }

    bool IsValidHeightModifying()
    {
        bool bIsSuccessed = true;
#if UNITY_EDITOR
        if(Application.isPlaying)
        {
            GameObject bossCastle = gameObject.FindChildObj("BossCastle(Clone)");
            if(bossCastle == default)
            {
                Debug.Log("Can't found Boss Castle");
                bIsSuccessed = false;
            }
            else
                Debug.Log("find Boss Castle");
        }
        else
        {
            /* To do? */
        } 
#else
        GameObject bossCastle = gameObject.FindChildObj("BossCastle(Clone)");
        if(bossCastle == default)
        {
            Debug.Log("Can't found Boss Castle");
            bIsSuccessed = false;
        }
        else
            Debug.Log("find Boss Castle");
#endif 
        return bIsSuccessed;
    }

}
