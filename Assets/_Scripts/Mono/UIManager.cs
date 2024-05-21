using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerAmountText;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Slider _audioSlider;
    [SerializeField] private List<AudioClip> _deathClips;
    private string _tempString = "";
    private void Update() {
        if (!_tempString.Equals((CapsuleSpawner.Instance.Player1s.Count + CapsuleSpawner.Instance.Player2s.Count).ToString())) {
            _audioSource.PlayOneShot(_deathClips[UnityEngine.Random.Range(0, _deathClips.Count)]);
            _playerAmountText.text = (CapsuleSpawner.Instance.Player1s.Count + CapsuleSpawner.Instance.Player2s.Count).ToString();
            _tempString = (CapsuleSpawner.Instance.Player1s.Count + CapsuleSpawner.Instance.Player2s.Count).ToString();
        }
    }

}
