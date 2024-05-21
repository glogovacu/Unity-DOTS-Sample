using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial struct PlayerSystemNoBurst : ISystem {
    private EntityManager _entityManager;
    private EntityQuery _playerEntityQuery;
    private FixedString64Bytes _sceneString;
    private FixedString64Bytes _ecsSceneString;
    public void OnCreate(ref SystemState state) {
        _entityManager = state.EntityManager;

        _playerEntityQuery = _entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerComponent>(),
            ComponentType.ReadOnly<LocalTransform>());

        _sceneString = new FixedString64Bytes(SceneManager.GetActiveScene().name);
        _ecsSceneString = new FixedString64Bytes("ECSOnlyScene");

        state.RequireForUpdate(_playerEntityQuery);
        state.RequireForUpdate<SpawnerComponent>();
    }
    public void OnUpdate(ref SystemState state) {
        var players = _playerEntityQuery.ToEntityArray(Allocator.TempJob);
        var playerComponents = _playerEntityQuery.ToComponentDataArray<PlayerComponent>(Allocator.TempJob);
        var transforms = _playerEntityQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        if (_sceneString == _ecsSceneString) {
            Debug.Log("Im playing");
            ProcessPlayers(players, playerComponents, transforms, ref state);
        } else {
            new FindClosestPlayerJob {
                PlayerComponents = playerComponents,
                PlayerEntities = players,
                Transforms = transforms,
            }.ScheduleParallel(_playerEntityQuery, state.Dependency).Complete();

            for (int i = 0; i < players.Length; i++) {
                if (!_entityManager.Exists(players[i]))
                    continue;

                Entity closestTarget = _entityManager.GetComponentData<PlayerComponent>(players[i]).PlayerTarget;
                if (!_entityManager.Exists(closestTarget)) {
                    closestTarget = FindClosestTarget(players, playerComponents, transforms, i);
                }
                UpdatePlayerTarget(players[i], closestTarget, playerComponents[i], transforms[i], ref state);
            }
        }


        state.Dependency = DisposeComponentDataArrays(players, playerComponents, transforms, state.Dependency);

    }
    private void ProcessPlayers(NativeArray<Entity> players, NativeArray<PlayerComponent> playerComponents, NativeArray<LocalTransform> transforms, ref SystemState state) {
        for (int i = 0; i < players.Length; i++) {
            if (!_entityManager.Exists(players[i]))
                continue;

            Entity closestTarget = FindClosestTarget(players, playerComponents, transforms, i);
            UpdatePlayerTarget(players[i], closestTarget, playerComponents[i], transforms[i], ref state);
        }

    }
    private Entity FindClosestTarget(NativeArray<Entity> players, NativeArray<PlayerComponent> playerComponents, NativeArray<LocalTransform> transforms, int currentPlayerIndex) {
        if (playerComponents[currentPlayerIndex].PlayerTarget != Entity.Null) {
            return playerComponents[currentPlayerIndex].PlayerTarget;
        }
        float closestDistance = float.MaxValue;
        Entity closestTarget = Entity.Null;
        float3 currentPosition = transforms[currentPlayerIndex].Position;

        for (int j = 0; j < players.Length; j++) {
            if (playerComponents[currentPlayerIndex].PlayerTeam == playerComponents[j].PlayerTeam || !_entityManager.Exists(players[j]))
                continue;

            float distance = math.distance(currentPosition, transforms[j].Position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestTarget = players[j];
            }
        }

        return closestTarget;
    }
    private void UpdatePlayerTarget(Entity player, Entity target, PlayerComponent playerComponent, LocalTransform transform, ref SystemState state) {
        if (playerComponent.PlayerTarget != target) {
            playerComponent.PlayerTarget = target;
            playerComponent.PlayerTargetTransform = _entityManager.GetComponentData<LocalTransform>(target);
            _entityManager.SetComponentData(player, playerComponent);
        }

        if (target != Entity.Null && _entityManager.Exists(target)) {
            MovePlayerTowardTarget(player, target, transform, ref state);
        }
    }
    private void MovePlayerTowardTarget(Entity player, Entity target, LocalTransform transform, ref SystemState state) {
        var targetPosition = _entityManager.GetComponentData<LocalTransform>(target).Position;
        var direction = math.normalize(targetPosition - transform.Position);
        float movementSpeed = 5.0f;
        var newPosition = transform.Position + direction * movementSpeed * Time.deltaTime;

        _entityManager.SetComponentData(player, LocalTransform.FromPosition(newPosition));

        if (math.distance(newPosition, targetPosition) < 1.0f) {
            _entityManager.DestroyEntity(target);
            AdjustSpawnCountAfterDestruction(player, ref state);
        }
    }
    private void AdjustSpawnCountAfterDestruction(Entity player, ref SystemState state) {
        RefRW<SpawnerComponent> spawner = SystemAPI.GetSingletonRW<SpawnerComponent>();
        var playerComponent = _entityManager.GetComponentData<PlayerComponent>(player);
        if (playerComponent.PlayerTeam == 1) {
            spawner.ValueRW.Player2SpawnCount--;
        } else {
            spawner.ValueRW.Player1SpawnCount--;
        }

    }
    private JobHandle DisposeComponentDataArrays(NativeArray<Entity> players, NativeArray<PlayerComponent> playerComponents, NativeArray<LocalTransform> transforms, JobHandle inputDeps) {
        inputDeps = players.Dispose(inputDeps);
        inputDeps = playerComponents.Dispose(inputDeps);
        inputDeps = transforms.Dispose(inputDeps);
        return inputDeps;
    }
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        return ecb.AsParallelWriter();
    }
}
