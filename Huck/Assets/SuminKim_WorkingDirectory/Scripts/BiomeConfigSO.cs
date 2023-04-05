using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����Ƽ Create�� ��ũ���ͺ� ������Ʈ�� ����� �ִ� menu�� ����
[CreateAssetMenu(fileName = "Biome Config", menuName = "Procedural Generation/Biome Configuration", order = -1)]
public class BiomeConfigSO : ScriptableObject
{
    // ���̿� �̸�
    public string Name;

    // �ּ� �ִ� �е� * �������� ���̿� ũ�� �о���
    [Range(0f, 1.0f)]
    public float MinIntensity = 0.5f;
    [Range(0f, 1.0f)]
    public float MaxIntensity = 1f;

    // �ּ� �ִ� ������ * �������� ���̿� ũ�� �۾���
    [Range(0f, 1.0f)]
    public float MaxDecayRate = 0.01f;
    [Range(0f, 1.0f)]
    public float MinDecayRate = 0.02f;

    // �� ǥ�� �÷�
    public Color mapColor = default;

    // ���� ������
    public GameObject HeightModifier;
    // �ؽ�ó���̾� ������
    public GameObject TerrainPainter;
    // ������Ʈ ��ġ��
    public GameObject ObjectPlacer;
    // ������ ����Ʈ ������
    public GameObject DetailPainter;

    public List<TextureConfig> RetrieveTextures()
    {
        if (TerrainPainter == null)
            return null;
        
        List<TextureConfig> allTextures = new List<TextureConfig>();
        BaseTexturePainter[] allPainters = TerrainPainter.GetComponents<BaseTexturePainter>();
        foreach(var painter in allPainters)
        {
            var painterTextures = painter.RetrieveTextures();

            if(painterTextures == null || painterTextures.Count == 0)
            {
                continue;
            }
            allTextures.AddRange(painterTextures);
        }

        return allTextures;
    }
    public List<TerrainDetailConfig> RetrieveTerrainDetails()
    {
        if (DetailPainter == null)
            return null;

        // extract all terrain details from every painter
        List<TerrainDetailConfig> allTerrainDetails = new List<TerrainDetailConfig>();
        BaseDetailPainter[] allPainters = DetailPainter.GetComponents<BaseDetailPainter>();
        foreach (var painter in allPainters)
        {
            var terrainDetails = painter.RetrieveTerrainDetails();

            if (terrainDetails == null || terrainDetails.Count == 0)
                continue;

            allTerrainDetails.AddRange(terrainDetails);
        }

        return allTerrainDetails;
    }

}
