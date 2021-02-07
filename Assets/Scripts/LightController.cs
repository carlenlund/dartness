using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {
  public AudioClip LightSwitchSound;

  private AudioSource _audioSource;
  private bool _enabled;

  // Start is called before the first frame update
  void Start() {
    _audioSource = GetComponent<AudioSource>();
    _enabled = true;
  }

  // Update is called once per frame
  void Update() {
    Light light = GetComponent<Light>();
    light.enabled = _enabled;
  }

  public void SetEnabled(bool enabled) {
    bool changed = enabled != _enabled;
    _enabled = enabled;

    if (changed) {
      _audioSource.pitch = 1.2f + Random.value * 0.2f;
      _audioSource.PlayOneShot(LightSwitchSound, 1.6f);
    }
  }
}
