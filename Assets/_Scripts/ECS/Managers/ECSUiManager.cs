using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ECSUiManager : StaticInstance<ECSUiManager> {

    [SerializeField] private TextMeshProUGUI _allPlayersText;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Slider _audioSlider;
    [SerializeField] private List<AudioClip> _deathClips;
    private string _tempString = "";

    private void Start() {
        _audioSlider.onValueChanged.AddListener((value) => {
            _audioSource.volume = value;
        });
    }
    public void UpdateAllPlayers(string strings) {
        if (!_tempString.Equals(strings)) {
            _audioSource.PlayOneShot(_deathClips[UnityEngine.Random.Range(0, _deathClips.Count)]);
            _allPlayersText.text = strings;
            _tempString = strings;
        }

    }
}
