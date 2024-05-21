using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class FpsUI : MonoBehaviour { 
    [SerializeField] private TextMeshProUGUI _fpsText;

    private int _lastFrameIndex;
    private float[] _frameDeltaTimeArray;

    private void Awake() {
        _frameDeltaTimeArray = new float[100];
    }
    private void Update() {
        _frameDeltaTimeArray[_lastFrameIndex] = Time.unscaledDeltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimeArray.Length;
        _fpsText.text = "FPS: " + Mathf.RoundToInt(CalculateFPS()).ToString();

    }
    private float CalculateFPS() {
        float total = 0f;
        foreach (float deltaTime in _frameDeltaTimeArray) {
            total += deltaTime;
        }
        return _frameDeltaTimeArray.Length / total;
    }
}
