using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

// Most of the gameplay logic and game state goes here.
// Some logic should potentially be moved to Game class.
public class Level : MonoBehaviour {
  public GameObject[] LevelPrefabs;
  public GameObject PlayerObject;
  public GameObject DartPrefab;
  public GameObject Darts;
  public GameObject Light;
  public AudioClip FanfareClip;
  public AudioClip PopClip;

  public event EventHandler<GameObject> EndEvent;

  private AudioSource _audioSource;

  private GameObject _levelObject;
  public int LevelIndex;

  public int TotalScore;
  private int _numStartDarts;
  private int _numDarts;
  private int _numHitOrMissedDarts;

  void Start() {
    _audioSource = GetComponent<AudioSource>();

    Player player = PlayerObject.GetComponent<Player>();
    player.DartThrowEvent += (_, args) => {
      HandleDartThrow(args.Position, args.Direction);
    };
  }

  void Update() {
  }

  public void Initialize() {
    _levelObject = null;
    LevelIndex = 0;
    TotalScore = 0;

    _numStartDarts = 0;
    _numDarts = 0;
    _numHitOrMissedDarts = 0;

    LoadLevel(LevelIndex);
  }

  private void GoToNextLevel() {
    ++LevelIndex;
    if (LevelIndex >= LevelPrefabs.Length) {
      LevelIndex = 0;
    }
    if (LevelIndex == 0) {
      TotalScore = 0;
      EndEvent?.Invoke(this, null);
    }
    LoadLevel(LevelIndex);
  }

  private void LoadLevel(int levelIndex) {
    GameObject levelPrefab = LevelPrefabs[levelIndex];
    Vector3 position = new Vector3(0.0f, 0.0f, -3.53f);
    _levelObject = Instantiate(levelPrefab, position, Quaternion.identity);

    DartBoard[] dartBoards = _levelObject.GetComponentsInChildren<DartBoard>();
    foreach (DartBoard dartBoard in dartBoards) {
      dartBoard.HitEvent += (_, score) => HandleDartBoardHit(score);
    }
    _audioSource.pitch = 1.2f + Random.value * 0.2f;
    _audioSource.PlayOneShot(PopClip, 1.4f);

    _numStartDarts = dartBoards.Length;
    _numDarts = _numStartDarts;
    _numHitOrMissedDarts = 0;

    HandleLevelStart();
  }

  private void InitializeDarts() {
    float xOffset = 0.0f;
    float zOffset = 0.0f;
    for (int i = 0; i < _numDarts; ++i) {
      if (i > 0 && i % 3 == 0) {
        xOffset = 0.0f;
        zOffset -= 0.6f;
      }
      Vector3 position = new Vector3(xOffset, 0.0f, zOffset);
      AddDart(position);
      xOffset += 0.2f;
    }
  }

  private void AddDart(Vector3 position) {
    GameObject dartObject = Instantiate(DartPrefab, position, Quaternion.identity);
    dartObject.transform.parent = Darts.transform;
    dartObject.transform.localPosition = position;
    Dart dart = dartObject.GetComponent<Dart>();

    _audioSource.pitch = 1.2f + Random.value * 0.2f;
    _audioSource.PlayOneShot(PopClip, 1.4f);

    bool hitHandled = false;
    dart.HitEvent += (_, gameObject) => {
      if (hitHandled) {
        return;
      }
      DartBoard dartBoard = gameObject.GetComponentInParent<DartBoard>();
      Dart otherDart = gameObject.GetComponentInParent<Dart>();
      Table table = gameObject.GetComponentInParent<Table>();
      Player player = gameObject.GetComponentInParent<Player>();
      if (dartBoard == null &&
          otherDart == null &&
          table == null &&
          player == null) {
        HandleDartBoardMiss();
        hitHandled = true;
      }
    };

    Throwable throwable = dartObject.GetComponent<Throwable>();
    throwable.onDetachFromHand.AddListener(HandleVRDartThrow);
  }

  private void HandleDartBoardHit(int score) {
    HandleDartBoardHitOrMiss();
    TotalScore += score;
  }

  private void HandleDartBoardMiss() {
    HandleDartBoardHitOrMiss();
  }

  private void HandleDartBoardHitOrMiss() {
    ++_numHitOrMissedDarts;
    if (_numHitOrMissedDarts >= _numStartDarts) {
      HandleLevelEnd();
    }
  }

  public void HandleVRDartThrow() {
    if (_numDarts <= 0) {
      return;
    }
    --_numDarts;
  }

  // Used for throwing with keybinding.
  // NOTE: Currently unused.
  private void HandleDartThrow(Vector3 position, Vector3 direction) {
    if (_numDarts <= 0) {
      return;
    }

    GameObject dartObject = Instantiate(DartPrefab, position, Quaternion.identity);
    dartObject.transform.parent = Darts.transform;
    Dart dart = dartObject.GetComponent<Dart>();
    dart.Throw(direction);

    bool hitHandled = false;

    dart.HitEvent += (_, gameObject) => {
      if (hitHandled) {
        return;
      }
      DartBoard dartBoard = gameObject.GetComponentInParent<DartBoard>();
      Dart otherDart = gameObject.GetComponentInParent<Dart>();
      if (dartBoard == null && otherDart == null) {
        HandleDartBoardMiss();
        hitHandled = true;
      }
    };

    --_numDarts;
  }

  private void HandleLevelStart() {
    StartCoroutine(StartLevel());
  }

  private void HandleLevelEnd() {
    StartCoroutine(EndLevel());
  }

  private IEnumerator StartLevel() {
    yield return new WaitForSeconds(5.0f);

    if (LevelIndex > 0) {
      LightController light = Light.GetComponent<LightController>();
      light.SetEnabled(false);
    }
    
    yield return new WaitForSeconds(2.0f);
    InitializeDarts();
  }

  private IEnumerator EndLevel() {
    yield return new WaitForSeconds(2.0f);
    _audioSource.pitch = 1.2f + Random.value * 0.2f;
    _audioSource.PlayOneShot(FanfareClip, 1.0f);

    yield return new WaitForSeconds(3.0f);

    LightController light = Light.GetComponent<LightController>();
    light.SetEnabled(true);

    EndEvent?.Invoke(this, null);

    yield return new WaitForSeconds(3.0f);

    Dart[] darts = FindObjectsOfType<Dart>();
    foreach (Dart dart in darts) {
      GameObject dartObject = dart.gameObject;
      // FIXME: Should be destroyed rather than deactivated.
      //GameObject.Destroy(dartObject);
      dartObject.SetActive(false);
    }

    GameObject.Destroy(_levelObject);

    yield return new WaitForSeconds(0.5f);
    GoToNextLevel();
  }
}
