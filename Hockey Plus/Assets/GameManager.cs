using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }
    public enum GameStatus { Paused, Playing }
    public GameStatus Status { get; private set; }

    [SerializeField, Min(0f)] float playerSpawnDistance = 3f;
    [SerializeField, Min(1)] int numberOfBalls = 1;
    [SerializeField, Min(0f)] float farthestBallExtent = 2f;

    [SerializeField] Rigidbody2D bottomPlayer;
    [SerializeField] Rigidbody2D topPlayer;
    [SerializeField] GameObject ballPrefab;

    public int[] RoundPoints { get; } = new int[2];
    public int[] GamePoints { get; } = new int[2];
    public event Action pointsChanged = delegate { };
    int remainingBalls;
    List<GameObject> balls = new List<GameObject>();

    WaitForSeconds pause = new WaitForSeconds(1f);

    void Awake() {
        if (Instance) {
            Debug.LogError("Multiple GameManagers not supported");
        } else {
            Instance = this;
        }
        FullReset();
    }

    public void FullReset() {
        StartCoroutine(nameof(InternalFullReset));
    }

    IEnumerator InternalFullReset() {
        ResetLayout();
        yield return pause;
        Status = GameStatus.Playing;
    }

    public void ResetLayout() {
        bottomPlayer.position = new Vector2(0f, -playerSpawnDistance);
        topPlayer.position = new Vector2(0f, playerSpawnDistance);

        if (numberOfBalls == 1) {
            balls.Add(Instantiate(ballPrefab, Vector2.zero, Quaternion.identity));
        } else {
            float ballSpread = 2f * farthestBallExtent / (numberOfBalls - 1);
            for (int i = 0; i < numberOfBalls; i++) {
                balls.Add(Instantiate(ballPrefab, new Vector2(-farthestBallExtent + i * ballSpread, 0f), Quaternion.identity));
            }
        }
        remainingBalls = numberOfBalls;
    }

    public void AddRoundPoint(int player) {
        RoundPoints[player] += 1;
        pointsChanged();
        remainingBalls -= 1;
        if (remainingBalls == 0 || RoundPoints[0] > numberOfBalls / 2 || RoundPoints[1] > numberOfBalls / 2) {
            EndRound();
        }
    }

    void EndRound() {
        foreach (GameObject ball in balls) {
            if (ball) {
                ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
        if (RoundPoints[0] > RoundPoints[1]) {
            GamePoints[0]++;
        } else if (RoundPoints[1] > RoundPoints[0]) {
            GamePoints[1]++;
        }
        RoundPoints[0] = RoundPoints[1] = 0;
        pointsChanged();
        StartCoroutine(nameof(PostRoundPause));
    }

    IEnumerator PostRoundPause() {
        Status = GameStatus.Paused;
        yield return pause;
        foreach (GameObject ball in balls) {
            if (ball) {
                Destroy(ball);
            }
        }
        balls.Clear();
        FullReset();
    }
}