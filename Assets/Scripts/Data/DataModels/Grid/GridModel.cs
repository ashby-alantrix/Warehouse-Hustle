
[System.Serializable]
public class GridValues
{
    public float row;
    public float col;
}

[System.Serializable]
public class NodeInfo
{
    public GridValues gridValues;

    public GridValues[] blockedGridValues;
}

[System.Serializable]
public class GridData
{
    public NodeInfo[] nodeInfos;
}
