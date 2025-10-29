
using System.Collections.Generic;

public class GridValues
{
    public float row;
    public float col;
}

public class NodeInfo
{
    public GridValues gridValues;

    public GridValues[] blockedGridValues;
}

public class GridData
{
    public NodeInfo[] nodeInfos;
}
