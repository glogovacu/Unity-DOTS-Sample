using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
    [SerializeField] private InputActionReference _ecsKey;
    [SerializeField] private InputActionReference _jobKey;
    [SerializeField] private InputActionReference _monoKey;
    private void Awake() {
        Debug.Log($"Before: {Application.targetFrameRate}, {QualitySettings.vSyncCount}");
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        Debug.Log($"After: {Application.targetFrameRate}, {QualitySettings.vSyncCount}");

    }
    private void Start() {
        var unmanaged = World.DefaultGameObjectInjectionWorld.Unmanaged;
        var playerNoBurstSystem = unmanaged.GetExistingUnmanagedSystem<PlayerSystemNoBurst>();
        unmanaged.ResolveSystemStateRef(playerNoBurstSystem).Enabled = false;
        var playerBurstSystem = unmanaged.GetExistingUnmanagedSystem<PlayerSystemWithBurst>();
        unmanaged.ResolveSystemStateRef(playerBurstSystem).Enabled = false;
        var spawnerNoBurstSystem = unmanaged.GetExistingUnmanagedSystem<SpawnerSystemNoBurst>();
        unmanaged.ResolveSystemStateRef(spawnerNoBurstSystem).Enabled = false;
        var spawnerWithBurstSystem = unmanaged.GetExistingUnmanagedSystem<SpawnerSystemWithBurst>();
        unmanaged.ResolveSystemStateRef(spawnerWithBurstSystem).Enabled = false;
        if (SceneManager.GetActiveScene().name.Equals("MonoScene")) {
            var sceneLoaderSystem = unmanaged.GetExistingUnmanagedSystem<SceneLoaderSystem>();
            unmanaged.ResolveSystemStateRef(sceneLoaderSystem).Enabled = false;
            var uiSystem = unmanaged.GetExistingUnmanagedSystem<UISystem>();
            unmanaged.ResolveSystemStateRef(uiSystem).Enabled = false;
        } else {
            var sceneLoaderSystem = unmanaged.GetExistingUnmanagedSystem<SceneLoaderSystem>();
            unmanaged.ResolveSystemStateRef(sceneLoaderSystem).Enabled = true;
            var uiSystem = unmanaged.GetExistingUnmanagedSystem<UISystem>();
            unmanaged.ResolveSystemStateRef(uiSystem).Enabled = true;
        }
        _ecsKey.action.started += EcsKey_Clicked;
        _jobKey.action.started += JobKey_Clicked;
        _monoKey.action.started += MonoKey_Clicked;
    }

    private void MonoKey_Clicked(InputAction.CallbackContext obj) {
        SceneManager.LoadScene("MonoScene");
    }

    private void JobKey_Clicked(InputAction.CallbackContext obj) {
        SceneManager.LoadScene("ECSWithJobScene");
    }

    private void EcsKey_Clicked(InputAction.CallbackContext obj) {
        SceneManager.LoadScene("ECSOnlyScene");
    }

    private void OnDestroy() {
        _ecsKey.action.started -= EcsKey_Clicked;
    }
}
