using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

public class Dart : MonoBehaviour {
  public event EventHandler<GameObject> HitEvent;

  public AudioClip ThrowClip;
  public AudioClip HitClip;

  private AudioSource _audioSource;
  private bool _stuckInDartBoard;
  private bool _pickedUp;

  void Start() {
    _audioSource = GetComponent<AudioSource>();
    _stuckInDartBoard = false;
    _pickedUp = false;

    Throwable throwable = GetComponent<Throwable>();
    throwable.onPickUp.AddListener(HandlePickup);
    throwable.onDetachFromHand.AddListener(HandleDetach);
  }

  void Update() {
    Rigidbody body = GetComponent<Rigidbody>();
    body.isKinematic = _stuckInDartBoard;
    body.useGravity = !_pickedUp;
  }

  private void OnCollisionEnter(Collision collision) {
    if (_pickedUp) {
      return;
    }

    GameObject gameObject = collision.gameObject;

    DartBoard dartBoard = gameObject.GetComponentInParent<DartBoard>();
    if (dartBoard != null) {
      _stuckInDartBoard = true;
      _audioSource.pitch = 1.2f + Random.value * 0.2f;
      _audioSource.PlayOneShot(HitClip, 1.0f);
    }

    HitEvent?.Invoke(this, gameObject);
  }

  public void HandlePickup() {
    _pickedUp = true;
  }

  public void HandleDetach() {
    _pickedUp = false;
    /*_audioSource.pitch = 0.9f + Random.value * 0.1f;
    _audioSource.PlayOneShot(ThrowClip, 0.8f);*/
  }

  public void Throw(Vector3 direction) {
    Rigidbody body = GetComponentInChildren<Rigidbody>();
    body.angularVelocity = new Vector3(0.0f, 10.0f, 0.0f);
    float initialSpeed = 14.0f;
    body.velocity = direction;
    body.velocity += new Vector3(0.0f, 0.2f, 0.0f);
    body.velocity *= initialSpeed;

    HandleDetach();
  }
}
