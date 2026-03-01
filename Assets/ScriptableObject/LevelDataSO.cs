
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelDataSO : ScriptableObject
{
    public List<TubeLevelDataSO> tubeLevelDataSO;
}
