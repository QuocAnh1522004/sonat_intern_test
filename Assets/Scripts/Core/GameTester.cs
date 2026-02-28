using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameTester : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TubeModel t1 = new TubeModel(3);
        t1.AddLayer(ColorType.Red);
        t1.AddLayer(ColorType.Yellow);
        t1.AddLayer(ColorType.Blue);

        TubeModel t2 = new TubeModel(4);
        List<TubeModel> tubes = new List<TubeModel>
        {
            t1, t2
        };
        GameModel game = new GameModel(tubes);
        game.TryPour(0, 1);
        Debug.Log("After Pour:");
        foreach (var tube in game.Tubes)
        {
            Debug.Log(string.Join(", ", tube.GetLayers()));
        }
    }   
}
