using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<LevelDataSO> _levels;
    [SerializeField] private GameController _gameController;
    [SerializeField] private GameObject LevelPassedPanel;
    private int currentLevelIndex = 0;
    public void LoadNextLevel()
    {
        if (_levels == null || _levels.Count == 0)
        {
            Debug.LogWarning("No levels configured!");
            return;
        }
        currentLevelIndex = (currentLevelIndex + 1) % _levels.Count;
        LoadLevel(_levels[currentLevelIndex]);
    }

    private void LoadLevel(LevelDataSO level)
    {
        ClearAllTubes();
        _gameController.levelDataSO = level;
        LevelPassedPanel.SetActive(false);
        _gameController.SetupGame();
    }

    public void ClearAllTubes()
    {
        TubeView[] tubes = FindObjectsOfType<TubeView>();

        foreach (var tube in tubes)
        {
            Destroy(tube.gameObject);
        }
    }

}
