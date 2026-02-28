using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public List<TubeView> tubeViews; //asign in inspector
    private int _selectedIndex = -1;
    private GameModel _gameModel;
    private int _maxLiquidStack = 4;

    private void Start()
    {
        SetupGame();
    }

    void SetupGame()
    {
        List<TubeModel> models = new List<TubeModel>();
        for(int i = 0; i < tubeViews.Count; i++)
        {
            TubeModel model = new TubeModel(_maxLiquidStack);
            //Temp setup
            if (i < 1)
            {
                model.AddLayer(ColorType.Red);
                model.AddLayer(ColorType.Yellow);
                model.AddLayer(ColorType.Blue);
            }

            models.Add(model);
            tubeViews[i].Initialize(model);
        }
        _gameModel = new GameModel(models);
    }

    public void OnTubeClicked(int index)
    {
        // First selection
        if (_selectedIndex == -1)
        {
            Debug.Log("Current tube index:" + index);
            _selectedIndex = index;
            return;
        }

        // Same tube clicked → cancel selection
        if (_selectedIndex == index)
        {
            Debug.Log("same tube" + _selectedIndex + "selected, canceling select" );
            _selectedIndex = -1;
            return;
        }

        // Try pour
        if (_gameModel.TryPour(_selectedIndex, index))
        {
            Debug.Log("Poured successfully from tube " + _selectedIndex+ " to tube " + index);
            RefreshAll();

            if (_gameModel.CheckWin())
            {
                Debug.Log("You Win!");
            }
        }

        // Always clear selection after attempt
        _selectedIndex = -1;
    }

    private void RefreshAll()
    {
        for(int i = 0; i < tubeViews.Count; i++)
        {
            tubeViews[i].Refresh();
        }
    }
}
