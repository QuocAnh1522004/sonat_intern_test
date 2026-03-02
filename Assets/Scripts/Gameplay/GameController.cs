using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _levelPassedPanel;
    [SerializeField] private GameObject _tubePrefab;
    [SerializeField] private GameObject _tubeSpawnArea;
    [SerializeField] private int _columns = 4;
    [SerializeField] private float _spacingX = 1.5f;
    [SerializeField] private float _spacingY = 2f;
    public LevelDataSO levelDataSO;
    private List<TubeView> tubeViews; 
    private GameModel _gameModel;
    private const int _maxLiquidStack = 4;
    private int _selectedIndex;
    private int _resetIndex = -1;
    private bool _isAnimating = false;

    private void Awake()
    {
        _selectedIndex = _resetIndex;
    }
    private void Start()
    {
        SetupGame();
    }

    void SetupGame()
    {     
        List<TubeModel> modelList = new List<TubeModel>();
        tubeViews = new List<TubeView>();
        int totalTubes = levelDataSO.tubeLevelDataSO.Count;
        for (int i = 0; i < totalTubes; i++)
        {
            TubeModel model = new TubeModel(_maxLiquidStack);
            foreach (var color in levelDataSO.tubeLevelDataSO[i].colorlayers)
            {
                model.AddLayer(color);
            }
            modelList.Add(model);

            int row = i / _columns;
            int col = i % _columns;
            float totalWidth = (_columns - 1) * _spacingX;
            Vector3 startOffset = new Vector3(-totalWidth / 2f, 0, 0);
            Vector3 spawnPos = _tubeSpawnArea.transform.position +
                   startOffset +
                   new Vector3(col * _spacingX, -row * _spacingY, 0);
            GameObject tubeObj = Instantiate(_tubePrefab, spawnPos, Quaternion.identity);
            TubeView view = tubeObj.GetComponent<TubeView>();
            view.Initialize(model, this, i);
            tubeViews.Add(view);         
        }
        _gameModel = new GameModel(modelList);
    }

    public async void OnTubeClicked(int targetIndex)
    {
        if (_isAnimating) return;
        TubeModel targetModel = tubeViews[targetIndex].Model;
        //Block first select on empty bottle
        if (_selectedIndex == _resetIndex && targetModel.IsEmpty) return;

        //Block completed tubes
        if (targetModel.IsFullAndFilledWithOneColor()) return;

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

        int pourCount = _gameModel.TryPour(_selectedIndex, targetIndex);
        //Try pour
        if (pourCount > 0)
        {
            _isAnimating = true;
            await tubeViews[_selectedIndex]
            .PlayPourAnimation(tubeViews[targetIndex], pourCount);
            _gameModel.ApplyPour(_selectedIndex, targetIndex, pourCount);
            Debug.Log("Poured successfully from tube " + _selectedIndex + " to tube " + targetIndex);
            RefreshAll();
            _selectedIndex = _resetIndex;
            if (_gameModel.CheckWin())
            {
                _levelPassedPanel.SetActive(true);
            }
            _isAnimating = false;
        }
        else
        {
            tubeViews[_selectedIndex].SetSelected(false);
            tubeViews[targetIndex].SetSelected(true); //will choose new tube to be selected if can not pour
            _selectedIndex = targetIndex;
        }
    }

    private void RefreshAll()
    {
        for(int i = 0; i < tubeViews.Count; i++)
        {
            tubeViews[i].Refresh();
        }
    }
}
