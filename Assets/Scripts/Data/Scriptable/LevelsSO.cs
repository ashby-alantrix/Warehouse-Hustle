using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSO", menuName = "LevelsSO")]
public class LevelsSO : BaseSO
{
    [SerializeField] TextAsset[] jsonFiles;
    
    private Dictionary<int, string> levelGridJsonDatasDict = new Dictionary<int, string>();

    public override void InitData()
    {
        if (levelGridJsonDatasDict.Count > 0) return;

        int totalFiles = jsonFiles.Length;

        for (int indexI = 1; indexI <= totalFiles; indexI++)
        {
            Debug.Log($"File for level {indexI}: {jsonFiles[indexI - 1]}");
            levelGridJsonDatasDict.Add(indexI, jsonFiles[indexI - 1].text);
        }
    }

    public string GetLevelJson(int level)
    {
        return levelGridJsonDatasDict.ContainsKey(level) ? levelGridJsonDatasDict[level] : "";
    }
}