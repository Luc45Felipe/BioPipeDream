using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private Text _currentScoreTxt, _recordScoreTxt;

    private int _currentScore;

    void Awake()
    {
        PipeManager.PuzzleCompleted += PipeManager_PuzzleCompleted;
        PipeCtrl.AnotherPipeFilling += PipeCtrl_AnotherPipeFilling;
        PipeCtrl.ImpossibleContinue += PipeCtrl_ImpossibleContinue;

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

    // Next scene
    void PipeManager_PuzzleCompleted()
    {
        PlayerPrefs.SetInt("CurrentScore", _currentScore);
        ReloadScene();
    }

    // Increment Current Score
    void PipeCtrl_AnotherPipeFilling()
    {
        _currentScore++;
        _currentScoreTxt.text = _currentScore.ToString();
    }

    // Gameover
    void PipeCtrl_ImpossibleContinue()
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
        PipeManager.PuzzleCompleted -= PipeManager_PuzzleCompleted;
        PipeCtrl.AnotherPipeFilling -= PipeCtrl_AnotherPipeFilling;
        PipeCtrl.ImpossibleContinue -= PipeCtrl_ImpossibleContinue;
    }
}
