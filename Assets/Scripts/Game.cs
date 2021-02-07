using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
  public GameObject LevelObject;
  public GameObject ScoreObject;
  public GameObject HighScoreObject;

  private int _score;
  private int _highScore;

  private void Start() {
    HandleLevelStart();

    _score = 0;
    _highScore = 0;

    Level level = LevelObject.GetComponent<Level>();
    level.EndEvent += (_, __) => {
      HandleLevelEnd();
    };
  }

  public void Update() {
    Text scoreText = ScoreObject.GetComponent<Text>();
    scoreText.text = _score.ToString();

    Text highScoreText = HighScoreObject.GetComponent<Text>();
    highScoreText.text = "Highscore: " + _highScore.ToString();
  }

  private void HandleLevelStart() {
    StartCoroutine(StartLevel());
  }

  private IEnumerator StartLevel() {
    yield return new WaitForSeconds(3.0f);

    Level level = LevelObject.GetComponent<Level>();
    level.Initialize();
  }

  private void HandleLevelEnd() {
    Level level = LevelObject.GetComponent<Level>();
    _score = level.TotalScore;
    if (level.LevelIndex == level.LevelPrefabs.Length - 1) {
      if (_score > _highScore) {
        _highScore = _score;
      }
    }
  }
}
