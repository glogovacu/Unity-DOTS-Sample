using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnerComponent : IComponentData{
    public Entity Player1Prefab;
    public Entity Player2Prefab;
    public float3 Player1SpawnPosition;
    public float3 Player2SpawnPosition;
    public int Player1SpawnCount;
    public int Player2SpawnCount;
    public int MaxSpawnCount1;
    public int MaxSpawnCount2;
    public SpawnerLoading SpawnerLoading;
}

public enum SpawnerLoading {
    Load,
    Unload
}
