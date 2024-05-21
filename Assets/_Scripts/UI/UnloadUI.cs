using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UnloadUI : MonoBehaviour {
    [SerializeField] private Button _unloadButton;
    [SerializeField] private Canvas _canvasToShow;

    private EntityManager _entityManager;
    private Entity _spawnerEntity; 
    private void Start() {
        _unloadButton.onClick.AddListener(() => {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _spawnerEntity = _entityManager.CreateEntityQuery(typeof(SpawnerComponent)).GetSingletonEntity();

            var unmanaged = World.DefaultGameObjectInjectionWorld.Unmanaged;

            var movementBurstSystem = unmanaged.GetExistingUnmanagedSystem<SpawnerSystemWithBurst>();
            var movementNoBurstSystem = unmanaged.GetExistingUnmanagedSystem<SpawnerSystemNoBurst>();

            // Stopping them so we dont interupt the ECB
            unmanaged.ResolveSystemStateRef(movementBurstSystem).Enabled = false;
            unmanaged.ResolveSystemStateRef(movementNoBurstSystem).Enabled = false;

            SpawnerComponent spawner = _entityManager.GetComponentData<SpawnerComponent>(_spawnerEntity);
            spawner.SpawnerLoading = SpawnerLoading.Unload;
            _entityManager.SetComponentData(_spawnerEntity, spawner);

            // Starting it back up so it cleans up the entities
            unmanaged.ResolveSystemStateRef(movementBurstSystem).Enabled = true;
            unmanaged.ResolveSystemStateRef(movementNoBurstSystem).Enabled = true;

            _canvasToShow.enabled = true;
        });
    }
}
