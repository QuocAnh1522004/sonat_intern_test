using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _levelPassedPanel;
    public List<TubeView> tubeViews; //asign in inspector
    private int _selectedIndex;
    private GameModel _gameModel;
    private const int _maxLiquidStack = 4;
    private int _resetIndex = -1;
    

    private void Start()
    {
        SetupGame();
        _selectedIndex = _resetIndex;
    }

    void SetupGame()
    {
        List<TubeModel> models = new List<TubeModel>();
        for (int i = 0; i < tubeViews.Count; i++)
        {
            TubeModel model = new TubeModel(_maxLiquidStack);

            switch (i)
            {
                case 0:
                    model.AddLayer(ColorType.Red);
                    model.AddLayer(ColorType.Blue);
                    model.AddLayer(ColorType.Yellow);
                    model.AddLayer(ColorType.Red);
                    break;

                case 1:
                    model.AddLayer(ColorType.Blue);
                    model.AddLayer(ColorType.Yellow);
                    model.AddLayer(ColorType.Red);
                    model.AddLayer(ColorType.Blue);
                    break;

                case 2:
                    model.AddLayer(ColorType.Yellow);
                    model.AddLayer(ColorType.Red);
                    model.AddLayer(ColorType.Blue);
                    model.AddLayer(ColorType.Yellow);
                    break;

                case 3:
                case 4:
                    // empty tube
                    break;
            }

            models.Add(model);
            tubeViews[i].Initialize(model);
        }
        _gameModel = new GameModel(models);
    }

    public void OnTubeClicked(int targetIndex)
    {
        //First selection
        if (_selectedIndex == _resetIndex)
        {
            Debug.Log("Current tube index:" + targetIndex);
            _selectedIndex = targetIndex;
            tubeViews[_selectedIndex].SetSelected(true);
            return;
        }

        //Same tube clicked → cancel selection
        if (_selectedIndex == targetIndex)
        {
            Debug.Log("same tube" + _selectedIndex + "selected, canceling select" );
            tubeViews[_selectedIndex].SetSelected(false);
            _selectedIndex = _resetIndex;      
            return;
        }

        //Try pour
        if (_gameModel.TryPour(_selectedIndex, targetIndex))
        {
            Debug.Log("Poured successfully from tube " + _selectedIndex + " to tube " + targetIndex);
            RefreshAll();
            _selectedIndex = _resetIndex;
            if (_gameModel.CheckWin())
            {
                _levelPassedPanel.SetActive(true);
            }
        }
        else
        {
            tubeViews[_selectedIndex].SetSelected(false);
            tubeViews[targetIndex].SetSelected(true); //will choose new tube to be selected if can not pour
            _selectedIndex = targetIndex;
        }

        ////Clear selection after attempt
        //_selectedIndex = -1;
    }

    private void RefreshAll()
    {
        for(int i = 0; i < tubeViews.Count; i++)
        {
            tubeViews[i].Refresh();
        }
    }
}
