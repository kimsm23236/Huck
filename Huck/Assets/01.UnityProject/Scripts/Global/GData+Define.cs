using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GData
{
    // Scene Name
    public const string SCENENAME_TITLE = "TitleScene";
    public const string SCENENAME_LOADING = "LoadingScene";
    public const string SCENENAME_PLAY = "PlayScene";

    // Layer Mask
    public const string PLAYER_MASK = "Player";
    public const string ENEMY_MASK = "Enemy";
    public const string BUILD_MASK = "Build";
    public const string GATHER_MASK = "Gather";
    public const string TERRAIN_MASK = "Terrain";
    public const string FLOOR_MASK = "Floor";
    public const string WALL_MASK = "Wall";
    public const string ANIMAL_MASK = "Animal";

    public const string POSTPROCESS_ON_LOADING = "PostProcessObject";

    // Asset Path
    public const string PREFAB_PATH = "Prefabs/";
    public const string UI_PATH = "UI/";
}

public enum EBiome
{
    BUSH,
    GRASSYPLAINS,
    RAINFOREST,
    SCRUB,
    SWAMP,
    DECAY,
    HIGHMOUNTAIN,
    MIDDLEMOUNTAIN,
    LOWMOUNTAIN
}
