using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial struct SpawnerSystemNoBurst : ISystem {
    private EntityQuery _playerQuery;
    private FixedString64Bytes _sceneString;
    private FixedString64Bytes _ecsSceneString;
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SpawnerComponent>();
        _playerQuery = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        _sceneString = new FixedString64Bytes(SceneManager.GetActiveScene().name);
        _ecsSceneString = new FixedString64Bytes("ECSOnlyScene");
    }

    public void OnUpdate(ref SystemState state) {
        if (_sceneString == _ecsSceneString) {
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
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        return ecb.AsParallelWriter();
    }
}
