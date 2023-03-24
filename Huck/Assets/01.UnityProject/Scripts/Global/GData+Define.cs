using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GData
{
    public const string PLAYER_MASK = "Player";
    public const string ENEMY_MASK = "Enemy";
    public const string BUILD_MASK = "Build";
    public const string GATHER_MASK = "Gather";
    public const string TERRAIN_MASK = "Terrain";
}

//! ������ �Ӽ��� �����ϱ� ���� Ÿ��
public enum TerrainType
{
    NONE = -1,
    PLAIN_PASS,
    OCEAN_N_PASS
}       // PuzzleType
