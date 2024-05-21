using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayModeUi : MonoBehaviour {
    [SerializeField] private Button _playButton;
    [SerializeField] private TMP_InputField _capsuleInputField;
    [SerializeField] private Toggle _movementToggle;
    [SerializeField] private Toggle _burstToggle;
    [SerializeField] private TextMeshProUGUI _howLongDidItTakeText;
    [SerializeField] private Canvas _playModeCanvas;
    [SerializeField] private TextMeshProUGUI _sceneText;

    private EntityManager _entityManager;
    private Entity _spawnerEntity;
    void Start() {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _sceneText.text = "Scene: " + SceneManager.GetActiveScene().name;
    }

    private void OnPlayButtonClicked() {
        if (SceneManager.GetActiveScene().name.Equals("MonoScene")) {
            CapsuleSpawner.Instance.SetNumberOfCapsules(int.Parse(_capsuleInputField.text));
            CapsuleSpawner.Instance.SpawnerInit();
            CapsuleSpawner.Instance.StartPlayMode();
            CapsuleSpawner.Instance.IsPlayerMovementEnabled = _movementToggle.isOn;
        } else {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _spawnerEntity = _entityManager.CreateEntityQuery(typeof(SpawnerComponent)).GetSingletonEntity();

            var unmanaged = World.DefaultGameObjectInjectionWorld.Unmanaged;
            int maxSpawns = int.Parse(_capsuleInputField.text);
            SpawnerComponent spawner = _entityManager.GetComponentData<SpawnerComponent>(_spawnerEntity);

            spawner.MaxSpawnCount1 = maxSpawns;
            spawner.MaxSpawnCount2 = maxSpawns;
            spawner.SpawnerLoading = SpawnerLoading.Load;
            spawner.Player1SpawnCount = 0;
            spawner.Player2SpawnCount = 0;

            _entityManager.SetComponentData(_spawnerEntity, spawner);

            var movementSystemWithBurst = unmanaged.GetExistingUnmanagedSystem<PlayerSystemWithBurst>();
            var movementSystemNoBurst = unmanaged.GetExistingUnmanagedSystem<PlayerSystemNoBurst>();
            unmanaged.ResolveSystemStateRef(movementSystemWithBurst).Enabled = _movementToggle.isOn && _burstToggle.isOn;
            unmanaged.ResolveSystemStateRef(movementSystemNoBurst).Enabled = _movementToggle.isOn && !_burstToggle.isOn;

            var spawnerSystemWithBurst = unmanaged.GetExistingUnmanagedSystem<SpawnerSystemWithBurst>();
            var spawnerSystemNoBurst = unmanaged.GetExistingUnmanagedSystem<SpawnerSystemNoBurst>();
            unmanaged.ResolveSystemStateRef(spawnerSystemWithBurst).Enabled = _burstToggle.isOn;
            unmanaged.ResolveSystemStateRef(spawnerSystemNoBurst).Enabled = !_burstToggle.isOn;

            
        }
        _playModeCanvas.enabled = false;
    }
    private void OnDestroy() {
    }
}
