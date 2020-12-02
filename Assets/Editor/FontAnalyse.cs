using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FontAnalyse : Editor
{
    [MenuItem("Font/AnalyseFont")]
    public static void Analyse()
    {
        string excelPath = Application.dataPath +"/Editor/"+ "/Excel/" + "Font.xlsx";
        string jsonSavePath = Application.dataPath + "/Resources/Json/Font.json";
  
        Dictionary<string, Dictionary<string, string>> fonts = FontAnalyse.AnalyseExcel(excelPath);
        string dataJson = LitJson.JsonMapper.ToJson(fonts);

        if (!File.Exists(jsonSavePath))
        {
            File.Create(jsonSavePath);
        }
        File.WriteAllText(jsonSavePath, dataJson, Encoding.UTF8);
        Debug.Log("successful");
    }
    static Dictionary<string, Dictionary<string, string>> AnalyseExcel(string excelPath)
    {
        Dictionary<string, Dictionary<string, string>> font = new Dictionary<string, Dictionary<string, string>>();
        DataSet dataSet = ExcelRead.Read(excelPath);
        if(dataSet == null)
        {
            return null;
        }
        for (int i = 0; i < dataSet.Tables.Count; ++i)
        {
            DataTable tabel = dataSet.Tables[i];
            if (tabel == null)
            {
                continue;
            }
            string[] languageKey = new string[tabel.Columns.Count];
            for(int j = 1;j < tabel.Columns.Count; ++j)
            {
                object data = ExcelRead.ReadDataByRowCol(tabel, 0, j);
                if (data != null && data.ToString() != "")
                {
                    string key = data.ToString();
                    languageKey[j] = key;
                    if (!font.ContainsKey(key))
                    {
                        font[key] = new Dictionary<string, string>();
                    }
                } 
            }
            for(int j = 1; j < tabel.Rows.Count; ++j)
            {
                object idObject = ExcelRead.ReadDataByRowCol(tabel, j, 0);
                if(idObject == null || idObject.ToString() == "")
                {
                    continue;
                }
                string id = idObject.ToString();
                for (int m = 1; m < tabel.Columns.Count; ++m)
                {
                    object data = ExcelRead.ReadDataByRowCol(tabel, j, m);
                    if(data != null && data.ToString() != "")
                    {
                        font[languageKey[m]][id] = data.ToString();
                    }
                }
            }  
        }
        return font;
    }
}
