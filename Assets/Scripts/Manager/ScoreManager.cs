using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        if(scoreText == null)
        {
            Debug.LogError("Score Text is not assigned in the ScoreManager.");
            return;
        }
        else
        {
            scoreText.text = $"0 / {GlobalVariable.Instance.GameClearScore}";
        }
    }

    public void UpdateScore(int score)
    {
        if(scoreText == null)
        {
            Debug.LogError("Score Text is not assigned in the ScoreManager.");
            return;
        }
        
        scoreText.text = $"{score} / {GlobalVariable.Instance.GameClearScore}";
    }
}
