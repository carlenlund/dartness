using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {
  public bool lightEnabled = true;

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    Light light = GetComponent<Light>();
    light.enabled = lightEnabled;
  }
}
