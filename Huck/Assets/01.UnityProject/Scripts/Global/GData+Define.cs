using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GData
{
    // Scene Name
    public const string SCENENAME_TITLE = "b_SampleTitleSample";
    public const string SCENENAME_LOADING = "b_SampleLoadingScene";
    public const string SCENENAME_PLAY = "b_SampleTestScene";

    // Layer Mask
    public const string PLAYER_MASK = "Player";
    public const string ENEMY_MASK = "Enemy";
    public const string BUILD_MASK = "Build";
    public const string GATHER_MASK = "Gather";
    public const string TERRAIN_MASK = "Terrain";
    public const string FLOOR_MASK = "Floor";

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
