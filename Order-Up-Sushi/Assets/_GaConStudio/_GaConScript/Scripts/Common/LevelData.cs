using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int levelId;
    public int foodNumber;
    public float coinReward;
    public List<PlateData> plates;
}

[Serializable]
public class PlateData
{
    public int plateId;
    public List<PlateStateData> states;
}

[Serializable]
public class PlateStateData
{
    public List<int> foods; // ID của sushi
}