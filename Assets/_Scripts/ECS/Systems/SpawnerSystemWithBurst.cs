using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

[BurstCompile]
public partial struct SpawnerSystemWithBurst : ISystem {
    private EntityQuery _playerQuery;
    private FixedString64Bytes _sceneString;
    private FixedString64Bytes _ecsSceneString;
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SpawnerComponent>();
        _playerQuery = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        _sceneString = new FixedString64Bytes(SceneManager.GetActiveScene().name);
        _ecsSceneString = new FixedString64Bytes("ECSOnlyScene");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if(_sceneString == _ecsSceneString) {
            var spawner = SystemAPI.GetSingletonRW<SpawnerComponent>();
            var spawnerEntity = SystemAPI.GetSingletonEntity<SpawnerComponent>();
            var randomAspect = SystemAPI.GetAspect<RandomAspect>(spawnerEntity);
            if (spawner.ValueRO.SpawnerLoading == SpawnerLoading.Unload) {
                return;
            }
            // Spawn Player1 entities if needed
            while (spawner.ValueRO.Player1SpawnCount < spawner.ValueRO.MaxSpawnCount1 / 2) {
                Entity newEntity1 = state.EntityManager.Instantiate(spawner.ValueRO.Player1Prefab);
                state.EntityManager.SetComponentData(newEntity1, LocalTransform.FromPosition(randomAspect.GetRandomSpawnPosition(spawner.ValueRO.Player1SpawnPosition)));
                spawner.ValueRW.Player1SpawnCount++;
            }

            // Spawn Player2 entities if needed
            while (spawner.ValueRO.Player2SpawnCount < spawner.ValueRO.MaxSpawnCount1 / 2) {
                Entity newEntity2 = state.EntityManager.Instantiate(spawner.ValueRO.Player2Prefab);
                state.EntityManager.SetComponentData(newEntity2, LocalTransform.FromPosition(randomAspect.GetRandomSpawnPosition(spawner.ValueRO.Player2SpawnPosition)));
                spawner.ValueRW.Player2SpawnCount++;
            }
        } else {
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

            if (SystemAPI.GetSingleton<SpawnerComponent>().SpawnerLoading == SpawnerLoading.Unload) {
                new DeletePlayerJob {
                    Ecb = ecb,
                }.ScheduleParallel();

                return;
            }

            new ProcessSpawner1Job {
                Ecb = ecb
            }.ScheduleParallel();

            new ProcessSpawner2Job {
                Ecb = ecb
            }.ScheduleParallel();
        }
        
    }
    [BurstCompile]
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        return ecb.AsParallelWriter();
    }
}
[BurstCompile]
public partial struct DeletePlayerJob : IJobEntity {
    public EntityCommandBuffer.ParallelWriter Ecb;
    public void Execute(PlayerComponent player, [ChunkIndexInQuery] int chunkIndex, Entity entity) {
        // Using the chunkIndex as the sort key for determinism
        Ecb.DestroyEntity(chunkIndex, entity);
    }
}
[BurstCompile]
public partial struct ProcessSpawner1Job : IJobEntity {
    public EntityCommandBuffer.ParallelWriter Ecb;
    private void Execute([ChunkIndexInQuery] int chunkIndex, ref SpawnerComponent spawner, RandomAspect randomAspect) {
        if(spawner.SpawnerLoading == SpawnerLoading.Unload) {
            return;
        }
        // Spawn Player1 entities if needed
        while (spawner.Player1SpawnCount < spawner.MaxSpawnCount1 / 2) {
            Entity newEntity1 = Ecb.Instantiate(chunkIndex, spawner.Player1Prefab);
            Ecb.SetComponent(chunkIndex, newEntity1, LocalTransform.FromPosition(randomAspect.GetRandomSpawnPosition(spawner.Player1SpawnPosition)));
            spawner.Player1SpawnCount++;
        }
    }
}
[BurstCompile]
public partial struct ProcessSpawner2Job : IJobEntity {
    public EntityCommandBuffer.ParallelWriter Ecb;
    private void Execute([ChunkIndexInQuery] int chunkIndex, ref SpawnerComponent spawner, RandomAspect randomAspect) {
        if (spawner.SpawnerLoading == SpawnerLoading.Unload) {
            return;
        }
        // Spawn Player2 entities if needed
        while (spawner.Player2SpawnCount < spawner.MaxSpawnCount2 / 2) {
            
            Entity newEntity2 = Ecb.Instantiate(chunkIndex, spawner.Player2Prefab);
            Ecb.SetComponent(chunkIndex, newEntity2, LocalTransform.FromPosition(randomAspect.GetRandomSpawnPosition(spawner.Player2SpawnPosition)));
            spawner.Player2SpawnCount++;
        }
    }
}

