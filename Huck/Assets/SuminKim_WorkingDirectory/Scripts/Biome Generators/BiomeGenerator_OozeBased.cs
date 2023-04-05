using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator_OozeBased : BaseBiomeMapGenerator
{
    public enum EBiomeMapBaseResolution
    {
        Size_64x64      = 64,
        Size_128x128    = 128,
        Size_256x256    = 256,
        Size_512x512    = 512,
        Size_1024x1024 = 1024,
    }

    // 시드 포인트 밀도
    [Range(0f, 1.0f)]
    public float BiomeSeedPointDensity = 0.1f;

    public EBiomeMapBaseResolution biomeMapResolution = EBiomeMapBaseResolution.Size_64x64;
    byte[,] BiomeMap_LowResolution;
    float[,] BiomeStrengths_LowResolution;

    public override void Execute(ProcGenConfigSO globalConfig, int mapResolution, byte[,] biomeMap, float[,] biomeStrengths)
    {
        Perform_BiomeGeneration_LowResolution(globalConfig, (int)biomeMapResolution);

        Perform_BiomeGeneration_HighResolution(globalConfig, (int)biomeMapResolution, mapResolution, biomeMap, biomeStrengths);
    }
    void Perform_BiomeGeneration_LowResolution(ProcGenConfigSO config, int mapResolution)
    {
        // 바이옴 맵, 강도 맵 메모리 할당
        BiomeMap_LowResolution = new byte[mapResolution, mapResolution];
        BiomeStrengths_LowResolution = new float[mapResolution, mapResolution];

        // 시드 포인트 갯수 설정
        int numSeedPoints = Mathf.FloorToInt(mapResolution * mapResolution * BiomeSeedPointDensity);
        List<byte> biomeToSpawn = new List<byte>(numSeedPoints);

        // 가중치를 바탕으로 바이옴 채우기
        float totalBiomeWeighting = config.TotalWeighting;
        for(int biomeIndex = 0; biomeIndex < config.NumBiomes; biomeIndex++)
        {
            // 바이옴 별 가중치에 따라 시드포인트 나누기
            int numEntries = Mathf.RoundToInt(numSeedPoints * config.Biomes[biomeIndex].Weighting / totalBiomeWeighting);

            for(int entryIndex = 0; entryIndex < numEntries; ++entryIndex)
            {
                // 계산된 바이옴 시드포인트의 갯수만큼 Add
                biomeToSpawn.Add((byte)biomeIndex);
            }
        }

        // 각각의 바이옴 스폰
        while(biomeToSpawn.Count > 0)
        {
            // 시드 포인트 랜덤으로 뽑기
            int seedPointIndex = Random.Range(0, biomeToSpawn.Count);

            // 뽑은 바이옴의 인덱스 변수
            byte biomeIndex = biomeToSpawn[seedPointIndex];

            // 리스트에서 제거
            biomeToSpawn.RemoveAt(seedPointIndex);

            // 바이옴 스폰
            Perform_SpawnIndividualBiome(config, biomeIndex, mapResolution);
        }
#if UNITY_EDITOR
        // 바이옴 맵 저장
        Texture2D biomeMapTexture = new Texture2D(mapResolution, mapResolution, TextureFormat.RGB24, false);
        for(int y = 0; y < mapResolution; y++)
        {
            for(int x = 0; x < mapResolution; x++)
            {
                float hue = ((float)BiomeMap_LowResolution[x, y] / (float)config.NumBiomes);
                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }
        biomeMapTexture.Apply();

        System.IO.File.WriteAllBytes("BiomeMap_LowResolution.png", biomeMapTexture.EncodeToPNG());
#endif  // UNITY_EDITOR
    }

    // 고해상도 바이옴 생성
    void Perform_BiomeGeneration_HighResolution(ProcGenConfigSO config, int lowResMapSize, int highResMapSize, byte[,] biomeMap, float[,] biomeStrengths)
    {
        // 맵 스케일 계산
        float mapScale = (float)lowResMapSize / highResMapSize;

        // calculate the high res map
        for (int y = 0; y < highResMapSize; y++)
        {
            int lowResY = Mathf.FloorToInt(y * mapScale);
            float yFraction = y * mapScale - lowResY;

            for (int x = 0; x < highResMapSize; x++)
            {
                int lowResX = Mathf.FloorToInt(x * mapScale);
                float xFraction = x * mapScale - lowResX;

                biomeMap[x, y] = CalculateHighResBiomeIndex(lowResMapSize, lowResX, lowResY, xFraction, yFraction);

                // BiomeMap[x, y] = BiomeMap_LowResolution[lowResX, lowResY];
            }
        }
#if UNITY_EDITOR
        // save out the biome map
        Texture2D biomeMapTexture = new Texture2D(highResMapSize, highResMapSize, TextureFormat.RGB24, false);
        for (int y = 0; y < highResMapSize; y++)
        {
            for (int x = 0; x < highResMapSize; x++)
            {
                float hue = ((float)biomeMap[x, y] / (float)config.NumBiomes);
                //Color color = config.Biomes[(int)biomeMap[x, y]].Biome.mapColor;
                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }
        biomeMapTexture.Apply();

        System.IO.File.WriteAllBytes("BiomeMap_HighResolution.png", biomeMapTexture.EncodeToPNG());
#endif  // UNITY_EDITOR
    }

    // 저해상도 바이옴 맵을 고해상도 바이옴 맵으로 스케일링
    byte CalculateHighResBiomeIndex(int lowResMapSize, int lowResX, int lowResY, float fractionX, float fractionY)
    {
        float A = BiomeMap_LowResolution[lowResX,     lowResY];
        float B = (lowResX + 1) < lowResMapSize ? BiomeMap_LowResolution[lowResX + 1, lowResY] : A;
        float C = (lowResY + 1) < lowResMapSize ? BiomeMap_LowResolution[lowResX,     lowResY + 1] : A;
        float D = 0;

        if ((lowResX + 1) >= lowResMapSize)
            D = C;
        else if ((lowResY + 1) >= lowResMapSize)
            D = B;
        else
            D = BiomeMap_LowResolution[lowResX + 1, lowResY + 1];

        // 이중 선형 보간 수행
        float filteredindex =   A * (1 - fractionX) * (1 - fractionY) + B * fractionX * (1 - fractionY) * 
                        C * fractionY * (1 - fractionX) + D * fractionX * fractionY;

        // 보간에 사용된 값을 기반으로 가능한 바이옴 배열을 생성
        float[] candidateBiome = new float[] { A, B, C, D };

        // 보간된 바이옴에 가장 가까운 이웃 바이옴 탐색
        float bestBiome = -1f;
        float bestDelta = float.MaxValue;

        for(int biomeIndex = 0; biomeIndex < candidateBiome.Length; biomeIndex++)
        {
            float delta = Mathf.Abs(filteredindex - candidateBiome[biomeIndex]);

            if(delta < bestDelta)
            {
                bestDelta = delta;
                bestBiome = candidateBiome[biomeIndex];
            }
        }

        return (byte)Mathf.RoundToInt(bestBiome);
    }
    
    // 8방향 탐색을 위한 방향배열
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
    

    void Perform_SpawnIndividualBiome(ProcGenConfigSO config, byte biomeIndex, int mapResolution)
    {
        // 바이옴 설정 캐싱
        BiomeConfigSO biomeConfig = config.Biomes[biomeIndex].Biome;

        // 맵 해상도 범위 내에서 랜덤으로 스폰 위치 결정
        Vector2Int spawnLocation = new Vector2Int(Random.Range(0, mapResolution), Random.Range(0, mapResolution));

        // 바이옴 설정에따라 시작 강도를 범위 내 랜덤으로 지정
        float startIntensity = Random.Range(biomeConfig.MinIntensity, biomeConfig.MaxIntensity);

        // 워킹리스트 설정
        Queue<Vector2Int> workingList = new Queue<Vector2Int>();
        workingList.Enqueue(spawnLocation);

        // 방문맵, 목표강도 맵 설정
        bool[,] visited = new bool[mapResolution, mapResolution];
        float[,] targetIntensity = new float[mapResolution, mapResolution];

        // 시작 강도 설정
        targetIntensity[spawnLocation.x, spawnLocation.y] = startIntensity;

        // Ooze 시작
        while(workingList.Count > 0) 
        {
            Vector2Int workingLocation = workingList.Dequeue();

            // 바이옴 맵에 바이옴 설정
            BiomeMap_LowResolution[workingLocation.x, workingLocation.y] = biomeIndex;
            visited[workingLocation.x, workingLocation.y] = true;
            // 강세 맵에 강세 설정
            BiomeStrengths_LowResolution[workingLocation.x, workingLocation.y] = targetIntensity[workingLocation.x, workingLocation.y];

            // 8방향 탐색 
            for(int neighbourIndex = 0; neighbourIndex < NeighbourOffsets.Length; neighbourIndex++)
            {
                Vector2Int neighbourLocation = workingLocation + NeighbourOffsets[neighbourIndex];

                // 범위 외 위치
                if (neighbourLocation.x < 0 || neighbourLocation.y < 0 || neighbourLocation.x >= mapResolution || neighbourLocation.y >= mapResolution)
                    continue;

                // 이미 방문한 위치
                if (visited[neighbourLocation.x, neighbourLocation.y])
                    continue;

                // 방문 표시
                visited[neighbourLocation.x, neighbourLocation.y] = true;

                // 감쇠치 계산
                float decayAmount = Random.Range(biomeConfig.MinDecayRate, biomeConfig.MaxDecayRate) * NeighbourOffsets[neighbourIndex].magnitude;
                // 탐색 위치 강세 계산
                float neighbourStrength = targetIntensity[workingLocation.x, workingLocation.y] - decayAmount;
                // 강세 맵에 대입
                targetIntensity[neighbourLocation.x, neighbourLocation.y] = neighbourStrength;

                // 강세가 0이하일 경우 퍼지는것을 멈춤
                if (neighbourStrength <= 0)
                {
                    continue;
                }
                // 워킹 리스트에 푸쉬
                workingList.Enqueue(neighbourLocation);
            }
        }
    }   // Perform_SpawnIndividualBiome()

}
