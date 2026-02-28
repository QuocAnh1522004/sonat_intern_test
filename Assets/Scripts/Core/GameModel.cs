using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel
{
    public List<TubeModel> Tubes { get; private set; }
    public GameModel(List<TubeModel> tubes)
    {
        Tubes = tubes;
    }
    public bool TryPour(int fromIndex, int toIndex)
    {
        if(fromIndex == toIndex) return false;
        TubeModel from = Tubes[fromIndex];
        TubeModel to = Tubes[toIndex];
        if(!from.CanPourTo(to)) return false;
        from.PourTo(to);
        return true;
    }

    public bool CheckWin()
    {
        return Tubes.All(t => t.IsComplete());
    }

}
