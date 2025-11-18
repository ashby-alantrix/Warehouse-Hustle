
using Newtonsoft.Json;
using System.Collections.Generic;

[System.Serializable]
public class GridValues
{
    public int row;
    public int col;
}

[System.Serializable]
public class NodeInfo
{
    public GridValues gridValues;

    public List<GridValues> blockedGridValues;

    //[JsonIgnore]
    //public List<string> blockedGridValueStr;
}

[System.Serializable]
public class GridData
{
    public NodeInfo[] nodeInfos;
}
