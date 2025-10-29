
using System;

public class GridValues
{
    public float row;
    public float col;
}

public class NodeInfo
{
    public GridValues gridValues; 

    public GridValues[] blockedGridValues =
    {
        new GridValues { row = 1, col = 2,},
        new GridValues { row = 1, col = 3,}
    };
}

public class GridData
{
    public NodeInfo[] nodeInfos;
}
