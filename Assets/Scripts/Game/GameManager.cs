using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuikaGame.Game
{
    public sealed class GameManager : MonoBehaviour
    {
        private const string HighScoreKey = "Suika.HighScore";

        [SerializeField]
        private bool freezePhysicsOnGameOver = true;

        public bool IsPlaying { get; private set; }
        public int Score { get; private set; }
        public int HighScore { get; private set; }

        public event Action<int> ScoreChanged;
        public event Action GameOver;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            Time.timeScale = 1f;
            Score = 0;
            IsPlaying = true;
            ScoreChanged?.Invoke(Score);
        }

        public void AddScore(int amount)
        {
            if (!IsPlaying || amount <= 0)
            {
                return;
            }

            Score += amount;
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
            }

            ScoreChanged?.Invoke(Score);
        }

        public void TriggerGameOver()
        {
            if (!IsPlaying)
            {
                return;
            }

            IsPlaying = false;
            if (freezePhysicsOnGameOver)
            {
                foreach (var body in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None))
                {
                    body.linearVelocity = Vector2.zero;
                    body.angularVelocity = 0f;
                    body.simulated = false;
                }
            }

            GameOver?.Invoke();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
