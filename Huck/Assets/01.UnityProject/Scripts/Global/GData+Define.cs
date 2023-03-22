using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GData
{
    public const string PLAYER_MASK = "Player";
    public const string ENEMY_MASK = "Enemy";
    public const string BUILD_MASK = "build";
    public const string GATHER_MASK = "Gather";
    public const string TERRAIN_MASK = "Terrain";
}

//! 지형의 속성을 정의하기 위한 타입
public enum TerrainType
{
    NONE = -1, 
    PLAIN_PASS,
    OCEAN_N_PASS
}       // PuzzleType
