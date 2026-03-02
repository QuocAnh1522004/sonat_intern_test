using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel
{
    private List<TubeModel> _tubes;
    public GameModel(List<TubeModel> tubes)
    {
        _tubes = tubes;
    }
    public int TryPour(int fromIndex, int toIndex)
    {
        return _tubes[fromIndex]
            .CalculatePourAmount(_tubes[toIndex]);
    }

    public void ApplyPour(int fromIndex, int toIndex, int amount)
    {
        _tubes[fromIndex]
            .ApplyPourTo(_tubes[toIndex], amount);
    }

    public bool CheckWin()
    {
        return _tubes.All(t => t.IsComplete());
    }

}
