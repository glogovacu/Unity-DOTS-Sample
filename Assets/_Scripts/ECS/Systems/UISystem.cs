using Unity.Collections;
using Unity.Entities;

public partial struct UISystem : ISystem {
    private EntityManager _entityManager;
    public void OnCreate(ref SystemState state) {
        _entityManager = state.EntityManager;
    }

    public void OnDestroy(ref SystemState state) {
    }
    
    public void OnUpdate(ref SystemState state) {
        var _playerEntityQuery = _entityManager.CreateEntityQuery(typeof(PlayerComponent));
        var _playerComponents = _playerEntityQuery.ToComponentDataArray<PlayerComponent>(Allocator.Temp);
        ECSUiManager.Instance.UpdateAllPlayers(_playerComponents.Length.ToString());
        _playerComponents.Dispose();
    }
}
