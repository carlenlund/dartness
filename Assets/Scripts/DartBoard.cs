using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartBoard : MonoBehaviour {
  public event EventHandler<int> HitEvent;

  private bool _isHit;

  public GameObject BoardObject;

  private Material _boardMaterial;
  private Material _boardHitMaterial;

  void Start() {
    _isHit = false;
    _boardMaterial = Resources.Load<Material>("Materials/DartBoard/Board");
    _boardHitMaterial = Resources.Load<Material>("Materials/DartBoard/BoardHit");
  }

  void Update() {
    MeshRenderer meshRenderer = BoardObject.GetComponent<MeshRenderer>();
    if (_isHit) {
      meshRenderer.material = _boardHitMaterial;
    } else {
      meshRenderer.material = _boardMaterial;
    }
  }

  private void OnCollisionEnter(Collision collision) {
    if (_isHit) {
      return;
    }

    GameObject gameObject = collision.gameObject;

    Dart dart = gameObject.GetComponentInParent<Dart>();
    if (dart != null) {
      Vector3 point = collision.GetContact(0).point;
      _isHit = true;
      int score = GetScoreFromPoint(point);
      HitEvent?.Invoke(this, score);
    }
  }

  private int GetScoreFromPoint(Vector2 point) {
    Bounds bounds = new Bounds(BoardObject.transform.position, Vector3.zero);
    Renderer renderer = BoardObject.GetComponent<Renderer>();
    bounds.Encapsulate(renderer.bounds);

    Vector3 center = bounds.center;
    float radius = bounds.extents[0];
    float dx = center.x - point.x;
    float dy = center.y - point.y;
    float distanceFromCenter = Mathf.Sqrt(dx * dx + dy * dy);
    float relativeDistance = 1.0f - distanceFromCenter / radius;

    return Mathf.FloorToInt(relativeDistance * 100.0f);
  }
}
