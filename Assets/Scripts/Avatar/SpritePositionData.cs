using System.Collections.Generic;

public class SpritePositionData
{
    public Dictionary<string, DirectionData> directions;
}

public class DirectionData
{
    public float[] x;
    public float[] y;
}