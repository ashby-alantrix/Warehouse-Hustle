using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : StoreDataBase
{
    [SerializeField] private TextAsset levelsJson;

    public override string GetLevelsJson()
    {
        return levelsJson.text;
    }
}
