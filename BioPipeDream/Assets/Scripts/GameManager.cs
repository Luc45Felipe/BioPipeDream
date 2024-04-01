using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text _currentScoreTxt, _recordScoreTxt;

    private int _currentScore;

    void Awake()
    {
        Pipe_Ctrl.ImpossibleContinue += GameOver;
        Pipe_Ctrl.NewPipeFilling += AddScorePoint;
        Pipe_Manager.PuzzleCompleted += NextPuzzle;

        GetScores();
    }

    void GetScores()
    {
        if(PlayerPrefs.HasKey("CurrentScore") == false)
        {
            PlayerPrefs.SetInt("CurrentScore", 0);
        }

        if(PlayerPrefs.HasKey("RecordScore") == false)
        {
            PlayerPrefs.SetInt("RecordScore", 0);
        }

        _currentScore = PlayerPrefs.GetInt("CurrentScore");

        _currentScoreTxt.text = PlayerPrefs.GetInt("CurrentScore").ToString();
        _recordScoreTxt.text = PlayerPrefs.GetInt("RecordScore").ToString();
    }

    void NextPuzzle()
    {
        PlayerPrefs.SetInt("CurrentScore", _currentScore);
        ReloadScene();
    }

    void AddScorePoint()
    {
        _currentScore++;
        _currentScoreTxt.text = _currentScore.ToString();
    }

    void GameOver()
    {
        if(_currentScore > PlayerPrefs.GetInt("RecordScore"))
        {
            PlayerPrefs.SetInt("RecordScore", _currentScore);
        }

        PlayerPrefs.SetInt("CurrentScore", 0);
        ReloadScene();
    }

    void ReloadScene()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDestroy()
    {
        Pipe_Ctrl.ImpossibleContinue -= GameOver;
        Pipe_Ctrl.NewPipeFilling -= AddScorePoint;
        Pipe_Manager.PuzzleCompleted -= NextPuzzle;
    }
}
