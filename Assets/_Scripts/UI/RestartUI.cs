using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartUI : MonoBehaviour {

    [SerializeField] private Button _restartButton;

    private void Start() {
        _restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MonoScene");
        });
    }
}
