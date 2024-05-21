using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct SceneLoaderSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SceneReference>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var sceneQuery = SystemAPI.QueryBuilder().WithAll<SceneReference>().Build();
        var requests = sceneQuery.ToComponentDataArray<SceneReference>(Allocator.Temp);
        for (int index = 0; index < requests.Length; ++index) {
            SceneSystem.LoadSceneAsync(state.WorldUnmanaged, requests[index].Value);
        }
    }
}
