using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CapsuleSpawner : Singleton<CapsuleSpawner>
{
    public List<GameObject> Player1s = new List<GameObject>();
    public List<GameObject> Player2s = new List<GameObject>();
    public bool IsPlayerMovementEnabled = false;
    [SerializeField] private GameObject _capsulePrefabPlayer1;
    [SerializeField] private GameObject _capsulePrefabPlayer2;
    [SerializeField] private int _numberOfCapsules = 100;

    [SerializeField] private Vector3 _player1SpawnPoint = new Vector3(20, 1, 0);
    [SerializeField] private Vector3 _player2SpawnPoint = new Vector3(-20, 1, 0);

    private bool _playMode = false;

    public void SpawnerInit() {
        SpawnCapsules(_player1SpawnPoint, "Player1", Player1s, _capsulePrefabPlayer1);
        SpawnCapsules(_player2SpawnPoint, "Player2", Player2s, _capsulePrefabPlayer2);
    }
    private void Update() {
        if (!_playMode) {
            return;
        }
        if (Player1s.Count < _numberOfCapsules / 2) {
            SpawnCapsules(_player1SpawnPoint, "Player1", Player1s, _capsulePrefabPlayer1);
        }
        if(Player2s.Count < _numberOfCapsules / 2) {
            SpawnCapsules(_player2SpawnPoint, "Player2", Player2s, _capsulePrefabPlayer2);
        }


    }

    private void SpawnCapsules(Vector3 spawnPoint, string layerName, List<GameObject> gameObjects, GameObject capsulePrefab) {
        for (int i = gameObjects.Count; i < _numberOfCapsules / 2; i++) {
            Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            Vector3 spawnPosition = spawnPoint + randomOffset;

            GameObject capsule = Instantiate(capsulePrefab, spawnPosition, Quaternion.identity);
            gameObjects.Add(capsule);
            capsule.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public void StartPlayMode() {
        _playMode = true;
    }
    public void SetNumberOfCapsules(int numberOfCapsules) {
        _numberOfCapsules = numberOfCapsules;
    }
}
