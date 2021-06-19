using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class JsonParser
{
    private string FileName;
    private string FilePath;

    public List<IterationManager.MatchData> MatchDatas = new List<IterationManager.MatchData>();

    public JsonParser(string name)
    {
        FileName = name + "_" + DateTime.Now + ".json";
        FilePath = Application.dataPath + "../JSONS/" + FileName;
    }


    public void InitFile()
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(MatchDatas));
    }
}
