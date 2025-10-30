public enum HEX_SIDE
{
    Top_Right = 0,
    Middle_Right = 1,
    Bottom_Right = 2,
    Bottom_Left = 3,
    Middle_Left = 4,
    Top_Left = 5
}

[System.Serializable]
public class OffsetData
{
    public float x;
    public float z;
}

[System.Serializable]
public class HexData
{
    public HEX_SIDE hexSide;
    public OffsetData offset;
}