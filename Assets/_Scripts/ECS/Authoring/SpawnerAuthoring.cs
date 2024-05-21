using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour {
    public GameObject Player1Prefab;
    public GameObject Player2Prefab;
    public float3 Player1Spawn;
    public float3 Player2Spawn;
    public float SpawnRate;
    public int SpawnMax;
}
class SpawnerBaker : Baker<SpawnerAuthoring> {
    public override void Bake(SpawnerAuthoring authoring) {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new SpawnerComponent {
            Player1Prefab = GetEntity(authoring.Player1Prefab, TransformUsageFlags.Dynamic),
            Player2Prefab = GetEntity(authoring.Player2Prefab, TransformUsageFlags.Dynamic),
            Player1SpawnPosition = authoring.Player1Spawn,
            Player2SpawnPosition = authoring.Player2Spawn,
            Player1SpawnCount = 0,
            Player2SpawnCount = 0,
            MaxSpawnCount1 = authoring.SpawnMax,
            MaxSpawnCount2 = authoring.SpawnMax,
            SpawnerLoading = SpawnerLoading.Load 
        });


    }
}