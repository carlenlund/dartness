using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
  public const float MOVEMENT_SPEED = 2.0f;

  void Start() {

  }

  void Update() {
    float movementSpeed = MOVEMENT_SPEED;
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
    if (direction.magnitude != 0) {
      direction.Normalize();
      Vector3 offset = direction * movementSpeed * Time.deltaTime;
      transform.position += offset;
      /*
      transform.rotation = Quaternion.AngleAxis(
          -90.0f + Mathf.Atan2(-direction.z, direction.x) * Mathf.Rad2Deg,
          new Vector3(0.0f, 1.0f, 0.0f));
      */
    }
  }
}
