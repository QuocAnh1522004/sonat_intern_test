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
        return _tubes[fromIndex].TryPourInto(_tubes[toIndex]);
    }

    public bool CheckWin()
    {
        return _tubes.All(t => t.IsComplete());
    }

}
