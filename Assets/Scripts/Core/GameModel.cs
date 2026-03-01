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
    public bool TryPour(int fromIndex, int toIndex)
    {
        if(fromIndex == toIndex) return false;
        TubeModel from = _tubes[fromIndex];
        TubeModel to = _tubes[toIndex];
        if(!from.CanPourTo(to)) return false;
        from.PourTo(to);
        return true;
    }

    public bool CheckWin()
    {
        return _tubes.All(t => t.IsComplete());
    }

}
